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
    public partial class Uninstall : Form
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

        public Uninstall()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            pictureBox1.Hide();
        }

        private void panel1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar2.Value = e.ProgressPercentage;
        }


        void UninstallMulti()
        {
            bool continuewithuninstall = false;
            statuslabel.Text = "Status: Preparing";
            progressBar1.Value = 25;
            allowinstalluninstall = false;
            currentlyinstallinguninstalling = true;
            uninstallButton.BackColor = SystemColors.GrayText;
            statuslabel.Text = "Status: Uninstalling Multiplayer";
            progressBar1.Value = 50;
            if(File.Exists(bsl + @"\Plugins\BeatSaberMultiplayer.dll"))
            {
                File.Delete(bsl + @"\Plugins\BeatSaberMultiplayer.dll");
                continuewithuninstall = true;
            }
            else if (File.Exists(bsl + @"\Plugins\BeatSaberMultiplayerLite.dll"))
            {
                File.Delete(bsl + @"\Plugins\BeatSaberMultiplayerLite.dll");
                continuewithuninstall = true;
            }
            else
            {
                DialogResult dialogResult2 = MessageBox.Show("No multiplayer install was not found! Would you like to continue?", "Uh Oh!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(dialogResult2 == DialogResult.Yes)
                {
                    continuewithuninstall = true;
                }
                else
                {
                    continuewithuninstall = false; 
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
                if(checkBox7.Checked == true)
                {
                    if(File.Exists(bsl + @"\Plugins\ScoreSaber.dll"))
                    {
                        File.Delete(bsl + @"\Plugins\ScoreSaber.dll");
                    }
                }
            }
            statuslabel.Text = "Status: Complete!";
            progressBar1.Value = 100;
            allowinstalluninstall = true;
            currentlyinstallinguninstalling = false;
            uninstallButton.BackColor = SystemColors.MenuHighlight;
            DialogResult dialogResult = MessageBox.Show("Multiplayer is uninstalled :( Would you like to exit?", "Complete!", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void closeForm1_Click(object sender, EventArgs e)
        {
            FindDirectory f1 = new FindDirectory();
            f1.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            UninstallMulti();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/200Tigersbloxed/BSMulti-Installer/wiki/Which-Multiplayer-Should-I-Install%3F");
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click_1(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
