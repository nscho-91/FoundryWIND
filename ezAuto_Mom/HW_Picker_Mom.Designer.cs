namespace ezAutoMom
{
    partial class HW_Picker_Mom
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
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonUnload = new System.Windows.Forms.Button();
            this.labelDI_Up = new System.Windows.Forms.Label();
            this.labelDI_Down = new System.Windows.Forms.Label();
            this.labelDI_Vac0 = new System.Windows.Forms.Label();
            this.labelDI_Vac1 = new System.Windows.Forms.Label();
            this.labelDO_Vac1 = new System.Windows.Forms.Label();
            this.labelDO_Vac0 = new System.Windows.Forms.Label();
            this.labelDO_Blow = new System.Windows.Forms.Label();
            this.labelDO_Down = new System.Windows.Forms.Label();
            this.grid = new PropertyGridEx.PropertyGridEx();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // checkView
            // 
            this.checkView.AutoSize = true;
            this.checkView.Location = new System.Drawing.Point(12, 12);
            this.checkView.Name = "checkView";
            this.checkView.Size = new System.Drawing.Size(59, 16);
            this.checkView.TabIndex = 0;
            this.checkView.Text = "Picker";
            this.checkView.UseVisualStyleBackColor = true;
            this.checkView.CheckedChanged += new System.EventHandler(this.checkView_CheckedChanged);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(12, 34);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(75, 23);
            this.buttonLoad.TabIndex = 1;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonUnload
            // 
            this.buttonUnload.Location = new System.Drawing.Point(12, 63);
            this.buttonUnload.Name = "buttonUnload";
            this.buttonUnload.Size = new System.Drawing.Size(75, 23);
            this.buttonUnload.TabIndex = 2;
            this.buttonUnload.Text = "Unload";
            this.buttonUnload.UseVisualStyleBackColor = true;
            this.buttonUnload.Click += new System.EventHandler(this.buttonUnload_Click);
            // 
            // labelDI_Up
            // 
            this.labelDI_Up.AutoSize = true;
            this.labelDI_Up.Location = new System.Drawing.Point(109, 39);
            this.labelDI_Up.Name = "labelDI_Up";
            this.labelDI_Up.Size = new System.Drawing.Size(37, 12);
            this.labelDI_Up.TabIndex = 3;
            this.labelDI_Up.Text = "DI_Up";
            // 
            // labelDI_Down
            // 
            this.labelDI_Down.AutoSize = true;
            this.labelDI_Down.Location = new System.Drawing.Point(109, 59);
            this.labelDI_Down.Name = "labelDI_Down";
            this.labelDI_Down.Size = new System.Drawing.Size(54, 12);
            this.labelDI_Down.TabIndex = 4;
            this.labelDI_Down.Text = "DI_Down";
            // 
            // labelDI_Vac0
            // 
            this.labelDI_Vac0.AutoSize = true;
            this.labelDI_Vac0.Location = new System.Drawing.Point(109, 79);
            this.labelDI_Vac0.Name = "labelDI_Vac0";
            this.labelDI_Vac0.Size = new System.Drawing.Size(50, 12);
            this.labelDI_Vac0.TabIndex = 5;
            this.labelDI_Vac0.Text = "DI_Vac0";
            // 
            // labelDI_Vac1
            // 
            this.labelDI_Vac1.AutoSize = true;
            this.labelDI_Vac1.Location = new System.Drawing.Point(109, 99);
            this.labelDI_Vac1.Name = "labelDI_Vac1";
            this.labelDI_Vac1.Size = new System.Drawing.Size(50, 12);
            this.labelDI_Vac1.TabIndex = 6;
            this.labelDI_Vac1.Text = "DI_Vac1";
            // 
            // labelDO_Vac1
            // 
            this.labelDO_Vac1.AutoSize = true;
            this.labelDO_Vac1.Location = new System.Drawing.Point(185, 99);
            this.labelDO_Vac1.Name = "labelDO_Vac1";
            this.labelDO_Vac1.Size = new System.Drawing.Size(56, 12);
            this.labelDO_Vac1.TabIndex = 10;
            this.labelDO_Vac1.Text = "DO_Vac1";
            this.labelDO_Vac1.Click += new System.EventHandler(this.labelDO_Vac1_Click);
            // 
            // labelDO_Vac0
            // 
            this.labelDO_Vac0.AutoSize = true;
            this.labelDO_Vac0.Location = new System.Drawing.Point(185, 79);
            this.labelDO_Vac0.Name = "labelDO_Vac0";
            this.labelDO_Vac0.Size = new System.Drawing.Size(56, 12);
            this.labelDO_Vac0.TabIndex = 9;
            this.labelDO_Vac0.Text = "DO_Vac0";
            this.labelDO_Vac0.Click += new System.EventHandler(this.labelDO_Vac0_Click);
            // 
            // labelDO_Blow
            // 
            this.labelDO_Blow.AutoSize = true;
            this.labelDO_Blow.Location = new System.Drawing.Point(185, 59);
            this.labelDO_Blow.Name = "labelDO_Blow";
            this.labelDO_Blow.Size = new System.Drawing.Size(56, 12);
            this.labelDO_Blow.TabIndex = 8;
            this.labelDO_Blow.Text = "DO_Blow";
            this.labelDO_Blow.Click += new System.EventHandler(this.labelDO_Blow_Click);
            // 
            // labelDO_Down
            // 
            this.labelDO_Down.AutoSize = true;
            this.labelDO_Down.Location = new System.Drawing.Point(185, 39);
            this.labelDO_Down.Name = "labelDO_Down";
            this.labelDO_Down.Size = new System.Drawing.Size(60, 12);
            this.labelDO_Down.TabIndex = 7;
            this.labelDO_Down.Text = "DO_Down";
            this.labelDO_Down.Click += new System.EventHandler(this.labelDO_Down_Click);
            // 
            // grid
            // 
            this.grid.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
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
            this.grid.Location = new System.Drawing.Point(93, 119);
            this.grid.Name = "grid";
            this.grid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.grid.Size = new System.Drawing.Size(179, 199);
            this.grid.TabIndex = 11;
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
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 200;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // HW_Picker_Mom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(284, 339);
            this.ControlBox = false;
            this.Controls.Add(this.grid);
            this.Controls.Add(this.labelDO_Vac1);
            this.Controls.Add(this.labelDO_Vac0);
            this.Controls.Add(this.labelDO_Blow);
            this.Controls.Add(this.labelDO_Down);
            this.Controls.Add(this.labelDI_Vac1);
            this.Controls.Add(this.labelDI_Vac0);
            this.Controls.Add(this.labelDI_Down);
            this.Controls.Add(this.labelDI_Up);
            this.Controls.Add(this.buttonUnload);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.checkView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "HW_Picker_Mom";
            this.Text = "HW_Picker_Mom";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HW_Picker_Mom_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkView;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Button buttonUnload;
        private System.Windows.Forms.Label labelDI_Up;
        private System.Windows.Forms.Label labelDI_Down;
        private System.Windows.Forms.Label labelDI_Vac0;
        private System.Windows.Forms.Label labelDI_Vac1;
        private System.Windows.Forms.Label labelDO_Vac1;
        private System.Windows.Forms.Label labelDO_Vac0;
        private System.Windows.Forms.Label labelDO_Blow;
        private System.Windows.Forms.Label labelDO_Down;
        private PropertyGridEx.PropertyGridEx grid;
        private System.Windows.Forms.Timer timer;
    }
}