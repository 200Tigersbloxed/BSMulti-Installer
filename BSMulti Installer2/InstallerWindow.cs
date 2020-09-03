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
        public string BeatSaberDirectory { get; set; }
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
                allowinstalluninstall = true;
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
            button.BackColor = highlighted ? Color.Green : (enabled ? SystemColors.MenuHighlight : SystemColors.GrayText) ;
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

        public bool TryDeleteMod(string gameDir, string fileName)
        {
            string modFile = Path.Combine(gameDir, Paths.Path_Plugins, fileName);
            bool success = true;
            if (File.Exists(modFile))
            {
                success = Utilities.Utilities.TryDelete(modFile);
            }
            modFile = Path.Combine(gameDir, Paths.Path_PendingPlugins, fileName);
            if (File.Exists(modFile))
            {
                success = success && Utilities.Utilities.TryDelete(modFile);
            }
            return success;
        }

        private async Task InstallMulti()
        {
            try
            {
                SetButtonEnabled(btnInstall, false);
                SetButtonEnabled(btnUninstall, false);
                SetButtonEnabled(btnSelectAndruzz, false, SelectedMultiplayerMod?.Name == "Multiplayer");
                SetButtonEnabled(btnSelectZinga, false, SelectedMultiplayerMod?.Name == "MultiplayerLite");
                statuslabel.Text = "Status: Preparing";
                pbOverall.Value = 0;
                pbComponent.Value = 0;
                allowinstalluninstall = false;
                currentlyinstallinguninstalling = true;

                Progress<OverallProgress> overallProgress = new Progress<OverallProgress>(OnOverallProgressChanged);
                Progress<ComponentProgress> componentProgress = new Progress<ComponentProgress>(OnComponentProgressChanged);
                string outputDir = BeatSaberDirectory;
                Directory.CreateDirectory(outputDir);
                foreach (var mod in InstallerConfig.ModGroup)
                {
                    if(mod != SelectedMultiplayerMod)
                    {
                        if(mod.Name == "Multiplayer")
                        {
                            TryDeleteMod(outputDir, "BeatSaberMultiplayer.dll");
                        }
                        if(mod.Name == "MultiplayerLite")
                        {
                            TryDeleteMod(outputDir, "BeatSaberMultiplayerLite.dll");
                        }
                    }
                }
                Installer installer = new Installer(outputDir, InstallerConfig, SelectedMultiplayerMod, SelectedMultiplayerMod.GetOptionalComponents(InstallerConfig), true);
                installer.EnsureValidInstaller();
                await installer.InstallMod(overallProgress, componentProgress);

                statuslabel.Text = "Status: Complete!";
                pbOverall.Value = 100;
                await Task.Delay(1000);
                

                DialogResult dialogResult = MessageBox.Show($"{SelectedMultiplayerMod.Name} v{SelectedMultiplayerMod.Version} is installed! Would you like to exit?", "Complete!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }catch(Exception ex)
            {
                MessageBox.Show($"{SelectedMultiplayerMod.Name} v{SelectedMultiplayerMod.Version} failed to install: {ex.Message}.");
            }
            finally
            {
                allowinstalluninstall = true;
                currentlyinstallinguninstalling = false;
                SetButtonEnabled(btnInstall, SelectedMultiplayerMod != null);
                SetButtonEnabled(btnUninstall, SelectedMultiplayerMod != null);
                SetButtonEnabled(btnSelectAndruzz, AndruzzMod != null, SelectedMultiplayerMod?.Name == "Multiplayer");
                SetButtonEnabled(btnSelectZinga, ZingaMod != null, SelectedMultiplayerMod?.Name == "MultiplayerLite");
            }
        }


        private void OnComponentProgressChanged(ComponentProgress progress)
        {
            pbComponent.Value = (int)(progress.Progress * 100);
        }

        private void OnOverallProgressChanged(OverallProgress progress)
        {
            if (progress.TotalStages <= 0)
            {
                statuslabel.Text = "";
                pbOverall.Value = 0;

            }
            statuslabel.Text = $"Status: Downloading {progress.Component} {progress.Stage}/{progress.TotalStages}";
            pbOverall.Value = ((progress.Stage - 1) * 100) / progress.TotalStages;
        }


        void UninstallMulti()
        {
            bool continuewithuninstall = false;
            statuslabel.Text = "Status: Preparing";
            pbOverall.Value = 25;
            allowinstalluninstall = false;
            currentlyinstallinguninstalling = true;
            btnUninstall.BackColor = SystemColors.GrayText;
            btnInstall.BackColor = SystemColors.GrayText;
            statuslabel.Text = "Status: Uninstalling Multiplayer";
            pbOverall.Value = 50;
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
            pbOverall.Value = 75;
            if (continuewithuninstall == true)
            {
                if (chkSongCore.Checked == true)
                {
                    if (File.Exists(BeatSaberDirectory + @"\Plugins\SongCore.dll"))
                    {
                        File.Delete(BeatSaberDirectory + @"\Plugins\SongCore.dll");
                    }
                }
                if (chkBsml.Checked == true)
                {
                    if (File.Exists(BeatSaberDirectory + @"\Plugins\BSML.dll"))
                    {
                        File.Delete(BeatSaberDirectory + @"\Plugins\BSML.dll");
                    }
                }
                if (chkBsUtils.Checked == true)
                {
                    if (File.Exists(BeatSaberDirectory + @"\Plugins\BS_Utils.dll"))
                    {
                        File.Delete(BeatSaberDirectory + @"\Plugins\BS_Utils.dll");
                    }
                }
                if (chkCustomAvatars.Checked == true)
                {
                    if (File.Exists(BeatSaberDirectory + @"\Plugins\CustomAvatar.dll"))
                    {
                        File.Delete(BeatSaberDirectory + @"\Plugins\CustomAvatar.dll");
                    }
                    Directory.Delete(BeatSaberDirectory + @"\DynamicOpenVR", true);
                }
                if (chkDiscordCore.Checked == true)
                {
                    if (File.Exists(BeatSaberDirectory + @"\Plugins\DiscordCore.dll"))
                    {
                        File.Delete(BeatSaberDirectory + @"\Plugins\DiscordCore.dll");
                    }
                    Directory.Delete(BeatSaberDirectory + @"\Libs\Native", true);
                }
                if (chkDynOVR.Checked == true)
                {
                    if (File.Exists(BeatSaberDirectory + @"\Plugins\DynamicOpenVR.manifest"))
                    {
                        File.Delete(BeatSaberDirectory + @"\Plugins\DynamicOpenVR.manifest");
                    }
                    if (File.Exists(BeatSaberDirectory + @"\Libs\DynamicOpenVR.dll"))
                    {
                        File.Delete(BeatSaberDirectory + @"\Libs\DynamicOpenVR.dll");
                    }
                }
                if (chkScoreSaber.Checked == true)
                {
                    if (File.Exists(BeatSaberDirectory + @"\Plugins\ScoreSaber.dll"))
                    {
                        File.Delete(BeatSaberDirectory + @"\Plugins\ScoreSaber.dll");
                    }
                }
            }
            statuslabel.Text = "Status: Complete!";
            pbOverall.Value = 100;
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

        private async void btnInstall_Click(object sender, EventArgs e)
        {
            if (allowinstalluninstall)
            {
                pbOverall.Value = 0;
                await InstallMulti();
            }
        }

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            if (allowinstalluninstall)
            {
                pbOverall.Value = 0;
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