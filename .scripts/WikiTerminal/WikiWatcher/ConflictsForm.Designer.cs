
namespace WikiWatcher
{
    partial class ConflictsForm
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
            this.conflictsListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // conflictsListBox
            // 
            this.conflictsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conflictsListBox.FormattingEnabled = true;
            this.conflictsListBox.Location = new System.Drawing.Point(0, 0);
            this.conflictsListBox.Name = "conflictsListBox";
            this.conflictsListBox.Size = new System.Drawing.Size(800, 450);
            this.conflictsListBox.TabIndex = 2;
            this.conflictsListBox.DoubleClick += new System.EventHandler(this.conflictsListBox_DoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "No conflicts found";
            // 
            // ConflictsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.conflictsListBox);
            this.Controls.Add(this.label1);
            this.Name = "ConflictsForm";
            this.Text = "WikiWatcher";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox conflictsListBox;
        private System.Windows.Forms.Label label1;
    }
}