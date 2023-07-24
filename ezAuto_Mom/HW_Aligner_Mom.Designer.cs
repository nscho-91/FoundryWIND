namespace ezAutoMom
{
    partial class HW_Aligner_Mom
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
            this.buttonRotate = new System.Windows.Forms.Button();
            this.grid = new PropertyGridEx.PropertyGridEx();
            this.buttonAlign = new System.Windows.Forms.Button();
            this.buttonVacOn = new System.Windows.Forms.Button();
            this.buttonHome = new System.Windows.Forms.Button();
            this.labelError = new System.Windows.Forms.Label();
            this.labelWafer = new System.Windows.Forms.Label();
            this.timerLabel = new System.Windows.Forms.Timer(this.components);
            this.buttonVacOff = new System.Windows.Forms.Button();
            this.buttonBCR = new System.Windows.Forms.Button();
            this.buttonOCR = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkView
            // 
            this.checkView.AutoSize = true;
            this.checkView.Location = new System.Drawing.Point(12, 12);
            this.checkView.Name = "checkView";
            this.checkView.Size = new System.Drawing.Size(63, 16);
            this.checkView.TabIndex = 13;
            this.checkView.Text = "Aligner";
            this.checkView.UseVisualStyleBackColor = true;
            this.checkView.CheckedChanged += new System.EventHandler(this.checkView_CheckedChanged);
            // 
            // buttonRotate
            // 
            this.buttonRotate.Location = new System.Drawing.Point(12, 179);
            this.buttonRotate.Name = "buttonRotate";
            this.buttonRotate.Size = new System.Drawing.Size(75, 23);
            this.buttonRotate.TabIndex = 16;
            this.buttonRotate.Text = "Rotate";
            this.buttonRotate.UseVisualStyleBackColor = true;
            this.buttonRotate.Click += new System.EventHandler(this.buttonRotate_Click);
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
            this.grid.Location = new System.Drawing.Point(93, 63);
            this.grid.Name = "grid";
            this.grid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.grid.Size = new System.Drawing.Size(178, 246);
            this.grid.TabIndex = 17;
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
            // buttonAlign
            // 
            this.buttonAlign.Location = new System.Drawing.Point(12, 92);
            this.buttonAlign.Name = "buttonAlign";
            this.buttonAlign.Size = new System.Drawing.Size(75, 23);
            this.buttonAlign.TabIndex = 18;
            this.buttonAlign.Text = "Align";
            this.buttonAlign.UseVisualStyleBackColor = true;
            this.buttonAlign.Click += new System.EventHandler(this.buttonAlign_Click);
            // 
            // buttonVacOn
            // 
            this.buttonVacOn.Location = new System.Drawing.Point(12, 34);
            this.buttonVacOn.Name = "buttonVacOn";
            this.buttonVacOn.Size = new System.Drawing.Size(75, 23);
            this.buttonVacOn.TabIndex = 20;
            this.buttonVacOn.Text = "Vac On";
            this.buttonVacOn.UseVisualStyleBackColor = true;
            this.buttonVacOn.Click += new System.EventHandler(this.buttonVacOn_Click);
            // 
            // buttonHome
            // 
            this.buttonHome.Location = new System.Drawing.Point(12, 208);
            this.buttonHome.Name = "buttonHome";
            this.buttonHome.Size = new System.Drawing.Size(75, 23);
            this.buttonHome.TabIndex = 21;
            this.buttonHome.Text = "Home";
            this.buttonHome.UseVisualStyleBackColor = true;
            this.buttonHome.Click += new System.EventHandler(this.buttonHome_Click);
            // 
            // labelError
            // 
            this.labelError.AutoSize = true;
            this.labelError.Location = new System.Drawing.Point(135, 39);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(52, 12);
            this.labelError.TabIndex = 23;
            this.labelError.Text = "No Error";
            // 
            // labelWafer
            // 
            this.labelWafer.AutoSize = true;
            this.labelWafer.Location = new System.Drawing.Point(93, 39);
            this.labelWafer.Name = "labelWafer";
            this.labelWafer.Size = new System.Drawing.Size(36, 12);
            this.labelWafer.TabIndex = 24;
            this.labelWafer.Text = "Wafer";
            // 
            // timerLabel
            // 
            this.timerLabel.Enabled = true;
            this.timerLabel.Interval = 200;
            this.timerLabel.Tick += new System.EventHandler(this.timerLabel_Tick);
            // 
            // buttonVacOff
            // 
            this.buttonVacOff.Location = new System.Drawing.Point(12, 63);
            this.buttonVacOff.Name = "buttonVacOff";
            this.buttonVacOff.Size = new System.Drawing.Size(75, 23);
            this.buttonVacOff.TabIndex = 25;
            this.buttonVacOff.Text = "Vac Off";
            this.buttonVacOff.UseVisualStyleBackColor = true;
            this.buttonVacOff.Click += new System.EventHandler(this.buttonVacOff_Click);
            // 
            // buttonBCR
            // 
            this.buttonBCR.Location = new System.Drawing.Point(12, 150);
            this.buttonBCR.Name = "buttonBCR";
            this.buttonBCR.Size = new System.Drawing.Size(75, 23);
            this.buttonBCR.TabIndex = 26;
            this.buttonBCR.Text = "BCR";
            this.buttonBCR.UseVisualStyleBackColor = true;
            this.buttonBCR.Click += new System.EventHandler(this.buttonBCR_Click);
            // 
            // buttonOCR
            // 
            this.buttonOCR.Location = new System.Drawing.Point(12, 121);
            this.buttonOCR.Name = "buttonOCR";
            this.buttonOCR.Size = new System.Drawing.Size(75, 23);
            this.buttonOCR.TabIndex = 27;
            this.buttonOCR.Text = "OCR";
            this.buttonOCR.UseVisualStyleBackColor = true;
            this.buttonOCR.Click += new System.EventHandler(this.buttonOCR_Click);
            // 
            // HW_Aligner_Mom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 321);
            this.Controls.Add(this.buttonOCR);
            this.Controls.Add(this.buttonBCR);
            this.Controls.Add(this.buttonVacOff);
            this.Controls.Add(this.labelWafer);
            this.Controls.Add(this.labelError);
            this.Controls.Add(this.buttonHome);
            this.Controls.Add(this.buttonVacOn);
            this.Controls.Add(this.buttonAlign);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.buttonRotate);
            this.Controls.Add(this.checkView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HW_Aligner_Mom";
            this.Text = "HW_Aligner";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkView;
        private System.Windows.Forms.Button buttonRotate;
        private PropertyGridEx.PropertyGridEx grid;
        private System.Windows.Forms.Button buttonAlign;
        private System.Windows.Forms.Button buttonVacOn;
        private System.Windows.Forms.Label labelError;
        private System.Windows.Forms.Label labelWafer;
        private System.Windows.Forms.Timer timerLabel;
        private System.Windows.Forms.Button buttonVacOff;
        private System.Windows.Forms.Button buttonBCR;
        private System.Windows.Forms.Button buttonOCR;
        protected System.Windows.Forms.Button buttonHome;
    }
}