using ParkingParser2.Core;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ParkingParser2.UDP
{
    class UdpClient
    {
        private string Ip;

        private int Port;

        private Socket Socket;

        private byte[] Buffer = new byte[1024];

        public UdpClient(string Ip, int Port)
        {
            try
            {
                this.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                IPEndPoint IPEndPoint = new IPEndPoint(IPAddress.Parse(Ip), Port);

                this.Socket.Bind(IPEndPoint);

                Logger.Log("Сокет успешно создан", RecordType.INFO, Source.UDP);
            }
            catch(Exception ex)
            {
                Logger.Log(ex.Message, RecordType.ERROR, Source.UDP);
            }
        }

        public void Start(CancellationToken Token, bool Demo)
        {
            Logger.Log("Прослушивание сокета запущено", RecordType.INFO, Source.UDP);

            if (!Demo) ReceiveRealData(Token);
            else ReceiveDemoData(Token);                               
        }

        private void Stop()
        {
            try
            {
                if (this.Socket != null)
                {
                    this.Socket.Shutdown(SocketShutdown.Both);
                    this.Socket.Close();
                }

                Logger.Log("Прослушивание сокета завершено принудительно системой", RecordType.WARNING, Source.UDP);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message, RecordType.ERROR, Source.UDP);
            }
        }

        private void ReceiveRealData(CancellationToken Token)
        {
            try
            {
                while (true)
                {
                    if (Token.IsCancellationRequested) return;

                    //Кол-во принимаемых байтов
                    int BytesCount = 0;

                    //Получение Ip адреса с которого пришли данные
                    EndPoint RemoteIp = new IPEndPoint(IPAddress.Any, 0);

                    //Принятие данных с udp пакетов
                    do
                    {
                        BytesCount = this.Socket.ReceiveFrom(this.Buffer, ref RemoteIp);
                    }
                    while (this.Socket.Available > 0);

                    IPEndPoint RemoteFullIp = RemoteIp as IPEndPoint;

                    UdpModel.Handle(this.Buffer);

                    Logger.Log(RemoteFullIp.Address.ToString() + ":" + RemoteFullIp.Port.ToString() + "| Данные получены и обработаны", RecordType.INFO, Source.UDP);

                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message, RecordType.ERROR, Source.UDP);
            }
            finally
            {
                this.Stop();
                Logger.Log("Произошла принудительная остановка сокета", RecordType.WARNING, Source.UDP);
            }
        }

        private void ReceiveDemoData(CancellationToken Token)
        {
            while (true)
            {
                if (Token.IsCancellationRequested) return;

                UdpModel.Handle(GenerateDemoData());
                Logger.Log("Демо данные были обработаны", RecordType.INFO, Source.UDP);
                Thread.Sleep(1000);
            }     
        }

        private byte[] GenerateDemoData()
        {
            byte[] UdpContent = new byte[511];

            UdpContent[0] = 0x01;
            UdpContent[1] = 0x00;
            UdpContent[2] = 0x00;
            UdpContent[3] = 0x01;
            UdpContent[4] = 0xF4;
            UdpContent[5] = 0x00;
            UdpContent[6] = 0x00;
            UdpContent[7] = 0x00;
            UdpContent[8] = 0x38;
            UdpContent[9] = 0x01;
            for (int i = 10; i < 510; i++) UdpContent[i] = 0x35;
            UdpContent[510] = 0x02;

            return UdpContent;
        }      
    }
}
