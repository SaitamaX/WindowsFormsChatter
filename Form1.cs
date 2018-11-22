using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;


namespace WindowsFormsChatter
{
    public partial class Form1 : Form
    {
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            //临时测试
            Form2 form2 = new Form2();
            form2.Show();
            try
            {
                IPAddress ip = IPAddress.Parse(textBox1.Text);
                IPEndPoint point = new IPEndPoint(ip, int.Parse(textBox2.Text));
                client.Connect(point);
                this.Hide();
               // Form2 form2 = new Form2();
                Thread th = new Thread(form2.ReceiveMsg);//进度线
                th.IsBackground = true;
                th.Start();
                form2.Show();
            }
            catch(Exception ex)
            {
                MessageBox.Show("未能连接服务器,错误信息如下：\r\n" + ex.Message,"Error");
            }
        }
    }
}
