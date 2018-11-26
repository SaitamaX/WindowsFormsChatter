using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace WindowsFormsChatter
{
    public partial class Form2 : Form
    {
        int counter = 0;//统计消息条数
        public Form2()
        {
            InitializeComponent();
        }
        private void show_Result(string str)
        {
            str = str.Trim().ToString();
            string[] s = str.Split('#', '$');
            ListViewGroup grp = new ListViewGroup();
            grp.Header = s[1];
            listView1.Groups.Add(grp);
            ListViewItem lvi = new ListViewItem();
            lvi.Text = s[s.Length - 2];
            lvi.Group = listView1.Groups[counter++];
            listView1.Items.Add(lvi);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //将聊天内容通过服务器发送给在同一聊天室的其他客户端
            //将聊天内容存放到对应的聊天室的文件中
        }


        private delegate int myDelegate(ComboBox combox);

        private int get_Number(ComboBox combox) {
            if (combox.InvokeRequired)
            {
                myDelegate md = new myDelegate(this.get_Number);
                this.Invoke(md, new object[] { combox });
            }
            else {
                if (comboBox1.Text == "")
                {//未选择聊天室时不接收信息
                    return -1;
                }
                else
                {
                    int chat_room_number = int.Parse(this.comboBox1.Text.Substring(comboBox1.Text.Length - 1, 1));
                    return chat_room_number;
                }
            }
            return -1;
        }


        public void receive_Msg(object obj)
        {
            Socket client = obj as Socket;
            int chat_room_number;
            while (true)
            {
                try
                {
                    if ((chat_room_number = get_Number(comboBox1)) == -1) {
                        continue;
                    }
                    byte[] buffer = new byte[1024 * 1024];
                    IPEndPoint client_ip = (IPEndPoint)client.RemoteEndPoint;
                    int n = client.Receive(buffer);
                    string content = Encoding.Unicode.GetString(buffer, 0, n);
                    show_Result(content);
                    if (!Directory.Exists(@".\ChatLog"))//存放该条内容到聊天记录中
                        Directory.CreateDirectory(@".\ChatLog");
                    FileInfo fi = new FileInfo(@".\ChatLog\Chat_Room_" +
                            chat_room_number.ToString() + ".txt");
                    if (!fi.Exists) { 
                        fi.Create();
                    }
                    StreamWriter writer = new StreamWriter(fi.OpenRead(),
                        UnicodeEncoding.GetEncoding("utf-16"));
                    writer.WriteLine("#" + client_ip.ToString() + "$" + content + "#");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    break;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int chat_room_number = int.Parse(this.comboBox1. Text.Substring//获取房间号
                (this.comboBox1.Text.Length - 1, 1));
            ColumnHeader clh = new ColumnHeader();
            clh.Text = "欢迎进入聊天室";
            clh.Width = 220;
            listView1.Columns.Add(clh);
            try
            {
                if (!Directory.Exists(@".\ChatLog"))
                {//第一次时要先创建文件夹
                    Directory.CreateDirectory(@".\ChatLog");
                }
                else
                {
                    FileInfo fi = new FileInfo(@".\ChatLog\Chat_Room_" +
                            chat_room_number.ToString() + ".txt");
                    if (!fi.Exists)//第一次进入此聊天室，创建txt文件即可
                    {
                        fi.Create();
                    }
                    else//将聊天记录显示到界面上
                    {
                        /*
                         * 用FileStream对象读取文件并显示到listview中
                         */
                        StreamReader reader = new StreamReader(fi.OpenRead(),
                            UnicodeEncoding.GetEncoding("utf-16"));
                        string content = string.Empty;
                        while ((content = reader.ReadLine()) != null)
                            show_Result(content);
                    }
                }
            }catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            //读取对应聊天室的聊天记录文件
        }
    }
}
