using BSMulti_Installer2.Utilities;
using BSMulti_Installer2.XML;
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
    public partial class InstallerWindow : Form
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

        private void InvokeSafe(EventHandler handler)
        {
            if (handler == null) return;
            if (InvokeRequired)
                Invoke(handler);
            else
                handler.Invoke(this, EventArgs.Empty);
        }

        private MultiplayerMod _selectedMultiplayerMod;
        public MultiplayerMod SelectedMultiplayerMod
        {
            get => _selectedMultiplayerMod;
            set
            {
                if (_selectedMultiplayerMod == value) return;
                _selectedMultiplayerMod = value;
                InvokeSafe(SelectedModChanged);
            }
        }
        public bool currentlyinstallinguninstalling = false;
        public bool allowinstalluninstall = false;
        public string bsl { get; set; }
        public MultiplayerMod AndruzzMod
        {
            get => _andruzzMod;
            set
            {
                if (_andruzzMod == value) return;
                _andruzzMod = value;
                InvokeSafe(AndruzzModChanged);
            }
        }
        public MultiplayerMod ZingaMod
        {
            get => _zingaMod;
            set
            {
                if (_zingaMod == value) return;
                _zingaMod = value;
                InvokeSafe(ZingaModChanged);
            }
        }

        public event EventHandler ZingaModChanged;
        public event EventHandler AndruzzModChanged;
        public event EventHandler SelectedModChanged;

        public InstallerWindow()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            ZingaModChanged += OnZingaModChanged;
            AndruzzModChanged += OnAndruzzModChanged;
            SelectedModChanged += OnSelectedModChanged;
            OnAndruzzModChanged(this, EventArgs.Empty);
            OnZingaModChanged(this, EventArgs.Empty);
            OnSelectedModChanged(this, EventArgs.Empty);

        }

        private void OnSelectedModChanged(object sender, EventArgs e)
        {
            if (SelectedMultiplayerMod != null)
            {
                pictureBox1.Image = BSMulti_Installer2.Properties.Resources.tick;
                SetButtonEnabled(btnInstall, true, false);
                SetButtonEnabled(btnUninstall, true, false);
                SetButtonEnabled(btnSelectAndruzz, AndruzzMod != null, SelectedMultiplayerMod.Name == "Multiplayer");
                SetButtonEnabled(btnSelectZinga, ZingaMod != null, SelectedMultiplayerMod.Name == "MultiplayerLite");

            }
            else
            {
                pictureBox1.Image = BSMulti_Installer2.Properties.Resources.cross;
                SetButtonEnabled(btnInstall, false, false);
                SetButtonEnabled(btnUninstall, false, false);
                SetButtonEnabled(btnSelectAndruzz, AndruzzMod != null, false);
                SetButtonEnabled(btnSelectZinga, ZingaMod != null, false);
            }
        }

        private void SetButtonEnabled(Button button, bool enabled, bool highlighted = false)
        {
            button.BackColor = enabled ? (highlighted ? Color.Green : SystemColors.MenuHighlight) : SystemColors.GrayText;
            button.Enabled = enabled;
        }

        private void OnAndruzzModChanged(object sender, EventArgs e)
        {
            if (AndruzzMod != null)
            {
                btnSelectAndruzz.BackColor = SystemColors.MenuHighlight;
                btnSelectAndruzz.Enabled = true;
            }
            else
            {
                btnSelectAndruzz.BackColor = SystemColors.GrayText;
                btnSelectAndruzz.Enabled = false;
            }
        }

        private void OnZingaModChanged(object sender, EventArgs e)
        {
            if (AndruzzMod != null)
            {
                btnSelectZinga.BackColor = SystemColors.MenuHighlight;
                btnSelectZinga.Enabled = true;
            }
            else
            {
                btnSelectZinga.BackColor = SystemColors.GrayText;
                btnSelectZinga.Enabled = false;
            }
        }

        private void pnlTitleBar_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void btnSelectAndruzz_Click(object sender, EventArgs e)
        {

            if (currentlyinstallinguninstalling == false)
            {
                SelectedMultiplayerMod = AndruzzMod;
            }
        }

        private void btnSelectZinga_Click(object sender, EventArgs e)
        {
            if (currentlyinstallinguninstalling == false)
            {
                SelectedMultiplayerMod = ZingaMod;
            }
        }

        void InstallMulti()
        {
            statuslabel.Text = "Status: Preparing";
            progressBar1.Value = 10;
            allowinstalluninstall = false;
            currentlyinstallinguninstalling = true;
            btnUninstall.BackColor = SystemColors.GrayText;
            btnInstall.BackColor = SystemColors.GrayText;
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
            //using (var wc = new WebClient())
            //{
            //    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadCompleted);
            //    wc.DownloadProgressChanged += wc_DownloadProgressChanged;
            //    if (multiselected == "a")
            //    {
            //        if (File.Exists(bsl + @"\Plugins\BeatSaberMultiplayerLite.dll"))
            //        {
            //            statuslabel.Text = "Status: Failed";
            //            allowinstalluninstall = true;
            //            currentlyinstallinguninstalling = false;
            //            btnUninstall.BackColor = SystemColors.MenuHighlight;
            //            btnInstall.BackColor = SystemColors.MenuHighlight;
            //            MessageBox.Show("Beat Saber Multiplayer Lite is installed! Installation Failed. Please Uninstall Zingabopp's Multiplayer Lite to continue installing Andruzzzhka's Multiplayer", "Uh Oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //        else
            //        {
            //            wc.DownloadFileAsync(new System.Uri("https://tigersserver.xyz/andruzzzhkalatest"), AppDomain.CurrentDomain.BaseDirectory + @"\Files\multiplayer.zip");
            //        }
            //    }
            //    else if (multiselected == "z")
            //    {
            //        if (File.Exists(bsl + @"\Plugins\BeatSaberMultiplayer.dll"))
            //        {
            //            statuslabel.Text = "Status: Failed";
            //            allowinstalluninstall = true;
            //            currentlyinstallinguninstalling = false;
            //            btnUninstall.BackColor = SystemColors.MenuHighlight;
            //            btnInstall.BackColor = SystemColors.MenuHighlight;
            //            MessageBox.Show("Beat Saber Multiplayer is installed! Installation Failed. Please Uninstall Andruzzzhka's Multiplayer to continue installing Zingabopp's Multiplayer Lite", "Uh Oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        }
            //        else
            //        {
            //            wc.DownloadFileAsync(new System.Uri("https://tigersserver.xyz/zingabopplatest"), AppDomain.CurrentDomain.BaseDirectory + @"\Files\multiplayer.zip");
            //        }
            //    }
            //}
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
                //    if (multiselected == "a")
                //    {
                //        if (dir.Name == "ca")
                //        {
                //            DirectoryInfo cadi = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\Files\ca");
                //            if (Directory.Exists(bsl + @"\CustomAvatars"))
                //            {
                //                // dont u dare delete someone's custom avatars folder
                //            }
                //            else
                //            {
                //                Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\Files\ca\CustomAvatars", bsl + @"\CustomAvatars");
                //            }
                //            if (Directory.Exists(bsl + @"\DynamicOpenVR"))
                //            {
                //                Directory.Delete(bsl + @"\DynamicOpenVR", true);
                //                Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\Files\ca\DynamicOpenVR", bsl + @"\DynamicOpenVR");
                //            }
                //            else
                //            {
                //                Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\Files\ca\DynamicOpenVR", bsl + @"\DynamicOpenVR");
                //            }
                //            foreach (DirectoryInfo cadir in cadi.GetDirectories())
                //            {
                //                if (cadir.Name == "Plugins")
                //                {
                //                    // Don't move CustomAvatar's DLL
                //                }
                //            }
                //        }
                //    }
                //    if (dir.Name == "dc")
                //    {
                //        DirectoryInfo dcdi = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"Files\dc");
                //        foreach (DirectoryInfo dcdir in dcdi.GetDirectories())
                //        {
                //            if (dcdir.Name == "Plugins")
                //            {
                //                foreach (FileInfo file in dcdir.GetFiles())
                //                {
                //                    if (File.Exists(bsl + @"\Plugins\" + file.Name))
                //                    {
                //                        File.Delete(bsl + @"\Plugins\" + file.Name);
                //                        File.Move(file.FullName, bsl + @"\Plugins\" + file.Name);
                //                    }
                //                    else
                //                    {
                //                        File.Move(file.FullName, bsl + @"\Plugins\" + file.Name);
                //                    }
                //                }
                //            }
                //            if (dcdir.Name == "Libs")
                //            {
                //                foreach (DirectoryInfo dcnativedir in dcdir.GetDirectories())
                //                {
                //                    if (Directory.Exists(bsl + @"\Libs\Native"))
                //                    {
                //                        Directory.Delete(bsl + @"\Libs\Native", true);
                //                        Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\Files\dc\Libs\Native", bsl + @"\Libs\Native");
                //                    }
                //                    else
                //                    {
                //                        Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\Files\dc\Libs\Native", bsl + @"\Libs\Native");
                //                    }
                //                }
                //            }
                //        }
                //    }
                //    if (dir.Name == "dep")
                //    {
                //        DirectoryInfo depdi = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"Files\dep\dep");
                //        foreach (DirectoryInfo depdir in depdi.GetDirectories())
                //        {
                //            if (depdir.Name == "Plugins")
                //            {
                //                foreach (FileInfo file in depdir.GetFiles())
                //                {
                //                    if (File.Exists(bsl + @"\Plugins\" + file.Name))
                //                    {
                //                        File.Delete(bsl + @"\Plugins\" + file.Name);
                //                        File.Move(file.FullName, bsl + @"\Plugins\" + file.Name);
                //                    }
                //                    else
                //                    {
                //                        File.Move(file.FullName, bsl + @"\Plugins\" + file.Name);
                //                    }
                //                }
                //            }
                //        }
                //    }
                //    if (multiselected == "a")
                //    {
                //        if (dir.Name == "dovr")
                //        {
                //            DirectoryInfo dovrdi = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"Files\dovr");
                //            foreach (DirectoryInfo dovrdir in dovrdi.GetDirectories())
                //            {
                //                if (dovrdir.Name == "Plugins")
                //                {
                //                    foreach (FileInfo file in dovrdir.GetFiles())
                //                    {
                //                        if (File.Exists(bsl + @"\Plugins\" + file.Name))
                //                        {
                //                            File.Delete(bsl + @"\Plugins\" + file.Name);
                //                            File.Move(file.FullName, bsl + @"\Plugins\" + file.Name);
                //                        }
                //                        else
                //                        {
                //                            File.Move(file.FullName, bsl + @"\Plugins\" + file.Name);
                //                        }
                //                    }
                //                }
                //                if (dovrdir.Name == "Libs")
                //                {
                //                    foreach (FileInfo file in dovrdir.GetFiles())
                //                    {
                //                        if (File.Exists(bsl + @"\Libs\" + file.Name))
                //                        {
                //                            File.Delete(bsl + @"\Libs\" + file.Name);
                //                            File.Move(file.FullName, bsl + @"\Libs\" + file.Name);
                //                        }
                //                        else
                //                        {
                //                            File.Move(file.FullName, bsl + @"\Libs\" + file.Name);
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //    if (dir.Name == "multiplayer")
                //    {
                //        DirectoryInfo multiplayerdi = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"Files\multiplayer");
                //        foreach (DirectoryInfo multiplayerdir in multiplayerdi.GetDirectories())
                //        {
                //            if (multiplayerdir.Name == "Plugins")
                //            {
                //                foreach (FileInfo file in multiplayerdir.GetFiles())
                //                {
                //                    if (File.Exists(bsl + @"\Plugins\" + file.Name))
                //                    {
                //                        File.Delete(bsl + @"\Plugins\" + file.Name);
                //                        File.Move(file.FullName, bsl + @"\Plugins\" + file.Name);
                //                    }
                //                    else
                //                    {
                //                        File.Move(file.FullName, bsl + @"\Plugins\" + file.Name);
                //                    }
                //                }
                //            }
                //            if (multiplayerdir.Name == "Libs")
                //            {
                //                foreach (FileInfo file in multiplayerdir.GetFiles())
                //                {
                //                    if (File.Exists(bsl + @"\Libs\" + file.Name))
                //                    {
                //                        File.Delete(bsl + @"\Libs\" + file.Name);
                //                        File.Move(file.FullName, bsl + @"\Libs\" + file.Name);
                //                    }
                //                    else
                //                    {
                //                        File.Move(file.FullName, bsl + @"\Libs\" + file.Name);
                //                    }
                //                }
                //            }
                //        }
                //    }
            }
            //if (multiselected == "a")
            //{
            //    if (File.Exists(@"Files\CustomAvatar.dll"))
            //    {
            //        if (File.Exists(bsl + @"\Plugins\CustomAvatar.dll"))
            //        {
            //            File.Delete(bsl + @"\Plugins\CustomAvatar.dll");
            //            File.Move(@"Files\CustomAvatar.dll", bsl + @"\Plugins\CustomAvatar.dll");
            //        }
            //        else
            //        {
            //            File.Move(@"Files\CustomAvatar.dll", bsl + @"\Plugins\CustomAvatar.dll");
            //        }
            //    }
            //}

            statuslabel.Text = "Status: Complete!";
            progressBar1.Value = 100;
            allowinstalluninstall = true;
            currentlyinstallinguninstalling = false;
            btnUninstall.BackColor = SystemColors.MenuHighlight;
            btnInstall.BackColor = SystemColors.MenuHighlight;
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
            btnUninstall.BackColor = SystemColors.GrayText;
            btnInstall.BackColor = SystemColors.GrayText;
            statuslabel.Text = "Status: Uninstalling Multiplayer";
            progressBar1.Value = 50;
            //if (multiselected == "a")
            //{
            //    if (File.Exists(bsl + @"\Plugins\BeatSaberMultiplayer.dll"))
            //    {
            //        File.Delete(bsl + @"\Plugins\BeatSaberMultiplayer.dll");
            //        continuewithuninstall = true;
            //    }
            //    else
            //    {
            //        DialogResult dialogResult2 = MessageBox.Show("Multiplayer was not found! Would you like to continue?", "Uh Oh!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //        if (dialogResult2 == DialogResult.Yes)
            //        {
            //            continuewithuninstall = true;
            //        }
            //        else
            //        {
            //            continuewithuninstall = false;
            //        }
            //    }
            //}
            //if (multiselected == "z")
            //{
            //    if (File.Exists(bsl + @"\Plugins\BeatSaberMultiplayerLite.dll"))
            //    {
            //        File.Delete(bsl + @"\Plugins\BeatSaberMultiplayerLite.dll");
            //        continuewithuninstall = true;
            //    }
            //    else
            //    {
            //        DialogResult dialogResult2 = MessageBox.Show("Multiplayer Lite was not found! Would you like to continue?", "Uh Oh!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //        if (dialogResult2 == DialogResult.Yes)
            //        {
            //            continuewithuninstall = true;
            //        }
            //        else
            //        {
            //            continuewithuninstall = false;
            //        }
            //    }
            //}
            statuslabel.Text = "Status: Uninstalling Dependencies";
            progressBar1.Value = 75;
            if (continuewithuninstall == true)
            {
                if (chkSongCore.Checked == true)
                {
                    if (File.Exists(bsl + @"\Plugins\SongCore.dll"))
                    {
                        File.Delete(bsl + @"\Plugins\SongCore.dll");
                    }
                }
                if (chkBsml.Checked == true)
                {
                    if (File.Exists(bsl + @"\Plugins\BSML.dll"))
                    {
                        File.Delete(bsl + @"\Plugins\BSML.dll");
                    }
                }
                if (chkBsUtils.Checked == true)
                {
                    if (File.Exists(bsl + @"\Plugins\BS_Utils.dll"))
                    {
                        File.Delete(bsl + @"\Plugins\BS_Utils.dll");
                    }
                }
                if (chkCustomAvatars.Checked == true)
                {
                    if (File.Exists(bsl + @"\Plugins\CustomAvatar.dll"))
                    {
                        File.Delete(bsl + @"\Plugins\CustomAvatar.dll");
                    }
                    Directory.Delete(bsl + @"\DynamicOpenVR", true);
                }
                if (chkDiscordCore.Checked == true)
                {
                    if (File.Exists(bsl + @"\Plugins\DiscordCore.dll"))
                    {
                        File.Delete(bsl + @"\Plugins\DiscordCore.dll");
                    }
                    Directory.Delete(bsl + @"\Libs\Native", true);
                }
                if (chkDynOVR.Checked == true)
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
                if (chkScoreSaber.Checked == true)
                {
                    if (File.Exists(bsl + @"\Plugins\ScoreSaber.dll"))
                    {
                        File.Delete(bsl + @"\Plugins\ScoreSaber.dll");
                    }
                }
            }
            statuslabel.Text = "Status: Complete!";
            progressBar1.Value = 100;
            allowinstalluninstall = true;
            currentlyinstallinguninstalling = false;
            btnUninstall.BackColor = SystemColors.MenuHighlight;
            btnInstall.BackColor = SystemColors.MenuHighlight;
            DialogResult dialogResult = MessageBox.Show("Multiplayer is uninstalled :( Would you like to exit?", "Complete!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnCloseForm_Click(object sender, EventArgs e)
        {
            GameInstallSelector f1 = new GameInstallSelector();
            f1.Show();
            this.Hide();
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            if (allowinstalluninstall)
            {
                progressBar1.Value = 0;
                InstallMulti();
            }
        }

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            if (allowinstalluninstall)
            {
                progressBar1.Value = 0;
                UninstallMulti();
            }
        }

        private void lblHowChoose_Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/200Tigersbloxed/BSMulti-Installer/wiki/Which-Multiplayer-Should-I-Install%3F");
        }

        private async void InstallerWindow_Load(object sender, EventArgs e)
        {
            MultiplayerInstallerConfiguration installerConfig = await GetInstallerConfig(ShowError);
            InstallerConfig = installerConfig;
            var mods = installerConfig.ModGroup;
            for (int i = 0; i < mods.Length; i++)
            {
                if (mods[i].Name == "Multiplayer")
                {
                    AndruzzMod = mods[i];
                }
                else if (mods[i].Name == "MultiplayerLite")
                {
                    ZingaMod = mods[i];
                }
            }

        }

        private void ShowError(string msg)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ShowError(msg)));
            }
            else
            {
                MessageBox.Show(msg);
            }
        }

        private MultiplayerInstallerConfiguration InstallerConfig;
        private MultiplayerMod _andruzzMod;
        private MultiplayerMod _zingaMod;

        private static async Task<MultiplayerInstallerConfiguration> GetInstallerConfig(Action<string> errorCallback = null)
        {
            MultiplayerInstallerConfiguration installerConfig = null;
            Exception webException = null;
            Exception fileException = null;
            bool localCacheUsed = false;
            try
            {
                var response = await WebUtils.HttpClient.GetAsync(Paths.URL_MultiplayerConfiguration).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                string content = null;
                using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    installerConfig = MultiplayerInstallerConfiguration.Deserialize(responseStream);
                    Stream stream = responseStream;
                    if (stream.CanSeek)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        using (var sr = new StreamReader(stream))
                        {
                            content = await sr.ReadToEndAsync().ConfigureAwait(false);
                        }
                    }
                    else
                        content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                if (content != null && installerConfig != null)
                {
                    try
                    {
                        File.WriteAllText(Paths.Path_CachedConfig, content);
                    }
                    catch (Exception ex)
                    {
                        errorCallback?.Invoke($"Error updating cached multiplayer configuration info: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                webException = ex;
            }
            if (installerConfig == null)
            {
                try
                {
                    installerConfig = Utilities.Utilities.GetInstallerConfigFromFile(Paths.Path_CachedConfig);
                    localCacheUsed = true;
                }
                catch (Exception ex)
                {
                    fileException = ex;
                }
            }
            if (installerConfig == null)
            {
                errorCallback?.Invoke($"Unable to retrieve multiplayer configuration info.\nWeb error: {webException?.Message ?? "No exception, but no result either."}\nFile error: {fileException?.Message ?? "No exception, but no result either."}");
            }
            else if (localCacheUsed)
            {
                errorCallback?.Invoke($"Unable to retrieve updated multiplayer configuration info, local cache used instead: {webException?.Message ?? "No exception, but no result either."}");
            }
            return installerConfig;
        }
    }
}