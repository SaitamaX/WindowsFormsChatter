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
        int counter = 0;//统计消息条数
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //将聊天内容通过服务器发送给在同一聊天室的其他客户端
            //将聊天内容存放到对应的聊天室的文件中
        }
        public void ReceiveMsg(object obj)
        {
            Socket client = obj as Socket;
            while (true)
            {
                try
                {
                    if (comboBox1.Text == "")//未选择聊天室时不接收信息
                        continue;
                    else {
                        int chat_room_number = int.Parse(this.comboBox1.Text.Substring//获取房间号
                        (comboBox1.Text.Length - 1, 1));
                    }
                    byte[] buffer = new byte[1024 * 1024];
                    int n = client.Receive(buffer);
                    string content = Encoding.Unicode.GetString(buffer, 0, n);
                    content = content.Trim().ToString();
                    string[] s = content.Split('#', '$');
                    ListViewGroup grp = new ListViewGroup();
                    grp.Header = s[1];
                    listView1.Groups.Add(grp);
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = s[s.Length - 2];
                    lvi.Group = listView1.Groups[counter++];
                    listView1.Items.Add(lvi);
                    if (!Directory.Exists(@".\ChatLog"))//第一次时要先创建文件夹
                        Directory.CreateDirectory(@".\ChatLog");
                    else
                    {
                        FileInfo fi = new FileInfo(@".\ChatLog\Chat_Room_" +
                                chat_room_number.ToString() + ".txt");
                        if (!fi.Exists)
                        {
                            fi.Create();
                        }
                        else
                        {
                        }
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
            try
            {
                if (!Directory.Exists(@".\ChatLog"))//第一次时要先创建文件夹
                    Directory.CreateDirectory(@".\ChatLog");
                else
                {
                    FileInfo fi = new FileInfo(@".\ChatLog\Chat_Room_" +
                            chat_room_number.ToString() + ".txt");
                    if (!fi.Exists)
                    {
                        fi.Create();
                    }
                    else
                    {
                        /*
                         * 用FileStream对象读取文件并显示到listview中
                         */
                        StreamReader reader = new StreamReader(fi.OpenRead(),
                            UnicodeEncoding.GetEncoding("utf-16"));
                        string content = string.Empty;
                        int counter = 0;//统计消息条数
                        while((content = reader.ReadLine()) != null)
                        {
                            content = content.Trim().ToString();
                            string[] s = content.Split('#','$');
                            ListViewGroup grp = new ListViewGroup();
                            grp.Header = s[1];
                            listView1.Groups.Add(grp);
                            ListViewItem lvi = new ListViewItem();
                            lvi.Text = s[s.Length - 2];
                            lvi.Group = listView1.Groups[counter++];
                            listView1.Items.Add(lvi);
                        }
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
