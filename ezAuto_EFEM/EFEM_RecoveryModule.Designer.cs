using ezAutoMom;
namespace ezAuto_EFEM
{
    partial class EFEM_RecoveryModule
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
            this.groupName = new System.Windows.Forms.GroupBox();
            this.comboSlot = new System.Windows.Forms.ComboBox();
            this.comboLoadPort = new System.Windows.Forms.ComboBox();
            this.checkEdit = new System.Windows.Forms.CheckBox();
            this.labelError = new System.Windows.Forms.Label();
            this.IOWaferExist = new ezAuto_EFEM.UserControlIOIndicator();
            this.groupName.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupName
            // 
            this.groupName.Controls.Add(this.labelError);
            this.groupName.Controls.Add(this.IOWaferExist);
            this.groupName.Controls.Add(this.comboSlot);
            this.groupName.Controls.Add(this.comboLoadPort);
            this.groupName.Controls.Add(this.checkEdit);
            this.groupName.Font = new System.Drawing.Font("굴림", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.groupName.Location = new System.Drawing.Point(12, 12);
            this.groupName.Name = "groupName";
            this.groupName.Size = new System.Drawing.Size(173, 201);
            this.groupName.TabIndex = 2;
            this.groupName.TabStop = false;
            this.groupName.Text = "Group";
            // 
            // comboSlot
            // 
            this.comboSlot.Font = new System.Drawing.Font("굴림", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.comboSlot.FormattingEnabled = true;
            this.comboSlot.Location = new System.Drawing.Point(6, 142);
            this.comboSlot.Name = "comboSlot";
            this.comboSlot.Size = new System.Drawing.Size(161, 29);
            this.comboSlot.TabIndex = 2;
            this.comboSlot.SelectedIndexChanged += new System.EventHandler(this.comboSlot_SelectedIndexChanged);
            // 
            // comboLoadPort
            // 
            this.comboLoadPort.Font = new System.Drawing.Font("굴림", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.comboLoadPort.FormattingEnabled = true;
            this.comboLoadPort.Location = new System.Drawing.Point(6, 107);
            this.comboLoadPort.Name = "comboLoadPort";
            this.comboLoadPort.Size = new System.Drawing.Size(161, 29);
            this.comboLoadPort.TabIndex = 1;
            this.comboLoadPort.SelectedIndexChanged += new System.EventHandler(this.comboLoadPort_SelectedIndexChanged);
            // 
            // checkEdit
            // 
            this.checkEdit.AutoSize = true;
            this.checkEdit.Font = new System.Drawing.Font("굴림", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.checkEdit.Location = new System.Drawing.Point(6, 76);
            this.checkEdit.Name = "checkEdit";
            this.checkEdit.Size = new System.Drawing.Size(57, 23);
            this.checkEdit.TabIndex = 0;
            this.checkEdit.Text = "Edit";
            this.checkEdit.UseVisualStyleBackColor = true;
            this.checkEdit.CheckedChanged += new System.EventHandler(this.checkEdit_CheckedChanged);
            // 
            // labelError
            // 
            this.labelError.AutoSize = true;
            this.labelError.Font = new System.Drawing.Font("굴림", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.labelError.Location = new System.Drawing.Point(6, 177);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(0, 16);
            this.labelError.TabIndex = 4;
            // 
            // IOWaferExist
            // 
            this.IOWaferExist.Font = new System.Drawing.Font("굴림", 10F);
            this.IOWaferExist.IOName = "Wafer Exist";
            this.IOWaferExist.Location = new System.Drawing.Point(8, 44);
            this.IOWaferExist.Margin = new System.Windows.Forms.Padding(0);
            this.IOWaferExist.Name = "IOWaferExist";
            this.IOWaferExist.Size = new System.Drawing.Size(158, 25);
            this.IOWaferExist.TabIndex = 3;
            // 
            // EFEM_RecoveryModule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(195, 225);
            this.Controls.Add(this.groupName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "EFEM_RecoveryModule";
            this.Text = "EFEM_RecoveryModule";
            this.groupName.ResumeLayout(false);
            this.groupName.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupName;
        private System.Windows.Forms.CheckBox checkEdit;
        private System.Windows.Forms.ComboBox comboSlot;
        private System.Windows.Forms.ComboBox comboLoadPort;
        private UserControlIOIndicator IOWaferExist;
        private System.Windows.Forms.Label labelError;

    }
}