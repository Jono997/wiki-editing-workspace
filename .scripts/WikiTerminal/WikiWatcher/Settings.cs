using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiWatcher
{
    public class Settings
    {
        /// <summary>
        /// The amount of time to wait between scans (in seconds)
        /// </summary>
        public int scanInterval;

        /// <summary>
        /// If true, WikiWatcher will stay open after Visual Studio Code closes<br />
        /// If false, WikiWatcher will close after the first scan perfomed where Visual Studio Code isn't running
        /// </summary>
        public bool keepOpenWithoutVSCode;

        public Settings()
        {
            scanInterval = 30;
            keepOpenWithoutVSCode = true;
        }
    }
}
