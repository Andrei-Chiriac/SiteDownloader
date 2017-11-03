using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SiteDownloader
{
    public class WebHelper
    {

        public static HtmlDocument GetDOM(string link)
        {
            HtmlDocument document = new HtmlDocument();
            document.Load(GetWebResourceAsStream(link));
            return document;
        }

        public static List<HtmlNode> GetHtmlNodes(string link)
        {
            return GetDOM(link)
                        .DocumentNode
                        .Descendants()
                        .ToList();
        }

        public static Stream GetWebResourceAsStream(string link)
        {
            WebRequest request = WebRequest.Create(link);
            WebResponse response = request.GetResponse();

            return response.GetResponseStream();
        }

        public static Stream GetWebResourceAsStream(string siteRoot, string relativeLink)
        {
            siteRoot = siteRoot.TrimEnd('/');

            while (relativeLink.StartsWith("../"))
            {
                relativeLink = relativeLink.Substring(3, relativeLink.Length - 3);
            }

            return GetWebResourceAsStream(siteRoot + "/" + relativeLink);

        }

        public static string GetHTML(string link)
        {
            Stream stream = GetWebResourceAsStream(link);
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();
            reader.Close();
            return text;
        }

        public static WebPage GetWebPage(string link)
        {
            return new WebPage(link, GetHTML(link));
        }
    }
}
