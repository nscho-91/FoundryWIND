namespace ezAutoMom
{
    partial class Work_Mom
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Work_Mom));
            this.buttonRunStart = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonRunCycle = new System.Windows.Forms.Button();
            this.buttonRunNew = new System.Windows.Forms.Button();
            this.buttonRunHome = new System.Windows.Forms.Button();
            this.buttonRunReset = new System.Windows.Forms.Button();
            this.buttonRunPause = new System.Windows.Forms.Button();
            this.labelModel = new System.Windows.Forms.Label();
            this.grid = new PropertyGridEx.PropertyGridEx();
            this.timerInvalid = new System.Windows.Forms.Timer(this.components);
            this.timerEnable = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonRunStart
            // 
            this.buttonRunStart.Image = ((System.Drawing.Image)(resources.GetObject("buttonRunStart.Image")));
            this.buttonRunStart.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonRunStart.Location = new System.Drawing.Point(6, 20);
            this.buttonRunStart.Name = "buttonRunStart";
            this.buttonRunStart.Size = new System.Drawing.Size(60, 23);
            this.buttonRunStart.TabIndex = 0;
            this.buttonRunStart.Text = "Start";
            this.buttonRunStart.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonRunStart.UseVisualStyleBackColor = true;
            this.buttonRunStart.Click += new System.EventHandler(this.buttonRunStart_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonRunCycle);
            this.groupBox1.Controls.Add(this.buttonRunNew);
            this.groupBox1.Controls.Add(this.buttonRunHome);
            this.groupBox1.Controls.Add(this.buttonRunReset);
            this.groupBox1.Controls.Add(this.buttonRunPause);
            this.groupBox1.Controls.Add(this.buttonRunStart);
            this.groupBox1.Location = new System.Drawing.Point(12, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(72, 195);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Run";
            // 
            // buttonRunCycle
            // 
            this.buttonRunCycle.Image = ((System.Drawing.Image)(resources.GetObject("buttonRunCycle.Image")));
            this.buttonRunCycle.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonRunCycle.Location = new System.Drawing.Point(6, 165);
            this.buttonRunCycle.Name = "buttonRunCycle";
            this.buttonRunCycle.Size = new System.Drawing.Size(60, 23);
            this.buttonRunCycle.TabIndex = 5;
            this.buttonRunCycle.Text = "Cycle";
            this.buttonRunCycle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonRunCycle.UseVisualStyleBackColor = true;
            this.buttonRunCycle.Click += new System.EventHandler(this.buttonRunCycle_Click);
            // 
            // buttonRunNew
            // 
            this.buttonRunNew.Image = ((System.Drawing.Image)(resources.GetObject("buttonRunNew.Image")));
            this.buttonRunNew.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonRunNew.Location = new System.Drawing.Point(6, 136);
            this.buttonRunNew.Name = "buttonRunNew";
            this.buttonRunNew.Size = new System.Drawing.Size(60, 23);
            this.buttonRunNew.TabIndex = 4;
            this.buttonRunNew.Text = "New";
            this.buttonRunNew.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonRunNew.UseVisualStyleBackColor = true;
            this.buttonRunNew.Click += new System.EventHandler(this.buttonRunNew_Click);
            // 
            // buttonRunHome
            // 
            this.buttonRunHome.Image = ((System.Drawing.Image)(resources.GetObject("buttonRunHome.Image")));
            this.buttonRunHome.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonRunHome.Location = new System.Drawing.Point(6, 107);
            this.buttonRunHome.Name = "buttonRunHome";
            this.buttonRunHome.Size = new System.Drawing.Size(60, 23);
            this.buttonRunHome.TabIndex = 3;
            this.buttonRunHome.Text = "Home";
            this.buttonRunHome.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonRunHome.UseVisualStyleBackColor = true;
            this.buttonRunHome.Click += new System.EventHandler(this.buttonRunHome_Click);
            // 
            // buttonRunReset
            // 
            this.buttonRunReset.Image = ((System.Drawing.Image)(resources.GetObject("buttonRunReset.Image")));
            this.buttonRunReset.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonRunReset.Location = new System.Drawing.Point(6, 78);
            this.buttonRunReset.Name = "buttonRunReset";
            this.buttonRunReset.Size = new System.Drawing.Size(60, 23);
            this.buttonRunReset.TabIndex = 2;
            this.buttonRunReset.Text = "Reset";
            this.buttonRunReset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonRunReset.UseVisualStyleBackColor = true;
            this.buttonRunReset.Click += new System.EventHandler(this.buttonRunReset_Click);
            // 
            // buttonRunPause
            // 
            this.buttonRunPause.Image = ((System.Drawing.Image)(resources.GetObject("buttonRunPause.Image")));
            this.buttonRunPause.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonRunPause.Location = new System.Drawing.Point(6, 49);
            this.buttonRunPause.Name = "buttonRunPause";
            this.buttonRunPause.Size = new System.Drawing.Size(60, 23);
            this.buttonRunPause.TabIndex = 1;
            this.buttonRunPause.Text = "Stop";
            this.buttonRunPause.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonRunPause.UseVisualStyleBackColor = true;
            this.buttonRunPause.Click += new System.EventHandler(this.buttonRunPause_Click);
            // 
            // labelModel
            // 
            this.labelModel.AutoSize = true;
            this.labelModel.Location = new System.Drawing.Point(16, 9);
            this.labelModel.Name = "labelModel";
            this.labelModel.Size = new System.Drawing.Size(40, 12);
            this.labelModel.TabIndex = 2;
            this.labelModel.Text = "Model";
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
            this.grid.DocCommentDescription.Size = new System.Drawing.Size(281, 36);
            this.grid.DocCommentDescription.TabIndex = 1;
            this.grid.DocCommentImage = null;
            // 
            // 
            // 
            this.grid.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.grid.DocCommentTitle.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.grid.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.grid.DocCommentTitle.Name = "";
            this.grid.DocCommentTitle.Size = new System.Drawing.Size(281, 16);
            this.grid.DocCommentTitle.TabIndex = 0;
            this.grid.DocCommentTitle.UseMnemonic = false;
            this.grid.Location = new System.Drawing.Point(90, 35);
            this.grid.Name = "grid";
            this.grid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.grid.Size = new System.Drawing.Size(287, 777);
            this.grid.TabIndex = 3;
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
            this.grid.ToolStrip.Size = new System.Drawing.Size(152, 23);
            this.grid.ToolStrip.TabIndex = 1;
            this.grid.ToolStrip.TabStop = true;
            this.grid.ToolStrip.Text = "PropertyGridToolBar";
            this.grid.ToolStrip.Visible = false;
            this.grid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.grid_PropertyValueChanged);
            // 
            // timerInvalid
            // 
            this.timerInvalid.Enabled = true;
            this.timerInvalid.Interval = 10;
            this.timerInvalid.Tick += new System.EventHandler(this.timerInvalid_Tick);
            // 
            // timerEnable
            // 
            this.timerEnable.Enabled = true;
            this.timerEnable.Interval = 1000;
            this.timerEnable.Tick += new System.EventHandler(this.timerEnable_Tick);
            // 
            // Work_Mom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 824);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.labelModel);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Work_Mom";
            this.Text = "Work_Mom";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Work_Mom_FormClosing);
            this.Resize += new System.EventHandler(this.Work_Mom_Resize);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonRunStart;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonRunCycle;
        private System.Windows.Forms.Button buttonRunNew;
        private System.Windows.Forms.Button buttonRunHome;
        private System.Windows.Forms.Button buttonRunReset;
        private System.Windows.Forms.Button buttonRunPause;
        private System.Windows.Forms.Label labelModel;
        private PropertyGridEx.PropertyGridEx grid;
        private System.Windows.Forms.Timer timerInvalid;
        private System.Windows.Forms.Timer timerEnable;
    }
}