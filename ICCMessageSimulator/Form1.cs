using BestoryIntelligentControlClientLibrary;
using JashOSC;
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
using Utils;

namespace CommandCenterTestApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public Dictionary<string, string> SettingFile = new Dictionary<string, string>();

        BestoryOSCClient clientInstance = null;
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Send()
        {
            clientInstance = BestoryOSCClient.GetInstance(
                tb_IP.Text,
                int.Parse(tb_port.Text), tb_CenterName.Text, false, true);

            OSCMessage msg = new OSCMessage(string.Format("/{0}/{1}", tb_CenterName.Text, tb_Address.Text)).Add(tb_ProjectName.Text);

            for (int i = 0; i != dgv_data.Rows.Count; i++)
            {
                DataGridViewRow row = dgv_data.Rows[i];

                string type = row.Cells[0].EditedFormattedValue.ToString();
                string data = row.Cells[1].EditedFormattedValue.ToString();

                switch (type.ToLower())
                {
                    case "string":
                        msg.Add(data);
                        break;
                    case "int":
                        msg.Add(int.Parse(data));
                        break;
                    case "float":
                        msg.Add(float.Parse(data));
                        break;
                    case "bool":
                        msg.Add(bool.Parse(data));
                        break;
                }
            }
            byte[] bytes = null;
            msg.ToBytes(out bytes);
            tb_bytes.Text = ToHexString(bytes);
            Clipboard.SetDataObject(tb_bytes.Text);//复制内容到粘贴板
            clientInstance.SendMessage(msg);
            //clientInstance.Shutdown();
        }

        private void LoadFile(string path)
        {
            dgv_data.Rows.Clear();

            LoadSettings.Instance.Load_Settings(Path.GetDirectoryName(path), path);

            tb_IP.Text = LoadSettings.Instance.settingsProfile.CenterIP;
            tb_port.Text = LoadSettings.Instance.settingsProfile.CenterPort.ToString();
            tb_CenterName.Text = LoadSettings.Instance.settingsProfile.CenterName = tb_CenterName.Text;
            tb_Address.Text = LoadSettings.Instance.settingsProfile.oscAddress;
            tb_ProjectName.Text = LoadSettings.Instance.settingsProfile.projectName;
            string str = LoadSettings.Instance.settingsProfile.data;
            string[] strs = str.Split(new Char[] { '(', ')' });

            foreach (var item in strs)
            {
                if (item != "")
                {
                    string[] item_s = item.Split('@');
                    additem(item_s[0], item_s[1]);
                }
            }
        }

        /// <summary>
        /// Add DataGridView item
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        public void additem(string v1, string v2)
        {
            DataGridViewRow dgvr = new DataGridViewRow();
            foreach (DataGridViewColumn c in this.dgv_data.Columns)
            {
                dgvr.Cells.Add(c.CellTemplate.Clone() as DataGridViewCell);
            }
            dgvr.Cells[0].Value = v1;
            dgvr.Cells[1].Value = v2;
            this.dgv_data.Rows.Add(dgvr);
        }

        /// <summary>
        /// Refresh drop-down box
        /// </summary>
        private void RefreshList(string path)
        {
            if (path == "") return;
            if (comboBox1.Items.Count > 0)
                comboBox1.Items.Clear();
            SettingFile = new Dictionary<string, string>();

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string[] files = Directory.GetFiles(path);
            foreach (var item in files)
            {
                string fileType = Path.GetExtension(item);
                string fileName = Path.GetFileNameWithoutExtension(item);
                switch (fileType)
                {
                    case ".osc":
                    //case ".ini":
                        comboBox1.Items.Add(fileName);
                        SettingFile.Add(fileName, item);
                        Console.WriteLine("文件名：" + fileName + "文件位置：" + item);
                        break;
                }
            }
            if (comboBox1.Items.Count <=  0)
            {
                LoadSettings.Instance.Load_Settings(IniFile.defaultFolderPath, IniFile.defaultFilePath);
                RefreshList(IniFile.defaultFolderPath);
                //LoadSettings.Instance.Load_Settings(path, Path.Combine(path, "AppConfig.ini"));
                //RefreshList(path);
                return;
            }
            else
            {
                comboBox1.Text = comboBox1.Items[0].ToString();
            }
        }

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            tb_txt.Text = PathHelper.GetSelectDirectory();
            //默认加载第一条
            RefreshList(tb_txt.Text);
        }

        /// <summary>
        /// 添加新的一条命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (!SettingFile.ContainsKey(comboBox1.Text))
            {
                // 添加
                Save();
                LoadSettings.Instance.Save_Settings(tb_txt.Text, Path.Combine(tb_txt.Text, comboBox1.Text + ".osc"));
                RefreshList(tb_txt.Text);//刷新
            }
            else
            {
                MessageBox.Show("文件名重复");
            }
        }

        /// <summary>
        /// 删除当前命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            string path = "";
            if (SettingFile.TryGetValue(comboBox1.Text, out path))
            {
                // 删除当前命令
                PathHelper.DeleteFile(path);
                RefreshList(tb_txt.Text);//刷新
            }
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Send();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save()
        {
            LoadSettings.Instance.settingsProfile.CenterIP = tb_IP.Text;
            LoadSettings.Instance.settingsProfile.CenterPort = int.Parse(tb_port.Text.Trim());
            LoadSettings.Instance.settingsProfile.CenterName = tb_CenterName.Text;
            LoadSettings.Instance.settingsProfile.oscAddress = tb_Address.Text;
            LoadSettings.Instance.settingsProfile.projectName = tb_ProjectName.Text;
            string str = "";

            foreach (DataGridViewRow c in this.dgv_data.Rows)
            {
                if (c.Cells[0].Value == null) continue;
                string str1 = "(" + c.Cells[0].Value + "@" + c.Cells[1].Value + ")";
                str += str1;
            }
            LoadSettings.Instance.settingsProfile.data = str;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            tb_txt.Text = IniFile.defaultFolderPath;
            //默认加载第一条
            RefreshList(tb_txt.Text);
        }

        /// <summary>
        /// 鼠标悬停显示输入框的文字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tb_txt_MouseHover(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty((sender as Control).Text))
            {
                if (tooltip != null)
                    tooltip.Dispose();
                tooltip = new ToolTip();
                tooltip.AutoPopDelay = 10000;
                tooltip.InitialDelay = 100;
                tooltip.ReshowDelay = 100;
                tooltip.SetToolTip((sender as Control), ((sender as Control).Text));
            }
        }

        /// <summary>
        /// 改变下拉框时加载配置文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string path = "";
            if (SettingFile.TryGetValue(comboBox1.Text, out path))
            {
                if (clientInstance != null)
                    clientInstance.Shutdown();
                LoadFile(path);
            }
        }

        /// <summary>
        /// 保存当前配置文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_save_Click(object sender, EventArgs e)
        {
            string path = "";
            if (SettingFile.TryGetValue(comboBox1.Text, out path))
            {
                Save();
                //保存
                LoadSettings.Instance.Save_Settings(Path.GetDirectoryName(path), path);
            }
        }

        public static string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                System.Text.StringBuilder strB = new System.Text.StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(" ");
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }
        private byte[] StringToBytes(string s)
        {
            string[] str = s.Split(' ');
            int n = str.Length;

            byte[] cmdBytes = null;
            int p = 0;


            for (int k = 0; k < n; k++)
            {
                int sLen = str[k].Length;
                int bytesLen = sLen / 2;
                int position = 0;
                byte[] bytes = new byte[bytesLen];
                for (int i = 0; i < bytesLen; i++)
                {
                    string abyte = str[k].Substring(position, 2);
                    bytes[i] = Convert.ToByte(abyte, 16);
                    position += 2;
                }

                if (position >= 2)
                {
                    byte[] cmdBytes2 = new byte[p + bytesLen];
                    if (cmdBytes != null)
                    {
                        Array.Copy(cmdBytes, 0, cmdBytes2, 0, p);
                    }
                    Array.Copy(bytes, 0, cmdBytes2, p, bytesLen);
                    cmdBytes = cmdBytes2;
                    p += bytesLen;
                }
            }

            return cmdBytes;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (clientInstance != null)
                clientInstance.Shutdown();
        }
    }
}
////1、ASII码：字符串中的每个字符用一个字节表示。
////每个字符实际上只使用了7位，从00h - 7Fh。只能表达128个字符。不能代表汉字，

