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

namespace ConcentratePanelClient
{
    static class ByteCopier
    {
        public static byte[] Buffer = new byte[256];

        public static void Copy(this byte[] dest, byte[] source, int indexStart)
        {
            for (int i = 0; i < source.Length; i++, indexStart++)
            {
                dest[indexStart] = source[i];
            }
        }
    }

    class Packet
    {
        public float C;
        public uint up, down, sum, dumpSum;
        public ushort i_trowel, i_revers, dump, period_answer;
        public float[] parameters = new float[9]; // a, b, c, d, e, n, f, g, h
        public IPAddress ipDat, maskDat, gwDat, dns1Dat, dns2Dat;
        public ushort portTCP, portUDP, remPortUDP;
        public IPAddress remIpUDP;
        public float min_I, max_I; // непонятно насчет i_out
        public byte isUDPEnable;
        public float Cb, Ci;
        public float p_term = 0, i_term = 0, d_term = 0;
        public byte P_factor, I_factor, D_factor;
        public ushort dump_i, Out, i_out;
        public float referens, output_I, damper;
        public IPAddress ipPan, maskPan, gwPan, dns1Pan, dns2Pan;
        public IPAddress remIpPan;
        public bool isPanelConnected;

        public void ParseReceivedBuffer(Byte[] buffer)
        {
            C = BitConverter.ToSingle(buffer, 0);
            up = BitConverter.ToUInt32(buffer, 4);
            down = BitConverter.ToUInt32(buffer, 8);
            sum = BitConverter.ToUInt32(buffer, 12);
            dumpSum = BitConverter.ToUInt32(buffer, 16);

            i_trowel = BitConverter.ToUInt16(buffer, 20);
            i_revers = BitConverter.ToUInt16(buffer, 22);
            dump = BitConverter.ToUInt16(buffer, 24);
            period_answer = BitConverter.ToUInt16(buffer, 26);

            for (int i = 0, index = 28; i < parameters.Length; i++, index += 4)
            {
                parameters[i] = BitConverter.ToSingle(buffer, index);
            }
            
            ipDat = new IPAddress(new byte[] { buffer[64], buffer[65], buffer[66], buffer[67] });
            maskDat = new IPAddress(new byte[] { buffer[68], buffer[69], buffer[70], buffer[71] });
            gwDat = new IPAddress(new byte[] { buffer[72], buffer[73], buffer[74], buffer[75] });
            dns1Dat = new IPAddress(new byte[] { buffer[76], buffer[77], buffer[78], buffer[79] });
            dns2Dat = new IPAddress(new byte[] { buffer[80], buffer[81], buffer[82], buffer[83] });

            portTCP = BitConverter.ToUInt16(buffer, 84);
            portUDP = BitConverter.ToUInt16(buffer, 86);
            remPortUDP = BitConverter.ToUInt16(buffer, 88);
            remIpUDP = new IPAddress(new byte[] { buffer[90], buffer[91], buffer[92], buffer[93] });

            min_I = BitConverter.ToSingle(buffer, 94);
            max_I = BitConverter.ToSingle(buffer, 98);
            isUDPEnable = buffer[106];

            Cb = BitConverter.ToSingle(buffer, 107);
            Ci = BitConverter.ToSingle(buffer, 111);
            p_term = BitConverter.ToSingle(buffer, 115);
            i_term = BitConverter.ToSingle(buffer, 119);
            d_term = BitConverter.ToSingle(buffer, 123);

            P_factor = buffer[127];
            I_factor = buffer[128];
            D_factor = buffer[129];

            dump_i = BitConverter.ToUInt16(buffer, 130);
            Out = BitConverter.ToUInt16(buffer, 132);
            i_out = BitConverter.ToUInt16(buffer, 134);

            referens = BitConverter.ToSingle(buffer, 136);
            output_I = BitConverter.ToSingle(buffer, 140);
            damper = BitConverter.ToSingle(buffer, 146);

            ipPan = new IPAddress(new byte[] { buffer[150], buffer[151], buffer[152], buffer[153] });
            maskPan = new IPAddress(new byte[] { buffer[154], buffer[155], buffer[156], buffer[157] });
            gwPan = new IPAddress(new byte[] { buffer[158], buffer[159], buffer[160], buffer[161] });
            dns1Pan = new IPAddress(new byte[] { buffer[162], buffer[163], buffer[164], buffer[165] });
            dns2Pan = new IPAddress(new byte[] { buffer[166], buffer[167], buffer[168], buffer[169] });
            remIpPan = new IPAddress(new byte[] { buffer[170], buffer[171], buffer[172], buffer[173] });
            isPanelConnected = buffer[174] > 0;
        }

