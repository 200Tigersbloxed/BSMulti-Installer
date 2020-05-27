﻿using Microsoft.Win32;
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

        public Form1()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
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
                            string[] capdrive = phrase.Split(':');
                            steaminstallpath = capdrive[0].ToUpper() + ":" + @"\Steam";
                        }
                    }
                }
            }

            if(userownssteam == false)
            {
                MessageBox.Show("Uh Oh!", "Steam Could not be found!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else{
                bsl = steaminstallpath + @"\steamapps\common\Beat Saber";
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
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.Name != @"C:\")
                {
                    if (Directory.Exists(d.Name + @"Oculus"))
                    {
                        oculusinstallpath = d.Name + @"Oculus";
                        userownsoculus = true;
                    }
                }
                else
                {
                    if (Directory.Exists(d.Name + @"Program Files (x86)\Oculus"))
                    {
                        oculusinstallpath = d.Name + @"Program Files (x86)\Oculus";
                        userownsoculus = true;
                    }
                }
            }

            if (userownsoculus == false)
            {
                MessageBox.Show("Oculus Could not be found!", "Uh Oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                bsl = oculusinstallpath + @"\Software\hyperbolic-magnetism-beat-saber";
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/200Tigersbloxed/BSMulti-Installer/blob/master/README.md");
        }
    }
}
