using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PacketDisplayer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Byte[] inBuffer = new Byte[256];
        Byte[] outBuffer = new Byte[256];

        IPAddress ipAddr = null;
        IPEndPoint endPoint = null;
        TcpClient tcpСlient;
        NetworkStream tcpStream;
        Thread th;
        IPAddress ipAddr2 = null;

        TcpListener server;
        Thread serverSender;

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
                    // получаем входящее подключение
                    using (client = server.AcceptTcpClient())
                    {
                        // получаем сетевой поток для чтения и записи
                        NetworkStream stream = client.GetStream();
                        
                        // сообщение для отправки клиенту 
                        while (client.Connected)
                        {
                            // преобразуем сообщение в массив байтов
                            byte[] outdata = new byte[256];
                            byte[] data = BitConverter.GetBytes(Convert.ToSingle(txtboxSendConc.Text));
                            int ind = 0;
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(100); // up
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(200); // down
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(300); // sum
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(400); // dumpsum
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes((ushort)10); // itrowel
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes((ushort)20); // irevers
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes((ushort)30); // dump
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes((ushort)40); // period_answer
                            CopyData(ref outdata, ref data, ref ind);

                            // params
                            { 
                                data = BitConverter.GetBytes(Convert.ToSingle(txtboxSendA.Text)); // a
                                CopyData(ref outdata, ref data, ref ind);

                                data = BitConverter.GetBytes(Convert.ToSingle(txtboxSendB.Text)); // b
                                CopyData(ref outdata, ref data, ref ind);

                                data = BitConverter.GetBytes(Convert.ToSingle(txtboxSendC.Text)); // c
                                CopyData(ref outdata, ref data, ref ind);

                                data = BitConverter.GetBytes(Convert.ToSingle(txtboxSendD.Text)); // d
                                CopyData(ref outdata, ref data, ref ind);

                                data = BitConverter.GetBytes(Convert.ToSingle(txtboxSendE.Text)); // e
                                CopyData(ref outdata, ref data, ref ind);

                                data = BitConverter.GetBytes(Convert.ToSingle(txtboxSendN.Text)); // n
                                CopyData(ref outdata, ref data, ref ind);

                                data = BitConverter.GetBytes(Convert.ToSingle(txtboxSendF.Text)); //f
                                CopyData(ref outdata, ref data, ref ind);

                                data = BitConverter.GetBytes(Convert.ToSingle(txtboxSendG.Text)); // g
                                CopyData(ref outdata, ref data, ref ind);

                                data = BitConverter.GetBytes(Convert.ToSingle(txtboxSendH.Text)); // h
                                CopyData(ref outdata, ref data, ref ind);
                            }

                            // net configs
                            {
                                data = IPAddress.Parse(txtboxSendIP.Text).GetAddressBytes(); // ip
                                CopyData(ref outdata, ref data, ref ind);

                                data = IPAddress.Parse(txtboxSendMask.Text).GetAddressBytes(); // mask
                                CopyData(ref outdata, ref data, ref ind);

                                data = IPAddress.Parse(txtboxSendGW.Text).GetAddressBytes(); // gw
                                CopyData(ref outdata, ref data, ref ind);

                                data = IPAddress.Parse(txtboxSendDNS1.Text).GetAddressBytes(); // pridns
                                CopyData(ref outdata, ref data, ref ind);

                                data = IPAddress.Parse(txtboxSendDNS2.Text).GetAddressBytes();  // secDns
                                CopyData(ref outdata, ref data, ref ind);

                                data = BitConverter.GetBytes(Convert.ToInt16(txtboxSendPortTCP.Text)); // portTCP
                                CopyData(ref outdata, ref data, ref ind);

                                data = BitConverter.GetBytes(Convert.ToInt16(txtboxSendPortUDP.Text)); // portUDP
                                CopyData(ref outdata, ref data, ref ind);

                                data = BitConverter.GetBytes(Convert.ToInt16(txtboxSendRemPortUDP.Text)); // portRemUDP
                                CopyData(ref outdata, ref data, ref ind);

                                data = IPAddress.Parse(txtboxSendRemIP.Text).GetAddressBytes(); // remIP
                                CopyData(ref outdata, ref data, ref ind);
                            }

                            data = BitConverter.GetBytes(4.0f); // i4
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(20.0f); // i20
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(10.0f); // iOut
                            CopyData(ref outdata, ref data, ref ind);

                            outBuffer[ind] = 255;
                            ind++;

                            data = BitConverter.GetBytes(11.0f); // cb
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(12.0f); // ci
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(13.0f); // p_term
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(14.0f); // i_term
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(15.0f); // d_term

                            outdata[ind] = 5;
                            ind++;
                            outdata[ind] = 6;
                            ind++;
                            outdata[ind] = 7;
                            ind++;
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(10000f); // dump_i
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(20000f); // out
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(30000f); // i_out
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(40000f); // referens
                            CopyData(ref outdata, ref data, ref ind);

                            data = BitConverter.GetBytes(Convert.ToSingle(txtboxSendOutputI)); // i_output
                            CopyData(ref outdata, ref data, ref ind);

                            // отправка сообщения
                            stream.Write(outdata, 0, outdata.Length);

                            Thread.Sleep(1000);
                        }

                        // закрываем подключение
                        client.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    serverSender?.Abort();
                    server?.Stop();
                    client.Close();
                }

            }
        }

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                IPAddress localAddr = null;
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localAddr = ip;
                        break;
                    }
                }

                server = new TcpListener(localAddr, 4000);
                
                // запуск слушателя
                server.Start(2);

                serverSender = new Thread(ServerWorking);

                serverSender.Start();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
