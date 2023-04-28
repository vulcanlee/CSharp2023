using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace csScanDirectoryForExtension
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string targetDirectory = @"C:\Users\vulca\Downloads\0000 Google Drive New";
            var result = CountFileExtensions(targetDirectory);

            foreach (var item in result.OrderByDescending(x => x.Value))
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }
            Console.WriteLine();
            Console.WriteLine($"統計檔案類型:{result.Count}");
        }
        static Dictionary<string, int> CountFileExtensions(string directoryPath)
        {
            var fileExtensionCounts = new Dictionary<string, int>();

            void ProcessDirectory(DirectoryInfo directoryInfo)
            {
                // Process all files in the current directory
                foreach (var fileInfo in directoryInfo.GetFiles())
                {
                    string extension = fileInfo.Extension.ToLower();

                    if (fileExtensionCounts.ContainsKey(extension))
                    {
                        fileExtensionCounts[extension]++;
                    }
                    else
                    {
                        fileExtensionCounts.Add(extension, 1);
                    }
                }

                // Recursively process all subdirectories
                foreach (var subDirectoryInfo in directoryInfo.GetDirectories())
                {
                    ProcessDirectory(subDirectoryInfo);
                }
            }

            ProcessDirectory(new DirectoryInfo(directoryPath));
            return fileExtensionCounts;
        }
    }
}