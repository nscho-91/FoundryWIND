namespace ezAuto_EFEM
{
    partial class UI_ManualJob_Hynix
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
            this.pnManualJob = new System.Windows.Forms.Panel();
            this.lbDescription = new System.Windows.Forms.Label();
            this.buttonSelectAll = new System.Windows.Forms.Button();
            this.buttonUnSelectAll = new System.Windows.Forms.Button();
            this.buttonAdd1 = new System.Windows.Forms.Button();
            this.buttonRun = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbRecipe = new System.Windows.Forms.TextBox();
            this.tbWaferSize = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbLotID = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panelWaferInfo = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnCheckMapFile = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxNameFilter = new System.Windows.Forms.TextBox();
            this.listViewRecipe = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderAngle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderMarkType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderModifyDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.tbCSTID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnManualJob.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnManualJob
            // 
            this.pnManualJob.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnManualJob.BackColor = System.Drawing.Color.SkyBlue;
            this.pnManualJob.Controls.Add(this.lbDescription);
            this.pnManualJob.Location = new System.Drawing.Point(0, 3);
            this.pnManualJob.Name = "pnManualJob";
            this.pnManualJob.Size = new System.Drawing.Size(670, 74);
            this.pnManualJob.TabIndex = 1;
            // 
            // lbDescription
            // 
            this.lbDescription.Font = new System.Drawing.Font("돋움", 25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbDescription.Location = new System.Drawing.Point(37, 8);
            this.lbDescription.Name = "lbDescription";
            this.lbDescription.Size = new System.Drawing.Size(517, 61);
            this.lbDescription.TabIndex = 0;
            this.lbDescription.Text = "Select Recipe And Insp. Slot";
            this.lbDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonSelectAll
            // 
            this.buttonSelectAll.Location = new System.Drawing.Point(16, 594);
            this.buttonSelectAll.Name = "buttonSelectAll";
            this.buttonSelectAll.Size = new System.Drawing.Size(102, 81);
            this.buttonSelectAll.TabIndex = 3;
            this.buttonSelectAll.Text = "Select All";
            this.buttonSelectAll.UseVisualStyleBackColor = true;
            this.buttonSelectAll.Click += new System.EventHandler(this.buttonSelectAll_Click);
            this.buttonSelectAll.Paint += new System.Windows.Forms.PaintEventHandler(this.buttonSelectAll_Paint);
            // 
            // buttonUnSelectAll
            // 
            this.buttonUnSelectAll.Location = new System.Drawing.Point(124, 594);
            this.buttonUnSelectAll.Name = "buttonUnSelectAll";
            this.buttonUnSelectAll.Size = new System.Drawing.Size(102, 81);
            this.buttonUnSelectAll.TabIndex = 4;
            this.buttonUnSelectAll.Text = "UnSelect All";
            this.buttonUnSelectAll.UseVisualStyleBackColor = true;
            this.buttonUnSelectAll.Click += new System.EventHandler(this.buttonUnSelectAll_Click);
            // 
            // buttonAdd1
            // 
            this.buttonAdd1.Location = new System.Drawing.Point(232, 594);
            this.buttonAdd1.Name = "buttonAdd1";
            this.buttonAdd1.Size = new System.Drawing.Size(102, 81);
            this.buttonAdd1.TabIndex = 5;
            this.buttonAdd1.Text = "Add 1";
            this.buttonAdd1.UseVisualStyleBackColor = true;
            this.buttonAdd1.Click += new System.EventHandler(this.buttonAdd1_Click);
            // 
            // buttonRun
            // 
            this.buttonRun.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonRun.Enabled = false;
            this.buttonRun.Location = new System.Drawing.Point(448, 594);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(102, 81);
            this.buttonRun.TabIndex = 7;
            this.buttonRun.Text = "Run";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(556, 594);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(102, 81);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.tbCSTID);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.tbRecipe);
            this.panel1.Controls.Add(this.tbWaferSize);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.tbLotID);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(3, 83);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(670, 295);
            this.panel1.TabIndex = 9;
            // 
            // tbRecipe
            // 
            this.tbRecipe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRecipe.Font = new System.Drawing.Font("굴림", 30F);
            this.tbRecipe.Location = new System.Drawing.Point(244, 139);
            this.tbRecipe.Name = "tbRecipe";
            this.tbRecipe.Size = new System.Drawing.Size(411, 53);
            this.tbRecipe.TabIndex = 8;
            // 
            // tbWaferSize
            // 
            this.tbWaferSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbWaferSize.Enabled = false;
            this.tbWaferSize.Font = new System.Drawing.Font("굴림", 30F);
            this.tbWaferSize.Location = new System.Drawing.Point(156, 208);
            this.tbWaferSize.Name = "tbWaferSize";
            this.tbWaferSize.Size = new System.Drawing.Size(209, 53);
            this.tbWaferSize.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("굴림", 30F);
            this.label4.Location = new System.Drawing.Point(29, 198);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 66);
            this.label4.TabIndex = 6;
            this.label4.Text = "Size :";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbLotID
            // 
            this.tbLotID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLotID.Font = new System.Drawing.Font("굴림", 30F);
            this.tbLotID.Location = new System.Drawing.Point(244, 74);
            this.tbLotID.Name = "tbLotID";
            this.tbLotID.Size = new System.Drawing.Size(411, 53);
            this.tbLotID.TabIndex = 4;
            this.tbLotID.TextChanged += new System.EventHandler(this.tbLotID_TextChanged);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("굴림", 30F);
            this.label3.Location = new System.Drawing.Point(16, 132);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(233, 66);
            this.label3.TabIndex = 2;
            this.label3.Text = "Recipe   :";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("굴림", 30F);
            this.label2.Location = new System.Drawing.Point(16, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(233, 66);
            this.label2.TabIndex = 1;
            this.label2.Text = "LOT ID   :";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelWaferInfo
            // 
            this.panelWaferInfo.BackColor = System.Drawing.SystemColors.Control;
            this.panelWaferInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelWaferInfo.Location = new System.Drawing.Point(0, 0);
            this.panelWaferInfo.Name = "panelWaferInfo";
            this.panelWaferInfo.Size = new System.Drawing.Size(360, 684);
            this.panelWaferInfo.TabIndex = 10;
            this.panelWaferInfo.Paint += new System.Windows.Forms.PaintEventHandler(this.panelWaferInfo_Paint);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnCheckMapFile);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.textBoxNameFilter);
            this.splitContainer1.Panel1.Controls.Add(this.pnManualJob);
            this.splitContainer1.Panel1.Controls.Add(this.listViewRecipe);
            this.splitContainer1.Panel1.Controls.Add(this.buttonSelectAll);
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            this.splitContainer1.Panel1.Controls.Add(this.buttonUnSelectAll);
            this.splitContainer1.Panel1.Controls.Add(this.buttonCancel);
            this.splitContainer1.Panel1.Controls.Add(this.buttonAdd1);
            this.splitContainer1.Panel1.Controls.Add(this.buttonRun);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelWaferInfo);
            this.splitContainer1.Size = new System.Drawing.Size(1036, 684);
            this.splitContainer1.SplitterDistance = 672;
            this.splitContainer1.TabIndex = 11;
            // 
            // btnCheckMapFile
            // 
            this.btnCheckMapFile.Location = new System.Drawing.Point(340, 594);
            this.btnCheckMapFile.Name = "btnCheckMapFile";
            this.btnCheckMapFile.Size = new System.Drawing.Size(102, 81);
            this.btnCheckMapFile.TabIndex = 31;
            this.btnCheckMapFile.Text = "Check Recipe";
            this.btnCheckMapFile.UseVisualStyleBackColor = true;
            this.btnCheckMapFile.Click += new System.EventHandler(this.btnCheckMapFile_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 390);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 12);
            this.label5.TabIndex = 29;
            this.label5.Text = "Text Filter";
            // 
            // textBoxNameFilter
            // 
            this.textBoxNameFilter.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxNameFilter.Location = new System.Drawing.Point(81, 384);
            this.textBoxNameFilter.Name = "textBoxNameFilter";
            this.textBoxNameFilter.Size = new System.Drawing.Size(201, 26);
            this.textBoxNameFilter.TabIndex = 28;
            this.textBoxNameFilter.TextChanged += new System.EventHandler(this.textBoxNameFilter_TextChanged);
            // 
            // listViewRecipe
            // 
            this.listViewRecipe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listViewRecipe.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderSize,
            this.columnHeaderAngle,
            this.columnHeaderMarkType,
            this.columnHeaderModifyDate});
            this.listViewRecipe.FullRowSelect = true;
            this.listViewRecipe.Location = new System.Drawing.Point(13, 419);
            this.listViewRecipe.Name = "listViewRecipe";
            this.listViewRecipe.Size = new System.Drawing.Size(645, 169);
            this.listViewRecipe.TabIndex = 27;
            this.listViewRecipe.UseCompatibleStateImageBehavior = false;
            this.listViewRecipe.View = System.Windows.Forms.View.Details;
            this.listViewRecipe.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewRecipe_ItemSelectionChanged);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 200;
            // 
            // columnHeaderSize
            // 
            this.columnHeaderSize.Text = "Size";
            this.columnHeaderSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderSize.Width = 110;
            // 
            // columnHeaderAngle
            // 
            this.columnHeaderAngle.Text = "Angle";
            this.columnHeaderAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderAngle.Width = 110;
            // 
            // columnHeaderMarkType
            // 
            this.columnHeaderMarkType.Text = "MarkType";
            this.columnHeaderMarkType.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderMarkType.Width = 110;
            // 
            // columnHeaderModifyDate
            // 
            this.columnHeaderModifyDate.Text = "ModifyDate";
            this.columnHeaderModifyDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderModifyDate.Width = 110;
            // 
            // timerUpdate
            // 
            this.timerUpdate.Enabled = true;
            this.timerUpdate.Interval = 200;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // tbCSTID
            // 
            this.tbCSTID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCSTID.Font = new System.Drawing.Font("굴림", 30F);
            this.tbCSTID.Location = new System.Drawing.Point(244, 8);
            this.tbCSTID.Name = "tbCSTID";
            this.tbCSTID.Size = new System.Drawing.Size(411, 53);
            this.tbCSTID.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("굴림", 30F);
            this.label1.Location = new System.Drawing.Point(16, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(233, 66);
            this.label1.TabIndex = 9;
            this.label1.Text = "CST ID   :";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UI_ManualJob_Hynix
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1036, 684);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UI_ManualJob_Hynix";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manual Job Schedule";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.UI_ManualJob_Hynix_Shown);
            this.VisibleChanged += new System.EventHandler(this.UI_ManualJob_VisibleChanged);
            this.pnManualJob.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnManualJob;
        private System.Windows.Forms.Label lbDescription;
        private System.Windows.Forms.Button buttonSelectAll;
        private System.Windows.Forms.Button buttonUnSelectAll;
        private System.Windows.Forms.Button buttonAdd1;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox tbLotID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelWaferInfo;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox textBoxNameFilter;
        private System.Windows.Forms.ListView listViewRecipe;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderSize;
        private System.Windows.Forms.ColumnHeader columnHeaderAngle;
        private System.Windows.Forms.ColumnHeader columnHeaderMarkType;
        private System.Windows.Forms.ColumnHeader columnHeaderModifyDate;
        private System.Windows.Forms.TextBox tbWaferSize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.TextBox tbRecipe;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnCheckMapFile;
        private System.Windows.Forms.TextBox tbCSTID;
        private System.Windows.Forms.Label label1;
    }
}