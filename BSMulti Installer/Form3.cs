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
            Directory.CreateDirectory(@"ZingaboppFiles\multiplayer");
            Directory.CreateDirectory(@"ZingaboppFiles\ca");
            Directory.CreateDirectory(@"ZingaboppFiles\dovr");
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
            using (var wc = new WebClient())
            {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_Completedcadll);
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri("https://tigersserver.xyz/customavatars"), AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\ca.zip");
            }
        }

        void wc_Completedcadll(object sender, AsyncCompletedEventArgs e)
        {
            using (var wc = new WebClient())
            {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_Completeddovr);
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri("https://tigersserver.xyz/cadll"), AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\CustomAvatar.dll");
            }
        }

        void wc_Completeddovr(object sender, AsyncCompletedEventArgs e)
        {
            using (var wc = new WebClient())
            {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_Completedca);
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri("https://tigersserver.xyz/dynamicopenvr"), AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\dovr.zip");
            }
        }

        void wc_Completedca(object sender, AsyncCompletedEventArgs e)
        {
            label3.Text = "Status: Un-packing ZIP file 3/5";
            progressBar1.Value = 60;
            ZipFile.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\multiplayer.zip", @"ZingaboppFiles\multiplayer");
            ZipFile.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\ca.zip", @"ZingaboppFiles\ca");
            ZipFile.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\dovr.zip", @"ZingaboppFiles\dovr");
            label3.Text = "Status: Moving Lib Files 4/5";
            progressBar1.Value = 80;
            System.IO.DirectoryInfo diLibs = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\multiplayer\Libs");
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
            System.IO.DirectoryInfo didovrLibs = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\dovr\Libs");
            foreach (FileInfo file in didovrLibs.GetFiles())
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
            System.IO.DirectoryInfo diPlugins = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\multiplayer\Plugins");
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
            System.IO.DirectoryInfo didovrPlugins = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\dovr\Plugins");
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
            if(File.Exists(bsdir + @"\Plugins\CustomAvatar.dll"))
            {
                File.Delete(bsdir + @"\Plugins\CustomAvatar.dll");
                File.Move(AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\CustomAvatar.dll", bsdir + @"\Plugins\CustomAvatar.dll");
            }
            else
            {
                File.Move(AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\CustomAvatar.dll", bsdir + @"\Plugins\CustomAvatar.dll");
            }
            if (Directory.Exists(bsdir + @"\DynamicOpenVR"))
            {

            }
            else
            {
                Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\ca\DynamicOpenVR", bsdir + @"\DynamicOpenVR");
            }
            if (Directory.Exists(bsdir + @"\CustomAvatars"))
            {
                if (Directory.Exists(bsdir + @"\CustomAvatars\Shaders"))
                {

                }
                else
                {
                    Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\ca\CustomAvatars\Shaders", bsdir + @"\CustomAvatars\Shaders");
                }
            }
            else
            {
                Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\ZingaboppFiles\ca\CustomAvatars", bsdir + @"\CustomAvatars");
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
                    button3.Visible = true;
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

        private void button3_Click(object sender, EventArgs e)
        {
            Form6 f6 = new Form6();
            f6.bsdir = bsdir;
            f6.FormClosed += new FormClosedEventHandler(Form6Closed);
            f6.Show();
            this.Hide();
        }

        void Form6Closed(object sender, FormClosedEventArgs e)
        {
            this.Show();
        }
    }
}
