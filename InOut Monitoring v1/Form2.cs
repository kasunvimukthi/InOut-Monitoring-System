using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InOut_Monitoring_v1
{
    public partial class Form2 : Form
    {
        Settings1 settings1 = Settings1.Default;
        String appName = "InOut Monitor";
        RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            //Settings1.Default.fullDayMessage = (int)numericUpDown1.Value;

            settings1.TimeMessage = (int)numericUpDown1.Value;

            if (checkBox1.Checked == true)
            {
                settings1.notificaton = true;
            }
            else
            {
                settings1.notificaton = false;
            }

            if (checkBox2.Checked == true)
            {
                settings1.AutoStart = true;
                
                registryKey.SetValue(appName, Application.ExecutablePath.ToString());
            }
            else
            {
                settings1.AutoStart = false;

                if (registryKey.GetValue(appName) != null)
                {

                    registryKey.DeleteValue(appName);
                }


            }

            settings1.Save();
            GetSettings();

            this.Close();

            string executablePath = Application.ExecutablePath; // Get the executable path

            // Exit the current program
            Application.Exit();

            // Start a new instance of the program
            Process.Start(executablePath);
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            GetSettings();
        }

        private void GetSettings()
        {
            numericUpDown1.Value = Convert.ToInt32(settings1.TimeMessage);

            if (settings1.notificaton == true)
            {
                checkBox1.Checked = true;
            }
            else
            {
                checkBox1.Checked = false;
            }

            if (settings1.AutoStart == true)
            {
                checkBox2.Checked = true;
            }
            else
            {
                checkBox2.Checked = false;
            }
        }
    }
}
