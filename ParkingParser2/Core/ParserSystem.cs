using AngleSharp.Dom;
using ParkingParser2.Parser;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ParkingParser2.Core
{
    class ParserSystem
    {
        private ControllerClient ControllerClient;

        private WebServerClient WebServerClient;

        public ParserSystem(string UrlController, string WebServerUrl)
        {
            this.ControllerClient = new ControllerClient(UrlController);
            this.WebServerClient = new WebServerClient(WebServerUrl);
        }

        public void Start(CancellationToken Token, bool Demo)
        {
            Logger.Log("Запуск HTTP клиента для системы парсинга, подождите некоторое время перед отправкой первого запроса.", RecordType.INFO, Source.Parser);

            //Бесконечный цикл для переодического парсинга
            while (true)
            {
                if (Token.IsCancellationRequested) return;
                OnTick(Demo);
                Thread.Sleep(1000);
            }
        }

        private async void OnTick(bool Demo)
        {
            // Если запущена демо версия
            if (!Demo) await SendRealData();
            else SendDemoData();
        }

        private async Task SendRealData()
        {
            try
            {
                // Отправляем запрос на микроконтроллер и получаем текст ответа
                string Response = await ControllerClient.GetHTMLPageAsync();

                try
                {
                    // Формируем документ из ответа
                    IDocument Document = await ControllerParser.GetDocument(Response);

                    //Получаем значения из документа
                    string[] Values = ControllerParser.GetValuesFromHTML(Document, 5);

                    // Отправка данных на веб-сервер
                    WebServerClient.SendGETRequest(Values);

                    Logger.Log("Отправлены данные: Sensor1 = " + Values[1] + " Sensor2 = " + Values[2] + " Sensor3 = " + Values[3] + " Sensor4 = " + Values[4], RecordType.INFO, Source.Parser);
                }
                catch (Exception ex)
                {
                    Logger.Log("Произошла ошибка подключения к веб-серверу", RecordType.ERROR, Source.Parser);
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Произошла ошибка подключения к контроллеру", RecordType.ERROR, Source.Parser);
            }
        }

        private void SendDemoData()
        {
            try
            {
                string[] Values = new string[5];

                //Демо-версия
                Values = DemoGenerate();

                // Отправка данных на веб-сервер
                WebServerClient.SendGETRequest(Values);

                Logger.Log("Отправлены данные: Sensor1 = " + Values[1] + " Sensor2 = " + Values[2] + " Sensor3 = " + Values[3] + " Sensor4 = " + Values[4], RecordType.INFO, Source.Parser);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message, RecordType.ERROR, Source.Parser);
            }
        }

        private string[] DemoGenerate()
        {
            Random Random = new Random();

            string[] Values = new string[5];

            for (int i = 0; i < 5; i++) Values[i] = Random.Next(1, 10).ToString();

            return Values;
        }
    }
}
