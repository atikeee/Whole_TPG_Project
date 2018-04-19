namespace TestPlanGen
{
    partial class PleaseWaitForm
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
            this.PleaseWaitLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PleaseWaitLabel
            // 
            this.PleaseWaitLabel.AutoSize = true;
            this.PleaseWaitLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PleaseWaitLabel.Location = new System.Drawing.Point(68, 20);
            this.PleaseWaitLabel.Name = "PleaseWaitLabel";
            this.PleaseWaitLabel.Size = new System.Drawing.Size(205, 36);
            this.PleaseWaitLabel.TabIndex = 0;
            this.PleaseWaitLabel.Text = "Please Wait ...";
            // 
            // PleaseWaitForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 78);
            this.Controls.Add(this.PleaseWaitLabel);
            this.Name = "PleaseWaitForm";
            this.Text = "PleaseWaitForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label PleaseWaitLabel;
    }
}