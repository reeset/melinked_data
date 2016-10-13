using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sample
{
    public class BibFrameTools
    {
        
        public enum rec_type { authority = 0, bibliographic = 1, authority_bibliographic = 2 }
        public enum processing_instructions { none = 0, name = 1, subject = 2, mixed = 3, linking = 4 }
        internal string MARCEDIT_VERSION = "";
        System.Collections.Hashtable collections_table = new System.Collections.Hashtable();

        public struct COLLECTION_RULE
        {
            public string name;
            public string label;
            public string uri;
            public string pattern;
            public string classpath;


            public COLLECTION_RULE(string pname,
                string plabel,
                string puri,
                string ppattern,
                string pclasspath)
            {
                label = plabel;
                uri = puri;
                pattern = ppattern;
                name = pname;
                classpath = pclasspath;
            }
        }


        public struct FIELD_RULE
        {
            public rec_type type;
            public string tag;
            public string subfield;
            public System.Collections.Hashtable ind2;
            public string index;
            public string sticky;
            public int atomize;
            public processing_instructions special_instructions;
            public string uri;
            public string vocab;

            public FIELD_RULE(rec_type ptype = rec_type.bibliographic,
                               string ptag = "000",
                               System.Collections.Hashtable pind2 = null,
                               string psubfield = "a",
                               string pindex = "",
                               string psticky = "",
                               int patomize = 0,
                               processing_instructions pspecial = processing_instructions.none,
                               string puri = "",
                               string pvocab = "")
            {
                type = ptype;
                tag = ptag;

                if (pind2 == null)
                {
                    ind2 = new System.Collections.Hashtable();
                }
                else
                {
                    ind2 = pind2;
                }
                subfield = psubfield;
                index = pindex;
                sticky = psticky;
                atomize = patomize;
                special_instructions = pspecial;
                uri = puri;
                vocab = pvocab;
            }

        }

        private string ConvertToUTF8(string smarc8)
        {
            try
            {
                mengine60.mengine60 objE = new mengine60.mengine60();
                smarc8 = objE.Replace_Diacritics(smarc8, mengine60.mengine60.MNEMONIC_DEFAULT);
                string stmp = objE.ConvertMARC8String(smarc8);
                return stmp;
            }
            catch
            {
                return smarc8;
            }
        }

       
        internal string BuildLinks(string sSource, string tmpDest, System.Collections.Hashtable hoptions)
        {

            //this is the dynamic cache piece
            System.Collections.Hashtable dynamic_cache = new System.Collections.Hashtable();


            bool workid = false;
            bool viaf = false;
            string viaf_index = "";
            bool lcid = false;
            bool autosubjects = false;
            bool f3xx = false;
            string oclcnum = "001";
            string rules_file = "";

            string oclcfield = "";
            string oclc_subfield = "";
            System.Collections.Hashtable rules_table = new System.Collections.Hashtable();



            if (hoptions.ContainsKey("RULESFILE") == true)
            {
                rules_file = (string)hoptions["RULESFILE"];

                System.IO.StreamReader xmlreader = new System.IO.StreamReader(rules_file, System.Text.Encoding.UTF8);
                string resource_data = xmlreader.ReadToEnd();
                xmlreader.Close();
                LoadRules(resource_data, ref rules_table);
                LoadCollections(resource_data, ref collections_table);
            } else
            {
                return "";
            }
            if (hoptions.ContainsKey("OCLCWORKID") == true)
            {
                workid = (bool)hoptions["OCLCWORKID"];
            }

            if (hoptions.ContainsKey("OCLCFIELD") == true)
            {
                oclcnum = (string)hoptions["OCLCFIELD"];
                if (oclcnum.IndexOf("$") == -1)
                {
                    oclcfield = oclcnum;
                    oclc_subfield = "";
                }
                else
                {
                    oclcfield = oclcnum.Substring(0, 3);
                    oclc_subfield = oclcnum.Substring(4);
                }
            }

            if (hoptions.ContainsKey("VIAF"))
            {
                viaf = (bool)hoptions["VIAF"];
                if (hoptions.ContainsKey("VIAFINDEX") == false)
                {
                    viaf = false;
                }
                else
                {
                    viaf_index = (string)hoptions["VIAFINDEX"];
                }
            }

            if (hoptions.ContainsKey("LCID"))
            {
                lcid = (bool)hoptions["LCID"];
            }

            if (hoptions.ContainsKey("AUTOSUBJECT"))
            {
                autosubjects = (bool)hoptions["AUTOSUBJECT"];
            }

            if (hoptions.ContainsKey("F3xx"))
            {
                f3xx = (bool)hoptions["F3xx"];
            }

            melinked_data.resolve_element o = new melinked_data.resolve_element();
            melinked_data.sparqlservice sq = new melinked_data.sparqlservice();

            System.IO.StreamReader reader = new System.IO.StreamReader(sSource, System.Text.Encoding.UTF8, false);
            System.IO.StreamWriter writer = new System.IO.StreamWriter(tmpDest, false, new System.Text.UTF8Encoding(false));

            string line = "";
            string tmp = "";
            string record = "";
            int icount = 1;
            string tmp_workid = String.Empty;
            rec_type current_record = rec_type.bibliographic;

            while (reader.Peek() > -1)
            {
                line = reader.ReadLine();

                if (line.ToLower().StartsWith("=ldr") ||
                    line.StartsWith("=000"))
                {
                    if (line.Substring(12, 1) == "z")
                    {
                        current_record = rec_type.authority;
                    }
                }

                if (line.Trim().Length == 0 || reader.Peek() == -1)
                {
                    if (record.Trim().Length > 0)
                    {
                        if (!string.IsNullOrEmpty(tmp_workid))
                        {
                            if (record.IndexOf("=787") == -1)
                            {
                                record += tmp_workid + System.Environment.NewLine;
                                tmp_workid = "";
                            }
                        }

                        
                        icount++;
                        writer.WriteLine(record);
                    }

                    record = "";
                }
                else
                {

                    if (line.Substring(1, 3) == oclcfield)
                    {
                        string tmpOCLC = "";
                        if (string.IsNullOrEmpty(oclc_subfield))
                        {
                            tmpOCLC = line.Substring(6);
                        }
                        else
                        {
                            string[] subf = line.Split("$".ToCharArray());
                            foreach (string part in subf)
                            {
                                if (part.StartsWith(oclc_subfield))
                                {
                                    if (part.ToLower().IndexOf("(ocolc)") > -1)
                                    {
                                        tmpOCLC = part.Substring(1).ToLower().Replace("(ocolc)", "").Trim();
                                    }

                                    if (part.ToLower().IndexOf("(oclc)") > -1)
                                    {
                                        tmpOCLC = part.Substring(1).ToLower().Replace("(oclc)", "").Trim();
                                    }
                                    break;
                                }
                            }
                        }

                        if (tmpOCLC.StartsWith("ocm") ||
                            tmpOCLC.StartsWith("ocn") ||
                            tmpOCLC.StartsWith("oc"))
                        {
                            tmpOCLC = System.Text.RegularExpressions.Regex.Replace(tmpOCLC, "oc[mn]?", "");
                        }

                        tmpOCLC = tmpOCLC.Trim(" ,.\\".ToCharArray());
                        if (workid == true && helpers.IsNumeric(tmpOCLC) == true)
                        {

                            tmp = o.ResolveTerm(tmpOCLC, melinked_data.resolve_element.LinkResolverService.OCLC_WORK_ID);
                            //System.Windows.Forms.MessageBox.Show(tmp);
                            //tmp_workid = "tmp: " + tmp;
                            //break;
                            if (tmp.Trim().Length > 0)
                            {
                                tmp_workid = @"=787  0\$nOCLC Work Id$o" + tmp;
                            }

                        }
                    }


                    if (rules_table.ContainsKey(line.Substring(1, 3)))
                    {
                        //we process the data.

                        FIELD_RULE myrule = (FIELD_RULE)rules_table[line.Substring(1, 3)];
                        if (((myrule.type == rec_type.bibliographic || myrule.type == rec_type.authority_bibliographic)
                            && current_record == rec_type.bibliographic)
                            || ((myrule.type == rec_type.authority_bibliographic || myrule.type == rec_type.authority)
                            && current_record == rec_type.authority))
                        {
                            //First -- let's see if there are special instructions
                            if (myrule.special_instructions == processing_instructions.linking)
                            {
                                //System.Windows.Forms.MessageBox.Show("here");
                                #region "Linking"
                                string[] linking_subfields = line.Split("$".ToCharArray());
                                foreach (string linking_subfield in linking_subfields)
                                {
                                    if (linking_subfield.StartsWith(myrule.subfield))
                                    {
                                        if (rules_table.ContainsKey(linking_subfield.Substring(1, 3)))
                                        {
                                            myrule = (FIELD_RULE)rules_table[linking_subfield.Substring(1, 3)];
                                            if (((myrule.type == rec_type.bibliographic || myrule.type == rec_type.authority_bibliographic)
                                                && current_record == rec_type.bibliographic)
                                                || ((myrule.type == rec_type.authority_bibliographic || myrule.type == rec_type.authority)
                                                && current_record == rec_type.authority))
                                            {
                                                //continue
                                                //System.Windows.Forms.MessageBox.Show(myrule.tag);
                                            }
                                            else
                                            {
                                                goto SkipProcessing;
                                            }
                                        }
                                    }
                                }

                                #endregion     
                            }

                            switch (myrule.special_instructions)
                            {
                                case processing_instructions.name:
                                case processing_instructions.mixed:
                                    {

                                        //System.Windows.Forms.MessageBox.Show(line);
                                        #region "names"
                                        melinked_data.resolve_element.LinkResolverService service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_NAME_LABEL;

                                        string t_controlval = myrule.vocab;


                                        if (myrule.ind2 != null && myrule.ind2.Count > 0)
                                        {
                                            string tind2 = line.Substring(7, 1);
                                            if (myrule.ind2.ContainsKey(tind2))
                                            {
                                                t_controlval = myrule.ind2[tind2].ToString();
                                            }
                                        }


                                        if (myrule.index.Trim().Length > 0 || t_controlval.Trim().Length > 0)
                                        {
                                            if (!string.IsNullOrEmpty(myrule.index) && line.IndexOf("$" + myrule.index) > -1)
                                            {
                                                //there is an index -- let's see if we can figure out which one.
                                                t_controlval = ExtractSubfield(line, myrule.index);
                                            }

                                            if (!string.IsNullOrEmpty(t_controlval))
                                            {
                                                service = GetVocabService(t_controlval.ToLower());
                                            }

                                        }


                                        //if (!string.IsNullOrEmpty(myrule.vocab))
                                        //{
                                        //    service = GetVocabService(myrule.vocab.ToLower());                                           
                                        //}
                                        tmp = o.NormalizeNameEx(line.Substring(8), myrule.subfield);
                                        if (tmp.IndexOf("{") > -1)
                                        {
                                            tmp = ConvertToUTF8(tmp);
                                        }
                                        //System.Windows.Forms.MessageBox.Show(tmp);
                                        if (lcid == true)
                                        {
                                            //System.Windows.Forms.MessageBox.Show(service.ToString());
                                            bool bProcess_Main = false;
                                            if (myrule.ind2 != null && myrule.ind2.Count > 0)
                                            {
                                                //System.Windows.Forms.MessageBox.Show("here" + "\n" + line.Substring(7,1));

                                                if (myrule.ind2.ContainsKey(line.Substring(7, 1)) == true)
                                                {
                                                    //System.Windows.Forms.MessageBox.Show("key found");
                                                    if (line.IndexOf("http") == -1)
                                                    {
                                                        bProcess_Main = true;
                                                    }
                                                }
                                            }
                                            else
                                            {

                                                if (line.IndexOf("http") == -1 &&
                                                    service != melinked_data.resolve_element.LinkResolverService.NONE)
                                                {
                                                    bProcess_Main = true;
                                                }
                                            }

                                            if (bProcess_Main == true)
                                            {
                                                //System.Windows.Forms.MessageBox.Show(line);
                                                #region "Pattern Data"
                                                //First, see if there is a pattern
                                                string pattern = "";
                                                string ptmp = "";
                                                if (collections_table.ContainsKey(t_controlval.ToLower()) == true)
                                                {
                                                    pattern = ((COLLECTION_RULE)collections_table[t_controlval]).pattern;
                                                }

                                                if (pattern != null && pattern != "")
                                                {
                                                    ptmp = ParsePatternTerms(line, t_controlval);
                                                    //System.Windows.Forms.MessageBox.Show(tmp + "\n" + t_controlval);
                                                }

                                                if (ptmp != "")
                                                {
                                                    line = ptmp;
                                                }
                                                #endregion
                                                else
                                                {

                                                    //}
                                                    //if (service == melinked_data.resolve_element.LinkResolverService.OCLC_FAST)
                                                    //{
                                                    //    if (line.ToLower().IndexOf("$" + myrule.uri) == -1)
                                                    //    {
                                                    //        System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                                    //        tmp = ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.OCLC_FAST, String.Empty, string.Empty, ref tb, ref dynamic_cache);
                                                    //        //tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.OCLC_FAST);
                                                    //        //System.Windows.Forms.MessageBox.Show("No fast item: " + tmp);
                                                    //        if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                    //        {
                                                    //            line += "$" + myrule.uri + tmp;
                                                    //        }
                                                    //    }
                                                    //    else
                                                    //    {
                                                    //        //System.Windows.Forms.MessageBox.Show(line);
                                                    //        string[] tmpfast = line.Split("$".ToCharArray());
                                                    //        for (int fastindex = 0; fastindex < tmpfast.Length; fastindex++)
                                                    //        {
                                                    //            if (tmpfast[fastindex].StartsWith("0(OCoLC)"))
                                                    //            {
                                                    //                if (tmpfast[fastindex].IndexOf("0(OCoLC)fst") > -1)
                                                    //                {
                                                    //                    tmpfast[fastindex] = tmpfast[fastindex].Replace("0(OCoLC)fst", @"0http://id.worldcat.org/fast/").Trim();
                                                    //                }
                                                    //                else
                                                    //                {
                                                    //                    //I'm not sure if this is actually in the wild, but 
                                                    //                    //its in my test set.
                                                    //                    tmpfast[fastindex] = tmpfast[fastindex].Replace("0(OCoLC) ", @"0http://id.worldcat.org/fast/").Trim();
                                                    //                    tmpfast[fastindex] = tmpfast[fastindex].Replace("0(OCoLC)", @"0http://id.worldcat.org/fast/").Trim();
                                                    //                }

                                                    //            }
                                                    //        }
                                                    //        line = string.Join("$", tmpfast);
                                                    //        //System.Windows.Forms.MessageBox.Show(line);
                                                    //    }
                                                    //}
                                                    //#endregion
                                                    //else
                                                    //{
                                                    //    #region "Not FAST"
                                                    #region "Not a pattern"
                                                    if (myrule.atomize < 1)
                                                    {
                                                        System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                                        //System.Windows.Forms.MessageBox.Show(tmp);
                                                        tmp = ResolveTerm(tmp, service, string.Empty, t_controlval, ref tb, ref dynamic_cache);
                                                        //tmp = o.ResolveTerm(tmp, service, string.Empty, ref tb);
                                                        if (tb["X-URI"] != null)
                                                        {
                                                            tmp = (string)tb["X-URI"];
                                                        }
                                                        if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                        {
                                                            line += "$" + myrule.uri + tmp;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //System.Windows.Forms.MessageBox.Show(line + "\n" +  myrule.subfield);
                                                        string[] tparts = ExtractParts(line.Substring(8), myrule.subfield, myrule.sticky);
                                                        //System.Windows.Forms.MessageBox.Show(tparts.Length.ToString());
                                                        string tmpline = "";
                                                        foreach (string pt in tparts)
                                                        {
                                                            //System.Windows.Forms.MessageBox.Show(pt);
                                                            //tmp = o.NormalizeTermEx(pt, myrule.subfield);
                                                            tmp = o.NormalizeNameEx(line.Substring(8), myrule.subfield);
                                                            //tmp = o.NormalizeSubjectEx(line.Substring(8), myrule.subfield);
                                                            if (tmp.IndexOf("{") > -1)
                                                            {
                                                                tmp = ConvertToUTF8(tmp);
                                                            }
                                                            System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                                            tmp = ResolveTerm(tmp, service, string.Empty, t_controlval, ref tb, ref dynamic_cache);
                                                            //tmp = o.ResolveTerm(tmp, service, string.Empty, ref tb);
                                                            //System.Windows.Forms.MessageBox.Show("tmp return: " + tmp + "\nservice: " + System.Convert.ToInt32(service));
                                                            if (tb["X-URI"] != null)
                                                            {
                                                                tmp = (string)tb["X-URI"];
                                                            }
                                                            if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                            {
                                                                tmpline += line.Substring(0, 8) + pt + "$" + myrule.uri + tmp + System.Environment.NewLine;
                                                            }
                                                        }
                                                        tmpline = tmpline.Trim(System.Environment.NewLine.ToCharArray());
                                                        //System.Windows.Forms.MessageBox.Show(line + "\n" + tmpline);

                                                        if (tmpline.Length > 0)
                                                        {
                                                            line = tmpline;
                                                        }
                                                    }
                                                    #endregion
                                                }
                                            }
                                            //tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV);
                                            //System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                            //if (line.Substring(0, 2) == "=6")
                                            //{
                                            //    tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_SUBJECTNAME_LABEL, string.Empty, ref tb);
                                            //    tmp = (string)tb["X-URI"];
                                            //}
                                            //else
                                            //{

                                            //    tmp = o.ResolveTerm(tmp, service, string.Empty, ref tb);
                                            //    tmp = (string)tb["X-URI"];
                                            //}


                                            //if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                            //{
                                            //    line += "$" + myrule.uri + tmp;
                                            //}
                                        }

                                        if (viaf == true)
                                        {
                                            System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                            tmp = o.NormalizeName(line.Substring(8));
                                            tmp = ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.OCLC_VIAF, t_controlval, viaf_index, ref tb, ref dynamic_cache);
                                            //tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.OCLC_VIAF, viaf_index);
                                            if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                            {
                                                line += "$" + myrule.uri + tmp;
                                            }
                                        }
                                        #endregion
                                        break;
                                    }
                                case processing_instructions.subject:
                                    {
                                        #region "subjects"
                                        melinked_data.resolve_element.LinkResolverService service = melinked_data.resolve_element.LinkResolverService.NONE;


                                        string t_controlval = myrule.vocab;


                                        if (myrule.ind2 != null && myrule.ind2.Count > 0)
                                        {
                                            string tind2 = line.Substring(7, 1);
                                            if (myrule.ind2.ContainsKey(tind2))
                                            {
                                                t_controlval = myrule.ind2[tind2].ToString();
                                            }
                                        }


                                        if (myrule.index.Trim().Length > 0 || t_controlval.Trim().Length > 0)
                                        {
                                            if (!string.IsNullOrEmpty(myrule.index) && line.IndexOf("$" + myrule.index) > -1)
                                            {
                                                //there is an index -- let's see if we can figure out which one.
                                                t_controlval = ExtractSubfield(line, myrule.index);
                                            }

                                            if (!string.IsNullOrEmpty(t_controlval))
                                            {
                                                service = GetVocabService(t_controlval.ToLower());
                                            }

                                        }


                                        //System.Windows.Forms.MessageBox.Show(service.ToString());
                                        tmp = o.NormalizeSubjectEx(line.Substring(8), myrule.subfield);

                                        if (tmp.IndexOf("{") > -1)
                                        {
                                            tmp = ConvertToUTF8(tmp);
                                        }

                                        if (autosubjects == true)
                                        {
                                            bool bProcess_Subject = false;
                                            if (myrule.ind2 != null && myrule.ind2.Count > 0)
                                            {
                                                //System.Windows.Forms.MessageBox.Show("here" + "\n" + line.Substring(7,1));

                                                if (myrule.ind2.ContainsKey(line.Substring(7, 1)) == true)
                                                {
                                                    //System.Windows.Forms.MessageBox.Show("key found");
                                                    if (line.IndexOf("http") == -1)
                                                    {
                                                        bProcess_Subject = true;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //we just process as a subject
                                                if (line.IndexOf("http") == -1 &&
                                                    service != melinked_data.resolve_element.LinkResolverService.NONE)
                                                {
                                                    bProcess_Subject = true;
                                                }
                                            }

                                            if (bProcess_Subject == true)
                                            {
                                                #region "Pattern Data"
                                                //First, see if there is a pattern
                                                string pattern = "";
                                                string ptmp = "";
                                                if (collections_table.ContainsKey(t_controlval.ToLower()) == true)
                                                {
                                                    pattern = ((COLLECTION_RULE)collections_table[t_controlval]).pattern;
                                                }


                                                if (pattern != null && pattern != "")
                                                {

                                                    ptmp = ParsePatternTerms(line, t_controlval);
                                                }

                                                if (ptmp != "")
                                                {
                                                    line = ptmp;
                                                }
                                                #endregion
                                                ////System.Windows.Forms.MessageBox.Show(line);
                                                //#region "FAST Process"
                                                ////we need to do something special with fast
                                                //if (service == melinked_data.resolve_element.LinkResolverService.OCLC_FAST)
                                                //{
                                                //    if (line.ToLower().IndexOf("$" + myrule.uri) == -1)
                                                //    {
                                                //        System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                                //        tmp = ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.OCLC_FAST, string.Empty, t_controlval, ref tb, ref dynamic_cache);
                                                //        //tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.OCLC_FAST);
                                                //        if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                //        {
                                                //            line += "$" + myrule.uri + tmp;
                                                //        }
                                                //    }
                                                //    else
                                                //    {
                                                //        string[] tmpfast = line.Split("$".ToCharArray());
                                                //        for (int fastindex = 0; fastindex < tmpfast.Length; fastindex++)
                                                //        {
                                                //            if (tmpfast[fastindex].StartsWith("0(OCoLC)"))
                                                //            {
                                                //                if (tmpfast[fastindex].IndexOf("0(OCoLC)fst") > -1)
                                                //                {
                                                //                    tmpfast[fastindex] = tmpfast[fastindex].Replace("0(OCoLC)fst", @"0http://id.worldcat.org/fast/").Trim();
                                                //                }
                                                //                else
                                                //                {
                                                //                    //I'm not sure if this is actually in the wild, but 
                                                //                    //its in my test set.
                                                //                    tmpfast[fastindex] = tmpfast[fastindex].Replace("0(OCoLC) ", @"0http://id.worldcat.org/fast/").Trim();
                                                //                    tmpfast[fastindex] = tmpfast[fastindex].Replace("0(OCoLC)", @"0http://id.worldcat.org/fast/").Trim();
                                                //                }

                                                //            }
                                                //        }
                                                //        line = string.Join("$", tmpfast);
                                                //    }
                                                //}
                                                //#endregion
                                                else
                                                {
                                                    if (myrule.atomize < 1)
                                                    {
                                                        System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                                        //System.Windows.Forms.MessageBox.Show(tmp);
                                                        tmp = ResolveTerm(tmp, service, string.Empty, t_controlval, ref tb, ref dynamic_cache);
                                                        //tmp = o.ResolveTerm(tmp, service, string.Empty, ref tb);
                                                        if (tb["X-URI"] != null)
                                                        {
                                                            tmp = (string)tb["X-URI"];
                                                        }
                                                        if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                        {
                                                            //System.Windows.Forms.MessageBox.Show(tmp);
                                                            line += "$" + myrule.uri + tmp;
                                                        }
                                                        //System.Windows.Forms.MessageBox.Show(line);
                                                    }
                                                    else
                                                    {
                                                        //System.Windows.Forms.MessageBox.Show(line + "\n" +  myrule.subfield);
                                                        string[] tparts = ExtractParts(line.Substring(8), myrule.subfield, myrule.sticky);
                                                        //System.Windows.Forms.MessageBox.Show(tparts.Length.ToString());
                                                        string tmpline = "";
                                                        foreach (string pt in tparts)
                                                        {
                                                            //System.Windows.Forms.MessageBox.Show(pt);
                                                            //tmp = o.NormalizeTermEx(pt, myrule.subfield);
                                                            tmp = o.NormalizeSubjectEx(line.Substring(8), myrule.subfield);
                                                            if (tmp.IndexOf("{") > -1)
                                                            {
                                                                tmp = ConvertToUTF8(tmp);
                                                            }
                                                            System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                                            tmp = ResolveTerm(tmp, service, string.Empty, t_controlval, ref tb, ref dynamic_cache);
                                                            //tmp = o.ResolveTerm(tmp, service, string.Empty, ref tb);
                                                            //System.Windows.Forms.MessageBox.Show("tmp return: " + tmp + "\nservice: " + System.Convert.ToInt32(service));
                                                            if (tb["X-URI"] != null)
                                                            {
                                                                tmp = (string)tb["X-URI"];
                                                            }
                                                            if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                            {
                                                                tmpline += line.Substring(0, 8) + pt + "$" + myrule.uri + tmp + System.Environment.NewLine;
                                                            }
                                                        }
                                                        tmpline = tmpline.Trim(System.Environment.NewLine.ToCharArray());
                                                        //System.Windows.Forms.MessageBox.Show(line + "\n" + tmpline);

                                                        if (tmpline.Length > 0)
                                                        {
                                                            line = tmpline;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        #region "old code that needs to be removed"
                                        /*
                                            if (line.StartsWith("=6"))
                                        {
                                            //=650  \0$a
                                            if (line.Substring(7, 1) == "0")
                                            {
                                                if (line.IndexOf("id.loc.gov") == -1)
                                                {
                                                    System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                                    if (line.StartsWith("=651"))
                                                    {
                                                        tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_CHILDREN_SUBJECTNAME_LABEL, string.Empty, ref tb);
                                                    }
                                                    else
                                                    {
                                                        tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_SUBJECT_LABEL, string.Empty, ref tb);
                                                    }
                                                    tmp = (string)tb["X-URI"];



                                                    if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                    {
                                                        line += "$" + myrule.uri + tmp;
                                                    }
                                                }
                                            }
                                            else if (line.Substring(7, 1) == "7" && autosubjects == true)
                                            {

                                                //read the $2 scan for fast headings
                                                if (line.ToLower().IndexOf("$2fast") > -1)
                                                {
                                                    #region fastheadings
                                                    if (line.ToLower().IndexOf("$" + myrule.uri) == -1)
                                                    {
                                                        tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.OCLC_FAST);
                                                        if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                        {
                                                            line += "$" + myrule.uri + tmp;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        string[] tmpfast = line.Split("$".ToCharArray());
                                                        for (int fastindex = 0; fastindex < tmpfast.Length; fastindex++)
                                                        {
                                                            if (tmpfast[fastindex].StartsWith("0(OCoLC)"))
                                                            {
                                                                if (tmpfast[fastindex].IndexOf("0(OCoLC)fst") > -1)
                                                                {
                                                                    tmpfast[fastindex] = tmpfast[fastindex].Replace("0(OCoLC)fst", @"0http://id.worldcat.org/fast/").Trim();
                                                                }
                                                                else
                                                                {
                                                                    //I'm not sure if this is actually in the wild, but 
                                                                    //its in my test set.
                                                                    tmpfast[fastindex] = tmpfast[fastindex].Replace("0(OCoLC) ", @"0http://id.worldcat.org/fast/").Trim();
                                                                    tmpfast[fastindex] = tmpfast[fastindex].Replace("0(OCoLC)", @"0http://id.worldcat.org/fast/").Trim();
                                                                }

                                                            }
                                                        }
                                                        line = string.Join("$", tmpfast);
                                                    }
                                                    #endregion
                                                }
                                                else if (line.ToLower().IndexOf("$2tgm") > -1)
                                                {
                                                    #region tgmheadings
                                                    System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                                    tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_TGM_LABEL, string.Empty, ref tb);
                                                    tmp = (string)tb["X-URI"];
                                                    if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                    {
                                                        line += "$" + myrule.uri + tmp;
                                                    }
                                                    #endregion
                                                }
                                                else if (line.ToLower().IndexOf("$2aat") > -1)
                                                {
                                                    #region aat headings
                                                    tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.GETTY_AAT_LABEL);
                                                    //System.Windows.Forms.MessageBox.Show(tmp);
                                                    //tmp_workid = "tmp: " + tmp;
                                                    //break;
                                                    if (tmp.Trim().Length > 0)
                                                    {
                                                        line += "$" + myrule.uri + tmp;
                                                    }
                                                    #endregion
                                                }
                                                else if (line.IndexOf("$2") > -1)
                                                {
                                                    service = melinked_data.resolve_element.LinkResolverService.NONE;
                                                    string t_cval = ExtractSubfield(line, myrule.index);


                                                    if (!string.IsNullOrEmpty(t_cval))
                                                    {
                                                        service = GetVocabService(t_cval.ToLower());
                                                    }

                                                    if (service != melinked_data.resolve_element.LinkResolverService.NONE)
                                                    {
                                                        System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                                        tmp = o.ResolveTerm(tmp, service, string.Empty, ref tb);
                                                        if (tb["X-URI"] != null)
                                                        {
                                                            tmp = (string)tb["X-URI"];
                                                        }
                                                        //tmp = (string)tb["X-URI"];
                                                        if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                        {
                                                            line += "$" + myrule.uri + tmp;
                                                        }
                                                    }
                                                }

                                            }
                                            else if (line.Substring(7, 1) == "2" && autosubjects == true)
                                            {
                                                #region mesh
                                                //tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.MESH_QUERY);
                                                ////System.Windows.Forms.MessageBox.Show(tmp);
                                                ////tmp_workid = "tmp: " + tmp;
                                                ////break;
                                                //if (tmp.Trim().Length > 0)
                                                //{
                                                //    line += "$0" + tmp;
                                                //}

                                                tmp = sq.ResolveTerm(tmp, melinked_data.sparqlservice.LinkResolverService.ID_NLM_GOV, line.Substring(1, 3));
                                                if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                {
                                                    line += "$" + myrule.uri + tmp;
                                                }
                                                #endregion
                                            }
                                            else if (line.Substring(7, 1) == "1" && autosubjects == true)
                                            {
                                                //lc childrens subjects
                                                System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                                tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_NAME_LABEL, string.Empty, ref tb);
                                                tmp = (string)tb["X-URI"];
                                                if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                {
                                                    line += "$" + myrule.uri + tmp;
                                                }
                                            }
                                            else
                                            {
                                                if (line.IndexOf("id.loc.gov") == -1)
                                                {
                                                    System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                                    tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_SUBJECT_LABEL, string.Empty, ref tb);
                                                    tmp = (string)tb["X-URI"];



                                                    if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                    {
                                                        line += "$" + myrule.uri + tmp;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            //Process the data here
                                            if (service != melinked_data.resolve_element.LinkResolverService.NONE)
                                            {
                                                if (myrule.atomize < 1)
                                                {
                                                    System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                                    //System.Windows.Forms.MessageBox.Show(tmp);
                                                    tmp = o.ResolveTerm(tmp, service, string.Empty, ref tb);
                                                    tmp = (string)tb["X-URI"];
                                                    if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                    {
                                                        line += "$" + myrule.uri + tmp;
                                                    }
                                                }
                                                else
                                                {
                                                    //System.Windows.Forms.MessageBox.Show(line + "\n" +  myrule.subfield);
                                                    string[] tparts = ExtractParts(line.Substring(8), myrule.subfield);
                                                    //System.Windows.Forms.MessageBox.Show(tparts.Length.ToString());
                                                    string tmpline = "";
                                                    foreach (string pt in tparts)
                                                    {
                                                        //System.Windows.Forms.MessageBox.Show(pt);
                                                        //tmp = o.NormalizeTermEx(pt, myrule.subfield);
                                                        tmp = o.NormalizeSubjectEx(line.Substring(8), myrule.subfield);
                                                        if (tmp.IndexOf("{") > -1)
                                                        {
                                                            tmp = ConvertToUTF8(tmp);
                                                        }
                                                        System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                                        tmp = o.ResolveTerm(tmp, service, string.Empty, ref tb);
                                                        //System.Windows.Forms.MessageBox.Show("tmp return: " + tmp + "\nservice: " + System.Convert.ToInt32(service));
                                                        tmp = (string)tb["X-URI"];
                                                        if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                        {
                                                            tmpline += line.Substring(0, 8) + pt + "$" + myrule.uri + tmp + System.Environment.NewLine;
                                                        }
                                                    }
                                                    tmpline = tmpline.Trim(System.Environment.NewLine.ToCharArray());
                                                    //System.Windows.Forms.MessageBox.Show(line + "\n" + tmpline);

                                                    if (tmpline.Length > 0)
                                                    {
                                                        line = tmpline;
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                         * */
                                        #endregion
                                        #endregion
                                        break;
                                    }
                                default:
                                    {
                                        #region "No Special Processing"
                                        tmp = o.NormalizeTermEx(line.Substring(8), myrule.subfield);
                                        melinked_data.resolve_element.LinkResolverService service = melinked_data.resolve_element.LinkResolverService.NONE;

                                        if (tmp.IndexOf("{") > -1)
                                        {
                                            tmp = ConvertToUTF8(tmp);
                                        }
                                        //if (line.Substring(0, 2) == "=3" && f3xx == true)
                                        //{
                                        //    switch (line.Substring(0, 4))
                                        //    {
                                        //        case "=336":
                                        //            service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_CONTENT_TYPE_LABEL;
                                        //            break;
                                        //        case "=337":
                                        //            service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_MEDIA_TYPES_LABEL;
                                        //            break;
                                        //        case "=338":
                                        //            service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_CARRIERS_TYPES_LABEL;
                                        //            break;
                                        //        case "=376":
                                        //        case "=374":
                                        //        case "=368":
                                        //        case "=372":
                                        //        case "=385":
                                        //        case "=380":
                                        //            service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_SUBJECT_LABEL;
                                        //            break;
                                        //        case "=382":
                                        //            service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_MEDIUM_PERFORMANCE_THESAURUS_LABEL;
                                        //            break;
                                        //        case "=375":
                                        //            service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_DEMOGRAPHIC_GROUP_TERMS_LABEL;
                                        //            break;
                                        //        //case "=373":
                                        //        //    service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_NAME_LABEL;
                                        //        //    break;
                                        //        default:
                                        //            service = melinked_data.resolve_element.LinkResolverService.NONE;
                                        //            break;
                                        //    }
                                        //}

                                        //once these are set -- we need to check and see if there is an index defined, and if there
                                        //is, has it been set.
                                        string t_controlval = myrule.vocab;

                                        if (myrule.index.Trim().Length > 0 || t_controlval.Trim().Length > 0)
                                        {
                                            if (!string.IsNullOrEmpty(myrule.index) && line.IndexOf("$" + myrule.index) > -1)
                                            {
                                                //there is an index -- let's see if we can figure out which one.
                                                t_controlval = ExtractSubfield(line, myrule.index);
                                            }

                                            if (!string.IsNullOrEmpty(t_controlval))
                                            {
                                                service = GetVocabService(t_controlval.ToLower());
                                            }

                                        }

                                        #region "Pattern Data"
                                        //First, see if there is a pattern
                                        string pattern = "";
                                        string ptmp = "";
                                        if (collections_table.ContainsKey(t_controlval.ToLower()) == true)
                                        {
                                            pattern = ((COLLECTION_RULE)collections_table[t_controlval]).pattern;
                                        }

                                        if (pattern != null && pattern != "")
                                        {
                                            ptmp = ParsePatternTerms(line, t_controlval);
                                        }

                                        if (ptmp != "")
                                        {
                                            line = ptmp;
                                        }
                                        #endregion
                                        else
                                        {
                                            if ((line.Substring(0, 2) == "=3" && f3xx == true) ||
                                                line.Substring(0, 2) != "=3")
                                            {
                                                //Process the data here
                                                if (service != melinked_data.resolve_element.LinkResolverService.NONE)
                                                {
                                                    if (myrule.atomize < 1)
                                                    {
                                                        System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                                        //System.Windows.Forms.MessageBox.Show(tmp);
                                                        tmp = ResolveTerm(tmp, service, string.Empty, t_controlval, ref tb, ref dynamic_cache);
                                                        //tmp = o.ResolveTerm(tmp, service, string.Empty, ref tb);
                                                        if (tb["X-URI"] != null)
                                                        {
                                                            tmp = (string)tb["X-URI"];
                                                        }
                                                        //tmp = (string)tb["X-URI"];
                                                        if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                        {
                                                            line += "$" + myrule.uri + tmp;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //System.Windows.Forms.MessageBox.Show(line + "\n" +  myrule.subfield);
                                                        string[] tparts = ExtractParts(line.Substring(8), myrule.subfield, myrule.sticky);
                                                        //System.Windows.Forms.MessageBox.Show(tparts.Length.ToString());
                                                        string tmpline = "";
                                                        foreach (string pt in tparts)
                                                        {
                                                            //System.Windows.Forms.MessageBox.Show(pt);
                                                            tmp = o.NormalizeTermEx(pt, myrule.subfield);

                                                            if (tmp.IndexOf("{") > -1)
                                                            {
                                                                tmp = ConvertToUTF8(tmp);
                                                            }
                                                            System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                                            tmp = ResolveTerm(tmp, service, string.Empty, t_controlval, ref tb, ref dynamic_cache);
                                                            //tmp = o.ResolveTerm(tmp, service, string.Empty, ref tb);
                                                            //System.Windows.Forms.MessageBox.Show("tmp return: " + tmp + "\nservice: " + System.Convert.ToInt32(service));
                                                            //see if the tb is null or not
                                                            if (tb["X-URI"] != null)
                                                            {
                                                                tmp = (string)tb["X-URI"];
                                                            }
                                                            if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                            {
                                                                tmpline += line.Substring(0, 8) + pt + "$" + myrule.uri + tmp + System.Environment.NewLine;
                                                            }
                                                        }
                                                        tmpline = tmpline.Trim(System.Environment.NewLine.ToCharArray());
                                                        //System.Windows.Forms.MessageBox.Show(line + "\n" + tmpline);

                                                        if (tmpline.Length > 0)
                                                        {
                                                            line = tmpline;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                        break;
                                    }
                            }

                        }
                    }

                SkipProcessing:
                    if (line.Trim().Length > 3)
                    {
                        if (helpers.IsNumeric(line.Substring(1, 3)) == true &&
                            System.Convert.ToInt32(line.Substring(1, 3)) > 787)
                        {
                            if (!string.IsNullOrEmpty(tmp_workid))
                            {
                                if (record.IndexOf("worldcat.org") == -1)
                                {
                                    record += tmp_workid + System.Environment.NewLine;
                                    tmp_workid = "";
                                }
                            }
                        }
                    }

                    record += line + System.Environment.NewLine;
                }

            }

            reader.Close();
            writer.Close();

            return tmpDest;
        }


        internal string ParsePatternTerms(string tmp, string collection_label)
        {
            //System.Windows.Forms.MessageBox.Show(line);
            string[] tmpfast = tmp.Split("$".ToCharArray());
            string pattern = "";
            bool bUpdate = false;
            for (int fastindex = 0; fastindex < tmpfast.Length; fastindex++)
            {
                if (tmpfast[fastindex].IndexOf("0(") > -1)
                {
                    //System.Windows.Forms.MessageBox.Show(tmpfast[fastindex] + "\n" + System.Text.RegularExpressions.Regex.IsMatch(tmpfast[fastindex], @"^0\(.*\)[A-Za-z0-9\-_ ]*$").ToString());

                    //We need to make sure it's a key
                    if (tmpfast[fastindex].IndexOf(@"(uri)") == -1 &&
                        System.Text.RegularExpressions.Regex.IsMatch(tmpfast[fastindex], @"^0\(.*\)[A-Za-z0-9\-_ ]*$"))
                    {
                        string id = System.Text.RegularExpressions.Regex.Replace(tmpfast[fastindex].Substring(1), @"\(.*\)", "");
                        if (collections_table.ContainsKey(collection_label.ToLower()) == true)
                        {
                            pattern = ((COLLECTION_RULE)collections_table[collection_label]).pattern;
                            tmpfast[fastindex] = "0" + pattern.Replace("{id}", id);
                            bUpdate = true;
                        }
                    }
                }
            }



            if (bUpdate == true)
            {
                string line = "";
                line = string.Join("$", tmpfast);
                return line;
            }
            return "";

        }



        internal string ResolveTerm(string tmp, melinked_data.resolve_element.LinkResolverService service, string search_index, string collection_label, ref System.Collections.Hashtable tb, ref System.Collections.Hashtable dynamic_cache)
        {

            melinked_data.resolve_element o = new melinked_data.resolve_element();
            string provided_uri = "";
            string classpath = "";
            string term = tmp;

            if (!string.IsNullOrEmpty(collection_label))
            {
                if (collections_table.ContainsKey(collection_label.ToLower()) == true)
                {
                    provided_uri = ((COLLECTION_RULE)collections_table[collection_label]).uri;
                    classpath = ((COLLECTION_RULE)collections_table[collection_label]).classpath;
                }
            }


            if (service != melinked_data.resolve_element.LinkResolverService.NONE)
            {

                //System.Windows.Forms.MessageBox.Show(term + "\n" + service.ToString() + "\n" + provided_uri + "\n" + classpath);
                if (dynamic_cache.ContainsKey(service) == false)
                {
                    //System.Windows.Forms.MessageBox.Show("save new:\n" + term);
                    dynamic_cache.Add(service, new System.Collections.Hashtable());
                    tmp = o.ResolveTerm(term, service, search_index, ref tb, provided_uri, classpath);


                    System.Collections.Hashtable temp_tb = new System.Collections.Hashtable();
                    if (tb["X-URI"] == null)
                    {
                        tb.Add("X-URI", tmp);
                    }
                    temp_tb.Add(term, tb);
                    dynamic_cache[service] = temp_tb;
                    return tmp;

                }
                else
                {

                    if (((System.Collections.Hashtable)dynamic_cache[service]).ContainsKey(term))
                    {
                        //System.Windows.Forms.MessageBox.Show("service defined, pull from cache:\n" + term);
                        tb = ((System.Collections.Hashtable)((System.Collections.Hashtable)dynamic_cache[service])[term]);
                        tmp = (string)tb["X-URI"];

                    }
                    else
                    {
                        //System.Windows.Forms.MessageBox.Show("service defined, new key:\n" + term);
                        tmp = o.ResolveTerm(term, service, search_index, ref tb, provided_uri, classpath);
                        System.Collections.Hashtable temp_tb = ((System.Collections.Hashtable)dynamic_cache[service]);

                        if (tb["X-URI"] == null)
                        {
                            tb.Add("X-URI", tmp);
                        }

                        temp_tb.Add(term, tb);
                        dynamic_cache[service] = temp_tb;
                    }
                    return tmp;
                }
            }
            else
            {
                return "";
            }

        }

        internal melinked_data.resolve_element.LinkResolverService GetVocabService(string p)
        {
            melinked_data.resolve_element.LinkResolverService service = melinked_data.resolve_element.LinkResolverService.NONE;

            switch (p)
            {
                case "lcshac":
                    service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_CHILDREN_SUBJECTNAME_LABEL;
                    break;
                case "lcdgt":
                    service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_DEMOGRAPHIC_GROUP_TERMS_LABEL;
                    break;
                case "lcsh":
                    service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_SUBJECT_LABEL;
                    break;
                case "lctgm":
                case "tgm":
                    service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_TGM_LABEL;
                    break;
                case "aat":
                    service = melinked_data.resolve_element.LinkResolverService.GETTY_AAT_LABEL;
                    break;
                case "ulan":
                    service = melinked_data.resolve_element.LinkResolverService.GETTY_ULAN_LABEL;
                    break;
                case "lcgft":
                    service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_GENRE_FORM_LABEL;
                    break;
                case "lcmpt":
                    service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_MEDIUM_PERFORMANCE_THESAURUS_LABEL;
                    break;
                case "naf":
                    service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_NAME_LABEL;
                    break;
                case "naf_lcsh":
                    service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_SUBJECTNAME_LABEL;
                    break;
                case "rdacontent":
                    service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_CONTENT_TYPE_LABEL;
                    break;
                case "rdamedia":
                    service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_MEDIA_TYPES_LABEL;
                    break;
                case "rdacarrier":
                    service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_CARRIERS_TYPES_LABEL;
                    break;
                case "mesh":
                    service = melinked_data.resolve_element.LinkResolverService.MESH_QUERY;
                    break;
                case "fast":
                    service = melinked_data.resolve_element.LinkResolverService.OCLC_FAST;
                    break;
                case "viaf":
                    service = melinked_data.resolve_element.LinkResolverService.OCLC_VIAF;
                    break;
                default:
                    if (!string.IsNullOrEmpty(p))
                    {
                        service = melinked_data.resolve_element.LinkResolverService.CUSTOM_TERMS;
                    }
                    else
                    {
                        service = melinked_data.resolve_element.LinkResolverService.NONE;
                    }
                    break;
            }
            return service;
        }

        private string ExtractSubfield(string line, string p)
        {
            string[] parts = line.Split("$".ToCharArray());
            foreach (string pt in parts)
            {
                if (pt.Substring(0, 1) == p)
                {
                    return pt.Substring(1).Trim();
                }
            }
            return "";
        }

        internal void LoadCollections(string rules_xml, ref System.Collections.Hashtable collections_table)
        {
            System.Xml.XmlDocument objXML = new System.Xml.XmlDocument();
            objXML.LoadXml(rules_xml);

            System.Xml.XmlNodeList rules = objXML.SelectNodes("/marcedit_linked_data_profile/collections/collection");

            foreach (System.Xml.XmlNode node in rules)
            {
                COLLECTION_RULE r = new COLLECTION_RULE();

                System.Xml.XmlNode name = node.SelectSingleNode("name");
                System.Xml.XmlNodeList labellist = node.SelectNodes("label");
                System.Xml.XmlNode uri = node.SelectSingleNode("uri");
                System.Xml.XmlNode classpath = node.SelectSingleNode("path");
                System.Xml.XmlNode pattern = node.SelectSingleNode("pattern");

                if (name != null)
                {
                    r.name = name.InnerText;
                }

                if (uri != null)
                {
                    r.uri = uri.InnerText;
                }

                if (classpath != null)
                {
                    r.classpath = classpath.InnerText;
                }

                if (pattern != null)
                {
                    r.pattern = pattern.InnerText;
                }

                if (labellist != null)
                {
                    foreach (System.Xml.XmlNode pnode in labellist)
                    {
                        r.label = pnode.InnerText;
                        if (collections_table.ContainsKey(r.label) == false)
                        {
                            collections_table.Add(r.label, r);
                        }
                    }
                }
            }
        }


        internal void LoadRules(string rules_xml, ref System.Collections.Hashtable rules_table)
        {
            System.Xml.XmlDocument objXML = new System.Xml.XmlDocument();
            objXML.LoadXml(rules_xml);

            System.Xml.XmlNodeList rules = objXML.SelectNodes("/marcedit_linked_data_profile/rules/field");

            foreach (System.Xml.XmlNode node in rules)
            {
                FIELD_RULE r = new FIELD_RULE();

                System.Xml.XmlNode tag = node.SelectSingleNode("tag");
                System.Xml.XmlNodeList ind2list = node.SelectNodes("ind2");
                System.Xml.XmlNode subfield = node.SelectSingleNode("subfields");
                System.Xml.XmlNode index = node.SelectSingleNode("index");
                System.Xml.XmlNode sticky = node.SelectSingleNode("sticky");
                System.Xml.XmlNode atomize = node.SelectSingleNode("atomize");
                System.Xml.XmlNode special_instructions = node.SelectSingleNode("special_instructions");
                System.Xml.XmlNode uri = node.SelectSingleNode("uri");
                System.Xml.XmlNode vocab = node.SelectSingleNode("vocab");

                if (tag != null)
                {
                    r.tag = tag.InnerText;
                }

                if (ind2list != null)
                {
                    foreach (System.Xml.XmlNode pnode in ind2list)
                    {
                        if (r.ind2 == null)
                        {
                            r.ind2 = new System.Collections.Hashtable();
                        }
                        r.ind2.Add(pnode.Attributes["value"].InnerText, pnode.Attributes["vocab"].InnerText);
                    }
                }


                if (subfield != null)
                {
                    r.subfield = subfield.InnerText;
                }

                if (index != null)
                {
                    r.index = index.InnerText;
                }
                else
                {
                    r.index = "";
                }

                if (sticky != null)
                {
                    r.sticky = sticky.InnerText;
                }
                else
                {
                    r.sticky = "";
                }

                if (atomize != null)
                {
                    try
                    {
                        r.atomize = System.Convert.ToInt32(atomize.InnerText);
                    }
                    catch
                    {
                        r.atomize = 0;
                    }
                }
                else
                {
                    r.atomize = 0;
                }

                if (special_instructions != null)
                {
                    string tmp_instruction = special_instructions.InnerText;
                    switch (tmp_instruction.ToLower())
                    {
                        case "name":
                            r.special_instructions = processing_instructions.name;
                            break;
                        case "subject":
                            r.special_instructions = processing_instructions.subject;
                            break;
                        case "mixed":
                            r.special_instructions = processing_instructions.mixed;
                            break;
                        case "linking":
                            r.special_instructions = processing_instructions.linking;
                            break;
                        default:
                            r.special_instructions = processing_instructions.none;
                            break;
                    }
                }

                if (uri != null)
                {
                    r.uri = uri.InnerText;
                }

                if (vocab != null)
                {
                    r.vocab = vocab.InnerText;
                }
                else
                {
                    r.vocab = "";
                }

                string tmp_type = node.Attributes["type"].InnerText;
                if (tmp_type.IndexOf("|") > -1)
                {
                    r.type = rec_type.authority_bibliographic;
                }
                else if (tmp_type.ToLower().IndexOf("authority") > -1)
                {
                    r.type = rec_type.authority;
                }
                else
                {
                    r.type = rec_type.bibliographic;
                }



                if (rules_table.ContainsKey(r.tag) == false)
                {
                    rules_table.Add(r.tag, r);
                }
            }
        }



        private string[] ExtractParts(string p, string field, string sticky = "")
        {
            System.Collections.ArrayList pp = new System.Collections.ArrayList();
            int icounter = -1;
            string[] broken = p.Split("$".ToCharArray());
            string constants = "";
            string csticky = "";
            bool bfound = false;
            foreach (string f in broken)
            {
                bfound = false;
                if (f.Trim().Length > 0)
                {
                    for (int x = 0; x < field.Length; x++)
                    {
                        if (f.StartsWith(field[x].ToString()))
                        {
                            if (icounter == -1 && pp.Count > 0)
                            {
                                pp[0] = (string)pp[0] + "$" + f;
                            }
                            else
                            {
                                pp.Add("$" + f);
                            }
                            icounter++;
                            bfound = true;
                        }
                    }

                    if (bfound == false)
                    {

                        if (f.StartsWith("2") ||
                            f.StartsWith("6") ||
                            f.StartsWith("8"))
                        {
                            constants += "$" + f;
                            continue;
                        }

                        if (f.Substring(0, 1).IndexOf(sticky) > -1)
                        {
                            csticky += "$" + f;
                            continue;
                        }

                        if (icounter == -1 && pp.Count == 0)
                        {
                            pp.Add("$" + f);
                        }
                        else
                        {
                            pp[0] = (string)pp[0] + "$" + f;
                        }

                    }
                }
            }
            if (pp.Count > 0 && constants.Length > 0)
            {
                for (int x = 0; x < pp.Count; x++)
                {
                    pp[x] = csticky + (string)pp[x] + constants;
                }
            }

            string[] returnarray = new string[pp.Count];
            pp.CopyTo(returnarray);
            return returnarray;
        }
    }
}

/*

        internal string BuildLinks(string sSource, string tmpDest, System.Collections.Hashtable hoptions)
        {

            bool workid = false;
            bool viaf = false;
            string viaf_index = "";
            bool lcid = false;
            bool autosubjects = false;
            bool f3xx = false;
            string oclcnum = "001";
            string rules_file = "";

            string oclcfield = "";
            string oclc_subfield = "";
            System.Collections.Hashtable rules_table = new System.Collections.Hashtable();


            if (hoptions.ContainsKey("RULESFILE") == true)
            {
                rules_file = (string)hoptions["RULESFILE"];
                if (!System.IO.File.Exists(rules_file))
                {
                    //we need to load from the res file
                    string resource_data = Properties.Resources.linked_data_profile;
                    LoadRules(resource_data, ref rules_table);
                    
                }
                else
                {
                    System.IO.StreamReader xmlreader = new System.IO.StreamReader(rules_file, System.Text.Encoding.UTF8);
                    string resource_data = xmlreader.ReadToEnd();
                    xmlreader.Close();
                    LoadRules(resource_data, ref rules_table);
                }
            }

            if (hoptions.ContainsKey("OCLCWORKID") == true)
            {
                workid = (bool)hoptions["OCLCWORKID"];
            }

            if (hoptions.ContainsKey("OCLCFIELD") == true)
            {
                oclcnum = (string)hoptions["OCLCFIELD"];
                if (oclcnum.IndexOf("$") == -1)
                {
                    oclcfield = oclcnum;
                    oclc_subfield = "";
                }
                else
                {
                    oclcfield = oclcnum.Substring(0, 3);
                    oclc_subfield = oclcnum.Substring(4);
                }
            }

            if (hoptions.ContainsKey("VIAF"))
            {
                viaf = (bool)hoptions["VIAF"];
                if (hoptions.ContainsKey("VIAFINDEX") == false)
                {
                    viaf = false;
                }
                else
                {
                    viaf_index = (string)hoptions["VIAFINDEX"];
                }
            }

            if (hoptions.ContainsKey("LCID"))
            {
                lcid = (bool)hoptions["LCID"];
            }

            if (hoptions.ContainsKey("AUTOSUBJECT"))
            {
                autosubjects = (bool)hoptions["AUTOSUBJECT"];
            }

            if (hoptions.ContainsKey("F3xx")) {
                f3xx = (bool)hoptions["F3xx"];
            }

            melinked_data.resolve_element o = new melinked_data.resolve_element();
            melinked_data.sparqlservice sq = new melinked_data.sparqlservice();

            System.IO.StreamReader reader = new System.IO.StreamReader(sSource, System.Text.Encoding.UTF8, false);
            System.IO.StreamWriter writer = new System.IO.StreamWriter(tmpDest, false, new System.Text.UTF8Encoding(false));

            string line = "";
            string tmp = "";
            string record = "";
            int icount = 1;
            string tmp_workid = String.Empty;


            while (reader.Peek() > -1)
            {
                line = reader.ReadLine();
                if (line.Trim().Length == 0 || reader.Peek() == -1)
                {
                    if (record.Trim().Length > 0)
                    {
                        if (!string.IsNullOrEmpty(tmp_workid))
                        {
                            if (record.IndexOf("=787") == -1)
                            {
                                record += tmp_workid + System.Environment.NewLine;
                                tmp_workid = "";
                            }
                        }

                        parent_Object.EventPump();
                        icount++;
                        writer.WriteLine(record);
                    }

                    record = "";
                }
                else
                {

                    switch (line.Substring(0, 4))
                    {
                        case "=336":
                        case "=337":
                        case "=338":
                        case "=382":
                        case "=385":
                        case "=375":
                        case "=376":
                        case "=373":
                        case "=374":
                        case "=368":
                            {
                                if (f3xx == true)
                                {
                                    if (line.Substring(0, 4) == "=376")
                                    {
                                        tmp = o.NormalizeTerm(line.Substring(8), true, "b");
                                    }
                                    else if (line.Substring(0, 4) == "=368")
                                    {
                                        tmp = o.NormalizeTerm(line.Substring(8), true, "c");
                                    }
                                    else
                                    {
                                        tmp = o.NormalizeTerm(line.Substring(8), true);
                                    }

                                    if (tmp.IndexOf("{") > -1)
                                    {
                                        tmp = ConvertToUTF8(tmp);
                                    }
                                    melinked_data.resolve_element.LinkResolverService service = melinked_data.resolve_element.LinkResolverService.NONE; 

                                    switch (line.Substring(0, 4))
                                    {
                                        case "=336":
                                            service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_CONTENT_TYPE_LABEL;
                                            break;
                                        case "=337":
                                            service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_MEDIA_TYPES_LABEL;
                                            break;
                                        case "=338":
                                            service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_CARRIERS_TYPES_LABEL;
                                            break;
                                        case "=376":
                                        case "=374":
                                        case "=368":
                                            service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_SUBJECT_LABEL;
                                            break;
                                        case "=372":
                                            if (line.IndexOf("$2lcsh") > -1)
                                            {
                                                service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_SUBJECT_LABEL;
                                            }
                                            else if (line.IndexOf("$2lcgft") > -1)
                                            {
                                                service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_GENRE_FORM_LABEL;
                                            }
                                            break;
                                        case "=382":
                                            service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_MEDIUM_PERFORMANCE_THESAURUS_LABEL;
                                            break;
                                        case "=385":
                                            service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_SUBJECT_LABEL;
                                            break;
                                        case "=375":
                                            service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_DEMOGRAPHIC_GROUP_TERMS_LABEL;
                                            break;
                                        case "=373":
                                            service = melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_NAME_LABEL;
                                            break;
                                        default:
                                            service = melinked_data.resolve_element.LinkResolverService.NONE;
                                            break;
                                    }

                                    switch (line.Substring(0, 4))
                                    {
                                        case "=336":
                                        case "=337":
                                        case "=338":
                                        case "=368":
                                        case "=376":
                                            {
                                                #region 33x -- not the 382
                                                System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                                tmp = o.ResolveTerm(tmp, service, string.Empty, ref tb);
                                                tmp = (string)tb["X-URI"];
                                                if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                {
                                                    line += "$0" + tmp;
                                                }
                                                break;
                                                #endregion
                                            }
                                        case "=382":
                                        case "=385":
                                        case "=375":
                                        case "=372":
                                        case "=373":
                                        case "=374":
                                            {
                                                if (line.IndexOf("$2lcsh") > -1 ||
                                                    line.IndexOf("$2lcdgt") > -1 ||
                                                    line.IndexOf("$2") > -1)
                                                {
                                                    #region 382 && 375
                                                    string[] tparts = ExtractParts(line.Substring(8), "a");
                                                    string tmpline = "";
                                                    foreach (string pt in tparts)
                                                    {
                                                        tmp = o.NormalizeTerm(pt, true);
                                                        if (tmp.IndexOf("{") > -1)
                                                        {
                                                            tmp = ConvertToUTF8(tmp);
                                                        }
                                                        System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                                        tmp = o.ResolveTerm(tmp, service, string.Empty, ref tb);
                                                        //System.Windows.Forms.MessageBox.Show("tmp return: " + tmp + "\nservice: " + System.Convert.ToInt32(service));
                                                        tmp = (string)tb["X-URI"];
                                                        if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                                        {
                                                            tmpline += line.Substring(0, 8) + pt + "$0" + tmp + System.Environment.NewLine;
                                                        }
                                                    }
                                                    tmpline = tmpline.Trim(System.Environment.NewLine.ToCharArray());
                                                    //System.Windows.Forms.MessageBox.Show(line + "\n" + tmpline);

                                                    if (tmpline.Length > 0)
                                                    {
                                                        line = tmpline;
                                                    }
                                                    #endregion
                                                }
                                                break;
                                            }
                                    }
                                }
                                break;
                            }

                        case "=100":
                        case "=110":
                        case "=111":
                        case "=130":
                        case "=600":
                        case "=610":
                        case "=611":
                        case "=700":
                        case "=710":
                        case "=711":
                        case "=730":
                            tmp = o.NormalizeName(line.Substring(8));
                            if (tmp.IndexOf("{") > -1)
                            {
                                tmp = ConvertToUTF8(tmp);
                            }
                            //System.Windows.Forms.MessageBox.Show(tmp);
                            if (lcid == true)
                            {
                                //tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV);
                                System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                if (line.Substring(0, 2) == "=6")
                                {
                                    tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_SUBJECTNAME_LABEL, string.Empty, ref tb);
                                    tmp = (string)tb["X-URI"];
                                }
                                else
                                {

                                    tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_NAME_LABEL, string.Empty, ref tb);
                                    tmp = (string)tb["X-URI"];
                                }

                                
                                if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                {
                                    line += "$0" + tmp;
                                }
                            }

                            if (viaf == true)
                            {
                                tmp = o.NormalizeName(line.Substring(8));
                                tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.OCLC_VIAF, viaf_index);
                                if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                {
                                    line += "$0" + tmp;
                                }
                            }
                            break;
                        case "=150":
                        case "=151":
                        case "=630":
                        case "=648":
                        case "=650":
                        case "=651":
                        case "=655":

                            if (line.Substring(0, 4) == "=655")
                            {
                                tmp = o.NormalizeSubject(line.Substring(8), true);
                            }
                            else
                            {
                                tmp = o.NormalizeSubject(line.Substring(8));
                            }

                            if (tmp.IndexOf("{") > -1)
                            {
                                tmp = ConvertToUTF8(tmp);
                            }
                            //=650  \0$a
                            if (line.Substring(7, 1) == "0" ||
                                line.Substring(0,4) == "=150" ||
                                line.Substring(0,4) == "=151")
                            {
                                //if (ck_loc.Checked == true)
                                //{
                                //tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV);
                                //if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                //{
                                //    line += "$0" + tmp;
                                //}
                                //}
                                if (line.IndexOf("id.loc.gov") == -1)
                                {
                                    System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                    tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_SUBJECT_LABEL, string.Empty, ref tb);
                                    tmp = (string)tb["X-URI"];



                                    if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                    {
                                        line += "$0" + tmp;
                                    }
                                }
                            }
                            else if (line.Substring(7, 1) == "7" && autosubjects == true)
                            {
                               
                                //read the $2 scan for fast headings
                                if (line.ToLower().IndexOf("$2fast") > -1)
                                {
                                    #region fastheadings
                                    if (line.ToLower().IndexOf("$0") == -1)
                                    {
                                        tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.OCLC_FAST);
                                        if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                        {
                                            line += "$0" + tmp;
                                        }
                                    }
                                    else
                                    {
                                        string[] tmpfast = line.Split("$".ToCharArray());
                                        for (int fastindex = 0; fastindex < tmpfast.Length; fastindex++)
                                        {
                                            if (tmpfast[fastindex].StartsWith("0(OCoLC)"))
                                            {
                                                if (tmpfast[fastindex].IndexOf("0(OCoLC)fst") > -1)
                                                {
                                                    tmpfast[fastindex] = tmpfast[fastindex].Replace("0(OCoLC)fst", @"0http://id.worldcat.org/fast/").Trim();
                                                }
                                                else
                                                {
                                                    //I'm not sure if this is actually in the wild, but 
                                                    //its in my test set.
                                                    tmpfast[fastindex] = tmpfast[fastindex].Replace("0(OCoLC) ", @"0http://id.worldcat.org/fast/").Trim();
                                                    tmpfast[fastindex] = tmpfast[fastindex].Replace("0(OCoLC)", @"0http://id.worldcat.org/fast/").Trim();
                                                }

                                            }
                                        }
                                        line = string.Join("$", tmpfast);
                                    }
                                    #endregion
                                }
                                else if (line.ToLower().IndexOf("$2tgm") > -1)
                                {
                                    #region tgmheadings
                                    System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                    tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_TGM_LABEL, string.Empty, ref tb);
                                    tmp = (string)tb["X-URI"];
                                    if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                    {
                                        line += "$0" + tmp;
                                    }
                                    #endregion
                                }
                                else if (line.ToLower().IndexOf("$2aat") > -1)
                                {
                                    #region aat headings
                                    tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.GETTY_AAT_LABEL);
                                    //System.Windows.Forms.MessageBox.Show(tmp);
                                    //tmp_workid = "tmp: " + tmp;
                                    //break;
                                    if (tmp.Trim().Length > 0)
                                    {
                                        line += "$0" + tmp;
                                    }
                                    #endregion
                                }
                               
                            }
                            else if (line.Substring(7, 1) == "2" && autosubjects == true)
                            {
                                #region mesh
                                //tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.MESH_QUERY);
                                ////System.Windows.Forms.MessageBox.Show(tmp);
                                ////tmp_workid = "tmp: " + tmp;
                                ////break;
                                //if (tmp.Trim().Length > 0)
                                //{
                                //    line += "$0" + tmp;
                                //}

                                tmp = sq.ResolveTerm(tmp, melinked_data.sparqlservice.LinkResolverService.ID_NLM_GOV, line.Substring(1, 3));
                                if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                {
                                    line += "$0" + tmp;
                                }
                                #endregion
                            }
                            else if (line.Substring(7, 1) == "1" && autosubjects == true)
                            {
                                //lc childrens subjects
                                System.Collections.Hashtable tb = new System.Collections.Hashtable();
                                tmp = o.ResolveTerm(tmp, melinked_data.resolve_element.LinkResolverService.ID_LOC_GOV_NAME_LABEL, string.Empty, ref tb);
                                tmp = (string)tb["X-URI"];
                                if (tmp.Trim().Length > 0 && line.IndexOf(tmp) < 0)
                                {
                                    line += "$0" + tmp;
                                }
                            }
                            break;
                        default:
                            if (line.Substring(1, 3) == oclcfield)
                            {
                                string tmpOCLC = "";
                                if (string.IsNullOrEmpty(oclc_subfield))
                                {
                                    tmpOCLC = line.Substring(6);
                                }
                                else
                                {
                                    string[] subf = line.Split("$".ToCharArray());
                                    foreach (string part in subf)
                                    {
                                        if (part.StartsWith(oclc_subfield))
                                        {
                                            if (part.ToLower().IndexOf("(ocolc)") > -1)
                                            {
                                                tmpOCLC = part.Substring(1).ToLower().Replace("(ocolc)", "").Trim();
                                            }

                                            if (part.ToLower().IndexOf("(oclc)") > -1)
                                            {
                                                tmpOCLC = part.Substring(1).ToLower().Replace("(oclc)", "").Trim();
                                            }
                                            break;
                                        }
                                    }
                                }

                                if (tmpOCLC.StartsWith("ocm") ||
                                    tmpOCLC.StartsWith("ocn") ||
                                    tmpOCLC.StartsWith("oc"))
                                {
                                    tmpOCLC = System.Text.RegularExpressions.Regex.Replace(tmpOCLC, "oc[mn]?", "");
                                }

                                tmpOCLC = tmpOCLC.Trim(" ,.\\".ToCharArray());
                                if (workid == true && helpers.IsNumeric(tmpOCLC) == true)
                                {

                                    tmp = o.ResolveTerm(tmpOCLC, melinked_data.resolve_element.LinkResolverService.OCLC_WORK_ID);
                                    //System.Windows.Forms.MessageBox.Show(tmp);
                                    //tmp_workid = "tmp: " + tmp;
                                    //break;
                                    if (tmp.Trim().Length > 0)
                                    {
                                        tmp_workid = @"=787  0\$nOCLC Work Id$o" + tmp;
                                    }

                                }
                            }
                            break;
                    }

                    if (line.Trim().Length > 3)
                    {
                        if (helpers.IsNumeric(line.Substring(1, 3)) == true &&
                            System.Convert.ToInt32(line.Substring(1, 3)) > 787)
                        {
                            if (!string.IsNullOrEmpty(tmp_workid))
                            {
                                if (record.IndexOf("=787") == -1)
                                {
                                    record += tmp_workid + System.Environment.NewLine;
                                    tmp_workid = "";
                                }
                            }
                        }
                    }

                    record += line + System.Environment.NewLine;
                }
            }

            reader.Close();
            writer.Close();

            return tmpDest;
        }

  
 
 **/
