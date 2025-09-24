using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hydrogen.GlobalManagers;

namespace Hydrogen.UserControls {
    public partial class SidePanel : UserControl {
        public SidePanel() {
            InitializeComponent();
        }

        /// <summary>
        /// 페인트 메서드 :: 패널의 테두리 색상 변경을 위한 직사각형 그리기 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tableLayoutPanel2_CellPaint(object sender, TableLayoutCellPaintEventArgs e) {
            GlobalUIManager.Instance.DrawRectangle(Color.FromArgb(163, 199, 249), e);
        }

        private void tableLayoutPanel3_CellPaint(object sender, TableLayoutCellPaintEventArgs e) {
            GlobalUIManager.Instance.DrawRectangle(Color.FromArgb(163, 199, 249), e);
        }

        private void tableLayoutPanel4_CellPaint(object sender, TableLayoutCellPaintEventArgs e) {
            GlobalUIManager.Instance.DrawRectangle(Color.FromArgb(163, 199, 249), e);
        }

        /// <summary>
        /// 마우스 이벤트 :: hover, leave, down, click 등
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void play_button_MouseHover(object sender, EventArgs e) {
            this.play_button.BackColor = System.Drawing.SystemColors.ControlLight;
        }

        private void play_button_MouseLeave(object sender, EventArgs e) {
            this.play_button.BackColor = Color.White;
        }

        private void play_button_MouseDown(object sender, MouseEventArgs e) {
            this.play_button.BackColor = Color.Silver;
        }
    }
}
