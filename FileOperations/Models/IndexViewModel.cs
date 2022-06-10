using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileOperations.Models
{
    public class IndexViewModel
    {
        public SQLParameters parameters { get; set; }
        public List<FileFolderModel> fileFolderModel { get; set; }
        public IndexViewModel(SQLParameters _parameters, List<FileFolderModel> _fileFolderModel)
        {
            parameters = _parameters;
            fileFolderModel = _fileFolderModel;
        }
    }
}
