using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConcentrateSensorClient
{
    public partial class Form1 : Form
    {
        internal Byte[] outBuffer = new Byte[256];
        readonly SynchronizationContext syncContext;
        IPAddress ipAddr = null;
        IPEndPoint endPoint = null;
        TcpClient tcpClient;
        NetworkStream tcpStream;
        Thread th;

        public Form1()
        {
            InitializeComponent();
            FormClosing += Form1_FormClosing;
            FormClosed += Form1_FormClosed;

            syncContext = SynchronizationContext.Current;
        }

        void Display(object value)
        {
            labelData.Text = BitConverter.ToSingle((byte[])value, 0).ToString("F") + " %";
        }

        void SetStatus(object str) => labelConnState.Text = 
            labelConnState.Text.Substring(0, 23) + "\n" + str.ToString();

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            if (tcpClient != null)
            {
                if (tcpClient.Connected)
                {
                    if (MessageBox.Show(this, "Желаете прервать соединение?", "",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        button1.Enabled = true;
                        return;
                    }
                }
                Disconnection();
                syncContext.Post(SetStatus, "Соединение прервано.");
            }

            try
            {
                ipAddr = IPAddress.Parse(textBoxIP.Text);
                endPoint = new IPEndPoint(ipAddr, Int32.Parse(textBoxPort.Text));
            }
            catch (Exception ex)
            {
                SetStatus(ex.Message);
                button1.Enabled = true;
                return;
            }

            try
            {
                {
                    Ping netMon = new Ping();
                    PingReply reply = netMon.Send(ipAddr, 1000);
                    if (reply.Status == IPStatus.TimedOut)
                    {
                        SetStatus("Время пинга вышло - соединение не установлено!");
                        button1.Enabled = true;
                        return;
                    }
                }

                tcpClient = new TcpClient();
                tcpClient.ReceiveTimeout = tcpClient.SendTimeout = 2000;
                tcpClient.Connect(endPoint);
                
                tcpStream = tcpClient.GetStream();

                th = new Thread(ReceiveRun);
                th.Start();

                syncContext.Post(SetStatus, "Соединение успешно!");
            }
            catch (SocketException ex)
            {
                syncContext.Post(SetStatus, ex.Message);
            }
            catch (Exception ex)
            {
                syncContext.Post(SetStatus, ex.Message);
            }
            finally
            {
                button1.Enabled = true;
            }
        }

        void ReceiveRun()
        {
            try
            {
                while (tcpClient.Connected)
                {
                    if (tcpStream.CanRead)
                    {
                        byte[] bytes = new byte[256];
                        while (tcpStream.DataAvailable)
                        {
                            tcpStream.Read(bytes, 0, 256);
                            syncContext.Post(Display, new byte[] { bytes[0], bytes[1], bytes[2], bytes[3] });
                            if (bytes[4] == 0)
                            {
                                MessageBox.Show("Сервер отключен от датчика.");
                            }
                        }
                    }
                    else
                    {
                        Disconnection();
                        syncContext.Post(SetStatus, "Соединение прервано.");
                        return;
                    }
                }
            }
            catch(ThreadAbortException)
            {
                //syncContext.Post(SetStatus, "Поток закрыт.");
            }
            catch (Exception e)
            {
                syncContext.Post(SetStatus, e.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Disconnection();
            syncContext.Post(SetStatus, "Соединение прервано.");
        }

        void Disconnection()
        {
            if (th != null)
                th.Abort();
            if (tcpClient != null)
            {
                tcpClient.Close();

                if (tcpStream != null)
                    tcpStream.Close();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tcpClient != null/* && tcpClient.Connected*/)
                Disconnection();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (tcpClient != null/* && tcpClient.Connected*/)
                Disconnection();
        }
    }
}
