using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WikiWatcher
{
    public partial class ConflictsForm : Form
    {
        private Dictionary<string, WatchResult> conflicts;
        private string[] conflictsKeys;

        public ConflictsForm(Dictionary<string, WatchResult> conflicts)
        {
            this.conflicts = conflicts;
            conflictsKeys = conflicts.Keys.ToArray();
            InitializeComponent();
            conflictsListBox.Visible = conflicts.Count > 0;
            foreach (string pagedata_path in conflictsKeys)
                conflictsListBox.Items.Add(pagedata_path.Substring(9, pagedata_path.Length - 14));
        }

        private void conflictsListBox_DoubleClick(object sender, EventArgs e)
        {
            if (conflictsListBox.SelectedIndex >= 0)
                new ResolveConflictForm(conflicts[conflictsKeys[conflictsListBox.SelectedIndex]]).ShowDialog();
        }
    }
}