        public Packet MakeCopy()
        {
            return MemberwiseClone() as Packet;
        }

        public Byte[] SendBuffer()
        {
            Byte[] temp;
            temp = BitConverter.GetBytes(C);
            ByteCopier.Buffer.Copy(temp, 0);

            temp = BitConverter.GetBytes(up);
            ByteCopier.Buffer.Copy(temp, 4);

            temp = BitConverter.GetBytes(down);
            ByteCopier.Buffer.Copy(temp, 8);

            temp = BitConverter.GetBytes(sum);
            ByteCopier.Buffer.Copy(temp, 12);

            temp = BitConverter.GetBytes(dumpSum);
            ByteCopier.Buffer.Copy(temp, 16);

            temp = BitConverter.GetBytes(i_trowel);
            ByteCopier.Buffer.Copy(temp, 20);

            temp = BitConverter.GetBytes(i_revers);
            ByteCopier.Buffer.Copy(temp, 22);

            temp = BitConverter.GetBytes(dump);
            ByteCopier.Buffer.Copy(temp, 24);

            temp = BitConverter.GetBytes(period_answer);
            ByteCopier.Buffer.Copy(temp, 26);


            for (int i = 0, index = 28; i < parameters.Length; i++, index += 4)
            {
                temp = BitConverter.GetBytes(parameters[i]);
                ByteCopier.Buffer.Copy(temp, index);
            }


            temp = ipDat.GetAddressBytes();
            ByteCopier.Buffer.Copy(temp, 64);

            temp = maskDat.GetAddressBytes();
            ByteCopier.Buffer.Copy(temp, 68);

            temp = gwDat.GetAddressBytes();
            ByteCopier.Buffer.Copy(temp, 72);

            temp = dns1Dat.GetAddressBytes();
            ByteCopier.Buffer.Copy(temp, 76);

            temp = dns2Dat.GetAddressBytes();
            ByteCopier.Buffer.Copy(temp, 80);

            temp = BitConverter.GetBytes(portTCP);
            ByteCopier.Buffer.Copy(temp, 84);

            temp = BitConverter.GetBytes(portUDP);
            ByteCopier.Buffer.Copy(temp, 86);

            temp = BitConverter.GetBytes(remPortUDP);
            ByteCopier.Buffer.Copy(temp, 88);

            temp = remIpUDP.GetAddressBytes();
            ByteCopier.Buffer.Copy(temp, 90);


            temp = BitConverter.GetBytes(min_I);
            ByteCopier.Buffer.Copy(temp, 94);

            temp = BitConverter.GetBytes(max_I);
            ByteCopier.Buffer.Copy(temp, 98);

            ByteCopier.Buffer.Copy(new byte[] { isUDPEnable }, 106);

            temp = BitConverter.GetBytes(Cb);
            ByteCopier.Buffer.Copy(temp, 107);

            temp = BitConverter.GetBytes(Ci);
            ByteCopier.Buffer.Copy(temp, 111);

            temp = BitConverter.GetBytes(p_term);
            ByteCopier.Buffer.Copy(temp, 115);

            temp = BitConverter.GetBytes(i_term);
            ByteCopier.Buffer.Copy(temp, 119);

            temp = BitConverter.GetBytes(d_term);
            ByteCopier.Buffer.Copy(temp, 123);

            ByteCopier.Buffer.Copy(new byte[] { P_factor }, 127);
            ByteCopier.Buffer.Copy(new byte[] { I_factor }, 128);
            ByteCopier.Buffer.Copy(new byte[] { D_factor }, 129);

            temp = BitConverter.GetBytes(dump_i);
            ByteCopier.Buffer.Copy(temp, 130);

            temp = BitConverter.GetBytes(Out);
            ByteCopier.Buffer.Copy(temp, 132);

            temp = BitConverter.GetBytes(i_out);
            ByteCopier.Buffer.Copy(temp, 134);

            temp = BitConverter.GetBytes(referens);
            ByteCopier.Buffer.Copy(temp, 136);

            temp = BitConverter.GetBytes(output_I);
            ByteCopier.Buffer.Copy(temp, 140);


            temp = ipPan.GetAddressBytes();
            ByteCopier.Buffer.Copy(temp, 150);

            temp = maskPan.GetAddressBytes();
            ByteCopier.Buffer.Copy(temp, 154);

            temp = gwPan.GetAddressBytes();
            ByteCopier.Buffer.Copy(temp, 158);

            temp = dns1Pan.GetAddressBytes();
            ByteCopier.Buffer.Copy(temp, 162);

            temp = dns2Pan.GetAddressBytes();
            ByteCopier.Buffer.Copy(temp, 166);

            temp = remIpPan.GetAddressBytes();
            ByteCopier.Buffer.Copy(temp, 170);

            ByteCopier.Buffer.Copy(new byte[] { (byte)(isPanelConnected ? 255 : 0) }, 174);

            return ByteCopier.Buffer;
        }
    }

