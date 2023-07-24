namespace ezAutoMom
{
    partial class ImageEdit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageEdit));
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonRedo = new System.Windows.Forms.Button();
            this.grid = new PropertyGridEx.PropertyGridEx();
            this.comboEdit = new System.Windows.Forms.ComboBox();
            this.groupOutput = new System.Windows.Forms.GroupBox();
            this.labelResult = new System.Windows.Forms.Label();
            this.groupAdvanced = new System.Windows.Forms.GroupBox();
            this.buttonApply2 = new System.Windows.Forms.Button();
            this.comboAdvanced = new System.Windows.Forms.ComboBox();
            this.groupOutput.SuspendLayout();
            this.groupAdvanced.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(139, 9);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 1;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // buttonRedo
            // 
            this.buttonRedo.Location = new System.Drawing.Point(220, 9);
            this.buttonRedo.Name = "buttonRedo";
            this.buttonRedo.Size = new System.Drawing.Size(75, 23);
            this.buttonRedo.TabIndex = 2;
            this.buttonRedo.Text = "Redo";
            this.buttonRedo.UseVisualStyleBackColor = true;
            this.buttonRedo.Click += new System.EventHandler(this.buttonRedo_Click);
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
            this.grid.DocCommentDescription.Size = new System.Drawing.Size(269, 36);
            this.grid.DocCommentDescription.TabIndex = 1;
            this.grid.DocCommentImage = null;
            // 
            // 
            // 
            this.grid.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.grid.DocCommentTitle.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.grid.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.grid.DocCommentTitle.Name = "";
            this.grid.DocCommentTitle.Size = new System.Drawing.Size(269, 16);
            this.grid.DocCommentTitle.TabIndex = 0;
            this.grid.DocCommentTitle.UseMnemonic = false;
            this.grid.Location = new System.Drawing.Point(12, 38);
            this.grid.Name = "grid";
            this.grid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.grid.Size = new System.Drawing.Size(275, 308);
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
            this.grid.ToolStrip.Size = new System.Drawing.Size(275, 25);
            this.grid.ToolStrip.TabIndex = 1;
            this.grid.ToolStrip.TabStop = true;
            this.grid.ToolStrip.Text = "PropertyGridToolBar";
            this.grid.ToolStrip.Visible = false;
            this.grid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.grid_PropertyValueChanged);
            // 
            // comboEdit
            // 
            this.comboEdit.FormattingEnabled = true;
            this.comboEdit.Items.AddRange(new object[] {
            "None",
            "Drawing",
            "CheckGV",
            "SaveROI",
            "Overlay",
            "Binarization",
            "Rotation",
            "Shifting",
            "Resize",
            "Mophology",
            "AddNoise",
            "MatrixCode",
            "OCR",
            "1DBarcode",
            "ZXingBCR",
            "Measurement",
            "CountBlob",
            "CountUnit"});
            this.comboEdit.Location = new System.Drawing.Point(12, 12);
            this.comboEdit.Name = "comboEdit";
            this.comboEdit.Size = new System.Drawing.Size(121, 20);
            this.comboEdit.TabIndex = 4;
            this.comboEdit.SelectedIndexChanged += new System.EventHandler(this.comboEdit_SelectedIndexChanged);
            // 
            // groupOutput
            // 
            this.groupOutput.Controls.Add(this.labelResult);
            this.groupOutput.Location = new System.Drawing.Point(12, 375);
            this.groupOutput.Name = "groupOutput";
            this.groupOutput.Size = new System.Drawing.Size(273, 46);
            this.groupOutput.TabIndex = 5;
            this.groupOutput.TabStop = false;
            this.groupOutput.Text = "Output";
            // 
            // labelResult
            // 
            this.labelResult.AutoSize = true;
            this.labelResult.Location = new System.Drawing.Point(6, 17);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(52, 12);
            this.labelResult.TabIndex = 0;
            this.labelResult.Text = "Result : ";
            // 
            // groupAdvanced
            // 
            this.groupAdvanced.Controls.Add(this.buttonApply2);
            this.groupAdvanced.Controls.Add(this.comboAdvanced);
            this.groupAdvanced.Location = new System.Drawing.Point(12, 427);
            this.groupAdvanced.Name = "groupAdvanced";
            this.groupAdvanced.Size = new System.Drawing.Size(273, 48);
            this.groupAdvanced.TabIndex = 6;
            this.groupAdvanced.TabStop = false;
            this.groupAdvanced.Text = "Advanced Tools";
            // 
            // buttonApply2
            // 
            this.buttonApply2.Location = new System.Drawing.Point(135, 16);
            this.buttonApply2.Name = "buttonApply2";
            this.buttonApply2.Size = new System.Drawing.Size(75, 23);
            this.buttonApply2.TabIndex = 1;
            this.buttonApply2.Text = "Apply";
            this.buttonApply2.UseVisualStyleBackColor = true;
            this.buttonApply2.Click += new System.EventHandler(this.buttonApply2_Click);
            // 
            // comboAdvanced
            // 
            this.comboAdvanced.FormattingEnabled = true;
            this.comboAdvanced.Items.AddRange(new object[] {
            "None",
            "OCRTool"});
            this.comboAdvanced.Location = new System.Drawing.Point(8, 19);
            this.comboAdvanced.Name = "comboAdvanced";
            this.comboAdvanced.Size = new System.Drawing.Size(121, 20);
            this.comboAdvanced.TabIndex = 0;
            this.comboAdvanced.SelectedIndexChanged += new System.EventHandler(this.comboAdvanced_SelectedIndexChanged);
            // 
            // ImageEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(673, 507);
            this.Controls.Add(this.groupAdvanced);
            this.Controls.Add(this.groupOutput);
            this.Controls.Add(this.comboEdit);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.buttonRedo);
            this.Controls.Add(this.buttonApply);
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ImageEdit";
            this.Text = "ImageEdit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImageEdit_FormClosing);
            this.Resize += new System.EventHandler(this.ImageEdit_Resize);
            this.groupOutput.ResumeLayout(false);
            this.groupOutput.PerformLayout();
            this.groupAdvanced.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Button buttonRedo;
        private PropertyGridEx.PropertyGridEx grid;
        private System.Windows.Forms.ComboBox comboEdit;
        private System.Windows.Forms.GroupBox groupOutput;
        private System.Windows.Forms.GroupBox groupAdvanced;
        private System.Windows.Forms.Button buttonApply2;
        private System.Windows.Forms.ComboBox comboAdvanced;
        private System.Windows.Forms.Label labelResult;
    }
}