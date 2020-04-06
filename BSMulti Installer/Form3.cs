using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.IO.Compression;

namespace BSMulti_Installer
{
    public partial class Form3 : Form
    {
        public string bsdir;
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            label3.Text = "Status: Initializing 1/5";
            progressBar1.Value = 20;
            Directory.CreateDirectory("ZingaboppFiles");
            DirectoryInfo di = new DirectoryInfo("ZingaboppFiles");
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
            label3.Text = "Status: Downloading File 2/5";
            progressBar1.Value = 40;
            using (var wc = new WebClient())
            {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadCompleted);
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri("https://tigersserver.xyz/zingabopplatest"), AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\multiplayer.zip");
            }
        }
        
        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar2.Value = e.ProgressPercentage;
        }

        void wc_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            label3.Text = "Status: Un-packing ZIP file 3/5";
            progressBar1.Value = 60;
            ZipFile.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\multiplayer.zip", "ZingaboppFiles");
            label3.Text = "Status: Moving Lib Files 4/5";
            progressBar1.Value = 80;
            System.IO.DirectoryInfo diLibs = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\Libs");
            foreach (FileInfo file in diLibs.GetFiles())
            {
                if (File.Exists(bsdir + @"\Libs\" + file.Name))
                {
                    
                }
                else
                {
                    file.MoveTo(bsdir + @"\Libs\" + file.Name);
                }
            }
            label3.Text = "Status: Moving Plugin Files 5/5";
            System.IO.DirectoryInfo diPlugins = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\Plugins");
            foreach (FileInfo file in diPlugins.GetFiles())
            {
                if (File.Exists(bsdir + @"\Plugins\" + file.Name))
                {

                }
                else
                {
                    file.MoveTo(bsdir + @"\Plugins\" + file.Name);
                }
            }
            label3.Text = "Status: Done!";
            progressBar1.Value = 100;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();

            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog1.SelectedPath;
                if(File.Exists(selectedPath + @"\Beat Saber.exe"))
                {
                    button1.Visible = true;
                    bsdir = selectedPath;
                }
                else
                {
                    MessageBox.Show("Beat Saber was not found in this location!", "Uh Oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form4 f4 = new Form4();
            f4.Show();
        }
    }
}