//byte[] b = Encoding.ASCII.GetBytes("yourstring");
//Console.Write(string.Join("-", b.Select(p => p.ToString())));
////  得到：121-111-117-114-115-116-114-105-110-103   

////2、Unicode码：字符串中的每个字符用两个字节表示。

//byte[] b = Encoding.Unicode.GetBytes("哈啊a1");
////  得到：200-84-74-85-97-0-49-0

////3、简体中文：字符串中的每个中文字符用两个字节表示，其他字符用一个字节表示。


//byte[] b = Encoding.GetEncoding("gb2312").GetBytes("哈啊a1");//繁体中文”big5”
//Console.Write(string.Join("-", b.Select(p => p.ToString())));
////  得到：185-254-176-161-97-49

////UTF-8中，一个汉字对应三个字节，GB2312中一个汉字占用两个字节。 
////不论何种编码，字母数字都不编码，特殊符号编码后占用一个字节。

////二、解码（转成字符串）：GetString、GetChars

//Encoding.XXX.GetString(byte[] data,[, index, count]);
////三、基于Base64（ASCII）编码的字符串与字节数组的转换
////1、将指定的字符串（它将二进制数据编码为 Base64 数字）转换为等效的 8 位无符号整数数组。

//byte[] bt = Convert.FromBase64String("字符串");
////2、将 8 位无符号整数数组的值转换为其用 Base64 数字编码的等效字符串表示形式。

//Convert.ToBase64String(byte[] data,[, index, count]);