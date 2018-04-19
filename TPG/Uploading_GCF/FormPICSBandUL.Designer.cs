namespace Uploading_db
{
    partial class FormPICSBandUL
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
            this.label_pics_version = new System.Windows.Forms.Label();
            this.textBoxband = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_submit = new System.Windows.Forms.Button();
            this.textBoxProj = new System.Windows.Forms.TextBox();
            this.textBoxBandULCA = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label_pics_version
            // 
            this.label_pics_version.AutoSize = true;
            this.label_pics_version.Location = new System.Drawing.Point(46, 10);
            this.label_pics_version.Name = "label_pics_version";
            this.label_pics_version.Size = new System.Drawing.Size(52, 17);
            this.label_pics_version.TabIndex = 1;
            this.label_pics_version.Text = "Project";
            // 
            // textBoxband
            // 
            this.textBoxband.Location = new System.Drawing.Point(49, 83);
            this.textBoxband.Multiline = true;
            this.textBoxband.Name = "textBoxband";
            this.textBoxband.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxband.Size = new System.Drawing.Size(644, 196);
            this.textBoxband.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(46, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(220, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "PICS Supported Bands for project";
            // 
            // button_submit
            // 
            this.button_submit.Location = new System.Drawing.Point(609, 411);
            this.button_submit.Name = "button_submit";
            this.button_submit.Size = new System.Drawing.Size(84, 32);
            this.button_submit.TabIndex = 3;
            this.button_submit.Text = "Submit";
            this.button_submit.UseVisualStyleBackColor = true;
            this.button_submit.Click += new System.EventHandler(this.button_submit_Click);
            // 
            // textBoxProj
            // 
            this.textBoxProj.Location = new System.Drawing.Point(49, 36);
            this.textBoxProj.Name = "textBoxProj";
            this.textBoxProj.Size = new System.Drawing.Size(138, 22);
            this.textBoxProj.TabIndex = 0;
            // 
            // textBoxBandULCA
            // 
            this.textBoxBandULCA.Location = new System.Drawing.Point(49, 316);
            this.textBoxBandULCA.Multiline = true;
            this.textBoxBandULCA.Name = "textBoxBandULCA";
            this.textBoxBandULCA.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxBandULCA.Size = new System.Drawing.Size(644, 89);
            this.textBoxBandULCA.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(46, 296);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(270, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "PICS Supported Bands for project (ULCA)";
            // 
            // FormPICSBandUL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 455);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxBandULCA);
            this.Controls.Add(this.textBoxProj);
            this.Controls.Add(this.button_submit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxband);
            this.Controls.Add(this.label_pics_version);
            this.Name = "FormPICSBandUL";
            this.Text = "FormPICSBandUL";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label_pics_version;
        private System.Windows.Forms.TextBox textBoxband;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_submit;
        private System.Windows.Forms.TextBox textBoxProj;
        private System.Windows.Forms.TextBox textBoxBandULCA;
        private System.Windows.Forms.Label label2;
    }
}