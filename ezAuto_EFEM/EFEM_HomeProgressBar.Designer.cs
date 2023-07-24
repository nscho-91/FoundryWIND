namespace ezAuto_EFEM
{
    partial class EFEM_HomeProgressBar
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EFEM_HomeProgressBar));
            this.labelWTR = new System.Windows.Forms.Label();
            this.progressBarWtr = new System.Windows.Forms.ProgressBar();
            this.labelLP1 = new System.Windows.Forms.Label();
            this.progressBarLP1 = new System.Windows.Forms.ProgressBar();
            this.labelLP2 = new System.Windows.Forms.Label();
            this.progressBarLP2 = new System.Windows.Forms.ProgressBar();
            this.labelAligner = new System.Windows.Forms.Label();
            this.progressBarAligner = new System.Windows.Forms.ProgressBar();
            this.labelVision = new System.Windows.Forms.Label();
            this.progressBarVision = new System.Windows.Forms.ProgressBar();
            this.timerPrgress = new System.Windows.Forms.Timer(this.components);
            this.progressBarLP3 = new System.Windows.Forms.ProgressBar();
            this.labelLP3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelWTR
            // 
            this.labelWTR.AutoSize = true;
            this.labelWTR.Enabled = false;
            this.labelWTR.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelWTR.Location = new System.Drawing.Point(12, 9);
            this.labelWTR.Name = "labelWTR";
            this.labelWTR.Size = new System.Drawing.Size(204, 20);
            this.labelWTR.TabIndex = 0;
            this.labelWTR.Text = "WTR Home Progress";
            // 
            // progressBarWtr
            // 
            this.progressBarWtr.Enabled = false;
            this.progressBarWtr.Location = new System.Drawing.Point(16, 32);
            this.progressBarWtr.Name = "progressBarWtr";
            this.progressBarWtr.Size = new System.Drawing.Size(556, 40);
            this.progressBarWtr.TabIndex = 1;
            // 
            // labelLP1
            // 
            this.labelLP1.AutoSize = true;
            this.labelLP1.Enabled = false;
            this.labelLP1.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelLP1.Location = new System.Drawing.Point(12, 85);
            this.labelLP1.Name = "labelLP1";
            this.labelLP1.Size = new System.Drawing.Size(255, 20);
            this.labelLP1.TabIndex = 2;
            this.labelLP1.Tag = "0";
            this.labelLP1.Text = "Loadport1 Home Progress";
            // 
            // progressBarLP1
            // 
            this.progressBarLP1.Enabled = false;
            this.progressBarLP1.Location = new System.Drawing.Point(16, 108);
            this.progressBarLP1.Name = "progressBarLP1";
            this.progressBarLP1.Size = new System.Drawing.Size(556, 40);
            this.progressBarLP1.TabIndex = 3;
            // 
            // labelLP2
            // 
            this.labelLP2.AutoSize = true;
            this.labelLP2.Enabled = false;
            this.labelLP2.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelLP2.Location = new System.Drawing.Point(12, 161);
            this.labelLP2.Name = "labelLP2";
            this.labelLP2.Size = new System.Drawing.Size(255, 20);
            this.labelLP2.TabIndex = 4;
            this.labelLP2.Tag = "1";
            this.labelLP2.Text = "Loadport2 Home Progress";
            // 
            // progressBarLP2
            // 
            this.progressBarLP2.Enabled = false;
            this.progressBarLP2.Location = new System.Drawing.Point(16, 184);
            this.progressBarLP2.Name = "progressBarLP2";
            this.progressBarLP2.Size = new System.Drawing.Size(556, 40);
            this.progressBarLP2.TabIndex = 5;
            // 
            // labelAligner
            // 
            this.labelAligner.AutoSize = true;
            this.labelAligner.Enabled = false;
            this.labelAligner.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelAligner.Location = new System.Drawing.Point(12, 325);
            this.labelAligner.Name = "labelAligner";
            this.labelAligner.Size = new System.Drawing.Size(221, 20);
            this.labelAligner.TabIndex = 6;
            this.labelAligner.Text = "Aligner Home Progress";
            // 
            // progressBarAligner
            // 
            this.progressBarAligner.Enabled = false;
            this.progressBarAligner.Location = new System.Drawing.Point(16, 348);
            this.progressBarAligner.Name = "progressBarAligner";
            this.progressBarAligner.Size = new System.Drawing.Size(556, 40);
            this.progressBarAligner.TabIndex = 7;
            // 
            // labelVision
            // 
            this.labelVision.AutoSize = true;
            this.labelVision.Enabled = false;
            this.labelVision.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelVision.Location = new System.Drawing.Point(12, 401);
            this.labelVision.Name = "labelVision";
            this.labelVision.Size = new System.Drawing.Size(216, 20);
            this.labelVision.TabIndex = 8;
            this.labelVision.Text = "Vision Home Progress";
            // 
            // progressBarVision
            // 
            this.progressBarVision.Enabled = false;
            this.progressBarVision.Location = new System.Drawing.Point(16, 424);
            this.progressBarVision.Name = "progressBarVision";
            this.progressBarVision.Size = new System.Drawing.Size(556, 40);
            this.progressBarVision.TabIndex = 9;
            // 
            // timerPrgress
            // 
            this.timerPrgress.Enabled = true;
            this.timerPrgress.Tick += new System.EventHandler(this.timerPrgress_Tick);
            // 
            // progressBarLP3
            // 
            this.progressBarLP3.Enabled = false;
            this.progressBarLP3.Location = new System.Drawing.Point(16, 265);
            this.progressBarLP3.Name = "progressBarLP3";
            this.progressBarLP3.Size = new System.Drawing.Size(556, 40);
            this.progressBarLP3.TabIndex = 11;
            // 
            // labelLP3
            // 
            this.labelLP3.AutoSize = true;
            this.labelLP3.Enabled = false;
            this.labelLP3.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelLP3.Location = new System.Drawing.Point(12, 242);
            this.labelLP3.Name = "labelLP3";
            this.labelLP3.Size = new System.Drawing.Size(255, 20);
            this.labelLP3.TabIndex = 10;
            this.labelLP3.Tag = "2";
            this.labelLP3.Text = "Loadport3 Home Progress";
            // 
            // EFEM_HomeProgressBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(587, 484);
            this.Controls.Add(this.progressBarLP3);
            this.Controls.Add(this.labelLP3);
            this.Controls.Add(this.progressBarVision);
            this.Controls.Add(this.labelVision);
            this.Controls.Add(this.progressBarAligner);
            this.Controls.Add(this.labelAligner);
            this.Controls.Add(this.progressBarLP2);
            this.Controls.Add(this.labelLP2);
            this.Controls.Add(this.progressBarLP1);
            this.Controls.Add(this.labelLP1);
            this.Controls.Add(this.progressBarWtr);
            this.Controls.Add(this.labelWTR);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EFEM_HomeProgressBar";
            this.Text = "EFEM_ProcessBar";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EFEM_HomeProgressBar_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelWTR;
        private System.Windows.Forms.ProgressBar progressBarWtr;
        private System.Windows.Forms.Label labelLP1;
        private System.Windows.Forms.ProgressBar progressBarLP1;
        private System.Windows.Forms.Label labelLP2;
        private System.Windows.Forms.ProgressBar progressBarLP2;
        private System.Windows.Forms.Label labelAligner;
        private System.Windows.Forms.ProgressBar progressBarAligner;
        private System.Windows.Forms.Label labelVision;
        private System.Windows.Forms.ProgressBar progressBarVision;
        private System.Windows.Forms.Timer timerPrgress;
        private System.Windows.Forms.ProgressBar progressBarLP3;
        private System.Windows.Forms.Label labelLP3;

    }
}