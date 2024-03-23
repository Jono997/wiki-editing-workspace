using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiTerminal
{
    [Serializable]
    class ConsoleState
    {
        public string lastCurrentDirectory;
        public string[] commandHistory;

        public ConsoleState()
        {
            lastCurrentDirectory = Program.dirs[Program.Dir.Root];
            commandHistory = new string[0];
        }
    }
}