    public partial class MainWindow : Window
    {
        Packet inpack, outpack;

        readonly SynchronizationContext syncContext;
        IPAddress connIP = null;
        IPEndPoint panelConn = null;
        TcpClient tcpClient;
        NetworkStream tcpStream;
        Thread th;

        bool isConnected = false;

        public MainWindow()
        {
            InitializeComponent();
            
            inpack = new Packet();

            this.Closing += MainWindow_Closing;

            syncContext = SynchronizationContext.Current;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isConnected)
                Disconnect();
        }

        private void ButtonConnect_Click(object sender, RoutedEventArgs e) // присоединиться к панели
        {
            if (!isConnected)
            {
                connIP = IPAddress.Parse(textBoxConnIP.Text);
                panelConn = new IPEndPoint(connIP, Int32.Parse(textBoxConnPort.Text));
                tcpClient = new TcpClient();

                try
                {
                    tcpClient.Connect(panelConn);
                    tcpStream = tcpClient.GetStream();
                    th = new Thread(ReceiveAndDisplayData) { IsBackground = false };
                    th.Start();
                    MessageBox.Show("Соединение выполнено успешно!");
                    buttonConnect.Content = "Отсоединиться";
                    isConnected = true;
                }
                catch (SocketException ex)
                {
                    MessageBox.Show("Соединиться не удалось.Ошибка: " + ex.ToString());
                }
            }
            else
            {
                Disconnect();
                MessageBox.Show("Соединение закрыто.");
            }
        }

        private void buttonSend_Click(object sender, RoutedEventArgs e) // отправить
        {
            // копия создается во время нажатия кнопки Копировать
            if (tcpStream == null)
            {
                MessageBox.Show("Сначала подключитесь к панели.");
                return;
            }
            else if (tcpStream.CanWrite)
            {
                if (outpack == null)
                {
                    MessageBox.Show("Сначала скопируйте данные, а потом изменяйте их.");
                    return;
                }
                if (MakePacketForSending())
                {
                    var outBuffer = outpack.SendBuffer();
                    tcpStream.Write(outBuffer, 0, outBuffer.Length);
                    MessageBox.Show("Пакет отправлен.");
                }
                else
                {
                    MessageBox.Show("Пакет не может быть отправлен.");
                }
            }
        }

