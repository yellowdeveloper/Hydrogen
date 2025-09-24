using Hydrogen.GlobalManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hydrogen.UserControls
{
    public partial class MainPanel : UserControl
    {
        decimal time = 0m;
        public MainPanel()
        {
            InitializeComponent();
        }

        public void AddValueToChart() {
            if (GlobalSerialManager.Instance.GetSerialReceivedData() != null) {
                time += 0.1m;
                try {
                    chart1.Series["Series1"].Points.AddXY(time, Int32.Parse(GlobalSerialManager.Instance.GetSerialReceivedData()));
                }
                catch {
                    chart1.Series["Series1"].Points.AddXY(time, 10);
                }
            }
            if (chart1.Series["Series1"].Points.Count > 10) {
                double min = chart1.Series["Series1"].Points[chart1.Series["Series1"].Points.Count - 10].XValue;
                double max = chart1.Series["Series1"].Points.Last().XValue;

                chart1.ChartAreas[0].AxisX.Minimum = min;
                chart1.ChartAreas[0].AxisX.Maximum = max;
            }

            // 차트의 X축 범위를 동적으로 확장
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(chart1.ChartAreas[0].AxisX.Minimum, chart1.ChartAreas[0].AxisX.Maximum);
        }

        private void tableLayoutPanel1_CellPaint(object sender, TableLayoutCellPaintEventArgs e) {
            //this.tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            GlobalUIManager.Instance.DrawRectangle(Color.FromArgb(163, 199, 249), e);
        }
    }
}
