using IniParser;
using IniParser.Model;
using ParkingParser2.Core;
using ParkingParser2.UDP;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ParkingParser2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileIniDataParser IniParser { get; set; } = new FileIniDataParser();

        private IniData IniData { get; set; } = new IniData();

        private const string DEFAULT_SETTINGS_PATH = @"C:\ProgramData\ParkingParser\Settings.ini";

        private const string DEFAULT_DIRECTORY_PATH = @"C:\ProgramData\ParkingParser";

        #region Токен на остановку работы 

        private CancellationTokenSource CancelTokenSourceParser;

        private CancellationToken TokenParser;

        private CancellationTokenSource CancelTokenSourceUdp;

        private CancellationToken TokenUdp;

        #endregion

        public MainWindow()
        {
            InitializeComponent();

            ApplicationInitialization();
        }

        private void ApplicationInitialization()
        {
            Logger.OnLog += LogSystem_OnLog;
            
            //Если приложение запущено в первый раз
            if (!Directory.Exists(DEFAULT_DIRECTORY_PATH)) FirstAppLaunch();
            else NotFirstAppLaunch();
        }      

        private void FirstAppLaunch()
        {
            try
            {
                Directory.CreateDirectory(DEFAULT_DIRECTORY_PATH);
                IniSettingsInitializationForFirstLaunch();
                //Создание файла конфигурации
                using (FileStream fstream = new FileStream(IniSettings.GetValue("PathToIniFile"), FileMode.OpenOrCreate))
                {
                    string contains = "[Main]\n" +
                        "UrlController = " + IniSettings.GetValue("UrlController") + "\n" +
                        "UrlProcedure = " + IniSettings.GetValue("UrlProcedure") + "\n" +
                        "ListenAddress = " + IniSettings.GetValue("ListenAddress") + "\n" +
                        "ListenPort = " + IniSettings.GetValue("ListenPort") + "\n" +
                        "PathToIniFile = " + IniSettings.GetValue("PathToIniFile") + "\n" +
                        "PathToLogFile = " + IniSettings.GetValue("PathToLogFile") + "\n" +
                        "PathToUDPOutputFile = " + IniSettings.GetValue("PathToUDPOutputFile");
                    byte[] array = Encoding.Default.GetBytes(contains);                   
                    fstream.Write(array, 0, array.Length);
                }

                FillSettingsTextBoxes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void NotFirstAppLaunch()
        {
            try
            {
                //Извлечение данных из файла конфигурации
                IniData = IniParser.ReadFile(DEFAULT_SETTINGS_PATH);
                IniSettingsInitializationNotForFirstLaunch();
                FillSettingsTextBoxes();

                //!!!!!Присваение пути до журнала сущности Logger
                //Logger.SetPath(Settings.LogPath)
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillSettingsTextBoxes()
        {
            UrlControllerTbx.Text = IniSettings.GetValue("UrlController");
            UrlProcedureTbx.Text = IniSettings.GetValue("UrlProcedure");
            ListenningAddressTbx.Text = IniSettings.GetValue("ListenAddress");
            ListenningPortTbx.Text = IniSettings.GetValue("ListenPort");
            PathToLogFileTbx.Text = IniSettings.GetValue("PathToLogFile");
            PathToUDPOutputFileTbx.Text = IniSettings.GetValue("PathToUDPOutputFile");
        }

        private void IniSettingsInitializationForFirstLaunch()
        {
            IniSettings.Add("UrlController", "http://172.19.1.34");
            IniSettings.Add("UrlProcedure", "http://u0828948.isp.regruhosting.ru/remote/add-value");
            IniSettings.Add("ListenAddress", "192.168.1.106");
            IniSettings.Add("ListenPort", "8888");
            IniSettings.Add("PathToIniFile", DEFAULT_SETTINGS_PATH);
            IniSettings.Add("PathToLogFile", DEFAULT_DIRECTORY_PATH + @"\log.txt");
            IniSettings.Add("PathToUDPOutputFile", DEFAULT_DIRECTORY_PATH + @"\udpoutput.csv");
        }

        private void IniSettingsInitializationNotForFirstLaunch()
        {
            if (IniData["Main"]["PathToLogFile"] != "null") IniSettings.Add("UrlController", IniData["Main"]["UrlController"]);
            if (IniData["Main"]["UrlProcedure"] != "null") IniSettings.Add("UrlProcedure", IniData["Main"]["UrlProcedure"]);
            if (IniData["Main"]["ListenAddress"] != "null") IniSettings.Add("ListenAddress", IniData["Main"]["ListenAddress"]);
            if (IniData["Main"]["ListenPort"] != "null") IniSettings.Add("ListenPort", IniData["Main"]["ListenPort"]);
            if (IniData["Main"]["PathToIniFile"] != "null") IniSettings.Add("PathToIniFile", IniData["Main"]["PathToIniFile"]);
            if (IniData["Main"]["PathToLogFile"] != "null") IniSettings.Add("PathToLogFile", IniData["Main"]["PathToLogFile"]);
            if (IniData["Main"]["PathToUDPOutputFile"] != "null") IniSettings.Add("PathToUDPOutputFile", IniData["Main"]["PathToUDPOutputFile"]);
        }

        #region События

        /// <summary>
        /// Изменение настроек парсера
        /// </summary>
        private void ApplyParserConfBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IniSettings.Edit("UrlController", UrlControllerTbx.Text);
                IniSettings.Edit("UrlProcedure", UrlProcedureTbx.Text);
                IniData["Main"]["UrlController"] = IniSettings.GetValue("UrlController");
                IniData["Main"]["UrlProcedure"] = IniSettings.GetValue("UrlProcedure");
                IniParser.WriteFile(DEFAULT_SETTINGS_PATH, IniData);
            }
            catch(Exception ex)
            {
                Logger.Log(ex.Message, RecordType.ERROR, Source.Parser);
            }           
        }

        /// <summary>
        /// Изменение настроек UDP прослушивателя
        /// </summary>
        private void ApplyUDPConfBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IniSettings.Edit("ListenAddress", ListenningAddressTbx.Text);
                IniSettings.Edit("ListenPort", ListenningPortTbx.Text);
                IniSettings.Edit("PathToUDPOutputFile", PathToUDPOutputFileTbx.Text);
                IniData["Main"]["ListenAddress"] = IniSettings.GetValue("ListenAddress");
                IniData["Main"]["ListenPort"] = IniSettings.GetValue("ListenPort");
                IniData["Main"]["PathToUDPOutputFile"] = IniSettings.GetValue("PathToUDPOutputFile");
                IniParser.WriteFile(DEFAULT_SETTINGS_PATH, IniData);
            }   
            catch(Exception ex)
            {
                Logger.Log(ex.Message, RecordType.ERROR, Source.UDP);
            }
        }

        /// <summary>
        /// Изменение общих настроек программы
        /// </summary>
        private void ApplyConfBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IniSettings.Edit("PathToLogFile", PathToLogFileTbx.Text);
                IniData["Main"]["PathToLogFile"] = IniSettings.GetValue("PathToLogFile");
                IniParser.WriteFile(DEFAULT_SETTINGS_PATH, IniData);
            }   
            catch(Exception ex)
            {
                Logger.Log(ex.Message, RecordType.ERROR, Source.System);
            }           
        }

        /// <summary>
        /// При логировании в системе
        /// </summary>
        private void LogSystem_OnLog(object sender, LoggerEventArgs e)
        {
            switch (e.Source)
            {
                case Source.Parser:
                    {                
                        Dispatcher.BeginInvoke(new ThreadStart(delegate
                        {
                            LogParserLbx.Items.Add(e.Record);
                        }));
                        break;
                    }
                case Source.UDP:
                    {
                        Dispatcher.BeginInvoke(new ThreadStart(delegate
                        {
                            LogUDPLbx.Items.Add(e.Record);
                        }));
                        break;
                    }
                case Source.System:
                    {
                        break;
                    }
            }                    
        }

        /// <summary>
        /// Изменение доступности полей в демо режиме парсера
        /// </summary>
        private void DemoParserCB_Checked(object sender, RoutedEventArgs e)
        {
            UrlControllerTbx.IsEnabled = (bool)!DemoParserCB.IsChecked;
        }

        /// <summary>
        /// Изменение доступности полей в демо режиме парсера
        /// </summary>
        private void DemoParserCB_Unchecked(object sender, RoutedEventArgs e)
        {
            UrlControllerTbx.IsEnabled = (bool)!DemoParserCB.IsChecked;
            UrlProcedureTbx.IsEnabled = (bool)!DemoParserCB.IsChecked;
        }

        /// <summary>
        /// Изменение доступности полей в демо режиме UDP
        /// </summary>
        private void DemoUDPParser_Checked(object sender, RoutedEventArgs e)
        {
            ListenningAddressTbx.IsEnabled = (bool)!DemoUDPCB.IsChecked;
            ListenningPortTbx.IsEnabled = (bool)!DemoUDPCB.IsChecked;
        }

        /// <summary>
        /// Изменение доступности полей в демо режиме UDP
        /// </summary>
        private void DemoUDPParser_Unchecked(object sender, RoutedEventArgs e)
        {
            ListenningAddressTbx.IsEnabled = (bool)!DemoUDPCB.IsChecked;
            ListenningPortTbx.IsEnabled = (bool)!DemoUDPCB.IsChecked;
        }

        /// <summary>
        /// Запуск парсера
        /// </summary>
        private void ParserLaunchBtn_Click(object sender, RoutedEventArgs e)
        {
            ParserSystem ParserSystem = new ParserSystem(IniSettings.GetValue("UrlController"), IniSettings.GetValue("UrlProcedure"));

            CancelTokenSourceParser = new CancellationTokenSource();
            TokenParser = CancelTokenSourceParser.Token;

            // Создание задачи для запуска HTTP клиента
            bool IsDemo = (bool)DemoParserCB.IsChecked;
            Task HTTPClientTask = new Task(() => ParserSystem.Start(TokenParser, IsDemo));

            try
            {
                
                //Запуск HTTP клиента          
                HTTPClientTask.Start();

                ParserLaunchBtn.IsEnabled = false;

                ParserStopBtn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Logger.Log(HTTPClientTask.Exception.InnerException.Message, RecordType.ERROR, Source.Parser);
            }
        }

        /// <summary>
        /// Остановка парсера
        /// </summary>
        private void ParserStopBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Отправка сигнала на отмену
                CancelTokenSourceParser.Cancel();

                // Блокирока кнопки на 1 сек
                Thread.Sleep(1000);

                // Освобождение ресурсов от токена и разблокировка кнопки
                CancelTokenSourceParser.Dispose();
                ParserLaunchBtn.IsEnabled = true;
                ParserStopBtn.IsEnabled = false;
                Logger.Log("Остановка HTTP клиента", RecordType.INFO, Source.Parser);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message, RecordType.ERROR, Source.Parser);
            }
        }

        /// <summary>
        /// Запуск UDP клиента
        /// </summary> 
        private void UDPLaunchBtn_Click(object sender, RoutedEventArgs e)
        {
            //ParserSystem ParserSystem = new ParserSystem(IniSettings.GetValue("UrlController"), IniSettings.GetValue("UrlProcedure"));
            UdpClient UDPClient = new UdpClient(IniSettings.GetValue("ListenAddress"), Convert.ToInt32(IniSettings.GetValue("ListenPort")));

            CancelTokenSourceUdp = new CancellationTokenSource();
            TokenUdp = CancelTokenSourceUdp.Token;

            bool IsDemo = (bool)DemoUDPCB.IsChecked;
            // Создание задачи для запуска HTTP клиента
            Task UDPClientTask = new Task(() => UDPClient.Start(TokenUdp, IsDemo));

            try
            {

                //Запуск HTTP клиента          
                UDPClientTask.Start();

                UDPLaunchBtn.IsEnabled = false;

                UDPStopBtn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Logger.Log(UDPClientTask.Exception.InnerException.Message, RecordType.ERROR, Source.UDP);
            }
        }

        /// <summary>
        /// Остановка UDP клиента
        /// </summary>
        private void UDPStopBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Отправка сигнала на отмену
                CancelTokenSourceUdp.Cancel();

                // Блокирока кнопки на 1 сек
                Thread.Sleep(1000);

                // Освобождение ресурсов от токена и разблокировка кнопки
                CancelTokenSourceUdp.Dispose();
                UDPLaunchBtn.IsEnabled = true;
                UDPStopBtn.IsEnabled = false;
                Logger.Log("Остановка UDP клиента", RecordType.INFO, Source.UDP);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message, RecordType.ERROR, Source.UDP);
            }
        }

        #endregion        
    }
}
