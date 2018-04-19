namespace Parse_Pixit_Table
{
    partial class FormUpload
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
            PresentationControls.CheckBoxProperties checkBoxProperties1 = new PresentationControls.CheckBoxProperties();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPICS = new System.Windows.Forms.TextBox();
            this.buttonUpload = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label4 = new System.Windows.Forms.Label();
            this.labelln = new System.Windows.Forms.Label();
            this.labelstat = new System.Windows.Forms.Label();
            this.label_version = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.textBoxlog = new System.Windows.Forms.TextBox();
            this.labellog = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.labelstat2 = new System.Windows.Forms.Label();
            this.checkBoxComboBox1 = new PresentationControls.CheckBoxComboBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(589, 95);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(123, 46);
            this.buttonBrowse.TabIndex = 0;
            this.buttonBrowse.Text = "Load PICS";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // textBoxPath
            // 
            this.textBoxPath.Location = new System.Drawing.Point(107, 44);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(605, 22);
            this.textBoxPath.TabIndex = 1;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(298, 202);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(333, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "PICS Version example:-  XMM7480_v1.3_20170101";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(85, 231);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "PICS Version ";
            // 
            // textBoxPICS
            // 
            this.textBoxPICS.Location = new System.Drawing.Point(286, 231);
            this.textBoxPICS.Name = "textBoxPICS";
            this.textBoxPICS.Size = new System.Drawing.Size(414, 22);
            this.textBoxPICS.TabIndex = 6;
            this.textBoxPICS.TextChanged += new System.EventHandler(this.textBoxPICS_TextChanged);
            // 
            // buttonUpload
            // 
            this.buttonUpload.Location = new System.Drawing.Point(301, 286);
            this.buttonUpload.Name = "buttonUpload";
            this.buttonUpload.Size = new System.Drawing.Size(123, 39);
            this.buttonUpload.TabIndex = 8;
            this.buttonUpload.Text = "Upload";
            this.buttonUpload.UseVisualStyleBackColor = true;
            this.buttonUpload.Click += new System.EventHandler(this.buttonUpload_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1361, 28);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.Enabled = false;
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            this.configToolStripMenuItem.Size = new System.Drawing.Size(65, 24);
            this.configToolStripMenuItem.Text = "Config";
            this.configToolStripMenuItem.Click += new System.EventHandler(this.configToolStripMenuItem_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(68, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 34);
            this.label4.TabIndex = 11;
            this.label4.Text = "Test \r\nGroup";
            // 
            // labelln
            // 
            this.labelln.AutoSize = true;
            this.labelln.Location = new System.Drawing.Point(678, 444);
            this.labelln.Name = "labelln";
            this.labelln.Size = new System.Drawing.Size(0, 17);
            this.labelln.TabIndex = 16;
            // 
            // labelstat
            // 
            this.labelstat.AutoSize = true;
            this.labelstat.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelstat.ForeColor = System.Drawing.Color.Red;
            this.labelstat.Location = new System.Drawing.Point(12, 522);
            this.labelstat.Name = "labelstat";
            this.labelstat.Size = new System.Drawing.Size(47, 20);
            this.labelstat.TabIndex = 17;
            this.labelstat.Text = "IDLE";
            // 
            // label_version
            // 
            this.label_version.AutoSize = true;
            this.label_version.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_version.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.label_version.Location = new System.Drawing.Point(71, 444);
            this.label_version.Name = "label_version";
            this.label_version.Size = new System.Drawing.Size(0, 24);
            this.label_version.TabIndex = 20;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pictureBox1.Location = new System.Drawing.Point(91, 311);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(27, 28);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 22;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(85, 275);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(174, 17);
            this.label9.TabIndex = 23;
            this.label9.Text = "DB Connection (for future)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(85, 163);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 17);
            this.label3.TabIndex = 26;
            this.label3.Text = "Spec Version";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(44, 360);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(689, 52);
            this.progressBar1.TabIndex = 27;
            // 
            // textBoxlog
            // 
            this.textBoxlog.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxlog.Location = new System.Drawing.Point(736, 83);
            this.textBoxlog.Multiline = true;
            this.textBoxlog.Name = "textBoxlog";
            this.textBoxlog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxlog.Size = new System.Drawing.Size(598, 484);
            this.textBoxlog.TabIndex = 28;
            this.textBoxlog.WordWrap = false;
            // 
            // labellog
            // 
            this.labellog.AutoSize = true;
            this.labellog.Location = new System.Drawing.Point(757, 44);
            this.labellog.Name = "labellog";
            this.labellog.Size = new System.Drawing.Size(46, 17);
            this.labellog.TabIndex = 29;
            this.labellog.Text = "LOG :";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(624, 291);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 34);
            this.button1.TabIndex = 30;
            this.button1.Text = "Delete";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelstat2
            // 
            this.labelstat2.AutoSize = true;
            this.labelstat2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelstat2.ForeColor = System.Drawing.Color.DarkGreen;
            this.labelstat2.Location = new System.Drawing.Point(18, 488);
            this.labelstat2.Name = "labelstat2";
            this.labelstat2.Size = new System.Drawing.Size(0, 26);
            this.labelstat2.TabIndex = 31;
            // 
            // checkBoxComboBox1
            // 
            checkBoxProperties1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxComboBox1.CheckBoxProperties = checkBoxProperties1;
            this.checkBoxComboBox1.DisplayMemberSingleItem = "";
            this.checkBoxComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.checkBoxComboBox1.FormattingEnabled = true;
            this.checkBoxComboBox1.Location = new System.Drawing.Point(223, 155);
            this.checkBoxComboBox1.Name = "checkBoxComboBox1";
            this.checkBoxComboBox1.Size = new System.Drawing.Size(476, 24);
            this.checkBoxComboBox1.TabIndex = 32;
            // 
            // FormUpload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1361, 579);
            this.Controls.Add(this.checkBoxComboBox1);
            this.Controls.Add(this.labelstat2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.labellog);
            this.Controls.Add(this.textBoxlog);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label_version);
            this.Controls.Add(this.labelstat);
            this.Controls.Add(this.labelln);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonUpload);
            this.Controls.Add(this.textBoxPICS);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxPath);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "FormUpload";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PICSUpload (v3.2)";
            this.Load += new System.EventHandler(this.FormUpload_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPICS;
        private System.Windows.Forms.Button buttonUpload;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem configToolStripMenuItem;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelln;
        private System.Windows.Forms.Label labelstat;
        private System.Windows.Forms.Label label_version;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox textBoxlog;
        private System.Windows.Forms.Label labellog;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelstat2;
        private PresentationControls.CheckBoxComboBox checkBoxComboBox1;
    }
}

