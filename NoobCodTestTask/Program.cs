using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Npgsql;
using System.Globalization;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.Collections.Generic;


namespace NoobCodTestTask
{
    [Delimiter(",")]
    public class Post
    {
        public static readonly string[] split = { "['", "']", "', '" };
        string[] rubricsList;
        static int countId;
        [Name("text")]
        public string Text { get; set; }
        [Name("created_date")]
        public DateTime CreatedDate { get; set; }
        [Name("rubrics")]
        public string Rubrics
        {
            get { return string.Join(", ", rubricsList); }            
            set { rubricsList = value.Split(split, StringSplitOptions.RemoveEmptyEntries); }
        }
        public int Id 
        { 
            get { return id; }
        }
        int id;
        public Post() { id = ++countId; }
    }
    internal class ConectDB
    {

    }

    internal class Program
    {
        internal static async Task Main(string[] args)
        {
            // Читаем конфиг
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //Проверяем поля конфига
            if (config.AppSettings.Settings["nameFile"].Value == "falsenull")
            {
                Console.Write("Введите путь к файлу данных: ");
                config.AppSettings.Settings["nameFile"].Value = Console.ReadLine();
                config.Save();
            }
            else Console.Write("Используется следующий файл данных: " + config.AppSettings.Settings["nameFile"].Value);
            string nameFile = config.AppSettings.Settings["nameFile"].Value;

            // Читаем csv
            // указываем путь к файлу csv

            using (var reader = new StreamReader(nameFile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<Post>();
                var dataSource = DateBaseConect(config);
                foreach (var post in records)
                {
                    DateBaseWrite(post, dataSource);
                    Console.ReadKey();
                }
                
            }

        }
        internal static NpgsqlDataSource DateBaseConect(Configuration config)
        {
            // Конектимся к БД
            string hostDB = "Host=" + config.AppSettings.Settings["hostDB"].Value;
            string userNameDB = "Username=" + config.AppSettings.Settings["userNameDB"].Value;
            string passwordDB = "Password=" + config.AppSettings.Settings["passwordDB"].Value;
            string nameDB = "Database=" + config.AppSettings.Settings["nameDB"].Value;

            string connectionString = hostDB + ";" + userNameDB + ";" + passwordDB + ";" + nameDB + ";";
            var dataSource = NpgsqlDataSource.Create(connectionString);
            return dataSource;

        }
        internal static async Task DateBaseWrite(Post post, NpgsqlDataSource dataSource)
        {
            // Insert some data
            await using (var cmd = dataSource.CreateCommand("INSERT INTO message (text) VALUES ($1)"))
            {
                cmd.Parameters.AddWithValue("Hello world");
                await cmd.ExecuteNonQueryAsync();
            }
            Console.ReadKey();
        }
        internal static async Task RoughDraft()
        {
            // Правим конфиг
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["nameFile"].Value = Console.ReadLine();
            config.Save();

            ConfigurationManager.RefreshSection("appSettings");

            Console.WriteLine(config.AppSettings.Settings["nameFile"].Value);
            string nameFile = config.AppSettings.Settings["nameFile"].Value;
            Console.ReadLine();

            // Read all the keys from the config file
            NameValueCollection sAll;
            sAll = ConfigurationManager.AppSettings;

            foreach (string s in sAll.AllKeys)
                Console.WriteLine(s + " Value: " + sAll.Get(s));
            Console.ReadLine();
            // Конектимся к БД
            string hostDB = "Host=";
            string userNameDB = "Username=";
            string passwordDB = "Password=";
            string nameDB = "Database=";

            string connectionString = hostDB + ";" + userNameDB + ";" + passwordDB + ";" + nameDB + ";";
            await using var dataSource = NpgsqlDataSource.Create(connectionString);


            // Insert some data
            await using (var cmd = dataSource.CreateCommand("INSERT INTO message (text) VALUES ($1)"))
            {
                cmd.Parameters.AddWithValue("Hello world");
                await cmd.ExecuteNonQueryAsync();
            }

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
            
            using (var reader = new StreamReader(nameFile))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<Post>();
                foreach (var record in records)
                {
                    Console.ReadKey();
                }

            }
            // Подключаемся к эластика


            // Обрабатываем поисковый запрос

            
        }
    }
}