using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IISLogViewer
{
    class FileProcessor
    {
        static void Main(string[] args)
        {
            var logFolder = @"C:\temp\indexlogs";
            var logFiles = Directory.GetFiles(logFolder, "u_ex*.log");

            var queryCountByUser = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var file in logFiles)
            {
                var usersAndQueries = new FileProcessor().TranslateFile(file);
                if (usersAndQueries == null)
                {
                    continue;
                }

                foreach (var kvp in usersAndQueries)
                {
                    queryCountByUser.TryGetValue(kvp.Key, out int count);
                    queryCountByUser[kvp.Key] = count + kvp.Value.Count;
                }
            }

            var topUsers = queryCountByUser.OrderByDescending(kvp => kvp.Value).ToArray();
            var text = string.Join("\r\n", topUsers.Select(t => $"{t.Key},{t.Value}"));

            var allUsersFiles = Directory.GetFiles(logFolder, "*.users.txt");
            var lastMonthUsers = GetLastTimeSpan(allUsersFiles, TimeSpan.FromDays(30));
            var lastWeekUsers = GetLastTimeSpan(allUsersFiles, TimeSpan.FromDays(7));

            WriteUsers(logFolder, "allusers.txt", allUsersFiles);
            WriteUsers(logFolder, "lastweekusers.txt", lastWeekUsers);
            WriteUsers(logFolder, "lastmonthusers.txt", lastMonthUsers);
        }

        private readonly Dictionary<string, HashSet<string>> usersAndQueries = new Dictionary<string, HashSet<string>>();

        private static void WriteUsers(string logFolder, string fileName, IEnumerable<string> usersFiles)
        {
            var allUsers = GetAllUsers(usersFiles);
            File.WriteAllLines(Path.Combine(logFolder, fileName), allUsers);
        }

        private void WriteUserList(string translatedFile)
        {
            var filePath = Path.ChangeExtension(translatedFile, ".users.txt");
            File.WriteAllLines(filePath, usersAndQueries.Keys.OrderBy(s => s));
        }

        private static IEnumerable<string> GetAllUsers(IEnumerable<string> usersFiles)
        {
            return usersFiles
                .SelectMany(f => File.ReadAllLines(f))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(u => u, StringComparer.OrdinalIgnoreCase);
        }

        private static IEnumerable<string> GetLastTimeSpan(string[] allUsersFiles, TimeSpan timeSpan)
        {
            return allUsersFiles.Where(f => (DateTime.UtcNow - GetDateFromFileName(f)) < timeSpan);
        }

        private static DateTime GetDateFromFileName(string fileName)
        {
            fileName = Path.GetFileNameWithoutExtension(fileName);
            fileName = fileName.Substring(4, 6);
            int year = int.Parse(fileName.Substring(0, 2)) + 2000;
            int month = int.Parse(fileName.Substring(2, 2));
            int day = int.Parse(fileName.Substring(4, 2));
            return new DateTime(year, month, day);
        }

        private Dictionary<string, HashSet<string>> TranslateFile(string file)
        {
            var translatedFile = Path.ChangeExtension(file, ".txt");
            if (File.Exists(translatedFile) &&
                File.Exists(Path.ChangeExtension(translatedFile, ".users.txt")) &&
                new FileInfo(translatedFile).LastWriteTimeUtc > new FileInfo(file).LastWriteTimeUtc)
            {
                return null;
            }

            try
            {
                Console.WriteLine("Processing file: " + file);

                var lines = new List<string>();
                using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var streamReader = new StreamReader(stream))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }

                var outputLines = new List<string>();

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    {
                        continue;
                    }

                    var processedLine = ProcessLine(line);
                    if (processedLine != null)
                    {
                        outputLines.Add(processedLine);
                    }
                }

                File.WriteAllLines(translatedFile, outputLines);
                WriteUserList(translatedFile);
            }
            catch (Exception)
            {
            }

            return usersAndQueries;
        }

        private string ProcessLine(string line)
        {
            var parts = line.Split(' ');

            var csUriStem = parts[4];
            var csUriQuery = parts[5];
            var csUserName = parts[7];
            var csStatus = parts[10];
            if (csUserName == "REDMOND\\ddmntr" || csUserName == "REDMOND\\ddserv7")
            {
                return null;
            }

            if (csUriStem.StartsWith("/Content/Icons/", StringComparison.OrdinalIgnoreCase) ||
                csUriStem == "/results.html" ||
                csUriStem == "/scripts.js" ||
                csUriStem == "/styles.css" ||
                csUriStem == "/header.html" ||
                csUriStem == "/favicon.ico" ||
                csUriStem == "/overview.html")
            {
                return null;
            }

            if (csUserName == "-" && csUriStem == "/" && csUriQuery == "-")
            {
                return null;
            }

            if (csUriStem == "/api/symbols/")
            {
                csUriStem = "";
            }

            if (csUriQuery.StartsWith("symbol="))
            {
                csUriQuery = csUriQuery.Substring(7);
            }

            var queryString = csUriStem + " " + csUriQuery;
            var result = csUserName + "\t" + queryString;
            if (result == "-	/technicaldetails.html -")
            {
                return null;
            }

            AddUserAndQuery(csUserName, queryString);

            return result;
        }

        private void AddUserAndQuery(string csUserName, string queryString)
        {
            HashSet<string> bucket = null;
            if (!usersAndQueries.TryGetValue(csUserName, out bucket))
            {
                bucket = new HashSet<string>();
                usersAndQueries.Add(csUserName, bucket);
            }

            bucket.Add(queryString);
        }
    }
}
