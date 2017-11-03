using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteDownloader
{
    class Program
    {
        static string _rootLink = "https://adminlte.io/themes/AdminLTE/";
        static string _menuIdOrClass = "sidebar-menu";

        static void Main(string[] args)
        {
            string domain = new Uri(_rootLink).Host;
            IODownloadContext IOContext = new IODownloadContext(domain, new FileStream("errors.txt", FileMode.Create), Console.OpenStandardOutput());
            new Downloader(_rootLink, _menuIdOrClass, IOContext).Download();
            IOContext.Close();

            Console.WriteLine("\n\n\n\n");
            Console.ReadKey();

        }
    }
}
