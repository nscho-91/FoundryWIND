using ezAutoMom;
namespace ezAuto_EFEM
{
    partial class UI_EFEM
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
            this.listViewJobLog = new System.Windows.Forms.ListView();
            this.No = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LoadPort = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.JobID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LotID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CarrierID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Recipe = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SlotMap = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.StartTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.EndTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.State = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelMonthDay = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.labelWPH = new System.Windows.Forms.Label();
            this.labelInspTime = new System.Windows.Forms.Label();
            this.panel12 = new System.Windows.Forms.Panel();
            this.btnBuzzerOff = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnRecovery = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnCycleStop = new System.Windows.Forms.Button();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.tableLayoutPanelLP = new System.Windows.Forms.TableLayoutPanel();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelUpperArm = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panelLowerArm = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panelAligner = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.labelStateStage = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.tableLayoutPanelFDC = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.lbEQPState = new System.Windows.Forms.Label();
            this.userControlOverViewLP1 = new ezAuto_EFEM.UserControlOverViewLP();
            this.userControlOverViewLP2 = new ezAuto_EFEM.UserControlOverViewLP();
            this.userControlOverViewLP3 = new ezAuto_EFEM.UserControlOverViewLP();
            this.IOStageLifterUp = new ezAuto_EFEM.UserControlIOIndicator();
            this.IOStageWaferExist = new ezAuto_EFEM.UserControlIOIndicator();
            this.WaferInfoStage = new ezAuto_EFEM.UserControlWaferInfo();
            this.IOStageVisionConnected = new ezAuto_EFEM.UserControlIOIndicator();
            this.WaferInfoAligner = new ezAuto_EFEM.UserControlWaferInfo();
            this.IOAlignerWaferExist = new ezAuto_EFEM.UserControlIOIndicator();
            this.WaferInfoLowerArm = new ezAuto_EFEM.UserControlWaferInfo();
            this.IOLowerArmWaferExist = new ezAuto_EFEM.UserControlIOIndicator();
            this.IOLowerArmSafty = new ezAuto_EFEM.UserControlIOIndicator();
            this.WaferInfoUpperArm = new ezAuto_EFEM.UserControlWaferInfo();
            this.IOUpperArmSafty = new ezAuto_EFEM.UserControlIOIndicator();
            this.IOUpperArmWaferExist = new ezAuto_EFEM.UserControlIOIndicator();
            this.panel12.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelUpperArm.SuspendLayout();
            this.panelLowerArm.SuspendLayout();
            this.panelAligner.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewJobLog
            // 
            this.listViewJobLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.No,
            this.LoadPort,
            this.JobID,
            this.LotID,
            this.CarrierID,
            this.Recipe,
            this.SlotMap,
            this.StartTime,
            this.EndTime,
            this.State});
            this.listViewJobLog.FullRowSelect = true;
            this.listViewJobLog.HideSelection = false;
            this.listViewJobLog.Location = new System.Drawing.Point(253, 12);
            this.listViewJobLog.Margin = new System.Windows.Forms.Padding(4);
            this.listViewJobLog.MultiSelect = false;
            this.listViewJobLog.Name = "listViewJobLog";
            this.listViewJobLog.Size = new System.Drawing.Size(996, 153);
            this.listViewJobLog.TabIndex = 7;
            this.listViewJobLog.UseCompatibleStateImageBehavior = false;
            this.listViewJobLog.View = System.Windows.Forms.View.Details;
            // 
            // No
            // 
            this.No.Text = "No.";
            this.No.Width = 40;
            // 
            // LoadPort
            // 
            this.LoadPort.Text = "LP";
            this.LoadPort.Width = 32;
            // 
            // JobID
            // 
            this.JobID.Text = "Job ID";
            this.JobID.Width = 57;
            // 
            // LotID
            // 
            this.LotID.Text = "Lot ID";
            this.LotID.Width = 91;
            // 
            // CarrierID
            // 
            this.CarrierID.Text = "Carrier ID";
            this.CarrierID.Width = 99;
            // 
            // Recipe
            // 
            this.Recipe.Text = "Recipe";
            this.Recipe.Width = 141;
            // 
            // SlotMap
            // 
            this.SlotMap.Text = "Slot Map";
            this.SlotMap.Width = 225;
            // 
            // StartTime
            // 
            this.StartTime.Text = "Start Time";
            this.StartTime.Width = 118;
            // 
            // EndTime
            // 
            this.EndTime.Text = "End Time";
            this.EndTime.Width = 110;
            // 
            // State
            // 
            this.State.Text = "State";
            this.State.Width = 80;
            // 
            // labelMonthDay
            // 
            this.labelMonthDay.AutoSize = true;
            this.labelMonthDay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelMonthDay.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelMonthDay.Location = new System.Drawing.Point(5, 1);
            this.labelMonthDay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMonthDay.Name = "labelMonthDay";
            this.labelMonthDay.Size = new System.Drawing.Size(224, 50);
            this.labelMonthDay.TabIndex = 9;
            this.labelMonthDay.Text = "04-04";
            this.labelMonthDay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTime.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelTime.Location = new System.Drawing.Point(5, 52);
            this.labelTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(224, 50);
            this.labelTime.TabIndex = 10;
            this.labelTime.Text = "13:13:13";
            this.labelTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelWPH
            // 
            this.labelWPH.AutoSize = true;
            this.labelWPH.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelWPH.Location = new System.Drawing.Point(5, 103);
            this.labelWPH.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelWPH.Name = "labelWPH";
            this.labelWPH.Size = new System.Drawing.Size(224, 28);
            this.labelWPH.TabIndex = 11;
            this.labelWPH.Text = "WPH : - / h";
            this.labelWPH.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelInspTime
            // 
            this.labelInspTime.AutoSize = true;
            this.labelInspTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelInspTime.Location = new System.Drawing.Point(5, 132);
            this.labelInspTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelInspTime.Name = "labelInspTime";
            this.labelInspTime.Size = new System.Drawing.Size(224, 20);
            this.labelInspTime.TabIndex = 12;
            this.labelInspTime.Text = "Insp Time : 00:00:00";
            this.labelInspTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel12
            // 
            this.panel12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel12.Controls.Add(this.btnBuzzerOff);
            this.panel12.Controls.Add(this.btnStop);
            this.panel12.Controls.Add(this.btnRecovery);
            this.panel12.Controls.Add(this.btnReset);
            this.panel12.Controls.Add(this.btnCycleStop);
            this.panel12.Controls.Add(this.btnHome);
            this.panel12.Controls.Add(this.btnStart);
            this.panel12.Location = new System.Drawing.Point(15, 173);
            this.panel12.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(113, 700);
            this.panel12.TabIndex = 19;
            // 
            // btnBuzzerOff
            // 
            this.btnBuzzerOff.Image = global::ezAuto_EFEM.Properties.Resources._1460027068_delete_16;
            this.btnBuzzerOff.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnBuzzerOff.Location = new System.Drawing.Point(8, 600);
            this.btnBuzzerOff.Margin = new System.Windows.Forms.Padding(4);
            this.btnBuzzerOff.Name = "btnBuzzerOff";
            this.btnBuzzerOff.Size = new System.Drawing.Size(96, 90);
            this.btnBuzzerOff.TabIndex = 6;
            this.btnBuzzerOff.Text = "Buzzer Off";
            this.btnBuzzerOff.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnBuzzerOff.UseVisualStyleBackColor = true;
            this.btnBuzzerOff.Click += new System.EventHandler(this.btnBuzzerOff_Click);
            // 
            // btnStop
            // 
            this.btnStop.Image = global::ezAuto_EFEM.Properties.Resources._1460027240_block_16;
            this.btnStop.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnStop.Location = new System.Drawing.Point(7, 110);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(96, 90);
            this.btnStop.TabIndex = 5;
            this.btnStop.Text = "Stop";
            this.btnStop.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnRecovery
            // 
            this.btnRecovery.Image = global::ezAuto_EFEM.Properties.Resources._1460027244_left_16;
            this.btnRecovery.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRecovery.Location = new System.Drawing.Point(8, 306);
            this.btnRecovery.Margin = new System.Windows.Forms.Padding(4);
            this.btnRecovery.Name = "btnRecovery";
            this.btnRecovery.Size = new System.Drawing.Size(96, 90);
            this.btnRecovery.TabIndex = 4;
            this.btnRecovery.Text = "Recovery";
            this.btnRecovery.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRecovery.UseVisualStyleBackColor = true;
            this.btnRecovery.Click += new System.EventHandler(this.btnRecovery_Click);
            // 
            // btnReset
            // 
            this.btnReset.Image = global::ezAuto_EFEM.Properties.Resources._1460027066_plus_16;
            this.btnReset.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnReset.Location = new System.Drawing.Point(7, 502);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(96, 90);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "Reset";
            this.btnReset.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnCycleStop
            // 
            this.btnCycleStop.Image = global::ezAuto_EFEM.Properties.Resources._1460027276_stop_16;
            this.btnCycleStop.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCycleStop.Location = new System.Drawing.Point(7, 208);
            this.btnCycleStop.Margin = new System.Windows.Forms.Padding(4);
            this.btnCycleStop.Name = "btnCycleStop";
            this.btnCycleStop.Size = new System.Drawing.Size(96, 90);
            this.btnCycleStop.TabIndex = 2;
            this.btnCycleStop.Text = "Cycle Stop";
            this.btnCycleStop.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCycleStop.UseVisualStyleBackColor = true;
            this.btnCycleStop.Click += new System.EventHandler(this.btnCycleStop_Click);
            // 
            // btnHome
            // 
            this.btnHome.Image = global::ezAuto_EFEM.Properties.Resources._1460027251_home_16;
            this.btnHome.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnHome.Location = new System.Drawing.Point(7, 404);
            this.btnHome.Margin = new System.Windows.Forms.Padding(4);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(96, 90);
            this.btnHome.TabIndex = 1;
            this.btnHome.Text = "Home";
            this.btnHome.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // btnStart
            // 
            this.btnStart.Image = global::ezAuto_EFEM.Properties.Resources._1460027246_right_16;
            this.btnStart.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnStart.Location = new System.Drawing.Point(7, 12);
            this.btnStart.Margin = new System.Windows.Forms.Padding(4);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(96, 90);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // tableLayoutPanelLP
            // 
            this.tableLayoutPanelLP.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanelLP.ColumnCount = 1;
            this.tableLayoutPanelLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelLP.Location = new System.Drawing.Point(133, 526);
            this.tableLayoutPanelLP.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanelLP.Name = "tableLayoutPanelLP";
            this.tableLayoutPanelLP.RowCount = 1;
            this.tableLayoutPanelLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelLP.Size = new System.Drawing.Size(1116, 478);
            this.tableLayoutPanelLP.TabIndex = 21;
            // 
            // timerUpdate
            // 
            this.timerUpdate.Enabled = true;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.SeaShell;
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.labelInspTime, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelMonthDay, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelWPH, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelTime, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(234, 153);
            this.tableLayoutPanel1.TabIndex = 23;
            // 
            // panelUpperArm
            // 
            this.panelUpperArm.BackColor = System.Drawing.Color.Honeydew;
            this.panelUpperArm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelUpperArm.Controls.Add(this.WaferInfoUpperArm);
            this.panelUpperArm.Controls.Add(this.IOUpperArmSafty);
            this.panelUpperArm.Controls.Add(this.IOUpperArmWaferExist);
            this.panelUpperArm.Controls.Add(this.label1);
            this.panelUpperArm.Controls.Add(this.label2);
            this.panelUpperArm.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelUpperArm.Location = new System.Drawing.Point(264, 117);
            this.panelUpperArm.Margin = new System.Windows.Forms.Padding(4);
            this.panelUpperArm.Name = "panelUpperArm";
            this.panelUpperArm.Size = new System.Drawing.Size(280, 111);
            this.panelUpperArm.TabIndex = 35;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Lime;
            this.label1.Location = new System.Drawing.Point(184, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 17);
            this.label1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(4, 5);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label2.Size = new System.Drawing.Size(115, 22);
            this.label2.TabIndex = 0;
            this.label2.Text = "Upper Arm";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // panelLowerArm
            // 
            this.panelLowerArm.BackColor = System.Drawing.Color.Honeydew;
            this.panelLowerArm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLowerArm.Controls.Add(this.WaferInfoLowerArm);
            this.panelLowerArm.Controls.Add(this.IOLowerArmWaferExist);
            this.panelLowerArm.Controls.Add(this.IOLowerArmSafty);
            this.panelLowerArm.Controls.Add(this.label3);
            this.panelLowerArm.Controls.Add(this.label4);
            this.panelLowerArm.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelLowerArm.Location = new System.Drawing.Point(264, 229);
            this.panelLowerArm.Margin = new System.Windows.Forms.Padding(4);
            this.panelLowerArm.Name = "panelLowerArm";
            this.panelLowerArm.Size = new System.Drawing.Size(280, 111);
            this.panelLowerArm.TabIndex = 36;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Lime;
            this.label3.Location = new System.Drawing.Point(184, 23);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 17);
            this.label3.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(1, 0);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label4.Size = new System.Drawing.Size(120, 22);
            this.label4.TabIndex = 0;
            this.label4.Text = "Lower Arm";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // panelAligner
            // 
            this.panelAligner.BackColor = System.Drawing.Color.Khaki;
            this.panelAligner.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelAligner.Controls.Add(this.WaferInfoAligner);
            this.panelAligner.Controls.Add(this.IOAlignerWaferExist);
            this.panelAligner.Controls.Add(this.label5);
            this.panelAligner.Controls.Add(this.label6);
            this.panelAligner.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelAligner.Location = new System.Drawing.Point(264, 4);
            this.panelAligner.Margin = new System.Windows.Forms.Padding(4);
            this.panelAligner.Name = "panelAligner";
            this.panelAligner.Size = new System.Drawing.Size(280, 111);
            this.panelAligner.TabIndex = 37;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Lime;
            this.label5.Location = new System.Drawing.Point(184, 23);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(0, 17);
            this.label5.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(1, 0);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label6.Size = new System.Drawing.Size(77, 22);
            this.label6.TabIndex = 0;
            this.label6.Text = "Aligner";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.LightSkyBlue;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.IOStageLifterUp);
            this.panel4.Controls.Add(this.IOStageWaferExist);
            this.panel4.Controls.Add(this.labelStateStage);
            this.panel4.Controls.Add(this.WaferInfoStage);
            this.panel4.Controls.Add(this.label7);
            this.panel4.Controls.Add(this.label8);
            this.panel4.Controls.Add(this.IOStageVisionConnected);
            this.panel4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel4.Location = new System.Drawing.Point(6, 118);
            this.panel4.Margin = new System.Windows.Forms.Padding(4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(252, 221);
            this.panel4.TabIndex = 38;
            // 
            // labelStateStage
            // 
            this.labelStateStage.AutoSize = true;
            this.labelStateStage.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelStateStage.Location = new System.Drawing.Point(4, 33);
            this.labelStateStage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelStateStage.Name = "labelStateStage";
            this.labelStateStage.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labelStateStage.Size = new System.Drawing.Size(50, 19);
            this.labelStateStage.TabIndex = 27;
            this.labelStateStage.Text = "State";
            this.labelStateStage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Lime;
            this.label7.Location = new System.Drawing.Point(184, 23);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(0, 17);
            this.label7.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("굴림", 16F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(4, 3);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label8.Size = new System.Drawing.Size(71, 22);
            this.label8.TabIndex = 0;
            this.label8.Text = "Vision";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label8.Click += new System.EventHandler(this.label8_Click);
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.LightGray;
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.tableLayoutPanelFDC);
            this.panel6.Controls.Add(this.panel1);
            this.panel6.Controls.Add(this.lbEQPState);
            this.panel6.Controls.Add(this.userControlOverViewLP1);
            this.panel6.Controls.Add(this.userControlOverViewLP2);
            this.panel6.Controls.Add(this.userControlOverViewLP3);
            this.panel6.Controls.Add(this.panel4);
            this.panel6.Controls.Add(this.panelAligner);
            this.panel6.Controls.Add(this.panelLowerArm);
            this.panel6.Controls.Add(this.panelUpperArm);
            this.panel6.Location = new System.Drawing.Point(134, 173);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(1003, 346);
            this.panel6.TabIndex = 41;
            // 
            // tableLayoutPanelFDC
            // 
            this.tableLayoutPanelFDC.BackColor = System.Drawing.Color.PaleTurquoise;
            this.tableLayoutPanelFDC.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanelFDC.ColumnCount = 1;
            this.tableLayoutPanelFDC.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelFDC.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanelFDC.Location = new System.Drawing.Point(790, 35);
            this.tableLayoutPanelFDC.Name = "tableLayoutPanelFDC";
            this.tableLayoutPanelFDC.RowCount = 6;
            this.tableLayoutPanelFDC.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanelFDC.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanelFDC.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanelFDC.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanelFDC.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanelFDC.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanelFDC.Size = new System.Drawing.Size(205, 304);
            this.tableLayoutPanelFDC.TabIndex = 44;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.PaleTurquoise;
            this.panel1.Controls.Add(this.label9);
            this.panel1.Location = new System.Drawing.Point(790, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(205, 33);
            this.panel1.TabIndex = 43;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("굴림", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label9.Location = new System.Drawing.Point(4, 4);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label9.Size = new System.Drawing.Size(53, 24);
            this.label9.TabIndex = 1;
            this.label9.Text = "FDC";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbEQPState
            // 
            this.lbEQPState.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lbEQPState.Font = new System.Drawing.Font("Segoe UI", 30F);
            this.lbEQPState.Location = new System.Drawing.Point(6, 5);
            this.lbEQPState.Name = "lbEQPState";
            this.lbEQPState.Size = new System.Drawing.Size(252, 109);
            this.lbEQPState.TabIndex = 42;
            this.lbEQPState.Text = "IDLE";
            this.lbEQPState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // userControlOverViewLP1
            // 
            this.userControlOverViewLP1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userControlOverViewLP1.Location = new System.Drawing.Point(550, 229);
            this.userControlOverViewLP1.Margin = new System.Windows.Forms.Padding(0);
            this.userControlOverViewLP1.Name = "userControlOverViewLP1";
            this.userControlOverViewLP1.Size = new System.Drawing.Size(234, 111);
            this.userControlOverViewLP1.TabIndex = 41;
            // 
            // userControlOverViewLP2
            // 
            this.userControlOverViewLP2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userControlOverViewLP2.Location = new System.Drawing.Point(550, 117);
            this.userControlOverViewLP2.Margin = new System.Windows.Forms.Padding(0);
            this.userControlOverViewLP2.Name = "userControlOverViewLP2";
            this.userControlOverViewLP2.Size = new System.Drawing.Size(234, 111);
            this.userControlOverViewLP2.TabIndex = 40;
            // 
            // userControlOverViewLP3
            // 
            this.userControlOverViewLP3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userControlOverViewLP3.Location = new System.Drawing.Point(550, 4);
            this.userControlOverViewLP3.Margin = new System.Windows.Forms.Padding(0);
            this.userControlOverViewLP3.Name = "userControlOverViewLP3";
            this.userControlOverViewLP3.Size = new System.Drawing.Size(234, 111);
            this.userControlOverViewLP3.TabIndex = 39;
            // 
            // IOStageLifterUp
            // 
            this.IOStageLifterUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IOStageLifterUp.IOName = "LifterUp";
            this.IOStageLifterUp.Location = new System.Drawing.Point(144, 65);
            this.IOStageLifterUp.Margin = new System.Windows.Forms.Padding(0);
            this.IOStageLifterUp.Name = "IOStageLifterUp";
            this.IOStageLifterUp.Size = new System.Drawing.Size(114, 31);
            this.IOStageLifterUp.TabIndex = 25;
            // 
            // IOStageWaferExist
            // 
            this.IOStageWaferExist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IOStageWaferExist.IOName = "WaferExist";
            this.IOStageWaferExist.Location = new System.Drawing.Point(144, 32);
            this.IOStageWaferExist.Margin = new System.Windows.Forms.Padding(0);
            this.IOStageWaferExist.Name = "IOStageWaferExist";
            this.IOStageWaferExist.Size = new System.Drawing.Size(133, 41);
            this.IOStageWaferExist.TabIndex = 28;
            // 
            // WaferInfoStage
            // 
            this.WaferInfoStage.Location = new System.Drawing.Point(8, 149);
            this.WaferInfoStage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.WaferInfoStage.Name = "WaferInfoStage";
            this.WaferInfoStage.Size = new System.Drawing.Size(168, 69);
            this.WaferInfoStage.TabIndex = 22;
            this.WaferInfoStage.Load += new System.EventHandler(this.WaferInfoStage_Load);
            // 
            // IOStageVisionConnected
            // 
            this.IOStageVisionConnected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IOStageVisionConnected.IOName = "Connected";
            this.IOStageVisionConnected.Location = new System.Drawing.Point(144, 0);
            this.IOStageVisionConnected.Margin = new System.Windows.Forms.Padding(0);
            this.IOStageVisionConnected.Name = "IOStageVisionConnected";
            this.IOStageVisionConnected.Size = new System.Drawing.Size(153, 44);
            this.IOStageVisionConnected.TabIndex = 26;
            // 
            // WaferInfoAligner
            // 
            this.WaferInfoAligner.Location = new System.Drawing.Point(4, 37);
            this.WaferInfoAligner.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.WaferInfoAligner.Name = "WaferInfoAligner";
            this.WaferInfoAligner.Size = new System.Drawing.Size(168, 69);
            this.WaferInfoAligner.TabIndex = 21;
            // 
            // IOAlignerWaferExist
            // 
            this.IOAlignerWaferExist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IOAlignerWaferExist.IOName = "WaferExist";
            this.IOAlignerWaferExist.Location = new System.Drawing.Point(174, 0);
            this.IOAlignerWaferExist.Margin = new System.Windows.Forms.Padding(0);
            this.IOAlignerWaferExist.Name = "IOAlignerWaferExist";
            this.IOAlignerWaferExist.Size = new System.Drawing.Size(104, 34);
            this.IOAlignerWaferExist.TabIndex = 20;
            // 
            // WaferInfoLowerArm
            // 
            this.WaferInfoLowerArm.Location = new System.Drawing.Point(4, 38);
            this.WaferInfoLowerArm.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.WaferInfoLowerArm.Name = "WaferInfoLowerArm";
            this.WaferInfoLowerArm.Size = new System.Drawing.Size(168, 69);
            this.WaferInfoLowerArm.TabIndex = 27;
            // 
            // IOLowerArmWaferExist
            // 
            this.IOLowerArmWaferExist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IOLowerArmWaferExist.IOName = "WaferExist";
            this.IOLowerArmWaferExist.Location = new System.Drawing.Point(173, 0);
            this.IOLowerArmWaferExist.Margin = new System.Windows.Forms.Padding(0);
            this.IOLowerArmWaferExist.Name = "IOLowerArmWaferExist";
            this.IOLowerArmWaferExist.Size = new System.Drawing.Size(107, 34);
            this.IOLowerArmWaferExist.TabIndex = 22;
            // 
            // IOLowerArmSafty
            // 
            this.IOLowerArmSafty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IOLowerArmSafty.IOName = "Safety";
            this.IOLowerArmSafty.Location = new System.Drawing.Point(173, 34);
            this.IOLowerArmSafty.Margin = new System.Windows.Forms.Padding(0);
            this.IOLowerArmSafty.Name = "IOLowerArmSafty";
            this.IOLowerArmSafty.Size = new System.Drawing.Size(67, 33);
            this.IOLowerArmSafty.TabIndex = 25;
            // 
            // WaferInfoUpperArm
            // 
            this.WaferInfoUpperArm.Location = new System.Drawing.Point(4, 38);
            this.WaferInfoUpperArm.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.WaferInfoUpperArm.Name = "WaferInfoUpperArm";
            this.WaferInfoUpperArm.Size = new System.Drawing.Size(168, 69);
            this.WaferInfoUpperArm.TabIndex = 26;
            // 
            // IOUpperArmSafty
            // 
            this.IOUpperArmSafty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IOUpperArmSafty.IOName = "Safety";
            this.IOUpperArmSafty.Location = new System.Drawing.Point(173, 34);
            this.IOUpperArmSafty.Margin = new System.Windows.Forms.Padding(0);
            this.IOUpperArmSafty.Name = "IOUpperArmSafty";
            this.IOUpperArmSafty.Size = new System.Drawing.Size(89, 34);
            this.IOUpperArmSafty.TabIndex = 24;
            // 
            // IOUpperArmWaferExist
            // 
            this.IOUpperArmWaferExist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IOUpperArmWaferExist.IOName = "WaferExist";
            this.IOUpperArmWaferExist.Location = new System.Drawing.Point(173, 0);
            this.IOUpperArmWaferExist.Margin = new System.Windows.Forms.Padding(0);
            this.IOUpperArmWaferExist.Name = "IOUpperArmWaferExist";
            this.IOUpperArmWaferExist.Size = new System.Drawing.Size(108, 34);
            this.IOUpperArmWaferExist.TabIndex = 21;
            // 
            // UI_EFEM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 1006);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.tableLayoutPanelLP);
            this.Controls.Add(this.panel12);
            this.Controls.Add(this.listViewJobLog);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "UI_EFEM";
            this.Text = "UI_EFEM";
            this.Load += new System.EventHandler(this.UI_EFEM_Load);
            this.VisibleChanged += new System.EventHandler(this.UI_EFEM_VisibleChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.UI_EFEM_Paint);
            this.panel12.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panelUpperArm.ResumeLayout(false);
            this.panelUpperArm.PerformLayout();
            this.panelLowerArm.ResumeLayout(false);
            this.panelLowerArm.PerformLayout();
            this.panelAligner.ResumeLayout(false);
            this.panelAligner.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Button btnCycleStop;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnRecovery;
        private System.Windows.Forms.ListView listViewJobLog;
        private System.Windows.Forms.ColumnHeader No;
        private System.Windows.Forms.ColumnHeader LoadPort;
        private System.Windows.Forms.ColumnHeader JobID;
        private System.Windows.Forms.ColumnHeader LotID;
        private System.Windows.Forms.ColumnHeader CarrierID;
        private System.Windows.Forms.ColumnHeader Recipe;
        private System.Windows.Forms.ColumnHeader SlotMap;
        private System.Windows.Forms.ColumnHeader StartTime;
        private System.Windows.Forms.ColumnHeader EndTime;
        private System.Windows.Forms.ColumnHeader State;
        private System.Windows.Forms.Label labelMonthDay;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Label labelWPH;
        private System.Windows.Forms.Label labelInspTime;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnBuzzerOff;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelLP;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panelUpperArm;
        private UserControlWaferInfo WaferInfoUpperArm;
        private UserControlIOIndicator IOUpperArmSafty;
        private UserControlIOIndicator IOUpperArmWaferExist;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelLowerArm;
        private UserControlWaferInfo WaferInfoLowerArm;
        private UserControlIOIndicator IOLowerArmWaferExist;
        private UserControlIOIndicator IOLowerArmSafty;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panelAligner;
        private UserControlWaferInfo WaferInfoAligner;
        private UserControlIOIndicator IOAlignerWaferExist;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel4;
        private UserControlIOIndicator IOStageLifterUp;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private UserControlWaferInfo WaferInfoStage;
        private UserControlOverViewLP userControlOverViewLP3;
        private UserControlOverViewLP userControlOverViewLP2;
        private System.Windows.Forms.Panel panel6;
        private UserControlOverViewLP userControlOverViewLP1;
        private System.Windows.Forms.Label labelStateStage;
        private UserControlIOIndicator IOStageVisionConnected;
        private UserControlIOIndicator IOStageWaferExist;
        private System.Windows.Forms.Label lbEQPState;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelFDC;
    }
}