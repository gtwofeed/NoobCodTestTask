using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Npgsql;
using System.Globalization;
using System.Configuration;
using System.Collections.Specialized;


namespace NoobCodTestTask
{
    [Delimiter(",")]
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
        static async Task Main(string[] args)
        {
            // Читаем конфиг
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //Проверяем поля конфига
            if (config.AppSettings.Settings["nameFile"].Value == "falsenull")
            {
                config.AppSettings.Settings["nameFile"].Value = Console.ReadLine();
                config.Save();
            }
            else config.AppSettings.Settings["nameFile"].Value;

            // Конектимся к БД
            string hostDB = "Host=";
            string userNameDB = "Username=";
            string passwordDB = "Password=";
            string nameDB = "Database=";

            hostDB += 

            string connectionString = hostDB + ";" + userNameDB + ";" + passwordDB + ";" + nameDB + ";";
            await using var dataSource = NpgsqlDataSource.Create(connectionString);

            Console.ReadLine();
        }
        static async Task RoughDraft()
        {
            var connectionString =
                "Host=localhost;" +
                "Username=postgres;" +
                "Password=****;" +
                "Database=post";
            await using var dataSource = NpgsqlDataSource.Create(connectionString);

            // Retrieve all rows
            await using (var cmd = dataSource.CreateCommand("SELECT title FROM book"))
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    Console.WriteLine(reader.GetString(0));
                }
            }
            // Читаем csv
            // указываем путь к файлу csv
            Console.ReadKey();
            string pathCsvFile = "C:\\Users\\Ruslan\\source\\repos\\NoobCodTestTask\\posts\\posts.csv";
            using (var reader = new StreamReader(pathCsvFile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<Post>();
                Console.ReadKey();
            }

            // Заполняем БД
            // Подключаемся к эластика
            // Обрабатываем поисковый запрос

            // Правим конфиг
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["nameFile"].Value = Console.ReadLine();
            config.Save();

            ConfigurationManager.RefreshSection("appSettings");

            Console.WriteLine(config.AppSettings.Settings["nameFile"].Value);
            Console.ReadLine();

            // Read all the keys from the config file
            NameValueCollection sAll;
            sAll = ConfigurationManager.AppSettings;

            foreach (string s in sAll.AllKeys)
                Console.WriteLine(s + " Value: " + sAll.Get(s));
            Console.ReadLine();
        }
    }
}