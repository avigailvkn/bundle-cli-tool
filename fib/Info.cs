using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fib
{
    internal class Info
    {
        public static string[] excludedDirectories = new string[]
        {
        "debug",
        "bin",
        "obj",
        "node_modules",
        "dist",
        "vendor",
        "build",
        "temp",
        "logs",
        "coverage",
        ".vscode",
        ".idea",
        ".settings",
        ".git",
        ".vs"
        };

        public static string[] allowedExtensions = new string[]
        {
           "all", "txt", "json", "html", "md", "cs", "java", "py", "cpp", "h", "js", "css", "docx", "c", "rsp",
           ".txt", ".json", ".html", ".md", ".cs", ".java", ".py", ".cpp", ".h", ".js", ".css", ".docx", ".c",".rsp"
        };
        public static string[] possibleExtensions = new string[]
        {
            ".txt", ".json", ".html", ".md", ".cs", ".java", ".py", ".cpp", ".h", ".js", ".css", ".docx", ".c",".rsp"
        };
    }
}
