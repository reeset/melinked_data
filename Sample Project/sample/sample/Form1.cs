using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void cmdProcess_Click(object sender, EventArgs e)
        {
            System.Collections.Hashtable hOptions = new System.Collections.Hashtable();
            hOptions["VIAF"] = false;
            hOptions["VIAFINDEX"] = "";
            hOptions["LCID"] = true;
            hOptions["OCLCWORKID"] = false;
            hOptions["AUTOSUBJECT"] = true;
            hOptions["OCLCFIELD"] = "001";
            hOptions["F3xx"] = true;

            hOptions["RULESFILE"] = txtRules.Text;
            int pCount = 0;

            BibFrameTools objF = new BibFrameTools();
            lbStatus.Text = "Beginning Process...please wait.";
            Application.DoEvents();
            string resultsfile = objF.BuildLinks(txtTerm.Text, txtResults.Text, hOptions);
            lbStatus.Text = "";
           
            System.Windows.Forms.MessageBox.Show("finished");                
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtTerm.Text = System.IO.Path.GetFullPath("../../resources" + System.IO.Path.DirectorySeparatorChar.ToString()) + "sample.mrk";
            txtRules.Text = System.IO.Path.GetFullPath("../../resources" + System.IO.Path.DirectorySeparatorChar.ToString()) + "linked_data_profile.xml";
            txtResults.Text = System.IO.Path.GetFullPath("../../resources" + System.IO.Path.DirectorySeparatorChar.ToString()) + "linked.txt";
        }
    }
}
