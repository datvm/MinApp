using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MinApp.Actions
{

    public class FileActionResult : StatusCodeActionResult
    {

        private const int BufferSize = 1024 * 1024; // 1MB

        private static IDictionary<string, string> ExtensionMimeMapping = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            #region Map List

            { ".aac", "audio/aac" },
            { ".abw", "application/x-abiword" },
            { ".arc", "application/octet-stream" },
            { ".avi", "video/x-msvideo" },
            { ".azw", "application/vnd.amazon.ebook" },
            { ".bin", "application/octet-stream" },
            { ".bmp", "image/bmp" },
            { ".bz", "application/x-bzip" },
            { ".bz2", "application/x-bzip2" },
            { ".csh", "application/x-csh" },
            { ".css", "text/css" },
            { ".csv", "text/csv" },
            { ".doc", "application/msword" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".eot", "application/vnd.ms-fontobject" },
            { ".epub", "application/epub+zip" },
            { ".es", "application/ecmascript" },
            { ".gif", "image/gif" },
            { ".htm", "text/html" },
            { ".html", "text/html" },
            { ".ico", "image/x-icon" },
            { ".ics", "text/calendar" },
            { ".jar", "application/java-archive" },
            { ".jpeg", "image/jpeg" },
            { ".jpg", "image/jpeg" },
            { ".js", "application/javascript" },
            { ".json", "application/json" },
            { ".mid", "audio/midi audio/x-midi" },
            { ".midi", "audio/midi audio/x-midi" },
            { ".mpeg", "video/mpeg" },
            { ".mpkg", "application/vnd.apple.installer+xml" },
            { ".odp", "application/vnd.oasis.opendocument.presentation" },
            { ".ods", "application/vnd.oasis.opendocument.spreadsheet" },
            { ".odt", "application/vnd.oasis.opendocument.text" },
            { ".oga", "audio/ogg" },
            { ".ogv", "video/ogg" },
            { ".ogx", "application/ogg" },
            { ".otf", "font/otf" },
            { ".png", "image/png" },
            { ".pdf", "application/pdf" },
            { ".ppt", "application/vnd.ms-powerpoint" },
            { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            { ".rar", "application/x-rar-compressed" },
            { ".rtf", "application/rtf" },
            { ".sh", "application/x-sh" },
            { ".svg", "image/svg+xml" },
            { ".swf", "application/x-shockwave-flash" },
            { ".tar", "application/x-tar" },
            { ".tif", "image/tiff" },
            { ".tiff", "image/tiff" },
            { ".ts", "application/typescript" },
            { ".ttf", "font/ttf" },
            { ".vsd", "application/vnd.visio" },
            { ".wav", "audio/wav" },
            { ".weba", "audio/webm" },
            { ".webm", "video/webm" },
            { ".webp", "image/webp" },
            { ".woff", "font/woff" },
            { ".woff2", "font/woff2" },
            { ".xhtml", "application/xhtml+xml" },
            { ".xls", "application/vnd.ms-excel" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ".xml", "application/xml" },
            { ".xul", "application/vnd.mozilla.xul+xml" },
            { ".zip", "application/zip" },
            { ".3gp", "video/3gpp" },
            { ".3g2", "video/3gpp2" },
            { ".7z", "application/x-7z-compressed" },

            #endregion
        };

        private const string DefaultContentType = "application/octet-stream";

        public string FilePath { get; set; }
        public byte[] FileData { get; set; }
        public Stream DataStream { get; set; }
        public string ContentType { get; set; } 

        public FileActionResult(string filePath, string contentType = null)
        {
            this.FilePath = filePath;
            this.ContentType = contentType;
        }

        public FileActionResult(byte[] fileData, string contentType)
        {
            this.FileData = fileData;
            this.ContentType = contentType;
        }

        public FileActionResult(Stream dataStream, string contentType)
        {
            this.DataStream = dataStream;
            this.ContentType = contentType;
        }

        public override void WriteResponse(HttpListenerContext context)
        {
            if (!string.IsNullOrEmpty(this.FilePath))
            {
                if (this.ContentType != null)
                {
                    this.ContentType = GuessContentType(this.FilePath);
                }

                using (var fileStream = new FileStream(this.FilePath, FileMode.Open))
                {
                    this.WriteStream(context, fileStream);
                }
            }
            else if (this.FileData != null)
            {
                this.WriteBytes(context);
            }
            else if (this.DataStream != null)
            {
                this.WriteStream(context, this.DataStream);
            }

            this.SetContentType(context);
            base.WriteResponse(context);
        }

        protected virtual void SetContentType(HttpListenerContext context)
        {
            context.Response.ContentType = this.ContentType ?? DefaultContentType;
        }

        protected virtual void WriteStream(HttpListenerContext context, Stream stream)
        {
            context.Response.ContentLength64 = stream.Length;
            stream.CopyTo(context.Response.OutputStream);
            context.Response.OutputStream.Close();
        }

        protected virtual void WriteBytes(HttpListenerContext context)
        {
            context.Response.ContentLength64 = this.FileData.Length;
            context.Response.OutputStream.Write(this.FileData, 0, this.FileData.Length);
            context.Response.OutputStream.Close();
        }

        private static string GuessContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath);

            if (!ExtensionMimeMapping.TryGetValue(extension, out var result))
            {
                result = DefaultContentType;
            }

            return result;
        }

    }

}
