using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileOperations.Models
{
    public class SQLParameters
    {
        public string IslemTipi { get; set; }
        public string Kosul { get; set; }
        public int Size { get; set; }
        public string Name { get; set; }
        public string Permission { get; set; }
        public string Hardlink { get; set; }
        public string User { get; set; }
        public string Group { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string FileType { get; set; }
        public string Directory { get; set; }
        public string Hata { get; set; }
    }
}
