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

namespace BSMulti_Installer2
{
    public partial class Form1 : Form
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

        public bool userownssteam = false;
        public bool userownsoculus = false;
        string steaminstallpath = "";
        string oculusinstallpath = "";
        public string bsl;
        public bool allownext = false;
        public string version = "v2.0.4";

        public Form1()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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

        private void button1_Click(object sender, EventArgs e)
        {
            // Find the Steam folder
            RegistryKey rk1s = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            if (rk1s != null)
            {
                RegistryKey rk2 = rk1s.OpenSubKey("Software");
                if (rk2 != null)
                {
                    RegistryKey rk3 = rk2.OpenSubKey("Valve");
                    if (rk3 != null)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey("Steam");
                        if (rk4 != null)
                        {
                            userownssteam = true;
                            string phrase = rk4.GetValue("SteamPath").ToString();
                            steaminstallpath = phrase;
                        }
                    }
                }
            }

            if(userownssteam == false)
            {
                MessageBox.Show("Uh Oh!", "Steam Could not be found!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else{
                bsl = steaminstallpath + @"/steamapps/common/Beat Saber";
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
                Form2 f2 = new Form2();
                f2.bsl = bsl;
                f2.Show();
                this.Hide();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Find the Oculus Folder
            RegistryKey rk1s = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            if (rk1s != null)
            {
                RegistryKey rk2 = rk1s.OpenSubKey("Software");
                if (rk2 != null)
                {
                    RegistryKey rk3 = rk2.OpenSubKey("WOW6432Node");
                    if (rk3 != null)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey("Oculus VR, LLC");
                        if (rk4 != null)
                        {
                            RegistryKey rk5 = rk4.OpenSubKey("Oculus");
                            if(rk5 != null)
                            {
                                RegistryKey rk6 = rk5.OpenSubKey("Config");
                                if(rk6 != null)
                                {
                                    userownsoculus = true;
                                    string phrase = rk6.GetValue("InitialAppLibrary").ToString();
                                    oculusinstallpath = phrase;
                                }
                            }
                        }
                    }
                }
            }

            if (userownsoculus == false)
            {
                MessageBox.Show("Oculus Could not be found!", "Uh Oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                bsl = oculusinstallpath + @"/Software/Software/hyperbolic-magnetism-beat-saber";
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/200Tigersbloxed/BSMulti-Installer/blob/master/README.md");
        }
    }
}
