namespace ezAuto_EFEM
{
    partial class HW_Aligner_EBR_3D
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
            this.timerSave = new System.Windows.Forms.Timer(this.components);
            this.buttonInspect = new System.Windows.Forms.Button();
            this.buttonBufH = new System.Windows.Forms.Button();
            this.buttonBufB = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkView
            // 
            this.checkView.AutoSize = true;
            this.checkView.Location = new System.Drawing.Point(12, 12);
            this.checkView.Name = "checkView";
            this.checkView.Size = new System.Drawing.Size(38, 16);
            this.checkView.TabIndex = 16;
            this.checkView.Text = "3D";
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
            this.grid.TabIndex = 20;
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
            // timerSave
            // 
            this.timerSave.Enabled = true;
            this.timerSave.Tick += new System.EventHandler(this.timerSave_Tick);
            // 
            // buttonInspect
            // 
            this.buttonInspect.Location = new System.Drawing.Point(12, 34);
            this.buttonInspect.Name = "buttonInspect";
            this.buttonInspect.Size = new System.Drawing.Size(75, 23);
            this.buttonInspect.TabIndex = 21;
            this.buttonInspect.Text = "Inspect";
            this.buttonInspect.UseVisualStyleBackColor = true;
            this.buttonInspect.Click += new System.EventHandler(this.buttonInspect_Click);
            // 
            // buttonBufH
            // 
            this.buttonBufH.Location = new System.Drawing.Point(12, 98);
            this.buttonBufH.Name = "buttonBufH";
            this.buttonBufH.Size = new System.Drawing.Size(75, 23);
            this.buttonBufH.TabIndex = 22;
            this.buttonBufH.Text = "BufH";
            this.buttonBufH.UseVisualStyleBackColor = true;
            this.buttonBufH.Click += new System.EventHandler(this.buttonBufH_Click);
            // 
            // buttonBufB
            // 
            this.buttonBufB.Location = new System.Drawing.Point(12, 127);
            this.buttonBufB.Name = "buttonBufB";
            this.buttonBufB.Size = new System.Drawing.Size(75, 23);
            this.buttonBufB.TabIndex = 23;
            this.buttonBufB.Text = "BufB";
            this.buttonBufB.UseVisualStyleBackColor = true;
            this.buttonBufB.Click += new System.EventHandler(this.buttonBufB_Click);
            // 
            // HW_Aligner_EBR_3D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.buttonBufB);
            this.Controls.Add(this.buttonBufH);
            this.Controls.Add(this.buttonInspect);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.checkView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HW_Aligner_EBR_3D";
            this.Text = "HW_Aligner_EBR_3D";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkView;
        private PropertyGridEx.PropertyGridEx grid;
        private System.Windows.Forms.Timer timerSave;
        private System.Windows.Forms.Button buttonInspect;
        private System.Windows.Forms.Button buttonBufH;
        private System.Windows.Forms.Button buttonBufB;
    }
}