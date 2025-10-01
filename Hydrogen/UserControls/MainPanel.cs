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
        private bool is_mouse_over;
        public MainPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 차트 업데이트 메서드
        /// </summary>
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
