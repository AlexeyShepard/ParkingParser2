using ParkingParser2.Core;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ParkingParser2.Parser
{
    class ControllerClient
    {
        private string Url;

        private string Response;

        public ControllerClient(string Url)
        {
            this.Url = Url;
            this.Response = "";
        }

        /// <summary>
        /// Получение исходного текста страницы
        /// </summary>
        /// <returns>Возращение исходного текста страницы</returns>
        public async Task<string> GetHTMLPageAsync()
        {
            try
            {
                HttpClient Client = new HttpClient();

                HttpRequestMessage HttpRequestMessage = new HttpRequestMessage(HttpMethod.Get, this.Url);

                HttpResponseMessage HttpResponseMessage = await Client.SendAsync(HttpRequestMessage);

                string StatusCode = HttpResponseMessage.StatusCode.ToString();

                Logger.Log("Код статуса: " + StatusCode, RecordType.ERROR, Source.Parser);

                this.Response = await HttpResponseMessage.Content.ReadAsStringAsync();

                return this.Response;
            }
            catch (Exception ex)
            {
                Logger.Log("Контроллер: " + ex.Message + " " + ex.HResult, RecordType.ERROR, Source.Parser);
                return this.Response;
            }
        }
    }
}
