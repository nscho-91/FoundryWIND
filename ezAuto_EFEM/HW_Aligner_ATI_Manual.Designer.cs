namespace ezAuto_EFEM
{
    partial class HW_Aligner_ATI_Manual
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
            this.components = new System.ComponentModel.Container();
            this.buttonJogRight = new System.Windows.Forms.Button();
            this.buttonJogLeft = new System.Windows.Forms.Button();
            this.buttonAlign = new System.Windows.Forms.Button();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // buttonJogRight
            // 
            this.buttonJogRight.Location = new System.Drawing.Point(52, 12);
            this.buttonJogRight.Name = "buttonJogRight";
            this.buttonJogRight.Size = new System.Drawing.Size(35, 23);
            this.buttonJogRight.TabIndex = 12;
            this.buttonJogRight.Text = ">";
            this.buttonJogRight.UseVisualStyleBackColor = true;
            this.buttonJogRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonJogRight_MouseDown);
            this.buttonJogRight.MouseLeave += new System.EventHandler(this.buttonJogRight_MouseLeave);
            this.buttonJogRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttonJogRight_MouseUp);
            // 
            // buttonJogLeft
            // 
            this.buttonJogLeft.Location = new System.Drawing.Point(12, 12);
            this.buttonJogLeft.Name = "buttonJogLeft";
            this.buttonJogLeft.Size = new System.Drawing.Size(35, 23);
            this.buttonJogLeft.TabIndex = 11;
            this.buttonJogLeft.Text = "<";
            this.buttonJogLeft.UseVisualStyleBackColor = true;
            this.buttonJogLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonJogLeft_MouseDown);
            this.buttonJogLeft.MouseLeave += new System.EventHandler(this.buttonJogLeft_MouseLeave);
            this.buttonJogLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttonJogLeft_MouseUp);
            // 
            // buttonAlign
            // 
            this.buttonAlign.Location = new System.Drawing.Point(130, 12);
            this.buttonAlign.Name = "buttonAlign";
            this.buttonAlign.Size = new System.Drawing.Size(75, 23);
            this.buttonAlign.TabIndex = 13;
            this.buttonAlign.Text = "Align";
            this.buttonAlign.UseVisualStyleBackColor = true;
            this.buttonAlign.Click += new System.EventHandler(this.buttonAlign_Click);
            // 
            // timer
            // 
            this.timer.Interval = 400;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // HW_Aligner_ATI_Manual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 512);
            this.Controls.Add(this.buttonAlign);
            this.Controls.Add(this.buttonJogRight);
            this.Controls.Add(this.buttonJogLeft);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HW_Aligner_ATI_Manual";
            this.Text = "HW_Aligner_ATI_Manual";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HW_Aligner_ATI_Manual_FormClosing);
            this.Load += new System.EventHandler(this.HW_Aligner_ATI_Manual_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.HW_Aligner_ATI_Manual_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HW_Aligner_ATI_Manual_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.HW_Aligner_ATI_Manual_KeyUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonJogRight;
        private System.Windows.Forms.Button buttonJogLeft;
        private System.Windows.Forms.Button buttonAlign;
        private System.Windows.Forms.Timer timer;
    }
}