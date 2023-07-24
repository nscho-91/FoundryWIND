namespace ezAuto_EFEM
{
    partial class UserControlOverViewLP
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelLP1 = new System.Windows.Forms.Panel();
            this.IODocking = new ezAuto_EFEM.UserControlIOIndicator();
            this.IOOpen = new ezAuto_EFEM.UserControlIOIndicator();
            this.IOPlaced = new ezAuto_EFEM.UserControlIOIndicator();
            this.labelLP = new System.Windows.Forms.Label();
            this.panelLP1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelLP1
            // 
            this.panelLP1.BackColor = System.Drawing.Color.Wheat;
            this.panelLP1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLP1.Controls.Add(this.IODocking);
            this.panelLP1.Controls.Add(this.IOOpen);
            this.panelLP1.Controls.Add(this.IOPlaced);
            this.panelLP1.Controls.Add(this.labelLP);
            this.panelLP1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLP1.Location = new System.Drawing.Point(0, 0);
            this.panelLP1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.panelLP1.Name = "panelLP1";
            this.panelLP1.Size = new System.Drawing.Size(203, 94);
            this.panelLP1.TabIndex = 16;
            // 
            // IODocking
            // 
            this.IODocking.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IODocking.IOName = "Docking";
            this.IODocking.Location = new System.Drawing.Point(107, 19);
            this.IODocking.Margin = new System.Windows.Forms.Padding(0);
            this.IODocking.Name = "IODocking";
            this.IODocking.Size = new System.Drawing.Size(93, 20);
            this.IODocking.TabIndex = 6;
            // 
            // IOOpen
            // 
            this.IOOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IOOpen.IOName = "Open";
            this.IOOpen.Location = new System.Drawing.Point(107, 39);
            this.IOOpen.Margin = new System.Windows.Forms.Padding(0);
            this.IOOpen.Name = "IOOpen";
            this.IOOpen.Size = new System.Drawing.Size(93, 20);
            this.IOOpen.TabIndex = 5;
            // 
            // IOPlaced
            // 
            this.IOPlaced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IOPlaced.IOName = "Placed";
            this.IOPlaced.Location = new System.Drawing.Point(107, -1);
            this.IOPlaced.Margin = new System.Windows.Forms.Padding(0);
            this.IOPlaced.Name = "IOPlaced";
            this.IOPlaced.Size = new System.Drawing.Size(93, 20);
            this.IOPlaced.TabIndex = 4;
            // 
            // labelLP
            // 
            this.labelLP.AutoSize = true;
            this.labelLP.Cursor = System.Windows.Forms.Cursors.Default;
            this.labelLP.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelLP.Location = new System.Drawing.Point(5, 5);
            this.labelLP.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelLP.Name = "labelLP";
            this.labelLP.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelLP.Size = new System.Drawing.Size(51, 24);
            this.labelLP.TabIndex = 0;
            this.labelLP.Text = "LP1";
            this.labelLP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UserControlOverViewLP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelLP1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UserControlOverViewLP";
            this.Size = new System.Drawing.Size(203, 94);
            this.panelLP1.ResumeLayout(false);
            this.panelLP1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelLP1;
        private UserControlIOIndicator IODocking;
        private UserControlIOIndicator IOOpen;
        private UserControlIOIndicator IOPlaced;
        private System.Windows.Forms.Label labelLP;
    }
}
