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
using static BSMulti_Installer2.Utilities.WebUtils;
using System.Reflection;

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
        public string bsl;
        public bool allownext = false;
        public Version AppVersion = Assembly.GetExecutingAssembly().GetName().Version;
        public string Version => $"{AppVersion.Major}.{AppVersion.Minor}.{AppVersion.Build}";

        public GameInstallSelector()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // check for internet
            if (await CheckForInternetConnection() != true)
            {
                MessageBox.Show("An Internet Connection is required!", "Not Connected to Internet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            // check if user can access website
            if (await CheckForWebsiteConnection() != true)
            {
                MessageBox.Show($"Failed to connect to {Paths.URL_TigerServer}. Please try again soon.", "Failed to Connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            bool versionSuccess = false;
            Exception versionException = null;
            try
            {
                var latestVersion = await VersionCheck.GetLatestVersionAsync(Paths.URL_ReleaseAPIPage);
                if (latestVersion.IsSet)
                {
                    versionSuccess = true;
                    if (Utilities.Utilities.CompareVersions(AppVersion, latestVersion.GetVersionArray()) > 0)
                    {
                        DialogResult drUpdate = MessageBox.Show("BSMulti-Installer is not up to date! Would you like to download the newest version?", "Uh Oh!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (drUpdate == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(Paths.URL_ReleasePage.AbsoluteUri);
                            Application.Exit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                versionException = ex;
            }
            if (!versionSuccess)
            {
                MessageBox.Show($"Error checking for latest BSMulti-Installer version: {versionException.Message ?? "No exception was thrown."}");
            }


            checkForMessage();

            Directory.CreateDirectory("Files");
        }

        private void pnlTitleBar_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
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

            if (installs == null || installs.Length == 0)
            {
                MessageBox.Show("Uh Oh!", "A Steam store install of Beat Saber could not be found!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                f2.BeatSaberDirectory = bsl;
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
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\Files"))
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
            if (File.Exists(bsl + @"/UserData/BeatSaberMultiplayer.json"))
            {
                mj = bsl + @"/UserData/BeatSaberMultiplayer.json";
                if (File.Exists(bsl + @"/Plugins/BeatSaberMultiplayer.dll"))
                {
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
        private static bool DevMessageShown = false;
        private async void checkForMessage()
        {
            if (DevMessageShown) return;
            try
            {
                var response = await HttpClient.GetAsync(Paths.URL_DeveloperMessage);
                string content = await response.Content.ReadAsStringAsync();
                string[] splitcontent = content.Split('|');
                if (splitcontent[0] == "Y")
                {
                    MessageBox.Show(splitcontent[1], "Message From Developer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                DevMessageShown = true;
            }
            catch
            {

            }
        }

        public static async Task<bool> CheckForInternetConnection()
        {
            try
            {
                var response = await HttpClient.GetAsync(Paths.URL_ConnectionCheck).ConfigureAwait(false);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<bool> CheckForWebsiteConnection()
        {
            try
            {
                var response = await HttpClient.GetAsync(Paths.URL_TigerServer).ConfigureAwait(false);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Paths.URL_ReadmePage.AbsoluteUri);
        }
    }
}
