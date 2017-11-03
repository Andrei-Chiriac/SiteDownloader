using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteDownloader
{
    public class Downloader
    {
        private string _siteRoot;
        private string _menuIdOrClass;
        private IODownloadContext _IOContext;
        private HtmlNode _menuElement;
        private List<HtmlNode> _elements;
        private List<string> _pageLinks;
        private List<WebPage> _pages = new List<WebPage>();
        private List<string> _resourceLinks = new List<string>();

        public Downloader(string siteRoot, string navElementId, IODownloadContext IOContext)
        {
            _siteRoot = siteRoot;
            _menuIdOrClass = navElementId;
            _IOContext = IOContext;
        }

        public void Download()
        {
            GetHtmlElementsForMainPage();
            FindMainMenu();

            ExtractPageLinksFromMenu();
            DownloadPages();
            SavePagesToDisk();

            ExtractResourceLinksFromPages();
            DownloadAndSaveResourcesToDisk();
            
        }

        private void GetHtmlElementsForMainPage() => _elements = WebHelper.GetHtmlNodes(_siteRoot);

        private void FindMainMenu() => _menuElement = GetMenuById() ?? GetMenuByClass() ?? throw new Exception("The menu was not found");

        private HtmlNode GetMenuById() => _elements.FirstOrDefault(x => x.Id == _menuIdOrClass);

        private HtmlNode GetMenuByClass() => _elements.FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Contains(_menuIdOrClass));

        private void ExtractPageLinksFromMenu()
        {
            _pageLinks = _menuElement
                                .Descendants()
                                .Where(node => node.Name == "a")
                                .Select(node => node.GetAttributeValue("href", string.Empty))
                                .Where(link => link != string.Empty && link != "#")
                                .Distinct()
                                .ToList();
        }


        private void DownloadPages()
        {
            Parallel.ForEach(_pageLinks.OrderBy(x => x), link =>
            {
                try
                {
                    _pages.Add(WebHelper.GetWebPage(_siteRoot + link));
                    _IOContext.WriteMessage("GOOD LINK " + link);
                }
                catch (Exception ex)
                {
                    _IOContext.WriteError("Error downloading page " + link + " - " + ex.Message);
                    _IOContext.WriteMessage("BAD LINK " + link);
                }
            });
        }


        private void SavePagesToDisk()
        {
            Parallel.ForEach(_pages, pagina =>
            {
                try
                {
                    _IOContext.TextToFile(pagina.HTML, @"\" + pagina.Link.Replace(_siteRoot,string.Empty));
                }
                catch (Exception ex)
                {
                    _IOContext.WriteError("Error saving page " + pagina.Link + " - " + ex.Message);
                }
            });
        }

        private void ExtractResourceLinksFromPages()
        {
            Parallel.ForEach(_pages, page => _resourceLinks.AddRange(page.GetResourceLinks()));
            _resourceLinks = _resourceLinks.Distinct().ToList();
        }

        private void DownloadAndSaveResourcesToDisk()
        {
            Parallel.ForEach(_resourceLinks.OrderBy(x => x), relativeLink =>
            {
                try
                {
                    _IOContext.StreamToFile(WebHelper.GetWebResourceAsStream(_siteRoot, relativeLink), @"\" + relativeLink.Replace("../", "").Replace("/", "\\"));
                    _IOContext.WriteMessage("GOOD LINK " + relativeLink);
                }
                catch (Exception ex)
                {
                    _IOContext.WriteError("Error downloading resource " + relativeLink + " - " + ex.Message);
                    _IOContext.WriteMessage("BAD LINK " + relativeLink);
                }
            });
        }

    }
}
