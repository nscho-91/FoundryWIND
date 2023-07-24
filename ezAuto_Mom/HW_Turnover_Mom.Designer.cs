namespace ezAutoMom
{
    partial class HW_Turnover_Mom
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
            this.buttonRunTurn = new System.Windows.Forms.Button();
            this.buttonSetReady = new System.Windows.Forms.Button();
            this.buttonTurn0 = new System.Windows.Forms.Button();
            this.buttonTurn1 = new System.Windows.Forms.Button();
            this.grid = new PropertyGridEx.PropertyGridEx();
            this.SuspendLayout();
            // 
            // checkView
            // 
            this.checkView.AutoSize = true;
            this.checkView.Location = new System.Drawing.Point(12, 12);
            this.checkView.Name = "checkView";
            this.checkView.Size = new System.Drawing.Size(74, 16);
            this.checkView.TabIndex = 0;
            this.checkView.Text = "Turnover";
            this.checkView.UseVisualStyleBackColor = true;
            this.checkView.CheckedChanged += new System.EventHandler(this.checkView_CheckedChanged);
            // 
            // buttonRunTurn
            // 
            this.buttonRunTurn.Location = new System.Drawing.Point(12, 34);
            this.buttonRunTurn.Name = "buttonRunTurn";
            this.buttonRunTurn.Size = new System.Drawing.Size(75, 23);
            this.buttonRunTurn.TabIndex = 1;
            this.buttonRunTurn.Text = "Run Turn";
            this.buttonRunTurn.UseVisualStyleBackColor = true;
            this.buttonRunTurn.Click += new System.EventHandler(this.buttonRunTurn_Click);
            // 
            // buttonSetReady
            // 
            this.buttonSetReady.Location = new System.Drawing.Point(12, 63);
            this.buttonSetReady.Name = "buttonSetReady";
            this.buttonSetReady.Size = new System.Drawing.Size(75, 23);
            this.buttonSetReady.TabIndex = 2;
            this.buttonSetReady.Text = "Set Ready";
            this.buttonSetReady.UseVisualStyleBackColor = true;
            this.buttonSetReady.Click += new System.EventHandler(this.buttonSetReady_Click);
            // 
            // buttonTurn0
            // 
            this.buttonTurn0.Location = new System.Drawing.Point(12, 92);
            this.buttonTurn0.Name = "buttonTurn0";
            this.buttonTurn0.Size = new System.Drawing.Size(75, 23);
            this.buttonTurn0.TabIndex = 3;
            this.buttonTurn0.Text = "Turn0";
            this.buttonTurn0.UseVisualStyleBackColor = true;
            this.buttonTurn0.Click += new System.EventHandler(this.buttonTurn0_Click);
            // 
            // buttonTurn1
            // 
            this.buttonTurn1.Location = new System.Drawing.Point(12, 121);
            this.buttonTurn1.Name = "buttonTurn1";
            this.buttonTurn1.Size = new System.Drawing.Size(75, 23);
            this.buttonTurn1.TabIndex = 4;
            this.buttonTurn1.Text = "Turn1";
            this.buttonTurn1.UseVisualStyleBackColor = true;
            this.buttonTurn1.Click += new System.EventHandler(this.buttonTurn1_Click);
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
            this.grid.Location = new System.Drawing.Point(93, 34);
            this.grid.Name = "grid";
            this.grid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.grid.Size = new System.Drawing.Size(179, 216);
            this.grid.TabIndex = 5;
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
            this.grid.ToolStrip.Size = new System.Drawing.Size(195, 25);
            this.grid.ToolStrip.TabIndex = 1;
            this.grid.ToolStrip.TabStop = true;
            this.grid.ToolStrip.Text = "PropertyGridToolBar";
            this.grid.ToolStrip.Visible = false;
            this.grid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.grid_PropertyValueChanged);
            // 
            // HW_Turnover_Mom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(284, 265);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.buttonTurn1);
            this.Controls.Add(this.buttonTurn0);
            this.Controls.Add(this.buttonSetReady);
            this.Controls.Add(this.buttonRunTurn);
            this.Controls.Add(this.checkView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HW_Turnover_Mom";
            this.Text = "HW_Turnover_Mom";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HW_Turnover_Mom_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkView;
        private System.Windows.Forms.Button buttonRunTurn;
        private System.Windows.Forms.Button buttonSetReady;
        private System.Windows.Forms.Button buttonTurn0;
        private System.Windows.Forms.Button buttonTurn1;
        private PropertyGridEx.PropertyGridEx grid;
    }
}