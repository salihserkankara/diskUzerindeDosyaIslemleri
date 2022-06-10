using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileOperations.Models
{
    public class FileFolderModel
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public DateTime DateModified { get; set; }
        public string Permissions { get; set; }
        public string Type { get; set; }
        public string StartLink { get; set; }
        public bool IsFolder { get; set; }
    }
}
