using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using FileOperations.Models;
using Microsoft.AspNetCore.SignalR;
using FileOperations.Hubs;
using Microsoft.AspNetCore.Hosting;


namespace FileOperations.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private IHostingEnvironment Environment;
        private readonly IHubContext<InfoHub> hubContext;

        List<FileFolderModel> filesR;
        SQLParameters parameters;
        public HomeController(IHubContext<InfoHub> _hubContext, IHostingEnvironment _environment)
        {
            filesR = new List<FileFolderModel>();
            hubContext = _hubContext;
            Environment = _environment;
        }

        public JsonResult ChangeFolder()
        {
            string yol = Environment.ContentRootPath;
            ProcessStartInfo psi = new ProcessStartInfo("cmd");
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow = false;
            psi.RedirectStandardInput = true;
            var proc = Process.Start(psi);
            proc.StandardInput.WriteLine("cd \"" + yol + "\"");
            proc.StandardInput.WriteLine("start FolderDialogApp.exe");
            proc.StandardInput.WriteLine("exit");
            return Json(new { sonuc = yol });
        }

        public JsonResult UpdateFolder()
        {
            string folderPath;
            try
            {
                using (var sr = new StreamReader("folderpath.yzgl"))
                {
                    folderPath = sr.ReadToEnd();
                }
            }
            catch
            {
                folderPath = "C:\\";
            }
            return Json(new { path = folderPath });
        }
        public async Task<IActionResult> Execute(string txt, string folderPath)
        {
            
            txt = txt.ToLower();
            if (folderPath.Length>2)
            {
                parameters = new SQLParameters();
                parameters.Directory = folderPath;
                parameters.IslemTipi = "select";
                parameters.Name = txt;
            }
            else
            {
                ExtractSQL sqlExe = new ExtractSQL();
                await sqlExe.Extract(txt);
                parameters = sqlExe.model;
            }
            
            if(!string.IsNullOrEmpty(parameters.Hata))
            {
                return PartialView("_Hata", parameters.Hata);
            }

            if(parameters.IslemTipi.Equals("select"))
            {
                ProcessStartInfo psi = new ProcessStartInfo("cmd");
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.CreateNoWindow = false;
                psi.RedirectStandardInput = true;
                var proc = Process.Start(psi);
                proc.StandardInput.WriteLine("cd \"" + parameters.Directory + "\"");
                proc.StandardInput.WriteLine("dir");
                proc.StandardInput.WriteLine("exit");
                string sa = proc.StandardOutput.ReadToEnd();

                List<FileFolderModel> files = ExtractModel.GetAll(sa);
                if (files.Where(x => x.Name.ToLower().Contains(parameters.Name.ToLower())).ToList().Count > 0)
                {
                    List<FileFolderModel> dosyalar = files.Where(x => x.Name.ToLower().Contains(parameters.Name.ToLower())).ToList();
                    foreach (var mdl in dosyalar)
                    {
                        if (ConditionsModel.IsValid(parameters, mdl) && !mdl.IsFolder)
                        {
                            filesR.Add(mdl);
                            await hubContext.Clients.All.SendAsync("buldu", mdl.StartLink);
                        }
                    }
                }
                foreach (FileFolderModel folder in files.Where(x => x.IsFolder))
                {
                    await Searching(folder.StartLink, parameters.Name);
                }

                return PartialView("_Dosyalar", new IndexViewModel(parameters, filesR));
            }
            else if(parameters.IslemTipi.Equals("delete"))
            {
                ProcessStartInfo psi = new ProcessStartInfo("cmd");
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.CreateNoWindow = false;
                psi.RedirectStandardInput = true;
                var proc = Process.Start(psi);
                proc.StandardInput.WriteLine("cd \"" + parameters.Directory + "\"");
                proc.StandardInput.WriteLine("dir");
                proc.StandardInput.WriteLine("exit");
                string sa = proc.StandardOutput.ReadToEnd();

                List<FileFolderModel> files = ExtractModel.GetAll(sa);
                if (files.Where(x => x.Name.ToLower().Contains(parameters.Name.ToLower())).ToList().Count > 0)
                {
                    List<FileFolderModel> dosyalar = files.Where(x => x.Name.ToLower().Contains(parameters.Name.ToLower())).ToList();
                    foreach (var mdl in dosyalar)
                    {
                        if (ConditionsModel.IsValid(parameters, mdl) && !mdl.IsFolder)
                        {
                            filesR.Add(mdl);
                            await hubContext.Clients.All.SendAsync("buldu", mdl.StartLink);
                        }
                    }
                }
                foreach (FileFolderModel folder in files.Where(x => x.IsFolder))
                {
                    await Searching(folder.StartLink, parameters.Name);
                }

                foreach (FileFolderModel deleteFile in filesR)
                {
                    ProcessStartInfo cmdTask = new ProcessStartInfo("cmd");
                    cmdTask.UseShellExecute = false;
                    cmdTask.RedirectStandardOutput = true;
                    cmdTask.CreateNoWindow = false;
                    cmdTask.RedirectStandardInput = true;
                    var proccess = Process.Start(cmdTask);
                    proccess.StandardInput.WriteLine("del \"" + deleteFile.StartLink + "\"");
                    proccess.StandardInput.WriteLine("exit");
                }
                return PartialView("_Silindi", filesR);
            }
            else
            {
                ProcessStartInfo psi = new ProcessStartInfo("cmd");
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.CreateNoWindow = false;
                psi.RedirectStandardInput = true;
                var proc = Process.Start(psi);
                string path = parameters.Directory;
                if(path.Substring(path.Length - 1) != "\\")
                {
                    path += "\\";
                }
                proc.StandardInput.WriteLine("copy NUL \""+ path + parameters.Name +"\"");
                proc.StandardInput.WriteLine("exit");
                return PartialView("_Basarili", path+parameters.Name);
            }
            
        }

        private async Task Searching(string yol, string txtt)
        {
            await hubContext.Clients.All.SendAsync("dinle", yol);
            ProcessStartInfo psi = new ProcessStartInfo("cmd");
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow = false;
            psi.RedirectStandardInput = true;
            var proc = Process.Start(psi);
            proc.StandardInput.WriteLine("cd \"" + yol + "\"");
            proc.StandardInput.WriteLine("dir");
            proc.StandardInput.WriteLine("exit");
            string sa = proc.StandardOutput.ReadToEnd();
            
            List<FileFolderModel> files = ExtractModel.GetAll(sa);
            if(files.Where(x => x.Name.ToLower().Contains(txtt.ToLower())).ToList().Count > 0)
            {
                List<FileFolderModel> dosyalar = files.Where(x => x.Name.ToLower().Contains(txtt.ToLower())).ToList();
                foreach (var mdl in dosyalar)
                {
                    if (ConditionsModel.IsValid(parameters, mdl))
                    {
                        filesR.Add(mdl);
                        await hubContext.Clients.All.SendAsync("buldu", mdl.StartLink);
                    }
                }
            }

            foreach (var folder in files.Where(x=> x.IsFolder))
            {
                await Searching(folder.StartLink, txtt);
            }
        }

        public JsonResult StartFile(string startLink)
        {
            ProcessStartInfo psi = new ProcessStartInfo("cmd");
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow = false;
            psi.RedirectStandardInput = true;
            var proc = Process.Start(psi);
            proc.StandardInput.WriteLine("start \"\" \"" + startLink + "\"");
            proc.StandardInput.WriteLine("exit");
            return Json(new { sonuc = 1 });
        }

        public JsonResult StartDirectory(string startLink)
        {
            string path = startLink.Substring(0, startLink.LastIndexOf("\\"));
            string app = startLink.Substring(startLink.LastIndexOf("\\") + 1);
            ProcessStartInfo psi = new ProcessStartInfo("cmd");
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.CreateNoWindow = false;
            psi.RedirectStandardInput = true;
            var proc = Process.Start(psi);
            proc.StandardInput.WriteLine("start \"\" \"" + path + "\"");
            proc.StandardInput.WriteLine("exit");
            return Json(new { sonuc = 1 });
        }
    }
}
