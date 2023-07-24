namespace ezAuto_EFEM
{
    partial class UI_Backside_ManualAlign
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
            this.buttonAlign = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonAlign
            // 
            this.buttonAlign.Location = new System.Drawing.Point(616, 726);
            this.buttonAlign.Name = "buttonAlign";
            this.buttonAlign.Size = new System.Drawing.Size(75, 23);
            this.buttonAlign.TabIndex = 0;
            this.buttonAlign.Text = "Align";
            this.buttonAlign.UseVisualStyleBackColor = true;
            this.buttonAlign.Click += new System.EventHandler(this.buttonAlign_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(697, 726);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // UI_Backside_ManualAlign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 761);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonAlign);
            this.Name = "UI_Backside_ManualAlign";
            this.Text = "HW_Backside_ManualAlignForm";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UI_Backside_ManualAlign_MouseDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonAlign;
        private System.Windows.Forms.Button buttonCancel;
    }
}