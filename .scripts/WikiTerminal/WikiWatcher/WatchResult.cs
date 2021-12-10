using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiWatcher
{
    public class WatchResult
    {
        /// <summary>
        /// The result of the watch operation
        /// 0: No change<br />
        /// 1: Successful change<br />
        /// 2: User intervention required
        /// </summary>
        public int result;

        /// <summary>
        /// The contents of the page. Empty if result is not 2.
        /// </summary>
        public string content;

        /// <summary>
        /// The revision id of the page. 0 if result is not 2.
        /// </summary>
        public int new_version;

        /// <summary>
        /// The SHA256 hash of content. Empty if result is not 2.
        /// </summary>
        public string new_hash;

        /// <summary>
        /// How this version conflict will be resolved.
        /// 0: Pending response
        /// 1: Overwrite local version
        /// 2: Ignore until next version
        /// </summary>
        public int resolve_action;
    }
}
