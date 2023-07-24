namespace ezAutoMom
{
    partial class HW_UnloadEV
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
            this.buttonUnload = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.grid = new PropertyGridEx.PropertyGridEx();
            this.SuspendLayout();
            // 
            // checkView
            // 
            this.checkView.AutoSize = true;
            this.checkView.Location = new System.Drawing.Point(12, 12);
            this.checkView.Name = "checkView";
            this.checkView.Size = new System.Drawing.Size(112, 16);
            this.checkView.TabIndex = 1;
            this.checkView.Text = "Unload Elevator";
            this.checkView.UseVisualStyleBackColor = true;
            this.checkView.CheckedChanged += new System.EventHandler(this.checkView_CheckedChanged);
            // 
            // buttonUnload
            // 
            this.buttonUnload.Location = new System.Drawing.Point(12, 34);
            this.buttonUnload.Name = "buttonUnload";
            this.buttonUnload.Size = new System.Drawing.Size(75, 23);
            this.buttonUnload.TabIndex = 2;
            this.buttonUnload.Text = "Unload";
            this.buttonUnload.UseVisualStyleBackColor = true;
            this.buttonUnload.Click += new System.EventHandler(this.buttonUnload_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(12, 92);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 5;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonDown
            // 
            this.buttonDown.Location = new System.Drawing.Point(12, 63);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(75, 23);
            this.buttonDown.TabIndex = 4;
            this.buttonDown.Text = "Down";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
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
            this.grid.Location = new System.Drawing.Point(93, 34);
            this.grid.Name = "grid";
            this.grid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.grid.Size = new System.Drawing.Size(179, 216);
            this.grid.TabIndex = 6;
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
            this.grid.ToolStrip.Size = new System.Drawing.Size(179, 25);
            this.grid.ToolStrip.TabIndex = 1;
            this.grid.ToolStrip.TabStop = true;
            this.grid.ToolStrip.Text = "PropertyGridToolBar";
            this.grid.ToolStrip.Visible = false;
            this.grid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.grid_PropertyValueChanged);
            // 
            // HW_UnloadEV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonDown);
            this.Controls.Add(this.buttonUnload);
            this.Controls.Add(this.checkView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HW_UnloadEV";
            this.Text = "HW_UnloadEV";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkView;
        private System.Windows.Forms.Button buttonUnload;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonDown;
        private PropertyGridEx.PropertyGridEx grid;
    }
}