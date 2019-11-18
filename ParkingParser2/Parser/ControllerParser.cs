using AngleSharp;
using AngleSharp.Dom;
using ParkingParser2.Core;
using System;
using System.Threading.Tasks;


namespace ParkingParser2.Parser
{
    public static class ControllerParser
    {
        /// <summary>
        /// Формирование документа из исходного текста
        /// </summary>
        /// <param name="Source">Исходный текст</param>
        /// <returns>Готовый документ</returns>
        public static async Task<IDocument> GetDocument(string Source)
        {
            try
            {
                IConfiguration config = Configuration.Default;

                IBrowsingContext context = BrowsingContext.New(config);

                IDocument document = await context.OpenAsync(req => req.Content(Source));

                return document;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message, RecordType.ERROR, Core.Source.Parser);
                return null;
            }

        }

        /// <summary>
        /// Парсинг HTML документа
        /// </summary>
        /// <param name="Document">Документ, который необходимо парсить</param>
        /// <returns>Массив значений</returns>
        public static string[] GetValuesFromHTML(IDocument Document, int Length)
        {
            try
            {
                IHtmlCollection<IElement> Values = Document.QuerySelectorAll("td");

                string[] ValueList = new string[Length];

                int index = 0;

                foreach (IElement Value in Values)
                {
                    ValueList[index] = Value.TextContent;
                    index++;
                }

                return ValueList;
            }
            catch (Exception ex)
            {
                Logger.Log("Не верное кол-во данных с HTML страницы", RecordType.ERROR, Source.Parser);
                return new string[Length];
            }
        }
    }
}
