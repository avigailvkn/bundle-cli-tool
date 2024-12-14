using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace fib
{
    internal class Methods
    {
        public static IEnumerable<FileInfo> ReadFiles(string sourceDir, bool recursive, string[] extensions, string[] excludedDirectories)
        {
            //TODO: exceptions handle
            DirectoryInfo currentDir = new DirectoryInfo(sourceDir);
            if (!currentDir.Exists)
            {
                PrintError($"Source directory not found: {currentDir.FullName}");
                return null;
            }
                
                



            if (recursive)
            {
                IEnumerable<string> subFiles = new List<string>();
                foreach (string extension in extensions)
                {
                    //get all subFiles pathes for this extension
                    subFiles = subFiles.Concat(
                        Directory.EnumerateFiles(currentDir.FullName, $"*{extension}", SearchOption.AllDirectories)
                        .Where(file => !excludedDirectories.Any(exDir => file.Substring(currentDir.FullName.Length).Contains(@$"\{exDir}\"))).ToList()
                        //from file in Directory.EnumerateFiles(currentDir.FullName, $"*{extension}", SearchOption.AllDirectories)
                        //                       where !excludedDirectories.Any(exDir => file.Substring(currentDir.FullName.Length).Contains(@$"\{exDir}\"))
                        //                       select file
                                               ); 
                }


                IEnumerable<FileInfo> files = subFiles.Select(s => new FileInfo(s)).ToList();
                    //from file in subFiles
                    //                          select new FileInfo(file);

                return files;
            }
            else
            {
                IEnumerable<FileInfo> files = currentDir.GetFiles().Where(file=> extensions.Contains(file.Extension)).ToList();
                    //from file in currentDir.GetFiles()
                    //                          where extensions.Contains(file.Extension)
                    //                          select file;
                return files;
            }

        }
        public static string[] Complete(string[] extensions)
        {
            if (extensions.Contains("all"))
                return Info.possibleExtensions;

            for (int i = 0; i < extensions.Length; i++)
            {
                if (!extensions[i].StartsWith("."))
                    extensions[i] = "." + extensions[i];
            }
            return extensions;
        }
        public static string BiggestNumberBundleFile(string dirPath)//TODO: send full path,valid path
        {
            string pattern = @"bundle(\d*)\.txt";
            //get files with bundle[number].txt
            IEnumerable<string> existingFiles = Directory.EnumerateFiles(dirPath).Select(file => file.Substring(dirPath.Length + 1)).Where(fileName => Regex.IsMatch(fileName, pattern)).ToList();


            if (existingFiles.Count() == 0)
                return dirPath + @"\bundle.txt";
            if (existingFiles.Count() == 1 && existingFiles.Any(fileName => fileName.Equals("bundle.txt")))
                return dirPath + @"\bundle1.txt";

            // Filter strings based on the regex pattern and extract the numbers
            var matches = existingFiles
                .Select(s => !s.Equals("bundle.txt") ? int.Parse(s.Substring("bundle".Length).Substring(0, s.Substring("bundle".Length).Length - 4)) : 0);

            // Get the maximum number
            int? maxNumber = matches.Any() ? matches.Max() : (int?)null;

            return @$"{dirPath}\bundle{maxNumber + 1}.txt";
        }

        public static IEnumerable<FileInfo> SortFiles(IEnumerable<FileInfo> files,string by)
        {
            if (by.Equals("name"))
            {
                return files.OrderBy(file=>file.Name);
            }

            return files.OrderBy(file => file.Extension).ThenBy(file=>file.Name);
        }

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            // Print the error message to the screen
            Console.WriteLine("An error occurred: " + message);

            // Reset the console text color to the default color
            Console.ResetColor();
        }
        public static string Contents(IEnumerable<FileInfo> files)
        {
            var fileCounts = files.GroupBy(file =>file.Extension)
                           .Select(group => new { Extension = group.Key, Count = group.Count() });
            string contents = "";
            foreach (var fileCount in fileCounts)
            {
                contents+=$"    {fileCount.Count} {fileCount.Extension} files{Environment.NewLine}";
            }
            return contents+"\n";
        }
    }
}
