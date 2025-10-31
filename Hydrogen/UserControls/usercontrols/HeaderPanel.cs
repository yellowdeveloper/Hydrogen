using Hydrogen.Forms;
using Hydrogen.GlobalManagers;
using Hydrogen.SerialComm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hydrogen.UserControls {
    public partial class HeaderPanel : UserControl {

        private bool is_mouse_over;

        private SerialManage _serial_manage;
        public HeaderPanel() {
            InitializeComponent();
        }

        /// <summary>
        /// 헤더 패널에 표시되는 연결 정보를 표시하기 위한 메서드
        /// </summary>
        /// <param name="text"></param>
        public void SetModelLabelText(string text) {
            model_lab.Text = text;
        }
        public void SetInterfaceLabelText(string text) {
            interface_lab.Text = text;
        }
        public void SetBaudrateLabelText(string text) {
            baudrate_lab.Text = text;
        }
        public void SetDebugDataText(string text) {
            debug_dat_lab.Text = text;
        }

        /// <summary>
        /// 시리얼 포트 연결 및 해제 토글 버튼
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cnt_but_Click(object sender, EventArgs e) {
            if (!GlobalSerialManager.Instance.GetIsConnected()) {
                if (_serial_manage.SerialConnect() == 0) {
                    GlobalSerialManager.Instance.SetIsConnected(true);
                    cnt_but.Text = "Disconnect";
                    cnt_but.ForeColor = Color.White;
                    cnt_but.BackColor = Color.Tomato;
                }
            }
            else {
                _serial_manage.SerialDisconnect();
                GlobalSerialManager.Instance.SetIsConnected(false);
                cnt_but.Text = "Connect";
                cnt_but.ForeColor = Color.Black;
                cnt_but.BackColor = Color.White;
            }

        }

        /// <summary>
        /// 시리얼 연결 설정 버튼 :: SetupForm 호출
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setup_btn_Click(object sender, EventArgs e) {
            using (SetupForm setup_form = new SetupForm()) {
                setup_form.ShowDialog();

                this.interface_lab.Text = $"Interface   :   {GlobalSerialManager.Instance.GetPortName()}";
                this.baudrate_lab.Text = $"Baudrate   :   {GlobalSerialManager.Instance.GetBaudrate().ToString()}";
            }
            
        }

        /// <summary>
        /// save 및 load 버튼 기능
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sav_btn_Click(object sender, EventArgs e) {
            GlobalConfigManager.Instance.SetConfig();
            string NowConfig = GlobalConfigManager.Instance.ConvertConfigToString();
            File.WriteAllText(Path.Combine(GlobalConfigManager.Instance.GetConfigFolderPath(), GlobalConfigManager.Instance.GetConfigFileName()), NowConfig);
        }

        private void load_btn_Click(object sender, EventArgs e) {
            InitializeManager.InitializeProgram();
            this.interface_lab.Text = $"Interface   :   {GlobalSerialManager.Instance.GetPortName()}";
            this.baudrate_lab.Text = $"Baudrate   :   {GlobalSerialManager.Instance.GetBaudrate().ToString()}";
        }

        public void InitializeSerialManager(SerialManage serial_manager) {
            this._serial_manage = serial_manager;
        }

        /// <summary>
        /// 이미지로 구현한 버튼에 호버 및 클릭시 색상 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_btn_MouseDown(object sender, MouseEventArgs e) {
            PictureBox button = sender as PictureBox;
            button.BackColor = Color.Silver;
        }

        private void img_btn_MouseHover(object sender, EventArgs e) {
            PictureBox button = sender as PictureBox;
            button.BackColor = System.Drawing.SystemColors.ControlLight;
            is_mouse_over = true;
        }

        private void img_btn_MouseLeave(object sender, EventArgs e) {
            PictureBox button = sender as PictureBox;
            button.BackColor = Color.White;
            is_mouse_over = false;
        }

        private void img_btn_MouseUp(object sender, MouseEventArgs e) {
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
        private void tableLayoutPanel6_CellPaint(object sender, TableLayoutCellPaintEventArgs e) {
            GlobalUIManager.Instance.DrawRectangle(Color.FromArgb(163, 199, 249), e);
        }

        private void tableLayoutPanel2_CellPaint(object sender, TableLayoutCellPaintEventArgs e) {
            GlobalUIManager.Instance.DrawRectangle(Color.FromArgb(163, 199, 249), e);
        }
    }
}
