namespace ezAxis
{
    partial class Axiss_Ajin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Axiss_Ajin));
            this.comboAxis = new System.Windows.Forms.ComboBox();
            this.labelAxis = new System.Windows.Forms.Label();
            this.groupSetting = new System.Windows.Forms.GroupBox();
            this.buttonSaveReg = new System.Windows.Forms.Button();
            this.buttonLoadMOT = new System.Windows.Forms.Button();
            this.groupMove = new System.Windows.Forms.GroupBox();
            this.buttonRepeat = new System.Windows.Forms.Button();
            this.buttonPos2 = new System.Windows.Forms.Button();
            this.buttonPos1 = new System.Windows.Forms.Button();
            this.buttonPos0 = new System.Windows.Forms.Button();
            this.groupJog = new System.Windows.Forms.GroupBox();
            this.buttonJogRight = new System.Windows.Forms.Button();
            this.buttonJogLeft = new System.Windows.Forms.Button();
            this.groupRun = new System.Windows.Forms.GroupBox();
            this.buttonReset = new System.Windows.Forms.Button();
            this.buttonHome = new System.Windows.Forms.Button();
            this.checkServoOn = new System.Windows.Forms.CheckBox();
            this.grid = new PropertyGridEx.PropertyGridEx();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.labelCmd = new System.Windows.Forms.Label();
            this.labelEnc = new System.Windows.Forms.Label();
            this.labelHome = new System.Windows.Forms.Label();
            this.labelLimitM = new System.Windows.Forms.Label();
            this.labelLimitP = new System.Windows.Forms.Label();
            this.labelInPos = new System.Windows.Forms.Label();
            this.labelAlram = new System.Windows.Forms.Label();
            this.labelEmg = new System.Windows.Forms.Label();
            this.groupSetting.SuspendLayout();
            this.groupMove.SuspendLayout();
            this.groupJog.SuspendLayout();
            this.groupRun.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboAxis
            // 
            this.comboAxis.FormattingEnabled = true;
            this.comboAxis.Location = new System.Drawing.Point(48, 12);
            this.comboAxis.Name = "comboAxis";
            this.comboAxis.Size = new System.Drawing.Size(150, 20);
            this.comboAxis.TabIndex = 0;
            this.comboAxis.SelectedIndexChanged += new System.EventHandler(this.comboAxis_SelectedIndexChanged);
            // 
            // labelAxis
            // 
            this.labelAxis.AutoSize = true;
            this.labelAxis.Location = new System.Drawing.Point(12, 15);
            this.labelAxis.Name = "labelAxis";
            this.labelAxis.Size = new System.Drawing.Size(30, 12);
            this.labelAxis.TabIndex = 1;
            this.labelAxis.Text = "Axis";
            // 
            // groupSetting
            // 
            this.groupSetting.Controls.Add(this.buttonSaveReg);
            this.groupSetting.Controls.Add(this.buttonLoadMOT);
            this.groupSetting.Location = new System.Drawing.Point(14, 38);
            this.groupSetting.Name = "groupSetting";
            this.groupSetting.Size = new System.Drawing.Size(88, 83);
            this.groupSetting.TabIndex = 2;
            this.groupSetting.TabStop = false;
            this.groupSetting.Text = "Setting";
            // 
            // buttonSaveReg
            // 
            this.buttonSaveReg.Location = new System.Drawing.Point(7, 49);
            this.buttonSaveReg.Name = "buttonSaveReg";
            this.buttonSaveReg.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveReg.TabIndex = 3;
            this.buttonSaveReg.Text = "Save Reg";
            this.buttonSaveReg.UseVisualStyleBackColor = true;
            this.buttonSaveReg.Click += new System.EventHandler(this.buttonSaveReg_Click);
            // 
            // buttonLoadMOT
            // 
            this.buttonLoadMOT.Location = new System.Drawing.Point(6, 20);
            this.buttonLoadMOT.Name = "buttonLoadMOT";
            this.buttonLoadMOT.Size = new System.Drawing.Size(76, 23);
            this.buttonLoadMOT.TabIndex = 3;
            this.buttonLoadMOT.Text = "Load MOT";
            this.buttonLoadMOT.UseVisualStyleBackColor = true;
            this.buttonLoadMOT.Click += new System.EventHandler(this.buttonLoadMOT_Click);
            // 
            // groupMove
            // 
            this.groupMove.Controls.Add(this.buttonRepeat);
            this.groupMove.Controls.Add(this.buttonPos2);
            this.groupMove.Controls.Add(this.buttonPos1);
            this.groupMove.Controls.Add(this.buttonPos0);
            this.groupMove.Location = new System.Drawing.Point(14, 127);
            this.groupMove.Name = "groupMove";
            this.groupMove.Size = new System.Drawing.Size(88, 142);
            this.groupMove.TabIndex = 4;
            this.groupMove.TabStop = false;
            this.groupMove.Text = "Move";
            // 
            // buttonRepeat
            // 
            this.buttonRepeat.Location = new System.Drawing.Point(6, 107);
            this.buttonRepeat.Name = "buttonRepeat";
            this.buttonRepeat.Size = new System.Drawing.Size(76, 23);
            this.buttonRepeat.TabIndex = 6;
            this.buttonRepeat.Text = "Repeat1~2";
            this.buttonRepeat.UseVisualStyleBackColor = true;
            this.buttonRepeat.Click += new System.EventHandler(this.buttonRepeat_Click);
            // 
            // buttonPos2
            // 
            this.buttonPos2.Location = new System.Drawing.Point(6, 78);
            this.buttonPos2.Name = "buttonPos2";
            this.buttonPos2.Size = new System.Drawing.Size(76, 23);
            this.buttonPos2.TabIndex = 5;
            this.buttonPos2.Text = "Pos 2";
            this.buttonPos2.UseVisualStyleBackColor = true;
            this.buttonPos2.Click += new System.EventHandler(this.buttonPos2_Click);
            // 
            // buttonPos1
            // 
            this.buttonPos1.Location = new System.Drawing.Point(6, 49);
            this.buttonPos1.Name = "buttonPos1";
            this.buttonPos1.Size = new System.Drawing.Size(76, 23);
            this.buttonPos1.TabIndex = 4;
            this.buttonPos1.Text = "Pos 1";
            this.buttonPos1.UseVisualStyleBackColor = true;
            this.buttonPos1.Click += new System.EventHandler(this.buttonPos1_Click);
            // 
            // buttonPos0
            // 
            this.buttonPos0.Location = new System.Drawing.Point(6, 20);
            this.buttonPos0.Name = "buttonPos0";
            this.buttonPos0.Size = new System.Drawing.Size(76, 23);
            this.buttonPos0.TabIndex = 3;
            this.buttonPos0.Text = "Pos 0";
            this.buttonPos0.UseVisualStyleBackColor = true;
            this.buttonPos0.Click += new System.EventHandler(this.buttonPos0_Click);
            // 
            // groupJog
            // 
            this.groupJog.Controls.Add(this.buttonJogRight);
            this.groupJog.Controls.Add(this.buttonJogLeft);
            this.groupJog.Location = new System.Drawing.Point(14, 275);
            this.groupJog.Name = "groupJog";
            this.groupJog.Size = new System.Drawing.Size(88, 54);
            this.groupJog.TabIndex = 7;
            this.groupJog.TabStop = false;
            this.groupJog.Text = "Jog";
            // 
            // buttonJogRight
            // 
            this.buttonJogRight.Location = new System.Drawing.Point(47, 20);
            this.buttonJogRight.Name = "buttonJogRight";
            this.buttonJogRight.Size = new System.Drawing.Size(35, 23);
            this.buttonJogRight.TabIndex = 10;
            this.buttonJogRight.Text = ">";
            this.buttonJogRight.UseVisualStyleBackColor = true;
            this.buttonJogRight.Click += new System.EventHandler(this.buttonJogRight_Click);
            this.buttonJogRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonJogRight_MouseDown);
            this.buttonJogRight.MouseLeave += new System.EventHandler(this.buttonJogRight_MouseLeave);
            this.buttonJogRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttonJogRight_MouseUp);
            // 
            // buttonJogLeft
            // 
            this.buttonJogLeft.Location = new System.Drawing.Point(7, 20);
            this.buttonJogLeft.Name = "buttonJogLeft";
            this.buttonJogLeft.Size = new System.Drawing.Size(35, 23);
            this.buttonJogLeft.TabIndex = 9;
            this.buttonJogLeft.Text = "<";
            this.buttonJogLeft.UseVisualStyleBackColor = true;
            this.buttonJogLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonJogLeft_MouseDown);
            this.buttonJogLeft.MouseLeave += new System.EventHandler(this.buttonJogLeft_MouseLeave);
            this.buttonJogLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttonJogLeft_MouseUp);
            // 
            // groupRun
            // 
            this.groupRun.Controls.Add(this.buttonReset);
            this.groupRun.Controls.Add(this.buttonHome);
            this.groupRun.Controls.Add(this.checkServoOn);
            this.groupRun.Location = new System.Drawing.Point(14, 335);
            this.groupRun.Name = "groupRun";
            this.groupRun.Size = new System.Drawing.Size(88, 103);
            this.groupRun.TabIndex = 8;
            this.groupRun.TabStop = false;
            this.groupRun.Text = "Run";
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(6, 71);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(76, 23);
            this.buttonReset.TabIndex = 8;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // buttonHome
            // 
            this.buttonHome.Location = new System.Drawing.Point(6, 42);
            this.buttonHome.Name = "buttonHome";
            this.buttonHome.Size = new System.Drawing.Size(76, 23);
            this.buttonHome.TabIndex = 7;
            this.buttonHome.Text = "Home";
            this.buttonHome.UseVisualStyleBackColor = true;
            this.buttonHome.Click += new System.EventHandler(this.buttonHome_Click);
            // 
            // checkServoOn
            // 
            this.checkServoOn.AutoSize = true;
            this.checkServoOn.Location = new System.Drawing.Point(7, 20);
            this.checkServoOn.Name = "checkServoOn";
            this.checkServoOn.Size = new System.Drawing.Size(76, 16);
            this.checkServoOn.TabIndex = 0;
            this.checkServoOn.Text = "Servo On";
            this.checkServoOn.UseVisualStyleBackColor = true;
            this.checkServoOn.Click += new System.EventHandler(this.checkServoOn_Click);
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
            this.grid.DocCommentDescription.Size = new System.Drawing.Size(264, 36);
            this.grid.DocCommentDescription.TabIndex = 1;
            this.grid.DocCommentImage = null;
            // 
            // 
            // 
            this.grid.DocCommentTitle.Cursor = System.Windows.Forms.Cursors.Default;
            this.grid.DocCommentTitle.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold);
            this.grid.DocCommentTitle.Location = new System.Drawing.Point(3, 3);
            this.grid.DocCommentTitle.Name = "";
            this.grid.DocCommentTitle.Size = new System.Drawing.Size(264, 16);
            this.grid.DocCommentTitle.TabIndex = 0;
            this.grid.DocCommentTitle.UseMnemonic = false;
            this.grid.Location = new System.Drawing.Point(120, 138);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(270, 477);
            this.grid.TabIndex = 9;
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
            this.grid.ToolStrip.Size = new System.Drawing.Size(270, 25);
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
            // labelCmd
            // 
            this.labelCmd.AutoSize = true;
            this.labelCmd.Location = new System.Drawing.Point(118, 47);
            this.labelCmd.Name = "labelCmd";
            this.labelCmd.Size = new System.Drawing.Size(50, 12);
            this.labelCmd.TabIndex = 10;
            this.labelCmd.Text = "Cmd : 0";
            // 
            // labelEnc
            // 
            this.labelEnc.AutoSize = true;
            this.labelEnc.Location = new System.Drawing.Point(118, 69);
            this.labelEnc.Name = "labelEnc";
            this.labelEnc.Size = new System.Drawing.Size(49, 12);
            this.labelEnc.TabIndex = 11;
            this.labelEnc.Text = "Enc  : 0";
            // 
            // labelHome
            // 
            this.labelHome.AutoSize = true;
            this.labelHome.Location = new System.Drawing.Point(118, 91);
            this.labelHome.Name = "labelHome";
            this.labelHome.Size = new System.Drawing.Size(38, 12);
            this.labelHome.TabIndex = 12;
            this.labelHome.Text = "Home";
            // 
            // labelLimitM
            // 
            this.labelLimitM.AutoSize = true;
            this.labelLimitM.Location = new System.Drawing.Point(178, 91);
            this.labelLimitM.Name = "labelLimitM";
            this.labelLimitM.Size = new System.Drawing.Size(38, 12);
            this.labelLimitM.TabIndex = 13;
            this.labelLimitM.Text = "Limit-";
            // 
            // labelLimitP
            // 
            this.labelLimitP.AutoSize = true;
            this.labelLimitP.Location = new System.Drawing.Point(238, 91);
            this.labelLimitP.Name = "labelLimitP";
            this.labelLimitP.Size = new System.Drawing.Size(38, 12);
            this.labelLimitP.TabIndex = 14;
            this.labelLimitP.Text = "Limit+";
            // 
            // labelInPos
            // 
            this.labelInPos.AutoSize = true;
            this.labelInPos.Location = new System.Drawing.Point(118, 113);
            this.labelInPos.Name = "labelInPos";
            this.labelInPos.Size = new System.Drawing.Size(37, 12);
            this.labelInPos.TabIndex = 15;
            this.labelInPos.Text = "InPos";
            // 
            // labelAlram
            // 
            this.labelAlram.AutoSize = true;
            this.labelAlram.Location = new System.Drawing.Point(178, 113);
            this.labelAlram.Name = "labelAlram";
            this.labelAlram.Size = new System.Drawing.Size(38, 12);
            this.labelAlram.TabIndex = 16;
            this.labelAlram.Text = "Alram";
            // 
            // labelEmg
            // 
            this.labelEmg.AutoSize = true;
            this.labelEmg.Location = new System.Drawing.Point(238, 113);
            this.labelEmg.Name = "labelEmg";
            this.labelEmg.Size = new System.Drawing.Size(31, 12);
            this.labelEmg.TabIndex = 17;
            this.labelEmg.Text = "Emg";
            // 
            // Axiss_Ajin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 684);
            this.Controls.Add(this.labelEmg);
            this.Controls.Add(this.labelAlram);
            this.Controls.Add(this.labelInPos);
            this.Controls.Add(this.labelLimitP);
            this.Controls.Add(this.labelLimitM);
            this.Controls.Add(this.labelHome);
            this.Controls.Add(this.labelEnc);
            this.Controls.Add(this.labelCmd);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.groupRun);
            this.Controls.Add(this.groupJog);
            this.Controls.Add(this.groupMove);
            this.Controls.Add(this.groupSetting);
            this.Controls.Add(this.labelAxis);
            this.Controls.Add(this.comboAxis);
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Axiss_Ajin";
            this.Text = "Axiss_Ajin";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Axiss_Ajin_FormClosing);
            this.Resize += new System.EventHandler(this.Axiss_Ajin_Resize);
            this.groupSetting.ResumeLayout(false);
            this.groupMove.ResumeLayout(false);
            this.groupJog.ResumeLayout(false);
            this.groupRun.ResumeLayout(false);
            this.groupRun.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboAxis;
        private System.Windows.Forms.Label labelAxis;
        private System.Windows.Forms.GroupBox groupSetting;
        private System.Windows.Forms.Button buttonLoadMOT;
        private System.Windows.Forms.Button buttonSaveReg;
        private System.Windows.Forms.GroupBox groupMove;
        private System.Windows.Forms.Button buttonRepeat;
        private System.Windows.Forms.Button buttonPos2;
        private System.Windows.Forms.Button buttonPos1;
        private System.Windows.Forms.Button buttonPos0;
        private System.Windows.Forms.GroupBox groupJog;
        private System.Windows.Forms.GroupBox groupRun;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Button buttonHome;
        private System.Windows.Forms.CheckBox checkServoOn;
        private System.Windows.Forms.Button buttonJogRight;
        private System.Windows.Forms.Button buttonJogLeft;
        private PropertyGridEx.PropertyGridEx grid;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label labelCmd;
        private System.Windows.Forms.Label labelEnc;
        private System.Windows.Forms.Label labelHome;
        private System.Windows.Forms.Label labelLimitM;
        private System.Windows.Forms.Label labelLimitP;
        private System.Windows.Forms.Label labelInPos;
        private System.Windows.Forms.Label labelAlram;
        private System.Windows.Forms.Label labelEmg;
    }
}