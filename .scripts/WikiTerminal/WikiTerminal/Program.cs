using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using ATEM;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;

namespace WikiTerminal
{
    partial class Program
    {
        internal enum Dir
        {
            Root,
            Scripts,
            Pagedata,
            Preprocessors
        }

        internal static Dictionary<Dir, string> dirs;

        /// <summary>
        /// The directory that is currently in focus (Environment.CurrentDirectory is not used for this because it's a lot easier to keep that on one specific folder)
        /// </summary>
        private static string currentDir;

        internal static ConsoleState consoleState;

        private static Regex commandRegex = new Regex(@"(?:""(.*?)""|([^ ]+))");

        static void Main(string[] args)
        {
            #region Initialise dirs
            dirs = new Dictionary<Dir, string>();
            {
                if (Debugger.IsAttached)
                    dirs.Add(Dir.Scripts, Environment.CurrentDirectory);
                else
                {
                    dirs.Add(Dir.Scripts, Path.Combine(Environment.GetCommandLineArgs()[0], ".."));
                    Environment.CurrentDirectory = dirs[Dir.Scripts];
                }
            }
            dirs.Add(Dir.Root, Path.Combine(dirs[Dir.Scripts], ".."));
            dirs.Add(Dir.Pagedata, Path.Combine(dirs[Dir.Scripts], "pagedata"));
            dirs.Add(Dir.Preprocessors, Path.Combine(dirs[Dir.Scripts], "preprocessors"));
            foreach (Dir d in Enum.GetValues(typeof(Dir)))
            {
                dirs[d] = Path.GetFullPath(dirs[d]);
                //Console.WriteLine($"{d}: {dirs[d]}");
            }
            #endregion

            Console.CancelKeyPress += Console_CancelKeyPress;
            string consolestate_path = Path.Combine(dirs[Dir.Scripts], "consolestate.bin");
            if (File.Exists(consolestate_path))
            {
                FileStream file = new FileStream(consolestate_path, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                consoleState = (ConsoleState)formatter.Deserialize(file);
                file.Close();
            }
            else
                consoleState = new ConsoleState();
            currentDir = consoleState.lastCurrentDirectory;

            #region Run WikiWatcher
            if (Process.GetProcessesByName("WikiWatcher").Length == 0)
                Process.Start("WikiWatcher.exe");
            #endregion

            while (TerminalInstance())
                continue;
        }

        private static bool TerminalInstance()
        {
            if (currentDir != dirs[Dir.Root])
                Console.Write($"{currentDir.Substring(dirs[Dir.Root].Length + 1)} ");
            Console.Write(">>> ");
            string command = Console.ReadLine();
            string[] command_split;
            int[,] command_index;

            #region Parse command
            {
                MatchCollection mc = commandRegex.Matches(command);
                command_split = new string[mc.Count];
                command_index = new int[mc.Count,2];
                for (int i = 0; i < mc.Count; i++)
                {
                    Match m = mc[i];
                    command_split[i] = m.Groups[m.Groups[1].Value == "" ? 2 : 1].Value;
                    command_index[i, 0] = m.Index;
                    command_index[i, 1] = m.Index + m.Value.Length;
                    //Console.WriteLine($"[{command.Substring(command_index[i, 0], m.Value.Length)}]");
                }
            }
            #endregion
            
            if (command_split.Length > 0)
            {
                #region Old code
                //switch (command_split[0].ToLower())
                //{
                //    case "exit":
                //        return false;
                //    case "cls":
                //        Console.Clear();
                //        break;
                //    case "dir":
                //        {
                //            string search_dir = currentDir;
                //            if (command_index.Length > 2)
                //                search_dir = PathFromArguments(command, command_split, command_index[1, 0]);
                //            foreach (string dir in Directory.GetDirectories(search_dir))
                //                if (dir != dirs[Dir.Scripts])
                //                    Console.WriteLine($"D: {Path.GetFileName(dir)}");
                //            foreach (string file in Directory.GetFiles(currentDir))
                //                Console.WriteLine($"F: {Path.GetFileName(file)}");
                //            break;
                //        }
                //    case "cd":
                //        {
                //            string dir_name = PathFromArguments(command, command_split, command_index[1, 0]);
                //            if (dir_name == null)
                //                break;
                //            if (!dir_name.Contains(dirs[Dir.Root]))
                //                currentDir = dirs[Dir.Root];
                //            else
                //                ChangeDirectory(dir_name);
                //            break;
                //        }
                //    case "mkdir":
                //        {
                //            string dir_name = PathFromArguments(command, command_split, command_index[1, 0]);
                //            if (dir_name == null)
                //                break;
                //            if (Directory.Exists(dir_name))
                //            {
                //                Console.Error.WriteLine($"\"{dir_name}\" ALREADY EXISTS");
                //                break;
                //            }
                //            try
                //            {
                //                Directory.CreateDirectory(dir_name);
                //            }
                //            catch (Exception e)
                //            {
                //                Console.Error.WriteLine(e.StackTrace);
                //                Console.Error.WriteLine(e.Message);
                //            }
                //            break;
                //        }
                //    case "rmdir":
                //        {
                //            string dir_name = PathFromArguments(command, command_split, command_index[1, 0]);
                //            if (dir_name == null)
                //                break;
                //            if (!Directory.Exists(dir_name))
                //            {
                //                Console.Error.WriteLine($"\"{dir_name}\" DOESN'T EXIST");
                //                break;
                //            }
                //            if (!DirectoryIsEmpty(dir_name))
                //            {
                //                Console.Write($"WARNING: \"{dir_name}\" is not empty and contains files. Are you sure you want to delete them? (Y/N): ");
                //                if (Console.ReadLine().ToUpper() != "Y")
                //                    break;
                //            }
                //            try
                //            {
                //                Directory.Delete(dir_name, true);
                //            }
                //            catch (Exception e)
                //            {
                //                Console.Error.WriteLine(e.StackTrace);
                //                Console.Error.WriteLine(e.Message);
                //            }
                //            break;
                //        }
                //    case "get":
                //        {
                //            string file_name = PathFromArguments(command, command_split, command_index[1, 0]);
                //            if (file_name == null)
                //                break;
                //            if (Directory.Exists(file_name))
                //            {
                //                Console.Error.WriteLine($"\"{file_name}\" MUST NOT BE A DIRECTORY");
                //            }
                //            if (File.Exists(file_name))
                //            {
                //                RunPython("getpage.py", "update", file_name);
                //                break;
                //            }
                //            if (command_split.Length == 2)
                //            {
                //                RunPython("getpage.py", "setup", file_name);
                //                break;
                //            }
                //            if (command_split.Length < 4)
                //            {
                //                Console.Error.WriteLine("INSUFFICIENT NUMBER OF ARGUMENTS");
                //                break;
                //            }
                //            RunPython("getpage.py", command_split);
                //            break;
                //        }
                //    case "del":
                //        {
                //            string file_name = PathFromArguments(command, command_split, command_index[1, 0]);
                //            if (file_name == null)
                //                break;
                //            if (!File.Exists(file_name))
                //            {
                //                Console.Error.WriteLine($"\"{file_name}\" DOESN'T EXIST");
                //                break;
                //            }
                //            try
                //            {
                //                DeleteFile(file_name);
                //            }
                //            catch (Exception e)
                //            {
                //                Console.Error.WriteLine(e.StackTrace);
                //                Console.Error.WriteLine(e.Message);
                //            }
                //            break;
                //        }
                //}
                #endregion
                foreach (ConsoleCommand cc in commands)
                    foreach (string alias in cc.aliases)
                        if (command_split[0] == alias)
                            return cc.Run(command, command_split, command_index);
            }
            return true;
        }

        private static string PathFromArguments(string command, string[] command_split, int command_stitch_index)
        {
            if (command_split.Length < 2)
            {
                Console.Error.WriteLine("INSUFFICIENT NUMBER OF ARGUMENTS");
                return null;
            }
            string retVal = command.Substring(command_stitch_index);
            retVal = retVal.Replace("\"", "");
            retVal = Path.Combine(currentDir, retVal);
            retVal = Path.GetFullPath(retVal);
            if (retVal.Contains(dirs[Dir.Root]))
                return retVal;
            return dirs[Dir.Root];
        }

        private static bool DirectoryIsEmpty(string dir)
        {
            if (Directory.GetFiles(dir).Length > 0)
                return false;
            foreach (string subdir in Directory.GetDirectories(dir))
                if (!DirectoryIsEmpty(Path.Combine(dir, subdir)))
                    return false;
            return true;
        }

        private static void ChangeDirectory(string dir)
        {
            if (Directory.Exists(dir))
                currentDir = dir;
            else
                ChangeDirectory(dir.Substring(0, dir.Length - Path.GetFileName(dir).Length - 1));
        }

        private static string PythonFriendlyPath(string path)
        {
            string p = path;
            if (!p.Contains(dirs[Dir.Root]))
                p = Path.GetFullPath(path);
            return p.Substring(dirs[Dir.Root].Length + 1);
        }

        private static void RunPython(string script, params string[] args)
        {
            string args_str = $"\"{script}\" \"{args.Stitch("\" \"")}\"";

            Process proc = new Process()
            {
                StartInfo = new ProcessStartInfo("python", args_str)
                {
                    UseShellExecute = false
                }
            };
            proc.Start();
            proc.WaitForExit();
        }

        private static Process RunCode(string path)
        {
            Process proc = new Process()
            {
                StartInfo = new ProcessStartInfo("code", $"\"{path}\"")
            };
            proc.Start();
            return proc;
        }

        private static void SaveConsoleState()
        {
            FileStream file = new FileStream(Path.Combine(dirs[Dir.Scripts], "consolestate.bin"), FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(file, consoleState);
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            
        }
    }
}
