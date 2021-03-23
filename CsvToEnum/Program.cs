using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using CsvHelper;
using CsvHelper.Configuration;

namespace CsvToEnum
{
    class Program
    {
        /// <summary>
        /// 0 - nazwa pliku
        /// 1 - plik wyjściowy
        /// 2 - namespace
        /// 3 - EnumName
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var csvFile = args[0];
            var configurationFile = args[1];


            var document = new XmlDocument();
            document.Load(configurationFile);
            var separator = document.DocumentElement?["separator"]?.InnerText;
            var header = document.DocumentElement?["header"]?.InnerText ?? throw new XmlException("Brak nagłówka w pliku konfiguracyjnym");
            var everyRow = document.DocumentElement?["everyRow"]?.InnerText;
            var footer = document.DocumentElement?["footer"]?.InnerText ?? throw new XmlException("Brak stopki w pliku konfiguracyjnym");
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(header);

            var streamReader = new StreamReader(csvFile);
            var currentLine = streamReader.ReadLine();
            var headerRecord = currentLine.Split(separator).ToList();
            var propNameColumn = new Column("Key", headerRecord.IndexOf("Key"));
            var valueColumn = new Column("Value", headerRecord.IndexOf("Value"));
            var columns = new List<Column>();
            for (var i = 0; i < headerRecord.Count; i++)
            {
                columns.Add(new Column(headerRecord[i], i));
            }

            while (!streamReader.EndOfStream)
            {
                currentLine = streamReader.ReadLine();
                var record = new Record(currentLine.Split(separator).ToList(), columns);

                Dictionary<string, string> d = new Dictionary<string, string>();
                for (int i = 0; i < columns.Count; i++)
                {
                    var currentColumn = columns[i];
                    var valueInCurrentColumn = record.Values[currentColumn.Index];
                    d.Add(currentColumn.Name, valueInCurrentColumn);
                }

                var everyRowEdited = ReplaceTextInCurlyBracets(everyRow, d);
                stringBuilder.AppendLine("\t"+everyRowEdited);
                var propName = record.Values[propNameColumn.Index];
                var value = record.Values[valueColumn.Index];
                stringBuilder.AppendLine("\t"+propName + '=' + value + ',');
            }

            stringBuilder.Append(footer);
            Console.WriteLine(stringBuilder);
        }

        private static string ReplaceTextInCurlyBracets(string text, Dictionary<string, string> dictionary)
        {
            var result = text;
            foreach (var kvp in dictionary)
            {
                if (!result.Contains($"{{{kvp.Key}}}")) continue;
                result = result.Replace($"{{{kvp.Key}}}", kvp.Value);

            }
            return result;
        }
        internal class Column
        {
            public Column(string name, int index)
            {
                Index = index;
                Name = name;
            }

            public override string ToString()
            {
                return this.Name;
            }

            public int Index { get; }
            public string Name { get; }
        }

        internal class Record
        {

            public Record(List<string> values, List<Column> columns)
            {
                Columns = columns;
                Values = values;
            }

            public List<string> Values { get; set; }
            public List<Column> Columns { get; set; }
        }
    }

    public class MyList<T>: List<T>
    {
        internal T this[string name] => base.Find(obj => obj.ToString() == name);
    }
}