namespace ezAuto_EFEM
{
    partial class HW_Aligner_EBR_Edge
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
            this.buttonInspect = new System.Windows.Forms.Button();
            this.buttonRGBSave = new System.Windows.Forms.Button();
            this.buttonRGBRead = new System.Windows.Forms.Button();
            this.buttonCamSetup = new System.Windows.Forms.Button();
            this.timerSave = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // checkView
            // 
            this.checkView.AutoSize = true;
            this.checkView.Location = new System.Drawing.Point(12, 12);
            this.checkView.Name = "checkView";
            this.checkView.Size = new System.Drawing.Size(53, 16);
            this.checkView.TabIndex = 15;
            this.checkView.Text = "Edge";
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
            this.grid.Location = new System.Drawing.Point(94, 34);
            this.grid.Name = "grid";
            this.grid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.grid.Size = new System.Drawing.Size(178, 216);
            this.grid.TabIndex = 19;
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
            // buttonInspect
            // 
            this.buttonInspect.Location = new System.Drawing.Point(12, 34);
            this.buttonInspect.Name = "buttonInspect";
            this.buttonInspect.Size = new System.Drawing.Size(75, 23);
            this.buttonInspect.TabIndex = 20;
            this.buttonInspect.Text = "Inspect";
            this.buttonInspect.UseVisualStyleBackColor = true;
            this.buttonInspect.Click += new System.EventHandler(this.buttonInspect_Click);
            // 
            // buttonRGBSave
            // 
            this.buttonRGBSave.Location = new System.Drawing.Point(13, 227);
            this.buttonRGBSave.Name = "buttonRGBSave";
            this.buttonRGBSave.Size = new System.Drawing.Size(75, 23);
            this.buttonRGBSave.TabIndex = 21;
            this.buttonRGBSave.Text = "RGB Save";
            this.buttonRGBSave.UseVisualStyleBackColor = true;
            this.buttonRGBSave.Click += new System.EventHandler(this.buttonRGBSave_Click);
            // 
            // buttonRGBRead
            // 
            this.buttonRGBRead.Location = new System.Drawing.Point(13, 198);
            this.buttonRGBRead.Name = "buttonRGBRead";
            this.buttonRGBRead.Size = new System.Drawing.Size(75, 23);
            this.buttonRGBRead.TabIndex = 22;
            this.buttonRGBRead.Text = "RGB Read";
            this.buttonRGBRead.UseVisualStyleBackColor = true;
            this.buttonRGBRead.Click += new System.EventHandler(this.buttonRGBRead_Click);
            // 
            // buttonCamSetup
            // 
            this.buttonCamSetup.Location = new System.Drawing.Point(13, 145);
            this.buttonCamSetup.Name = "buttonCamSetup";
            this.buttonCamSetup.Size = new System.Drawing.Size(75, 23);
            this.buttonCamSetup.TabIndex = 23;
            this.buttonCamSetup.Text = "CamSetup";
            this.buttonCamSetup.UseVisualStyleBackColor = true;
            this.buttonCamSetup.Click += new System.EventHandler(this.buttonCamSetup_Click);
            // 
            // timerSave
            // 
            this.timerSave.Enabled = true;
            this.timerSave.Tick += new System.EventHandler(this.timerSave_Tick);
            // 
            // HW_Aligner_EBR_Edge
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.buttonCamSetup);
            this.Controls.Add(this.buttonRGBRead);
            this.Controls.Add(this.buttonRGBSave);
            this.Controls.Add(this.buttonInspect);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.checkView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HW_Aligner_EBR_Edge";
            this.Text = "HW_Aligner_EBR_Edge";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkView;
        private PropertyGridEx.PropertyGridEx grid;
        private System.Windows.Forms.Button buttonInspect;
        private System.Windows.Forms.Button buttonRGBSave;
        private System.Windows.Forms.Button buttonRGBRead;
        private System.Windows.Forms.Button buttonCamSetup;
        private System.Windows.Forms.Timer timerSave;
    }
}