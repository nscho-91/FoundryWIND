namespace ezAutoMom
{
    partial class Alarm_Mom
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
            this.tbModule = new System.Windows.Forms.TextBox();
            this.tbAlarmCode = new System.Windows.Forms.TextBox();
            this.tbDetail = new System.Windows.Forms.TextBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pbModule = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbModule)).BeginInit();
            this.SuspendLayout();
            // 
            // tbModule
            // 
            this.tbModule.BackColor = System.Drawing.SystemColors.Info;
            this.tbModule.Font = new System.Drawing.Font("굴림", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbModule.Location = new System.Drawing.Point(12, 83);
            this.tbModule.Multiline = true;
            this.tbModule.Name = "tbModule";
            this.tbModule.ReadOnly = true;
            this.tbModule.Size = new System.Drawing.Size(187, 35);
            this.tbModule.TabIndex = 1;
            this.tbModule.Text = "Module Name";
            this.tbModule.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbAlarmCode
            // 
            this.tbAlarmCode.BackColor = System.Drawing.SystemColors.Info;
            this.tbAlarmCode.Font = new System.Drawing.Font("굴림", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbAlarmCode.Location = new System.Drawing.Point(205, 83);
            this.tbAlarmCode.Multiline = true;
            this.tbAlarmCode.Name = "tbAlarmCode";
            this.tbAlarmCode.ReadOnly = true;
            this.tbAlarmCode.Size = new System.Drawing.Size(548, 35);
            this.tbAlarmCode.TabIndex = 3;
            this.tbAlarmCode.Text = "Alarm Code";
            this.tbAlarmCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbDetail
            // 
            this.tbDetail.BackColor = System.Drawing.Color.White;
            this.tbDetail.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbDetail.Location = new System.Drawing.Point(358, 125);
            this.tbDetail.Multiline = true;
            this.tbDetail.Name = "tbDetail";
            this.tbDetail.ReadOnly = true;
            this.tbDetail.Size = new System.Drawing.Size(395, 339);
            this.tbDetail.TabIndex = 5;
            this.tbDetail.Text = "Detail......";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::ezAutoMom.Properties.Resources._2;
            this.pictureBox3.Location = new System.Drawing.Point(205, 12);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(548, 65);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 7;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::ezAutoMom.Properties.Resources._1;
            this.pictureBox2.Location = new System.Drawing.Point(12, 12);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(187, 65);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 6;
            this.pictureBox2.TabStop = false;
            // 
            // pbModule
            // 
            this.pbModule.BackgroundImage = global::ezAutoMom.Properties.Resources.cross;
            this.pbModule.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pbModule.Location = new System.Drawing.Point(77, 197);
            this.pbModule.Name = "pbModule";
            this.pbModule.Size = new System.Drawing.Size(198, 178);
            this.pbModule.TabIndex = 4;
            this.pbModule.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(205, 470);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(312, 54);
            this.button1.TabIndex = 8;
            this.button1.Text = "닫기";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Alarm_Mom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(764, 536);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.tbDetail);
            this.Controls.Add(this.pbModule);
            this.Controls.Add(this.tbAlarmCode);
            this.Controls.Add(this.tbModule);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Alarm_Mom";
            this.Text = "Alarm_Mom";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Alarm_Mom_FormClosing_1);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbModule)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbModule;
        private System.Windows.Forms.TextBox tbAlarmCode;
        private System.Windows.Forms.PictureBox pbModule;
        private System.Windows.Forms.TextBox tbDetail;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Button button1;
    }
}