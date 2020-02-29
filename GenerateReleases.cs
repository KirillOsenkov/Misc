/*
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net472</TargetFramework>
    <Prefer32Bit>true</Prefer32Bit>
    <DebugType>embedded</DebugType>
  </PropertyGroup>

</Project>
*/

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace GenerateReleases
{
    class Program
    {
        static void Main(string[] args)
        {
            var directory = Environment.CurrentDirectory;
            if (args.Length == 1)
            {
                directory = args[0];
            }

            if (!Directory.Exists(directory))
            {
                Console.Error.WriteLine($"Directory {directory} doesn't exist");
                return;
            }

            var nupkg = Directory.GetFiles(directory, "*.nupkg").FirstOrDefault();
            if (!File.Exists(nupkg))
            {
                Console.Error.WriteLine($"No .nupkg files found in {directory}");
            }

            var releases = Path.Combine(Path.GetDirectoryName(nupkg), "RELEASES");
            var sha1 = SHA1Hash(nupkg).ToUpperInvariant();

            var text = $"{sha1} {Path.GetFileName(nupkg)} {new FileInfo(nupkg).Length}";
            File.WriteAllText(releases, text);
        }

        public static string SHA1Hash(string filePath)
        {
            return Hash(filePath, new SHA1Managed());
        }

        public static string Hash(string filePath, HashAlgorithm hash)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            using (hash)
            {
                var result = hash.ComputeHash(stream);
                return ByteArrayToHexString(result);
            }
        }

        public static string ByteArrayToHexString(byte[] bytes, int digits = 0)
        {
            if (digits == 0)
            {
                digits = bytes.Length * 2;
            }

            char[] c = new char[digits];
            byte b;
            for (int i = 0; i < digits / 2; i++)
            {
                b = ((byte)(bytes[i] >> 4));
                c[i * 2] = (char)(b > 9 ? b + 87 : b + 0x30);
                b = ((byte)(bytes[i] & 0xF));
                c[i * 2 + 1] = (char)(b > 9 ? b + 87 : b + 0x30);
            }

            return new string(c);
        }
    }
}
