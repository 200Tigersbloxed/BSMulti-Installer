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
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace BSMulti_Installer
{
    public partial class Form6 : Form
    {
        public string bsdir { get; set; }

        public Form6()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label3.Text = "Checking if Multiplayer is installed...";
            progressBar1.Value = 25;
            if(File.Exists(bsdir + @"\Plugins\BeatSaberMultiplayerLite.dll"))
            {
                label3.Text = "Removing Multiplayer...";
                progressBar1.Value = 50;
                File.Delete(bsdir + @"\Plugins\BeatSaberMultiplayerLite.dll");
                if(File.Exists(bsdir + @"\Plugins\BeatSaberMultiplayerLite.pdb"))
                {
                    File.Delete(bsdir + @"\Plugins\BeatSaberMultiplayerLite.pdb");
                }
                label3.Text = "Removing Extra Options...";
                progressBar1.Value = 75;
                if (checkBox2.Checked == true)
                {
                    if (File.Exists(bsdir + @"\Libs\Lidgren.Network.dll"))
                    {
                        File.Delete(bsdir + @"\Libs\Lidgren.Network.dll");
                    }

                    if (File.Exists(bsdir + @"\Libs\NSpeex.dll"))
                    {
                        File.Delete(bsdir + @"\Libs\NSpeex.dll");
                    }
                }
                if(checkBox3.Checked == true)
                {
                    if (File.Exists(bsdir + @"\Plugins\CustomAvatar.dll"))
                    {
                        File.Delete(bsdir + @"\Plugins\CustomAvatar.dll");
                    }

                    if (Directory.Exists(bsdir + @"\DynamicOpenVR"))
                    {
                        Directory.Delete(bsdir + @"\DynamicOpenVR");
                    }
                }
                if(checkBox4.Checked == true)
                {
                    if (File.Exists(bsdir + @"\Plugins\DynamicOpenVR.manifest"))
                    {
                        File.Delete(bsdir + @"\Plugins\DynamicOpenVR.manifest");
                    }

                    if (File.Exists(bsdir + @"\Libs\DynamicOpenVR.dll"))
                    {
                        File.Delete(bsdir + @"\Libs\DynamicOpenVR.dll");
                    }
                }
                if(checkBox5.Checked == true)
                {
                    if (File.Exists(bsdir + @"\Plugins\DiscordCore.dll"))
                    {
                        File.Delete(bsdir + @"\Plugins\DiscordCore.dll");
                    }

                    if (Directory.Exists(bsdir + @"\Libs\Native"))
                    {
                        Directory.Delete(bsdir + @"\Libs\Native", true);
                    }
                }
                label3.Text = "Done!";
                progressBar1.Value = 100;
            }
            else
            {
                progressBar1.Value = 100;
                ProgressBarColor.SetState(progressBar1, 2);
                MessageBox.Show("BeatSaberMultiplayerLite.dll Was not found / Plugin not installed.", "Uh Oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }

    public static class ProgressBarColor
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this ProgressBar p, int state)
        {
            SendMessage(p.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }
}
