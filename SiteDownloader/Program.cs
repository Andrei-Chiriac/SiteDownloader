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
        static void Main(string[] args)
        {
            var site = new Site()
            {
                RootLink = "https://adminlte.io/themes/AdminLTE/",
                MenuIdentifier = "sidebar-menu"
            };
            

            IODownloadContext IOContext = new IODownloadContext(site.Domain, new FileStream("errors.txt", FileMode.Create), Console.OpenStandardOutput());
            new Downloader(site, IOContext).Download();
            IOContext.Close();

            Console.WriteLine("\n\n\n\n");
            Console.ReadKey();
        }
    }
}
