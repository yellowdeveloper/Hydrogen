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
using static System.Collections.Specialized.BitVector32;

namespace Hydrogen.UserControls {
    public partial class SidePanel : UserControl {

        private bool is_mouse_over;
        private SerialManage _serial_manage;
        public SidePanel() {
            InitializeComponent();
            log_file_name_text_box.Text = GlobalConfigManager.Instance.GetNowLogFileName();
            log_file_path_text_box.Text = GlobalConfigManager.Instance.GetLogFolderPath();

            InitializeManager.OnProgramInitialized += InitializeCommands;
            GlobalConfigManager.OnOptionSaved += SaveCommands;

            this.Disposed += (s, e) => {
                InitializeManager.OnProgramInitialized -= InitializeCommands;
                GlobalConfigManager.OnOptionSaved -= SaveCommands;
            };
        }
        public void InitializeSerialManager(SerialManage serial_manage) {
            _serial_manage = serial_manage;
        }

        private void InitializeCommands() {
            Dictionary<string, string> ctrl_cmd = GlobalConfigManager.Instance.GetCtrlCmdSection();
            Dictionary<string, string> data_cmd = GlobalConfigManager.Instance.GetDataCmdSection();

            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();

            if (ctrl_cmd != null) {
                foreach (var cmd in ctrl_cmd) {
                    dataGridView1.Rows.Add(cmd.Key, cmd.Value);
                }
            }

            if (data_cmd == null) return;
            foreach (var cmd in data_cmd) {
                dataGridView2.Rows.Add(cmd.Key, cmd.Value);
            }
        }

        private void SaveCommands() {
            StringBuilder sb = new StringBuilder();

            string serialConfig = GlobalConfigManager.Instance.ConvertConfigToString();
            sb.AppendLine(serialConfig);

            sb.AppendLine($"[DataCommandInfo]");
            foreach (DataGridViewRow row in dataGridView2.Rows) {
                string cc_name = row.Cells[0].Value?.ToString() ?? null;
                string cc_value = row.Cells[1].Value?.ToString() ?? null;
                if (cc_name != null && cc_value != null) sb.AppendLine($"{cc_name}={cc_value}");
            }
            sb.AppendLine();

            sb.AppendLine($"[CtrlCommandInfo]");
            foreach (DataGridViewRow row in dataGridView1.Rows) {
                string dc_name = row.Cells[0].Value?.ToString() ?? null;
                string dc_value = row.Cells[1].Value?.ToString() ?? null;
                if (dc_name != null && dc_value != null) sb.AppendLine($"{dc_name}={dc_value}");
            }

            string NowCmdConfig = sb.ToString();
            Console.WriteLine($"\n{NowCmdConfig}\n");
            File.WriteAllText(Path.Combine(GlobalConfigManager.Instance.GetConfigFolderPath(), GlobalConfigManager.Instance.GetConfigFileName()), NowCmdConfig);
        }

        // Send Command from Control Command Table
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex == 2) {
                string cmd_str = String.Empty;
                try {
                    cmd_str = dataGridView1[1, e.RowIndex].Value.ToString();

                    byte cmd = Convert.ToByte(cmd_str, 16);
                    _serial_manage.SerialSendCmd(cmd);
                }
                catch (Exception ex) {
                    GlobalLogManager.Instance.ConsoleLog("WARN", $"Error while Sending Command :: {ex}");
                    GlobalLogManager.Instance.AddLogToFile("WARN", $"Error while Sending Command :: {ex}");
                }
                
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
