using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace melinked_data
{
    public class resolve_element
    {

        public const string ID_LOC_GOV_SEARCH_URL = "http://id.loc.gov/search/?q=%22{search_terms}%22&start=1&format=atom";
        public const string ID_LOC_GOV_NAME_LABEL_URL = "http://id.loc.gov/authorities/names/label/{search_terms}";
        //public const string ID_LOC_GOV_NAME_LABEL_URL = "http://reeset.net/{search_terms}";
        public const string ID_LOC_GOV_SUBJECT_LABEL_URL = "http://id.loc.gov/authorities/subjects/label/{search_terms}";
        public const string ID_LOC_GOV_SUBJECT_NAME_LABEL_URL = "http://id.loc.gov/authorities/label/{search_terms}";
        public const string ID_LOC_GOV_CHILDREN_SUBJECT_LABEL_URL = "http://id.loc.gov/authorities/childrensSubjects/label/{search_terms}";
        public const string ID_LOC_GOV_TGM_LABEL_URL = "http://id.loc.gov/authorities/graphicMaterials/label/{search_terms}";
        public const string ID_LOC_GOV_GENRE_FORM_LABEL_URL = "http://id.loc.gov/authorities/genreForms/label/{search_terms}";
        public const string ID_LOC_GOV_PERFORMANCE_MEDIUMS_LABEL_URL = "http://id.loc.gov/authorities/performanceMediums/label/{search_terms}";
        public const string ID_LOC_GOV_DEMOGRAPHIC_GROUP_URL = "http://id.loc.gov/authorities/demographicTerms/label/{search_terms}";
        
        //RDA vocabularies -- leveraging id.loc.gov for this (33x fields)
        public const string ID_LOC_GOV_CONTENT_TYPE_URL = "http://id.loc.gov/authorities/contentTypes/label/{search_terms}";
        public const string ID_LOC_GOV_CARRIERS_URL = "http://id.loc.gov/authorities/carriers/label/{search_terms}";
        public const string ID_LOC_GOV_MEDIA_TYPES_URL = "http://id.loc.gov/authorities/mediaTypes/label/{search_terms}";

        //382, 385
        public const string ID_LOC_GOV_PERFORMANCE_MEDIUM_THESAURUS_URL = "http://id.loc.gov/authorities/performanceMediums/label/{search_terms}";
        public const string ID_LOC_GOV_DEMOGRAPHIC_GROUP_TERMS_URL = "http://id.loc.gov/authorities/demographicTerms/label/{search_terms}";

        //aat
        //public const string GETTY_AAT_URL = "http://vocab.getty.edu/sparql.json?query=select+%3FSubject+%3FTerm+%3FParents+%3FScopeNote+%7B%0A%0A++%3FSubject+a+skos%3AConcept%3B+luc%3Aterm+%27+%22{search_terms}%22+%27%3B%0A%0A+++++gvp%3AprefLabelGVP+%5Bxl%3AliteralForm+%3FTerm%5D.%0A%0A++optional+%7B%3FSubject+gvp%3AparentStringAbbrev+%3FParents%7D%0A%0A++optional+%7B%3FSubject+skos%3AscopeNote+%5Bdct%3Alanguage+gvp_lang%3Aen%3B+rdf%3Avalue+%3FScopeNote%5D%7D%7D%0A%0A&toc=Exact-Match_Full_Text_Search_Query&implicit=true&equivalent=false&_form=%2FqueriesF";
        public const string GETTY_AAT_URL = @"http://vocab.getty.edu/sparql.json?query=select%20*%20%7B%3Fsubj%20rdfs%3Alabel%20%22{search_terms}%22%40en%7D&toc=Combination_Full-Text_and_Exact_String_Match&implicit=true&equivalent=false&_form=/queriesF";
        
        //ulan
        //public const string GETTY_ULAN_URL = "http://vocab.getty.edu/sparql.json?query=select%20%3FSubject%20%3FTerm%20%3FParents%20%3FScopeNote%20%7B%0A%0A%20%20%3FSubject%20a%20skos%3AConcept%3B%20rdfs%3Alabel%20%22{search_terms}%22%3B%0A%0A%20%20%20%20%20gvp%3AprefLabelGVP%20%5Bxl%3AliteralForm%20%3FTerm%5D.%0A%0A%20%20optional%20%7B%3FSubject%20gvp%3AparentStringAbbrev%20%3FParents%7D%0A%0A%20%20optional%20%7B%3FSubject%20skos%3AscopeNote%20%5Bdct%3Alanguage%20gvp_lang%3Aen%3B%20rdf%3Avalue%20%3FScopeNote%5D%7D%7D%0A%0A&toc=Case-insensitive_Full_Text_Search_Query&implicit=true&equivalent=false&_form=/queriesF";
        public const string GETTY_ULAN_URL = @"http://vocab.getty.edu/sparql.json?query=select%20%3FSubject%20%3FTerm%20%3FParents%20%3FScopeNote%20%7B%0A%0A%20%20%3FSubject%20a%20skos%3AConcept%3B%20luc%3Aterm%20'%20%22{search_terms}%22%20'%3B%0A%0A%20%20%20%20%20gvp%3AprefLabelGVP%20%5Bxl%3AliteralForm%20%3FTerm%5D.%0A%0A%20%20optional%20%7B%3FSubject%20gvp%3AparentStringAbbrev%20%3FParents%7D%0A%0A%20%20optional%20%7B%3FSubject%20skos%3AscopeNote%20%5Bdct%3Alanguage%20gvp_lang%3Aen%3B%20rdf%3Avalue%20%3FScopeNote%5D%7D%7D%0A&toc=Combination_Full-Text_and_Exact_String_Match&implicit=true&equivalent=false&_form=/queriesF";


        //+and+local.sources+any+"lc"+&maximumRecords=5&startRecord=1&sortKeys=holdingscount&http:accept=application/rss%2bxml
        //the target for LC limits to LC -- I might have to include others to make this work for everyone
        public const string OCLC_VIAF_SEARCH_URL = "http://viaf.org/viaf/search?query=local.mainHeadingEl+all+%22{search_terms}%22{source_terms}+&maximumRecords={max_terms}&startRecord=1&sortKeys=holdingscount&http:accept=application/rss%2bxml";
        public const string OCLC_FAST_SEARCH_URL = "http://fast.oclc.org/searchfast/fastsuggest?&query={search_terms}&queryIndex=suggestall&queryReturn=suggestall%2Cidroot%2Cauth&suggest=autoSubject&rows=1";
        public const string OCLC_WORK_ID = "http://worldcat.org/oclc/{search_terms}.jsonld";
        //public const string MESH_SPARQL_URL = "https://id.nlm.nih.gov/mesh/query?query=PREFIX+rdf%3A+%3Chttp%3A%2F%2Fwww.w3.org%2F1999%2F02%2F22-rdf-syntax-ns%23%3E%0D%0APREFIX+rdfs%3A+%3Chttp%3A%2F%2Fwww.w3.org%2F2000%2F01%2Frdf-schema%23%3E%0D%0APREFIX+xsd%3A+%3Chttp%3A%2F%2Fwww.w3.org%2F2001%2FXMLSchema%23%3E%0D%0APREFIX+owl%3A+%3Chttp%3A%2F%2Fwww.w3.org%2F2002%2F07%2Fowl%23%3E%0D%0APREFIX+meshv%3A+%3Chttp%3A%2F%2Fid.nlm.nih.gov%2Fmesh%2Fvocab%23%3E%0D%0APREFIX+mesh%3A+%3Chttp%3A%2F%2Fid.nlm.nih.gov%2Fmesh%2F%3E%0D%0APREFIX+mesh2015%3A+%3Chttp%3A%2F%2Fid.nlm.nih.gov%2Fmesh%2F2015%2F%3E%0D%0APREFIX+mesh2016%3A+%3Chttp%3A%2F%2Fid.nlm.nih.gov%2Fmesh%2F2016%2F%3E%0D%0A%0D%0ASELECT+%3Fd+%3FdName+%0D%0AFROM+%3Chttp%3A%2F%2Fid.nlm.nih.gov%2Fmesh%3E%0D%0AWHERE+%7B%0D%0A++%3Fd+a+meshv%3ADescriptor+.%0D%0A++%3Fd+rdfs%3Alabel+%3FdName+%0D%0A++FILTER%28REGEX%28%3FdName%2C%27{search_terms}%27%2C%27i%27%29%29+%0D%0A%7D+%0D%0AORDER+BY+%3Fd+%0D%0A&format=JSON&inference=true&year=current&limit=1&offset=0";
        //public const string MESH_SPARQL_URL = @"https://id.nlm.nih.gov/mesh/sparql?query=PREFIX+rdf%3A+%3Chttp%3A%2F%2Fwww.w3.org%2F1999%2F02%2F22-rdf-syntax-ns%23%3E%0D%0APREFIX+rdfs%3A+%3Chttp%3A%2F%2Fwww.w3.org%2F2000%2F01%2Frdf-schema%23%3E%0D%0APREFIX+meshv%3A+%3Chttp%3A%2F%2Fid.nlm.nih.gov%2Fmesh%2Fvocab%23%3E%0D%0APREFIX+mesh%3A+%3Chttp%3A%2F%2Fid.nlm.nih.gov%2Fmesh%2F%3E%0D%0A%0D%0ASELECT+*%0D%0AFROM+%3Chttp%3A%2F%2Fid.nlm.nih.gov%2Fmesh%3E%0D%0AWHERE+%7B%0D%0A++%3Fd+a+meshv%3ADescriptor+.%0D%0A++%3Fd+rdfs%3Alabel+%22{search_terms}%22%40en%0D%0A+++++%0D%0A%7D+%0D%0A&format=JSON&inference=true&year=current&limit=50&offset=0#lodestart-sparql-results";
        public const string MESH_SPARQL_URL = @"https://id.nlm.nih.gov/mesh/sparql?query=PREFIX+rdf%3A+%3Chttp%3A%2F%2Fwww.w3.org%2F1999%2F02%2F22-rdf-syntax-ns%23%3E%0D%0APREFIX+rdfs%3A+%3Chttp%3A%2F%2Fwww.w3.org%2F2000%2F01%2Frdf-schema%23%3E%0D%0APREFIX+meshv%3A+%3Chttp%3A%2F%2Fid.nlm.nih.gov%2Fmesh%2Fvocab%23%3E%0D%0APREFIX+mesh%3A+%3Chttp%3A%2F%2Fid.nlm.nih.gov%2Fmesh%2F%3E%0D%0A%0D%0ASELECT+*%0D%0AFROM+%3Chttp%3A%2F%2Fid.nlm.nih.gov%2Fmesh%3E%0D%0AWHERE+%7B%0D%0A++%3Fd+a+meshv%3A{descriptor_label}+.%0D%0A++%3Fd+rdfs%3Alabel+%22{search_terms}%22%40en%0D%0A+++++%0D%0A%7D+%0D%0A&format=JSON&inference=true&year=current&limit=50&offset=0#lodestart-sparql-results";
        private string pMARCEDIT_VERSION = "6.2";
        
        public enum LinkResolverService
        {
            NONE = 0,
            ID_LOC_GOV = 1,
            OCLC_VIAF = 2, 
            OCLC_FAST = 3,
            OCLC_WORK_ID = 4,
            ID_LOC_GOV_NAME_LABEL=5,
            ID_LOC_GOV_SUBJECT_LABEL=6,
            ID_LOC_GOV_SUBJECTNAME_LABEL = 7, 
            ID_LOC_GOV_CHILDREN_SUBJECTNAME_LABEL=8, 
            ID_LOC_GOV_TGM_LABEL = 9, 
            ID_LOC_GOV_GENRE_FORM_LABEL = 10,
            ID_LOC_GOV_PERFORMANCE_MEDIUM_LABEL = 11,
            ID_LOC_GOV_DEMOGRAPHIC_GROUP_LABEL = 12, 
            GETTY_AAT_LABEL = 13, 
            GETTY_ULAN_LABEL = 14, 
            ID_LOC_GOV_CONTENT_TYPE_LABEL= 15,
            ID_LOC_GOV_CARRIERS_TYPES_LABEL = 16,
            ID_LOC_GOV_MEDIA_TYPES_LABEL = 17, 
            ID_LOC_GOV_MEDIUM_PERFORMANCE_THESAURUS_LABEL = 18, 
            ID_LOC_GOV_DEMOGRAPHIC_GROUP_TERMS_LABEL = 19, 
            MESH_QUERY = 20, 
            CUSTOM_TERMS=-1000
        }

        public string MARCEDIT_VERSION
        {
            set { pMARCEDIT_VERSION = value; }
            get { return pMARCEDIT_VERSION; }
        }

        public enum RSS_TYPES
        {
            rss = 1,
            atom = 2
        }

        internal bool IsNumeric(string s)
        {
            if (String.IsNullOrEmpty(s)) return false;
            for (int x = 0; x < s.Length; x++)
            {
                if (Char.IsNumber(s[x]) == false)
                {
                    return false;
                }
            }
            return true;
        }

        private string getURI(LinkResolverService service)
        {
            switch (service)
            {
                case LinkResolverService.OCLC_WORK_ID:
                    return OCLC_WORK_ID;
                case LinkResolverService.ID_LOC_GOV:
                    return ID_LOC_GOV_SEARCH_URL;
                case LinkResolverService.ID_LOC_GOV_NAME_LABEL:
                    return ID_LOC_GOV_NAME_LABEL_URL;
                case LinkResolverService.ID_LOC_GOV_SUBJECT_LABEL:
                    return ID_LOC_GOV_SUBJECT_LABEL_URL;
                case LinkResolverService.ID_LOC_GOV_CHILDREN_SUBJECTNAME_LABEL:
                    return ID_LOC_GOV_CHILDREN_SUBJECT_LABEL_URL;
                case LinkResolverService.ID_LOC_GOV_SUBJECTNAME_LABEL:
                    return ID_LOC_GOV_SUBJECT_NAME_LABEL_URL;
                case LinkResolverService.OCLC_VIAF:
                    return OCLC_VIAF_SEARCH_URL;
                case LinkResolverService.OCLC_FAST:
                    return OCLC_FAST_SEARCH_URL;
                case LinkResolverService.ID_LOC_GOV_TGM_LABEL:
                    return ID_LOC_GOV_TGM_LABEL_URL;
                case LinkResolverService.ID_LOC_GOV_DEMOGRAPHIC_GROUP_LABEL:
                    return ID_LOC_GOV_DEMOGRAPHIC_GROUP_URL;
                case LinkResolverService.ID_LOC_GOV_GENRE_FORM_LABEL:
                    return ID_LOC_GOV_GENRE_FORM_LABEL_URL;
                case LinkResolverService.ID_LOC_GOV_PERFORMANCE_MEDIUM_LABEL:
                    return ID_LOC_GOV_PERFORMANCE_MEDIUMS_LABEL_URL;
                case LinkResolverService.GETTY_AAT_LABEL:
                    return GETTY_AAT_URL;
                case LinkResolverService.GETTY_ULAN_LABEL:
                    return GETTY_ULAN_URL;
                case LinkResolverService.ID_LOC_GOV_CARRIERS_TYPES_LABEL:
                    return ID_LOC_GOV_CARRIERS_URL;
                case LinkResolverService.ID_LOC_GOV_CONTENT_TYPE_LABEL:
                    return ID_LOC_GOV_CONTENT_TYPE_URL;
                case LinkResolverService.ID_LOC_GOV_MEDIA_TYPES_LABEL:
                    return ID_LOC_GOV_MEDIA_TYPES_URL;
                case LinkResolverService.ID_LOC_GOV_MEDIUM_PERFORMANCE_THESAURUS_LABEL:
                    return ID_LOC_GOV_PERFORMANCE_MEDIUM_THESAURUS_URL;
                case LinkResolverService.ID_LOC_GOV_DEMOGRAPHIC_GROUP_TERMS_LABEL:
                    return ID_LOC_GOV_DEMOGRAPHIC_GROUP_URL;
                case LinkResolverService.MESH_QUERY:
                    return MESH_SPARQL_URL;
                default:
                    return "";
            }

        }

        public string ResolveTerm(string term, LinkResolverService service, string search_index = "")
        {
            System.Collections.Hashtable tmp = new System.Collections.Hashtable();
            return ResolveTerm(term, service, search_index, ref tmp);
        }

        public string ResolveTerm(string term, LinkResolverService service, 
            string search_index, 
            ref System.Collections.Hashtable optionret, 
            string provided_uri = "",
            string customClassPath = "")
        {

            LeaveDotsAndSlashesEscaped();

            if (service == LinkResolverService.NONE) { return "";}
            string tmp = String.Empty;
            string search_uri_string = provided_uri;
            
            if (string.IsNullOrEmpty(provided_uri)) {
                search_uri_string = getURI(service);
            }

            try
            {
                switch (service)
                {
                    case LinkResolverService.OCLC_WORK_ID:
                        {

                            int tries = 0;
                            term = term.TrimEnd(@"\".ToCharArray());
                            term = term.Trim();
                            if (IsNumeric(term))
                            {
                                term = System.Convert.ToInt32(term).ToString();
                            }

                            //I'd like to think about how to get rid of this goto.
                        getWorkId:
                            string work_tmp = search_uri_string.Replace("{search_terms}", term);
                            work_tmp = ReadUri(work_tmp);

                            if (work_tmp.IndexOf("exampleOfWork") == -1)
                            {
                                //Run it again
                                work_tmp = search_uri_string.Replace("{search_terms}", term);
                                work_tmp = ReadUri(work_tmp);

                            }
                            //System.Windows.Forms.MessageBox.Show(work_tmp);
                            //System.Collections.Hashtable work_tmph = ParseJson(work_tmp);
                            try
                            {
                                Newtonsoft.Json.Linq.JObject jobj = ParseJsonObject(work_tmp);
                                //System.Windows.Forms.MessageBox.Show("here");
                                //System.Windows.Forms.MessageBox.Show(jobj.ToString());
                                work_tmp = String.Empty;
                                if (jobj.Count > 0)
                                {

                                    foreach (Newtonsoft.Json.Linq.JObject jitem in jobj["@graph"])
                                    {
                                        if (jitem["exampleOfWork"] != null)
                                        {
                                            work_tmp = jitem["exampleOfWork"].ToString();
                                            break;
                                        }
                                    }
                                    //if (work_tmph.Contains("exampleOfWork")) { 

                                    //    work_tmp = work_tmph["exampleOfWork"].ToString();
                                    //}
                                }
                                tmp = work_tmp;
                                //System.Windows.Forms.MessageBox.Show(tmp);
                            }
                            catch {
                                tries++;
                                if (tries >= 2)
                                {
                                    tmp = "";
                                }
                                else
                                {
                                    goto getWorkId;
                                }
                                
                            }

                            break;
                        }
                    case LinkResolverService.ID_LOC_GOV:
                        {
                            tmp = search_uri_string.Replace("{search_terms}", term);
                            tmp = ReadUri(tmp);
                            
                            tmp = getLinks(tmp, RSS_TYPES.atom);
                            break;
                        }
                    case LinkResolverService.ID_LOC_GOV_NAME_LABEL:
                    case LinkResolverService.ID_LOC_GOV_SUBJECT_LABEL:
                    case LinkResolverService.ID_LOC_GOV_CHILDREN_SUBJECTNAME_LABEL:
                    case LinkResolverService.ID_LOC_GOV_SUBJECTNAME_LABEL:
                    case LinkResolverService.ID_LOC_GOV_TGM_LABEL:
                    case LinkResolverService.ID_LOC_GOV_DEMOGRAPHIC_GROUP_LABEL:
                    case LinkResolverService.ID_LOC_GOV_GENRE_FORM_LABEL:
                    case LinkResolverService.ID_LOC_GOV_PERFORMANCE_MEDIUM_LABEL:
                    case LinkResolverService.ID_LOC_GOV_CARRIERS_TYPES_LABEL:
                    case LinkResolverService.ID_LOC_GOV_CONTENT_TYPE_LABEL:
                    case LinkResolverService.ID_LOC_GOV_MEDIA_TYPES_LABEL:
                    case LinkResolverService.ID_LOC_GOV_DEMOGRAPHIC_GROUP_TERMS_LABEL:
                    case LinkResolverService.ID_LOC_GOV_MEDIUM_PERFORMANCE_THESAURUS_LABEL:
                        {
                            //System.Windows.Forms.MessageBox.Show("term: " + term);
                            tmp = search_uri_string.Replace("{search_terms}", System.Uri.EscapeDataString(term));
                            //System.Windows.Forms.MessageBox.Show(tmp);
                            System.Collections.Hashtable headertable = new System.Collections.Hashtable();
                            string[] headers = new string[2];
                            headers[0] = "X-PrefLabel";
                            headers[1] = "X-URI";

                            headertable = ReadUriHeaders(tmp, headers);
                            tmp = (string)headertable[headers[1]];
                            optionret = headertable;
                            break;
                        }
                    case LinkResolverService.OCLC_VIAF:
                        {
                            
                            if (search_index == String.Empty)
                            {
                                tmp = search_uri_string.Replace("{search_terms}", System.Uri.EscapeDataString(term)).Replace("{max_terms}", "20").Replace("{source_terms}", "");
                            }
                            else
                            {
                                tmp = search_uri_string.Replace("{search_terms}", System.Uri.EscapeDataString(term)).Replace("{max_terms}", "20").Replace("{source_terms}", "+and+local.sources+any+%22" + search_index + "%22");
                                //+and+local.sources+any+%22lc%22
                            }
                            
                            tmp = ReadUri(tmp);
                            //System.Windows.Forms.MessageBox.Show(tmp);
                            tmp = getLinks(tmp, RSS_TYPES.rss, term);
                            break;
                        }
                    case LinkResolverService.OCLC_FAST:
                        {
                            tmp = search_uri_string.Replace("{search_terms}", System.Uri.EscapeDataString(term.Replace("--", " ")));
                            //System.Windows.Forms.MessageBox.Show(tmp);
                            tmp = ReadUri(tmp);
                            //System.Windows.Forms.MessageBox.Show(tmp);
                            System.Collections.Hashtable tmph = ParseJson(tmp);
                            tmp = String.Empty;
                            if (tmph.Count > 0)
                            {
                                if (tmph.ContainsKey("idroot"))
                                {
                                    tmp = tmph["idroot"].ToString();
                                    tmp = System.Text.RegularExpressions.Regex.Replace(tmp, "fst[0]*", "http://id.worldcat.org/fast/");
                                }
                            }
                            break;
                        }
                    case LinkResolverService.MESH_QUERY:
                        {
                            string descriptor_label = "Descriptor";
                            if (term.IndexOf("--") > -1)
                            {
                                //term = term.Substring(0, term.IndexOf("--")).TrimEnd();
                                term = term.Replace("--", "/");
                                descriptor_label = "AllowedDescriptorQualifierPair";
                                //search_label = descriptorpairlabel;
                            }
                            tmp = search_uri_string.Replace("{descriptor_label}", descriptor_label).Replace("{search_terms}", System.Uri.EscapeDataString(term));
                            //System.Windows.Forms.MessageBox.Show(tmp);
                            System.Collections.Hashtable headertable = new System.Collections.Hashtable();
                            string[] headers = new string[2];
                            headers[0] = "X-PrefLabel";
                            headers[1] = "X-URI";

                            string work_tmp = "";
                            work_tmp = ReadUri(tmp);

                            //string work_tmp = search_uri_string.Replace("{search_terms}", term);
                            //sparqlservice s = new sparqlservice();
                            //tmp = s.ResolveTerm(term, sparqlservice.LinkResolverService.ID_NLM_GOV);

                           
                            //work_tmp = ReadUri(work_tmp);

                            
                            //if (work_tmp.IndexOf("id.nlm.nih.gov") == -1)
                            //{
                            //    tmp = "";

                            //}
                            //System.Windows.Forms.MessageBox.Show(work_tmp);
                            //System.Collections.Hashtable work_tmph = ParseJson(work_tmp);
                            try
                            {
                            if (work_tmp.Trim().Length > 0)
                            {
                                //var jobj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(work_tmp);
                                //Newtonsoft.Json.Linq.JObject jobj = ParseJsonObject(work_tmp);
                                //System.Windows.Forms.MessageBox.Show("here");
                                //System.Windows.Forms.MessageBox.Show("The object: " + jobj.ToString());
                                //work_tmp = String.Empty;
                                //string turi = "";
                                //foreach (Newtonsoft.Json.Linq.JObject jitem in jobj["@graph"])
                                //{
                                //    if (jitem["exampleOfWork"] != null)
                                //    {
                                //        work_tmp = jitem["exampleOfWork"].ToString();
                                //        break;
                                //    }
                                //}

                                Newtonsoft.Json.Linq.JToken token = Newtonsoft.Json.Linq.JObject.Parse(work_tmp);

                                //int page = (int)token.SelectToken("page");
                                //int totalPages = (int)token.SelectToken("total_pages");
                                //
                                
                                string turi = (string)token.SelectToken(customClassPath);
                                if (!string.IsNullOrEmpty(turi))
                                {
                                    tmp = turi;
                                }
                                else
                                {
                                    tmp = "";
                                }
                                //if (jobj["results"]["bindings"][0]["d"]["value"] != null)
                                //{
                                //    turi = (string)jobj["results"]["bindings"][0]["d"]["value"].ToString();
                                //}
                                //if (!String.IsNullOrEmpty(turi))
                                //{
                                //    tmp = turi;
                                //}
                                //else
                                //{
                                //    tmp = "";
                                //}

                                break;
                            }
                            }
                            catch { }
                            
                            break;
                        }
                    case LinkResolverService.GETTY_AAT_LABEL:
                    case LinkResolverService.GETTY_ULAN_LABEL:
                        {
                            //System.Windows.Forms.MessageBox.Show(System.Uri.EscapeDataString(term) + "\n" + System.Uri.EscapeUriString(term));
                            string work_tmp = search_uri_string.Replace("{search_terms}", System.Uri.EscapeDataString(term.ToLower()));                            
                            //System.Windows.Forms.MessageBox.Show(work_tmp);
                            work_tmp = ReadUri(work_tmp);

                            if (work_tmp.IndexOf("http://vocab.getty.edu/") == -1)
                            {
                                tmp = "";

                            }
                            //System.Windows.Forms.MessageBox.Show(work_tmp);
                            //System.Collections.Hashtable work_tmph = ParseJson(work_tmp);
                            try
                            {

                                Newtonsoft.Json.Linq.JToken jtoken = Newtonsoft.Json.Linq.JObject.Parse(work_tmp);
                                //Newtonsoft.Json.Linq.JToken jttoken = Newtonsoft.Json.Linq.JObject.Parse(work_tmp);
                                //int page = (int)token.SelectToken("page");
                                //int totalPages = (int)token.SelectToken("total_pages");
                                //

                                string turi = "";

                                if (work_tmp.IndexOf("aat") > -1)
                                {
                                    turi = (string)jtoken.SelectToken(customClassPath);
                                }
                                else if (work_tmp.IndexOf("ulan") > -1)
                                {
                                    turi = (string)jtoken.SelectToken(customClassPath);
                                }
                                

                                //string turi = (string)jtoken.SelectToken("results.bindings[0].subj.value");
                                

                                
                                if (!string.IsNullOrEmpty(turi))
                                {
                                    work_tmp = turi;
                                }
                                else
                                {
                                    work_tmp = "";
                                }




                                //if (jitem["value"] != null)
                                //{
                                //    if (jitem["value"].ToString().IndexOf("http://vocab.getty.edu/") > -1)
                                //    {
                                //        work_tmp = jitem["value"].ToString();
                                //        break;
                                //    }
                                //}


                                tmp = work_tmp;
                                break;
                            }
                            catch { }
                            break;
                        }
                    case LinkResolverService.CUSTOM_TERMS:
                        {
                            string work_tmp = search_uri_string.Replace("{search_terms}", System.Uri.EscapeDataString(term.ToLower()));
                            //System.Windows.Forms.MessageBox.Show(work_tmp);
                            work_tmp = ReadUri(work_tmp);
                            //System.Windows.Forms.MessageBox.Show(work_tmp);
                            //System.Windows.Forms.MessageBox.Show(customClassPath);
                            if (string.IsNullOrEmpty(work_tmp))
                            {
                                tmp = "";                               
                            }
                            try
                            {

                                Newtonsoft.Json.Linq.JToken jtoken = Newtonsoft.Json.Linq.JObject.Parse(work_tmp);
                                //Newtonsoft.Json.Linq.JToken jttoken = Newtonsoft.Json.Linq.JObject.Parse(work_tmp);
                                //int page = (int)token.SelectToken("page");
                                //int totalPages = (int)token.SelectToken("total_pages");
                                //

                                string turi = "";

                                turi = (string)jtoken.SelectToken(customClassPath);

                                //System.Windows.Forms.MessageBox.Show(turi);
                                if (!string.IsNullOrEmpty(turi))
                                {
                                    work_tmp = turi;
                                }
                                else
                                {
                                    work_tmp = "";
                                }


                                tmp = work_tmp;
                                break;
                            }
                            catch { }//(System.Exception jexep){System.Windows.Forms.MessageBox.Show(jexep.ToString()); }

                            break;
                        }
                }
            }
            catch { tmp = ""; }
            return tmp;
        }

        public System.Collections.Hashtable ReadUriHeaders(string uri, string[] headers, bool bfollowRedirects = false)
        {
            
            ResetRequest:
            System.Net.ServicePointManager.DefaultConnectionLimit = 10;
            System.Collections.Hashtable headerTable = new System.Collections.Hashtable();

            LeaveDotsAndSlashesEscaped();

            //System.Windows.Forms.MessageBox.Show(uri);
            System.Net.WebRequest.DefaultWebProxy = null;
            //System.Net.HttpWebRequest objRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(MyUri(uri));
            System.Net.HttpWebRequest objRequest =
            (System.Net.HttpWebRequest)System.Net.WebRequest.Create(uri);

            objRequest.UserAgent = "MarcEdit 6.2 Headings Retrieval (caching)";
            objRequest.Proxy = null;
            //don't need the redirect in this case -- just need the initial set of headers.
            objRequest.AllowAutoRedirect = false;

            //Changing the default timeout from 100 seconds to 30 seconds.
            objRequest.Timeout = 30000;
            //System.Net.HttpWebResponse objResponse = null;
            //.Create(new System.Uri(uri));
            objRequest.Method = "HEAD";


            try
            {
                using (var objResponse = (System.Net.HttpWebResponse)objRequest.GetResponse())
                {
                
                
                    //objResponse = (System.Net.HttpWebResponse)objRequest.GetResponse();
                    
                    if (objResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        foreach (string name in headers)
                        {
                            headerTable.Add(name, "");
                        }
                    }
                    else if ((int)objResponse.StatusCode == 429) 
                    {
                        //do something.
                    }
                    else
                    {
                        //System.Windows.Forms.MessageBox.Show(((int)objResponse.StatusCode).ToString());
                        //System.Windows.Forms.MessageBox.Show(uri);
                        //foreach (string hk in objResponse.Headers.Keys)
                        //{
                        //    System.Windows.Forms.MessageBox.Show("Key: " + hk + "\nValue: " + objResponse.Headers[hk]);
                        //}
                        foreach (string name in headers)
                        {
                            if (objResponse.Headers.AllKeys.Contains(name))
                            {
                                //System.Windows.Forms.MessageBox.Show("Name: " + name + "\nValue: " + objResponse.Headers[name]);
                                string orig_header = objResponse.Headers[name];
                                byte[] b = System.Text.Encoding.GetEncoding(28591).GetBytes(orig_header);

                                headerTable.Add(name, System.Text.Encoding.UTF8.GetString(b));
                                //headerTable.Add(name, objResponse.CharacterSet);
                            }
                            else
                            {
                                //System.Windows.Forms.MessageBox.Show("here");
                                headerTable.Add(name, "");
                            }
                        }
                    }
                }
                //objResponse.Close();

                return headerTable;
            }
            catch (System.Net.WebException wp)
            {
                if (wp.Status == System.Net.WebExceptionStatus.ProtocolError)
                {
                    var err_response = wp.Response as System.Net.HttpWebResponse;
                    if (err_response != null)
                    {
                        if (((int)err_response.StatusCode) == 429)
                        {
                            string retryafter = "3";
                            int sleep = 0;
                            try
                            {
                                retryafter = err_response.Headers["Retry-After"];
                                if (IsNumeric(retryafter) == false) {
                                    retryafter = "3";
                                }

                                sleep = System.Convert.ToInt32(retryafter);

                            }
                            catch
                            {
                                retryafter = "3";
                            }

                            //sleep it
                            Delay(sleep * 1000);
                            goto ResetRequest;
                        }
                    }
                    else
                    {
                        // no http status code available
                    }
                }
                foreach (string name in headers)
                {
                    headerTable.Add(name, "");
                }
                headerTable.Add("error", wp.ToString());
                //if (objResponse != null)
                //{
                //    objResponse.Close();
                //}
                return headerTable;
            }
            catch (System.Exception p)
            {
                //System.Windows.Forms.MessageBox.Show(p.ToString() + "\n" + p.Message + "\n" + p.StackTrace + "\n"+ uri);
                //headerTable.Add("X-URI", uri + ": " + uri.Length.ToString() + "\n" + p.ToString());
                foreach (string name in headers)
                {
                    headerTable.Add(name, "");
                }
                headerTable.Add("error", p.ToString());
                //if (objResponse != null)
                //{
                //    objResponse.Close();
                //}
                return headerTable;
            }
        }

        private Task Delay(double milliseconds)
        {
            var tcs = new TaskCompletionSource<bool>();
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += (obj, args) =>
            {
                tcs.TrySetResult(true);
            };
            timer.Interval = milliseconds;
            timer.AutoReset = false;
            timer.Start();
            return tcs.Task;
        }

        private void LeaveDotsAndSlashesEscaped()
        {
            var getSyntaxMethod =
                typeof(UriParser).GetMethod("GetSyntax", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            if (getSyntaxMethod == null)
            {
                //probably isn't 4.0
                return;
            }


            System.Reflection.MethodInfo getSyntax = typeof(UriParser).GetMethod("GetSyntax", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            System.Reflection.FieldInfo flagsField = typeof(UriParser).GetField("m_Flags", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (getSyntax != null && flagsField != null)
            {
                foreach (string scheme in new[] { "http", "https" })
                {
                    UriParser parser = (UriParser)getSyntax.Invoke(null, new object[] { scheme });
                    if (parser != null)
                    {
                        int flagsValue = (int)flagsField.GetValue(parser);
                        // Clear the CanonicalizeAsFilePath attribute
                        if ((flagsValue & 0x1000000) != 0)
                            flagsField.SetValue(parser, flagsValue & ~0x1000000);
                    }
                }
            }


            var uriParser = getSyntaxMethod.Invoke(null, new object[] { "http" });

            var setUpdatableFlagsMethod =
                uriParser.GetType().GetMethod("SetUpdatableFlags", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (setUpdatableFlagsMethod == null)
            {
                //throw new MissingMethodException("UriParser", "SetUpdatableFlags");
                //probably isn't 4.0
                return;
            }

            setUpdatableFlagsMethod.Invoke(uriParser, new object[] { 0 });
        }

        //This is a workaround to fix a bug in .NET 4.0
        //The bug causes .NET to invalidate urls with a period 
        //in the folder/file level (trailing)
        //this is fixed in 4.5
        private Uri MyUri(string url)
        {
            //Uri uri = new Uri(url);
            System.Reflection.MethodInfo getSyntax = typeof(UriParser).GetMethod("GetSyntax", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            System.Reflection.FieldInfo flagsField = typeof(UriParser).GetField("m_Flags", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (getSyntax != null && flagsField != null)
            {
                foreach (string scheme in new[] { "http", "https" })
                {
                    UriParser parser = (UriParser)getSyntax.Invoke(null, new object[] { scheme });
                    if (parser != null)
                    {
                        int flagsValue = (int)flagsField.GetValue(parser);
                        // Clear the CanonicalizeAsFilePath attribute
                        if ((flagsValue & 0x1000000) != 0)
                            flagsField.SetValue(parser, flagsValue & ~0x1000000);
                    }
                }
            }
            Uri uri = new Uri(url);
            return uri;
        }

        public string ReadUri(string uri)
        {

            try
            {
                LeaveDotsAndSlashesEscaped();

                System.Net.WebRequest.DefaultWebProxy = null;
                
                System.Net.HttpWebRequest objRequest =
                (System.Net.HttpWebRequest)System.Net.WebRequest.Create(uri);
                //System.Net.HttpWebRequest objRequest =
                //(System.Net.HttpWebRequest)System.Net.WebRequest.Create(MyUri(uri));
                objRequest.Proxy = null;
                
                //if (cglobal.PublicProxy != null)
                //{
                //    objRequest.Proxy = cglobal.PublicProxy;
                //}
                objRequest.UserAgent = "MarcEdit 6.2 Headings Retrieval (caching)";
                objRequest.Proxy = null;
                objRequest.Accept = "*/*";
                
                //Changing the default timeout from 100 seconds to 30 seconds.
                objRequest.Timeout = 30000;

                System.Net.WebResponse objResponse = objRequest.GetResponse();

                System.IO.StreamReader reader = new System.IO.StreamReader(objResponse.GetResponseStream(), System.Text.Encoding.GetEncoding(1252));
                string tmpVal = reader.ReadToEnd().Trim();
                //System.Windows.Forms.MessageBox.Show(uri + "\n" + tmpVal);
                reader.Close();
                objResponse.Close();

                return tmpVal;
            }
            catch //(System.Exception xx)
            {
                //System.Windows.Forms.MessageBox.Show(uri + "\n" + xx.ToString());
                return "";
            }
        }

       

        public string NormalizeTerm(string term, bool onlySubfieldA=false, string customfield = "a")
        {
            string rterm = string.Empty;
            if (onlySubfieldA == false)
            {
                rterm = System.Text.RegularExpressions.Regex.Replace(term, @"\$[a-z]", " -- ");
                if (rterm.IndexOf("$") > -1)
                {
                    rterm = rterm.Substring(0, rterm.IndexOf("$"));
                }
                rterm = rterm.TrimEnd(".? ".ToCharArray());
            }
            else
            {
                string tmpterm = "";
                string[] tparts = term.Split("$".ToCharArray());
                foreach (string s in tparts)
                {
                    if (s.Trim().Length > 0)
                    {
                        if (s.Substring(0, 1).ToLower() == customfield)
                        {
                            tmpterm = s.Substring(1).TrimEnd();
                            break;
                        }
                    }
                }
                rterm = tmpterm.TrimEnd(".?, ".ToCharArray());
            }
            return rterm;
        }

        public string NormalizeTermEx(string term, string customfield = "a")
        {

            string tmpterm = "";
            string rterm = "";
            string[] tparts = term.Split("$".ToCharArray());
            foreach (string s in tparts)
            {
                if (s.Trim().Length > 0)
                {
                    for (int x = 0; x < customfield.Length; x++)
                    {
                        if (s.Substring(0, 1).ToLower() == customfield[x].ToString())
                        {
                            if (tmpterm.Length == 0)
                            {
                                tmpterm = s.Substring(1).TrimEnd();
                                break;
                            }
                            else
                            {
                                tmpterm += " -- " + s.Substring(1).TrimEnd();
                            }
                        }
                    }
                }
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(tmpterm, @"[A-Z]\.$") == false)
            {
                //this looks to see if the element is an abbreviation at the end.
                tmpterm = tmpterm.TrimEnd(".?,".ToCharArray());
            }
            else
            {
                tmpterm = tmpterm.TrimEnd("?, ".ToCharArray());
            }
            rterm = tmpterm; //.TrimEnd(".?, ".ToCharArray());
            return rterm;
        }
            
        public string NormalizeSubjectEx(string term, string subfields, bool onlySubfieldA = false)
        {
            //valid $a,$b,$v, $x, $y, $z
            string tmpterm = "";
            
            string[] tparts = term.Split("$".ToCharArray());
            foreach (string s in tparts)
            {
                if (s.Trim().Length > 0)
                {
                    for (int x = 0; x < subfields.Length; x++)
                    {
                        if (s.Substring(0, 1) == subfields[x].ToString())
                        {
                            if (tmpterm.Length == 0)
                            {
                                tmpterm = s.Substring(1).TrimEnd();
                            }
                            else
                            {
                                tmpterm += "--" + s.Substring(1).TrimEnd();
                            }
                        }
                    }
                }
            }


            if (System.Text.RegularExpressions.Regex.IsMatch(tmpterm, @"[A-Z]\.$") == false)
            {
                //this looks to see if the element is an abbreviation at the end.
                tmpterm = tmpterm.TrimEnd(".?,".ToCharArray());
            }
            else
            {
                tmpterm = tmpterm.TrimEnd("?, ".ToCharArray());
            }

            return tmpterm;
        }

        public string NormalizeSubject(string term, bool onlySubfieldA=false)
        {
            //valid $a,$b,$v, $x, $y, $z
            string tmpterm = "";
            string[] tparts = term.Split("$".ToCharArray());
            foreach (string s in tparts)
            {
                if (s.Trim().Length > 0)
                {
                    switch (s.Substring(0, 1).ToLower())
                    {
                        case "a":
                            tmpterm = s.Substring(1).TrimEnd();
                            break;
                        case "b":
                        case "v":
                        case "x":
                        case "y":
                        case "z":
                            if (onlySubfieldA == false)
                            {
                                tmpterm += "--" + s.Substring(1).TrimEnd();
                            }
                            break;
                    }
                }
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(tmpterm, @"[A-Z]\.$") == false)
            {
                //this looks to see if the element is an abbreviation at the end.
                tmpterm = tmpterm.TrimEnd(".?,".ToCharArray());
            }
            else
            {
                tmpterm = tmpterm.TrimEnd("?, ".ToCharArray());
            }
            
            return tmpterm;
        }

        public string NormalizeNameEx(string term, string subfields, bool bPersonal = true)
        {

            //Valid are $a,$b,$c$d$q
            //System.Windows.Forms.MessageBox.Show(term);
            string tmpterm = "";
            string[] tparts = term.Split("$".ToCharArray());
            foreach (string s in tparts)
            {
                if (s.Trim().Length > 0)
                {
                    switch (s.Substring(0, 1).ToLower())
                    {
                        case "a":
                            tmpterm = s.Substring(1).TrimEnd();

                            if (term.Substring(term.IndexOf("$a") + 2).IndexOf("$") == -1)
                            {
                                if (System.Text.RegularExpressions.Regex.IsMatch(tmpterm, @"[A-Z]\.$") == false)
                                {
                                    if (bPersonal == false)
                                    {
                                        //I need to come up with something a bit more refined -- maybe something like:
                                        if (System.Text.RegularExpressions.Regex.IsMatch(tmpterm, @", \w{1,3}.$") == false)
                                            tmpterm = tmpterm.TrimEnd(".".ToCharArray());
                                    } else
                                    {
                                        tmpterm = tmpterm.TrimEnd(".".ToCharArray());
                                    }
                                    //tmpterm = tmpterm.TrimEnd(".".ToCharArray());
                                }
                            }
                            //if (System.Text.RegularExpressions.Regex.IsMatch(tmpterm, "[A-Z]$"))
                            //{
                            //    tmpterm += ".";
                            //}
                            break;
                        default:
                            if (subfields.IndexOf(s.Substring(0, 1).ToLower()) > -1)
                            {
                                tmpterm += " " + s.Substring(1).TrimEnd();
                            }
                            break;
                    }
                }
            }

            if (tparts.Length > 2)
            {
                if (tmpterm.Trim().EndsWith(","))
                {
                    tmpterm = tmpterm.TrimEnd(", ".ToCharArray());
                }
                else
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(tmpterm, @"[A-Z]\.$"))
                    {
                        tmpterm = tmpterm.TrimEnd(",;: ".ToCharArray());
                    }
                    else
                    {
                        //I need to come up with something a bit more refined -- maybe something like:
                        if (System.Text.RegularExpressions.Regex.IsMatch(tmpterm, @", \w{1,3}.$") == false)
                        {
                            tmpterm = tmpterm.TrimEnd(".,;: ".ToCharArray());
                        } else
                        {
                            tmpterm = tmpterm.TrimEnd(",;: ".ToCharArray());
                        }
                    }
                }
            }

            //System.Windows.Forms.MessageBox.Show(tmpterm + "\n" + subfields);
            return tmpterm;
        }

        public string NormalizeName(string term, bool bPersonal = true)
        {

            //Valid are $a,$b,$c$d$q
            //System.Windows.Forms.MessageBox.Show(term);
            string tmpterm = "";
            string[] tparts = term.Split("$".ToCharArray());
            foreach (string s in tparts)
            {
                if (s.Trim().Length > 0)
                {
                    switch (s.Substring(0, 1).ToLower())
                    {
                        case "a":
                            tmpterm = s.Substring(1).TrimEnd();

                            if (term.Substring(term.IndexOf("$a") + 2).IndexOf("$") == -1)
                            {
                                if (System.Text.RegularExpressions.Regex.IsMatch(tmpterm, @"[A-Z]\.$") == false)
                                {
                                    if (bPersonal == false)
                                    {
                                        //I need to come up with something a bit more refined -- maybe something like:
                                        if (System.Text.RegularExpressions.Regex.IsMatch(tmpterm, @", \w{1,4}.$") == false)
                                            tmpterm = tmpterm.TrimEnd(".".ToCharArray());
                                    } else
                                    {
                                        tmpterm = tmpterm.TrimEnd(".".ToCharArray());
                                    }
                                }
                            }
                            //if (System.Text.RegularExpressions.Regex.IsMatch(tmpterm, "[A-Z]$"))
                            //{
                            //    tmpterm += ".";
                            //}
                            break;
                        case "b":
                        case "c":
                        case "d":
                        case "q":
                        case "n":
                        case "p":

                            tmpterm += " " + s.Substring(1).TrimEnd();
                            break;
                    }
                }
            }

            if (tparts.Length > 2)
            {
                if (tmpterm.Trim().EndsWith(","))
                {
                    tmpterm = tmpterm.TrimEnd(", ".ToCharArray());
                }
                else
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(tmpterm, @"[A-Z]\.$")){ 
                        tmpterm = tmpterm.TrimEnd(", ".ToCharArray());
                    } else {
                        if (System.Text.RegularExpressions.Regex.IsMatch(tmpterm, @", \w{1,3}.$") == false)
                        {
                            tmpterm = tmpterm.TrimEnd("., ".ToCharArray());
                        }
                        else
                        {
                            tmpterm = tmpterm.TrimEnd(", ".ToCharArray());
                        }
                    }
                }
            }
            return tmpterm;
        }

        public Newtonsoft.Json.Linq.JObject ParseJsonObject(string json)
        {
            Newtonsoft.Json.Linq.JObject o = Newtonsoft.Json.Linq.JObject.Parse(json);
            return o;
        }

        public System.Collections.Hashtable ParseJson(string json)
        {
            string[] sparts = json.Split("\"".ToCharArray());
            System.Collections.Hashtable sHash = new System.Collections.Hashtable();
            int index = 0;
            while (index + 4 <= sparts.Length)
            {
                if (sHash.ContainsKey(sparts[index + 1]) == false)
                {
                    sHash.Add(sparts[index + 1], sparts[index + 3]);
                }
                index = index + 4;
            }
            return sHash;
        }

        public string getLinks(string s, RSS_TYPES xml_type)
        {
            return getLinks(s, xml_type, String.Empty);
        }

        public string getLinks(string s, RSS_TYPES xml_type, string term)
        {
            System.Collections.Hashtable opts = new System.Collections.Hashtable();
            return getLinks(s, xml_type, term, ref opts);
        }

        public string getLinks(string s, RSS_TYPES xml_type, string term, ref System.Collections.Hashtable opts)
        {
            if (s.ToLower().IndexOf("<html") > -1)
            {
                //this isn't xml
                return "";
            }
            //System.Windows.Forms.MessageBox.Show(s);
            string id = String.Empty;
            string ret_Title = string.Empty;

            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(s);

            System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(doc.NameTable);
            System.Xml.XmlNode node = null;

            switch (xml_type)
            {
                case RSS_TYPES.atom:

                    nsmgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");

                    node = doc.SelectSingleNode("atom:feed/atom:entry[1]/atom:link/@href", nsmgr);
                    if (node != null)
                    {
                        id = node.InnerText;
                        //string[] id_parts = id.Split("/".ToCharArray());
                        //if (id.IndexOf("authorities/names") > -1)
                        //{
                        //    id = "(lccn)" + id_parts[id_parts.Length - 1];
                        //}
                        //else if (id.IndexOf("authorities/subjects") > -1)
                        //{
                        //    id = "(lcsh)" + id_parts[id_parts.Length-1];
                        //}
                    }

                    node = doc.SelectSingleNode("atom:feed/atom:entry[1]/atom:title", nsmgr);
                    if (node != null)
                    {
                        ret_Title = node.InnerText;
                        opts.Add("title", ret_Title);
                    }

                    break;
                case RSS_TYPES.rss:
                    nsmgr.AddNamespace("opensearch", "http://a9.com/-/spec/opensearch/1.1/");

                    if (term == String.Empty)
                    {
                        node = doc.SelectSingleNode("rss/channel/item/link", nsmgr);
                        if (node != null)
                        {
                            id = node.InnerText;
                            //string[] id_parts = id.Split("/".ToCharArray());
                            //if (id.IndexOf("authorities/names") > -1)
                            //{
                            //    id = "(lccn)" + id_parts[id_parts.Length - 1];
                            //}
                            //else if (id.IndexOf("authorities/subjects") > -1)
                            //{
                            //    id = "(lcsh)" + id_parts[id_parts.Length-1];
                            //}
                        }
                        break;
                    }
                    else {
                        System.Xml.XmlNodeList nlist = doc.SelectNodes("rss/channel/item");
                        if (nlist != null)
                        {
                            foreach (System.Xml.XmlNode snode in nlist)
                            {
                                string tterm = snode.SelectSingleNode("title").InnerText;
                                string tid = snode.SelectSingleNode("link").InnerText;

                                //System.Windows.Forms.MessageBox.Show(tterm + "\n" + tid);
                                //normalize the tterm and the term
                                if ((System.Text.RegularExpressions.Regex.Replace(tterm, @"\W", "").IndexOf(System.Text.RegularExpressions.Regex.Replace(term, @"\W", "")) > -1))
                                {
                                    id = tid;
                                    break;
                                } else if (System.Text.RegularExpressions.Regex.Replace(term, @"\W", "").IndexOf(System.Text.RegularExpressions.Regex.Replace(tterm, @"\W", "")) > -1) 
                                {
                                    id = tid;
                                    break;
                                }

                            }
                        }
                        break;
                    }
                    
            }
            return id;
        }
    }
}
