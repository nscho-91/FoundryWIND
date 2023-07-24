namespace ezTools
{
    partial class MarsLog
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
            this.timerSave = new System.Windows.Forms.Timer(this.components);
            this.tabControlLog = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listBoxTotal = new System.Windows.Forms.ListBox();
            this.combo = new System.Windows.Forms.ComboBox();
            this.grid = new PropertyGridEx.PropertyGridEx();
            this.btnClear = new System.Windows.Forms.Button();
            this.tabControlLog.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerSave
            // 
            this.timerSave.Enabled = true;
            this.timerSave.Interval = 50;
            this.timerSave.Tick += new System.EventHandler(this.timerSave_Tick);
            // 
            // tabControlLog
            // 
            this.tabControlLog.Controls.Add(this.tabPage1);
            this.tabControlLog.Location = new System.Drawing.Point(12, 38);
            this.tabControlLog.Name = "tabControlLog";
            this.tabControlLog.SelectedIndex = 0;
            this.tabControlLog.Size = new System.Drawing.Size(229, 430);
            this.tabControlLog.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listBoxTotal);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(221, 404);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Total";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listBoxTotal
            // 
            this.listBoxTotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxTotal.FormattingEnabled = true;
            this.listBoxTotal.HorizontalScrollbar = true;
            this.listBoxTotal.ItemHeight = 12;
            this.listBoxTotal.Location = new System.Drawing.Point(3, 3);
            this.listBoxTotal.Name = "listBoxTotal";
            this.listBoxTotal.Size = new System.Drawing.Size(215, 398);
            this.listBoxTotal.TabIndex = 0;
            // 
            // combo
            // 
            this.combo.FormattingEnabled = true;
            this.combo.Items.AddRange(new object[] {
            "Log",
            "Setup"});
            this.combo.Location = new System.Drawing.Point(12, 12);
            this.combo.Name = "combo";
            this.combo.Size = new System.Drawing.Size(97, 20);
            this.combo.TabIndex = 6;
            this.combo.TabStop = false;
            this.combo.SelectedIndexChanged += new System.EventHandler(this.combo_SelectedIndexChanged);
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
            this.grid.DocCommentDescription.Size = new System.Drawing.Size(87, 36);
            this.grid.DocCommentDescription.TabIndex = 1;
            this.grid.DocCommentImage = null;
            // 
            // 
            // 
            this.grid.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.grid.DocCommentTitle.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.grid.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.grid.DocCommentTitle.Name = "";
            this.grid.DocCommentTitle.Size = new System.Drawing.Size(87, 16);
            this.grid.DocCommentTitle.TabIndex = 0;
            this.grid.DocCommentTitle.UseMnemonic = false;
            this.grid.Location = new System.Drawing.Point(247, 60);
            this.grid.Name = "grid";
            this.grid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.grid.Size = new System.Drawing.Size(93, 145);
            this.grid.TabIndex = 7;
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
            this.grid.ToolStrip.Size = new System.Drawing.Size(93, 25);
            this.grid.ToolStrip.TabIndex = 1;
            this.grid.ToolStrip.TabStop = true;
            this.grid.ToolStrip.Text = "PropertyGridToolBar";
            this.grid.ToolStrip.Visible = false;
            this.grid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.grid_PropertyValueChanged);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(162, 9);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 8;
            this.btnClear.Text = "Log Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // MarsLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 480);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.combo);
            this.Controls.Add(this.tabControlLog);
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "MarsLog";
            this.Text = "MarsLog";
            this.Resize += new System.EventHandler(this.MarsLog_Resize);
            this.tabControlLog.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerSave;
        private System.Windows.Forms.TabControl tabControlLog;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ComboBox combo;
        private PropertyGridEx.PropertyGridEx grid;
        private System.Windows.Forms.ListBox listBoxTotal;
        private System.Windows.Forms.Button btnClear;
    }
}