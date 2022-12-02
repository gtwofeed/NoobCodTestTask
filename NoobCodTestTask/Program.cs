﻿using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Npgsql;
using System.Globalization;

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
            await RoughDraft();
        }
        static async Task RoughDraft()
        {
            var connectionString =
                "Host=localhost;" +
                "Username=postgres;" +
                "Password=****;" +
                "Database=stepik";
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
        }
    }
}