using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteDownloader
{
    public class Site
    {
        public Site()
        {
            WebPages = new List<WebPage>();
            PageLinks = new List<string>();
            ResourceLinks = new List<string>();
            RootPageElements = new List<HtmlNode>();
        }

        public string Domain => new Uri(RootLink).Host;

        public string RootLink { get; set; }

        public List<HtmlNode> RootPageElements { get; set; }


        public HtmlNode MenuElement { get; set; }

        public string MenuIdentifier { get; set; }


        public List<WebPage> WebPages { get; set; }

        public List<string> PageLinks { get; set; }

        public List<string> ResourceLinks { get; set; }
        
    }
}
