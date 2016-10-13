using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Writing.Formatting;
using VDS.RDF.Writing;

namespace melinked_data
{
    public class sparqlservice
    {
        private const string ID_NLM_SPARQL_URL = "http://id.nlm.nih.gov/mesh/sparql?inference=true";
        private string pError = "";
        public enum LinkResolverService
        {
            ID_NLM_GOV = 1,
        }

        public string GetError
        {
            get { return pError; }
            set { pError = value; }
        }

        public string ResolveTerm(string term, LinkResolverService service, string field = "650")
        {
            string tmp = String.Empty;
            string squery = BuildQuery(term, service,field);
            switch (service)
            {
                case LinkResolverService.ID_NLM_GOV:
                    //System.Windows.Forms.Clipboard.SetText(tmp);
                    //System.Windows.Forms.MessageBox.Show(tmp);
                    try
                    {
                        SparqlResultSet rtmp = ReadEndPoint(sparqlservice.ID_NLM_SPARQL_URL, squery);
                        tmp = getLinks(rtmp, 0);
                    }
                    catch
                    {
                        GetError = squery;
                    }

                    break;
            }
            return tmp;
        }

        private string BuildQuery(string term, LinkResolverService service, string field="650")
        {
            string q = "";
            switch (service)
            {
                case LinkResolverService.ID_NLM_GOV:
                    //string descriptorLabel = "meshv:Descriptor";
                    //string descriptorpairlabel = "meshv:DescriptorQualifierPair";
                    //string geodescriptorlabel = "meshv:GeographicalDescriptor";
                    //string search_label = descriptorLabel;
                    if (term.IndexOf("--") > -1)
                    {
                        //term = term.Substring(0, term.IndexOf("--")).TrimEnd();
                        term = term.Replace("--", "/");
                        //search_label = descriptorpairlabel;
                    }

                    if (field == "651")
                    {
                        //search_label = geodescriptorlabel;
                    }

                    if (term.IndexOf("(") > -1)
                    {
                        term = term.Replace("(", @"\\(").Replace(")", @"\\)");
                    }

                    q = "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>\n" + 
                        "PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>\n" + 
                        "PREFIX meshv: <http://id.nlm.nih.gov/mesh/vocab#>\n" + 
                        "PREFIX mesh: <http://id.nlm.nih.gov/mesh/>\n" + 
                        "\n" + 
                        "SELECT *\n" + 
                        "FROM <http://id.nlm.nih.gov/mesh>\n" + 
                        "WHERE {\n" + 
                        "?d a meshv:Descriptor .\n" +
                        "?d rdfs:label \"" + System.Uri.EscapeDataString(term) + "\"@en\n" +
                        "}"; 

                    //q = "PREFIX rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#>\n" +
                    //    "PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>\n" +
                    //    "PREFIX xsd: <http://www.w3.org/2001/XMLSchema#>\n" +
                    //    "PREFIX owl: <http://www.w3.org/2002/07/owl#>\n" +
                    //    "PREFIX meshv: <http://id.nlm.nih.gov/mesh/vocab#>\n" +
                    //    "PREFIX mesh: <http://id.nlm.nih.gov/mesh/>\n" +
                    //    "SELECT distinct ?d ?dName \n" +
                    //    "FROM <http://id.nlm.nih.gov/mesh>\n" +
                    //    "WHERE {\n" +
                    //    "  ?d a " + search_label + " .\n" +
                    //    "  ?d rdfs:label ?dName \n" + 
                    //    "  FILTER(REGEX(?dName,'" + term + "','i')) \n" + 
                    //    "} \n" +
                    //    "ORDER BY ?dLabel";
                    break;
            }

            return q;
        }


        public SparqlResultSet ReadEndPoint(string uri, string spQuery)
        {

            System.Uri objURI = new Uri(uri);            
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(objURI);
            SparqlResultSet results = endpoint.QueryWithResultSet(spQuery);

            return results;


        }

        public string NormalizeTerm(string term)
        {
            string rterm = System.Text.RegularExpressions.Regex.Replace(term, @"\$[a-z]", " -- ");
            if (rterm.IndexOf("$") > -1)
            {
                rterm = rterm.Substring(0, rterm.IndexOf("$"));
            }
            rterm = rterm.TrimEnd(".? ".ToCharArray());
            return rterm;
        }

        public string NormalizeSubject(string term)
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
                            tmpterm += "--" + s.Substring(1).TrimEnd();
                            break;
                    }
                }
            }

            tmpterm = tmpterm.TrimEnd(".?, ".ToCharArray());

            return tmpterm;
        }

        public string NormalizeName(string term)
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
                            break;
                        case "b":
                        case "c":
                        case "d":
                        case "q":

                            tmpterm += " " + s.Substring(1).TrimEnd();
                            break;
                    }
                }
            }

            tmpterm = tmpterm.TrimEnd(".?, ".ToCharArray());
            return tmpterm;
        }



        public string getLinks(SparqlResultSet rset, int rindex)
        {

            string url = "";
            string label = "";

            foreach (SparqlResult r in rset)
            {
                url = r[0].ToString();
                label = r[1].ToString();
                break;
            }

            if (url.Trim().Length > 0)
            {
                return url;
            }

            return "";
        }
    }
}
