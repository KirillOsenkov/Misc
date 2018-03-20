using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JsonParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var file = @"airportcodes.txt";
            var lines = File.ReadAllLines(file);
            var codes = new Dictionary<string, Dictionary<string, string>>();
            var keys = new HashSet<string>();
            foreach (var line in lines)
            {
                var dictionary = ParseLine(line);
                if (dictionary.TryGetValue("id", out var id))
                {
                    codes.Add(id, dictionary);
                }
                else
                {
                    Console.WriteLine("Missing id: " + line);
                }
                
                keys.UnionWith(dictionary.Keys);
            }

            var sb = new StringBuilder();
            foreach (var kvp in codes)
            {
                var code = kvp.Value;
                var Id = GetValue(code, "id");
                var Name = GetValue(code, "name");
                var NameEnglish = GetValue(code, "nameEnglish");
                var State = GetValue(code, "state") ?? GetValue(code, "stateShort");
                var Country = GetValue(code, "country");
                var City = GetValue(code, "city");
                var City2 = GetValue(code, "city2");
                var City3 = GetValue(code, "city3");
                sb.AppendLine($"    (\"{Id}\", \"{Name}\", \"{NameEnglish}\", \"{State}\", \"{Country}\", \"{City}\", \"{City2}\", \"{City3}\"),");
            }

            File.WriteAllText(@"C:\temp\airportcodes.txt", sb.ToString());

            foreach (var key in keys.OrderBy(s => s))
            {
                Console.WriteLine(key);
            }

            Console.ReadKey();
        }

        private static string GetValue(Dictionary<string, string> dictionary, string key)
        {
            dictionary.TryGetValue(key, out var result);
            return result;
        }

        private static Dictionary<string, string> ParseLine(string line)
        {
            line = line.Substring(1, line.Length - 3);

            var parts = new List<string>();
            bool inQuotes = false;
            int start = 0;
            int index = 0;
            foreach (var ch in line)
            {
                if (ch == ',' && !inQuotes)
                {
                    parts.Add(line.Substring(start, index - start));
                    start = index + 1;
                }
                else if (ch == '"')
                {
                    inQuotes = !inQuotes;
                }

                index++;
            }

            parts.Add(line.Substring(start, index - start));

            var dictionary = new Dictionary<string, string>();
            foreach (var part in parts)
            {
                var nameValueParts = part.Split(':');
                var name = nameValueParts[0];
                var value = nameValueParts[1];
                value = value.Substring(1, value.Length - 2);
                dictionary[name] = value;
            }

            return dictionary;
        }
    }
}
