using fib;
using System.CommandLine;
using System.Diagnostics.Tracing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

Option<string[]> extensionOption = new Option<string[]>(
    name: "--extensions",
    description: "list of extensions of the files to bundle")
{ IsRequired = true, AllowMultipleArgumentsPerToken = true }
    .FromAmong(Info.allowedExtensions);
extensionOption.AddAlias("-e");

Option<FileInfo> outputFileOption = new Option<FileInfo>(
    name: "--output-file",
    description: "name of output file",
        isDefault: true,
            parseArgument: result =>//result->FileInfo(result.Tokens.Single().Value)
            {
                if (result.Tokens.Count == 0)//no -o option was chosen
                {//what if bundle exists?                    
                    return new FileInfo(Methods.BiggestNumberBundleFile(Directory.GetCurrentDirectory()));
                }
                string? filePath = result.Tokens.Single().Value;
                if (File.Exists(filePath))
                {
                    //this does an error message
                    result.ErrorMessage = "Output file exists already, give another path for output file";
                    return null;
                }
                if (!filePath.EndsWith(".txt"))
                {
                    result.ErrorMessage = "The output file is expected to be .txt file";
                    return null;
                }
                else
                {
                    return new FileInfo(filePath);
                }
            });
outputFileOption.AddAlias("-o");

Option<DirectoryInfo> inputDirectoryOption = new Option<DirectoryInfo>(
    name: "--input-dir",
    description: "name of input directory",
    isDefault: true,
            parseArgument: result =>//result->FileInfo(result.Tokens.Single().Value)
            {
                if (result.Tokens.Count == 0)//no -i option was chosen
                {
                    return new DirectoryInfo(Directory.GetCurrentDirectory());

                }
                string? dirPath = result.Tokens.Single().Value;
                if (!Directory.Exists(dirPath))
                {
                    //this does an error message
                    result.ErrorMessage = "Input Directory does not exist";
                    return null;
                }
                else
                {
                    return new DirectoryInfo(dirPath);
                }
            });
inputDirectoryOption.AddAlias("-i");

Option<bool> noteOption = new Option<bool>(
    name: "--note",
    description: "include a comment with code source for each file");
noteOption.AddAlias("-n");

Option<string> sortOption = new Option<string>(
    name: "--sort",
    description: "sort the files, default by file-names",
    getDefaultValue: () => "name"
    ).FromAmong("extension", "name");
sortOption.AddAlias("-s");

Option<bool> removeEmptyLinesOption = new Option<bool>(
    name: "--remove-empty-lines",
    description: "remove empty lines from source files");
removeEmptyLinesOption.AddAlias("-rel");

Option<bool> recursiveOption = new Option<bool>(
    name: "--recursive",
    description: "do the action recursively");
recursiveOption.AddAlias("-rec");

Option<string> authorOption = new Option<string>(
    name: "--author",
    description: "include author name");
authorOption.AddAlias("-a");

RootCommand rootCommand = new RootCommand("root command description");

Command bundleCommand = new Command(
    name: "bundle",
    description: $"bundle source code files into one file\nfiles in the following sub directories wont be included:\n{string.Join("|", Info.excludedDirectories)}")
    {extensionOption, outputFileOption, noteOption, sortOption,removeEmptyLinesOption,recursiveOption,authorOption,inputDirectoryOption};

Command createRspCommand = new Command(
    name: "create-rsp",
    description: "create .rsp file ");

createRspCommand.SetHandler(() =>
{
    Handlers.HandleCreateRsp();
});
//TODO: make file tasks async
bundleCommand.SetHandler((extensionOption, outputFileOption, noteOption, sortOption, removeEmptyLinesOption, recursiveOption, authorOption, inputDirectoryOption) =>
{
    Handlers.HandleBundle(extensionOption, outputFileOption, noteOption, sortOption, removeEmptyLinesOption, recursiveOption, authorOption, inputDirectoryOption);
},
   extensionOption, outputFileOption, noteOption, sortOption, removeEmptyLinesOption, recursiveOption, authorOption, inputDirectoryOption);

rootCommand.AddCommand(bundleCommand);
rootCommand.AddCommand(createRspCommand);

return await rootCommand.InvokeAsync(args);



