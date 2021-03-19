using System;
using System.IO;
using System.Text;
using System.Threading;

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
            var inputFile = args[0];
            var outputFile = args[1];
            var @namespace = args[2];
            var enumName = args[3];
            var separator = args[4];

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("using System;")
                .AppendLine()
                .AppendLine($"namespace {@namespace}")
                .AppendLine("{")
                .AppendLine($"\tpublic enum {enumName}")
                .AppendLine("\t{");

            foreach (var line in File.ReadAllLines(inputFile))
            {
                var lineSplitted = line.Split(separator);
                var enumObjectName = lineSplitted[0];
                var enumValue = lineSplitted[1];
                var firstAttribute = lineSplitted[2];
                var secondAttribute = lineSplitted[3];

                stringBuilder.AppendLine($"\t\t[DomyslnePowiadomienie({firstAttribute}, {secondAttribute})]")
                    .AppendLine($"\t\t{enumObjectName} = {enumValue},");
            }

            stringBuilder.AppendLine("\t}").AppendLine("}");

            File.WriteAllText(outputFile, stringBuilder.ToString());
        }
    }
}
