using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class UploadFileResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
    }
}
