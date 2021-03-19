using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EnumToCsv
{
    class Program
    {
        private const char separator = '|';
        static void Main(string[] args)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var allLines = File.ReadAllLines(args[0]);
            var enumObjectsWithIndex = allLines.Select((line, index) => new Tuple<string, int>(line, index))
                .Where((tuple) => tuple.Item1.Contains('='));
            foreach (var o in enumObjectsWithIndex)
            {
                var line = o.Item1.Trim();
                var enumName = line.Split('=')[0].Trim();
                var enumValue = line.Split('=')[1].Trim().Split(',')[0];
                var lineIndex = o.Item2;
                string firstAttribute = string.Empty;
                string secondAttribute = string.Empty;
                if (allLines[lineIndex -1].Contains('['))
                {
                    var lineWithAttributes = allLines[lineIndex - 1].Trim();
                    firstAttribute = lineWithAttributes.Substring(lineWithAttributes.IndexOf('(')+1).Split(',')[0];
                    secondAttribute = lineWithAttributes.Substring(lineWithAttributes.IndexOf(',')+1).Split(')')[0].Trim();
                }

                stringBuilder.Append(enumName).Append(separator).Append(enumValue).Append(separator).Append(firstAttribute).Append(separator).Append(secondAttribute).AppendLine();
            }
            File.WriteAllText(args[1], stringBuilder.ToString());
        }
    }
}
