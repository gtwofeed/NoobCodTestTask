using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Npgsql;
using System.Globalization;
using System.Configuration;
using System.Collections.Specialized;
using System.Xml.Linq;
using System.Collections.Generic;
using NpgsqlDbType = NpgsqlTypes.NpgsqlDbType;


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
                //    = value.Split(split, StringSplitOptions.RemoveEmptyEntries);
                //foreach (var rubric in rubricsListName)
                //{
                //    rubricsDist.TryAdd(rubric, rubricsDist.Count + 1);
                //    rubricsListId.Add(rubricsDist[rubric]);
                //}
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
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //Проверяем поля конфига
            if (config.AppSettings.Settings["nameFile"].Value == "falsenull")
            {
                Console.Write("Введите путь к файлу данных: ");
                config.AppSettings.Settings["nameFile"].Value = Console.ReadLine();
                config.Save();
            }
            else Console.WriteLine("Используется следующий файл данных: " + config.AppSettings.Settings["nameFile"].Value);
            string nameFile = config.AppSettings.Settings["nameFile"].Value;

            // Читаем csv
            // указываем путь к файлу csv
            using (var readerFile = new StreamReader(nameFile))
            using (var csv = new CsvReader(readerFile, CultureInfo.InvariantCulture))
            {
                // Конектимся к БД
                var dataSource = DateBaseConect(config);

                var records = csv.GetRecords<Post>();

                HashSet<int> chekDistValue = new();
                foreach (var post in records)
                {
                    // Заполняем таблицу message
                    //await using (var cmd = dataSource.CreateCommand("INSERT INTO message (text_id, text, created_date) VALUES ($1, $2, $3)"))
                    //{
                    //    cmd.Parameters.AddWithValue(NpgsqlDbType.Integer, post.Id);
                    //    cmd.Parameters.AddWithValue(post.Text);
                    //    cmd.Parameters.AddWithValue(NpgsqlDbType.Timestamp, post.CreatedDate);
                    //    await cmd.ExecuteNonQueryAsync();
                    //}

                    // Заполняем таблицу rubrics
                    await using (var cmd = dataSource.CreateCommand("INSERT INTO rubrics (rubrics_id, rubrics_name) VALUES ($1, $2)"))
                    {
                        foreach (var rubric in post.rubricsDist)
                        {
                            if (chekDistValue.Add(rubric.Value))
                            {
                                cmd.Parameters.AddWithValue(NpgsqlDbType.Integer, rubric.Value);
                                cmd.Parameters.AddWithValue(rubric.Key);
                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    // Заполняем связывающию таблицу message_rubrics
                    //await using (var cmd = dataSource.CreateCommand("INSERT INTO message_rubrics (text_id, rubrics_id) VALUES ($1, $2)"))
                    //{
                    //    foreach (var rubric in post.rubricsDist)
                    //    {
                    //        cmd.Parameters.AddWithValue(NpgsqlDbType.Integer, post.Id);
                    //        cmd.Parameters.AddWithValue(NpgsqlDbType.Integer, rubric.Value);
                    //        await cmd.ExecuteNonQueryAsync();
                    //    }
                    //}
                }

            }
            Console.WriteLine("END");

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
            // var conn = new NpgsqlConnection(connectionString);
            return dataSource;

        }
        internal static async Task DateBaseWrite(Post post, NpgsqlDataSource dataSource)
        {
            //string values = string.Format("{0}, '{1}', '{2}'", post.Id, post.Text, post.CreatedDate);
            //int count = 0;
            //Console.WriteLine($"INSERT INTO message (text_id, text, created_date) VALUES ({post.Id}, '{post.Text}', '{post.CreatedDate}'");
            //await using (var cmd = dataSource.CreateCommand($"INSERT INTO message (text_id, text, created_date) VALUES (1, 'drha', '{post.CreatedDate}'"));
            //await cmd.ExecuteNonQueryAsync();
            //{
            //cmd.Parameters.AddWithValue(values);
            //Console.WriteLine(++count);
            // dataSource
            //}
            //string values = string.Format("{0}, '{1}', '{2}'", post.Id, post.Text, post.CreatedDate);
            //await conn.OpenAsync();
            await using var cmd = dataSource.CreateCommand($"INSERT INTO message (text_id, text, created_date) VALUES ({post.Id}, '{post.Text}', '{post.CreatedDate}'");
            await cmd.ExecuteNonQueryAsync();
            //Console.ReadLine();
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