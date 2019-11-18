using ParkingParser2.Core;
using System;
using System.IO;

namespace ParkingParser2.UDP
{
    public static class UdpModel
    {
        private static int CountMeasureInt = 500; 
        
        private static byte Version;// 0x01;

        private static byte[] CountMeasure = new byte[4];//  0x00, 0x00, 0x01, 0xF4  //500

        private static byte[] Time = new byte[4];// 0x00, 0x00, 0x00, 0x38  //54 мс

        private static byte STX;// 0x01;

        private static byte[] DataMeasure = new byte[CountMeasureInt];

        private static byte ETX;// 0x02;

        private static byte[] FullUdpContent;
        
        public static void Handle(byte[] UdpBody)
        {
            for (int i = 0; i < UdpBody.Length; i++)
            {
                if (i == 0) Version = UdpBody[i];
                if (i == 1) for (int j = 0; j < 4; j++) CountMeasure[j] = UdpBody[i];
                if (i == 5) for (int j = 0; j < 4; j++) Time[j] = UdpBody[i];
                if (i == 6) STX = UdpBody[i];
                if (i <= 7) for (int j = 0; j < CountMeasureInt; j++) DataMeasure[j] = UdpBody[i];
                if (i == UdpBody.Length) ETX = UdpBody[i];               
            }

            FullUdpContent = UdpBody;

            if (IsValidate()) WriteToFile();
            else CleanModel();
        }

        private static bool IsValidate()
        {
            return true;
        }

        private static void CleanModel()
        {
            Version = 0x00;
            for (int j = 0; j < 4; j++) CountMeasure[j] = 0x00;
            for (int j = 0; j < 4; j++) Time[j] = 0x00;
            for (int j = 0; j < CountMeasureInt; j++) DataMeasure[j] = 0x00;
            STX = 0x00;
            ETX = 0x00;
        }

        private static void WriteToFile()
        {
            if (IsOutputFileExist())
            {
                using (FileStream fstream = new FileStream(IniSettings.GetValue("PathToUDPOutputFile"), FileMode.OpenOrCreate))
                {
                    fstream.Seek(0, SeekOrigin.End);

                    byte[] UdpContentTemp = new byte[FullUdpContent.Length * 2];

                    int j = 1;
                    int k = 0;

                    for(int i = 10; i < FullUdpContent.Length; i++)
                    {
                        UdpContentTemp[k] = FullUdpContent[i];
                        UdpContentTemp[j] = 0x3B;
                        k += 2;
                        j += 2;
                    }

                    fstream.Write(UdpContentTemp, 0, UdpContentTemp.Length);
                }
            }
            else CreateOutputFile();
        }

        private static bool IsOutputFileExist()
        {
            return File.Exists(IniSettings.GetValue("PathToUDPOutputFile"));
        }

        private static void CreateOutputFile()
        {
            try
            {
                using (FileStream FileStream = new FileStream(IniSettings.GetValue("PathToUDPOutputFile"), FileMode.OpenOrCreate)) { }

                Logger.Log("Создание файла конфигурации", RecordType.INFO, Source.UDP);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message, RecordType.ERROR, Source.UDP);
            }
        }
    }
}
