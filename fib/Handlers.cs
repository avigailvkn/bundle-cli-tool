using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fib
{
    internal class Handlers
    {
        public static void HandleBundle(string[] extensionOption, FileInfo outputFileOption, bool noteOption, string sortOption, bool removeEmptyLinesOption, bool recursiveOption, string authorOption, DirectoryInfo inputDirectoryOption)
        {
            extensionOption = Methods.Complete(extensionOption);
            //Console.Write("extensionOption: ");
            //extensionOption.ToList<string>().ForEach(exten => Console.Write(exten + " "));
            //Console.WriteLine();
            //Console.WriteLine($"outputFileOption: {outputFileOption}");
            //Console.WriteLine($"noteOption: {noteOption}");
            //Console.WriteLine($"sortOption: {sortOption}");
            //Console.WriteLine($"removeEmptyLinesOption: {removeEmptyLinesOption}");
            //Console.WriteLine($"recursiveOption: {recursiveOption}");
            //Console.WriteLine($"authorOption: {authorOption}");
            //Console.WriteLine($"inputDirectoryOption: {inputDirectoryOption}");

            IEnumerable<FileInfo> files = Methods.ReadFiles(inputDirectoryOption.FullName, recursiveOption, extensionOption, Info.excludedDirectories);
            if (files == null)
                return;

            int filesCount = files.Count();
            if (filesCount == 0)
            {
                Methods.PrintError($"no files with the following extensions: {string.Join(", ", extensionOption)}\nwere found in: {inputDirectoryOption.FullName}");
                return;
            }
            files = Methods.SortFiles(files, sortOption);

            try
            {
                using (StreamWriter sw = outputFileOption.CreateText())
                {
                    if (authorOption != null)
                    {
                        sw.WriteLine("Author: " + authorOption);
                    }

                    sw.WriteLine($"Contents: {filesCount} files");
                    sw.WriteLine(Methods.Contents(files));

                    foreach (FileInfo file in files)
                    {
                        if (file.FullName.Equals(outputFileOption.FullName))
                        {
                            Methods.PrintError("cant read from "+file.FullName);
                            continue;
                        }

                        sw.WriteLine("File Name: " + file.Name);
                        if (noteOption)
                        {
                            sw.WriteLine("Relative Path: " + file.FullName.Substring(inputDirectoryOption.FullName.Length + 1));
                        }
                        if (File.ReadAllText(file.FullName).Length == 0)
                            sw.WriteLine("(Empty File)");
                        else
                        {
                            sw.WriteLine("......");
                            foreach (string line in File.ReadAllLines(file.FullName))
                            {
                                if (!removeEmptyLinesOption || !line.Equals(""))
                                {
                                    sw.WriteLine("  " + line);
                                }
                            }
                            sw.WriteLine("......");
                        }

                        sw.WriteLine(Environment.NewLine);
                    }

                }
                Console.WriteLine($"The file {outputFileOption.Name} was created successfully");
                Console.WriteLine($"{filesCount} files bundled inside");
            }
            catch (DirectoryNotFoundException ex)
            {
                Methods.PrintError(ex.Message);
            }
            catch (Exception ex)
            {
                Methods.PrintError(ex.Message);
            }
        }
        public static void HandleCreateRsp()
        {
            var options = new
            {
                Author = "--author",
                Extensions = "--extensions",
                OutputFile = "--output-file",
                Note = "--note",
                Sort = "--sort",
                RemoveEmptyLines = "--remove-empty-lines",
                Recursive = "--recursive",
                InputDirectory = "--input-dir",
            };
            while (true)
            {
                Console.WriteLine("Creating the .rsp file, LETS BEGIN! ^_^");
                Console.WriteLine("type parameters for each of the following options\nyou can skip [OPTIONAL] and the parameter will get default value\n");
                Dictionary<string, string> map = new Dictionary<string, string>();
                Console.WriteLine("option --input-dir:[OPTIONAL] input directory path, default is current directory");
                map[options.InputDirectory] = Console.ReadLine();
                Console.WriteLine("option --output-file:[OPTIONAL] output file path, default is \"bundle.txt\"");
                map[options.OutputFile] = Console.ReadLine();
                bool validInput = false;
                while (!validInput)
                {
                    validInput = true;
                    Console.WriteLine("option --extensions:[REQUIRED] file extensions you want to bundle\ncan be one or more from these:");
                    Console.WriteLine(string.Join("|", Info.allowedExtensions));
                    Console.WriteLine("all means all the extensions from the list");
                    map[options.Extensions] = Console.ReadLine();
                    string[] words = map[options.Extensions].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length == 0)
                    {
                        Methods.PrintError("the --extensions option is [REQIURED]");
                        validInput = false;
                        continue;
                    }
                    foreach (string word in words)
                    {
                        if (!Info.allowedExtensions.Contains(word))
                        {
                            Methods.PrintError("a not allowed file extension found in the parameter you gave: " + word);
                            validInput = false;
                            break;
                        }
                    }
                }
                Console.WriteLine("option --author:[OPTIONAL] write author name in the bundle file, default is without author name");
                map[options.Author] = Console.ReadLine();
                Console.WriteLine("option --note:[y/n] note the source for each bundled file? y/n");
                map[options.Note] = Console.ReadLine();
                while (true)
                {
                    Console.WriteLine("option --sort:[OPTIONAL] sort the bundled files by name/extension, default is name");
                    map[options.Sort] = Console.ReadLine();
                    if (!map[options.Sort].Equals("name") && !map[options.Sort].Equals("extension") && !map[options.Sort].Trim().Equals(""))
                    {
                        Methods.PrintError($"invalid argument for sort option:{map[options.Note]}");
                        continue;
                    }
                    break;
                }
                Console.WriteLine("option --recursive:[y/n] search for files recursively in sub directories of the input derectory? y/n");
                map[options.Recursive] = Console.ReadLine();
                Console.WriteLine("option --remove-empty-lines:[y/n] remove empty lines from bundled files? y/n");
                map[options.RemoveEmptyLines] = Console.ReadLine();
                string bundleRsp = "#command syntax and parameters for fib bundle\n#usage: fib @bundle.rsp\nbundle";
                foreach (KeyValuePair<string, string> keyValue in map)
                {
                    if (keyValue.Key.Equals(options.InputDirectory))
                    {
                        if (keyValue.Value.Trim().Equals(""))
                        {
                            bundleRsp += "\n" + keyValue.Key + " ";
                            bundleRsp += $"\"{Directory.GetCurrentDirectory()}\"";
                        }
                    }
                    if (keyValue.Key.Equals(options.Note) || keyValue.Key.Equals(options.Recursive) || keyValue.Key.Equals(options.RemoveEmptyLines))//bool option
                    {
                        bundleRsp += "\n" + keyValue.Key + " ";
                        if (keyValue.Value.Equals("y"))
                            bundleRsp += "true";
                        else
                            bundleRsp += "false";
                    }
                    else if (!keyValue.Value.Trim().Equals(""))
                    {
                        bundleRsp += "\n" + keyValue.Key + " " + keyValue.Value;
                    }
                }
                Console.WriteLine("a bundle.rsp file will be created in the current directory, overriding any existing files with the same name");
                Console.WriteLine("the bindle.rsp file contents will be the following:\n");
                Console.WriteLine(bundleRsp);
                Console.WriteLine("do you want to change something? y/n");
                string answer = Console.ReadLine();
                if (answer.Equals("y"))
                {
                    Console.WriteLine("Ok, lets start over!\n");
                    continue;
                }
                else
                {
                    //create .rsp (override old) in current directory
                    try
                    {
                        // File.Create("bundle.rsp");
                        File.WriteAllText("bundle.rsp", bundleRsp);
                        Console.WriteLine("bundle.rsp file was created successfully!\nrun: fib @bundle.rsp");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Methods.PrintError(ex.Message);
                    }
                }
            }
        }
    }
}
