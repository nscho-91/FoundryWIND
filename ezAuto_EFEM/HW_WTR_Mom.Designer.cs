namespace ezAuto_EFEM
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
            this.comboPosition = new System.Windows.Forms.ComboBox();
            this.comboSlot = new System.Windows.Forms.ComboBox();
            this.comboArm = new System.Windows.Forms.ComboBox();
            this.comboWaferSize = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkView
            // 
            this.checkView.AutoSize = true;
            this.checkView.Location = new System.Drawing.Point(12, 12);
            this.checkView.Name = "checkView";
            this.checkView.Size = new System.Drawing.Size(50, 16);
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
            this.grid.DocCommentDescription.Location = new System.Drawing.Point(3, 19);
            this.grid.DocCommentDescription.Name = "";
            this.grid.DocCommentDescription.Size = new System.Drawing.Size(173, 36);
            this.grid.DocCommentDescription.TabIndex = 1;
            this.grid.DocCommentImage = null;
            // 
            // 
            // 
            this.grid.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.grid.DocCommentTitle.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.grid.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.grid.DocCommentTitle.Name = "";
            this.grid.DocCommentTitle.Size = new System.Drawing.Size(173, 16);
            this.grid.DocCommentTitle.TabIndex = 0;
            this.grid.DocCommentTitle.UseMnemonic = false;
            this.grid.Location = new System.Drawing.Point(93, 84);
            this.grid.Name = "grid";
            this.grid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.grid.Size = new System.Drawing.Size(179, 268);
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
            this.labelLArmCheck.Location = new System.Drawing.Point(126, 36);
            this.labelLArmCheck.Name = "labelLArmCheck";
            this.labelLArmCheck.Size = new System.Drawing.Size(110, 12);
            this.labelLArmCheck.TabIndex = 17;
            this.labelLArmCheck.Text = "LArm Wafer Check";
            // 
            // labelUArmCheck
            // 
            this.labelUArmCheck.AutoSize = true;
            this.labelUArmCheck.Location = new System.Drawing.Point(126, 57);
            this.labelUArmCheck.Name = "labelUArmCheck";
            this.labelUArmCheck.Size = new System.Drawing.Size(111, 12);
            this.labelUArmCheck.TabIndex = 18;
            this.labelUArmCheck.Text = "UArm Wafer Check";
            // 
            // buttonVacOff
            // 
            this.buttonVacOff.Location = new System.Drawing.Point(6, 214);
            this.buttonVacOff.Name = "buttonVacOff";
            this.buttonVacOff.Size = new System.Drawing.Size(75, 24);
            this.buttonVacOff.TabIndex = 31;
            this.buttonVacOff.Text = "Vac Off";
            this.buttonVacOff.UseVisualStyleBackColor = true;
            this.buttonVacOff.Click += new System.EventHandler(this.buttonVacOff_Click);
            // 
            // buttonHome
            // 
            this.buttonHome.Location = new System.Drawing.Point(12, 36);
            this.buttonHome.Name = "buttonHome";
            this.buttonHome.Size = new System.Drawing.Size(75, 23);
            this.buttonHome.TabIndex = 30;
            this.buttonHome.Text = "Home";
            this.buttonHome.UseVisualStyleBackColor = true;
            this.buttonHome.Click += new System.EventHandler(this.buttonHome_Click);
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(12, 65);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(75, 23);
            this.buttonReset.TabIndex = 28;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // buttonPut
            // 
            this.buttonPut.Location = new System.Drawing.Point(6, 151);
            this.buttonPut.Name = "buttonPut";
            this.buttonPut.Size = new System.Drawing.Size(75, 24);
            this.buttonPut.TabIndex = 26;
            this.buttonPut.Text = "Put";
            this.buttonPut.UseVisualStyleBackColor = true;
            this.buttonPut.Click += new System.EventHandler(this.buttonPut_Click);
            // 
            // buttonVacOn
            // 
            this.buttonVacOn.Location = new System.Drawing.Point(6, 184);
            this.buttonVacOn.Name = "buttonVacOn";
            this.buttonVacOn.Size = new System.Drawing.Size(75, 24);
            this.buttonVacOn.TabIndex = 15;
            this.buttonVacOn.Text = "Vac On";
            this.buttonVacOn.UseVisualStyleBackColor = true;
            this.buttonVacOn.Click += new System.EventHandler(this.buttonVacOn_Click);
            // 
            // buttonGet
            // 
            this.buttonGet.Location = new System.Drawing.Point(6, 122);
            this.buttonGet.Name = "buttonGet";
            this.buttonGet.Size = new System.Drawing.Size(75, 24);
            this.buttonGet.TabIndex = 27;
            this.buttonGet.Text = "Get";
            this.buttonGet.UseVisualStyleBackColor = true;
            this.buttonGet.Click += new System.EventHandler(this.buttonGet_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboWaferSize);
            this.groupBox1.Controls.Add(this.comboPosition);
            this.groupBox1.Controls.Add(this.comboSlot);
            this.groupBox1.Controls.Add(this.comboArm);
            this.groupBox1.Controls.Add(this.buttonVacOff);
            this.groupBox1.Controls.Add(this.buttonGet);
            this.groupBox1.Controls.Add(this.buttonPut);
            this.groupBox1.Controls.Add(this.buttonVacOn);
            this.groupBox1.Location = new System.Drawing.Point(6, 94);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(86, 246);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Motion";
            // 
            // comboPosition
            // 
            this.comboPosition.FormattingEnabled = true;
            this.comboPosition.Location = new System.Drawing.Point(3, 70);
            this.comboPosition.Name = "comboPosition";
            this.comboPosition.Size = new System.Drawing.Size(79, 20);
            this.comboPosition.TabIndex = 37;
            // 
            // comboSlot
            // 
            this.comboSlot.FormattingEnabled = true;
            this.comboSlot.Location = new System.Drawing.Point(7, 44);
            this.comboSlot.Name = "comboSlot";
            this.comboSlot.Size = new System.Drawing.Size(73, 20);
            this.comboSlot.TabIndex = 36;
            // 
            // comboArm
            // 
            this.comboArm.FormattingEnabled = true;
            this.comboArm.Location = new System.Drawing.Point(6, 18);
            this.comboArm.Name = "comboArm";
            this.comboArm.Size = new System.Drawing.Size(73, 20);
            this.comboArm.TabIndex = 35;
            // 
            // comboWaferSize
            // 
            this.comboWaferSize.FormattingEnabled = true;
            this.comboWaferSize.Location = new System.Drawing.Point(4, 96);
            this.comboWaferSize.Name = "comboWaferSize";
            this.comboWaferSize.Size = new System.Drawing.Size(79, 20);
            this.comboWaferSize.TabIndex = 38;
            // 
            // HW_WTR_Mom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 373);
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
        protected System.Windows.Forms.ComboBox comboPosition;
        protected System.Windows.Forms.ComboBox comboSlot;
        protected System.Windows.Forms.ComboBox comboArm;
        protected System.Windows.Forms.ComboBox comboWaferSize;
    }
}