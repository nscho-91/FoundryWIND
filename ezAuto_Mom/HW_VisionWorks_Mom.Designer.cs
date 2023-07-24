namespace ezAutoMom
{
    partial class HW_VisionWorks_Mom
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
            this.grid = new PropertyGridEx.PropertyGridEx();
            this.buttonUnload = new System.Windows.Forms.Button();
            this.buttonGetState = new System.Windows.Forms.Button();
            this.buttonHome = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonLotStart = new System.Windows.Forms.Button();
            this.buttonLotEnd = new System.Windows.Forms.Button();
            this.buttonLoadDone = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonInkChange = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkView
            // 
            this.checkView.AutoSize = true;
            this.checkView.Location = new System.Drawing.Point(12, 12);
            this.checkView.Name = "checkView";
            this.checkView.Size = new System.Drawing.Size(93, 16);
            this.checkView.TabIndex = 17;
            this.checkView.Text = "VisionWorks";
            this.checkView.UseVisualStyleBackColor = true;
            this.checkView.CheckedChanged += new System.EventHandler(this.checkView_CheckedChanged);
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(12, 34);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(75, 23);
            this.buttonLoad.TabIndex = 18;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
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
            this.grid.Location = new System.Drawing.Point(93, 34);
            this.grid.Name = "grid";
            this.grid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.grid.Size = new System.Drawing.Size(178, 282);
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
            // buttonUnload
            // 
            this.buttonUnload.Location = new System.Drawing.Point(12, 63);
            this.buttonUnload.Name = "buttonUnload";
            this.buttonUnload.Size = new System.Drawing.Size(75, 23);
            this.buttonUnload.TabIndex = 20;
            this.buttonUnload.Text = "Unload";
            this.buttonUnload.UseVisualStyleBackColor = true;
            this.buttonUnload.Click += new System.EventHandler(this.buttonUnload_Click);
            // 
            // buttonGetState
            // 
            this.buttonGetState.Location = new System.Drawing.Point(12, 92);
            this.buttonGetState.Name = "buttonGetState";
            this.buttonGetState.Size = new System.Drawing.Size(75, 23);
            this.buttonGetState.TabIndex = 21;
            this.buttonGetState.Text = "Get State";
            this.buttonGetState.UseVisualStyleBackColor = true;
            this.buttonGetState.Click += new System.EventHandler(this.buttonGetState_Click);
            // 
            // buttonHome
            // 
            this.buttonHome.Location = new System.Drawing.Point(12, 121);
            this.buttonHome.Name = "buttonHome";
            this.buttonHome.Size = new System.Drawing.Size(75, 23);
            this.buttonHome.TabIndex = 22;
            this.buttonHome.Text = "Home";
            this.buttonHome.UseVisualStyleBackColor = true;
            this.buttonHome.Click += new System.EventHandler(this.buttonHome_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(12, 150);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 23;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(12, 178);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(75, 23);
            this.buttonStop.TabIndex = 24;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonLotStart
            // 
            this.buttonLotStart.Location = new System.Drawing.Point(12, 207);
            this.buttonLotStart.Name = "buttonLotStart";
            this.buttonLotStart.Size = new System.Drawing.Size(75, 23);
            this.buttonLotStart.TabIndex = 25;
            this.buttonLotStart.Text = "LotStart";
            this.buttonLotStart.UseVisualStyleBackColor = true;
            this.buttonLotStart.Click += new System.EventHandler(this.buttonLotStart_Click);
            // 
            // buttonLotEnd
            // 
            this.buttonLotEnd.Location = new System.Drawing.Point(12, 235);
            this.buttonLotEnd.Name = "buttonLotEnd";
            this.buttonLotEnd.Size = new System.Drawing.Size(75, 23);
            this.buttonLotEnd.TabIndex = 26;
            this.buttonLotEnd.Text = "LotEnd";
            this.buttonLotEnd.UseVisualStyleBackColor = true;
            this.buttonLotEnd.Click += new System.EventHandler(this.buttonLotEnd_Click);
            // 
            // buttonLoadDone
            // 
            this.buttonLoadDone.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLoadDone.Location = new System.Drawing.Point(12, 264);
            this.buttonLoadDone.Name = "buttonLoadDone";
            this.buttonLoadDone.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadDone.TabIndex = 27;
            this.buttonLoadDone.Text = "LoadDone";
            this.buttonLoadDone.UseVisualStyleBackColor = true;
            this.buttonLoadDone.Click += new System.EventHandler(this.buttonLoadDone_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(12, 293);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 28;
            this.button1.Text = "UnLoadDone";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonInkChange
            // 
            this.buttonInkChange.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonInkChange.Location = new System.Drawing.Point(12, 322);
            this.buttonInkChange.Name = "buttonInkChange";
            this.buttonInkChange.Size = new System.Drawing.Size(75, 23);
            this.buttonInkChange.TabIndex = 29;
            this.buttonInkChange.Text = "Ink Change";
            this.buttonInkChange.UseVisualStyleBackColor = true;
            this.buttonInkChange.Click += new System.EventHandler(this.buttonInkChange_Click);
            // 
            // HW_VisionWorks_Mom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 351);
            this.Controls.Add(this.buttonInkChange);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonLoadDone);
            this.Controls.Add(this.buttonLotEnd);
            this.Controls.Add(this.buttonLotStart);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.buttonHome);
            this.Controls.Add(this.buttonGetState);
            this.Controls.Add(this.buttonUnload);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.checkView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HW_VisionWorks_Mom";
            this.Text = "HW_VisionWorks";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkView;
        private System.Windows.Forms.Button buttonLoad;
        private PropertyGridEx.PropertyGridEx grid;
        private System.Windows.Forms.Button buttonUnload;
        private System.Windows.Forms.Button buttonGetState;
        private System.Windows.Forms.Button buttonHome;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button buttonLotStart;
        private System.Windows.Forms.Button buttonLotEnd;
        private System.Windows.Forms.Button buttonLoadDone;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonInkChange;
    }
}