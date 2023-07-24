namespace ezAutoMom
{
    partial class HW_LoadPort_Mom
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
            this.buttonMap = new System.Windows.Forms.Button();
            this.labelUnload = new System.Windows.Forms.Label();
            this.labelLoad = new System.Windows.Forms.Label();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.labelCoverClose = new System.Windows.Forms.Label();
            this.labelCoverOpen = new System.Windows.Forms.Label();
            this.buttonCover = new System.Windows.Forms.Button();
            this.grid = new PropertyGridEx.PropertyGridEx();
            this.checkView = new System.Windows.Forms.CheckBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.buttonHome = new System.Windows.Forms.Button();
            this.buttonReset = new System.Windows.Forms.Button();
            this.btnLiftUP = new System.Windows.Forms.Button();
            this.btnLiftDown = new System.Windows.Forms.Button();
            this.buttonLoadPortMove = new System.Windows.Forms.Button();
            this.comboSlot = new System.Windows.Forms.ComboBox();
            this.btn300Pos = new System.Windows.Forms.Button();
            this.btnRingPos = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonMap
            // 
            this.buttonMap.Location = new System.Drawing.Point(10, 92);
            this.buttonMap.Name = "buttonMap";
            this.buttonMap.Size = new System.Drawing.Size(75, 23);
            this.buttonMap.TabIndex = 22;
            this.buttonMap.Text = "Mapping";
            this.buttonMap.UseVisualStyleBackColor = true;
            this.buttonMap.Click += new System.EventHandler(this.buttonMap_Click);
            // 
            // labelUnload
            // 
            this.labelUnload.AutoSize = true;
            this.labelUnload.Location = new System.Drawing.Point(215, 39);
            this.labelUnload.Name = "labelUnload";
            this.labelUnload.Size = new System.Drawing.Size(44, 12);
            this.labelUnload.TabIndex = 21;
            this.labelUnload.Text = "Unload";
            // 
            // labelLoad
            // 
            this.labelLoad.AutoSize = true;
            this.labelLoad.Location = new System.Drawing.Point(176, 39);
            this.labelLoad.Name = "labelLoad";
            this.labelLoad.Size = new System.Drawing.Size(33, 12);
            this.labelLoad.TabIndex = 20;
            this.labelLoad.Text = "Load";
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(10, 63);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(75, 23);
            this.buttonLoad.TabIndex = 19;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // labelCoverClose
            // 
            this.labelCoverClose.AutoSize = true;
            this.labelCoverClose.Location = new System.Drawing.Point(132, 39);
            this.labelCoverClose.Name = "labelCoverClose";
            this.labelCoverClose.Size = new System.Drawing.Size(38, 12);
            this.labelCoverClose.TabIndex = 16;
            this.labelCoverClose.Text = "Close";
            // 
            // labelCoverOpen
            // 
            this.labelCoverOpen.AutoSize = true;
            this.labelCoverOpen.Location = new System.Drawing.Point(91, 39);
            this.labelCoverOpen.Name = "labelCoverOpen";
            this.labelCoverOpen.Size = new System.Drawing.Size(35, 12);
            this.labelCoverOpen.TabIndex = 15;
            this.labelCoverOpen.Text = "Open";
            // 
            // buttonCover
            // 
            this.buttonCover.Location = new System.Drawing.Point(10, 34);
            this.buttonCover.Name = "buttonCover";
            this.buttonCover.Size = new System.Drawing.Size(75, 23);
            this.buttonCover.TabIndex = 14;
            this.buttonCover.Text = "Cover";
            this.buttonCover.UseVisualStyleBackColor = true;
            this.buttonCover.Click += new System.EventHandler(this.buttonCover_Click);
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
            this.grid.DocCommentDescription.Size = new System.Drawing.Size(171, 36);
            this.grid.DocCommentDescription.TabIndex = 1;
            this.grid.DocCommentImage = null;
            // 
            // 
            // 
            this.grid.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.grid.DocCommentTitle.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.grid.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.grid.DocCommentTitle.Name = "";
            this.grid.DocCommentTitle.Size = new System.Drawing.Size(171, 16);
            this.grid.DocCommentTitle.TabIndex = 0;
            this.grid.DocCommentTitle.UseMnemonic = false;
            this.grid.Location = new System.Drawing.Point(91, 63);
            this.grid.Name = "grid";
            this.grid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.grid.Size = new System.Drawing.Size(177, 348);
            this.grid.TabIndex = 13;
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
            this.grid.Click += new System.EventHandler(this.grid_Click);
            // 
            // checkView
            // 
            this.checkView.AutoSize = true;
            this.checkView.Location = new System.Drawing.Point(12, 12);
            this.checkView.Name = "checkView";
            this.checkView.Size = new System.Drawing.Size(74, 16);
            this.checkView.TabIndex = 12;
            this.checkView.Text = "LoadPort";
            this.checkView.UseVisualStyleBackColor = true;
            this.checkView.CheckedChanged += new System.EventHandler(this.checkView_CheckedChanged);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 250;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // buttonHome
            // 
            this.buttonHome.Location = new System.Drawing.Point(10, 121);
            this.buttonHome.Name = "buttonHome";
            this.buttonHome.Size = new System.Drawing.Size(75, 23);
            this.buttonHome.TabIndex = 23;
            this.buttonHome.Text = "Home";
            this.buttonHome.UseVisualStyleBackColor = true;
            this.buttonHome.Click += new System.EventHandler(this.buttonHome_Click);
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(10, 150);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(75, 23);
            this.buttonReset.TabIndex = 24;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // btnLiftUP
            // 
            this.btnLiftUP.Location = new System.Drawing.Point(10, 179);
            this.btnLiftUP.Name = "btnLiftUP";
            this.btnLiftUP.Size = new System.Drawing.Size(75, 23);
            this.btnLiftUP.TabIndex = 25;
            this.btnLiftUP.Text = "Lift Up";
            this.btnLiftUP.UseVisualStyleBackColor = true;
            this.btnLiftUP.Click += new System.EventHandler(this.btnLiftUP_Click);
            // 
            // btnLiftDown
            // 
            this.btnLiftDown.Location = new System.Drawing.Point(10, 208);
            this.btnLiftDown.Name = "btnLiftDown";
            this.btnLiftDown.Size = new System.Drawing.Size(75, 23);
            this.btnLiftDown.TabIndex = 26;
            this.btnLiftDown.Text = "Lift Down";
            this.btnLiftDown.UseVisualStyleBackColor = true;
            this.btnLiftDown.Click += new System.EventHandler(this.btnLiftDown_Click);
            // 
            // buttonLoadPortMove
            // 
            this.buttonLoadPortMove.Location = new System.Drawing.Point(10, 263);
            this.buttonLoadPortMove.Name = "buttonLoadPortMove";
            this.buttonLoadPortMove.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadPortMove.TabIndex = 27;
            this.buttonLoadPortMove.Text = "Slot Move";
            this.buttonLoadPortMove.UseVisualStyleBackColor = true;
            this.buttonLoadPortMove.Click += new System.EventHandler(this.buttonLoadPortMove_Click);
            // 
            // comboSlot
            // 
            this.comboSlot.FormattingEnabled = true;
            this.comboSlot.Location = new System.Drawing.Point(10, 237);
            this.comboSlot.Name = "comboSlot";
            this.comboSlot.Size = new System.Drawing.Size(75, 20);
            this.comboSlot.TabIndex = 35;
            // 
            // btn300Pos
            // 
            this.btn300Pos.Location = new System.Drawing.Point(10, 292);
            this.btn300Pos.Name = "btn300Pos";
            this.btn300Pos.Size = new System.Drawing.Size(75, 23);
            this.btn300Pos.TabIndex = 36;
            this.btn300Pos.Text = "300Pos";
            this.btn300Pos.UseVisualStyleBackColor = true;
            this.btn300Pos.Click += new System.EventHandler(this.btn300Pos_Click);
            // 
            // btnRingPos
            // 
            this.btnRingPos.Location = new System.Drawing.Point(10, 321);
            this.btnRingPos.Name = "btnRingPos";
            this.btnRingPos.Size = new System.Drawing.Size(75, 23);
            this.btnRingPos.TabIndex = 37;
            this.btnRingPos.Text = "RingPos";
            this.btnRingPos.UseVisualStyleBackColor = true;
            this.btnRingPos.Click += new System.EventHandler(this.btnRingPos_Click);
            // 
            // HW_LoadPort_Mom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 423);
            this.Controls.Add(this.btnRingPos);
            this.Controls.Add(this.btn300Pos);
            this.Controls.Add(this.comboSlot);
            this.Controls.Add(this.buttonLoadPortMove);
            this.Controls.Add(this.btnLiftDown);
            this.Controls.Add(this.btnLiftUP);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.buttonHome);
            this.Controls.Add(this.buttonMap);
            this.Controls.Add(this.labelUnload);
            this.Controls.Add(this.labelLoad);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.labelCoverClose);
            this.Controls.Add(this.labelCoverOpen);
            this.Controls.Add(this.buttonCover);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.checkView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HW_LoadPort_Mom";
            this.Text = "HW_LoadPort";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonMap;
        private System.Windows.Forms.Label labelUnload;
        private System.Windows.Forms.Label labelLoad;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Label labelCoverClose;
        private System.Windows.Forms.Label labelCoverOpen;
        private System.Windows.Forms.Button buttonCover;
        private PropertyGridEx.PropertyGridEx grid;
        private System.Windows.Forms.CheckBox checkView;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button buttonHome;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Button btnLiftUP;
        private System.Windows.Forms.Button btnLiftDown;
        private System.Windows.Forms.Button buttonLoadPortMove;
        protected System.Windows.Forms.ComboBox comboSlot;
        private System.Windows.Forms.Button btn300Pos;
        private System.Windows.Forms.Button btnRingPos;
    }
}