        private void buttonCopy_Click(object sender, RoutedEventArgs e)
        {
            // sensor network configs
            textBoxDatIP.Text = labelDatIP.Content.ToString();
            textBoxDatMask.Text = labelDatMask.Content.ToString();
            textBoxDatGW.Text = labelDatGW.Content.ToString();
            textBoxDatDNS1.Text = labelDatDNS1.Content.ToString();
            textBoxDatDNS2.Text = labelDatDNS2.Content.ToString();

            // parametres
            textBoxParameterA.Text = labelParameterA.Content.ToString();
            textBoxParameterB.Text = labelParameterB.Content.ToString();
            textBoxParameterC.Text = labelParameterC.Content.ToString();
            textBoxParameterD.Text = labelParameterD.Content.ToString();
            textBoxParameterE.Text = labelParameterE.Content.ToString();
            textBoxParameterF.Text = labelParameterF.Content.ToString();
            textBoxParameterG.Text = labelParameterG.Content.ToString();
            textBoxParameterH.Text = labelParameterH.Content.ToString();
            textBoxParameterN.Text = labelParameterN.Content.ToString();

            // changable data
            textBoxI_Trowel.Text = labelI_Trowel.Content.ToString();
            textBoxI_Revers.Text = labelI_Revers.Content.ToString();
            textBoxDump.Text = labelDump.Content.ToString();
            textBoxPeriod_Answer.Text = labelPeriod_Answer.Content.ToString();
            textBoxPidP.Text = labelPidP.Content.ToString();
            textBoxPidI.Text = labelPidI.Content.ToString();
            textBoxPidD.Text = labelPidD.Content.ToString();
            textBoxDump_i.Text = labelDump_i.Content.ToString();
            textBoxReferens.Text = labelReferens.Content.ToString();

            outpack = inpack.MakeCopy();

            // panel network configs
            textBoxPanIP.Text = outpack.ipPan.ToString();
            textBoxPanMask.Text = outpack.maskPan.ToString();
            textBoxPanGW.Text = outpack.gwPan.ToString();
            textBoxPanDNS1.Text = outpack.dns1Pan.ToString();
            textBoxPanDNS2.Text = outpack.dns2Pan.ToString();
            textBoxPanRemIP.Text = outpack.remIpPan.ToString();
        }

