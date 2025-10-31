using Hydrogen.GlobalManagers;
using Hydrogen.SerialComm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hydrogen.UserControls {
    public partial class SidePanel : UserControl {

        private bool is_mouse_over;
        private SerialManage _serial_manage;
        private bool initialized = false;
        public SidePanel() {
            InitializeComponent();
            log_file_name_text_box.Text = GlobalConfigManager.Instance.GetNowLogFileName();
            log_file_path_text_box.Text = GlobalConfigManager.Instance.GetLogFolderPath();

            InitializeManager.OnProgramInitialized += InitializeCommands;

            this.Disposed += (s, e) => {
                InitializeManager.OnProgramInitialized -= InitializeCommands;
            };
        }
        public void InitializeSerialManager(SerialManage serial_manage) {
            _serial_manage = serial_manage;
        }

        private void InitializeCommands() {
            if (initialized == true) return;
            Dictionary<string, string> ctrl_cmd = GlobalConfigManager.Instance.GetCtrlCmdSection();
            Dictionary<string, string> data_cmd = GlobalConfigManager.Instance.GetDataCmdSection();

            foreach (var cmd in ctrl_cmd) {
                dataGridView1.Rows.Add(cmd.Key, cmd.Value);
            }

            foreach (var cmd in data_cmd) {
                dataGridView2.Rows.Add(cmd.Key, cmd.Value);
            }
            initialized = true;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex == 2) {
                _serial_manage.SerialSendCmd(0x1A);
            }
            
        }

        private void play_button_Click(object sender, EventArgs e) {

        }

        /// <summary>
        /// 로그 파일 이름 설정 관련 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox2_Click(object sender, EventArgs e) {
            log_file_name_text_box.Text = GlobalConfigManager.Instance.GetNowLogFileName();
            log_file_name_text_box.ForeColor = SystemColors.ControlDark;
        }

        private void log_file_name_text_box_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                GlobalConfigManager.Instance.SetLogFileName(log_file_name_text_box.Text);
                log_file_name_text_box.ForeColor = SystemColors.ControlDark;
            }
        }

        private void log_file_name_text_box_Click(object sender, EventArgs e) {
            log_file_name_text_box.ForeColor = Color.Black;
        }

        private void log_file_name_text_box_Leave(object sender, EventArgs e) {
            log_file_name_text_box.ForeColor = SystemColors.ControlDark;
        }

        /// <summary>
        /// 로그 파일 경로 설정 관련 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Click(object sender, EventArgs e) {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog()) {
                if (fbd.ShowDialog() == DialogResult.OK) {
                    GlobalConfigManager.Instance.SetLogFolderPath(fbd.SelectedPath);
                    log_file_path_text_box.Text = fbd.SelectedPath;
                }
            }
        }

        private void log_file_path_text_box_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                GlobalConfigManager.Instance.SetLogFolderPath(log_file_path_text_box.Text);
                log_file_path_text_box.ForeColor = SystemColors.ControlDark;
            }
        }
        private void log_file_path_text_box_Click(object sender, EventArgs e) {
            log_file_path_text_box.ForeColor = Color.Black;
        }

        private void log_file_path_text_box_Leave(object sender, EventArgs e) {
            log_file_path_text_box.ForeColor = SystemColors.ControlDark;
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
        private void tableLayoutPanel2_CellPaint(object sender, TableLayoutCellPaintEventArgs e) {
            GlobalUIManager.Instance.DrawRectangle(Color.FromArgb(163, 199, 249), e);
        }

        private void tableLayoutPanel3_CellPaint(object sender, TableLayoutCellPaintEventArgs e) {
            GlobalUIManager.Instance.DrawRectangle(Color.FromArgb(163, 199, 249), e);
        }

        private void tableLayoutPanel4_CellPaint(object sender, TableLayoutCellPaintEventArgs e) {
            GlobalUIManager.Instance.DrawRectangle(Color.FromArgb(163, 199, 249), e);
        }
    }
}
