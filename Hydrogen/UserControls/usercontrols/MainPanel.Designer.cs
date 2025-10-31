namespace Hydrogen.UserControls
{
    partial class MainPanel
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.y_scale_text_box = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.x_scale_label = new System.Windows.Forms.Label();
            this.y_scale_label = new System.Windows.Forms.Label();
            this.x_scale_text_box = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.chart1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(10);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1076, 625);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.tableLayoutPanel1_CellPaint);
            // 
            // chart1
            // 
            chartArea1.AxisX.LineColor = System.Drawing.Color.Gainsboro;
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.Gainsboro;
            chartArea1.AxisX.MajorTickMark.Enabled = false;
            chartArea1.AxisX.MajorTickMark.LineColor = System.Drawing.Color.WhiteSmoke;
            chartArea1.AxisY.LineColor = System.Drawing.Color.Gainsboro;
            chartArea1.AxisY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.Gainsboro;
            chartArea1.AxisY.MajorTickMark.LineColor = System.Drawing.Color.Gainsboro;
            chartArea1.Name = "default";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(2, 24);
            this.chart1.Margin = new System.Windows.Forms.Padding(2, 1, 2, 2);
            this.chart1.Name = "chart1";
            series1.BorderWidth = 2;
            series1.ChartArea = "default";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.DeepSkyBlue;
            series1.Legend = "Legend1";
            series1.Name = "default";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(1072, 599);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 7;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 103F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel2.Controls.Add(this.y_scale_text_box, 6, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.pictureBox1, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.x_scale_label, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.y_scale_label, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.x_scale_text_box, 4, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1072, 20);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // y_scale_text_box
            // 
            this.y_scale_text_box.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.y_scale_text_box.Dock = System.Windows.Forms.DockStyle.Fill;
            this.y_scale_text_box.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.y_scale_text_box.Location = new System.Drawing.Point(1025, 5);
            this.y_scale_text_box.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.y_scale_text_box.Name = "y_scale_text_box";
            this.y_scale_text_box.Size = new System.Drawing.Size(44, 14);
            this.y_scale_text_box.TabIndex = 5;
            this.y_scale_text_box.Text = "100%";
            this.y_scale_text_box.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.y_scale_text_box.MouseClick += new System.Windows.Forms.MouseEventHandler(this.text_box_Click);
            this.y_scale_text_box.KeyDown += new System.Windows.Forms.KeyEventHandler(this.y_scale_text_box_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(40)))), ((int)(((byte)(80)))));
            this.label1.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Result Graph";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.Image = global::Hydrogen.Properties.Resources.play;
            this.pictureBox1.Location = new System.Drawing.Point(106, 3);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(14, 14);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.img_button_MouseDown);
            this.pictureBox1.MouseLeave += new System.EventHandler(this.img_button_MouseLeave);
            this.pictureBox1.MouseHover += new System.EventHandler(this.img_button_MouseHover);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.img_button_MouseUp);
            // 
            // x_scale_label
            // 
            this.x_scale_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.x_scale_label.AutoSize = true;
            this.x_scale_label.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.x_scale_label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(40)))), ((int)(((byte)(80)))));
            this.x_scale_label.Location = new System.Drawing.Point(873, 2);
            this.x_scale_label.Margin = new System.Windows.Forms.Padding(0);
            this.x_scale_label.Name = "x_scale_label";
            this.x_scale_label.Size = new System.Drawing.Size(48, 15);
            this.x_scale_label.TabIndex = 3;
            this.x_scale_label.Text = "x scale :";
            // 
            // y_scale_label
            // 
            this.y_scale_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.y_scale_label.AutoSize = true;
            this.y_scale_label.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.y_scale_label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(40)))), ((int)(((byte)(80)))));
            this.y_scale_label.Location = new System.Drawing.Point(973, 2);
            this.y_scale_label.Margin = new System.Windows.Forms.Padding(0);
            this.y_scale_label.Name = "y_scale_label";
            this.y_scale_label.Size = new System.Drawing.Size(48, 15);
            this.y_scale_label.TabIndex = 4;
            this.y_scale_label.Text = "y scale :";
            // 
            // x_scale_text_box
            // 
            this.x_scale_text_box.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.x_scale_text_box.Dock = System.Windows.Forms.DockStyle.Fill;
            this.x_scale_text_box.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.x_scale_text_box.Location = new System.Drawing.Point(925, 5);
            this.x_scale_text_box.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.x_scale_text_box.Name = "x_scale_text_box";
            this.x_scale_text_box.Size = new System.Drawing.Size(44, 14);
            this.x_scale_text_box.TabIndex = 0;
            this.x_scale_text_box.Text = "1s";
            this.x_scale_text_box.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.x_scale_text_box.MouseClick += new System.Windows.Forms.MouseEventHandler(this.text_box_Click);
            this.x_scale_text_box.KeyDown += new System.Windows.Forms.KeyEventHandler(this.x_scale_text_box_KeyDown);
            // 
            // MainPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MainPanel";
            this.Size = new System.Drawing.Size(1076, 625);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label x_scale_label;
        private System.Windows.Forms.Label y_scale_label;
        private System.Windows.Forms.TextBox x_scale_text_box;
        private System.Windows.Forms.TextBox y_scale_text_box;
    }
}
