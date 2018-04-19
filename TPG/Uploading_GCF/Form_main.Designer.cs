namespace Uploading_db
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
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxFile = new System.Windows.Forms.TextBox();
            this.textBoxVer = new System.Windows.Forms.TextBox();
            this.buttonUpload = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.configToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadBSInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxsh = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelln = new System.Windows.Forms.Label();
            this.labelstat = new System.Windows.Forms.Label();
            this.buttondel = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonabort = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxlog = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(782, 31);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(123, 46);
            this.buttonBrowse.TabIndex = 0;
            this.buttonBrowse.Text = "Open Test Plan";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // textBoxPath
            // 
            this.textBoxPath.Location = new System.Drawing.Point(107, 44);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(608, 22);
            this.textBoxPath.TabIndex = 1;
            this.textBoxPath.TextChanged += new System.EventHandler(this.textBoxPath_TextChanged);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(88, 156);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "File Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(88, 208);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(148, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Version (GCF/PTCRB)";
            // 
            // textBoxFile
            // 
            this.textBoxFile.Location = new System.Drawing.Point(301, 151);
            this.textBoxFile.Name = "textBoxFile";
            this.textBoxFile.Size = new System.Drawing.Size(414, 22);
            this.textBoxFile.TabIndex = 5;
            // 
            // textBoxVer
            // 
            this.textBoxVer.Location = new System.Drawing.Point(301, 208);
            this.textBoxVer.Name = "textBoxVer";
            this.textBoxVer.Size = new System.Drawing.Size(414, 22);
            this.textBoxVer.TabIndex = 7;
            // 
            // buttonUpload
            // 
            this.buttonUpload.Location = new System.Drawing.Point(782, 93);
            this.buttonUpload.Name = "buttonUpload";
            this.buttonUpload.Size = new System.Drawing.Size(123, 50);
            this.buttonUpload.TabIndex = 8;
            this.buttonUpload.Text = "Upload";
            this.buttonUpload.UseVisualStyleBackColor = true;
            this.buttonUpload.Click += new System.EventHandler(this.buttonUpload_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configToolStripMenuItem,
            this.uploadBSInfoToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1488, 28);
            this.menuStrip1.TabIndex = 9;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // configToolStripMenuItem
            // 
            this.configToolStripMenuItem.Name = "configToolStripMenuItem";
            this.configToolStripMenuItem.Size = new System.Drawing.Size(65, 24);
            this.configToolStripMenuItem.Text = "Config";
            this.configToolStripMenuItem.Click += new System.EventHandler(this.configToolStripMenuItem_Click);
            // 
            // uploadBSInfoToolStripMenuItem
            // 
            this.uploadBSInfoToolStripMenuItem.Name = "uploadBSInfoToolStripMenuItem";
            this.uploadBSInfoToolStripMenuItem.Size = new System.Drawing.Size(121, 24);
            this.uploadBSInfoToolStripMenuItem.Text = "Upload BS info";
            this.uploadBSInfoToolStripMenuItem.Click += new System.EventHandler(this.uploadBSInfoToolStripMenuItem_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(88, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 17);
            this.label4.TabIndex = 11;
            this.label4.Text = "TC Category";
            // 
            // textBoxsh
            // 
            this.textBoxsh.Location = new System.Drawing.Point(301, 273);
            this.textBoxsh.Name = "textBoxsh";
            this.textBoxsh.ReadOnly = true;
            this.textBoxsh.Size = new System.Drawing.Size(414, 22);
            this.textBoxsh.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(91, 273);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 17);
            this.label5.TabIndex = 13;
            this.label5.Text = "SheetName";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(91, 342);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 17);
            this.label6.TabIndex = 15;
            this.label6.Text = "Status";
            // 
            // labelln
            // 
            this.labelln.AutoSize = true;
            this.labelln.Location = new System.Drawing.Point(298, 342);
            this.labelln.Name = "labelln";
            this.labelln.Size = new System.Drawing.Size(0, 17);
            this.labelln.TabIndex = 16;
            // 
            // labelstat
            // 
            this.labelstat.AutoSize = true;
            this.labelstat.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelstat.ForeColor = System.Drawing.Color.Red;
            this.labelstat.Location = new System.Drawing.Point(99, 403);
            this.labelstat.Name = "labelstat";
            this.labelstat.Size = new System.Drawing.Size(113, 48);
            this.labelstat.TabIndex = 17;
            this.labelstat.Text = "IDLE";
            // 
            // buttondel
            // 
            this.buttondel.Location = new System.Drawing.Point(782, 357);
            this.buttondel.Name = "buttondel";
            this.buttondel.Size = new System.Drawing.Size(62, 25);
            this.buttondel.TabIndex = 18;
            this.buttondel.Text = "Delete";
            this.buttondel.UseVisualStyleBackColor = true;
            this.buttondel.Click += new System.EventHandler(this.buttondel_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(751, 252);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(170, 17);
            this.label7.TabIndex = 19;
            this.label7.Text = "Delete all data for version";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.label8.Location = new System.Drawing.Point(778, 290);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(0, 24);
            this.label8.TabIndex = 20;
            // 
            // buttonabort
            // 
            this.buttonabort.Enabled = false;
            this.buttonabort.Location = new System.Drawing.Point(782, 171);
            this.buttonabort.Name = "buttonabort";
            this.buttonabort.Size = new System.Drawing.Size(123, 50);
            this.buttonabort.TabIndex = 21;
            this.buttonabort.Text = "Abort";
            this.buttonabort.UseVisualStyleBackColor = true;
            this.buttonabort.Click += new System.EventHandler(this.buttonabort_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pictureBox1.Location = new System.Drawing.Point(22, 498);
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
            this.label9.Location = new System.Drawing.Point(62, 512);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(174, 17);
            this.label9.TabIndex = 23;
            this.label9.Text = "DB Connection (for future)";
            // 
            // textBoxlog
            // 
            this.textBoxlog.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxlog.Location = new System.Drawing.Point(941, 60);
            this.textBoxlog.Multiline = true;
            this.textBoxlog.Name = "textBoxlog";
            this.textBoxlog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxlog.Size = new System.Drawing.Size(535, 466);
            this.textBoxlog.TabIndex = 24;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(936, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 25);
            this.label2.TabIndex = 25;
            this.label2.Text = "Log";
            // 
            // FormUpload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1488, 562);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxlog);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.buttonabort);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.buttondel);
            this.Controls.Add(this.labelstat);
            this.Controls.Add(this.labelln);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxsh);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.buttonUpload);
            this.Controls.Add(this.textBoxVer);
            this.Controls.Add(this.textBoxFile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxPath);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "FormUpload";
            this.Text = "Uploading to Database";
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxFile;
        private System.Windows.Forms.TextBox textBoxVer;
        private System.Windows.Forms.Button buttonUpload;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem configToolStripMenuItem;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxsh;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelln;
        private System.Windows.Forms.Label labelstat;
        private System.Windows.Forms.Button buttondel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonabort;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxlog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem uploadBSInfoToolStripMenuItem;
    }
}

