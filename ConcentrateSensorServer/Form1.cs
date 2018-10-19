using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ConcentrateSensorServer
{
    public partial class Form1 : Form
    {
        TcpListener server;
        Thread serverSender;
        IPAddress localAddr = null;
        readonly SynchronizationContext syncContext;
        bool serverStarted;

        
        public Form1()
        {
            InitializeComponent();
            serverStarted = false;
            syncContext = SynchronizationContext.Current;
        }

        void AddText(object s) => listBox1.Items.Add(s);

        void CopyData(ref byte[] destArr, ref byte[] sourceArr, ref int ind)
        {
            for (int i = 0; i < sourceArr.Length; i++)
            {
                destArr[ind] = sourceArr[i];
                ind++;
            }
        }

        void ServerWorking()
        {
            TcpClient client = null;
            while (true)
            {
                try
                {
                    syncContext.Post(AddText, "Ожидание подключений... ");
                    
                    // получаем входящее подключение
                    using (client = server.AcceptTcpClient())
                    {
                        syncContext.Post(AddText, "Подключен клиент.");

                        // получаем сетевой поток для чтения и записи
                        NetworkStream stream = client.GetStream();
                        Random rnd = new Random();
                        
                        // сообщение для отправки клиенту 
                        float response = 1f;
                        while (client.Connected)
                        {
                            // преобразуем сообщение в массив байтов
                            byte[] outdata = new byte[256];
                            byte[] data = BitConverter.GetBytes(response);
                            int ind = 0;
                            CopyData(ref outdata, ref data, ref ind);

                            data = new byte[] { 0, 0, 0, 1 }; // up
                            CopyData(ref outdata, ref data, ref ind);

                            data = new byte[] { 0, 0, 0, 2 }; // down
                            CopyData(ref outdata, ref data, ref ind);

                            data = new byte[] { 0, 0, 0, 3 }; // sum
                            CopyData(ref outdata, ref data, ref ind);

                            data = new byte[] { 0, 0, 0, 4 }; // dumpsum
                            CopyData(ref outdata, ref data, ref ind);

                            data = new byte[] { 0, 15 }; // itrowel
                            CopyData(ref outdata, ref data, ref ind);

                            data = new byte[] { 0, 20 }; // irevers
                            CopyData(ref outdata, ref data, ref ind);

                            data = new byte[] { 0, 30 }; // dump
                            CopyData(ref outdata, ref data, ref ind);

                            data = new byte[] { 0, 200 }; // period_answer
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(1.23f); // a
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(2.34f); // b
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(3.45f); // c
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(4.56f); // d
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(5.67f); // e
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(6.78f); // n
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(7.89f); //f
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(8.90f); // g
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(9.01f); // h
                            CopyData(ref outdata, ref data, ref ind);

                            data = localAddr.GetAddressBytes(); // ip
                            CopyData(ref outdata, ref data, ref ind);

                            data = new byte[] { 255, 255, 255, 10 }; // mask
                            CopyData(ref outdata, ref data, ref ind);

                            data = new byte[] { 192, 168, 99, 11 }; // gw
                            CopyData(ref outdata, ref data, ref ind);

                            data = new byte[] { 192, 168, 99, 12 }; // pridns
                            CopyData(ref outdata, ref data, ref ind);

                            data = new byte[] { 192, 168, 99, 13 }; // secDns
                            CopyData(ref outdata, ref data, ref ind);

                            outdata[ind] = 160;
                            ind++;
                            outdata[ind] = 15;
                            ind++;

                            outdata[ind] = 160;
                            ind++;
                            outdata[ind] = 15;
                            ind++;

                            outdata[ind] = 160;
                            ind++;
                            outdata[ind] = 15;
                            ind++;

                            data = new byte[] { 192, 168, 99, 101 }; // remIP
                            CopyData(ref outdata, ref data, ref ind);


                            data = BitConverter.GetBytes(4.0f); // i4
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(20.0f); // i20
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(response);
                            int inex = 107;
                            CopyData(ref outdata, ref data, ref inex);

                            outdata[127] = 20;
                            outdata[128] = 30;
                            outdata[129] = 40;

                            ind = 140;
                            data = BitConverter.GetBytes(10.0f); // i_output
                            CopyData(ref outdata, ref data, ref ind);

                            // отправка сообщения
                            stream.Write(outdata, 0, outdata.Length);

                            Thread.Sleep(1000);
                            response += rnd.Next(-1000, 1000) / 1000f;
                        }

                        // закрываем подключение
                        client.Close();
                    }
                }
                catch(ThreadAbortException)
                {
                    syncContext.Post(AddText, "Передача данных завершена.");
                    client.Close();
                    return;
                }
                catch(Exception ex)
                {
                    syncContext.Post(AddText, ex.Message);
                    client.Close();
                    return;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!serverStarted)
            {
                try
                {
                    IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

                    foreach (IPAddress ip in host.AddressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork)
                        {
                            localAddr = ip;
                            break;
                        }
                    }

                    if (Int32.TryParse(textBoxPort.Text, out int port))
                        server = new TcpListener(localAddr, port);
                    else
                        syncContext.Post(AddText, "Wrong Port.");

                    // запуск слушателя
                    server.Start(2);

                    serverSender = new Thread(ServerWorking) { IsBackground = true };

                    button1.Text = "Stop Server";
                    serverStarted = true;

                    serverSender.Start();
                
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            else
            {
                Disconnection();
            }
        }

        void Disconnection()
        {
            try
            {
                serverSender?.Abort();
                server?.Stop();
                button1.Text = "Start Server";
                serverStarted = false;
            }
            catch(Exception ex)
            {
                return;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serverStarted)
                Disconnection();
        }
    }
}
