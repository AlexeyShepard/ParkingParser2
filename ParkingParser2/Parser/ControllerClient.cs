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

                this.Response = await Client.GetStringAsync(this.Url);

                return this.Response;
            }
            catch (Exception ex)
            {
                Logger.Log("Контроллер: " + ex.Message, RecordType.ERROR, Source.Parser);
                return this.Response;
            }
        }
    }
}
