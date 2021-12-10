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
    public partial class SettingsForm : Form
    {
        public DialogResult dialogResult;
        private Settings settings;

        public SettingsForm(Settings settings)
        {
            dialogResult = DialogResult.Cancel;
            this.settings = settings;
            InitializeComponent();
            scanIntervalNumericUpDown.Value = settings.scanInterval;
            keepWikiWatcherOpenCheckBox.Checked = settings.keepOpenWithoutVSCode;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            settings.scanInterval = (int)scanIntervalNumericUpDown.Value;
            settings.keepOpenWithoutVSCode = keepWikiWatcherOpenCheckBox.Checked;
            dialogResult = DialogResult.OK;
            Close();
        }
    }
}
