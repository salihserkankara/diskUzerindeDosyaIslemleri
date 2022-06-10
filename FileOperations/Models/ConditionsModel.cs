using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileOperations.Models
{
    public class ConditionsModel
    {
        public static bool IsValid(SQLParameters parameters, FileFolderModel file)
        {
            if (!string.IsNullOrEmpty(parameters.FileType))
            {
                if(!parameters.FileType.ToLower().Equals(file.Type.ToLower()))
                {
                    return false;
                }
            }

            if(DateTime.Compare(parameters.ModifiedDate, new DateTime(2000, 1, 1)) > 0)
            {
                if(DateTime.Compare(new DateTime(file.DateModified.Year, file.DateModified.Month, file.DateModified.Day), parameters.ModifiedDate) != 0)
                {
                    return false;
                }
            }

            if(!string.IsNullOrEmpty(parameters.Permission))
            {
                if (!parameters.Permission.ToUpper().Equals(file.Permissions))
                {
                    return false;
                }
            }

            if(!string.IsNullOrEmpty(parameters.User))
            {
                if(!file.StartLink.ToLower().Contains(parameters.User.ToLower()))
                {
                    return false;
                }
            }

            if(parameters.Size > 0)
            {
                int fileSize = parameters.Size;
                if(fileSize <1024) //byte
                {
                    if (fileSize != file.Size)
                    {
                        return false;
                    }
                }
                else if(fileSize >= 1024 && fileSize < 1022976) //K
                {
                    if((int)(fileSize / 1024) != (int)(file.Size / 1024))
                    {
                        return false;
                    }
                }
                else if(fileSize >= 1022976 && fileSize < 1048576) //M
                {
                    if((int)(fileSize / 1048576) != (int)(file.Size / 1048576))
                    {
                        return false;
                    }
                }
                else if(fileSize >= 1048576) //G
                {
                    if ((int)(fileSize / 1073741824) != (int)(file.Size / 1073741824))
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }
    }
}
