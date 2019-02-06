using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s1e2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(@"enter zip archive file path (e.g. C:\temp\s1e2\loremipsum.zip)");
            var zipPath = Console.ReadLine().Replace("\"", "");

            // validating (trying to open) and asking to try again in case of input issues
            while (true)
                {
                try
                {
                    var archive = ZipFile.OpenRead(zipPath);
                    break;
                }
                catch
                {
                    Console.WriteLine(@"provided archive can`t be opened, please try again (e.g. C:\temp\s1e2\loremipsum.zip)");
                    zipPath = Console.ReadLine().Replace("\"", "");
                }
            }

            // calculating compression ratio and oldest file (using "Last Modified" for this, because "Date Created" does not represent meaningul info)
            // accepting error in case of empty archive or any other magic
            try {
            var archive = ZipFile.OpenRead(zipPath);
            var oldestFile = archive.Entries[0];

            foreach (var entry in archive.Entries)
            {
                float compressedRatio = (float)entry.CompressedLength / entry.Length * 100;
                if (entry.LastWriteTime < oldestFile.LastWriteTime)
                {
                    oldestFile = entry;
                }
                Console.WriteLine(string.Format("File: {0}, Compression Ration {1:F2}%", entry.FullName, compressedRatio));
            }
            var days = (DateTime.Now.Date - oldestFile.LastWriteTime.Date).TotalDays;
            Console.WriteLine($"{oldestFile.FullName} is oldest file and it is {days} days old (based on Last Modified)");
            }
            catch
            {
                Console.WriteLine("calculations for compression ratio and oldest file were not successful, double-check the data provided");
            }

            // archive extraction, keep trying again in case something goes wrong (too many validation and input errors to consider)
            Console.WriteLine(@"specify directory to extract files (e.g. C:\temp\s1e2\extracted\)");
            var extractPath = Console.ReadLine();

            while (true)
            {
                try
                {
                    Directory.CreateDirectory(extractPath);
                    ZipFile.ExtractToDirectory(zipPath, extractPath);
                    Console.WriteLine("extraction completed");
                    break;
                }
                catch
                {
                    Console.WriteLine(@"extraction failed, specify new directory to extract (e.g. C:\temp\s1e2\extracted\)");
                    extractPath = Console.ReadLine();
                }
            }
            Console.WriteLine("press ENTER to exit the app");
            Console.ReadLine();
        }
    }
}