namespace ezAutoMom
{
    partial class XGem300Pro_Mom
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
            this.comboView = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioXGem_Close = new System.Windows.Forms.RadioButton();
            this.radioXGem_Stop = new System.Windows.Forms.RadioButton();
            this.radioXGem_Start = new System.Windows.Forms.RadioButton();
            this.radioXGem_Init = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkControl_Enable = new System.Windows.Forms.CheckBox();
            this.radioControl_Remote = new System.Windows.Forms.RadioButton();
            this.radioControl_Local = new System.Windows.Forms.RadioButton();
            this.radioControl_Offline = new System.Windows.Forms.RadioButton();
            this.grid = new PropertyGridEx.PropertyGridEx();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.groupTest = new System.Windows.Forms.GroupBox();
            this.comboTest = new System.Windows.Forms.ComboBox();
            this.tbTest = new System.Windows.Forms.TextBox();
            this.buttonTest = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupTest.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboView
            // 
            this.comboView.FormattingEnabled = true;
            this.comboView.Items.AddRange(new object[] {
            "SV",
            "CEID",
            "ALID",
            "Setup"});
            this.comboView.Location = new System.Drawing.Point(115, 12);
            this.comboView.Name = "comboView";
            this.comboView.Size = new System.Drawing.Size(82, 20);
            this.comboView.TabIndex = 14;
            this.comboView.SelectedIndexChanged += new System.EventHandler(this.comboView_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioXGem_Close);
            this.groupBox2.Controls.Add(this.radioXGem_Stop);
            this.groupBox2.Controls.Add(this.radioXGem_Start);
            this.groupBox2.Controls.Add(this.radioXGem_Init);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(97, 110);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "XGem State";
            // 
            // radioXGem_Close
            // 
            this.radioXGem_Close.AutoSize = true;
            this.radioXGem_Close.Location = new System.Drawing.Point(6, 86);
            this.radioXGem_Close.Name = "radioXGem_Close";
            this.radioXGem_Close.Size = new System.Drawing.Size(56, 16);
            this.radioXGem_Close.TabIndex = 7;
            this.radioXGem_Close.TabStop = true;
            this.radioXGem_Close.Text = "Close";
            this.radioXGem_Close.UseVisualStyleBackColor = true;
            // 
            // radioXGem_Stop
            // 
            this.radioXGem_Stop.AutoSize = true;
            this.radioXGem_Stop.Location = new System.Drawing.Point(6, 64);
            this.radioXGem_Stop.Name = "radioXGem_Stop";
            this.radioXGem_Stop.Size = new System.Drawing.Size(48, 16);
            this.radioXGem_Stop.TabIndex = 6;
            this.radioXGem_Stop.TabStop = true;
            this.radioXGem_Stop.Text = "Stop";
            this.radioXGem_Stop.UseVisualStyleBackColor = true;
            // 
            // radioXGem_Start
            // 
            this.radioXGem_Start.AutoSize = true;
            this.radioXGem_Start.Location = new System.Drawing.Point(6, 42);
            this.radioXGem_Start.Name = "radioXGem_Start";
            this.radioXGem_Start.Size = new System.Drawing.Size(48, 16);
            this.radioXGem_Start.TabIndex = 5;
            this.radioXGem_Start.TabStop = true;
            this.radioXGem_Start.Text = "Start";
            this.radioXGem_Start.UseVisualStyleBackColor = true;
            // 
            // radioXGem_Init
            // 
            this.radioXGem_Init.AutoSize = true;
            this.radioXGem_Init.Location = new System.Drawing.Point(6, 20);
            this.radioXGem_Init.Name = "radioXGem_Init";
            this.radioXGem_Init.Size = new System.Drawing.Size(39, 16);
            this.radioXGem_Init.TabIndex = 4;
            this.radioXGem_Init.TabStop = true;
            this.radioXGem_Init.Text = "Init";
            this.radioXGem_Init.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkControl_Enable);
            this.groupBox1.Controls.Add(this.radioControl_Remote);
            this.groupBox1.Controls.Add(this.radioControl_Local);
            this.groupBox1.Controls.Add(this.radioControl_Offline);
            this.groupBox1.Location = new System.Drawing.Point(12, 128);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(97, 114);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Control State";
            // 
            // checkControl_Enable
            // 
            this.checkControl_Enable.AutoSize = true;
            this.checkControl_Enable.Location = new System.Drawing.Point(6, 20);
            this.checkControl_Enable.Name = "checkControl_Enable";
            this.checkControl_Enable.Size = new System.Drawing.Size(63, 16);
            this.checkControl_Enable.TabIndex = 3;
            this.checkControl_Enable.Text = "Enable";
            this.checkControl_Enable.UseVisualStyleBackColor = true;
            this.checkControl_Enable.CheckedChanged += new System.EventHandler(this.checkControl_Enable_CheckedChanged);
            // 
            // radioControl_Remote
            // 
            this.radioControl_Remote.AutoSize = true;
            this.radioControl_Remote.Location = new System.Drawing.Point(6, 90);
            this.radioControl_Remote.Name = "radioControl_Remote";
            this.radioControl_Remote.Size = new System.Drawing.Size(66, 16);
            this.radioControl_Remote.TabIndex = 2;
            this.radioControl_Remote.TabStop = true;
            this.radioControl_Remote.Text = "Remote";
            this.radioControl_Remote.UseVisualStyleBackColor = true;
            // 
            // radioControl_Local
            // 
            this.radioControl_Local.AutoSize = true;
            this.radioControl_Local.Location = new System.Drawing.Point(6, 68);
            this.radioControl_Local.Name = "radioControl_Local";
            this.radioControl_Local.Size = new System.Drawing.Size(54, 16);
            this.radioControl_Local.TabIndex = 1;
            this.radioControl_Local.TabStop = true;
            this.radioControl_Local.Text = "Local";
            this.radioControl_Local.UseVisualStyleBackColor = true;
            // 
            // radioControl_Offline
            // 
            this.radioControl_Offline.AutoSize = true;
            this.radioControl_Offline.Location = new System.Drawing.Point(6, 46);
            this.radioControl_Offline.Name = "radioControl_Offline";
            this.radioControl_Offline.Size = new System.Drawing.Size(58, 16);
            this.radioControl_Offline.TabIndex = 0;
            this.radioControl_Offline.TabStop = true;
            this.radioControl_Offline.Text = "Offline";
            this.radioControl_Offline.UseVisualStyleBackColor = true;
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
            this.grid.DocCommentDescription.Size = new System.Drawing.Size(340, 36);
            this.grid.DocCommentDescription.TabIndex = 1;
            this.grid.DocCommentImage = null;
            // 
            // 
            // 
            this.grid.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.grid.DocCommentTitle.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.grid.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.grid.DocCommentTitle.Name = "";
            this.grid.DocCommentTitle.Size = new System.Drawing.Size(340, 16);
            this.grid.DocCommentTitle.TabIndex = 0;
            this.grid.DocCommentTitle.UseMnemonic = false;
            this.grid.LineColor = System.Drawing.SystemColors.ControlDark;
            this.grid.Location = new System.Drawing.Point(115, 38);
            this.grid.Name = "grid";
            this.grid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.grid.Size = new System.Drawing.Size(346, 765);
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
            this.grid.ToolStrip.Size = new System.Drawing.Size(93, 25);
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
            // groupTest
            // 
            this.groupTest.Controls.Add(this.buttonTest);
            this.groupTest.Controls.Add(this.tbTest);
            this.groupTest.Controls.Add(this.comboTest);
            this.groupTest.Location = new System.Drawing.Point(12, 248);
            this.groupTest.Name = "groupTest";
            this.groupTest.Size = new System.Drawing.Size(97, 104);
            this.groupTest.TabIndex = 15;
            this.groupTest.TabStop = false;
            this.groupTest.Text = "Test";
            // 
            // comboTest
            // 
            this.comboTest.FormattingEnabled = true;
            this.comboTest.Location = new System.Drawing.Point(6, 20);
            this.comboTest.Name = "comboTest";
            this.comboTest.Size = new System.Drawing.Size(85, 20);
            this.comboTest.TabIndex = 16;
            // 
            // tbTest
            // 
            this.tbTest.Location = new System.Drawing.Point(6, 46);
            this.tbTest.Name = "tbTest";
            this.tbTest.Size = new System.Drawing.Size(85, 21);
            this.tbTest.TabIndex = 17;
            // 
            // buttonTest
            // 
            this.buttonTest.Location = new System.Drawing.Point(6, 73);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(85, 23);
            this.buttonTest.TabIndex = 16;
            this.buttonTest.Text = "Run";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // XGem300Pro_Mom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 815);
            this.Controls.Add(this.groupTest);
            this.Controls.Add(this.comboView);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grid);
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "XGem300Pro_Mom";
            this.Text = "XGem64_Mom";
            this.Resize += new System.EventHandler(this.XGem300Pro_Mom_Resize);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupTest.ResumeLayout(false);
            this.groupTest.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboView;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioXGem_Close;
        private System.Windows.Forms.RadioButton radioXGem_Stop;
        private System.Windows.Forms.RadioButton radioXGem_Start;
        private System.Windows.Forms.RadioButton radioXGem_Init;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkControl_Enable;
        private System.Windows.Forms.RadioButton radioControl_Remote;
        private System.Windows.Forms.RadioButton radioControl_Local;
        private System.Windows.Forms.RadioButton radioControl_Offline;
        private PropertyGridEx.PropertyGridEx grid;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.GroupBox groupTest;
        private System.Windows.Forms.Button buttonTest;
        protected System.Windows.Forms.TextBox tbTest;
        protected System.Windows.Forms.ComboBox comboTest;
    }
}