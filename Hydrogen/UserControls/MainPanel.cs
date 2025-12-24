using Hydrogen.Forms;
using Hydrogen.GlobalManagers;
using Hydrogen.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Hydrogen.UserControls
{
    public partial class MainPanel : UserControl
    {
        decimal time = 0m;
        private bool is_mouse_over;
        private bool is_dragging;

        private Point lastPos = new Point(0, 0);

        private const int dc_max = 8388608;
        private const int dc_min = -8388608;

        private int count = 0;

        public MainPanel() {
            InitializeComponent();
            chart1.ChartAreas["Raw"].AxisX.LabelStyle.Format = "F1";
            chart1.ChartAreas["Raw"].AxisY.LabelStyle.Format = "F1";

            chart1.Series["Raw"].LegendText = "Raw: #LAST{N0}";

            Color[] myCustomColors = new Color[]
            {
                Color.DeepSkyBlue,
                Color.Red,
                Color.Orange,
                Color.LawnGreen
            };

            chart1.PaletteCustomColors = myCustomColors;

            chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
        }

        private void pictureBox2_Click(object sender, EventArgs e) {
            GlobalLogManager.Instance.ConsoleLog("OK", $"chart1.Series.Count: {chart1.Series.Count}");
            chart1.Series.Clear();
            AddNewSeriesToChart("Raw");

            time = 0;
        }

        /// <summary>
        /// 그래프 로깅 시작 버튼 구현
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Click(object sender, EventArgs e) {
            if(!GlobalUIManager.Instance.GetIsGraphLogging()) {
                pictureBox1.Image = Resources.Stop;
                GlobalUIManager.Instance.SetIsGraphLogging(true);
            }
            else {
                pictureBox1.Image = Resources.play;
                GlobalUIManager.Instance.SetIsGraphLogging(false);
            }
        }

        /// <summary>
        /// x, y축 스케일 텍스트박스 관련 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void text_box_Click(object sender, MouseEventArgs e) {
            var textBox = sender as TextBox;
            textBox.Text = "";
            textBox.ForeColor = Color.Black;
        }

        private void x_scale_text_box_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                if (x_scale_text_box.Text != "") {
                    if (!x_scale_text_box.Text.EndsWith("s")) {
                        x_scale_text_box.Text = x_scale_text_box.Text + "s";
                    }
                }
                else {
                    x_scale_text_box.Text = GlobalUIManager.Instance.GetXScale().ToString() + "s";
                    x_scale_text_box.ForeColor = SystemColors.ControlDark;
                }
                
                try {
                    GlobalUIManager.Instance.SetXScale(Int32.Parse(x_scale_text_box.Text.Substring(0, x_scale_text_box.Text.Length - 1)));
                }
                catch {
                    GlobalUIManager.Instance.SetDebugStat($"Setup Error - Invalid Scale Value {x_scale_text_box.Text}");
                    using (ErrorForm error_form = new ErrorForm()) {
                        error_form.ShowDialog();
                    }
                }
                
                x_scale_text_box.ForeColor = SystemColors.ControlDark;
                try {
                    UpdateChartAreaX("Raw");
                }
                catch (Exception ex) {
                    GlobalLogManager.Instance.ConsoleLog("WARN", $"Error While Updating Chart {ex}");
                }
            }
        }

        private void y_scale_text_box_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                if (y_scale_text_box.Text != "") {
                    if (!y_scale_text_box.Text.EndsWith("%")) {
                        y_scale_text_box.Text = y_scale_text_box.Text + "%";
                    }
                }
                else {
                    y_scale_text_box.Text = GlobalUIManager.Instance.GetYScale().ToString() + "%";
                    y_scale_text_box.ForeColor = SystemColors.ControlDark;
                }

                try {
                    GlobalUIManager.Instance.SetYScale(double.Parse(y_scale_text_box.Text.Substring(0, y_scale_text_box.Text.Length - 1)));
                }
                catch {
                    GlobalUIManager.Instance.SetDebugStat($"Setup Error - Invalid Scale Value {y_scale_text_box.Text}");
                    using (ErrorForm error_form = new ErrorForm()) {
                        error_form.ShowDialog();
                    }
                }

                y_scale_text_box.ForeColor = SystemColors.ControlDark;
                try {
                    UpdateChartAreaY("Raw", Int32.Parse(GlobalSerialManager.Instance.GetSerialReceivedDataRaw()));
                }
                catch (Exception ex) {
                    GlobalLogManager.Instance.ConsoleLog("WARN", $"Error While Updating Chart {ex}");
                }
                
            }
        }

        /// <summary>
        /// 차트 Series 컨트롤
        /// </summary>
        /// <param name="series_name"></param>
        private void AddNewSeriesToChart(string series_name) {
            if (!SeriesExists(series_name)) {
                chart1.Series.Add(series_name);
                chart1.Series[series_name].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                chart1.Series[series_name].BorderWidth = 2;

                chart1.Series[series_name].LegendText = $"{series_name}: " + "#LAST{N0}";
            }
        }

        private void RemoveSeriesFromChart(int cnt) {
            for (int i = cnt; i > 0; i--) {
                chart1.Series.RemoveAt(i);
            }
        }

        private bool SeriesExists(string name) {
            var series_found = chart1.Series.FindByName(name);

            if (series_found != null) return true;
            else return false;
        }

        /// <summary>
        /// 차트 업데이트 메서드
        /// </summary>
        public void UpdateChart(string[] series) {
            if (!GlobalUIManager.Instance.GetIsGraphLogging() || !GlobalSerialManager.Instance.GetIsConnected()) return;

            time += 0.1m;
            int value = 0;

            for (int i = 1; i < series.Length; i++) AddNewSeriesToChart(series[i]);

            if (series.Length < chart1.Series.Count) RemoveSeriesFromChart(chart1.Series.Count - series.Length);

            value = AddValueToChart(series[0]);
            for (int i = 1; i < series.Length; i++) {
                AddValueToChart(series[i]);
            }

            AddInfoToChartTitle("MAX", GlobalUIManager.Instance.GetMaxRaw().ToString());
            AddInfoToChartTitle("MIN", GlobalUIManager.Instance.GetMinRaw().ToString());
            AddInfoToChartTitle("MIN-MAX DIFF", GlobalUIManager.Instance.GetDiffRaw().ToString());

            UpdateChartAreaX(series[0]);
            if (GlobalUIManager.Instance.GetIsAxisYLocked()) return;
            //UpdateChartAreaY(series[0], value);
            int avg = CalAverage(value);
            UpdateChartAreaY(series[0], avg);
        }

        private int AddValueToChart(string series) {
            double value = 0;
            if (GlobalSerialManager.Instance.GetSerialReceivedDataRaw() != null) {
                if (!SeriesExists(series)) return 0;

                switch (series)
                {
                    case "Raw":
                        value = Int32.Parse(GlobalSerialManager.Instance.GetSerialReceivedDataRaw());
                        break;
                    case "SAF":
                        if (GlobalSerialManager.Instance.GetSerialReceivedDataRaw() == null) return (int)value;
                        value = Int32.Parse(GlobalSerialManager.Instance.GetSerialReceivedDataSAF());
                        break;
                    case "LPF":
                        if (GlobalSerialManager.Instance.GetSerialReceivedDataLPF() == null) return (int)value;
                        value = Double.Parse(GlobalSerialManager.Instance.GetSerialReceivedDataLPF());
                        break;
                    case "MAF":
                        if (GlobalSerialManager.Instance.GetSerialReceivedDataMAF() == null) return (int)value;
                        value = Double.Parse(GlobalSerialManager.Instance.GetSerialReceivedDataMAF());
                        break;
                }
                
                chart1.Series[series].Points.AddXY(time, value);
                GlobalLogManager.Instance.ConsoleLog("OK", $"Added value({series}) :: {value}");
            }
            return (int)value;
        }

        private void UpdateChartAreaX(string series) {
            double window_size = GlobalUIManager.Instance.GetXScale();

            double x_max = chart1.Series[series].Points.Last().XValue;

            double x_min = x_max - window_size;

            if (x_min < 0) x_min = 0;
            if (x_max < window_size) x_max = window_size;

            chart1.ChartAreas[series].AxisX.Minimum = x_min;
            chart1.ChartAreas[series].AxisX.Maximum = x_max;
        }

        private void UpdateChartAreaY(string series, double value) {

            if (value == 0) value = 1000;

            double y_max = (value > 0) ? dc_max : dc_min;

            y_max = value + (y_max * (GlobalUIManager.Instance.GetYScale() / 100.0f));
            double y_min = (value * 2) - y_max;

            if (y_max > dc_max) y_max = dc_max;
            if (y_min < dc_min) y_min = dc_min;

            try
            {
                chart1.ChartAreas[series].AxisY.Maximum = (value <= 0) ? y_min : y_max;
                chart1.ChartAreas[series].AxisY.Minimum = (value >= 0) ? y_min : y_max;
            }
            catch (Exception ex)
            {
                GlobalLogManager.Instance.ConsoleLog("ERROR", $"{ex}");
            }
        }

        private int CalAverage(int value) {
            int sum = GlobalUIManager.Instance.GetIntervalXSum();
            if (count >= GlobalUIManager.Instance.GetXScale() * 10) { sum = 0; }
            if (sum == 0) count = 0;
            sum += value;
            count++;
            int avg = sum / count;
            GlobalUIManager.Instance.SetIntervalXSum(sum);
            return avg;
        }

        private void AddInfoToChartTitle(string name, string value) {
            string text = $"{name}: {value:N0}";
            if (chart1.Titles.FindByName(name) == null) { 
                var title = new System.Windows.Forms.DataVisualization.Charting.Title();

                title.Name = name;
                title.Text = text;
                title.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
                title.Alignment = System.Drawing.ContentAlignment.TopCenter;
                title.ForeColor = Color.Red;
                title.DockedToChartArea = "Raw";
                title.DockingOffset = -1;

                chart1.Titles.Add(title);
            }
            else {
                chart1.Titles[name].Text = text;
            }
        }

        private void y_scale_lock_check_box_CheckedChanged(object sender, EventArgs e) {
            if (GlobalUIManager.Instance.GetIsAxisYLocked()) {
                GlobalUIManager.Instance.SetIsAxisYLocked(false);
            }
            else {
                GlobalUIManager.Instance.SetIsAxisYLocked(true);
            }
        }

        /// <summary>
        /// 이미지로 구현한 버튼에 호버 및 클릭시 색상 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_button_MouseHover(object sender, EventArgs e) {
            PictureBox button = sender as PictureBox;
            button.BackColor = System.Drawing.SystemColors.ControlLight;
            is_mouse_over = true;
        }

        private void img_button_MouseLeave(object sender, EventArgs e) {
            PictureBox button = sender as PictureBox;
            button.BackColor = Color.White;
            is_mouse_over = false;
        }

        private void img_button_MouseDown(object sender, MouseEventArgs e) {
            PictureBox button = sender as PictureBox;
            button.BackColor = Color.Silver;
        }
        private void img_button_MouseUp(object sender, MouseEventArgs e) {
            PictureBox button = sender as PictureBox;
            if (is_mouse_over == true) {
                button.BackColor = System.Drawing.SystemColors.ControlLight;
            }
            else {
                button.BackColor = Color.White;
            }
        }

        /// <summary>
        /// 차트 드래그 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chart1_MouseDown(object sender, MouseEventArgs e) {
            is_dragging = true;
            lastPos = e.Location;
            chart1.Cursor = Cursors.Hand;
        }

        private void chart1_MouseUp(object sender, MouseEventArgs e) {
            is_dragging = false;
            chart1.Cursor = Cursors.Default;
        }

        private void chart1_MouseMove(object sender, MouseEventArgs e) {
            if (is_dragging == false) return;
            try {
                var axisX = chart1.ChartAreas["Raw"].AxisX;
                double window_size = GlobalUIManager.Instance.GetXScale();

                double lastX = axisX.PixelPositionToValue(lastPos.X);
                double currentX = axisX.PixelPositionToValue(e.Location.X);

                double x_max = axisX.Maximum;
                double x_min = x_max - window_size;

                double lastXValue = axisX.PixelPositionToValue(lastPos.X);
                double currentXValue = axisX.PixelPositionToValue(e.Location.X);

                double deltaX = (lastX - currentX); 

                x_max = x_max + deltaX;
                x_min = x_min + deltaX;

                if (x_min < 0) x_min = 0;
                if (x_max < window_size) x_max = window_size;

                axisX.Minimum = x_min;
                axisX.Maximum = x_max;

                lastPos = e.Location;
            }
            catch (Exception ex){
                GlobalLogManager.Instance.ConsoleLog("ERROR", $"{ex}");
            }
        }

        /// <summary>
        /// 패널 보더 색상 변경
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tableLayoutPanel1_CellPaint(object sender, TableLayoutCellPaintEventArgs e) {
            //this.tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            GlobalUIManager.Instance.DrawRectangle(Color.FromArgb(163, 199, 249), e);
        }
    }
}
