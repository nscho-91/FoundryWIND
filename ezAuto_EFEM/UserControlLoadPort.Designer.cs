namespace ezAuto_EFEM
{
    partial class UserControlLoadPort
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
            this.components = new System.ComponentModel.Container();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.splitContainerMain = new ezAuto_EFEM.DoubleBufferSplitPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.brnService = new System.Windows.Forms.Button();
            this.btnUploadReq = new System.Windows.Forms.Button();
            this.btnAccess = new System.Windows.Forms.Button();
            this.btnReload = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerUpdate
            // 
            this.timerUpdate.Enabled = true;
            this.timerUpdate.Interval = 250;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.tableLayoutPanel1);
            this.splitContainerMain.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainerMain_Panel1_Paint);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel2_Paint);
            this.splitContainerMain.Size = new System.Drawing.Size(869, 563);
            this.splitContainerMain.SplitterDistance = 196;
            this.splitContainerMain.TabIndex = 7;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.buttonLoad, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.brnService, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnUploadReq, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnAccess, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnReload, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.progressBar1, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 95);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(194, 365);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // buttonLoad
            // 
            this.buttonLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLoad.Location = new System.Drawing.Point(6, 222);
            this.buttonLoad.Margin = new System.Windows.Forms.Padding(6);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(182, 137);
            this.buttonLoad.TabIndex = 0;
            this.buttonLoad.Text = "LOAD";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // brnService
            // 
            this.brnService.BackColor = System.Drawing.SystemColors.ControlDark;
            this.brnService.Dock = System.Windows.Forms.DockStyle.Fill;
            this.brnService.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.brnService.Location = new System.Drawing.Point(1, 1);
            this.brnService.Margin = new System.Windows.Forms.Padding(1);
            this.brnService.Name = "brnService";
            this.brnService.Size = new System.Drawing.Size(192, 34);
            this.brnService.TabIndex = 5;
            this.brnService.Text = "Out of Service";
            this.brnService.UseVisualStyleBackColor = false;
            this.brnService.Click += new System.EventHandler(this.brnService_Click);
            // 
            // btnUploadReq
            // 
            this.btnUploadReq.BackColor = System.Drawing.Color.LightBlue;
            this.btnUploadReq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnUploadReq.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUploadReq.Location = new System.Drawing.Point(1, 73);
            this.btnUploadReq.Margin = new System.Windows.Forms.Padding(1);
            this.btnUploadReq.Name = "btnUploadReq";
            this.btnUploadReq.Size = new System.Drawing.Size(192, 34);
            this.btnUploadReq.TabIndex = 6;
            this.btnUploadReq.Text = "Unload Request";
            this.btnUploadReq.UseVisualStyleBackColor = false;
            this.btnUploadReq.Click += new System.EventHandler(this.btnUploadReq_Click);
            // 
            // btnAccess
            // 
            this.btnAccess.BackColor = System.Drawing.SystemColors.ControlDark;
            this.btnAccess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAccess.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAccess.Location = new System.Drawing.Point(1, 37);
            this.btnAccess.Margin = new System.Windows.Forms.Padding(1);
            this.btnAccess.Name = "btnAccess";
            this.btnAccess.Size = new System.Drawing.Size(192, 34);
            this.btnAccess.TabIndex = 4;
            this.btnAccess.Text = "Manual";
            this.btnAccess.UseVisualStyleBackColor = false;
            this.btnAccess.Click += new System.EventHandler(this.btnAccess_Click);
            // 
            // btnReload
            // 
            this.btnReload.BackColor = System.Drawing.Color.Pink;
            this.btnReload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnReload.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnReload.Location = new System.Drawing.Point(1, 109);
            this.btnReload.Margin = new System.Windows.Forms.Padding(1);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(192, 34);
            this.btnReload.TabIndex = 7;
            this.btnReload.Text = "Reload";
            this.btnReload.UseVisualStyleBackColor = false;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar1.Location = new System.Drawing.Point(6, 168);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(6);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(182, 42);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 144);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(188, 18);
            this.label1.TabIndex = 3;
            this.label1.Text = "Inspection Progress";
            // 
            // UserControlLoadPort
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainerMain);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "UserControlLoadPort";
            this.Size = new System.Drawing.Size(869, 563);
            this.splitContainerMain.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timerUpdate;
        private DoubleBufferSplitPanel splitContainerMain;
        private System.Windows.Forms.Button btnAccess;
        private System.Windows.Forms.Button brnService;
        private System.Windows.Forms.Button btnUploadReq;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
