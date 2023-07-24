namespace ezAuto_EFEM
{
    partial class UI_Simulation
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
            if (disposing && (components != null)) {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UI_Simulation));
            this.panelLP2 = new System.Windows.Forms.Panel();
            this.panelLP1 = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panelWaferInAligner = new ezAuto_EFEM.DoubleBufferPanel();
            this.PanelBoat = new ezAuto_EFEM.DoubleBufferPanel();
            this.panelWaferInVS = new ezAuto_EFEM.DoubleBufferPanel();
            this.PanelScanY = new ezAuto_EFEM.DoubleBufferPanel();
            this.panelWTR = new ezAuto_EFEM.DoubleBufferPanel();
            this.PanelWaferInLArm = new ezAuto_EFEM.DoubleBufferPanel();
            this.panelWaferInUArm = new ezAuto_EFEM.DoubleBufferPanel();
            this.lbVisionConnect = new System.Windows.Forms.Label();
            this.PanelBoat.SuspendLayout();
            this.panelWTR.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLP2
            // 
            this.panelLP2.BackColor = System.Drawing.Color.Transparent;
            this.panelLP2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelLP2.BackgroundImage")));
            this.panelLP2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelLP2.Location = new System.Drawing.Point(506, 188);
            this.panelLP2.Name = "panelLP2";
            this.panelLP2.Size = new System.Drawing.Size(110, 110);
            this.panelLP2.TabIndex = 0;
            // 
            // panelLP1
            // 
            this.panelLP1.BackColor = System.Drawing.Color.Transparent;
            this.panelLP1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelLP1.BackgroundImage")));
            this.panelLP1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelLP1.Location = new System.Drawing.Point(506, 367);
            this.panelLP1.Name = "panelLP1";
            this.panelLP1.Size = new System.Drawing.Size(110, 110);
            this.panelLP1.TabIndex = 1;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panelWaferInAligner
            // 
            this.panelWaferInAligner.BackColor = System.Drawing.Color.Transparent;
            this.panelWaferInAligner.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelWaferInAligner.Location = new System.Drawing.Point(340, 41);
            this.panelWaferInAligner.Name = "panelWaferInAligner";
            this.panelWaferInAligner.Size = new System.Drawing.Size(114, 82);
            this.panelWaferInAligner.TabIndex = 4;
            this.panelWaferInAligner.Visible = false;
            // 
            // PanelBoat
            // 
            this.PanelBoat.BackColor = System.Drawing.Color.Transparent;
            this.PanelBoat.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("PanelBoat.BackgroundImage")));
            this.PanelBoat.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PanelBoat.Controls.Add(this.panelWaferInVS);
            this.PanelBoat.Location = new System.Drawing.Point(87, 103);
            this.PanelBoat.Name = "PanelBoat";
            this.PanelBoat.Size = new System.Drawing.Size(133, 118);
            this.PanelBoat.TabIndex = 3;
            // 
            // panelWaferInVS
            // 
            this.panelWaferInVS.BackColor = System.Drawing.Color.Transparent;
            this.panelWaferInVS.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelWaferInVS.Location = new System.Drawing.Point(3, 13);
            this.panelWaferInVS.Name = "panelWaferInVS";
            this.panelWaferInVS.Size = new System.Drawing.Size(114, 82);
            this.panelWaferInVS.TabIndex = 3;
            this.panelWaferInVS.Visible = false;
            // 
            // PanelScanY
            // 
            this.PanelScanY.BackColor = System.Drawing.Color.Transparent;
            this.PanelScanY.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.ScanY1;
            this.PanelScanY.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PanelScanY.Location = new System.Drawing.Point(12, 113);
            this.PanelScanY.Name = "PanelScanY";
            this.PanelScanY.Size = new System.Drawing.Size(257, 101);
            this.PanelScanY.TabIndex = 4;
            // 
            // panelWTR
            // 
            this.panelWTR.BackColor = System.Drawing.Color.Transparent;
            this.panelWTR.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelWTR.Controls.Add(this.PanelWaferInLArm);
            this.panelWTR.Controls.Add(this.panelWaferInUArm);
            this.panelWTR.Location = new System.Drawing.Point(325, 226);
            this.panelWTR.Name = "panelWTR";
            this.panelWTR.Size = new System.Drawing.Size(129, 193);
            this.panelWTR.TabIndex = 2;
            // 
            // PanelWaferInLArm
            // 
            this.PanelWaferInLArm.BackColor = System.Drawing.Color.Transparent;
            this.PanelWaferInLArm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PanelWaferInLArm.Location = new System.Drawing.Point(3, 103);
            this.PanelWaferInLArm.Name = "PanelWaferInLArm";
            this.PanelWaferInLArm.Size = new System.Drawing.Size(114, 82);
            this.PanelWaferInLArm.TabIndex = 6;
            this.PanelWaferInLArm.Visible = false;
            // 
            // panelWaferInUArm
            // 
            this.panelWaferInUArm.BackColor = System.Drawing.Color.Transparent;
            this.panelWaferInUArm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelWaferInUArm.Location = new System.Drawing.Point(3, 15);
            this.panelWaferInUArm.Name = "panelWaferInUArm";
            this.panelWaferInUArm.Size = new System.Drawing.Size(114, 82);
            this.panelWaferInUArm.TabIndex = 5;
            this.panelWaferInUArm.Visible = false;
            // 
            // lbVisionConnect
            // 
            this.lbVisionConnect.BackColor = System.Drawing.Color.Salmon;
            this.lbVisionConnect.Font = new System.Drawing.Font("굴림", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbVisionConnect.Location = new System.Drawing.Point(2, 2);
            this.lbVisionConnect.Name = "lbVisionConnect";
            this.lbVisionConnect.Size = new System.Drawing.Size(280, 59);
            this.lbVisionConnect.TabIndex = 5;
            this.lbVisionConnect.Text = "Not Connected";
            this.lbVisionConnect.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UI_Simulation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(664, 562);
            this.Controls.Add(this.lbVisionConnect);
            this.Controls.Add(this.panelWaferInAligner);
            this.Controls.Add(this.PanelBoat);
            this.Controls.Add(this.PanelScanY);
            this.Controls.Add(this.panelWTR);
            this.Controls.Add(this.panelLP1);
            this.Controls.Add(this.panelLP2);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "UI_Simulation";
            this.Text = "UI_Simulation";
            this.Resize += new System.EventHandler(this.UI_Simulation_Resize);
            this.PanelBoat.ResumeLayout(false);
            this.panelWTR.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLP2;
        private System.Windows.Forms.Panel panelLP1;
        private System.Windows.Forms.Timer timer1;
        private DoubleBufferPanel panelWTR;
        private DoubleBufferPanel PanelBoat;
        private DoubleBufferPanel PanelScanY;
        private DoubleBufferPanel panelWaferInVS;
        private DoubleBufferPanel panelWaferInAligner;
        private DoubleBufferPanel panelWaferInUArm;
        private DoubleBufferPanel PanelWaferInLArm;
        private System.Windows.Forms.Label lbVisionConnect;

    }
}