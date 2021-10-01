using System.Collections.Generic;

namespace Core.Utilities.Helpers.FileHelper
{
    public class FileCategory
    {
        public string FolderName { get; set; }
        public Dictionary<string, string> ExtensionMimeType { get; set; }
        public double MaximumUploadSizeInByte { get; set; }
    }
}
