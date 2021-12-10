using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiTerminal
{
    class ConsoleCommand
    {
        /// <summary>
        /// The name of the command
        /// </summary>
        public string name { get; private set; }

        /// <summary>
        /// The possible names to call the command
        /// </summary>
        public string[] aliases { get; private set; }

        /// <summary>
        /// The method the run when calling the command
        /// </summary>
        private Func<string, string[], int[,], bool> method;

        public ConsoleCommand(string name, string[] aliases, Func<string, string[], int[,], bool> method)
        {
            this.name = name;
            this.aliases = aliases;
            this.method = method;
        }

        public ConsoleCommand(string name, string[] aliases, Func<string[], bool> method)
            : this(name, aliases, delegate(string command, string[] command_split, int[,] command_index)
            {
                return method.Invoke(command_split);
            })
        { }

        public ConsoleCommand(string name, string[] aliases, Func<bool> method)
            : this(name, aliases, delegate(string[] command_split)
            {
                return method.Invoke();
            })
        { }

        public bool Run(string command, string[] command_split, int[,] command_index)
        {
            try
            {
                return method.Invoke(command, command_split, command_index);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Unhandled Exception: {e.GetType()}: {e.Message}");
                Console.Error.WriteLine(e.StackTrace);
                return true;
            }
        }
    }
}
