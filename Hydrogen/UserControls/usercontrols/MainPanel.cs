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

namespace Hydrogen.UserControls
{
    public partial class MainPanel : UserControl
    {
        decimal time = 0m;
        private bool is_mouse_over;
        public MainPanel() {
            InitializeComponent();
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

                GlobalUIManager.Instance.SetXScale(Int32.Parse(x_scale_text_box.Text.Substring(0, x_scale_text_box.Text.Length - 1)));
                x_scale_text_box.ForeColor = SystemColors.ControlDark;
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

                GlobalUIManager.Instance.SetYScale(Int32.Parse(y_scale_text_box.Text.Substring(0, y_scale_text_box.Text.Length - 1)));
                y_scale_text_box.ForeColor = SystemColors.ControlDark;
            }
        }

        /// <summary>
        /// 차트 업데이트 메서드
        /// </summary>
        public void AddValueToChart(String series) {
            if (!GlobalUIManager.Instance.GetIsGraphLogging() || !GlobalSerialManager.Instance.GetIsConnected()) return;

            int value = 0;

            if (GlobalSerialManager.Instance.GetSerialReceivedData() != null) {
                time += 0.1m;

                value = Int32.Parse(GlobalSerialManager.Instance.GetSerialReceivedData());

                chart1.Series[series].Points.AddXY(time, value);
            }

            if (chart1.Series[series].Points.Count > 0)
            {
                double window_size = GlobalUIManager.Instance.GetXScale();

                double max = chart1.Series[series].Points.Last().XValue;

                double min = max - window_size;

                if (min < 0) min = 0;

                if (max < window_size) max = window_size;

                chart1.ChartAreas[series].AxisY.Minimum = value - value / GlobalUIManager.Instance.GetYScale();
                chart1.ChartAreas[series].AxisY.Maximum = value + value / GlobalUIManager.Instance.GetYScale();

                chart1.ChartAreas[series].AxisX.Minimum = min;
                chart1.ChartAreas[series].AxisX.Maximum = max;
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
