namespace ezAutoMom
{
    partial class HW_WTR_Mom
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
            this.checkView = new System.Windows.Forms.CheckBox();
            this.grid = new PropertyGridEx.PropertyGridEx();
            this.labelLArmCheck = new System.Windows.Forms.Label();
            this.labelUArmCheck = new System.Windows.Forms.Label();
            this.buttonVacOff = new System.Windows.Forms.Button();
            this.buttonHome = new System.Windows.Forms.Button();
            this.buttonReset = new System.Windows.Forms.Button();
            this.buttonPut = new System.Windows.Forms.Button();
            this.buttonVacOn = new System.Windows.Forms.Button();
            this.buttonGet = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnPUTRETA = new System.Windows.Forms.Button();
            this.btnPUTEXRT = new System.Windows.Forms.Button();
            this.btnGETRETA = new System.Windows.Forms.Button();
            this.btnGETEXRT = new System.Windows.Forms.Button();
            this.comboWaferSize = new System.Windows.Forms.ComboBox();
            this.comboSlot = new System.Windows.Forms.ComboBox();
            this.comboPosition = new System.Windows.Forms.ComboBox();
            this.comboArm = new System.Windows.Forms.ComboBox();
            this.buttonGripOff = new System.Windows.Forms.Button();
            this.buttonGripOn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkView
            // 
            this.checkView.AutoSize = true;
            this.checkView.Location = new System.Drawing.Point(10, 12);
            this.checkView.Name = "checkView";
            this.checkView.Size = new System.Drawing.Size(52, 17);
            this.checkView.TabIndex = 13;
            this.checkView.Text = "WTR";
            this.checkView.UseVisualStyleBackColor = true;
            this.checkView.CheckedChanged += new System.EventHandler(this.checkView_CheckedChanged);
            // 
            // grid
            // 
            // 
            // 
            // 
            this.grid.DocCommentDescription.AutoEllipsis = true;
            this.grid.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.grid.DocCommentDescription.Location = new System.Drawing.Point(3, 18);
            this.grid.DocCommentDescription.Name = "";
            this.grid.DocCommentDescription.Size = new System.Drawing.Size(172, 37);
            this.grid.DocCommentDescription.TabIndex = 1;
            this.grid.DocCommentImage = null;
            // 
            // 
            // 
            this.grid.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.grid.DocCommentTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.grid.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.grid.DocCommentTitle.Name = "";
            this.grid.DocCommentTitle.Size = new System.Drawing.Size(172, 15);
            this.grid.DocCommentTitle.TabIndex = 0;
            this.grid.DocCommentTitle.UseMnemonic = false;
            this.grid.Location = new System.Drawing.Point(80, 91);
            this.grid.Name = "grid";
            this.grid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.grid.Size = new System.Drawing.Size(153, 456);
            this.grid.TabIndex = 16;
            this.grid.ToolbarVisible = false;
            // 
            // 
            // 
            this.grid.ToolStrip.AccessibleName = "도구 모음";
            this.grid.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this.grid.ToolStrip.AllowMerge = false;
            this.grid.ToolStrip.AutoSize = false;
            this.grid.ToolStrip.CanOverflow = false;
            this.grid.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.grid.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.grid.ToolStrip.Location = new System.Drawing.Point(0, 1);
            this.grid.ToolStrip.Name = "";
            this.grid.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.grid.ToolStrip.Size = new System.Drawing.Size(130, 25);
            this.grid.ToolStrip.TabIndex = 1;
            this.grid.ToolStrip.TabStop = true;
            this.grid.ToolStrip.Text = "PropertyGridToolBar";
            this.grid.ToolStrip.Visible = false;
            this.grid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.grid_PropertyValueChanged);
            // 
            // labelLArmCheck
            // 
            this.labelLArmCheck.AutoSize = true;
            this.labelLArmCheck.Location = new System.Drawing.Point(108, 39);
            this.labelLArmCheck.Name = "labelLArmCheck";
            this.labelLArmCheck.Size = new System.Drawing.Size(97, 13);
            this.labelLArmCheck.TabIndex = 17;
            this.labelLArmCheck.Text = "LArm Wafer Check";
            // 
            // labelUArmCheck
            // 
            this.labelUArmCheck.AutoSize = true;
            this.labelUArmCheck.Location = new System.Drawing.Point(108, 62);
            this.labelUArmCheck.Name = "labelUArmCheck";
            this.labelUArmCheck.Size = new System.Drawing.Size(99, 13);
            this.labelUArmCheck.TabIndex = 18;
            this.labelUArmCheck.Text = "UArm Wafer Check";
            // 
            // buttonVacOff
            // 
            this.buttonVacOff.Location = new System.Drawing.Point(5, 233);
            this.buttonVacOff.Name = "buttonVacOff";
            this.buttonVacOff.Size = new System.Drawing.Size(64, 25);
            this.buttonVacOff.TabIndex = 31;
            this.buttonVacOff.Text = "Vac Off";
            this.buttonVacOff.UseVisualStyleBackColor = true;
            this.buttonVacOff.Click += new System.EventHandler(this.buttonVacOff_Click);
            // 
            // buttonHome
            // 
            this.buttonHome.Location = new System.Drawing.Point(10, 39);
            this.buttonHome.Name = "buttonHome";
            this.buttonHome.Size = new System.Drawing.Size(64, 25);
            this.buttonHome.TabIndex = 30;
            this.buttonHome.Text = "Home";
            this.buttonHome.UseVisualStyleBackColor = true;
            this.buttonHome.Click += new System.EventHandler(this.buttonHome_Click);
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(10, 70);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(64, 25);
            this.buttonReset.TabIndex = 28;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // buttonPut
            // 
            this.buttonPut.Location = new System.Drawing.Point(5, 169);
            this.buttonPut.Name = "buttonPut";
            this.buttonPut.Size = new System.Drawing.Size(64, 25);
            this.buttonPut.TabIndex = 26;
            this.buttonPut.Text = "Put";
            this.buttonPut.UseVisualStyleBackColor = true;
            this.buttonPut.Click += new System.EventHandler(this.buttonPut_Click);
            // 
            // buttonVacOn
            // 
            this.buttonVacOn.Location = new System.Drawing.Point(5, 200);
            this.buttonVacOn.Name = "buttonVacOn";
            this.buttonVacOn.Size = new System.Drawing.Size(64, 25);
            this.buttonVacOn.TabIndex = 15;
            this.buttonVacOn.Text = "Vac On";
            this.buttonVacOn.UseVisualStyleBackColor = true;
            this.buttonVacOn.Click += new System.EventHandler(this.buttonVacOn_Click);
            // 
            // buttonGet
            // 
            this.buttonGet.Location = new System.Drawing.Point(5, 138);
            this.buttonGet.Name = "buttonGet";
            this.buttonGet.Size = new System.Drawing.Size(64, 25);
            this.buttonGet.TabIndex = 27;
            this.buttonGet.Text = "Get";
            this.buttonGet.UseVisualStyleBackColor = true;
            this.buttonGet.Click += new System.EventHandler(this.buttonGet_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnPUTRETA);
            this.groupBox1.Controls.Add(this.btnPUTEXRT);
            this.groupBox1.Controls.Add(this.btnGETRETA);
            this.groupBox1.Controls.Add(this.btnGETEXRT);
            this.groupBox1.Controls.Add(this.comboWaferSize);
            this.groupBox1.Controls.Add(this.comboSlot);
            this.groupBox1.Controls.Add(this.comboPosition);
            this.groupBox1.Controls.Add(this.comboArm);
            this.groupBox1.Controls.Add(this.buttonVacOff);
            this.groupBox1.Controls.Add(this.buttonGet);
            this.groupBox1.Controls.Add(this.buttonPut);
            this.groupBox1.Controls.Add(this.buttonVacOn);
            this.groupBox1.Location = new System.Drawing.Point(3, 99);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(76, 400);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Motion";
            // 
            // btnPUTRETA
            // 
            this.btnPUTRETA.Location = new System.Drawing.Point(5, 359);
            this.btnPUTRETA.Name = "btnPUTRETA";
            this.btnPUTRETA.Size = new System.Drawing.Size(64, 25);
            this.btnPUTRETA.TabIndex = 39;
            this.btnPUTRETA.Text = "Put RETA";
            this.btnPUTRETA.UseVisualStyleBackColor = true;
            this.btnPUTRETA.Click += new System.EventHandler(this.btnPUTRETA_Click);
            // 
            // btnPUTEXRT
            // 
            this.btnPUTEXRT.Location = new System.Drawing.Point(5, 327);
            this.btnPUTEXRT.Name = "btnPUTEXRT";
            this.btnPUTEXRT.Size = new System.Drawing.Size(64, 25);
            this.btnPUTEXRT.TabIndex = 38;
            this.btnPUTEXRT.Text = "Put EXRT";
            this.btnPUTEXRT.UseVisualStyleBackColor = true;
            this.btnPUTEXRT.Click += new System.EventHandler(this.btnPUTEXRT_Click);
            // 
            // btnGETRETA
            // 
            this.btnGETRETA.Location = new System.Drawing.Point(5, 296);
            this.btnGETRETA.Name = "btnGETRETA";
            this.btnGETRETA.Size = new System.Drawing.Size(64, 25);
            this.btnGETRETA.TabIndex = 37;
            this.btnGETRETA.Text = "Get RETA";
            this.btnGETRETA.UseVisualStyleBackColor = true;
            this.btnGETRETA.Click += new System.EventHandler(this.btnGETRETA_Click);
            // 
            // btnGETEXRT
            // 
            this.btnGETEXRT.Location = new System.Drawing.Point(5, 264);
            this.btnGETEXRT.Name = "btnGETEXRT";
            this.btnGETEXRT.Size = new System.Drawing.Size(64, 25);
            this.btnGETEXRT.TabIndex = 36;
            this.btnGETEXRT.Text = "Get EXRT";
            this.btnGETEXRT.UseVisualStyleBackColor = true;
            this.btnGETEXRT.Click += new System.EventHandler(this.btnGETEXRT_Click);
            // 
            // comboWaferSize
            // 
            this.comboWaferSize.FormattingEnabled = true;
            this.comboWaferSize.Location = new System.Drawing.Point(3, 103);
            this.comboWaferSize.Name = "comboWaferSize";
            this.comboWaferSize.Size = new System.Drawing.Size(73, 21);
            this.comboWaferSize.TabIndex = 35;
            // 
            // comboSlot
            // 
            this.comboSlot.FormattingEnabled = true;
            this.comboSlot.Location = new System.Drawing.Point(3, 75);
            this.comboSlot.Name = "comboSlot";
            this.comboSlot.Size = new System.Drawing.Size(73, 21);
            this.comboSlot.TabIndex = 34;
            // 
            // comboPosition
            // 
            this.comboPosition.FormattingEnabled = true;
            this.comboPosition.Location = new System.Drawing.Point(3, 47);
            this.comboPosition.Name = "comboPosition";
            this.comboPosition.Size = new System.Drawing.Size(73, 21);
            this.comboPosition.TabIndex = 33;
            // 
            // comboArm
            // 
            this.comboArm.FormattingEnabled = true;
            this.comboArm.Location = new System.Drawing.Point(3, 18);
            this.comboArm.Name = "comboArm";
            this.comboArm.Size = new System.Drawing.Size(73, 21);
            this.comboArm.TabIndex = 32;
            // 
            // buttonGripOff
            // 
            this.buttonGripOff.Location = new System.Drawing.Point(10, 522);
            this.buttonGripOff.Name = "buttonGripOff";
            this.buttonGripOff.Size = new System.Drawing.Size(64, 25);
            this.buttonGripOff.TabIndex = 41;
            this.buttonGripOff.Text = "Grip Off";
            this.buttonGripOff.UseVisualStyleBackColor = true;
            this.buttonGripOff.Click += new System.EventHandler(this.buttonGripOff_Click);
            // 
            // buttonGripOn
            // 
            this.buttonGripOn.Location = new System.Drawing.Point(10, 489);
            this.buttonGripOn.Name = "buttonGripOn";
            this.buttonGripOn.Size = new System.Drawing.Size(64, 25);
            this.buttonGripOn.TabIndex = 40;
            this.buttonGripOn.Text = "Grip On";
            this.buttonGripOn.UseVisualStyleBackColor = true;
            this.buttonGripOn.Click += new System.EventHandler(this.buttonGripOn_Click);
            // 
            // HW_WTR_Mom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(243, 560);
            this.Controls.Add(this.buttonGripOff);
            this.Controls.Add(this.buttonGripOn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonHome);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.labelUArmCheck);
            this.Controls.Add(this.labelLArmCheck);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.checkView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HW_WTR_Mom";
            this.Text = "HW_WTR";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkView;
        private PropertyGridEx.PropertyGridEx grid;
        private System.Windows.Forms.Label labelLArmCheck;
        private System.Windows.Forms.Label labelUArmCheck;
        private System.Windows.Forms.Button buttonVacOff;
        private System.Windows.Forms.Button buttonHome;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Button buttonPut;
        private System.Windows.Forms.Button buttonVacOn;
        private System.Windows.Forms.Button buttonGet;
        protected System.Windows.Forms.GroupBox groupBox1;
        protected System.Windows.Forms.ComboBox comboWaferSize;
        protected System.Windows.Forms.ComboBox comboSlot;
        protected System.Windows.Forms.ComboBox comboPosition;
        protected System.Windows.Forms.ComboBox comboArm;
        private System.Windows.Forms.Button btnPUTRETA;
        private System.Windows.Forms.Button btnPUTEXRT;
        private System.Windows.Forms.Button btnGETRETA;
        private System.Windows.Forms.Button btnGETEXRT;
        private System.Windows.Forms.Button buttonGripOff;
        private System.Windows.Forms.Button buttonGripOn;
    }
}