using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BSMulti_Installer2
{
    public partial class Form2 : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        public string multiselected = "";
        public bool currentlyinstallinguninstalling = false;
        public bool allowinstalluninstall = false;
        public string bsl { get; set; }

        public Form2()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        private void panel1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (currentlyinstallinguninstalling == false)
            {
                multiselected = "a";
                pictureBox1.Image = BSMulti_Installer2.Properties.Resources.tick;
                button3.BackColor = SystemColors.MenuHighlight;
                button4.BackColor = SystemColors.MenuHighlight;
                button1.BackColor = Color.Green;
                button2.BackColor = SystemColors.MenuHighlight;
                allowinstalluninstall = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (currentlyinstallinguninstalling == false)
            {
                multiselected = "z";
                pictureBox1.Image = BSMulti_Installer2.Properties.Resources.tick;
                button3.BackColor = SystemColors.MenuHighlight;
                button4.BackColor = SystemColors.MenuHighlight;
                button2.BackColor = Color.Green;
                button1.BackColor = SystemColors.MenuHighlight;
                allowinstalluninstall = true;
            }
        }

        void InstallMulti()
        {
                statuslabel.Text = "Status: Preparing";
            progressBar1.Value = 10;
                allowinstalluninstall = false;
                currentlyinstallinguninstalling = true;
                button3.BackColor = SystemColors.GrayText;
                button4.BackColor = SystemColors.GrayText;
                Directory.CreateDirectory("Files");
                DirectoryInfo di = new DirectoryInfo("Files");
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
                Directory.CreateDirectory(@"Files\multiplayer");
                Directory.CreateDirectory(@"Files\dovr");
                Directory.CreateDirectory(@"Files\ca");
                Directory.CreateDirectory(@"Files\dc");
                Directory.CreateDirectory(@"Files\dep");
                statuslabel.Text = "Status: Downloading Multiplayer 1/6";
            progressBar1.Value = 20;
            using (var wc = new WebClient())
                {
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadCompleted);
                    wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                    if(multiselected == "a") {
                        if(File.Exists(bsl + @"\Plugins\BeatSaberMultiplayerLite.dll"))
                        {
                        statuslabel.Text = "Status: Failed";
                        allowinstalluninstall = true;
                        currentlyinstallinguninstalling = false;
                        button3.BackColor = SystemColors.MenuHighlight;
                        button4.BackColor = SystemColors.MenuHighlight;
                        MessageBox.Show("Beat Saber Multiplayer Lite is installed! Installation Failed. Please Uninstall Zingabopp's Multiplayer Lite to continue installing Andruzzzhka's Multiplayer", "Uh Oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            wc.DownloadFileAsync(new System.Uri("https://tigersserver.xyz/andruzzzhkalatest"), AppDomain.CurrentDomain.BaseDirectory + @"\Files\multiplayer.zip");
                        }
                    }
                    else if(multiselected == "z")
                    {
                        if (File.Exists(bsl + @"\Plugins\BeatSaberMultiplayer.dll"))
                        {
                        statuslabel.Text = "Status: Failed";
                        allowinstalluninstall = true;
                        currentlyinstallinguninstalling = false;
                        button3.BackColor = SystemColors.MenuHighlight;
                        button4.BackColor = SystemColors.MenuHighlight;
                        MessageBox.Show("Beat Saber Multiplayer is installed! Installation Failed. Please Uninstall Andruzzzhka's Multiplayer to continue installing Zingabopp's Multiplayer Lite", "Uh Oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            wc.DownloadFileAsync(new System.Uri("https://tigersserver.xyz/zingabopplatest"), AppDomain.CurrentDomain.BaseDirectory + @"\Files\multiplayer.zip");
                        }
                    }
                }
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar2.Value = e.ProgressPercentage;
        }

        void wc_DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            statuslabel.Text = "Status: Downloading CA 2/6";
            progressBar1.Value = 30;
            using (var wc = new WebClient())
            {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadCompletedca);
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri("https://tigersserver.xyz/customavatars"), AppDomain.CurrentDomain.BaseDirectory + @"\Files\ca.zip");
            }
        }

        void wc_DownloadCompletedca(object sender, AsyncCompletedEventArgs e)
        {
            statuslabel.Text = "Status: Downloading DOVR 3/6";
            progressBar1.Value = 40;
            using (var wc = new WebClient())
            {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadCompleteddovr);
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri("https://tigersserver.xyz/dynamicopenvr"), AppDomain.CurrentDomain.BaseDirectory + @"\Files\dovr.zip");
            }
        }

        void wc_DownloadCompleteddovr(object sender, AsyncCompletedEventArgs e)
        {
            statuslabel.Text = "Status: Downloading DC 4/6";
            progressBar1.Value = 50;
            using (var wc = new WebClient())
            {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadCompleteddc);
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri("https://tigersserver.xyz/discordcore"), AppDomain.CurrentDomain.BaseDirectory + @"\Files\dc.zip");
            }
        }

        void wc_DownloadCompleteddc(object sender, AsyncCompletedEventArgs e)
        {
            statuslabel.Text = "Status: Downloading CustomAvatar.dll 5/6";
            progressBar1.Value = 60;
            using (var wc = new WebClient())
            {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadCompletedcadll);
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri("https://tigersserver.xyz/cadll"), AppDomain.CurrentDomain.BaseDirectory + @"\Files\CustomAvatar.dll");
            }
        }

        void wc_DownloadCompletedcadll(object sender, AsyncCompletedEventArgs e)
        {
            statuslabel.Text = "Status: Downloading DEP 6/6";
            progressBar1.Value = 70;
            using (var wc = new WebClient())
            {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadCompleteddep);
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileAsync(new System.Uri("https://tigersserver.xyz/dep"), AppDomain.CurrentDomain.BaseDirectory + @"\Files\dep.zip");
            }
        }

        void wc_DownloadCompleteddep(object sender, AsyncCompletedEventArgs e)
        {
            InstallMultiContinued();
        }

        void InstallMultiContinued()
        {
                statuslabel.Text = "Status: Extracting Files";
            progressBar1.Value = 80;
            DirectoryInfo di = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\Files");
                foreach (FileInfo file in di.GetFiles())
                {
                    string[] splitdot = file.Name.Split('.');
                    if (splitdot[1] == "zip")
                    {
                        ZipFile.ExtractToDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\Files\" + splitdot[0] + @".zip", @"Files\" + splitdot[0]);
                    }
                }
                statuslabel.Text = "Status: Moving Files";
            progressBar1.Value = 90;
            foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    if(dir.Name == "ca")
                    {
                        DirectoryInfo cadi = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\Files\ca");
                        if (Directory.Exists(bsl + @"\CustomAvatars")) { }
                        else
                        {
                            Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\Files\ca\CustomAvatars", bsl + @"\CustomAvatars");
                        }
                        if (Directory.Exists(bsl + @"\DynamicOpenVR")) { }
                        else
                        {
                            Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\Files\ca\DynamicOpenVR", bsl + @"\DynamicOpenVR");
                        }
                        foreach (DirectoryInfo cadir in cadi.GetDirectories())
                        {
                            if (cadir.Name == "Plugins")
                            {
                                // Don't move CustomAvatar's DLL
                            }
                        }
                    }
                    if(dir.Name == "dc")
                    {
                        DirectoryInfo dcdi = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"Files\dc");
                        foreach (DirectoryInfo dcdir in dcdi.GetDirectories())
                        {
                            if (dcdir.Name == "Plugins")
                            {
                                foreach (FileInfo file in dcdir.GetFiles())
                                {
                                    if (File.Exists(bsl + @"\Plugins\" + file.Name)) { }
                                    else
                                    {
                                        File.Move(file.FullName, bsl + @"\Plugins\" + file.Name);
                                    }
                                }
                            }
                            if (dcdir.Name == "Libs")
                            {
                                foreach (DirectoryInfo dcnativedir in dcdir.GetDirectories())
                                {
                                    if (Directory.Exists(bsl + @"\Libs\Native")) { }
                                    else
                                    {
                                        Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\Files\dc\Libs\Native", bsl + @"\Libs\Native");
                                    }
                                }
                            }
                        }
                    }
                    if(dir.Name == "dep")
                    {
                        DirectoryInfo depdi = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"Files\dep\dep");
                        foreach (DirectoryInfo depdir in depdi.GetDirectories())
                        {
                            if (depdir.Name == "Plugins")
                            {
                                foreach (FileInfo file in depdir.GetFiles())
                                {
                                    if (File.Exists(bsl + @"\Plugins\" + file.Name)) { }
                                    else
                                    {
                                        File.Move(file.FullName, bsl + @"\Plugins\" + file.Name);
                                    }
                                }
                            }
                        }
                    }
                    if (dir.Name == "dovr")
                    {
                        DirectoryInfo dovrdi = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"Files\dovr");
                        foreach (DirectoryInfo dovrdir in dovrdi.GetDirectories())
                        {
                            if (dovrdir.Name == "Plugins")
                            {
                                foreach (FileInfo file in dovrdir.GetFiles())
                                {
                                    if (File.Exists(bsl + @"\Plugins\" + file.Name)) { }
                                    else
                                    {
                                        File.Move(file.FullName, bsl + @"\Plugins\" + file.Name);
                                    }
                                }
                            }
                            if (dovrdir.Name == "Libs")
                            {
                                foreach (FileInfo file in dovrdir.GetFiles())
                                {
                                    if (File.Exists(bsl + @"\Libs\" + file.Name)) { }
                                    else
                                    {
                                        File.Move(file.FullName, bsl + @"\Libs\" + file.Name);
                                    }
                                }
                            }
                        }
                    }
                    if (dir.Name == "multiplayer")
                    {
                        DirectoryInfo multiplayerdi = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"Files\multiplayer");
                        foreach (DirectoryInfo multiplayerdir in multiplayerdi.GetDirectories())
                        {
                            if (multiplayerdir.Name == "Plugins")
                            {
                                foreach (FileInfo file in multiplayerdir.GetFiles())
                                {
                                    if (File.Exists(bsl + @"\Plugins\" + file.Name)) { }
                                    else
                                    {
                                        File.Move(file.FullName, bsl + @"\Plugins\" + file.Name);
                                    }
                                }
                            }
                            if (multiplayerdir.Name == "Libs")
                            {
                                foreach (FileInfo file in multiplayerdir.GetFiles())
                                {
                                    if (File.Exists(bsl + @"\Libs\" + file.Name)) { }
                                    else
                                    {
                                        File.Move(file.FullName, bsl + @"\Libs\" + file.Name);
                                    }  
                                }
                            }
                        }
                    }
                }
            if(multiselected == "a")
            {
                if (File.Exists(@"Files\CustomAvatar.dll"))
                {
                    if (File.Exists(bsl + @"\Plugins\CustomAvatar.dll"))
                    {
                        File.Delete(bsl + @"\Plugins\CustomAvatar.dll");
                        File.Move(@"Files\CustomAvatar.dll", bsl + @"\Plugins\CustomAvatar.dll");
                    }
                    else
                    {
                        File.Move(@"Files\CustomAvatar.dll", bsl + @"\Plugins\CustomAvatar.dll");
                    }
                }
            }
                
                statuslabel.Text = "Status: Complete!";
            progressBar1.Value = 100;
            allowinstalluninstall = true;
                currentlyinstallinguninstalling = false;
                button3.BackColor = SystemColors.MenuHighlight;
                button4.BackColor = SystemColors.MenuHighlight;
                DialogResult dialogResult = MessageBox.Show("Multiplayer is installed! Would you like to exit?", "Complete!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    Application.Exit();
                }
        }

        void UninstallMulti()
        {
            bool continuewithuninstall = false;
            statuslabel.Text = "Status: Preparing";
            progressBar1.Value = 25;
            allowinstalluninstall = false;
            currentlyinstallinguninstalling = true;
            button3.BackColor = SystemColors.GrayText;
            button4.BackColor = SystemColors.GrayText;
            statuslabel.Text = "Status: Uninstalling Multiplayer";
            progressBar1.Value = 50;
            if (multiselected == "a")
            {
                if(File.Exists(bsl + @"\Plugins\BeatSaberMultiplayer.dll"))
                {
                    File.Delete(bsl + @"\Plugins\BeatSaberMultiplayer.dll");
                    continuewithuninstall = true;
                }
                else
                {
                    DialogResult dialogResult2 = MessageBox.Show("Multiplayer was not found! Would you like to continue?", "Uh Oh!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if(dialogResult2 == DialogResult.Yes)
                    {
                        continuewithuninstall = true;
                    }
                    else
                    {
                        continuewithuninstall = false; 
                    }
                }
            }
            if (multiselected == "z")
            {
                if (File.Exists(bsl + @"\Plugins\BeatSaberMultiplayerLite.dll"))
                {
                    File.Delete(bsl + @"\Plugins\BeatSaberMultiplayerLite.dll");
                    continuewithuninstall = true;
                }
                else
                {
                    DialogResult dialogResult2 = MessageBox.Show("Multiplayer Lite was not found! Would you like to continue?", "Uh Oh!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dialogResult2 == DialogResult.Yes)
                    {
                        continuewithuninstall = true;
                    }
                    else
                    {
                        continuewithuninstall = false;
                    }
                }
            }
            statuslabel.Text = "Status: Uninstalling Dependencies";
            progressBar1.Value = 75;
            if (continuewithuninstall == true)
            {
                if(checkBox1.Checked == true)
                {
                    if(File.Exists(bsl + @"\Plugins\SongCore.dll"))
                    {
                        File.Delete(bsl + @"\Plugins\SongCore.dll");
                    }
                }
                if(checkBox2.Checked == true)
                {
                    if (File.Exists(bsl + @"\Plugins\BSML.dll"))
                    {
                        File.Delete(bsl + @"\Plugins\BSML.dll");
                    }
                }
                if(checkBox3.Checked == true)
                {
                    if (File.Exists(bsl + @"\Plugins\BS_Utils.dll"))
                    {
                        File.Delete(bsl + @"\Plugins\BS_Utils.dll");
                    }
                }
                if(checkBox4.Checked == true)
                {
                    if (File.Exists(bsl + @"\Plugins\CustomAvatar.dll"))
                    {
                        File.Delete(bsl + @"\Plugins\CustomAvatar.dll");
                    }
                    Directory.Delete(bsl + @"\DynamicOpenVR", true);
                }
                if(checkBox5.Checked == true)
                {
                    if (File.Exists(bsl + @"\Plugins\DiscordCore.dll"))
                    {
                        File.Delete(bsl + @"\Plugins\DiscordCore.dll");
                    }
                    Directory.Delete(bsl + @"\Libs\Native", true);
                }
                if(checkBox6.Checked == true)
                {
                    if (File.Exists(bsl + @"\Plugins\DynamicOpenVR.manifest"))
                    {
                        File.Delete(bsl + @"\Plugins\DynamicOpenVR.manifest");
                    }
                    if (File.Exists(bsl + @"\Libs\DynamicOpenVR.dll"))
                    {
                        File.Delete(bsl + @"\Libs\DynamicOpenVR.dll");
                    }
                }
            }
            statuslabel.Text = "Status: Complete!";
            progressBar1.Value = 100;
            allowinstalluninstall = true;
            currentlyinstallinguninstalling = false;
            button3.BackColor = SystemColors.MenuHighlight;
            button4.BackColor = SystemColors.MenuHighlight;
            DialogResult dialogResult = MessageBox.Show("Multiplayer is uninstalled :( Would you like to exit?", "Complete!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void closeForm1_Click(object sender, EventArgs e)
        {
            Form1 f1 = new Form1();
            f1.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (allowinstalluninstall)
            {
                progressBar1.Value = 0;
                InstallMulti();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (allowinstalluninstall)
            {
                progressBar1.Value = 0;
                UninstallMulti();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/200Tigersbloxed/BSMulti-Installer/wiki/Which-Multiplayer-Should-I-Install%3F");
        }
    }
}
