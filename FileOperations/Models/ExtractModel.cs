using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FileOperations.Models
{
    public class ExtractModel
    {
        public static List<FileFolderModel> GetAll(string output)
        {
            List<FileFolderModel> model = new List<FileFolderModel>();

            string[] satirlar = output.Split("\r\n");
            string newPath = "C:\\";

            foreach (string item in satirlar)
            {
                if (item.Contains("Directory of "))
                {
                    newPath = item.Trim();
                    newPath = newPath.Substring(newPath.IndexOf("Directory of ") + 13).Trim() + "\\";
                }
                if (item.Length > 0)
                {
                    if (item.StartsWith('0') || item.StartsWith('1') || item.StartsWith('2') || item.StartsWith('3'))
                    {
                        FileFolderModel tm = new FileFolderModel();
                        tm.DateModified = Convert.ToDateTime(item.Substring(0, 17).Replace("  ", " ").Replace('.', '/'));

                        if (item.Contains("<DIR>"))
                        {
                            tm.IsFolder = true;
                            tm.Name = item.Remove(0, item.IndexOf("<DIR>") + 5);
                            tm.Name = tm.Name.Trim();
                            tm.StartLink = newPath + tm.Name + "\\";
                            if (tm.Name != "." && tm.Name != "..")
                            {
                                model.Add(tm);
                            }
                        }
                        else
                        {
                            string kesit = item.Remove(0, 17).Trim();
                            if (!kesit.Contains("<SYMLINK>"))
                            {
                                try
                                {
                                    tm.Size = Convert.ToInt32(kesit.Substring(0, kesit.IndexOf(' ')).Replace(".", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty));
                                }
                                catch
                                {

                                }
                                
                                tm.Name = kesit.Remove(0, kesit.IndexOf(' ')).Trim();
                                tm.StartLink = newPath + tm.Name;
                                ProcessStartInfo psi = new ProcessStartInfo("cmd");
                                psi.UseShellExecute = false;
                                psi.RedirectStandardOutput = true;
                                psi.CreateNoWindow = false;
                                psi.RedirectStandardInput = true;
                                var proc = Process.Start(psi);
                                proc.StandardInput.WriteLine("icacls \"" + tm.StartLink + "\"");
                                proc.StandardInput.WriteLine("exit");
                                string cmdOutput = proc.StandardOutput.ReadToEnd();
                                
                                tm.Permissions = GetPermissions(cmdOutput);
                                if(tm.Name.LastIndexOf('.') == -1)
                                {
                                    tm.Type = "null";
                                }
                                else
                                {
                                    tm.Type = tm.Name.Substring(tm.Name.LastIndexOf('.'));
                                }
                                
                                model.Add(tm);
                            }

                        }
                    }
                }
            }


            return model;
        }


        public static string GetPermissions(string cmdOutput)
        {
            string permission = "";
            int startIndex = cmdOutput.IndexOf("SYSTEM:") + 7;
            int endLine = cmdOutput.IndexOf("\\", startIndex);
            string kesit = cmdOutput.Substring(startIndex, endLine - startIndex);
            if(kesit.Contains("DENY"))
            {
                if(kesit.Contains("(M)"))
                {
                    permission = "W";
                }
                else if(kesit.Contains("(W)"))
                {
                    permission = "R";
                }
            }
            else
            {
                permission = "X";
            }
            return permission;
        }
    }
}
