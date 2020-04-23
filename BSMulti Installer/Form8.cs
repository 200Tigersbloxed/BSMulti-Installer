using Newtonsoft.Json;
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

namespace BSMulti_Installer
{
    public partial class Form8 : Form
    {
        public string bsdir;
        public bool bsm = false;
        public bool bsml = false;
        public Form8()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog1.SelectedPath;
                if (File.Exists(selectedPath + @"\Beat Saber.exe"))
                {
                    bsdir = selectedPath;
                    if(File.Exists(bsdir + @"\UserData\BeatSaberMultiplayer.json"))
                    {
                        bsm = true;
                        string json = System.IO.File.ReadAllText(bsdir + @"\UserData\BeatSaberMultiplayer.json");
                        dynamic bsmj = JsonConvert.DeserializeObject(json);
                        var num = 0;
                        foreach(var k in bsmj["_serverHubIPs"])
                        {
                            listBox1.Items.Add((string)bsmj._serverHubIPs[num] + ":" + (string)bsmj._serverHubPorts[num]);
                            num = num + 1;
                        }
                    }

                    if (File.Exists(bsdir + @"\UserData\BeatSaberMultiplayer.json"))
                    {
                        bsml = true;
                        string jsonl = System.IO.File.ReadAllText(bsdir + @"\UserData\BeatSaberMultiplayerLite.json");
                        dynamic bsmlj = JsonConvert.DeserializeObject(jsonl);
                        var numl = 0;
                        foreach (var k in bsmlj["_serverHubIPs"])
                        {
                            listBox2.Items.Add((string)bsmlj._serverHubIPs[numl] + ":" + (string)bsmlj._serverHubPorts[numl]);
                            numl = numl + 1;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Beat Saber was not found in this location!", "Uh Oh!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(bsm == true)
            {
                
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(bsml == true)
            {
                System.Diagnostics.Process.Start(bsdir + @"\UserData\BeatSaberMultiplayerLite.json");
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(bsdir + @"\UserData\BeatSaberMultiplayer.json");
        }
    }
}
