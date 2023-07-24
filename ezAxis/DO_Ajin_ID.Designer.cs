namespace ezAxis
{
    partial class DO_Ajin_ID
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
            this.button = new System.Windows.Forms.Button();
            this.text = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkRepeat = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // button
            // 
            this.button.Location = new System.Drawing.Point(184, 12);
            this.button.Name = "button";
            this.button.Size = new System.Drawing.Size(51, 23);
            this.button.TabIndex = 5;
            this.button.Text = "OK";
            this.button.UseVisualStyleBackColor = true;
            this.button.Click += new System.EventHandler(this.button_Click);
            // 
            // text
            // 
            this.text.Location = new System.Drawing.Point(34, 12);
            this.text.Name = "text";
            this.text.Size = new System.Drawing.Size(144, 21);
            this.text.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "ID";
            // 
            // checkRepeat
            // 
            this.checkRepeat.AutoSize = true;
            this.checkRepeat.Location = new System.Drawing.Point(14, 42);
            this.checkRepeat.Name = "checkRepeat";
            this.checkRepeat.Size = new System.Drawing.Size(63, 16);
            this.checkRepeat.TabIndex = 6;
            this.checkRepeat.Text = "Repeat";
            this.checkRepeat.UseVisualStyleBackColor = true;
            this.checkRepeat.CheckedChanged += new System.EventHandler(this.checkRepeat_CheckedChanged);
            // 
            // DO_Ajin_ID
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 70);
            this.Controls.Add(this.checkRepeat);
            this.Controls.Add(this.button);
            this.Controls.Add(this.text);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DO_Ajin_ID";
            this.Text = "DO_Ajin_ID";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DO_Ajin_ID_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button;
        private System.Windows.Forms.TextBox text;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkRepeat;
    }
}