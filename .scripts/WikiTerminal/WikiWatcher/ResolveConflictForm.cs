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
    public partial class ResolveConflictForm : Form
    {
        private WatchResult conflict;

        public ResolveConflictForm(WatchResult conflict)
        {
            this.conflict = conflict;
            InitializeComponent();
            newVersionTextBox.Text = conflict.content;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            conflict.resolve_action = 1;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            conflict.resolve_action = 2;
            Close();
        }
    }
}
