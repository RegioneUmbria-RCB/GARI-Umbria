using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AgronicaCoreModelsSTD.Utility
{
    public class FileWrapperAlt
    {
        public FileWrapperAlt() { }
        public FileWrapperAlt(string fileName, string fileBytes)
        {
            FileName = fileName;
            Base64FileStr = fileBytes;
        }

        public string FileName { get; set; }
        public string Base64FileStr { get; set; }
    }
}
