namespace ezAuto_EFEM
{
    partial class HW_Aligner_EBR_EBR_Graph
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chartEBR = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.listViewEBR = new System.Windows.Forms.ListView();
            this.columnHeaderIndex = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderEBR = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderBevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeaderAngle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.chartEBR)).BeginInit();
            this.SuspendLayout();
            // 
            // chartEBR
            // 
            chartArea1.AxisX.Interval = 60D;
            chartArea1.AxisX.Maximum = 360D;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.Name = "ChartArea1";
            this.chartEBR.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chartEBR.Legends.Add(legend1);
            this.chartEBR.Location = new System.Drawing.Point(12, 12);
            this.chartEBR.Name = "chartEBR";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.Green;
            series1.Legend = "Legend1";
            series1.Name = "EBR";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            series2.Legend = "Legend1";
            series2.Name = "Bevel";
            this.chartEBR.Series.Add(series1);
            this.chartEBR.Series.Add(series2);
            this.chartEBR.Size = new System.Drawing.Size(500, 252);
            this.chartEBR.TabIndex = 0;
            this.chartEBR.Text = "chart1";
            // 
            // listViewEBR
            // 
            this.listViewEBR.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderIndex,
            this.columnHeaderEBR,
            this.columnHeaderBevel,
            this.columnHeaderAngle});
            this.listViewEBR.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.listViewEBR.GridLines = true;
            this.listViewEBR.Location = new System.Drawing.Point(12, 270);
            this.listViewEBR.Name = "listViewEBR";
            this.listViewEBR.Size = new System.Drawing.Size(500, 500);
            this.listViewEBR.TabIndex = 1;
            this.listViewEBR.UseCompatibleStateImageBehavior = false;
            this.listViewEBR.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderIndex
            // 
            this.columnHeaderIndex.Text = "Index";
            this.columnHeaderIndex.Width = 75;
            // 
            // columnHeaderEBR
            // 
            this.columnHeaderEBR.Text = "EBR";
            this.columnHeaderEBR.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderEBR.Width = 140;
            // 
            // columnHeaderBevel
            // 
            this.columnHeaderBevel.Text = "Bevel";
            this.columnHeaderBevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderBevel.Width = 140;
            // 
            // columnHeaderAngle
            // 
            this.columnHeaderAngle.Text = "Angle";
            this.columnHeaderAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeaderAngle.Width = 140;
            // 
            // HW_Aligner_EBR_EBR_Graph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 870);
            this.Controls.Add(this.listViewEBR);
            this.Controls.Add(this.chartEBR);
            this.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HW_Aligner_EBR_EBR_Graph";
            this.Text = "HW_Aligner_EBR_EBR_Graph";
            this.Load += new System.EventHandler(this.HW_Aligner_EBR_EBR_Graph_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartEBR)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataVisualization.Charting.Chart chartEBR;
        public System.Windows.Forms.ListView listViewEBR;
        private System.Windows.Forms.ColumnHeader columnHeaderIndex;
        private System.Windows.Forms.ColumnHeader columnHeaderEBR;
        private System.Windows.Forms.ColumnHeader columnHeaderBevel;
        private System.Windows.Forms.ColumnHeader columnHeaderAngle;
    }
}