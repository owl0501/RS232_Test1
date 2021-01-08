using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace RS232_Test1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //序列埠物件
        SerialPort sport1 = new SerialPort();
        //傳送檢查
        private bool isSending =false;
        //
        private void Form1_Load(object sender, EventArgs e)
        {
            isSending = false;
            GetPorts();
            
        }
        //委派事件
        delegate void dlgMession(string st);
        private void displayText(string st)
        {
            richTextBox1.AppendText(st+"\r\n");
        }
        //取得序列埠
        private void GetPorts()
        {
            string[] portnames = SerialPort.GetPortNames();
            foreach (var item in portnames)
            {
                comboBox1.Items.Add(item);
            }
        }
        //連線
        private void Connected()
        {
            try
            {
                //鮑率
                sport1.BaudRate = 9600;
                //資料位元
                sport1.DataBits = 8;
                sport1.PortName = comboBox1.Text;
                //兩個停止位
                sport1.StopBits = System.IO.Ports.StopBits.One;
                //無奇偶校驗位
                sport1.Parity = System.IO.Ports.Parity.None;
                sport1.ReadTimeout = 100;
                //開啟SerialPort
                sport1.Open();
                if (!sport1.IsOpen)
                {
                    MessageBox.Show("連線失敗");
                    return;
                }
                else
                {
                    richTextBox1.AppendText(comboBox1.Text + "連線成功\r\n");
                }
            }
            catch (Exception ex)
            {
                //釋放(關閉)SerialPort
                sport1.Dispose();
                richTextBox1.AppendText(ex.Message);
            }
        }
        //中斷連線
        private void Disconnected()
        {
            try
            {
                if (sport1.IsOpen)
                {
                    sport1.Close();
                    richTextBox1.AppendText($"{sport1.PortName}已中斷連線...\r\n");
                }
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText(ex.Message);
            }
        }
        //資料傳送
        private void Sending()
        {
            isSending = true;
            try 
            {
                //sport1.Write(textBox2.Text);
                sport1.Write(textBox2.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }
            finally
            {
                isSending = false;
            }
        }
        //資料接收
        private void Received()
        {
            try
            {
                int len = sport1.BytesToRead;
                string receivedData = string.Empty;
                if (len != 0)
                {
                    byte[] buff = new byte[len];
                    sport1.Read(buff, 0, len);
                    receivedData = Encoding.Default.GetString(buff);
                }
                //MessageBox.Show($"{receivedData}");
                //richTextBox1.AppendText(receivedData + "\r\n");
                dlgMession dlg = new dlgMession(displayText);
                this.Invoke(dlg, receivedData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }


        }
        #region--按鈕事件--
        private void btnConnected_Click(object sender, EventArgs e)
        {
            Connected();
        }
        //
        private void btnDisconnected_Click(object sender, EventArgs e)
        {
            Disconnected();
        }
        //
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!isSending)
            {
                Thread t = new Thread(Sending);
                t.IsBackground = true;
                t.Start();
            }
        }
        //
        private void btnRecieved_Click(object sender, EventArgs e)
        {
            Received();
        }

        #endregion
    }
}
