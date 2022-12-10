using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Npgsql;
using System.Globalization;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.Collections.Generic;
using NpgsqlDbType = NpgsqlTypes.NpgsqlDbType;
using Nest;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace NoobCodTestTask
{
    [Delimiter(",")]
    public class Post
    {
        static readonly string[] split = { "['", "']", "', '" };
        public static Dictionary<string, int> rubricsDistAll;
        static int countId;
        public Dictionary<string, int> rubricsDist = new();
        //string[] rubricsListName;
        //List<int> rubricsListId;
        [Name("text")]
        public string Text { get; set; }
        [Name("created_date")]
        public DateTime CreatedDate { get; set; }
        [Name("rubrics")]
        public string Rubrics
        {
            get { return string.Join(", ", rubricsDist.Keys); }
            set
            {
                foreach (var rubric in value.Split(split, StringSplitOptions.RemoveEmptyEntries))
                {
                    rubricsDistAll.TryAdd(rubric, rubricsDistAll.Count + 1);
                    rubricsDist.TryAdd(rubric, rubricsDistAll[rubric]);
                }
            }
        }
        public int Id
        {
            get { return id; }
        }
        int id;
        static Post() { rubricsDistAll = new(); }
        public Post() { id = ++countId; }
    }

    internal class Program
    {
        internal static async Task Main(string[] args)
        {
            // Читаем конфиг
            Console.WriteLine("Читаем конфиг");
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //Проверяем поля конфига
            Console.WriteLine("Проверяем поля конфига");
            if (config.AppSettings.Settings["nameFile"].Value == "falsenull")
            {
                Console.Write("Введите путь к файлу данных: ");
                config.AppSettings.Settings["nameFile"].Value = Console.ReadLine();
                config.Save();
            }
            else Console.WriteLine("Используется следующий файл данных: " + config.AppSettings.Settings["nameFile"].Value);
            string nameFile = config.AppSettings.Settings["nameFile"].Value;

            // Подключаемся к эластик
            Console.WriteLine("Подключаемся к эластик");
            string indexElastic = "0";
            var settings = new ConnectionSettings(new Uri("http://localhost:9200")).DefaultIndex(indexElastic);
            var clientElastic = new ElasticClient(settings);

            //Dictionary<int, string> text = new();

            // Читаем csv
            Console.WriteLine("Читаем csv");
            // указываем путь к файлу csv
            using (var readerFile = new StreamReader(nameFile))
            using (var csv = new CsvReader(readerFile, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<Post>().ToList();
                HashSet<int> chekDistValue = new();
                List<int> id = records.Select(x => x.Id).ToList();
                List<string> text = records.Select(x => x.Text).ToList();
                List<DateTime> createdDate = records.Select(x => x.CreatedDate).ToList();
                List<string> rubricName = Post.rubricsDistAll.Select(x => x.Key).ToList();
                List<int> rubricId = Post.rubricsDistAll.Select(x => x.Value).ToList();
                // Конектимся к БД
                Console.WriteLine("Конектимся к БД");
                var connectionString = "Host=localhost;Username=postgres;Password=yfrfpe.obq;Database=post";
                var conn = new NpgsqlConnection(connectionString);
                conn.Open();
                Console.WriteLine("открываем транзакцию");
                var transaction = conn.BeginTransaction(); // открываем транзакцию
                // Заполняем таблицу message
                Console.WriteLine("Заполняем таблицы");
                var cmd1 = new NpgsqlCommand("INSERT INTO message (text_id, text, created_date) select unnest ($1), unnest ($2), unnest ($3)", conn, transaction)
                {
                    Parameters =
                        {
                            new() { Value =  id},
                            new() { Value =  text},
                            new() { Value =  createdDate}
                        }
                };
                cmd1.ExecuteNonQuery();

                // Заполняем таблицу rubrics
                Console.WriteLine("Заполняем таблицу rubrics");
                var cmd2 = new NpgsqlCommand("INSERT INTO rubrics (rubrics_id, rubrics_name) select unnest ($1), unnest ($2)", conn, transaction)
                {
                    Parameters =
                    {
                        new() { Value = rubricId },
                        new() { Value = rubricName }
                    }
                };
                cmd2.ExecuteNonQuery();

                // Заполняем связывающию таблицу message_rubrics
                Console.WriteLine("Заполняем связывающию таблицу message_rubrics");
                foreach (var post in records)
                {
                    foreach (var rubric in post.rubricsDist)
                    {
                        var cmd3 = new NpgsqlCommand("INSERT INTO message_rubrics (text_id, rubrics_id) VALUES ($1, $2)", conn, transaction)
                        {
                            Parameters =
                        {
                            new() { Value = post.Id },
                            new() { Value = rubric.Value }
                        }
                        };
                        cmd3.ExecuteNonQuery();
                    }
                }
                transaction.Commit(); // завершаем транзакцию                    
                conn.Close(); // закрываем соединение
            }
            Console.Write("ПОИСК: ");
            string serch = Console.ReadLine();
            var searchRequest = new SearchRequest<Post>(Nest.Indices.All)
            {
                From = 0,
                Size = 2,
                Query = new MatchQuery
                {
                    Field = Infer.Field<Post>(f => f.Text),
                    Query = serch
                }
            };

            var searchResponse = await clientElastic.SearchAsync<Post>(searchRequest);
            Console.WriteLine();
        }
        internal static NpgsqlConnection DateBaseConect(Configuration config)
        {
            // Конектимся к БД
            string hostDB = "Host=" + config.AppSettings.Settings["hostDB"].Value;
            string userNameDB = "Username=" + config.AppSettings.Settings["userNameDB"].Value;
            string passwordDB = "Password=" + config.AppSettings.Settings["passwordDB"].Value;
            string nameDB = "Database=" + config.AppSettings.Settings["nameDB"].Value;

            string connectionString = hostDB + ";" + userNameDB + ";" + passwordDB + ";" + nameDB + ";";
            // var dataSource = NpgsqlDataSource.Create(connectionString); // возможно пригодиться на вытягивании
            var conn = new NpgsqlConnection(connectionString);
            return conn;

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
            //("INSERT INTO some_table (some_field) VALUES (8)");
            //await command.ExecuteNonQueryAsync();
            await using (var cmd = dataSource.CreateCommand("INSERT INTO message (text) VALUES ($1)"))
            {
                cmd.Parameters.AddWithValue("Hello world");
                await cmd.ExecuteNonQueryAsync();
            }
            // Retrieve all rows
            await using (var cmd = dataSource.CreateCommand("SELECT table_name FROM information_schema.tables WHERE table_schema NOT IN ('information_schema','pg_catalog');"))
            await using (var readerDB = await cmd.ExecuteReaderAsync())
            {
                while (await readerDB.ReadAsync())
                {
                    Console.WriteLine(readerDB.GetString(0));
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