        void ReceiveAndDisplayData()
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
                            inpack.ParseReceivedBuffer(bytes);
                            Dispatcher.Invoke(new Action<Packet>(Display), inpack.MakeCopy());
                        }
                    }
                    else
                    {
                        Disconnect();
                        return;
                    }
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void Disconnect()
        {
            th.Abort();
            tcpClient.Close();
            tcpStream.Close();
            buttonConnect.Content = "Подключиться";
            isConnected = false;
        }

        void Display(Packet pack)
        {
            // tabitem 1
            labelDatIP.Content = pack.ipDat;
            labelDatMask.Content = pack.maskDat;
            labelDatGW.Content = pack.gwDat;
            labelDatDNS1.Content = pack.dns1Dat;
            labelDatDNS2.Content = pack.dns2Dat;
            labelPortTCP.Content = pack.portTCP;
            labelPortUDP.Content = pack.portUDP;
            labelRemPortUDP.Content = pack.remPortUDP;
            labelRemIPUDP.Content = pack.remIpUDP.ToString();
            labelIsUDP_Enable.Content = pack.isUDPEnable > 0 ? "UDP включен" : "UDP не включен";

            // tabitem 2
            labelParameterA.Content = pack.parameters[0].ToString();
            labelParameterB.Content = pack.parameters[1].ToString();
            labelParameterC.Content = pack.parameters[2].ToString();
            labelParameterD.Content = pack.parameters[3].ToString();
            labelParameterE.Content = pack.parameters[4].ToString();
            labelParameterN.Content = pack.parameters[5].ToString();
            labelParameterF.Content = pack.parameters[6].ToString();
            labelParameterG.Content = pack.parameters[7].ToString();
            labelParameterH.Content = pack.parameters[8].ToString();

            // tabitem 3
            labelI_Trowel.Content = pack.i_trowel;
            labelI_Revers.Content = pack.i_revers;
            labelDump.Content = pack.dump;
            labelPeriod_Answer.Content = pack.period_answer;
            labelPidP.Content = pack.P_factor;
            labelPidI.Content = pack.I_factor;
            labelPidD.Content = pack.D_factor;
            labelDump_i.Content = pack.dump_i;
            labelReferens.Content = pack.referens;

            // tabitem 4
            labelC.Content = pack.C;
            labelCb.Content = pack.Cb;
            labelCi.Content = pack.Ci;
            labelUp.Content = pack.up;
            labelDown.Content = pack.down;
            labelSum.Content = pack.sum;
            labelDumpSum.Content = pack.dumpSum;
            labelMinI.Content = pack.min_I;
            labelMaxI.Content = pack.max_I;

            // tabitem 5
            labelP_term.Content = pack.p_term;
            labelI_term.Content = pack.i_term;
            labelD_term.Content = pack.d_term;
            labelOut.Content = pack.Out;
            labelI_Out.Content = pack.i_out;
            labelOutput_I.Content = pack.output_I;
        }

        // берет все заполненые значения и пишет их в outpack
        bool MakePacketForSending()
        {
            try
            {
                IPAddress tempAddr = null;
                float tempFloat = 0;
                ushort tempUshort = 0;

                // сетевые настройки датчика
                {
                    if (textBoxDatIP.Text.Length != 0)
                    {
                        tempAddr = IPAddress.Parse(textBoxDatIP.Text);
                        if (tempAddr != null)
                            outpack.ipDat = tempAddr;
                    }

                    if (textBoxDatMask.Text.Length != 0)
                    {
                        tempAddr = IPAddress.Parse(textBoxDatMask.Text);
                        if (tempAddr != null)
                            outpack.maskDat = tempAddr;
                    }

                    if (textBoxDatGW.Text.Length != 0)
                    {
                        tempAddr = IPAddress.Parse(textBoxDatGW.Text);
                        if (tempAddr != null)
                            outpack.gwDat = tempAddr;
                    }

                    if (textBoxDatDNS1.Text.Length != 0)
                    {
                        tempAddr = IPAddress.Parse(textBoxDatDNS1.Text);
                        if (tempAddr != null)
                            outpack.dns1Dat = tempAddr;
                    }

                    if (textBoxDatDNS2.Text.Length != 0)
                    {
                        tempAddr = IPAddress.Parse(textBoxDatDNS2.Text);
                        if (tempAddr != null)
                            outpack.dns2Dat = tempAddr;
                    }
                }

                // params
                {
                    if (textBoxParameterA.Text.Length != 0)
                    {
                        tempFloat = float.Parse(textBoxParameterA.Text);
                        if (!float.IsNaN(tempFloat))
                            outpack.parameters[0] = tempFloat;
                    }

                    if (textBoxParameterB.Text.Length != 0)
                    {
                        tempFloat = float.Parse(textBoxParameterB.Text);
                        if (!float.IsNaN(tempFloat))
                            outpack.parameters[1] = tempFloat;
                    }

                    if (textBoxParameterC.Text.Length != 0)
                    {
                        tempFloat = float.Parse(textBoxParameterC.Text);
                        if (!float.IsNaN(tempFloat))
                            outpack.parameters[2] = tempFloat;
                    }

                    if (textBoxParameterD.Text.Length != 0)
                    {
                        tempFloat = float.Parse(textBoxParameterD.Text);
                        if (!float.IsNaN(tempFloat))
                            outpack.parameters[3] = tempFloat;
                    }

                    if (textBoxParameterE.Text.Length != 0)
                    {
                        tempFloat = float.Parse(textBoxParameterE.Text);
                        if (!float.IsNaN(tempFloat))
                            outpack.parameters[4] = tempFloat;
                    }

                    if (textBoxParameterN.Text.Length != 0)
                    {
                        tempFloat = float.Parse(textBoxParameterN.Text);
                        if (!float.IsNaN(tempFloat))
                            outpack.parameters[5] = tempFloat;
                    }

                    if (textBoxParameterF.Text.Length != 0)
                    {
                        tempFloat = float.Parse(textBoxParameterF.Text);
                        if (!float.IsNaN(tempFloat))
                            outpack.parameters[6] = tempFloat;
                    }

                    if (textBoxParameterG.Text.Length != 0)
                    {
                        tempFloat = float.Parse(textBoxParameterG.Text);
                        if (!float.IsNaN(tempFloat))
                            outpack.parameters[7] = tempFloat;
                    }

                    if (textBoxParameterH.Text.Length != 0)
                    {
                        tempFloat = float.Parse(textBoxParameterH.Text);
                        if (!float.IsNaN(tempFloat))
                            outpack.parameters[8] = tempFloat;
                    }
                }

                // settings
                {
                    if (textBoxI_Trowel.Text.Length != 0)
                    {
                        if (!ushort.TryParse(textBoxI_Trowel.Text, out tempUshort))
                            throw new Exception("Неверно введен ток хода.");
                        else
                            outpack.i_trowel = tempUshort;

                    }

                    if (textBoxI_Revers.Text.Length != 0)
                    {
                        if (!ushort.TryParse(textBoxI_Revers.Text, out tempUshort))
                            throw new Exception("Неверно введен ток реверса.");
                        else
                            outpack.i_revers = tempUshort;
                    }

                    if (textBoxDump.Text.Length != 0)
                    {
                        if (!ushort.TryParse(textBoxDump.Text, out tempUshort))
                            throw new Exception("Неверно введен dump.");
                        else
                            outpack.dump = tempUshort;
                    }

                    if (textBoxPeriod_Answer.Text.Length != 0)
                    {
                        if (!ushort.TryParse(textBoxPeriod_Answer.Text, out tempUshort))
                            throw new Exception("Неверно введен dump.");
                        else
                            outpack.period_answer = tempUshort;
                    }

                    byte tempByte = 0;

                    if (textBoxPidP.Text.Length != 0)
                    {
                        if (!byte.TryParse(textBoxPidP.Text, out tempByte))
                            throw new Exception("Неверно введен PID P.");
                        else
                            outpack.P_factor = tempByte;
                    }

                    if (textBoxPidI.Text.Length != 0)
                    {
                        if (!byte.TryParse(textBoxPidI.Text, out tempByte))
                            throw new Exception("Неверно введен PID I.");
                        else
                            outpack.I_factor = tempByte;
                    }

                    if (textBoxPidD.Text.Length != 0)
                    {
                        if (!byte.TryParse(textBoxPidD.Text, out tempByte))
                            throw new Exception("Неверно введен PID D.");
                        else
                            outpack.D_factor = tempByte;
                    }

                    if (textBoxDump_i.Text.Length != 0)
                    {
                        if (!ushort.TryParse(textBoxDump_i.Text, out tempUshort))
                            throw new Exception("Неверно введен dump_i.");
                        else
                            outpack.dump_i = tempUshort;
                    }

                    if (textBoxReferens.Text.Length != 0)
                    {
                        tempFloat = float.Parse(textBoxReferens.Text);
                        if (!float.IsNaN(tempFloat))
                            outpack.referens = tempFloat;
                    }
                }

                // сетевые настройки панели
                {
                    if (textBoxPanIP.Text.Length != 0)
                    {
                        tempAddr = IPAddress.Parse(textBoxPanIP.Text);
                        if (tempAddr != null)
                            outpack.ipPan = tempAddr;
                    }

                    if (textBoxPanMask.Text.Length != 0)
                    {
                        tempAddr = IPAddress.Parse(textBoxPanMask.Text);
                        if (tempAddr != null)
                            outpack.maskPan = tempAddr;
                    }

                    if (textBoxPanGW.Text.Length != 0)
                    {
                        tempAddr = IPAddress.Parse(textBoxPanGW.Text);
                        if (tempAddr != null)
                            outpack.gwPan = tempAddr;
                    }

                    if (textBoxPanDNS1.Text.Length != 0)
                    {
                        tempAddr = IPAddress.Parse(textBoxPanDNS1.Text);
                        if (tempAddr != null)
                            outpack.dns1Pan = tempAddr;
                    }

                    if (textBoxPanDNS2.Text.Length != 0)
                    {
                        tempAddr = IPAddress.Parse(textBoxPanDNS2.Text);
                        if (tempAddr != null)
                            outpack.dns2Pan = tempAddr;
                    }

                    if (textBoxPanRemIP.Text.Length != 0)
                    {
                        tempAddr = IPAddress.Parse(textBoxPanRemIP.Text);
                        if (tempAddr != null)
                            outpack.remIpPan = tempAddr;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
                return false;
            }
        }
        
    }
}
