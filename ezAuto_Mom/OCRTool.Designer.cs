namespace ezAutoMom
{
    partial class OCRTool
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
            this.buttonInit = new System.Windows.Forms.Button();
            this.buttonLoadImg = new System.Windows.Forms.Button();
            this.buttonAnalysis = new System.Windows.Forms.Button();
            this.buttonLoadFont = new System.Windows.Forms.Button();
            this.grid = new PropertyGridEx.PropertyGridEx();
            this.listSegment = new System.Windows.Forms.ListView();
            this.Char = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Index = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonTrain = new System.Windows.Forms.Button();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonInit
            // 
            this.buttonInit.Location = new System.Drawing.Point(12, 12);
            this.buttonInit.Name = "buttonInit";
            this.buttonInit.Size = new System.Drawing.Size(75, 23);
            this.buttonInit.TabIndex = 0;
            this.buttonInit.Text = "Init Table";
            this.buttonInit.UseVisualStyleBackColor = true;
            this.buttonInit.Click += new System.EventHandler(this.buttonInit_Click);
            // 
            // buttonLoadImg
            // 
            this.buttonLoadImg.Location = new System.Drawing.Point(93, 12);
            this.buttonLoadImg.Name = "buttonLoadImg";
            this.buttonLoadImg.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadImg.TabIndex = 1;
            this.buttonLoadImg.Text = "Load Img";
            this.buttonLoadImg.UseVisualStyleBackColor = true;
            this.buttonLoadImg.Click += new System.EventHandler(this.buttonLoadImg_Click);
            // 
            // buttonAnalysis
            // 
            this.buttonAnalysis.Location = new System.Drawing.Point(174, 12);
            this.buttonAnalysis.Name = "buttonAnalysis";
            this.buttonAnalysis.Size = new System.Drawing.Size(75, 23);
            this.buttonAnalysis.TabIndex = 2;
            this.buttonAnalysis.Text = "Analysis";
            this.buttonAnalysis.UseVisualStyleBackColor = true;
            this.buttonAnalysis.Click += new System.EventHandler(this.buttonAnalysis_Click);
            // 
            // buttonLoadFont
            // 
            this.buttonLoadFont.Location = new System.Drawing.Point(255, 12);
            this.buttonLoadFont.Name = "buttonLoadFont";
            this.buttonLoadFont.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadFont.TabIndex = 3;
            this.buttonLoadFont.Text = "Load Font";
            this.buttonLoadFont.UseVisualStyleBackColor = true;
            this.buttonLoadFont.Click += new System.EventHandler(this.buttonLoadFont_Click);
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
            this.grid.DocCommentDescription.Size = new System.Drawing.Size(312, 36);
            this.grid.DocCommentDescription.TabIndex = 1;
            this.grid.DocCommentImage = null;
            // 
            // 
            // 
            this.grid.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.grid.DocCommentTitle.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.grid.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.grid.DocCommentTitle.Name = "";
            this.grid.DocCommentTitle.Size = new System.Drawing.Size(312, 16);
            this.grid.DocCommentTitle.TabIndex = 0;
            this.grid.DocCommentTitle.UseMnemonic = false;
            this.grid.Location = new System.Drawing.Point(12, 41);
            this.grid.Name = "grid";
            this.grid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.grid.Size = new System.Drawing.Size(318, 216);
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
            this.grid.ToolStrip.Size = new System.Drawing.Size(275, 25);
            this.grid.ToolStrip.TabIndex = 1;
            this.grid.ToolStrip.TabStop = true;
            this.grid.ToolStrip.Text = "PropertyGridToolBar";
            this.grid.ToolStrip.Visible = false;
            this.grid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.grid_PropertyValueChanged);
            // 
            // listSegment
            // 
            this.listSegment.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Char,
            this.Index});
            this.listSegment.LabelEdit = true;
            this.listSegment.Location = new System.Drawing.Point(12, 263);
            this.listSegment.Name = "listSegment";
            this.listSegment.Size = new System.Drawing.Size(208, 161);
            this.listSegment.TabIndex = 5;
            this.listSegment.UseCompatibleStateImageBehavior = false;
            this.listSegment.View = System.Windows.Forms.View.Details;
            this.listSegment.SelectedIndexChanged += new System.EventHandler(this.listSegment_SelectedIndexChanged);
            this.listSegment.Click += new System.EventHandler(this.listSegment_Click);
            this.listSegment.DoubleClick += new System.EventHandler(this.listSegment_DoubleClick);
            this.listSegment.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.listSegment_KeyPress);
            // 
            // Char
            // 
            this.Char.Text = "Char";
            this.Char.Width = 100;
            // 
            // Index
            // 
            this.Index.Text = "Index";
            this.Index.Width = 100;
            // 
            // buttonTrain
            // 
            this.buttonTrain.Location = new System.Drawing.Point(226, 292);
            this.buttonTrain.Name = "buttonTrain";
            this.buttonTrain.Size = new System.Drawing.Size(75, 23);
            this.buttonTrain.TabIndex = 6;
            this.buttonTrain.Text = "Train";
            this.buttonTrain.UseVisualStyleBackColor = true;
            this.buttonTrain.Click += new System.EventHandler(this.buttonTrain_Click);
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(226, 263);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonRemove.TabIndex = 7;
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
            // 
            // OCRTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 1019);
            this.ControlBox = false;
            this.Controls.Add(this.buttonRemove);
            this.Controls.Add(this.buttonTrain);
            this.Controls.Add(this.listSegment);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.buttonLoadFont);
            this.Controls.Add(this.buttonAnalysis);
            this.Controls.Add(this.buttonLoadImg);
            this.Controls.Add(this.buttonInit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "OCRTool";
            this.Text = "OCRTool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OCRTool_FormClosing);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.OCRTool_Paint);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonInit;
        private System.Windows.Forms.Button buttonLoadImg;
        private System.Windows.Forms.Button buttonAnalysis;
        private System.Windows.Forms.Button buttonLoadFont;
        private PropertyGridEx.PropertyGridEx grid;
        private System.Windows.Forms.ListView listSegment;
        private System.Windows.Forms.ColumnHeader Index;
        private System.Windows.Forms.ColumnHeader Char;
        private System.Windows.Forms.Button buttonTrain;
        private System.Windows.Forms.Button buttonRemove;
    }
}