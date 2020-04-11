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
                        bsml = true;
                        string json = System.IO.File.ReadAllText(bsdir + @"\UserData\BeatSaberMultiplayer.json");
                        dynamic bsm = JsonConvert.DeserializeObject(json);
                        var num = 0;
                        foreach(var k in bsm["_serverHubIPs"])
                        {
                            listBox1.Items.Add((string)bsm._serverHubIPs[num]);
                            num = num + 1;
                        }
                        panel1.Visible = true;
                    }
                    else
                    {
                        
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
                string json = System.IO.File.ReadAllText(bsdir + @"\UserData\BeatSaberMultiplayer.json");
                dynamic bsmcj = JsonConvert.DeserializeObject(json);

            }
        }
    }
}
