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

/********消息格式：
 
# + 消息类型（0或1）+ $ + 消息发送人 + $ + 消息内容 + #

eg:  "#0$127.0.0.1$hello world#"

******************/

//聊天记录文件访问冲突，编码问题，服务器功能     进度线
namespace WindowsFormsChatter
{
    public partial class Form2 : Form
    {
        int counter = 0;//统计消息条数
        int room_number = -1;//所在房间
        Socket client;
        public Form2(object obj)
        {
            client = obj as Socket;
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }
        private void show_Result(string [] s)
        {
            ListViewGroup grp = new ListViewGroup();
            grp.Header = s[2];
            listView1.Groups.Add(grp);
            ListViewItem lvi = new ListViewItem();
            lvi.Text = s[s.Length - 2];
            lvi.Group = listView1.Groups[counter++];
            listView1.Items.Add(lvi);
            listView1.EnsureVisible(counter - 1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "")
            {
                return;//未输入时按动按钮无效
            }
            else
            {
                IPEndPoint client_ip = (IPEndPoint)client.LocalEndPoint;
                byte[] buffer = Encoding.Unicode.GetBytes("#0$" + client_ip.ToString()
                    + "$" + textBox1.Text + "#");
                client.Send(buffer);
                string content = "#1$" + client_ip.ToString()
                    + "$" + textBox1.Text + "#";
                string[] s = content.Split('#', '$');
                show_Result(s);
                if (!Directory.Exists(@".\ChatLog"))//存放该条内容到聊天记录中
                    Directory.CreateDirectory(@".\ChatLog");
                FileInfo fi = new FileInfo(@".\ChatLog\Chat_Room_" +
                        room_number.ToString() + ".txt");
                if (!fi.Exists)
                {
                    fi.Create().Close();
                }
                StreamWriter writer = new StreamWriter(fi.ToString(), true, Encoding.Unicode);
                writer.WriteLine("#0$" + client_ip.ToString() + "$" + textBox1.Text + "#");
                writer.Flush();
                writer.Close();
                if (textBox1.Text != "")//发送消息后清空输入
                    textBox1.Text = "";
            }
            //将聊天内容通过服务器发送给在同一聊天室的其他客户端
        }


        //private delegate int myDelegate(ComboBox combox);

        //private int get_Number(ComboBox combox) {
        //    if (combox.InvokeRequired)
        //    {
        //        myDelegate md = new myDelegate(this.get_Number);
        //        this.Invoke(md, new object[] { combox });
        //    }
        //    else {
        //        if (comboBox1.Text == "")
        //        {//未选择聊天室时不接收信息
        //            return -1;
        //        }
        //        else
        //        {
        //            room_number = int.Parse(this.comboBox1.Text.Substring(comboBox1.Text.Length - 1, 1));
        //            return room_number;
        //        }
        //    }
        //    return -1;
        //}


        public void receive_Msg()
        {
            //Socket client = obj as Socket;
            while (true)
            {
                try
                {
                    //if (get_Number(comboBox1) == -1) {//无法跳出此处 
                    //    continue;
                    //}
                    if (room_number == -1)//未选择房间，此时不接收消息
                        continue;
                    byte[] buffer = new byte[1024 * 1024];
                    int n = client.Receive(buffer);
                    System.Console.WriteLine(n);
                    string content = Encoding.Unicode.GetString(buffer, 0, n);
                    System.Console.WriteLine(content);
                    string[] temp = content.Split('\0');
                    content = temp[0];
                    System.Console.WriteLine(content);
                    string[] s = content.Split('#', '$');
                    show_Result(s);//显示接收的消息
                    if (!Directory.Exists(@".\ChatLog"))//存放该条内容到聊天记录中
                        Directory.CreateDirectory(@".\ChatLog");
                    FileInfo fi = new FileInfo(@".\ChatLog\Chat_Room_" +
                            room_number.ToString() + ".txt");
                    if (!fi.Exists) { 
                        fi.Create().Close();
                    }
                    StreamWriter writer = new StreamWriter(fi.ToString(), true, Encoding.Unicode );
                    writer.WriteLine(content);
                    writer.Flush();
                    writer.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    break;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)//
        {
            if (button1.Enabled)//非首次选择聊天室，清空输出区
            {
                listView1.Groups.Clear();
                listView1.Items.Clear();
                listView1.Columns.Clear();
                counter = 0;
            }
            button1.Enabled = true;//在选择了聊天室后才可发送信息
            room_number = int.Parse(this.comboBox1.Text.Substring//获取房间号
                (this.comboBox1.Text.Length - 1, 1));
            IPEndPoint client_ip = (IPEndPoint)client.RemoteEndPoint;
            byte[] buffer = Encoding.Unicode.GetBytes("#1$" + client_ip.ToString()
                + "$" + room_number.ToString() + "#");
            client.Send(buffer);
            //变更了聊天室应先将聊天室信息发送给服务器
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
                            room_number.ToString() + ".txt");
                    if (!fi.Exists)//第一次进入此聊天室，创建txt文件即可
                    {
                        fi.Create().Close();
                    }
                    else//将聊天记录显示到界面上
                    {
                        /*
                         * 用FileStream对象读取文件并显示到listview中
                         */
                        StreamReader reader = new StreamReader(fi.ToString(),Encoding.Unicode);
                        string content;
                        while ((content = reader.ReadLine()) != null)
                        {
                            content = content.Trim().ToString();
                            string[] s = content.Split('#', '$');
                            show_Result(s);
                        }
                        reader.Close();
                    }
                }
            }catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            //读取对应聊天室的聊天记录文件
        }

        void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(
                    "是否要退出客户端",
                    "提示",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question) != DialogResult.OK)
            {
                e.Cancel = true;
                System.Environment.Exit(0);
                return;
            }
        }
    }
}
