namespace ezTools
{
    partial class LogView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogView));
            this.grid = new PropertyGridEx.PropertyGridEx();
            this.combo = new System.Windows.Forms.ComboBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.buttonPopup = new System.Windows.Forms.Button();
            this.tabControlLog = new System.Windows.Forms.TabControl();
            this.Total = new System.Windows.Forms.TabPage();
            this.listBox_total = new System.Windows.Forms.ListBox();
            this.tabControlLog.SuspendLayout();
            this.Total.SuspendLayout();
            this.SuspendLayout();
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
            this.grid.Location = new System.Drawing.Point(143, 54);
            this.grid.Name = "grid";
            this.grid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.grid.Size = new System.Drawing.Size(93, 145);
            this.grid.TabIndex = 4;
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
            // combo
            // 
            this.combo.FormattingEnabled = true;
            this.combo.Items.AddRange(new object[] {
            "Log",
            "Setup",
            "String"});
            this.combo.Location = new System.Drawing.Point(124, 14);
            this.combo.Name = "combo";
            this.combo.Size = new System.Drawing.Size(97, 20);
            this.combo.TabIndex = 5;
            this.combo.TabStop = false;
            this.combo.SelectedIndexChanged += new System.EventHandler(this.combo_SelectedIndexChanged);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 200;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // buttonPopup
            // 
            this.buttonPopup.Location = new System.Drawing.Point(12, 12);
            this.buttonPopup.Name = "buttonPopup";
            this.buttonPopup.Size = new System.Drawing.Size(92, 23);
            this.buttonPopup.TabIndex = 6;
            this.buttonPopup.Text = "Show Popup";
            this.buttonPopup.UseVisualStyleBackColor = true;
            this.buttonPopup.Click += new System.EventHandler(this.buttonPopup_Click);
            // 
            // tabControlLog
            // 
            this.tabControlLog.Controls.Add(this.Total);
            this.tabControlLog.Location = new System.Drawing.Point(12, 54);
            this.tabControlLog.Name = "tabControlLog";
            this.tabControlLog.SelectedIndex = 0;
            this.tabControlLog.Size = new System.Drawing.Size(125, 196);
            this.tabControlLog.TabIndex = 7;
            // 
            // Total
            // 
            this.Total.Controls.Add(this.listBox_total);
            this.Total.Location = new System.Drawing.Point(4, 22);
            this.Total.Name = "Total";
            this.Total.Padding = new System.Windows.Forms.Padding(3);
            this.Total.Size = new System.Drawing.Size(117, 170);
            this.Total.TabIndex = 0;
            this.Total.Text = "Total";
            this.Total.UseVisualStyleBackColor = true;
            // 
            // listBox_total
            // 
            this.listBox_total.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox_total.FormattingEnabled = true;
            this.listBox_total.HorizontalScrollbar = true;
            this.listBox_total.ItemHeight = 12;
            this.listBox_total.Location = new System.Drawing.Point(3, 3);
            this.listBox_total.Name = "listBox_total";
            this.listBox_total.Size = new System.Drawing.Size(111, 164);
            this.listBox_total.TabIndex = 0;
            // 
            // LogView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.tabControlLog);
            this.Controls.Add(this.buttonPopup);
            this.Controls.Add(this.combo);
            this.Controls.Add(this.grid);
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LogView";
            this.Text = "LogView";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogView_FormClosing);
            this.Resize += new System.EventHandler(this.LogView_Resize);
            this.tabControlLog.ResumeLayout(false);
            this.Total.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private PropertyGridEx.PropertyGridEx grid;
        private System.Windows.Forms.ComboBox combo;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button buttonPopup;
        private System.Windows.Forms.TabControl tabControlLog;
        private System.Windows.Forms.TabPage Total;
        private System.Windows.Forms.ListBox listBox_total;
    }
}