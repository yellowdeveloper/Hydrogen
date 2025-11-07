using Hydrogen.GlobalManagers;
using Hydrogen.Properties;
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

        private DataGridViewRow[] _enabled_rows = new DataGridViewRow[5];
        public SidePanel() {
            InitializeComponent();
            log_file_name_text_box.Text = GlobalConfigManager.Instance.GetNowLogDefaultFileName();
            log_file_path_text_box.Text = GlobalConfigManager.Instance.GetLogFolderPath();
            hydrogen_percent_text_box.Text = GlobalUIManager.Instance.GetHydrogenPercent().ToString() + "%";

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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex == 2) {
                string cmd_str = String.Empty;
                string cmd_name = String.Empty;

                try {
                    cmd_str = dataGridView1[1, e.RowIndex].Value.ToString();
                    cmd_name = dataGridView1[0, e.RowIndex].Value.ToString();

                    byte cmd = Convert.ToByte(cmd_str, 16);
                    _serial_manage.SerialSendCmd(cmd);

                    CheckCommand(cmd_name, cmd_str, e.RowIndex);

                    dataGridView1[1, e.RowIndex].Style.BackColor = Color.LightGreen;
                }
                catch (Exception ex) {
                    GlobalLogManager.Instance.ConsoleLog("WARN", $"Error while Sending Command :: {ex}");
                    GlobalLogManager.Instance.AddLogToFile("WARN", $"Error while Sending Command :: {ex}");
                }
            }
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e) {
            if (e.ColumnIndex == 3) {
                string cmd_str = String.Empty;
                string cmd_name = String.Empty;

                try {
                    cmd_str = dataGridView2[1, e.RowIndex].Value.ToString();
                    cmd_name = dataGridView2[0, e.RowIndex].Value.ToString();

                    CheckCommand(cmd_name, cmd_str, e.RowIndex);
                }
                catch (Exception ex) {
                    GlobalLogManager.Instance.ConsoleLog("WARN", $"Error while Sending Command :: {ex}");
                    GlobalLogManager.Instance.AddLogToFile("WARN", $"Error while Sending Command :: {ex}");
                }
            }
        }

        private void CheckCommand(string cmd_name, string cmd_str, int row_index) {
            if (cmd_name.StartsWith("Read")) {
                if (_enabled_rows[0] == null) {
                    dataGridView1.Rows[row_index].Cells[1].Style.BackColor = Color.LightGreen;
                    _enabled_rows[0] = dataGridView1.Rows[row_index];
                }
                else {
                    _enabled_rows[0].Cells[1].Style.BackColor = Color.White;
                    dataGridView1.Rows[row_index].Cells[1].Style.BackColor = Color.LightGreen;
                    _enabled_rows[0] = dataGridView1.Rows[row_index];
                }
            }
            else if (cmd_name.StartsWith("DR")) {
                if (_enabled_rows[1] == null) {
                    dataGridView1.Rows[row_index].Cells[1].Style.BackColor = Color.LightGreen;
                    _enabled_rows[1] = dataGridView1.Rows[row_index];
                }
                else {
                    _enabled_rows[1].Cells[1].Style.BackColor = Color.White;
                    dataGridView1.Rows[row_index].Cells[1].Style.BackColor = Color.LightGreen;
                    _enabled_rows[1] = dataGridView1.Rows[row_index];
                }
            }
            else if (cmd_name.StartsWith("Gain")) {
                if (_enabled_rows[2] == null) {
                    dataGridView1.Rows[row_index].Cells[1].Style.BackColor = Color.LightGreen;
                    _enabled_rows[2] = dataGridView1.Rows[row_index];
                }
                else {
                    _enabled_rows[2].Cells[1].Style.BackColor = Color.White;
                    dataGridView1.Rows[row_index].Cells[1].Style.BackColor = Color.LightGreen;
                    _enabled_rows[2] = dataGridView1.Rows[row_index];
                }
                GlobalUIManager.Instance.SetMaxRaw(0);
                GlobalUIManager.Instance.SetMinRaw(0);
            }
            else if (cmd_name.StartsWith("LPF")) {
                if (_enabled_rows[3] == null) {
                    byte cmd = Convert.ToByte(cmd_str, 16);
                    _serial_manage.SerialSendCmd(cmd);

                    dataGridView2.Rows[row_index].Cells[1].Style.BackColor = Color.LightGreen;
                    _enabled_rows[3] = dataGridView2.Rows[row_index];

                    if (_enabled_rows[4] == null) GlobalSerialManager.Instance.SetFilter(GlobalSerialManager.Filter.LPF);
                }
                else {
                    byte cmd = Convert.ToByte("0xFD", 16);
                    _serial_manage.SerialSendCmd(cmd);

                    _enabled_rows[3].Cells[1].Style.BackColor = Color.White;
                    if (_enabled_rows[4]!=null) _enabled_rows[4].Cells[1].Style.BackColor = Color.White;

                    _enabled_rows[3] = null;
                    _enabled_rows[4] = null;

                    GlobalSerialManager.Instance.SetFilter(GlobalSerialManager.Filter.Raw);
                }
            }

            else if (cmd_name.StartsWith("AF_")) {
                if ((_enabled_rows[4] == null)) {
                    byte cmd = Convert.ToByte(cmd_str, 16);
                    _serial_manage.SerialSendCmd(cmd);

                    dataGridView2.Rows[row_index].Cells[1].Style.BackColor = Color.LightGreen;
                    _enabled_rows[4] = dataGridView2.Rows[row_index];
                    GlobalSerialManager.Instance.SetFilter(GlobalSerialManager.Filter.AVG);
                }
                else if (_enabled_rows[4] != dataGridView2.Rows[row_index]) {
                    byte cmd = Convert.ToByte(cmd_str, 16);
                    _serial_manage.SerialSendCmd(cmd);

                    _enabled_rows[4].Cells[1].Style.BackColor = Color.White;
                    dataGridView2.Rows[row_index].Cells[1].Style.BackColor = Color.LightGreen;
                    _enabled_rows[4] = dataGridView2.Rows[row_index];
                    GlobalSerialManager.Instance.SetFilter(GlobalSerialManager.Filter.AVG);
                }
                else {
                    byte cmd = Convert.ToByte("0x2D", 16);
                    _serial_manage.SerialSendCmd(cmd);

                    _enabled_rows[4].Cells[1].Style.BackColor = Color.White;
                    _enabled_rows[4] = null;
                    if (_enabled_rows[3] != null) GlobalSerialManager.Instance.SetFilter(GlobalSerialManager.Filter.LPF);
                    else GlobalSerialManager.Instance.SetFilter(GlobalSerialManager.Filter.Raw);
                }
            }
            else {

            }
        }

        private void play_button_Click(object sender, EventArgs e) {
            if (!GlobalUIManager.Instance.GetIsTxtLogging()) {
                play_button.Image = Resources.Stop;
                GlobalUIManager.Instance.SetIsTxtLogging(true);
                GlobalLogManager.Instance.OpenDataLogFile();
            }
            else {
                play_button.Image = Resources.play;
                GlobalUIManager.Instance.SetIsTxtLogging(false);
                GlobalLogManager.Instance.CloseDataLogFile();
            }
        }

        /// <summary>
        /// 로그 파일 이름 설정 관련 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox2_Click(object sender, EventArgs e) {
            log_file_name_text_box.Text = GlobalConfigManager.Instance.GetNowLogDefaultFileName();
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
                    GlobalConfigManager.Instance.SetLogFolderPath(fbd.SelectedPath + @"\Log");
                    log_file_path_text_box.Text = fbd.SelectedPath + @"\Log";
                }

                Directory.CreateDirectory(GlobalConfigManager.Instance.GetLogFolderPath());
                Directory.CreateDirectory(GlobalConfigManager.Instance.GetDebugLogFolderPath());
                Directory.CreateDirectory(GlobalConfigManager.Instance.GetErrorLogFolderPath());
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hydrogen_percent_text_box_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                GlobalUIManager.Instance.SetHydrogenPercent(hydrogen_percent_text_box.Text.Substring(0, hydrogen_percent_text_box.TextLength - 1));
                hydrogen_percent_text_box.ForeColor = SystemColors.ControlDark;
            }
        }
        private void hydrogen_percent_text_box_Click(object sender, EventArgs e) {
            hydrogen_percent_text_box.ForeColor = Color.Black;
        }
        private void hydrogen_percent_text_box_Leave(object sender, EventArgs e) {
            hydrogen_percent_text_box.ForeColor = SystemColors.ControlDark;
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
