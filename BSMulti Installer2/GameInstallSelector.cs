using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Net;
using Newtonsoft.Json;
using BSMulti_Installer2.Utilities;

namespace BSMulti_Installer2
{
    public partial class GameInstallSelector : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool ReleaseCapture();

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

        public bool userownssteam = false;
        public bool userownsoculus = false;
        string steaminstallpath = "";
        string oculusinstallpath = "";
        public string bsl;
        public bool allownext = false;
        public string version = "v2.0.5";

        public GameInstallSelector()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // check for internet
            if (CheckForInternetConnection() == true) { }
            else
            {
                MessageBox.Show("An Internet Connection is required!", "Not Connected to Internet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            // check if user can access website
            if (CheckForWebsiteConnection() == true) { }
            else
            {
                MessageBox.Show("Failed to connect to https://tigersserver.xyz. Please try again soon.", "Failed to Connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://pastebin.com/raw/S8v9a7Ba");
            StreamReader reader = new StreamReader(stream);
            String content = reader.ReadToEnd();
            if(version != content)
            {
                DialogResult drUpdate = MessageBox.Show("BSMulti-Installer is not up to date! Would you like to download the newest version?", "Uh Oh!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if(drUpdate == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://github.com/200Tigersbloxed/BSMulti-Installer/releases/latest");
                    Application.Exit();
                }
            }

            checkForMessage();
            
            Directory.CreateDirectory("Files");
        }

        private void panel1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void closeForm1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnFindSteam_Click(object sender, EventArgs e)
        {
            // Find the Steam folder
            var installs = BeatSaberTools.GetSteamBeatSaberInstalls();

            if(installs == null || installs.Length == 0)
            {
                MessageBox.Show("Uh Oh!", "A Steam store install of Beat Saber could not be found!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else{
                bsl = installs.First().InstallPath;
                if(Directory.Exists(bsl))
                {
                    if(File.Exists(bsl + @"\Beat Saber.exe"))
                    {
                        if(File.Exists(bsl + @"\IPA.exe"))
                        {
                            textBox1.Text = bsl;
                            pictureBox1.Image = BSMulti_Installer2.Properties.Resources.tick;
                            button4.BackColor = SystemColors.MenuHighlight;
                            allownext = true;
                            runVerifyCheck();
                            findMultiplayerVersion();
                        }
                        else
                        {
                            MessageBox.Show("IPA.exe Could not be found! Is Beat Saber Modded?", "Uh Oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Beat Saber.exe Could not be found! Is Beat Saber Installed?", "Uh Oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Beat Saber Could not be found! Is Beat Saber Installed under Steam?", "Uh Oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bsl = "";
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (allownext)
            {
                InstallerWindow f2 = new InstallerWindow();
                f2.bsl = bsl;
                f2.Show();
                this.Hide();
            }
        }

        private void btnFindOculus_Click(object sender, EventArgs e)
        {
            //Find the Oculus Folder
            var installs = BeatSaberTools.GetOculusBeatSaberInstalls();

            if (installs == null || installs.Length == 0)
            {
                MessageBox.Show("Uh Oh!", "An Oculus store install of Beat Saber could not be found!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                bsl = installs.First().InstallPath;
                if (Directory.Exists(bsl))
                {
                    if (File.Exists(bsl + @"\Beat Saber.exe"))
                    {
                        if (File.Exists(bsl + @"\IPA.exe"))
                        {
                            textBox1.Text = bsl;
                            pictureBox1.Image = BSMulti_Installer2.Properties.Resources.tick;
                            button4.BackColor = SystemColors.MenuHighlight;
                            allownext = true;
                            runVerifyCheck();
                            findMultiplayerVersion();
                        }
                        else
                        {
                            MessageBox.Show("IPA.exe Could not be found! Is Beat Saber Modded?", "Uh Oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Beat Saber.exe Could not be found! Is Beat Saber Installed?", "Uh Oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Beat Saber Could not be found! Is Beat Saber Installed under Oculus?", "Uh Oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bsl = "";
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog1.SelectedPath;
                if (File.Exists(selectedPath + @"\Beat Saber.exe"))
                {
                    if (File.Exists(selectedPath + @"\IPA.exe"))
                    {
                        bsl = selectedPath;
                        textBox1.Text = bsl;
                        pictureBox1.Image = BSMulti_Installer2.Properties.Resources.tick;
                        button4.BackColor = SystemColors.MenuHighlight;
                        allownext = true;
                        runVerifyCheck();
                        findMultiplayerVersion();
                    }
                    else
                    {
                        MessageBox.Show("IPA.exe was not found! Is Beat Saber Modded?", "Uh Oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Beat Saber was not found in this location! Is Beat Saber Installed?", "Uh Oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool verifyPermissions(string dir)
        {
            try
            {
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(dir);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        void runVerifyCheck()
        {
            if (verifyPermissions(bsl)) { }
            else
            {
                MessageBox.Show("Please run the installer as administrator to continue! (Beat Saber Folder Denied)", "Access Denied to Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            if(Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\Files"))
            {
                if (verifyPermissions(AppDomain.CurrentDomain.BaseDirectory + @"\Files")) { }
                else
                {
                    MessageBox.Show("Please run the installer as administrator to continue! (Installer Folder Denied)", "Access Denied to Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
            else
            {
                Directory.CreateDirectory("Files");
                if (verifyPermissions(AppDomain.CurrentDomain.BaseDirectory + @"\Files")) { }
                else
                {
                    MessageBox.Show("Please run the installer as administrator to continue! (Installer Folder Denied)", "Access Denied to Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
        }

        void findMultiplayerVersion()
        {
            string mj;
            string mlj;
            // check for the json data
            if(File.Exists(bsl + @"/UserData/BeatSaberMultiplayer.json"))
            {
                mj = bsl + @"/UserData/BeatSaberMultiplayer.json";
                if (File.Exists(bsl + @"/Plugins/BeatSaberMultiplayer.dll")) {
                    // multiplayer is installed
                    string json = System.IO.File.ReadAllText(mj);
                    dynamic bsmj = JsonConvert.DeserializeObject(json);
                    label8.Text = "Multiplayer Version: " + bsmj["_modVersion"];
                }
                else
                {
                    // no multiplayer
                    label8.Text = "Multiplayer Version: Not Installed";
                }
            }
            else
            {
                // no multiplayer
                label8.Text = "Multiplayer Version: Not Installed";
            }

            if (File.Exists(bsl + @"/UserData/BeatSaberMultiplayer.json"))
            {
                mlj = bsl + @"/UserData/BeatSaberMultiplayerLite.json";
                if (File.Exists(bsl + @"/Plugins/BeatSaberMultiplayerLite.dll"))
                {
                    // multiplayer is installed
                    string json = System.IO.File.ReadAllText(mlj);
                    dynamic bsmj = JsonConvert.DeserializeObject(json);
                    label9.Text = "MultiplayerLite Version: " + bsmj["_modVersion"];
                }
                else
                {
                    // no multiplayer
                    label9.Text = "MultiplayerLite Version: Not Installed";
                }
            }
            else
            {
                // no multiplayer
                label9.Text = "MultiplayerLite Version: Not Installed";
            }
        }

        void checkForMessage()
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://pastebin.com/raw/vaXRephy");
            StreamReader reader = new StreamReader(stream);
            String content = reader.ReadToEnd();
            string[] splitcontent = content.Split('|');
            if(splitcontent[0] == "Y")
            {
                MessageBox.Show(splitcontent[1], "Message From Developer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckForWebsiteConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("https://tigersserver.xyz"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/200Tigersbloxed/BSMulti-Installer/blob/master/README.md");
        }
    }
}
