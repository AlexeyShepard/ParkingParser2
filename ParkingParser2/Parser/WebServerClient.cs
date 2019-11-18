using ParkingParser2.Core;
using System;
using System.Net;

namespace ParkingParser2.Parser
{
    class WebServerClient
    {
        private string Url;

        public WebServerClient(string WebServerUrl)
        {
            this.Url = WebServerUrl;
        }
        
        public void SendGETRequest(string[] Params)
        {
            DateTime Date = new DateTime();
            Date = DateTime.Now;

            try
            {
                // Формирование HTTP запроса
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(this.Url + "?sensor1=" + Params[1] + "&sensor2=" + Params[2] + "&sensor3=" + Params[3] + "&sensor4=" + Params[4] + "&date=" + Date.ToString("yyyy-MM-dd HH:mm:ss"));

                // Отправка и получение ответа
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (Exception ex)
            {
                Logger.Log("Веб-сервер: " + ex.Message, RecordType.ERROR, Source.Parser);
            }
        }
    }
}
