using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Threading;

namespace Ping
{
    public partial class Form1 : Form
    {
        string pathfile;
        int delay;
        int countTry;
        public Form1()
        {
            InitializeComponent();
        }

        private async void check(object sender, EventArgs e)
        {
            try
            {
                delay = int.Parse(textBox2.Text) * 1000;
                if (delay > 0)
                {
                    delay = int.Parse(textBox2.Text) * 1000;
                } else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Задержка не верна, установлено значение по умолчанию", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox2.Text = 10.ToString();
                delay = int.Parse(textBox2.Text) * 1000;
            }

            try
            {
                countTry = int.Parse(textBox3.Text);
                if (countTry > 0)
                {
                    delay = int.Parse(textBox3.Text) * 1000;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Кол-во попыток установлено неверно, установлено значение по умолчанию", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBox3.Text = 1.ToString();
                countTry = int.Parse(textBox3.Text);
            }

            FileStream fs;
            StreamReader sr;
            List<String> ip = null;
            label4.Visible = false;
            try
            {
                fs = new FileStream(pathfile, FileMode.Open);
                sr = new StreamReader(fs);
                ip = new List<string>();
                while (!sr.EndOfStream)
                {
                    ip.Add(sr.ReadLine());
                }
                sr.Dispose();
                fs.Dispose();
                progressBar1.Value = 0;
                progressBar1.Maximum = ip.Count;
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                var list = new List<Task>();
                foreach (var item in ip)
                {
                    list.Add(SendPing(item));
                }
                await Task.WhenAll(list);
                label4.Visible = true;
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Путь не верен", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
            }

            

        }

        async private Task SendPing(string host)
        {
            listBox1.Items.Clear();
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
            try
            {
                PingReply pr = null;
                for (int i = 1; i <= countTry; i++)
                {
                    pr = await ping.SendPingAsync(host, delay);
                    if (pr.Status == IPStatus.Success)
                    {
                        break;
                    }
                    await Task.Delay(5000);
                }
                listBox1.Items.Add($"{host};{pr.Status}");
                progressBar1.Value++;
            }
            catch (PingException)
            {
                listBox1.Items.Add($"{host};Unknown Host");
                progressBar1.Value++;
            }
            
        }

        private void export(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "export.csv";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream fs;
                StreamWriter sw;
                try
                {
                    fs = new FileStream(saveFileDialog1.FileName, FileMode.Create);
                    sw = new StreamWriter(fs);
                }
                catch (Exception exc)
                {
                    throw exc;
                }
                foreach (var item in listBox1.Items)
                {
                    sw.Write(item.ToString()+"\n");
                }

                sw.Dispose();
                fs.Dispose();
            }
        }

        private void openfile(object sender, EventArgs e)
        {
            openFileDialog1.FileName = null;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pathfile = openFileDialog1.FileName;
                textBox1.Text = pathfile;
            }
            label4.Visible = false;
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Disshell");

        }
    }
}
