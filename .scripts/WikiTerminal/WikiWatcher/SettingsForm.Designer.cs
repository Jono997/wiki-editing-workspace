
namespace WikiWatcher
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.keepWikiWatcherOpenCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.scanIntervalNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.scanIntervalNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // keepWikiWatcherOpenCheckBox
            // 
            this.keepWikiWatcherOpenCheckBox.AutoSize = true;
            this.keepWikiWatcherOpenCheckBox.Location = new System.Drawing.Point(15, 38);
            this.keepWikiWatcherOpenCheckBox.Name = "keepWikiWatcherOpenCheckBox";
            this.keepWikiWatcherOpenCheckBox.Size = new System.Drawing.Size(303, 17);
            this.keepWikiWatcherOpenCheckBox.TabIndex = 0;
            this.keepWikiWatcherOpenCheckBox.Text = "Keep WikiWatcher running after Visual Studio Code closes";
            this.keepWikiWatcherOpenCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Time inbetween scans (seconds):";
            // 
            // scanIntervalNumericUpDown
            // 
            this.scanIntervalNumericUpDown.Location = new System.Drawing.Point(183, 12);
            this.scanIntervalNumericUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.scanIntervalNumericUpDown.Name = "scanIntervalNumericUpDown";
            this.scanIntervalNumericUpDown.Size = new System.Drawing.Size(57, 20);
            this.scanIntervalNumericUpDown.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 61);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(314, 32);
            this.button1.TabIndex = 3;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 103);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.scanIntervalNumericUpDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.keepWikiWatcherOpenCheckBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "WikiWatcher settings";
            ((System.ComponentModel.ISupportInitialize)(this.scanIntervalNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox keepWikiWatcherOpenCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown scanIntervalNumericUpDown;
        private System.Windows.Forms.Button button1;
    }
}