namespace ezAutoMom
{
    partial class WTR_Mom
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
            this.tbID = new System.Windows.Forms.TextBox();
            this.comboLocate = new System.Windows.Forms.ComboBox();
            this.comboArm = new System.Windows.Forms.ComboBox();
            this.grid = new PropertyGridEx.PropertyGridEx();
            this.labelCheckLower = new System.Windows.Forms.Label();
            this.labelCheckUpper = new System.Windows.Forms.Label();
            this.buttonPut = new System.Windows.Forms.Button();
            this.buttonGet = new System.Windows.Forms.Button();
            this.buttonRun = new System.Windows.Forms.Button();
            this.buttonHome = new System.Windows.Forms.Button();
            this.checkView = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // tbID
            // 
            this.tbID.Location = new System.Drawing.Point(14, 220);
            this.tbID.Name = "tbID";
            this.tbID.Size = new System.Drawing.Size(75, 21);
            this.tbID.TabIndex = 52;
            this.tbID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // comboLocate
            // 
            this.comboLocate.FormattingEnabled = true;
            this.comboLocate.Location = new System.Drawing.Point(14, 194);
            this.comboLocate.Name = "comboLocate";
            this.comboLocate.Size = new System.Drawing.Size(75, 20);
            this.comboLocate.TabIndex = 51;
            // 
            // comboArm
            // 
            this.comboArm.FormattingEnabled = true;
            this.comboArm.Location = new System.Drawing.Point(14, 168);
            this.comboArm.Name = "comboArm";
            this.comboArm.Size = new System.Drawing.Size(75, 20);
            this.comboArm.TabIndex = 50;
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
            this.grid.DocCommentDescription.Size = new System.Drawing.Size(172, 36);
            this.grid.DocCommentDescription.TabIndex = 1;
            this.grid.DocCommentImage = null;
            // 
            // 
            // 
            this.grid.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.grid.DocCommentTitle.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.grid.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.grid.DocCommentTitle.Name = "";
            this.grid.DocCommentTitle.Size = new System.Drawing.Size(172, 16);
            this.grid.DocCommentTitle.TabIndex = 0;
            this.grid.DocCommentTitle.UseMnemonic = false;
            this.grid.LineColor = System.Drawing.SystemColors.ControlDark;
            this.grid.Location = new System.Drawing.Point(95, 34);
            this.grid.Name = "grid";
            this.grid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.grid.Size = new System.Drawing.Size(178, 314);
            this.grid.TabIndex = 49;
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
            // labelCheckLower
            // 
            this.labelCheckLower.AutoSize = true;
            this.labelCheckLower.Location = new System.Drawing.Point(12, 77);
            this.labelCheckLower.Name = "labelCheckLower";
            this.labelCheckLower.Size = new System.Drawing.Size(80, 12);
            this.labelCheckLower.TabIndex = 48;
            this.labelCheckLower.Text = "Check Lower";
            // 
            // labelCheckUpper
            // 
            this.labelCheckUpper.AutoSize = true;
            this.labelCheckUpper.Location = new System.Drawing.Point(12, 63);
            this.labelCheckUpper.Name = "labelCheckUpper";
            this.labelCheckUpper.Size = new System.Drawing.Size(78, 12);
            this.labelCheckUpper.TabIndex = 47;
            this.labelCheckUpper.Text = "Check Upper";
            // 
            // buttonPut
            // 
            this.buttonPut.Location = new System.Drawing.Point(14, 139);
            this.buttonPut.Name = "buttonPut";
            this.buttonPut.Size = new System.Drawing.Size(75, 23);
            this.buttonPut.TabIndex = 46;
            this.buttonPut.Text = "Put";
            this.buttonPut.UseVisualStyleBackColor = true;
            this.buttonPut.Click += new System.EventHandler(this.buttonPut_Click);
            // 
            // buttonGet
            // 
            this.buttonGet.Location = new System.Drawing.Point(14, 110);
            this.buttonGet.Name = "buttonGet";
            this.buttonGet.Size = new System.Drawing.Size(75, 23);
            this.buttonGet.TabIndex = 45;
            this.buttonGet.Text = "Get";
            this.buttonGet.UseVisualStyleBackColor = true;
            this.buttonGet.Click += new System.EventHandler(this.buttonGet_Click);
            // 
            // buttonRun
            // 
            this.buttonRun.Location = new System.Drawing.Point(14, 34);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(75, 23);
            this.buttonRun.TabIndex = 44;
            this.buttonRun.Text = "Run";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // buttonHome
            // 
            this.buttonHome.Location = new System.Drawing.Point(14, 276);
            this.buttonHome.Name = "buttonHome";
            this.buttonHome.Size = new System.Drawing.Size(75, 23);
            this.buttonHome.TabIndex = 43;
            this.buttonHome.Text = "Home";
            this.buttonHome.UseVisualStyleBackColor = true;
            this.buttonHome.Click += new System.EventHandler(this.buttonHome_Click);
            // 
            // checkView
            // 
            this.checkView.AutoSize = true;
            this.checkView.Location = new System.Drawing.Point(14, 12);
            this.checkView.Name = "checkView";
            this.checkView.Size = new System.Drawing.Size(50, 16);
            this.checkView.TabIndex = 42;
            this.checkView.Text = "WTR";
            this.checkView.UseVisualStyleBackColor = true;
            this.checkView.CheckedChanged += new System.EventHandler(this.checkView_CheckedChanged);
            // 
            // WTR_Mom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 360);
            this.Controls.Add(this.tbID);
            this.Controls.Add(this.comboLocate);
            this.Controls.Add(this.comboArm);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.labelCheckLower);
            this.Controls.Add(this.labelCheckUpper);
            this.Controls.Add(this.buttonPut);
            this.Controls.Add(this.buttonGet);
            this.Controls.Add(this.buttonRun);
            this.Controls.Add(this.buttonHome);
            this.Controls.Add(this.checkView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "WTR_Mom";
            this.Text = "WTR_Mom";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PropertyGridEx.PropertyGridEx grid;
        private System.Windows.Forms.Label labelCheckLower;
        private System.Windows.Forms.Label labelCheckUpper;
        private System.Windows.Forms.Button buttonPut;
        private System.Windows.Forms.Button buttonGet;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Button buttonHome;
        private System.Windows.Forms.CheckBox checkView;
        protected System.Windows.Forms.ComboBox comboArm;
        protected System.Windows.Forms.TextBox tbID;
        protected System.Windows.Forms.ComboBox comboLocate;
    }
}