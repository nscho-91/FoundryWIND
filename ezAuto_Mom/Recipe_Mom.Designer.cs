namespace ezAutoMom
{
    partial class Recipe_Mom
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Recipe_Mom));
            this.grid_Recipe = new PropertyGridEx.PropertyGridEx();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnDelete = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.listViewRecipe = new System.Windows.Forms.ListView();
            this.columnHeaderName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderAngle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderMarkType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderModifyDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textBoxNameFilter = new System.Windows.Forms.TextBox();
            this.timerEnable = new System.Windows.Forms.Timer(this.components);
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grid_Recipe
            // 
            this.grid_Recipe.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.grid_Recipe.DocCommentDescription.AutoEllipsis = true;
            this.grid_Recipe.DocCommentDescription.Cursor = System.Windows.Forms.Cursors.Default;
            this.grid_Recipe.DocCommentDescription.Location = new System.Drawing.Point(3, 19);
            this.grid_Recipe.DocCommentDescription.Name = "";
            this.grid_Recipe.DocCommentDescription.Size = new System.Drawing.Size(373, 36);
            this.grid_Recipe.DocCommentDescription.TabIndex = 1;
            this.grid_Recipe.DocCommentImage = null;
            // 
            // 
            // 
            this.grid_Recipe.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.grid_Recipe.DocCommentTitle.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.grid_Recipe.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.grid_Recipe.DocCommentTitle.Name = "";
            this.grid_Recipe.DocCommentTitle.Size = new System.Drawing.Size(373, 16);
            this.grid_Recipe.DocCommentTitle.TabIndex = 0;
            this.grid_Recipe.DocCommentTitle.UseMnemonic = false;
            this.grid_Recipe.LineColor = System.Drawing.SystemColors.ControlDark;
            this.grid_Recipe.Location = new System.Drawing.Point(483, 3);
            this.grid_Recipe.Name = "grid_Recipe";
            this.grid_Recipe.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.grid_Recipe.Size = new System.Drawing.Size(379, 405);
            this.grid_Recipe.TabIndex = 20;
            this.grid_Recipe.ToolbarVisible = false;
            // 
            // 
            // 
            this.grid_Recipe.ToolStrip.AccessibleName = "도구 모음";
            this.grid_Recipe.ToolStrip.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            this.grid_Recipe.ToolStrip.AllowMerge = false;
            this.grid_Recipe.ToolStrip.AutoSize = false;
            this.grid_Recipe.ToolStrip.CanOverflow = false;
            this.grid_Recipe.ToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.grid_Recipe.ToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.grid_Recipe.ToolStrip.Location = new System.Drawing.Point(0, 1);
            this.grid_Recipe.ToolStrip.Name = "";
            this.grid_Recipe.ToolStrip.Padding = new System.Windows.Forms.Padding(2, 0, 1, 0);
            this.grid_Recipe.ToolStrip.Size = new System.Drawing.Size(240, 18);
            this.grid_Recipe.ToolStrip.TabIndex = 1;
            this.grid_Recipe.ToolStrip.TabStop = true;
            this.grid_Recipe.ToolStrip.Text = "PropertyGridToolBar";
            this.grid_Recipe.ToolStrip.Visible = false;
            this.grid_Recipe.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.grid_Recipe_PropertyValueChanged);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(859, 162);
            this.panel1.TabIndex = 21;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.Turquoise;
            this.panel2.Controls.Add(this.btnDelete);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.btnRefresh);
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(862, 92);
            this.panel2.TabIndex = 24;
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnDelete.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.Navy;
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.Location = new System.Drawing.Point(592, 8);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(82, 63);
            this.btnDelete.TabIndex = 26;
            this.btnDelete.Text = "Delete";
            this.btnDelete.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(45, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 20);
            this.label1.TabIndex = 24;
            this.label1.Text = "Recipe Setting";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnRefresh.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.Navy;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.Location = new System.Drawing.Point(680, 8);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(82, 63);
            this.btnRefresh.TabIndex = 23;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnSave.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.Navy;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.Location = new System.Drawing.Point(768, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(82, 63);
            this.btnSave.TabIndex = 22;
            this.btnSave.Text = "Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // listViewRecipe
            // 
            this.listViewRecipe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listViewRecipe.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderName,
            this.columnHeaderSize,
            this.columnHeaderAngle,
            this.columnHeaderMarkType,
            this.columnHeaderModifyDate});
            this.listViewRecipe.FullRowSelect = true;
            this.listViewRecipe.Location = new System.Drawing.Point(3, 35);
            this.listViewRecipe.Name = "listViewRecipe";
            this.listViewRecipe.Size = new System.Drawing.Size(474, 373);
            this.listViewRecipe.TabIndex = 25;
            this.listViewRecipe.UseCompatibleStateImageBehavior = false;
            this.listViewRecipe.View = System.Windows.Forms.View.Details;
            this.listViewRecipe.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listViewRecipe_ColumnClick);
            this.listViewRecipe.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewRecipe_ItemSelectionChanged);
            this.listViewRecipe.Click += new System.EventHandler(this.listViewRecipe_Click);
            this.listViewRecipe.DoubleClick += new System.EventHandler(this.listViewRecipe_DoubleClick);
            this.listViewRecipe.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewRecipe_KeyDown);
            this.listViewRecipe.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listViewRecipe_MouseDown);
            // 
            // columnHeaderName
            // 
            this.columnHeaderName.Text = "Name";
            this.columnHeaderName.Width = 189;
            // 
            // columnHeaderSize
            // 
            this.columnHeaderSize.Text = "Size";
            this.columnHeaderSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // columnHeaderAngle
            // 
            this.columnHeaderAngle.Text = "Angle";
            this.columnHeaderAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // columnHeaderMarkType
            // 
            this.columnHeaderMarkType.Text = "MarkType";
            this.columnHeaderMarkType.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderMarkType.Width = 80;
            // 
            // columnHeaderModifyDate
            // 
            this.columnHeaderModifyDate.Text = "ModifyDate";
            this.columnHeaderModifyDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderModifyDate.Width = 80;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 82);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textBoxNameFilter);
            this.splitContainer1.Panel1.Controls.Add(this.listViewRecipe);
            this.splitContainer1.Panel1.Controls.Add(this.grid_Recipe);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(859, 577);
            this.splitContainer1.SplitterDistance = 411;
            this.splitContainer1.TabIndex = 26;
            // 
            // textBoxNameFilter
            // 
            this.textBoxNameFilter.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.textBoxNameFilter.Location = new System.Drawing.Point(3, 3);
            this.textBoxNameFilter.Name = "textBoxNameFilter";
            this.textBoxNameFilter.Size = new System.Drawing.Size(474, 26);
            this.textBoxNameFilter.TabIndex = 26;
            this.textBoxNameFilter.TextChanged += new System.EventHandler(this.textBoxNameFilter_TextChanged);
            // 
            // timerEnable
            // 
            this.timerEnable.Tick += new System.EventHandler(this.timerEnable_Tick);
            // 
            // Recipe_Mom
            // 
            this.AutoHidePortion = 0.99D;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(858, 660);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Recipe_Mom";
            this.Text = "UI_RecipeSetting";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Recipe_Mom_FormClosing);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Recipe_Paint);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private PropertyGridEx.PropertyGridEx grid_Recipe;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ListView listViewRecipe;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnDelete;
        public System.Windows.Forms.TextBox textBoxNameFilter;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderSize;
        private System.Windows.Forms.ColumnHeader columnHeaderAngle;
        private System.Windows.Forms.ColumnHeader columnHeaderMarkType;
        private System.Windows.Forms.ColumnHeader columnHeaderModifyDate;
        private System.Windows.Forms.Timer timerEnable;
    }
}