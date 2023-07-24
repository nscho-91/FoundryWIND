namespace ezAuto_EFEM
{
    partial class UI_JobLog
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
            if (disposing && (components != null)) {
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
            this.listViewJobLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewJobLog.FullRowSelect = true;
            this.listViewJobLog.HideSelection = false;
            this.listViewJobLog.Location = new System.Drawing.Point(0, 0);
            this.listViewJobLog.Margin = new System.Windows.Forms.Padding(4);
            this.listViewJobLog.MultiSelect = false;
            this.listViewJobLog.Name = "listViewJobLog";
            this.listViewJobLog.Size = new System.Drawing.Size(284, 262);
            this.listViewJobLog.TabIndex = 8;
            this.listViewJobLog.UseCompatibleStateImageBehavior = false;
            this.listViewJobLog.View = System.Windows.Forms.View.Details;
            this.listViewJobLog.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.listViewJobLog_ColumnWidthChanged);
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
            // UI_JobLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.ControlBox = false;
            this.Controls.Add(this.listViewJobLog);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "UI_JobLog";
            this.TabText = "JobLog";
            this.ResumeLayout(false);

        }

        #endregion

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

    }
}