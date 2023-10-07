using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InOut_Monitoring_v1
{
    public partial class Form1 : Form
    {
        Settings1 settings1 = Settings1.Default;

        private Timer timer, timer2;

        SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\Kasun Perera\source\repos\InOut Monitoring v1\InOut Monitoring v1\InOut2.mdf"";Integrated Security=True;Connect Timeout=30");
        SqlCommand cmd = new SqlCommand();

        int lastID;
        string Query, LastDate, time1Str, time2Str, startDateStr, endDateStr, LastOutTime;
        TimeSpan difference, comparisonTime1, comparisonTime2;
        private void button1_Click(object sender, EventArgs e)
        {
            GetLastDate();

            if (LastDate == currentTime.ToString("yyyy/MM/dd"))
            {
                // Display a confirmation message box
                DialogResult result = MessageBox.Show("Do you want to reset your in time?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Check the user's choice
                if (result == DialogResult.Yes)
                {
                    // User clicked "Yes," so perform the desired action
                   // Console.WriteLine("User confirmed. Performing action...");

                    UpdateTime();
                    startDateStr = currentTime.ToString("yyyy/MM/dd"); // Format: yyyy-MM-dd
                    label1.Text = currentTime.AddHours(9).ToString("yyyy/MM/dd HH:mm:ss:tt");
                    label6.Text = currentTime.AddHours(4.5).ToString("yyyy/MM/dd HH:mm:ss:tt");
                    label5.Text = currentTime.ToString("yyyy/MM/dd HH:mm:ss:tt");

                }
                else
                {
                    // User clicked "No," so cancel the action
                  //  Console.WriteLine("User canceled the action.");
                }

            }
            else
            {
                GetlastID();

                conn.Open();

                lastID = lastID + 1;

                Query = "INSERT INTO InOut VALUES (@Id, @Date, @In, @Out, @Hours)";

                cmd = new SqlCommand(Query, conn);

                // Replace these with your actual date and time values.
                cmd.Parameters.AddWithValue("@ID", lastID);

                cmd.Parameters.AddWithValue("@Date", currentTime.ToString("yyyy/MM/dd"));
                cmd.Parameters.AddWithValue("@In", currentTime.ToString("yyyy/MM/dd HH:mm:ss:tt"));
                cmd.Parameters.AddWithValue("@Out", "");
                cmd.Parameters.AddWithValue("@Hours", "");


                cmd.ExecuteNonQuery();



                conn.Close();
            }

            View();
        }

        DateTime currentTime;

        public Form1()
        {
            InitializeComponent();

            // Initialize the Timer control
            timer = new Timer();
            timer.Interval = 1000; // 1000 milliseconds = 1 second
            timer.Tick += TimerTick;

            timer2 = new Timer();
            timer2.Interval = settings1.TimeMessage*1000;
            timer2.Tick += TimerTick2;

            timer.Start();
            timer2.Start();

            View();
            comboBox1.Text = "Full Day";
            comboBox1Changed();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            GetLastOutTime();

            if (LastOutTime != null)
            {
                // Display a confirmation message box
                DialogResult result2 = MessageBox.Show("Do you want to reset your out time?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // Check the user's choice
                if (result2 == DialogResult.Yes)
                {
                    UpdateLastOutTime();
                    //Console.WriteLine("UpdateLastOutTime1");
                }
                else
                {
                   // Console.WriteLine("User Click No");

                }
            }
            else
            {
                UpdateLastOutTime();
               // Console.WriteLine("UpdateLastOutTime2");

            }

            View();
        }

        private void TimerTick2(object sender, EventArgs e)
        {
            if (settings1.notificaton == true)
            {
                comparisonTime1 = new TimeSpan(9, 0, 0);
                comparisonTime2 = new TimeSpan(4, 3, 0);


                if (LastDate != currentTime.ToString("yyyy/MM/dd") && LastOutTime != null || LastDate == null && LastOutTime == null)
                {
                    SendNotification("Please use fingur print", "You did't use fingur print", "Please use fingur print & click set in time button");
                }

                else if (comboBox1.Text == "Full Day" && (difference > comparisonTime1) == true || comboBox1.Text == "Half Day" && (difference > comparisonTime2) == true)
                {
                    SendNotification("Please use fingur print", "You can leave now", "Please use fingur print & click set out time button");

                }
            }
            
        }

        private void SendNotification(String text, String title, String tiptext)
        {
            //notifyIcon1.Icon = new System.Drawing.Icon(Path.GetFullPath("alarm.ico"));
            notifyIcon1.Text = text;
            notifyIcon1.Visible = true;
            notifyIcon1.BalloonTipTitle = title;
            notifyIcon1.BalloonTipText = tiptext;
            notifyIcon1.ShowBalloonTip(100);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            currentTime = DateTime.Now;
            label10.Text = currentTime.ToString("yyyy/MM/dd HH:mm:ss:tt");
            GetLastOutTime();
            GetLastIn();
            CheckLastIn();
            GetLastDate();
        }

        private void CheckLastIn()
        {
            if (time1Str != null && label5.Text != "0000/00/00 00:00:00")
            {
                TimeCount();
            }
            else
            {
                label7.Text = "00:00:00";
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                SendNotification("", "", "Your Application has been minimized");
            }else if (this.WindowState == FormWindowState.Normal)
            {
                SendNotification("", "", "Your Application has come back to normal");
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result2 = MessageBox.Show("Do you want to Clear this table?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Check the user's choice
            if (result2 == DialogResult.Yes)
            {
               // Console.WriteLine("User clear the table");

                conn.Open();
                Query = "DELETE FROM InOut";
                cmd = new SqlCommand(Query, conn);
                cmd.ExecuteNonQuery();
                conn.Close();

                //UpdateTime();
                startDateStr = "";
                label1.Text = "0000/00/00 00:00:00";
                label6.Text = "0000/00/00 00:00:00";
                label5.Text = "0000/00/00 00:00:00";

                View();
            }
            else
            {
               // Console.WriteLine("User Click No");

            }
            
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog(); // Show Form2 as a modal dialog
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifyIcon1.Visible = false;
            notifyIcon1.Dispose();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon1.Visible = false;
            notifyIcon1.Dispose();
        }

        private void comboBox1Changed()
        {
            if (comboBox1.Text == "Full Day")
            {
                label1.Enabled = true;
                label5.Enabled = true;
                label6.Enabled = false;
            }
            else
            {
                label1.Enabled = false;
                label5.Enabled = true;
                label6.Enabled = true;
            }
        }

        private void UpdateTime()
        {
            conn.Open();
            Query = "update InOut set InTime=@In,OutTime=@Out,Hours=@Hours where Id = (select max(Id) from InOut)";
            cmd = new SqlCommand(Query, conn);

            cmd.Parameters.AddWithValue("@In", currentTime.ToString("yyyy/MM/dd HH:mm:ss:tt"));
            cmd.Parameters.AddWithValue("@Out", "");
            cmd.Parameters.AddWithValue("@Hours", "");

            // Execute the select command to get the last inserted ID
            object result = cmd.ExecuteNonQuery();

            if (result != null)
            {
                //Console.WriteLine("In time updated");
            }
            else
            {
               // Console.WriteLine("In time not updated");
            }

            conn.Close();
        }

        private void GetLastDate()
        {
            conn.Open();
            Query = "SELECT Date FROM InOut WHERE Id = (SELECT MAX(Id) FROM InOut);";
            cmd = new SqlCommand(Query, conn);

            // Execute the select command to get the last inserted ID
            object result = cmd.ExecuteScalar();

            if (result != null)
            {
                LastDate = result.ToString();
                startDateStr = LastDate;
            }
            else
            {
                //Console.WriteLine("LastDate not found in the table.");
                startDateStr = currentTime.ToString("yyyy/MM/dd"); // Format: yyyy-MM-dd

            }

            conn.Close();
        }

        private void GetlastID()
        {
            conn.Open();
            Query = "SELECT MAX(Id) FROM InOut";
            cmd = new SqlCommand(Query, conn);

            // Execute the select command to get the last inserted ID
            object result = cmd.ExecuteScalar();

            if (result != DBNull.Value)
            {
                lastID = Convert.ToInt32(result);
                //Console.WriteLine($"Last inserted ID: {lastID}");
            }
            else
            {
                //Console.WriteLine("No records found in the table.");
            }

            conn.Close();
        }

        private void UpdateLastOutTime()
        {
            conn.Open();
            Query = "update InOut set OutTime=@Out,Hours=@Hours where Id = (select max(Id) from InOut)";
            cmd = new SqlCommand(Query, conn);

            cmd.Parameters.AddWithValue("@Out", currentTime.ToString("yyyy/MM/dd HH:mm:ss:tt"));
            cmd.Parameters.AddWithValue("@Hours", label7.Text);

            // Execute the select command to get the last inserted ID
            object result = cmd.ExecuteNonQuery();

            if (result != null)
            {
                //Console.WriteLine("Out time updated");
            }
            else
            {
                //Console.WriteLine("Out time not updated");
            }

            conn.Close();
        }

        private void GetLastOutTime()
        {
            conn.Open();
            Query = "SELECT OutTime FROM InOut WHERE Id = (SELECT MAX(Id) FROM InOut)";
            cmd = new SqlCommand(Query, conn);

            // Execute the select command to get the last inserted ID
            object result = cmd.ExecuteScalar();

            if (result != null)
            {
                LastOutTime = result.ToString();
            }
            else
            {
                //Console.WriteLine("LastOutTime not found in the table.");

            }

            conn.Close();
        }

        private void TimeCount()
        {
            time2Str = currentTime.ToString("yyyy/MM/dd HH:mm:ss:tt");

            // Parse the date strings into DateTime objects
            DateTime date1 = DateTime.ParseExact(time1Str, "yyyy/MM/dd HH:mm:ss:tt", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(time2Str, "yyyy/MM/dd HH:mm:ss:tt", CultureInfo.InvariantCulture);

            // Calculate the time difference
            difference = date2 - date1;

            label7.Text = difference.ToString();
        }

        private void GetLastIn()
        {
            conn.Open();
            Query = "SELECT InTime FROM InOut WHERE Id = (SELECT MAX(Id) FROM InOut);";
            cmd = new SqlCommand(Query, conn);

            // Execute the select command to get the last inserted ID
            object result = cmd.ExecuteScalar();

            DateTime inputDateTime;

            if (result != null)
            {

                // Try parsing the input date and time
                if (DateTime.TryParseExact(result.ToString(), "yyyy/MM/dd HH:mm:ss:tt", null, DateTimeStyles.None, out inputDateTime))
                {
                    if (result.ToString().Split(' ')[0] == currentTime.ToString("yyyy/MM/dd"))
                    {
                        comboBox1Changed();

                        // Add 9 hours to the input date and time
                        label1.Text = inputDateTime.AddHours(9).ToString("yyyy/MM/dd HH:mm:ss:tt");
                        label6.Text = inputDateTime.AddHours(4.5).ToString("yyyy/MM/dd HH:mm:ss:tt");
                        time1Str = result.ToString();
                        label5.Text = time1Str;
                    }
                    else
                    {
                        if (LastOutTime == "")
                        {
                            if (DateTime.TryParseExact(result.ToString(), "yyyy/MM/dd HH:mm:ss:tt", null, DateTimeStyles.None, out inputDateTime))
                            {

                                comboBox1Changed();

                                // Add 9 hours to the input date and time
                                startDateStr = result.ToString();
                                label1.Text = inputDateTime.AddHours(9).ToString("yyyy/MM/dd HH:mm:ss:tt");
                                label6.Text = inputDateTime.AddHours(4.5).ToString("yyyy/MM/dd HH:mm:ss:tt");
                                time1Str = result.ToString();
                                label5.Text = time1Str;

                                //Console.WriteLine("time1Str " + time1Str);



                            }

                        }

                        else
                        {
                            label1.Enabled = false;
                            label6.Enabled = false;
                            label5.Enabled = false;
                            //Console.WriteLine("Invalid date and time format.");
                            //Console.WriteLine("time1Str " + time1Str);

                        }
                    }


                }
                else
                {
                    //Console.WriteLine("Invalid date and time format.");
                }

            }
            else
            {
               // Console.WriteLine("LastInTime not found in the table");
            }

            conn.Close();
        }

        private void View()
        {
            conn.Open();
            Query = "SELECT * FROM InOut";
            cmd = new SqlCommand(Query, conn);

            if (cmd != null)
            {
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            else
            {
               // Console.WriteLine("Data Not Found");
            }


            conn.Close();

        }
    }
}
