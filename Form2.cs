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

namespace WindowsFormsChatter
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //显示聊天记录其他内容
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //将聊天内容通过服务器发送给在同一聊天室的其他客户端
            //将聊天内容存放到对应的聊天室的文件中
        }
        public void ReceiveMsg(object s)
        {
            Socket client = s as Socket;
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024];
                    int n = client.Receive(buffer);
                    string words = Encoding.UTF8.GetString(buffer, 0, n);
                    listView1.Items.Add(words);
                    //解析数据并显示到listview中
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
            int chat_room_number = int.Parse(this.comboBox1.Text.Substring//获取房间号
                (this.comboBox1.Text.Length - 1, 1));
            ColumnHeader clh = new ColumnHeader();
            clh.Text = "聊天室" + chat_room_number.ToString();
            clh.Width = 220;
            listView1.Columns.Add(clh);
            FileInfo fi;//
            try
            {
                if (!Directory.Exists(@".\ChatLog"))//第一次时要先创建文件夹
                    Directory.CreateDirectory(@".\ChatLog");
                else
                {
                    if (File.Exists(@".\ChatLog\Chat_Room_" + chat_room_number.ToString() + ".txt"))
                        fi = new FileInfo(@".\ChatLog\Chat_Room_" + 
                            chat_room_number.ToString() + ".txt");

                }
                ListViewGroup grp = new ListViewGroup();
                grp.Header = "我";
                listView1.Groups.Add(grp);
                ListViewItem lvi = new ListViewItem();
                lvi.Text = "this is first message";
                lvi.Group = listView1.Groups[0];
                listView1.Items.Add(lvi);
            }catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            //读取对应聊天室的聊天记录文件
        }
       
    }
}
