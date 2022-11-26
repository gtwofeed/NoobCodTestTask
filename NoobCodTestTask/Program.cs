using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Collections;
using System.Globalization;
using System.IO;

namespace NoobCodTestTask
{
    [Delimiter(",")]
    //[CultureInfo("")]
    public class Post
    {
        [Name("text")]
        public string Text { get; set; }
        [Name("created_date")]
        public string CreatedDate { get; set; }
        [Name("rubrics")]
        public string Rubrics { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // Читаем csv
            // указываем путь к файлу csv
            string pathCsvFile = "...\\posts\\posts.csv";
            using (var reader = new StreamReader(pathCsvFile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<Post>();
                Console.ReadKey();
            }
            
            // Заполняем БД
            // Подключаемся к эластика
            // Обрабатываем поисковый запрос
        }
    }
}