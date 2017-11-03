using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteDownloader
{
    public class IODownloadContext
    {
        string _directory;
        StreamWriter _errorWriter;
        StreamWriter _messageWriter;

        object _errorLock = new object();
        object _messageLock = new object();

        public IODownloadContext(string downloadDirectory, Stream errorStream, Stream messgeStream)
        {
            _directory = downloadDirectory;

            _errorWriter = new StreamWriter(errorStream);
            _messageWriter = new StreamWriter(messgeStream);
        }

        public void WriteError(string error)
        {
            lock (_errorLock)
            {
                _errorWriter.WriteLine(error);
            }
        }

        public void WriteMessage(string message)
        {
            lock (_messageLock)
            {
                _messageWriter.WriteLine(message);
            }
        }

        public void Close()
        {
            _errorWriter.Close();
            _messageWriter.Close();
        }

        public void StreamToFile(Stream stream, string relativePath)
        {
            string fullPath = _directory + relativePath;

            if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            }

            using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }
        }

        public void TextToFile(string text, string relativePath)
        {
            string fullPath = _directory + relativePath;

            if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            }

            StreamWriter sw = new StreamWriter(fullPath);
            sw.Write(text);
            sw.Close();
        }

    }
}
