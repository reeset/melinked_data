using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sample
{
    internal static class helpers
    {

        private static System.CodeDom.Compiler.TempFileCollection tmpColl = new System.CodeDom.Compiler.TempFileCollection();
        #region CatchEntities
        internal static string CatchEntities(string str)
        {
            //System.Windows.Forms.MessageBox.Show(str);
            try
            {
                //byte[] b;
                System.Text.RegularExpressions.Regex objReg = new System.Text.RegularExpressions.Regex(@"((?<start>[^\\]\\x)|(?<start>^\\x))(?<chars>[a-zA-Z0-9]*)", System.Text.RegularExpressions.RegexOptions.Singleline);
                System.Text.RegularExpressions.MatchCollection objMatches;
                objMatches = objReg.Matches(str, 0);
                if (objMatches.Count > 0)
                {
                    foreach (System.Text.RegularExpressions.Match objMatch in objMatches)
                    {
                        try
                        {
                            str = str.Replace(objMatch.Value, ((char)(System.Convert.ToInt32(objMatch.Groups["chars"].Value, 16))).ToString());
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch { }

            return str;

        }

        #endregion

        #region stripWhiteSpace

        internal static string stripWhiteSpace(string s)
        {
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"\s+");         // remove all whitespace 
            s = r.Replace(s, " "); // compress all whitespace to one space 
            return s;
        }
        #endregion

        #region stripHtml
        internal static string stripHtml(string strHtml)
        {
            if (strHtml.ToLower().IndexOf("<body") > -1)
            {
                strHtml = strHtml.Substring(strHtml.ToLower().IndexOf("<body"));
            }
            //System.Windows.Forms.MessageBox.Show(strHtml);
            //Strip javascript
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("<script.*</script>", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
            strHtml = r.Replace(strHtml, " "); // compress all whitespace to one space 

            //Strips the HTML tags from strHTML 
            System.Text.RegularExpressions.Regex objRegExp
                = new System.Text.RegularExpressions.Regex("<(.|\n)+?>");

            // Replace all tags with a space, otherwise words either side 
            // of a tag might be concatenated 
            string strOutput = objRegExp.Replace(strHtml, " ");

            // Replace all < and > with &lt; and &gt; 
            strOutput = strOutput.Replace("<", "&lt;");
            strOutput = strOutput.Replace(">", "&gt;");

            return strOutput;
        }
        #endregion

        #region Expand_Functions 
        internal static string Expand_Functions(string s)
        {
            if (s.IndexOf("{date(\"yyyy") > -1)
            {
                s = s.Replace("{date(\"yyyy\")}", DateTime.Now.ToString("yyyy"));
                s = s.Replace("{date(\"yyyy/mm/dd\")}", DateTime.Now.ToString("yyyy/MM/dd"));
                s = s.Replace("{date(\"yyyy-mm-dd\")}", DateTime.Now.ToString("yyyy-MM-dd"));
                s = s.Replace("{date(\"D\")}", DateTime.Now.ToString("D", System.Globalization.DateTimeFormatInfo.InvariantInfo));
            }


            return s;
        }
        #endregion

        #region CustomSort
        //This sorts the swap field function
        internal static string CustomSort(string mfield, string fields)
        {
            if (mfield.StartsWith("=") == true)
            {
                mfield = mfield.Substring(1);
            }
            string[] afields = fields.Split("\n".ToCharArray());
            System.Collections.ArrayList flist = new System.Collections.ArrayList();
            System.Collections.ArrayList alist = new System.Collections.ArrayList();

            foreach (string l in afields)
            {
                if (l.StartsWith("=" + mfield) == true)
                {
                    alist.Add(l);
                }
                else
                {
                    flist.Add(l);
                }
            }

            //Now we sort

            bool bset = false;
            for (int x = 0; x < flist.Count; x++)
            {
                string l = (string)flist[x];
                if (l.Trim().Length > 0)
                {
                    if (IsNumeric(l.Substring(1, 3)) == true && IsNumeric(mfield) == true)
                    {
                        if (System.Convert.ToInt32(l.Substring(1, 3)) > System.Convert.ToInt32(mfield))
                        {
                            flist.InsertRange(x, alist);
                            bset = true;
                            break;
                        }
                    }
                    else if (mfield.ToLower() == "ldr")
                    {
                        flist.InsertRange(0, alist);
                        bset = true;
                        break;
                    }
                }
                else
                {
                    if (bset == false)
                    {
                        flist.InsertRange(x, alist);
                        bset = true;
                        break;
                    }
                }
            }


            if (bset == false)
            {
                flist.AddRange(alist);
            }
            string[] final_array = new string[flist.Count];
            flist.CopyTo(final_array);

            return String.Join("\n", final_array);

        }
        #endregion

        #region specialRegExtensions
        internal static string specialRegExtensions(string sarg, string sregexp, string sreplace, out int intMatch)
        {
            sarg = sarg.Replace(")", "{rpara}");
            System.Text.RegularExpressions.Regex objReg = new System.Text.RegularExpressions.Regex(sregexp);
            System.Text.RegularExpressions.MatchCollection objMatch = objReg.Matches(sarg);
            intMatch = objMatch.Count;
            //System.Windows.Forms.MessageBox.Show(intMatch.ToString());
            //System.Windows.Forms.MessageBox.Show(sarg + "\n" + sregexp + "\n" + sreplace);

            foreach (System.Text.RegularExpressions.Match objm in objMatch)
            {
                System.Text.RegularExpressions.GroupCollection objgc = objm.Groups;
                for (int x = 0; x < objgc.Count; x++)
                {
                    if (sreplace.IndexOf("ucase($" + x.ToString() + ")") > -1)
                    {
                        //System.Windows.Forms.MessageBox.Show("ucase($" + x.ToString() + ")" + "\n" +
                        //                                     objgc[x].Value.ToUpper());
                        string t_i = objgc[x].Value.ToUpperInvariant();
                        //if (IsNumeric(t_i) == true)
                        //{
                        t_i = @"{me_num_literal}" + t_i;
                        //}
                        sreplace = sreplace.Replace("ucase($" + x.ToString() + ")", t_i);
                        //sreplace = sreplace.Replace("ucase($" + x.ToString() + ")", objgc[x].Value.ToUpper());
                    }
                    else if (sreplace.IndexOf("lcase($" + x.ToString() + ")") > -1)
                    {
                        //System.Windows.Forms.MessageBox.Show("lcase($" + x.ToString() + ")");
                        string t_i = objgc[x].Value.ToLowerInvariant();
                        //if (IsNumeric(t_i) == true)
                        //{
                        t_i = @"{me_num_literal}" + t_i;
                        //}
                        sreplace = sreplace.Replace("lcase($" + x.ToString() + ")", t_i);
                        //sreplace = sreplace.Replace("lcase($" + x.ToString() + ")", objgc[x].Value.ToLower());
                    }
                    else if (sreplace.IndexOf("titlecase($" + x.ToString() + ")") > -1)
                    {
                        //System.Windows.Forms.MessageBox.Show("lcase($" + x.ToString() + ")");
                        System.Globalization.TextInfo myTI = new System.Globalization.CultureInfo("en-US", false).TextInfo;

                        string t_i = myTI.ToTitleCase(myTI.ToLower(objgc[x].Value));
                        //if (IsNumeric(t_i) == true)
                        //{
                        t_i = @"{me_num_literal}" + t_i;
                        //}
                        sreplace = sreplace.Replace("titlecase($" + x.ToString() + ")", t_i);
                        //sreplace = sreplace.Replace("lcase($" + x.ToString() + ")", objgc[x].Value.ToLower());
                    }
                }
            }
            //Console.WriteLine(sreplace);
            //System.Windows.Forms.MessageBox.Show(sarg + "\n" + sreplace);
            string sret = objReg.Replace(sarg, sreplace);

            sret = sret.Replace("{me_num_literal}", "");
            //System.Windows.Forms.MessageBox.Show(sret);
            if (sret.IndexOf("lcase(") != -1 || sret.IndexOf("ucase(") != -1 || sret.IndexOf("titlecase(") != -1)
            {
                sret = ProcessCase(sret);
            }

            return sret.Replace("{rpara}", ")").Replace("{RPARA}", ")");
            //return sret;

        }
        #endregion

        #region ProcessCase
        internal static string ProcessCase(string haystack)
        {
            string pre = "";
            string post = "";
            string val = "";

            //System.Windows.Forms.MessageBox.Show(haystack);
            while (haystack.IndexOf("lcase(") > -1)
            {
                pre = haystack.Substring(0, haystack.IndexOf("lcase("));
                val = haystack.Substring(pre.Length + "lcase(".Length);
                if (val.IndexOf(")") > -1)
                {
                    val = val.Substring(0, val.IndexOf(")")).ToLowerInvariant();
                }
                post = haystack.Substring(pre.Length + "lcase(".Length + val.Length + 1);
                haystack = pre + val + post;
                //System.Windows.Forms.MessageBox.Show(haystack);
            }

            while (haystack.IndexOf("ucase(") > -1)
            {
                pre = haystack.Substring(0, haystack.IndexOf("ucase("));
                val = haystack.Substring(pre.Length + "ucase(".Length);
                if (val.IndexOf(")") > -1)
                {
                    val = val.Substring(0, val.IndexOf(")")).ToUpperInvariant();//ToLower();
                }

                post = haystack.Substring(pre.Length + "ucase(".Length + val.Length + 1);
                //System.Windows.Forms.MessageBox.Show(pre + "\n" + val + "\n" + post);
                haystack = pre + val + post;
            }

            while (haystack.IndexOf("titlecase(") > -1)
            {
                System.Globalization.TextInfo myTI = new System.Globalization.CultureInfo("en-US", false).TextInfo;

                pre = haystack.Substring(0, haystack.IndexOf("titlecase("));
                val = haystack.Substring(pre.Length + "titlecase(".Length);
                if (val.IndexOf(")") > -1)
                {
                    val = myTI.ToTitleCase(myTI.ToLower(val.Substring(0, val.IndexOf(")"))));
                }
                //System.Windows.Forms.MessageBox.Show(val);
                post = haystack.Substring(pre.Length + "titlecase(".Length + val.Length + 1);
                haystack = pre + val + post;
            }


            return haystack;
        }

        #endregion



        #region IsNumeric
        internal static bool IsNumeric(string s)
        {
            if (string.IsNullOrEmpty(s)) { return false; }

            for (int x = 0; x < s.Length; x++)
            {
                if (Char.IsNumber(s[x]) == false)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        internal static string[] ExtractSubfield(string field, string subfield)
        {
            System.Collections.ArrayList catchfields = new System.Collections.ArrayList();
            if (!string.IsNullOrEmpty(field))
            {
                string[] t = field.Split("$".ToCharArray());
                foreach (string part in t)
                {
                    if (part.StartsWith(subfield))
                    {
                        catchfields.Add(part.Substring(1));
                    }
                }
            }

            if (catchfields.Count > 0)
            {
                return (string[])catchfields.ToArray(typeof(string));
            }
            else
            {
                return null;
            }
        }


        internal static string GetTempPath()
        {
            //string sValue = "";
            //cglobal.cxmlini.GetSettings(this.XMLPath(), "settings", "TempPath", System.IO.Path.GetTempPath(), ref sValue);
            //if (sValue.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString())==false) 
            //{
            //    sValue += System.IO.Path.DirectorySeparatorChar;
            //}
            string sValue = System.IO.Path.GetTempPath();
            if (sValue.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) == false)
            {
                sValue += System.IO.Path.DirectorySeparatorChar.ToString();
            }
            return sValue;
        }

        internal static string GenerateTempFile()
        {
            string pgTempPath = GetTempPath();
            string tmpPath = System.IO.Path.GetTempPath();

            if (pgTempPath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) == false)
            {
                pgTempPath += System.IO.Path.DirectorySeparatorChar;
            }

            if (tmpPath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) == false)
            {
                tmpPath += System.IO.Path.DirectorySeparatorChar;
            }

            string tmpName = "";
            if (pgTempPath != tmpPath)
            {
                if (System.IO.Directory.Exists(pgTempPath) == false)
                {
                    tmpName = System.IO.Path.Combine(tmpPath, System.IO.Path.GetRandomFileName() + "marc" + System.Guid.NewGuid().ToString("N") + ".tmp");//System.IO.Path.GetTempFileName();
                    //System.Windows.Forms.MessageBox.Show(tmpName);
                    pgTempPath = "";
                }
                else
                {
                    if (pgTempPath.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) == false)
                    {
                        pgTempPath += System.IO.Path.DirectorySeparatorChar;
                    }
                    tmpName = pgTempPath + System.IO.Path.GetRandomFileName() + "marc" + System.Guid.NewGuid().ToString("N") + ".tmp";
                }
            }
            else
            {
                //tmpName = System.IO.Path.GetTempFileName();
                tmpName = System.IO.Path.Combine(tmpPath, System.IO.Path.GetRandomFileName() + "marc" + System.Guid.NewGuid().ToString("N") + ".tmp");//System.IO.Path.GetTempFileName();
                //System.Windows.Forms.MessageBox.Show(tmpName);
            }

            if (System.IO.File.Exists(tmpName) == false)
            {
                System.IO.FileStream tStream = System.IO.File.Create(tmpName);
                tStream.Close();
            }

            try
            {
                tmpColl.AddFile(tmpName, false);
            }
            catch
            {
                //if this happens, it means a temp file exists already in the 
                //catch and I'm needing to just ignore this because 
                //it will already be used
                //make sure the file isn't locked -- if it is, get a new temp file using the operating system

                try
                {
                    System.IO.FileStream tStream = System.IO.File.Create(tmpName);
                    tStream.Close();
                }
                catch
                {
                    //if the above errors -- we create a temp file using the operating system
                    //which should always be unique
                    tmpName = pgTempPath + System.IO.Path.GetRandomFileName() + "marc" + System.Guid.NewGuid().ToString("N") + ".tmp";
                    if (System.IO.File.Exists(tmpName) == false)
                    {
                        System.IO.FileStream tStream = System.IO.File.Create(tmpName);
                        tStream.Close();
                    }

                    tmpColl.AddFile(tmpName, false);
                }
            }
            return tmpName;
            //return 1;
        }

    }
}
