using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO.Compression;

namespace BSMulti_Installer
{
    public partial class Form5 : Form
    {
        public string bsdir;
        public bool adl = false;
        public bool dovr = false;
        public bool ca = false;
        public bool dc = false;
        public bool cadll = false;
        public Form5()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form4 f4 = new Form4();
            f4.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog1.SelectedPath;
                if (File.Exists(selectedPath + @"\Beat Saber.exe"))
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

        private void button1_Click(object sender, EventArgs e)
        {
            label3.Text = "Status: Initializing 1/7";
            progressBar1.Value = 14;
            Directory.CreateDirectory("AndruzzzhkaFiles");
            DirectoryInfo di = new DirectoryInfo("AndruzzzhkaFiles");
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
            Directory.CreateDirectory(@"AndruzzzhkaFiles\multiplayer");
            Directory.CreateDirectory(@"AndruzzzhkaFiles\dovr");
            Directory.CreateDirectory(@"AndruzzzhkaFiles\ca");
            Directory.CreateDirectory(@"AndruzzzhkaFiles\dc");
            label3.Text = "Status: Downloading Files 2/7";
            progressBar1.Value = 28;
            using (var wc = new WebClient())
            {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadCompleted);
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri("https://tigersserver.xyz/andruzzzhkalatest"), AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\multiplayer.zip");
            }
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar2.Value = e.ProgressPercentage;
        }

        void wc_DownloadCompleteddovr(object sender, AsyncCompletedEventArgs e)
        {
            using (var wc = new WebClient())
            {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadCompletedca);
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri("https://tigersserver.xyz/customavatars"), AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\ca.zip");
            }
            ca = true;
        }

        void wc_DownloadCompletedca(object sender, AsyncCompletedEventArgs e)
        {
            using (var wc = new WebClient())
            {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadCompleteddc);
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri("https://tigersserver.xyz/discordcore"), AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\dc.zip");
            }
            dc = true;
        }

        void wc_DownloadCompleteddc(object sender, AsyncCompletedEventArgs e)
        {
            using (var wc = new WebClient())
            {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadCompletedcadll);
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri("https://tigersserver.xyz/cadll"), AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\CustomAvatar.dll");
            }
            cadll = true;
        }

        void wc_DownloadCompletedcadll(object sender, AsyncCompletedEventArgs e)
        {
            if (dovr == true && ca == true && dc == true && cadll == true)
            {
                label3.Text = "Status: Un-packing ZIP files 3/7";
                progressBar1.Value = 42;
                ZipFile.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\multiplayer.zip", @"AndruzzzhkaFiles\multiplayer");
                ZipFile.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\dovr.zip", @"AndruzzzhkaFiles\dovr");
                ZipFile.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\ca.zip", @"AndruzzzhkaFiles\ca");
                ZipFile.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\dc.zip", @"AndruzzzhkaFiles\dc");
                label3.Text = "Status: Moving Lib Files 4/7";
                progressBar1.Value = 56;
                System.IO.DirectoryInfo dimultiplayerLibs = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\multiplayer\Libs");
                foreach (FileInfo file in dimultiplayerLibs.GetFiles())
                {
                    if (File.Exists(bsdir + @"\Libs\" + file.Name))
                    {

                    }
                    else
                    {
                        file.MoveTo(bsdir + @"\Libs\" + file.Name);
                    }
                }
                System.IO.DirectoryInfo didovrLibs = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\dovr\Libs");
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
                System.IO.DirectoryInfo didcLibs = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\dc\Libs");
                if (Directory.Exists(bsdir + @"\Libs\Native"))
                {

                }
                else
                {
                    Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\dc\Libs\Native", bsdir + @"\Libs\Native");
                }
                foreach (FileInfo file in didcLibs.GetFiles())
                {
                    if (File.Exists(bsdir + @"\Libs\" + file.Name))
                    {

                    }
                    else
                    {
                        file.MoveTo(bsdir + @"\Libs\" + file.Name);
                    }
                }
                label3.Text = "Status: Moving Plugin Files 5/7";
                System.IO.DirectoryInfo dimultiplayerPlugins = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\multiplayer\Plugins");
                foreach (FileInfo file in dimultiplayerPlugins.GetFiles())
                {
                    if (File.Exists(bsdir + @"\Plugins\" + file.Name))
                    {

                    }
                    else
                    {
                        file.MoveTo(bsdir + @"\Plugins\" + file.Name);
                    }
                }
                System.IO.DirectoryInfo didovrPlugins = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\dovr\Plugins");
                foreach (FileInfo file in didovrPlugins.GetFiles())
                {
                    if (File.Exists(bsdir + @"\Plugins\" + file.Name))
                    {

                    }
                    else
                    {
                        file.MoveTo(bsdir + @"\Plugins\" + file.Name);
                    }
                }
                System.IO.DirectoryInfo didcPlugins = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\dc\Plugins");
                foreach (FileInfo file in didcPlugins.GetFiles())
                {
                    if (File.Exists(bsdir + @"\Plugins\" + file.Name))
                    {

                    }
                    else
                    {
                        file.MoveTo(bsdir + @"\Plugins\" + file.Name);
                    }
                }
                if (File.Exists(bsdir + @"\Plugins\CustomAvatar.dll"))
                {
                    File.Delete(bsdir + @"\Plugins\CustomAvatar.dll");
                    File.Move(AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\CustomAvatar.dll", bsdir + @"\Plugins\CustomAvatar.dll");
                }
                else
                {
                    File.Move(AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\CustomAvatar.dll", bsdir + @"\Plugins\CustomAvatar.dll");
                }
                label3.Text = "Status: Checking for CustomAvatars 6/7";
                progressBar1.Value = 70;
                if (Directory.Exists(bsdir + @"\CustomAvatars"))
                {
                    if (Directory.Exists(bsdir + @"\CustomAvatars\Shaders"))
                    {

                    }
                    else
                    {
                        Directory.Move(AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\ca\CustomAvatars\Shaders", bsdir + @"\CustomAvatars\Shaders");
                    }
                }
                else
                {
                    Directory.Move(AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\ca\CustomAvatars", bsdir + @"\CustomAvatars");
                }
                label3.Text = "Status: Installing DynamicOpenVR 7/7";
                progressBar1.Value = 84;
                if (Directory.Exists(bsdir + @"\DynamicOpenVR"))
                {

                }
                else
                {
                    Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\ca\DynamicOpenVR", bsdir + @"\DynamicOpenVR");
                }
                label3.Text = "Status: Done!";
                progressBar1.Value = 100;
            }
        }

        void wc_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            adl = true;
            if(dovr == false)
            {
                using (var wc = new WebClient())
                {
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadCompleteddovr);
                    wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                    wc.DownloadFileAsync(new System.Uri("https://tigersserver.xyz/dynamicopenvr"), AppDomain.CurrentDomain.BaseDirectory + @"\AndruzzzhkaFiles\dovr.zip");
                }
                dovr = true;
            } 
        }
    }
}
