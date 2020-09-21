using Docker.DotNet.Models;
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

namespace Tsausova_Kinosaal
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Label[,] _arr = new Label[4, 4];
        Label[] read = new Label[4];
        Button osta, kinni;
        bool ost = false;
        readonly string[] arr_pilet;

        public int Port { get; private set; }
        public NetworkCredential Credentials { get; private set; }
        public bool EnableSsl { get; private set; }
        public object Interaction { get; private set; }

        private void Form1_Load(object sender, EventArgs e)
        {
            string text = "";
            StreamWriter to_file;
            if (!File.Exists("Kino.txt"))
            {
                to_file = new StreamWriter("Kino.txt", false);
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        text += i + "," + j + ",false";
                    }
                    text += "\n";
                }
                to_file.Write(text);
                to_file.Close();
            }
            StreamReader from_file = new StreamReader("Kino.txt", false);
            string[] arr = from_file.ReadToEnd().Split('\n');
            from_file.Close();
            this.Size = new Size(300, 430);
            //this.BackgroundImage = Image.FromFile("Images/saal.jpg");
            for (int i = 0; i < 4; i++)
            {
                read[i] = new Label();
                read[i].Text = "Rida" + (i + 1);
                read[i].Size = new Size(50, 50);
                read[i].Location = new Point(1, i * 50);
                this.Controls.Add(read[i]);

                for (int j = 0; j < 4; j++)
                {
                    _arr[i, j] = new Label();
                    string[] arv = arr[i].Split(';');
                    string[] ardNum = arv[j].Split(';');
                    if (ardNum[2] == "true")
                    {
                        _arr[i, j].BackColor = Color.Red;
                    }
                    else
                    {
                        _arr[i, j].BackColor = Color.Green;
                    }
                    _arr[i, j].Text = "Koht" + (j + 1); //"Rida" + i +
                    _arr[i, j].Size = new Size(50, 50);
                    _arr[i, j].BackColor = Color.Green;
                    _arr[i, j].BorderStyle = BorderStyle.Fixed3D;
                    _arr[i, j].Location = new Point(j * 50 + 50, i * 50);
                    this.Controls.Add(_arr[i, j]);
                    _arr[i, j].Tag = new int[] { i, j };
                    _arr[i, j].Click += new System.EventHandler(Form1_Click);
                }
            }
            osta = new Button();
            osta.Text = "Osta";
            osta.Location = new Point(50, 200);
            this.Controls.Add(osta);
            osta.Click += Osta_Click;
            kinni = new Button();
            kinni.Text = "Kinni";
            kinni.Location = new Point(150, 200);
            this.Controls.Add(kinni);
            kinni.Click += Kinni_Click;
        }

        private void Kinni_Click(object sender, EventArgs e)
        {
            string text = "";
            StreamWriter to_file;
            to_file = new StreamWriter("Kino.txt", false);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (_arr[i, j].BackColor == Color.Yellow)
                    {
                        Osta_Click_Func();
                    }
                }
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (_arr[i, j].BackColor == Color.Red)
                    {
                        text += i + "," + j + ",true;";
                    }
                    else
                    {
                        text += i + "," + j + ",false;";
                    }
                }
                text += "\n";
            }
            to_file.Write(text);
            to_file.Close();
            this.Close();
        }
        private void Osta_Click_Func()
        {
            if (ost == true)
            {
                var vastus = MessageBox.Show("Kas oled kindel?", "Appolo küsib", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (vastus == DialogResult.Yes)
                {
                    int t = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (_arr[i, j].BackColor == Color.Yellow)
                            {
                                t++;
                                _arr[i, j].BackColor = Color.Red;
                                //Сохранить каждый билет в файл
                                StreamWriter pilet = new StreamWriter("Pilet"+ (t+1).ToString()+ "Rida" + i.ToString()+"koht" + j.ToString()+".txt");
                                //arr_pilet.Append<string> ( "Pilet" + (t+1).ToString() + "Rida" + i.ToString() + "koht" + j.ToString() + ".txt");
                                pilet.WriteLine("Pilet" + (t + 1).ToString() + "Rida" + i.ToString() + "koht" + j.ToString() + ".txt");
                                pilet.Close();                       
                            }
                        }
                    }
                    Pilet_saada();
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (_arr[i, j].BackColor == Color.Yellow)
                            {
                                _arr[i, j].Text = "Koht" + (j + 1);
                                _arr[i, j].BackColor = Color.Green;
                                ost = false;
                            }
                        }
                    }
                }
            }
            else { MessageBox.Show("On vaja midagi valida!"); }

        }

        private void Pilet_saada()
        {
            string adress = Interaction.InputBox("Sisesta e-mail", "Kuhu saada", "anastassiatsausova23@gmail.com");
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                {
                    Port = 587;
                    //string password = Interaction.InputBox("Sisesta salasõna");
                    Credentials = new System.Net.NetworkCredential("anastassiatsausova23@gmail.com", "+37258291227");
                    EnableSsl = true;
                };
                mail.From = new MailAddress("anastassiatsausova23@gmail.com");
                mail.To.Add(adress);
                mail.Subject = "Pilet";
                mail.Body = "Pilet on ostetud";
                smtpClient.Send(mail);
                MessageBox.Show("Pilet oli saadetud mailile:" + adress);
            }
            catch (Exception)
            {
                MessageBox.Show("Viga");
            }
        }
        private void Insert_To_Database(int t, int i, int j)
        {
            string connectionString;
            SqlConnection con;
            connectionString = "@C:/USERS/OPILANE/SOURCE/REPOS/TSAUSOVA_KINOSAAL/KINOSAAL.MDF";
            con = new SqlConnection(connectionString);
            con.Open();
            SqlCommand command;
            string sql = "INSERT INTO Ostetud_Piletid(Id, Rida, Koht VALUES (" + t + "," + (i + 1) + "," + (j + 1) + ")";
            command = new SqlCommand(sql, con);
            command.ExecuteNonQuery();
            command.Dispose();
            con.Close();
            MessageBox.Show("Andmebaasi oli lisatud: ");
        }



        private void Osta_Click(object sender, EventArgs e)
        {
            Osta_Click_Func();
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            var label = (Label)sender; // запомнили на какую надпись нажали
            var tag = (int[])label.Tag; // определили координаты надписи

            if (_arr[tag[0], tag[1]].Text != "Kinni")
            {
                _arr[tag[0], tag[1]].Text = "Kinni";
                _arr[tag[0], tag[1]].BackColor = Color.Yellow;
                ost = true;
            }
            else
            {
                MessageBox.Show("Koht" + (tag[0] + 1) + (tag[1] + 1) + "koht on juba ostetud");
            }
            //_arr[1, 2].Text = "Это место уже куплено";
        }
    }
}
