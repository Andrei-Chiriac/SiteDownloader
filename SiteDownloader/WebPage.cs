using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace SiteDownloader
{
    public class WebPage
    {
        public WebPage(string link, string html)
        {
            Link = link;
            HTML = html;
        }

        public string HTML { get; private set; }

        public string Link { get; private set; }

        HtmlDocument _DOM;
        public HtmlDocument DOM
        {
            get
            {
                if (_DOM == null)
                {
                    _DOM = new HtmlDocument();
                    _DOM.LoadHtml(HTML);
                }

                return _DOM;
            }
        }

        public List<string> GetResourceLinks()
        {
            var resources = new List<string>();

            resources.AddRange(GetStyleLinks());
            resources.AddRange(GetScriptLinks());
            resources.AddRange(GetImageLinks());
            
            return resources;
        }

        private List<string> GetStyleLinks()
        {
            return DOM
                    .DocumentNode
                    .Descendants()
                    .Where(x => x.Name == "head")
                    .First()
                    .Descendants()
                    .Where(x => x.Name == "link")
                    .Select(x => x.GetAttributeValue("href", string.Empty))
                    .Where(x => x != string.Empty && x != "#")
                    .ToList();
        }

        private List<string> GetScriptLinks()
        {
            return DOM
                     .DocumentNode
                     .Descendants()
                     .Where(x => x.Name == "script")
                     .Select(x => x.GetAttributeValue("src", ""))
                     .Where(x => x != string.Empty && x != "#")
                     .ToList();
        }

        private List<string> GetImageLinks()
        {
            return DOM
                    .DocumentNode
                    .Descendants()
                    .Where(x => x.Name == "img")
                    .Select(x => x.GetAttributeValue("src", ""))
                    .Where(x => x != string.Empty && x != "#")
                    .ToList();
        }
    }
}