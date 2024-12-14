using fib;
using System.CommandLine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

Option<string[]> fileExtensionsOption = new Option<string[]>(
    name: "--extensions",
    description: "list of extensions of the files to bundle")
{ IsRequired = true, AllowMultipleArgumentsPerToken = true }
    .FromAmong(Info.allowedExtensions);
fileExtensionsOption.AddAlias("-e");

Option<FileInfo> outputFilePathOption = new Option<FileInfo>(
    name: "--output-file",
    description: "name of output file",
        isDefault: true,
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0)
                {
                    return new FileInfo(Methods.GetLargestNumberedBundleFile(Directory.GetCurrentDirectory()));
                }
                string? filePath = result.Tokens.Single().Value;
                if (File.Exists(filePath))
                {
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
outputFilePathOption.AddAlias("-o");

Option<DirectoryInfo> sourceDirectoryOption = new Option<DirectoryInfo>(
    name: "--input-dir",
    description: "name of input directory",
    isDefault: true,
            parseArgument: result =>
            {
                if (result.Tokens.Count == 0)
                {
                    return new DirectoryInfo(Directory.GetCurrentDirectory());
                }
                string? dirPath = result.Tokens.Single().Value;
                if (!Directory.Exists(dirPath))
                {
                    result.ErrorMessage = "Input Directory does not exist";
                    return null;
                }
                else
                {
                    return new DirectoryInfo(dirPath);
                }
            });
sourceDirectoryOption.AddAlias("-i");

Option<bool> includeNoteOption = new Option<bool>(
    name: "--note",
    description: "include a comment with code source for each file");
includeNoteOption.AddAlias("-n");

Option<string> fileSortingOption = new Option<string>(
    name: "--sort",
    description: "sort the files, default by file-names",
    getDefaultValue: () => "name"
    ).FromAmong("extension", "name");
fileSortingOption.AddAlias("-s");

Option<bool> stripEmptyLinesOption = new Option<bool>(
    name: "--remove-empty-lines",
    description: "remove empty lines from source files");
stripEmptyLinesOption.AddAlias("-rel");

Option<bool> recurseOption = new Option<bool>(
    name: "--recursive",
    description: "do the action recursively");
recurseOption.AddAlias("-rec");

Option<string> contributorNameOption = new Option<string>(
    name: "--author",
    description: "include author name");
contributorNameOption.AddAlias("-a");

RootCommand mainCommand = new RootCommand("root command description");

Command createBundleCommand = new Command(
    name: "bundle",
    description: $"bundle source code files into one file\nfiles in the following sub directories wont be included:\n{string.Join("|", Info.excludedDirectories)}")
    { fileExtensionsOption, outputFilePathOption, includeNoteOption, fileSortingOption, stripEmptyLinesOption, recurseOption, contributorNameOption, sourceDirectoryOption };

Command generateRspFileCommand = new Command(
    name: "create-rsp",
    description: "create .rsp file ");

generateRspFileCommand.SetHandler(() =>
{
    Handlers.ProcessCreateRsp();
});

createBundleCommand.SetHandler((fileExtensionsOption, outputFilePathOption, includeNoteOption, fileSortingOption, stripEmptyLinesOption, recurseOption, contributorNameOption, sourceDirectoryOption) =>
{
    Handlers.ProcessBundle(fileExtensionsOption, outputFilePathOption, includeNoteOption, fileSortingOption, stripEmptyLinesOption, recurseOption, contributorNameOption, sourceDirectoryOption);
},
   fileExtensionsOption, outputFilePathOption, includeNoteOption, fileSortingOption, stripEmptyLinesOption, recurseOption, contributorNameOption, sourceDirectoryOption);

mainCommand.AddCommand(createBundleCommand);
mainCommand.AddCommand(generateRspFileCommand);

return await mainCommand.InvokeAsync(args);
