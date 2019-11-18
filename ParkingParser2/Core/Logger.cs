using System;
using System.IO;
using System.Text;
using System.Windows;

namespace ParkingParser2.Core
{
    public class Logger
    {
        public delegate void LoggerHandler(object sender, LoggerEventArgs e);

        public static event LoggerHandler OnLog;

        public Logger() { }

        public static void Log(String Message, RecordType RecordType, Source Source)
        {
            if (IsLogExist())
            {
                DateTime CurrentTime = DateTime.Now;
                string Record = "";

                //Запись в файл через поток       
                using (FileStream FileStream = new FileStream(IniSettings.GetValue("PathToLogFile"), FileMode.Open))
                {
                    FileStream.Seek(0, SeekOrigin.End);

                    //Добавление к записи дату
                    Record += CurrentTime.ToString("dd.MM.yyyy hh:mm:ss") + " |";

                    //Формирование сообщения в зависимости от источника
                    switch (Source)
                    {
                        case Source.Parser:
                            {
                                Record += " Parser |";
                                break;
                            }
                        case Source.UDP:
                            {
                                Record += " UDP |";
                                break;
                            }
                        case Source.System:
                            {
                                Record += " System |";
                                break;
                            }
                    }

                    //Формирование сообщения в зависимости от типа
                    switch (RecordType)
                    {
                        case RecordType.INFO:
                            {
                                Record += " INFO |";
                                break;
                            }
                        case RecordType.WARNING:
                            {
                                Record += " WARNING |";
                                break;
                            }
                        case RecordType.ERROR:
                            {
                                Record += " ERROR |";
                                break;
                            }
                    }

                    //Добавление сообщения
                    Record += " " + Message + "\n";
                    byte[] array = Encoding.Default.GetBytes(Record);

                    //Запись массива байтов в файл
                    FileStream.Write(array, 0, array.Length);
                }

                OnLog?.Invoke(new Logger(), new LoggerEventArgs(Record, Source));
            }
            else CreateFile();
        }

        private static bool IsLogExist()
        {
            return File.Exists(IniSettings.GetValue("PathToLogFile"));
        }

        private static void CreateFile()
        {
            try
            {
                using (FileStream FileStream = new FileStream(IniSettings.GetValue("PathToLogFile"), FileMode.OpenOrCreate)) { }

                Log("Создание файла конфигурации", RecordType.INFO, Source.System);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }       
    }
    public class LoggerEventArgs
    {
        public string Record { get; set; }

        public Source Source { get; set; }

        public LoggerEventArgs(string Record, Source Source)
        {
            this.Record = Record;
            this.Source = Source;
        }
    }
}
