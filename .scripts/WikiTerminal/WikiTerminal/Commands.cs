using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WikiTerminal
{
    partial class Program
    {
        private static ConsoleCommand[] commands =
        {
            #region Nagivation
            new ConsoleCommand("Help", new string[] { "help" }, delegate()
            {
                Console.WriteLine("cls\t\t\t\t\tClears the output");
                Console.WriteLine("ls <directory>\t\t\t\tShows the files and directories in the directory provided. If none is provided, the current directory is used instead.");
                Console.WriteLine("cd [directory]\t\t\t\tChanges the current directory to the directory provided.");
                Console.WriteLine("mkdir [directory]\t\t\tCreates the directory provided.");
                Console.WriteLine("mv [file] [new_location]\t\tMoves the file provided to the new location provided.");
                Console.WriteLine("mvdir [directory] [new_location]\tSame as mv, but for directories.");
                Console.WriteLine("rm [file]\t\t\t\tDeletes the file provided. WARNING: Files deleted are not sent to the recycle bin, they are deleted permanently from the computer.");
                Console.WriteLine("rmdir [directory]\t\t\tSame as rm, but for directories.");
                Console.WriteLine("login\t\t\t\t\tStarts the wiki login setup process.");
                Console.WriteLine("logout\t\t\t\t\tLogs out of your wiki account.");
                Console.WriteLine("get [file] [wiki] [page]\t\tDownloads the page provided from the wiki provided and save it to the file provided.");
                Console.WriteLine("get [file]\t\t\t\tStarts the advanced page download process, with the file provided if it doesn't already exist. Replaces the file provided's contents with the most recent version from the wiki if it does exist.");
                Console.WriteLine("set [file] <summary>\t\t\tEdits the wiki page the file provided was downloaded from to the contents of that file, with the edit summary provided. If no edit summary is provided, the summary will be blank.");
                Console.WriteLine("view [file]\t\t\t\tOpens the file provided's origin in a browser.");
                Console.WriteLine("lspre\t\t\t\t\tLists all the page preprocessors on this machine.");
                Console.WriteLine("mkpre [preprocessor]\t\t\tCreates the preprocessor provided.");
                Console.WriteLine("pre [preprocessor]\t\t\tOpens the preprocessor provided is Visual Studio Code.");
                Console.WriteLine("rmpre [preprocessor]\t\t\tDeletes the preprocessor provided.");
                Console.WriteLine("help\t\t\t\t\tPrints this message");
                Console.WriteLine("exit\t\t\t\t\tCloses WikiTerminal");
                return true;
            }),
            new ConsoleCommand("Exit", new string[] { "exit", "quit", "ex", "q" }, delegate()
            {
                return false;
            }),
            new ConsoleCommand("Clear", new string[] { "cls", "clear" }, delegate()
            {
                Console.Clear();
                return true;
            }),
            new ConsoleCommand("DirectoryContents", new string[] { "dir", "ls" }, delegate(string command, string[] command_split, int[,] command_index)
            {
                string search_dir = currentDir;
                if (command_index.Length > 2)
                    search_dir = PathFromArguments(command, command_split, command_index[1, 0]);
                foreach (string dir in Directory.GetDirectories(search_dir))
                    if (Path.GetFileName(dir)[0] != '.')
                        Console.WriteLine($"D: {Path.GetFileName(dir)}");
                foreach (string file in Directory.GetFiles(search_dir))
                    Console.WriteLine($"F: {Path.GetFileName(file)}");
                return true;
            }),
            new ConsoleCommand("ChangeDirectory", new string[] { "cd" }, delegate(string command, string[] command_split, int[,] command_index)
            {
                 string dir_name = PathFromArguments(command, command_split, command_index[1, 0]);
                 if (dir_name == null)
                     return true;
                 if (!dir_name.Contains(dirs[Dir.Root]))
                     currentDir = dirs[Dir.Root];
                 else
                     ChangeDirectory(dir_name);
                 return true;
            }),
            new ConsoleCommand("MoveUpDir", new string[] { "cd.." }, delegate()
            {
                string up_dir = Path.GetFullPath(Path.Combine(currentDir, ".."));
                if (up_dir.Contains(dirs[Dir.Root]))
                    currentDir = up_dir;
                return true;
            }),
            #endregion
            #region File/Directory manipulation
            new ConsoleCommand("MakeDirectory", new string[] { "mkdir", "makedir" }, delegate(string command, string[] command_split, int[,] command_index)
            {
                string dir_name = PathFromArguments(command, command_split, command_index[1, 0]);
                if (dir_name == null)
                    return true;
                if (Directory.Exists(dir_name))
                {
                    Console.Error.WriteLine($"\"{dir_name}\" ALREADY EXISTS");
                    return true;
                }
                Directory.CreateDirectory(dir_name);
                string pagedata_path = Path.Combine(dirs[Dir.Pagedata], dir_name.Substring(dirs[Dir.Root].Length + 1));
                if (!Directory.Exists(pagedata_path))
                    Directory.CreateDirectory(pagedata_path);
                return true;
            }),
            new ConsoleCommand("MoveFile", new string[] { "mv", "move", "rename", "rn" }, delegate(string command, string[] command_split, int[,] command_index)
            {
                if (command_split.Length < 3)
                {
                    Console.Error.WriteLine("INSUFFICIENT NUMBER OF ARGUMENTS");
                    return true;
                }
                string old_name = Path.GetFullPath(Path.Combine(currentDir, command_split[1]));
                string new_name = PathFromArguments(command, command_split, command_index[2, 0]);
                if (!File.Exists(old_name))
                {
                    Console.Error.WriteLine($"\"{old_name}\" DOESN'T EXIST");
                    return true;
                }
                if (File.Exists(new_name))
                {
                    Console.Error.WriteLine($"\"{new_name}\" ALREADY EXISTS");
                    return true;
                }
                File.Move(old_name, new_name);
                string old_pagedata = Path.Combine(dirs[Dir.Pagedata], old_name.Substring(dirs[Dir.Root].Length + 1)) + ".json";
                if (File.Exists(old_pagedata))
                    File.Move(old_pagedata, Path.Combine(dirs[Dir.Pagedata], new_name.Substring(dirs[Dir.Root].Length + 1)) + ".json");
                return true;
            }),
            new ConsoleCommand("MoveDirectory", new string[] { "mvdir", "movedir", "renamedir", "rndir" }, delegate(string command, string[] command_split, int[,] command_index)
            {
                if (command_split.Length < 3)
                {
                    Console.Error.WriteLine("INSUFFICIENT NUMBER OF ARGUMENTS");
                    return true;
                }
                string old_name = Path.GetFullPath(Path.Combine(currentDir, command_split[1]));
                string new_name = PathFromArguments(command, command_split, command_index[2, 0]);
                if (!File.Exists(old_name))
                {
                    Console.Error.WriteLine($"\"{old_name}\" DOESN'T EXIST");
                    return true;
                }
                if (File.Exists(new_name))
                {
                    Console.Error.WriteLine($"\"{new_name}\" ALREADY EXISTS");
                    return true;
                }
                Directory.Move(old_name, new_name);
                string old_pagedata = Path.Combine(dirs[Dir.Pagedata], old_name.Substring(dirs[Dir.Root].Length + 1));
                if (Directory.Exists(old_pagedata))
                    Directory.Move(old_pagedata, Path.Combine(dirs[Dir.Pagedata], new_name.Substring(dirs[Dir.Root].Length + 1)));
                return true;
            }),
            new ConsoleCommand("RemoveFile", new string[] { "del", "rm", "delete", "remove" }, delegate(string command, string[] command_split, int[,] command_index)
            {
                string file_name = PathFromArguments(command, command_split, command_index[1, 0]);
                if (file_name == null)
                    return true;
                if (File.Exists(file_name))
                {
                    string pagedata_path = Path.Combine(dirs[Dir.Pagedata], file_name.Substring(dirs[Dir.Root].Length + 1)) + ".json";
                    File.Delete(file_name);
                    if (File.Exists(pagedata_path))
                        File.Delete(pagedata_path);
                }
                else
                    Console.Error.WriteLine($"\"{file_name}\" DOESN'T EXIST");
                return true;
            }),
            new ConsoleCommand("RemoveDirectory", new string[] { "rmdir", "removedir", "deldir", "deletedir" }, delegate(string command, string[] command_split, int[,] command_index)
            {
                string dir_name = PathFromArguments(command, command_split, command_index[1, 0]);
                if (dir_name == null)
                    return true;
                if (!Directory.Exists(dir_name))
                {
                    Console.Error.WriteLine($"\"{dir_name}\" DOESN'T EXIST");
                    return true;
                }
                if (!DirectoryIsEmpty(dir_name))
                {
                    Console.Write($"WARNING: \"{dir_name}\" is not empty and contains files. Are you sure you want to delete them? (Y/N): ");
                    if (Console.ReadLine().ToUpper() != "Y")
                        return true;
                }
                string pagedata_path = Path.Combine(dirs[Dir.Pagedata], dir_name.Substring(dirs[Dir.Root].Length + 1));
                Directory.Delete(dir_name, true);
                if (Directory.Exists(pagedata_path))
                    Directory.Delete(pagedata_path, true);
                return true;
            }),
            #endregion
            #region Wiki interaction
            new ConsoleCommand("Login", new string[] { "login" }, delegate()
            {
                RunPython("Login.py");
                return true;
            }),
            new ConsoleCommand("Logout", new string[] { "logout" }, delegate()
            {
                RunPython("Logout.py");
                return true;
            }),
            new ConsoleCommand("GetPage", new string[] { "get", "pull" }, delegate(string command, string[] command_split, int[,] command_index)
            {
                string file_name = PathFromArguments(command, command_split, command_index[1, 0]);
                if (file_name == null)
                    return true;
                if (Directory.Exists(file_name))
                {
                    Console.Error.WriteLine($"\"{file_name}\" MUST NOT BE A DIRECTORY");
                    return true;
                }
                string relative_file_name = file_name.Substring(dirs[Dir.Root].Length + 1);
                if (File.Exists(file_name))
                {
                    RunPython("getpage.py", "update", relative_file_name);
                    return true;
                }
                if (command_split.Length == 2)
                {
                    RunPython("getpage.py", "setup", relative_file_name);
                    return true;
                }
                if (command_split.Length < 4)
                {
                    Console.Error.WriteLine("INSUFFICIENT NUMBER OF ARGUMENTS");
                    return true;
                }
                RunPython("getpage.py", new string[] { "get", Path.GetFullPath(Path.Combine(currentDir, command_split[1])).Substring(dirs[Dir.Root].Length + 1), command_split[2], command_split[3] });
                return true;
            }),
            new ConsoleCommand("SetPage", new string[] { "set", "push" }, delegate(string command, string[] command_split, int[,] command_index)
            {
                if (command_split.Length < 2)
                {
                    Console.Error.WriteLine("INSUFFICIENT NUMBER OF ARGUMENTS");
                    return true;
                }
                string file_name = Path.GetFullPath(Path.Combine(currentDir, command_split[1]));
                if (!file_name.Contains(dirs[Dir.Root]))
                {
                    Console.Error.WriteLine($"\"{file_name}\" IS OUTSIDE OF THE WORKSPACE");
                    return true;
                }
                if (!File.Exists(file_name))
                {
                    Console.Error.WriteLine($"\"{file_name}\" DOESN'T EXIST");
                    return true;
                }
                file_name = file_name.Substring(dirs[Dir.Root].Length + 1);
                if (command_split.Length > 2)
                {
                    string edit_summary = command.Substring(command_index[2, 0]);
                    if (edit_summary[0] == '"' && edit_summary.Last() == '"')
                        edit_summary = edit_summary.Substring(1, edit_summary.Length - 2);
                    RunPython("setpage.py", file_name, edit_summary);
                }
                else
                    RunPython("setpage.py", file_name);
                return true;
            }),
            new ConsoleCommand("ViewPage", new string[] { "view" }, delegate(string command, string[] command_split, int[,] command_index)
            {
                string file_path = PathFromArguments(command, command_split, command_index[1, 0]);
                if (file_path == null)
                    return true;
                RunPython("viewOnWiki.py", file_path.Substring(dirs[Dir.Root].Length + 1));
                return true;
            }),
            #endregion
            #region Preprocessors
            new ConsoleCommand("ListPreprocessors", new string[] { "pres", "predir", "prels", "dirpre", "lspre" }, delegate() {
                foreach (string preprocessor in Directory.GetFiles(dirs[Dir.Preprocessors]))
                    Console.WriteLine(Path.GetFileName(preprocessor).Substring(0, Path.GetFileName(preprocessor).Length - 3));
                return true;
            }),
            new ConsoleCommand("MakePreprocessor", new string[] { "mkpre", "makepre" }, delegate(string command, string[] command_split, int[,] command_index)
            {
                if (command_split.Length < 2)
                {
                    Console.Error.WriteLine("INSUFFICIENT NUMBER OF ARGUMENTS");
                    return true;
                }
                string preprocessor_name = command.Substring(command_index[1, 0]).Replace("\"", "");
                string preprocessor_path = Path.GetFullPath(Path.Combine(dirs[Dir.Preprocessors], preprocessor_name)) + ".py";
                if (File.Exists(preprocessor_path))
                {
                    Console.Error.WriteLine($"PREPROCESSOR \"{preprocessor_name}\" ALREADY EXISTS");
                    return true;
                }
                File.WriteAllLines(preprocessor_path, new string[]
                {
                    "# The function to process content when downloaded from the wiki",
                    "def to_client(wikitext):",
                    "    return wikitext",
                    "",
                    "# The function to process content before uploading to the wiki",
                    "def to_wiki(wikitext):",
                    "    return wikitext"
                }, Encoding.UTF8);
                Console.WriteLine("Opening in Visual Studio Code...");
                RunCode(preprocessor_path);
                return true;
            }),
            new ConsoleCommand("EditPreprocessor", new string[] { "pre", "editpre" }, delegate(string command, string[] command_split, int[,] command_index)
            {
                if (command_split.Length < 2)
                {
                    Console.Error.WriteLine("INSUFFICIENT NUMBER OF ARGUMENTS");
                    return true;
                }
                string preprocessor_name = command.Substring(command_index[1, 0]).Replace("\"", "");
                string preprocessor_path = Path.GetFullPath(Path.Combine(dirs[Dir.Preprocessors], preprocessor_name)) + ".py";
                if (!File.Exists(preprocessor_path))
                {
                    Console.Error.WriteLine($"PREPROCESSOR \"{preprocessor_name}\" DOESN'T EXIST");
                    return true;
                }
                Console.WriteLine("Opening in Visual Studio Code...");
                RunCode(preprocessor_path);
                return true;
            }),
            new ConsoleCommand("DeletePreprocessor", new string[] { "delpre", "rmpre" }, delegate(string command, string[] command_split, int[,] command_index)
            {
                if (command_split.Length < 2)
                {
                    Console.Error.WriteLine("INSUFFICIENT NUMBER OF ARGUMENTS");
                    return true;
                }
                string preprocessor_name = Path.GetFullPath(Path.Combine(dirs[Dir.Preprocessors], command.Substring(command_index[1, 0]).Replace("\"", ""))) + ".py";
                if (File.Exists(preprocessor_name))
                    File.Delete(preprocessor_name);
                else
                    Console.Error.WriteLine($"\"{preprocessor_name}\" DOESN'T EXIST");
                return true;
            })
            #endregion
        };
    }
}
