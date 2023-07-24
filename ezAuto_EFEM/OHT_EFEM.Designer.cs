namespace ezAuto_EFEM
{
    partial class OHT_EFEM
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
            this.MainCycleTimer = new System.Windows.Forms.Timer(this.components);
            this.lbState = new System.Windows.Forms.Label();
            this.lbdetail = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.lbStateLP1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.gbloadport2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutLP2 = new System.Windows.Forms.TableLayoutPanel();
            this.gbloadport1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutLP1 = new System.Windows.Forms.TableLayoutPanel();
            this.bCkLP2 = new System.Windows.Forms.Button();
            this.bCkLP1 = new System.Windows.Forms.Button();
            this.bLP2UnLoad = new System.Windows.Forms.Button();
            this.btnLP1Reset = new System.Windows.Forms.Button();
            this.bLP2Load = new System.Windows.Forms.Button();
            this.btnLP2Reset = new System.Windows.Forms.Button();
            this.bTPDelay = new System.Windows.Forms.Button();
            this.bLP1UnLoad = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbErrorList = new System.Windows.Forms.ListBox();
            this.bLP1Load = new System.Windows.Forms.Button();
            this.bLP1Status = new System.Windows.Forms.Button();
            this.lbStateLP2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button4 = new System.Windows.Forms.Button();
            this.gbloadport3 = new System.Windows.Forms.GroupBox();
            this.tableLayoutLP3 = new System.Windows.Forms.TableLayoutPanel();
            this.bLP2Status = new System.Windows.Forms.Button();
            this.bLP3Status = new System.Windows.Forms.Button();
            this.bLP3Load = new System.Windows.Forms.Button();
            this.bLP3UnLoad = new System.Windows.Forms.Button();
            this.bCkLP3 = new System.Windows.Forms.Button();
            this.lbStateLP3 = new System.Windows.Forms.Label();
            this.btnLP3Reset = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.gbloadport2.SuspendLayout();
            this.gbloadport1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.gbloadport3.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainCycleTimer
            // 
            this.MainCycleTimer.Enabled = true;
            this.MainCycleTimer.Tick += new System.EventHandler(this.MainCycleTimer_Tick_1);
            // 
            // lbState
            // 
            this.lbState.AutoSize = true;
            this.lbState.Font = new System.Drawing.Font("Gulim", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbState.Location = new System.Drawing.Point(929, 253);
            this.lbState.Name = "lbState";
            this.lbState.Size = new System.Drawing.Size(0, 64);
            this.lbState.TabIndex = 26;
            // 
            // lbdetail
            // 
            this.lbdetail.AutoSize = true;
            this.lbdetail.Font = new System.Drawing.Font("Gulim", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbdetail.Location = new System.Drawing.Point(935, 341);
            this.lbdetail.Name = "lbdetail";
            this.lbdetail.Size = new System.Drawing.Size(0, 27);
            this.lbdetail.TabIndex = 27;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(21, 11);
            this.button1.TabIndex = 36;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lbStateLP1
            // 
            this.lbStateLP1.BackColor = System.Drawing.Color.PaleGreen;
            this.tableLayoutPanel1.SetColumnSpan(this.lbStateLP1, 2);
            this.lbStateLP1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbStateLP1.Location = new System.Drawing.Point(6, 578);
            this.lbStateLP1.Margin = new System.Windows.Forms.Padding(6);
            this.lbStateLP1.Name = "lbStateLP1";
            this.lbStateLP1.Size = new System.Drawing.Size(224, 28);
            this.lbStateLP1.TabIndex = 34;
            this.lbStateLP1.Text = "lbStateLP1";
            this.lbStateLP1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button2
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.button2, 2);
            this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button2.Location = new System.Drawing.Point(475, 765);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(233, 34);
            this.button2.TabIndex = 29;
            this.button2.Text = "Reset Enable";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // gbloadport2
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.gbloadport2, 2);
            this.gbloadport2.Controls.Add(this.tableLayoutLP2);
            this.gbloadport2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbloadport2.Location = new System.Drawing.Point(239, 281);
            this.gbloadport2.Name = "gbloadport2";
            this.gbloadport2.Size = new System.Drawing.Size(230, 288);
            this.gbloadport2.TabIndex = 3;
            this.gbloadport2.TabStop = false;
            this.gbloadport2.Text = "Loadport2";
            // 
            // tableLayoutLP2
            // 
            this.tableLayoutLP2.ColumnCount = 2;
            this.tableLayoutLP2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutLP2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutLP2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutLP2.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutLP2.Name = "tableLayoutLP2";
            this.tableLayoutLP2.RowCount = 8;
            this.tableLayoutLP2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP2.Size = new System.Drawing.Size(224, 268);
            this.tableLayoutLP2.TabIndex = 1;
            // 
            // gbloadport1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.gbloadport1, 2);
            this.gbloadport1.Controls.Add(this.tableLayoutLP1);
            this.gbloadport1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbloadport1.Location = new System.Drawing.Point(3, 281);
            this.gbloadport1.Name = "gbloadport1";
            this.gbloadport1.Size = new System.Drawing.Size(230, 288);
            this.gbloadport1.TabIndex = 2;
            this.gbloadport1.TabStop = false;
            this.gbloadport1.Text = "Loadport1";
            // 
            // tableLayoutLP1
            // 
            this.tableLayoutLP1.ColumnCount = 2;
            this.tableLayoutLP1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutLP1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutLP1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutLP1.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutLP1.Name = "tableLayoutLP1";
            this.tableLayoutLP1.RowCount = 8;
            this.tableLayoutLP1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP1.Size = new System.Drawing.Size(224, 268);
            this.tableLayoutLP1.TabIndex = 0;
            // 
            // bCkLP2
            // 
            this.bCkLP2.BackColor = System.Drawing.SystemColors.Control;
            this.bCkLP2.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Cassette;
            this.bCkLP2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tableLayoutPanel1.SetColumnSpan(this.bCkLP2, 2);
            this.bCkLP2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bCkLP2.Location = new System.Drawing.Point(239, 191);
            this.bCkLP2.Name = "bCkLP2";
            this.bCkLP2.Size = new System.Drawing.Size(230, 84);
            this.bCkLP2.TabIndex = 12;
            this.bCkLP2.UseVisualStyleBackColor = false;
            this.bCkLP2.Click += new System.EventHandler(this.bCkLP2_Click);
            // 
            // bCkLP1
            // 
            this.bCkLP1.BackColor = System.Drawing.SystemColors.Control;
            this.bCkLP1.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Loadport;
            this.bCkLP1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tableLayoutPanel1.SetColumnSpan(this.bCkLP1, 2);
            this.bCkLP1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bCkLP1.Location = new System.Drawing.Point(3, 191);
            this.bCkLP1.Name = "bCkLP1";
            this.bCkLP1.Size = new System.Drawing.Size(230, 84);
            this.bCkLP1.TabIndex = 11;
            this.bCkLP1.UseVisualStyleBackColor = false;
            this.bCkLP1.Click += new System.EventHandler(this.bCkLP1_Click);
            // 
            // bLP2UnLoad
            // 
            this.bLP2UnLoad.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Unload;
            this.bLP2UnLoad.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bLP2UnLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bLP2UnLoad.Enabled = false;
            this.bLP2UnLoad.Location = new System.Drawing.Point(357, 77);
            this.bLP2UnLoad.Name = "bLP2UnLoad";
            this.bLP2UnLoad.Size = new System.Drawing.Size(112, 108);
            this.bLP2UnLoad.TabIndex = 8;
            this.bLP2UnLoad.UseVisualStyleBackColor = true;
            // 
            // btnLP1Reset
            // 
            this.btnLP1Reset.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button;
            this.btnLP1Reset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel1.SetColumnSpan(this.btnLP1Reset, 2);
            this.btnLP1Reset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLP1Reset.Enabled = false;
            this.btnLP1Reset.Location = new System.Drawing.Point(3, 615);
            this.btnLP1Reset.Name = "btnLP1Reset";
            this.btnLP1Reset.Size = new System.Drawing.Size(230, 34);
            this.btnLP1Reset.TabIndex = 28;
            this.btnLP1Reset.Text = "Retry Key";
            this.btnLP1Reset.UseVisualStyleBackColor = true;
            this.btnLP1Reset.Click += new System.EventHandler(this.btnLP1Reset_Click);
            this.btnLP1Reset.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnLP1Reset_MouseDown);
            this.btnLP1Reset.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnLP1Reset_MouseUp);
            // 
            // bLP2Load
            // 
            this.bLP2Load.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Load;
            this.bLP2Load.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bLP2Load.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bLP2Load.Enabled = false;
            this.bLP2Load.Location = new System.Drawing.Point(239, 77);
            this.bLP2Load.Name = "bLP2Load";
            this.bLP2Load.Size = new System.Drawing.Size(112, 108);
            this.bLP2Load.TabIndex = 7;
            this.bLP2Load.Tag = "1";
            this.bLP2Load.UseVisualStyleBackColor = true;
            // 
            // btnLP2Reset
            // 
            this.btnLP2Reset.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button;
            this.btnLP2Reset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel1.SetColumnSpan(this.btnLP2Reset, 2);
            this.btnLP2Reset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLP2Reset.Enabled = false;
            this.btnLP2Reset.Location = new System.Drawing.Point(239, 615);
            this.btnLP2Reset.Name = "btnLP2Reset";
            this.btnLP2Reset.Size = new System.Drawing.Size(230, 34);
            this.btnLP2Reset.TabIndex = 30;
            this.btnLP2Reset.Text = "Retry Key";
            this.btnLP2Reset.UseVisualStyleBackColor = true;
            this.btnLP2Reset.Click += new System.EventHandler(this.btnLP2Reset_Click);
            this.btnLP2Reset.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnLP2Reset_MouseDown);
            this.btnLP2Reset.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnLP2Reset_MouseUp);
            // 
            // bTPDelay
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.bTPDelay, 2);
            this.bTPDelay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bTPDelay.Location = new System.Drawing.Point(239, 765);
            this.bTPDelay.Name = "bTPDelay";
            this.bTPDelay.Size = new System.Drawing.Size(230, 34);
            this.bTPDelay.TabIndex = 25;
            this.bTPDelay.Text = "적용";
            this.bTPDelay.UseVisualStyleBackColor = true;
            this.bTPDelay.Click += new System.EventHandler(this.bTPDelay_Click);
            // 
            // bLP1UnLoad
            // 
            this.bLP1UnLoad.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Unload;
            this.bLP1UnLoad.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bLP1UnLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bLP1UnLoad.Enabled = false;
            this.bLP1UnLoad.Location = new System.Drawing.Point(121, 77);
            this.bLP1UnLoad.Name = "bLP1UnLoad";
            this.bLP1UnLoad.Size = new System.Drawing.Size(112, 108);
            this.bLP1UnLoad.TabIndex = 6;
            this.bLP1UnLoad.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 6);
            this.groupBox1.Controls.Add(this.lbErrorList);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 655);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(705, 104);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ErrorList";
            // 
            // lbErrorList
            // 
            this.lbErrorList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbErrorList.FormattingEnabled = true;
            this.lbErrorList.ItemHeight = 12;
            this.lbErrorList.Location = new System.Drawing.Point(3, 17);
            this.lbErrorList.Name = "lbErrorList";
            this.lbErrorList.Size = new System.Drawing.Size(699, 84);
            this.lbErrorList.TabIndex = 32;
            // 
            // bLP1Load
            // 
            this.bLP1Load.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Load;
            this.bLP1Load.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bLP1Load.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bLP1Load.Enabled = false;
            this.bLP1Load.Location = new System.Drawing.Point(3, 77);
            this.bLP1Load.Name = "bLP1Load";
            this.bLP1Load.Size = new System.Drawing.Size(112, 108);
            this.bLP1Load.TabIndex = 5;
            this.bLP1Load.Tag = "";
            this.bLP1Load.UseVisualStyleBackColor = true;
            // 
            // bLP1Status
            // 
            this.bLP1Status.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button;
            this.bLP1Status.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel1.SetColumnSpan(this.bLP1Status, 2);
            this.bLP1Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bLP1Status.Location = new System.Drawing.Point(3, 3);
            this.bLP1Status.Name = "bLP1Status";
            this.bLP1Status.Size = new System.Drawing.Size(230, 68);
            this.bLP1Status.TabIndex = 9;
            this.bLP1Status.Text = "Manual";
            this.bLP1Status.UseVisualStyleBackColor = true;
            this.bLP1Status.Click += new System.EventHandler(this.bLP1Status_Click);
            this.bLP1Status.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bLP1Status_MouseDown);
            this.bLP1Status.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bLP1Status_MouseUp);
            // 
            // lbStateLP2
            // 
            this.lbStateLP2.BackColor = System.Drawing.Color.PaleGreen;
            this.tableLayoutPanel1.SetColumnSpan(this.lbStateLP2, 2);
            this.lbStateLP2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbStateLP2.Location = new System.Drawing.Point(242, 578);
            this.lbStateLP2.Margin = new System.Windows.Forms.Padding(6);
            this.lbStateLP2.Name = "lbStateLP2";
            this.lbStateLP2.Size = new System.Drawing.Size(224, 28);
            this.lbStateLP2.TabIndex = 35;
            this.lbStateLP2.Text = "lbStateLP2";
            this.lbStateLP2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.Controls.Add(this.gbloadport3, 4, 3);
            this.tableLayoutPanel1.Controls.Add(this.lbStateLP2, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.bLP1Status, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.bLP2Status, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.bLP1Load, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.bLP1UnLoad, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.bTPDelay, 2, 7);
            this.tableLayoutPanel1.Controls.Add(this.btnLP2Reset, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.bLP2Load, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnLP1Reset, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.bLP2UnLoad, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.bCkLP1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.bCkLP2, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.gbloadport1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.gbloadport2, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.button2, 3, 7);
            this.tableLayoutPanel1.Controls.Add(this.lbStateLP1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.bLP3Status, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.bLP3Load, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.bLP3UnLoad, 5, 1);
            this.tableLayoutPanel1.Controls.Add(this.bCkLP3, 4, 2);
            this.tableLayoutPanel1.Controls.Add(this.lbStateLP3, 4, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnLP3Reset, 4, 5);
            this.tableLayoutPanel1.Controls.Add(this.button4, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 7);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 74F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 114F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(711, 802);
            this.tableLayoutPanel1.TabIndex = 35;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(121, 765);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(21, 11);
            this.button4.TabIndex = 37;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Visible = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // gbloadport3
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.gbloadport3, 2);
            this.gbloadport3.Controls.Add(this.tableLayoutLP3);
            this.gbloadport3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbloadport3.Location = new System.Drawing.Point(475, 281);
            this.gbloadport3.Name = "gbloadport3";
            this.gbloadport3.Size = new System.Drawing.Size(233, 288);
            this.gbloadport3.TabIndex = 1;
            this.gbloadport3.TabStop = false;
            this.gbloadport3.Text = "Loadport3";
            // 
            // tableLayoutLP3
            // 
            this.tableLayoutLP3.ColumnCount = 2;
            this.tableLayoutLP3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutLP3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutLP3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutLP3.Location = new System.Drawing.Point(3, 17);
            this.tableLayoutLP3.Name = "tableLayoutLP3";
            this.tableLayoutLP3.RowCount = 8;
            this.tableLayoutLP3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutLP3.Size = new System.Drawing.Size(227, 268);
            this.tableLayoutLP3.TabIndex = 1;
            // 
            // bLP2Status
            // 
            this.bLP2Status.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button;
            this.bLP2Status.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel1.SetColumnSpan(this.bLP2Status, 2);
            this.bLP2Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bLP2Status.Location = new System.Drawing.Point(239, 3);
            this.bLP2Status.Name = "bLP2Status";
            this.bLP2Status.Size = new System.Drawing.Size(230, 68);
            this.bLP2Status.TabIndex = 10;
            this.bLP2Status.Text = "Manual";
            this.bLP2Status.UseVisualStyleBackColor = true;
            this.bLP2Status.Click += new System.EventHandler(this.bLP2Status_Click);
            this.bLP2Status.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bLP2Status_MouseDown);
            this.bLP2Status.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bLP2Status_MouseUp);
            // 
            // bLP3Status
            // 
            this.bLP3Status.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button;
            this.bLP3Status.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel1.SetColumnSpan(this.bLP3Status, 2);
            this.bLP3Status.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bLP3Status.Location = new System.Drawing.Point(475, 3);
            this.bLP3Status.Name = "bLP3Status";
            this.bLP3Status.Size = new System.Drawing.Size(233, 68);
            this.bLP3Status.TabIndex = 36;
            this.bLP3Status.Text = "Manual";
            this.bLP3Status.UseVisualStyleBackColor = true;
            this.bLP3Status.Click += new System.EventHandler(this.bLP3Status_Click);
            this.bLP3Status.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bLP3Status_MouseDown);
            this.bLP3Status.MouseUp += new System.Windows.Forms.MouseEventHandler(this.bLP3Status_MouseUp);
            // 
            // bLP3Load
            // 
            this.bLP3Load.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Load;
            this.bLP3Load.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bLP3Load.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bLP3Load.Location = new System.Drawing.Point(475, 77);
            this.bLP3Load.Name = "bLP3Load";
            this.bLP3Load.Size = new System.Drawing.Size(112, 108);
            this.bLP3Load.TabIndex = 37;
            this.bLP3Load.UseVisualStyleBackColor = true;
            // 
            // bLP3UnLoad
            // 
            this.bLP3UnLoad.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Unload;
            this.bLP3UnLoad.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bLP3UnLoad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bLP3UnLoad.Location = new System.Drawing.Point(593, 77);
            this.bLP3UnLoad.Name = "bLP3UnLoad";
            this.bLP3UnLoad.Size = new System.Drawing.Size(115, 108);
            this.bLP3UnLoad.TabIndex = 38;
            this.bLP3UnLoad.UseVisualStyleBackColor = true;
            // 
            // bCkLP3
            // 
            this.bCkLP3.BackColor = System.Drawing.SystemColors.Control;
            this.bCkLP3.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Cassette;
            this.bCkLP3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tableLayoutPanel1.SetColumnSpan(this.bCkLP3, 2);
            this.bCkLP3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bCkLP3.Location = new System.Drawing.Point(475, 191);
            this.bCkLP3.Name = "bCkLP3";
            this.bCkLP3.Size = new System.Drawing.Size(233, 84);
            this.bCkLP3.TabIndex = 39;
            this.bCkLP3.UseVisualStyleBackColor = false;
            this.bCkLP3.Click += new System.EventHandler(this.bCkLP3_Click);
            // 
            // lbStateLP3
            // 
            this.lbStateLP3.BackColor = System.Drawing.Color.PaleGreen;
            this.tableLayoutPanel1.SetColumnSpan(this.lbStateLP3, 2);
            this.lbStateLP3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbStateLP3.Location = new System.Drawing.Point(478, 578);
            this.lbStateLP3.Margin = new System.Windows.Forms.Padding(6);
            this.lbStateLP3.Name = "lbStateLP3";
            this.lbStateLP3.Size = new System.Drawing.Size(227, 28);
            this.lbStateLP3.TabIndex = 40;
            this.lbStateLP3.Text = "lbStateLP3";
            this.lbStateLP3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnLP3Reset
            // 
            this.btnLP3Reset.BackgroundImage = global::ezAuto_EFEM.Properties.Resources.Button;
            this.btnLP3Reset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel1.SetColumnSpan(this.btnLP3Reset, 2);
            this.btnLP3Reset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnLP3Reset.Enabled = false;
            this.btnLP3Reset.Location = new System.Drawing.Point(475, 615);
            this.btnLP3Reset.Name = "btnLP3Reset";
            this.btnLP3Reset.Size = new System.Drawing.Size(233, 34);
            this.btnLP3Reset.TabIndex = 41;
            this.btnLP3Reset.Text = "Retry Key";
            this.btnLP3Reset.UseVisualStyleBackColor = true;
            this.btnLP3Reset.Click += new System.EventHandler(this.btnLP3Reset_Click);
            this.btnLP3Reset.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnLP3Reset_MouseDown);
            this.btnLP3Reset.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnLP3Reset_MouseUp);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("GulimChe", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label1.Location = new System.Drawing.Point(3, 762);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 40);
            this.label1.TabIndex = 42;
            this.label1.Text = "OHT2.1";
            // 
            // OHT_EFEM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(711, 802);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.lbdetail);
            this.Controls.Add(this.lbState);
            this.Font = new System.Drawing.Font("Gulim", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "OHT_EFEM";
            this.Text = "OHT";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OHT_EFEM_FormClosing_1);
            this.gbloadport2.ResumeLayout(false);
            this.gbloadport1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.gbloadport3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer MainCycleTimer;
        private System.Windows.Forms.Label lbState;
        private System.Windows.Forms.Label lbdetail;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lbStateLP1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox gbloadport3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutLP3;
        private System.Windows.Forms.Label lbStateLP2;
        private System.Windows.Forms.Button bLP1Status;
        private System.Windows.Forms.Button bLP2Status;
        private System.Windows.Forms.Button bLP1Load;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lbErrorList;
        private System.Windows.Forms.Button bLP1UnLoad;
        private System.Windows.Forms.Button bTPDelay;
        private System.Windows.Forms.Button btnLP2Reset;
        private System.Windows.Forms.Button bLP2Load;
        private System.Windows.Forms.Button btnLP1Reset;
        private System.Windows.Forms.Button bLP2UnLoad;
        private System.Windows.Forms.Button bCkLP1;
        private System.Windows.Forms.Button bCkLP2;
        private System.Windows.Forms.GroupBox gbloadport1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutLP1;
        private System.Windows.Forms.GroupBox gbloadport2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutLP2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button bLP3Status;
        private System.Windows.Forms.Button bLP3Load;
        private System.Windows.Forms.Button bLP3UnLoad;
        private System.Windows.Forms.Button bCkLP3;
        private System.Windows.Forms.Label lbStateLP3;
        private System.Windows.Forms.Button btnLP3Reset;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label1;
    }
}