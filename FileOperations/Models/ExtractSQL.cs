using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileOperations.Models
{
    public class ExtractSQL
    {
        public SQLParameters model { get; set; } = null;
        public ExtractSQL()
        {
            
        }

        public async Task Extract(string sqlQuery)
        {
            model = new SQLParameters();
            string girdi = sqlQuery;
            if (girdi.IndexOf(" ") == 6)
            {
                string islemTipi = girdi.Substring(0, 6);
                if (islemTipi == "select" && girdi.IndexOf("from") - 1 == girdi.IndexOf(" ", 7))
                {
                    model.IslemTipi = islemTipi;
                    girdi = girdi.Replace(islemTipi + " ", null);
                    int ilkSelect_From = 0;
                    int sonSelect_From = girdi.IndexOf("from", ilkSelect_From + 1) - 1;
                    string selectKosul = girdi[ilkSelect_From..sonSelect_From];
                    if (selectKosul.Contains(" "))
                    {
                        model.Hata = "regular expressions hatası!";
                        return;
                    }
                    if (selectKosul.Contains(","))
                    {
                        string virgulSelectKosul = selectKosul;
                        while (virgulSelectKosul.Contains(","))
                        {
                            int virgulIndex = virgulSelectKosul.IndexOf(",", 0);
                            if (virgulIndex == 0)
                            {
                                virgulSelectKosul = virgulSelectKosul[1..];
                                virgulIndex = virgulSelectKosul.IndexOf(",", 0);
                            }
                            if (virgulIndex == -1)
                            {
                                virgulIndex = virgulSelectKosul.Length;
                            }
                            string selectVirgulKosul = virgulSelectKosul.Substring(0, virgulIndex);
                            model.Kosul += selectVirgulKosul;
                            virgulSelectKosul = virgulSelectKosul[virgulIndex..];
                        }
                    }
                    else
                    {
                        model.Kosul += selectKosul;
                    }
                    girdi = girdi.Replace(selectKosul + " ", null);

                    if (girdi.IndexOf(" ", 0) == 4 && girdi.IndexOf("where") - 1 == girdi.IndexOf(" ", 5))
                    {
                        if (girdi.Substring(0, 4) == "from")
                        {
                            girdi = girdi.Replace("from ", null);
                            int ilkFrom_Where = 0;
                            int sonFrom_Where = girdi.IndexOf("where", ilkFrom_Where + 1) - 1;
                            string fromKosul = girdi[ilkFrom_Where..sonFrom_Where];
                            if (fromKosul.Contains(" "))
                            {
                                model.Hata = "regular expressions hatası!";
                                return;
                            }
                            model.Directory = Directory(fromKosul);
                            girdi = girdi.Replace(fromKosul + " ", null);

                            if (girdi.Substring(0, 5) == "where" && girdi.IndexOf(" ") == 5)
                            {
                                girdi = girdi.Replace("where ", null);
                                int ilkWhere_ = 0;
                                int sonWhere_ = girdi.Length;
                                string whereKosul = girdi[ilkWhere_..sonWhere_];
                                if (whereKosul == "")
                                {
                                    model.Hata = "sql yapısına aykırı!";
                                    return;
                                }
                                else if (whereKosul.Contains(" "))
                                {
                                    model.Hata = "girilen şart regular expressionsa aykırı!";
                                    return;
                                }
                                else if (whereKosul.Contains(","))
                                {
                                    model.Hata = "şartlar && ile ayrılmalı!";
                                    return;
                                }
                                if (whereKosul.Contains("&&"))
                                {
                                    if (whereKosul.Contains(" "))
                                    {
                                        model.Hata = "girilen şart regular expressionsa aykırı!";
                                        return;
                                    }
                                    string ampersandWhereKosul = whereKosul;
                                    while (ampersandWhereKosul.Contains("&&"))
                                    {
                                        int ampersandIndex = ampersandWhereKosul.IndexOf("&&", 0);
                                        if (ampersandIndex == 0)
                                        {
                                            ampersandWhereKosul = ampersandWhereKosul[2..];
                                            ampersandIndex = ampersandWhereKosul.IndexOf("&&", 0);
                                        }
                                        if (ampersandIndex == -1)
                                        {
                                            ampersandIndex = ampersandWhereKosul.Length;
                                        }
                                        string whereAmpersandKosul = ampersandWhereKosul.Substring(0, ampersandIndex);
                                        ampersandWhereKosul = ampersandWhereKosul[ampersandIndex..];
                                        if (whereAmpersandKosul.Contains("name=") && model.Kosul.Contains("name"))
                                        {
                                            model.Name = WhereName(whereAmpersandKosul);
                                            continue;
                                        }
                                        else if (whereAmpersandKosul.Contains("size=") && model.Kosul.Contains("name"))
                                        {
                                            model.Size = WhereSize(whereAmpersandKosul);
                                            continue;
                                        }
                                        else if (whereAmpersandKosul.Contains("permission=") && model.Kosul.Contains("permission"))
                                        {
                                            model.Permission = WherePermission(whereAmpersandKosul);
                                            continue;
                                        }
                                        else if (whereAmpersandKosul.Contains("hardlink=") && model.Kosul.Contains("hardlink"))
                                        {
                                            model.Hardlink = WhereHardlink(whereAmpersandKosul);
                                            continue;
                                        }
                                        else if (whereAmpersandKosul.Contains("user=") && model.Kosul.Contains("user"))
                                        {
                                            model.User = WhereUser(whereAmpersandKosul);
                                            continue;
                                        }
                                        else if (whereAmpersandKosul.Contains("group=") && model.Kosul.Contains("group"))
                                        {
                                            model.Group = WhereGroup(whereAmpersandKosul);
                                            continue;
                                        }
                                        else if (whereAmpersandKosul.Contains("modifieddate=") && model.Kosul.Contains("modifieddate"))
                                        {
                                            model.ModifiedDate = WhereModifiedDate(whereAmpersandKosul);
                                            continue;
                                        }
                                        else if (whereAmpersandKosul.Contains("filetype=") && model.Kosul.Contains("filetype"))
                                        {
                                            model.FileType = WhereFileType(whereAmpersandKosul);
                                            continue;
                                        }
                                        else
                                        {
                                            model.Hata = "girilen bütün şartların ataması yapılmalı!";
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    if (whereKosul.Contains("name=") && model.Kosul.Contains("name"))
                                    {
                                        model.Name = WhereName(whereKosul);
                                        model.Kosul = model.Kosul.Replace("name", null);
                                    }
                                    else if (whereKosul.Contains("size=") && model.Kosul.Contains("size"))
                                    {
                                        model.Size = WhereSize(whereKosul);
                                        model.Kosul = model.Kosul.Replace("size", null);
                                    }
                                    else if (whereKosul.Contains("permission=") && model.Kosul.Contains("permission"))
                                    {
                                        model.Permission = WherePermission(whereKosul);
                                        model.Kosul = model.Kosul.Replace("permission", null);
                                    }
                                    else if (whereKosul.Contains("hardlink=") && model.Kosul.Contains("hardlink"))
                                    {
                                        model.Hardlink = WhereHardlink(whereKosul);
                                        model.Kosul = model.Kosul.Replace("hardlink", null);
                                    }
                                    else if (whereKosul.Contains("user=") && model.Kosul.Contains("user"))
                                    {
                                        model.User = WhereUser(whereKosul);
                                        model.Kosul = model.Kosul.Replace("user", null);
                                    }
                                    else if (whereKosul.Contains("group=") && model.Kosul.Contains("group"))
                                    {
                                        model.Group = WhereGroup(whereKosul);
                                        model.Kosul = model.Kosul.Replace("group", null);
                                    }
                                    else if (whereKosul.Contains("modifieddate=") && model.Kosul.Contains("modifieddate"))
                                    {
                                        model.ModifiedDate = WhereModifiedDate(whereKosul);
                                        model.Kosul = model.Kosul.Replace("modifieddate", null);
                                    }
                                    else if (whereKosul.Contains("filetype=") && model.Kosul.Contains("filetype"))
                                    {
                                        model.FileType = WhereFileType(whereKosul);
                                        model.Kosul = model.Kosul.Replace("filetype", null);
                                    }
                                    else
                                    {
                                        model.Hata = "girilen şartın ataması yapılmalı!";
                                        return;
                                    }
                                    if (String.IsNullOrEmpty(model.Kosul) == false)
                                    {
                                        model.Hata = "girilen şartın ataması yapılmalı!";
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                model.Hata = "sql yapısına aykırı!";
                                return;
                            }
                        }
                        else
                        {
                            model.Hata = "sql yapısına aykırı!";
                            return;
                        }
                    }
                    else if (selectKosul == "*")
                    {
                        model.Kosul = "namesizepermissionhardlinkusergroupmodifieddatefiletype";
                    }
                    else if (girdi.Contains("where") == false)
                    {
                        model.Hata = "girilen şartın ataması yapılmalı!";
                        return;
                    }
                    else
                    {
                        model.Hata = "regular expressions hatası!";
                        return;
                    }

                }
                else if (islemTipi == "delete" && girdi.IndexOf("from") - 1 == 6)
                {
                    model.IslemTipi = islemTipi;
                    girdi = girdi.Replace(islemTipi + " ", null);

                    if (girdi.IndexOf(" ", 0) == 4 && girdi.IndexOf("where") - 1 == girdi.IndexOf(" ", 5))
                    {
                        if (girdi.Substring(0, 4) == "from")
                        {
                            girdi = girdi.Replace("from ", null);
                            int ilkFrom_Where = 0;
                            int sonFrom_Where = girdi.IndexOf("where", ilkFrom_Where + 1) - 1;
                            string fromKosul = girdi[ilkFrom_Where..sonFrom_Where];
                            if (fromKosul.Contains(" "))
                            {
                                model.Hata = "regular expressions hatası!";
                                return;
                            }
                            model.Directory = Directory(fromKosul);
                            girdi = girdi.Replace(fromKosul + " ", null);

                            if (girdi.Substring(0, 5) == "where")
                            {
                                girdi = girdi.Replace("where ", null);
                                girdi = girdi.Replace("where", null);
                                int ilkWhere_ = 0;
                                int sonWhere_ = girdi.Length;
                                string whereKosul = girdi[ilkWhere_..sonWhere_];
                                if (whereKosul == "")
                                {
                                    model.Hata = "sql yapısına aykırı!";
                                    return;
                                }
                                else if (whereKosul.Contains(" "))
                                {
                                    model.Hata = "girilen şart regular expressionsa aykırı!";
                                    return;
                                }
                                else if (whereKosul.Contains(","))
                                {
                                    model.Hata = "şartlar && ile ayrılmalı!";
                                    return;
                                }
                                if (whereKosul.Contains("&&"))
                                {
                                    if (whereKosul.Contains(" "))
                                    {
                                        model.Hata = "girilen şart regular expressionsa aykırı!";
                                        return;
                                    }
                                    string ampersandWhereKosul = whereKosul;
                                    while (ampersandWhereKosul.Contains("&&"))
                                    {
                                        int ampersandIndex = ampersandWhereKosul.IndexOf("&&", 0);
                                        if (ampersandIndex == 0)
                                        {
                                            ampersandWhereKosul = ampersandWhereKosul[2..];
                                            ampersandIndex = ampersandWhereKosul.IndexOf("&&", 0);
                                        }
                                        if (ampersandIndex == -1)
                                        {
                                            ampersandIndex = ampersandWhereKosul.Length;
                                        }
                                        string whereAmpersandKosul = ampersandWhereKosul.Substring(0, ampersandIndex);
                                        ampersandWhereKosul = ampersandWhereKosul[ampersandIndex..];
                                        if (whereAmpersandKosul.Contains("name="))
                                        {
                                            model.Name = WhereName(whereAmpersandKosul);
                                            continue;
                                        }
                                        else if (whereAmpersandKosul.Contains("size="))
                                        {
                                            model.Size = WhereSize(whereAmpersandKosul);
                                            continue;
                                        }
                                        else if (whereAmpersandKosul.Contains("permission="))
                                        {
                                            model.Permission = WherePermission(whereAmpersandKosul);
                                            continue;
                                        }
                                        else if (whereAmpersandKosul.Contains("hardlink="))
                                        {
                                            model.Hardlink = WhereHardlink(whereAmpersandKosul);
                                            continue;
                                        }
                                        else if (whereAmpersandKosul.Contains("user="))
                                        {
                                            model.User = WhereUser(whereAmpersandKosul);
                                            continue;
                                        }
                                        else if (whereAmpersandKosul.Contains("group="))
                                        {
                                            model.Group = WhereGroup(whereAmpersandKosul);
                                            continue;
                                        }
                                        else if (whereAmpersandKosul.Contains("modifieddate="))
                                        {
                                            model.ModifiedDate = WhereModifiedDate(whereAmpersandKosul);
                                            continue;
                                        }
                                        else if (whereAmpersandKosul.Contains("filetype="))
                                        {
                                            model.FileType = WhereFileType(whereAmpersandKosul);
                                            continue;
                                        }
                                        else
                                        {
                                            model.Hata = "girilen bütün şartların ataması yapılmalı!";
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    if (whereKosul.Contains("name="))
                                    {
                                        model.Name = WhereName(whereKosul);
                                    }
                                    else if (whereKosul.Contains("size="))
                                    {
                                        model.Size = WhereSize(whereKosul);
                                    }
                                    else if (whereKosul.Contains("permission="))
                                    {
                                        model.Permission = WherePermission(whereKosul);
                                    }
                                    else if (whereKosul.Contains("hardlink="))
                                    {
                                        model.Hardlink = WhereHardlink(whereKosul);
                                    }
                                    else if (whereKosul.Contains("user="))
                                    {
                                        model.User = WhereUser(whereKosul);
                                    }
                                    else if (whereKosul.Contains("group="))
                                    {
                                        model.Group = WhereGroup(whereKosul);
                                    }
                                    else if (whereKosul.Contains("modifieddate="))
                                    {
                                        model.ModifiedDate = WhereModifiedDate(whereKosul);
                                    }
                                    else if (whereKosul.Contains("filetype="))
                                    {
                                        model.FileType = WhereFileType(whereKosul);
                                    }
                                    else
                                    {
                                        model.Hata = "girilen şartın ataması yapılmalı!";
                                        return;
                                    }
                                }
                            }
                        }
                        else
                        {
                            model.Hata = "sql yapısına aykırı!";
                            return;
                        }
                    }
                    else
                    {
                        model.Hata = "sql yapısına aykırı!";
                        return;
                    }
                }
                else if (islemTipi == "insert")
                {
                    girdi = girdi.Replace(islemTipi + " ", null);
                    if (girdi.Substring(0, 4) == "into" && girdi.IndexOf(" ") == 4 && girdi.IndexOf("values") - 1 == girdi.IndexOf(" ", 5))
                    {
                        model.IslemTipi = islemTipi;
                        girdi = girdi.Replace("into ", null);
                        int ilkInto_Values = 0;
                        int sonInto_Values = girdi.IndexOf(" ");
                        string insertKosul = girdi[ilkInto_Values..sonInto_Values];
                        model.Directory = Directory(insertKosul);
                        girdi = girdi.Replace(insertKosul + " ", null);
                        if (girdi.Substring(0, 6) == "values" && girdi.IndexOf(" ") == 6 && girdi.Contains("("))
                        {
                            girdi = girdi.Replace("values ", null);
                            if (girdi.Contains(" "))
                            {
                                model.Hata = "şartın regular expressionsına aykırı!";
                                return;
                            }
                            else
                            {
                                int ilk = girdi.IndexOf("(") + 1;
                                int son = girdi.IndexOf(")");
                                model.Name = girdi.Substring(ilk, son - ilk);
                            }
                        }
                        else
                        {
                            model.Hata = "sql yapısına aykırı!";
                            return;
                        }
                    }
                    else
                    {
                        model.Hata = "sql yapısına aykırı!";
                        return;
                    }
                }
                else if (islemTipi != "select" && islemTipi != "delete" && islemTipi != "insert")
                {
                    model.Hata = "ilk değer select, insert veya delete olmalı!";
                    return;
                }
                else
                {
                    model.Hata = "sql yapısına aykırı!";
                    return;
                }
            }
            else if (girdi.Contains("from") == false)
            {
                model.Hata = "sql yapısına aykırı!";
                return;
            }
            else
            {
                model.Hata = "sql yapısına aykırı!";
                return;
            }
        }
        private string WhereName(string wn)
        {
            int ilkIndex = wn.IndexOf('=', 0) + 1;
            int sonIndex = wn.Length;
            string donen = wn[ilkIndex..sonIndex];
            if (donen == null || donen == "")
            {
                model.Hata = "şartın regular expressionsına aykırı!";
                return null;
            }
            else
            {
                return donen;
            }
        }
        private int WhereSize(string ws)
        {
            string whereSizeKosul = ws;
            char a;
            string kgm = "";
            int kgmIndex = 0;
            string boyut;
            int boyutIndex = whereSizeKosul.IndexOf("=", 0) + 1;
            string kontrolSize = ws[boyutIndex..ws.Length];
            if (String.IsNullOrEmpty(kontrolSize))
            {
                model.Hata = "şartın regular expressionsına aykırı!";
                return 0;
            }
            if (kontrolSize.Length > 1)
            {
                string kontrolSize2 = kontrolSize.Remove(kontrolSize.Length - 1);
                a = kontrolSize2.Last();
            }
            else
            {
                a = kontrolSize.Last();
            }
            if (kontrolSize == null || kontrolSize == "" || Char.IsLetter(a))
            {
                model.Hata = "şartın regular expressionsına aykırı!";
                return 0;
            }
            if (whereSizeKosul.Contains("k") || whereSizeKosul.Contains("g") || whereSizeKosul.Contains("m"))
            {
                if (whereSizeKosul.Contains("k"))
                {
                    kgmIndex = whereSizeKosul.IndexOf("k", 0);
                    kgm = "k";
                }
                else if (whereSizeKosul.Contains("g"))
                {
                    kgmIndex = whereSizeKosul.IndexOf("g", 0);
                    kgm = "g";
                }
                else if (whereSizeKosul.Contains("m"))
                {
                    kgmIndex = whereSizeKosul.IndexOf("m", 0);
                    kgm = "m";
                }
            }
            else
            {
                kgmIndex = ws.Length;
            }
            boyut = whereSizeKosul[boyutIndex..kgmIndex];
            char sonIndex = whereSizeKosul.Last();
            if (Char.IsLetter(sonIndex))
            {
                if (kgm.Equals("k") || kgm.Equals("g") || kgm.Equals("m"))
                {
                    if (kgm.Equals("k"))
                    {
                        return int.Parse(boyut) * 1024;
                    }
                    else if (kgm.Equals("g"))
                    {
                        return int.Parse(boyut) * 1073741824;
                    }
                    else
                    {
                        return int.Parse(boyut) * 1048576;
                    }
                }
                else
                {
                    model.Hata = "hatalı boyut belirttiniz...k, g veya m olmalıdır!";
                    return 0;
                }
            }
            else
            {
                return int.Parse(boyut);
            }
        }
        private string WherePermission(string wp)
        {
            int ilkIndex = wp.IndexOf("=") + 1;
            int sonIndex = wp.Length;
            string izin = wp[ilkIndex..sonIndex];
            if (izin.Length == 1 && Char.IsLetter(char.Parse(izin)))
            {
                if (izin.Equals("r") || izin.Equals("w") || izin.Equals("x"))
                {
                    return izin;
                }
                else
                {
                    model.Hata = "izin r, w veya x olmalı!";
                    return null;
                }
            }
            else if (String.IsNullOrEmpty(izin))
            {
                model.Hata = "şartın regular expressionsına aykırı!";
                return null;
            }
            else
            {
                model.Hata = "izin r, w veya x olmalı!";
                return null;
            }
        }
        private string WhereHardlink(string wh)
        {
            int ilkIndex = wh.IndexOf("=") + 1;
            int sonIndex = wh.Length;
            string hardlink = wh[ilkIndex..sonIndex];
            if (String.IsNullOrEmpty(hardlink))
            {
                model.Hata = "şartın regular expressionsına aykırı!";
                return null;
            }
            else
            {
                return hardlink;
            }
        }
        private string WhereUser(string wu)
        {
            int ilkIndex = wu.IndexOf("=") + 1;
            int sonIndex = wu.Length;
            string user = wu[ilkIndex..sonIndex];
            if (String.IsNullOrEmpty(user))
            {
                model.Hata = "şartın regular expressionsına aykırı!";
                return null;
            }
            else
            {
                return user;
            }
        }
        private string WhereGroup(string wg)
        {
            int ilkIndex = wg.IndexOf("=") + 1;
            int sonIndex = wg.Length;
            string group = wg[ilkIndex..sonIndex];
            if (String.IsNullOrEmpty(group))
            {
                model.Hata = "şartın regular expressionsına aykırı!";
                return null;
            }
            else
            {
                return group;
            }
        }
        private DateTime WhereModifiedDate(string wmd)
        {
            int ilkIndex = wmd.IndexOf("=") + 1;
            int sonIndex = wmd.Length;
            string modifiedDate = wmd[ilkIndex..sonIndex];
            if (String.IsNullOrEmpty(modifiedDate))
            {
                model.Hata = "şartın regular expressionsına aykırı!";
                return DateTime.Parse("01.01.0001");
            }
            else
            {
                DateTime dateTime = DateTime.Parse(modifiedDate);
                if (dateTime.ToString("dd.MM.yyyy") == modifiedDate)
                {
                    return dateTime;
                }
                else
                {
                    model.Hata = "modified date dd.MM.yyyy olacak şekilde girilmelidir!";
                    return DateTime.Parse("01.01.0001");
                }
            }

        }
        private string WhereFileType(string wft)
        {
            int ilkIndex = wft.IndexOf("=") + 1;
            int sonIndex = wft.Length;
            string fileType = wft[ilkIndex..sonIndex];
            if (fileType.Contains(".") && fileType.IndexOf(".") == 0 && fileType.Length > 1)
            {
                return fileType;
            }
            else if (String.IsNullOrEmpty(fileType))
            {
                model.Hata = "şartın regular expressionsına aykırı!";
                return null;
            }
            else
            {
                model.Hata = "filetype . ile başlayıp bir uzantı olması gerekmektedir! (örn= .jpg)";
                return null;
            }
        }
        private string Directory(string d)
        {
            if (d.Contains(" "))
            {
                model.Hata = "dizin boşluk içermemelidir!";
                return null;
            }
            else if (d.Contains(@":\"))
            {
                return d;
            }
            else if (String.IsNullOrEmpty(d))
            {
                model.Hata = "sql yapısına aykırı!";
                return null;
            }
            else
            {
                model.Hata = @"yol x:\xxx\xxx şeklinde belirtilmelidir.!";
                return null;
            }
        }
    }
}
