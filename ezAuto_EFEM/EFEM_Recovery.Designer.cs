using ezAutoMom;
namespace ezAuto_EFEM
{
    partial class EFEM_Recovery
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
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.buttonRecover = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.pnWaferInfo = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 250;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // buttonRecover
            // 
            this.buttonRecover.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.buttonRecover.Location = new System.Drawing.Point(59, 660);
            this.buttonRecover.Name = "buttonRecover";
            this.buttonRecover.Size = new System.Drawing.Size(201, 44);
            this.buttonRecover.TabIndex = 0;
            this.buttonRecover.Text = "Recover";
            this.buttonRecover.UseVisualStyleBackColor = true;
            this.buttonRecover.Click += new System.EventHandler(this.buttonRecover_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("굴림", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.buttonCancel.Location = new System.Drawing.Point(315, 660);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(177, 44);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // pnWaferInfo
            // 
            this.pnWaferInfo.BackColor = System.Drawing.Color.Snow;
            this.pnWaferInfo.Location = new System.Drawing.Point(546, 3);
            this.pnWaferInfo.Name = "pnWaferInfo";
            this.pnWaferInfo.Size = new System.Drawing.Size(391, 743);
            this.pnWaferInfo.TabIndex = 2;
            this.pnWaferInfo.Paint += new System.Windows.Forms.PaintEventHandler(this.pnWaferInfo_Paint);
            // 
            // EFEM_Recovery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(937, 716);
            this.Controls.Add(this.pnWaferInfo);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonRecover);
            this.Name = "EFEM_Recovery";
            this.Text = "EFEM_Recovery";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button buttonRecover;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Panel pnWaferInfo;
    }
}