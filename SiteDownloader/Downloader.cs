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

        private IODownloadContext _IOContext;
        public Site _site;

        public Downloader(Site site, IODownloadContext IOContext)
        {
            _IOContext = IOContext;
            _site = site;
        }

        public void Download()
        {
            //menu
            FindMainMenu();

            //pages
            ExtractPageLinksFromMenu();
            DownloadPages();
            SavePagesToDisk();

            //resources
            ExtractResourceLinksFromPages();
            DownloadAndSaveResourcesToDisk();
        }

        private void FindMainMenu()
        {
            _site.MenuElement = WebHelper.GetHtmlNodes(_site.RootLink)
                                         .FirstOrDefault(x => x.Id == _site.MenuIdentifier ||
                                                        (x.Attributes.Contains("class") && 
                                                         x.Attributes["class"].Value.Contains(_site.MenuIdentifier))) ?? 
                                                         throw new Exception("The menu was not found");
        }
        
        private void ExtractPageLinksFromMenu()
        {
            _site.PageLinks = _site
                                .MenuElement
                                .Descendants()
                                .Where(node => node.Name == "a")
                                .Select(node => node.GetAttributeValue("href", string.Empty))
                                .Where(link => link != string.Empty && link != "#")
                                .Distinct()
                                .ToList();
        }


        private void DownloadPages()
        {
            Parallel.ForEach(_site.PageLinks.OrderBy(x => x), link =>
            {
                try
                {
                    _site.WebPages.Add(WebHelper.GetWebPage(_site.RootLink + link));
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
            Parallel.ForEach(_site.WebPages, pagina =>
            {
                try
                {
                    _IOContext.TextToFile(pagina.HTML, @"\" + pagina.Link.Replace(_site.RootLink, string.Empty));
                }
                catch (Exception ex)
                {
                    _IOContext.WriteError("Error saving page " + pagina.Link + " - " + ex.Message);
                }
            });
        }

        private void ExtractResourceLinksFromPages()
        {
            Parallel.ForEach(_site.WebPages, page => _site.ResourceLinks.AddRange(page.GetResourceLinks()));
            _site.ResourceLinks = _site.ResourceLinks.Distinct().ToList();
        }

        private void DownloadAndSaveResourcesToDisk()
        {
            Parallel.ForEach(_site.ResourceLinks.OrderBy(x => x), relativeLink =>
            {
                try
                {
                    _IOContext.StreamToFile(WebHelper.GetWebResourceAsStream(_site.RootLink, relativeLink), @"\" + relativeLink.Replace("../", "").Replace("/", "\\"));
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
