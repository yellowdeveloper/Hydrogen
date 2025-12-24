using Hydrogen.Forms;
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

        private const byte filter_en = 0xFA;
        private const byte filter_dis = 0xFD;
        private const byte ctrl_en = 0xCA;

        private bool is_mouse_over;
        private SerialManage _serial_manage;

        private DataGridViewRow[] _enabled_rows = new DataGridViewRow[6];
        public SidePanel() {
            InitializeComponent();
            log_file_name_text_box.Text = GlobalConfigManager.Instance.GetNowLogDefaultFileName();
            log_file_path_text_box.Text = GlobalConfigManager.Instance.GetLogFolderPath();
            hydrogen_percent_text_box.Text = GlobalUIManager.Instance.GetHydrogenPercent().ToString();
            auto_stop_text_box.Text = GlobalLogManager.Instance.GetAutoStopCount().ToString();
            InitializeCommands();

            InitializeManager.OnProgramInitialized += InitializeCommands;
            GlobalConfigManager.OnOptionSaved += SaveCommands;
            GlobalSerialManager.FilterStatusChanged += FilterOff;

            this.Disposed += (s, e) => {
                InitializeManager.OnProgramInitialized -= InitializeCommands;
                GlobalConfigManager.OnOptionSaved -= SaveCommands;
                GlobalSerialManager.FilterStatusChanged -= FilterOff;
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
                    _serial_manage.SerialSendCmd(ctrl_en, cmd);

                    CheckCommand(cmd_name, cmd_str, e.RowIndex);

                    dataGridView1[1, e.RowIndex].Style.BackColor = Color.LightGreen;
                }
                catch (Exception ex) {
                    GlobalLogManager.Instance.ConsoleLog("WARN", $"Error while Sending Command :: {ex}");
                    GlobalLogManager.Instance.AddLogToFile("WARN", $"Error while Sending Command :: {ex}");
                }
            }
        }

        public void AutoCheckChange() {
            checkBox1.Checked = !GlobalLogManager.Instance.GetAutoStopEnabled();
            play_button.Image = Resources.play;
            rec_state_label.BackColor = Color.White;
            rec_state_label.ForeColor = Color.DarkGray;
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
                    GlobalUIManager.Instance.SetGain(cmd_name.Substring(4));
                    GlobalLogManager.Instance.ConsoleLog("COM", $"Gain Set To {GlobalUIManager.Instance.GetGain()}");
                }
                else {
                    _enabled_rows[2].Cells[1].Style.BackColor = Color.White;
                    dataGridView1.Rows[row_index].Cells[1].Style.BackColor = Color.LightGreen;
                    _enabled_rows[2] = dataGridView1.Rows[row_index];
                    GlobalUIManager.Instance.SetGain(cmd_name.Substring(4));
                }
                GlobalUIManager.Instance.SetMaxRaw(0);
                GlobalUIManager.Instance.SetMinRaw(0);
            }
            else if (cmd_name.StartsWith("SAF_")) {
                if ((_enabled_rows[3] == null)) {
                    byte cmd = Convert.ToByte(cmd_str, 16);
                    _serial_manage.SerialSendCmd(filter_en, cmd);

                    dataGridView2.Rows[row_index].Cells[1].Style.BackColor = Color.LightGreen;
                    _enabled_rows[3] = dataGridView2.Rows[row_index];

                    GlobalSerialManager.Instance.SetIsSafEnabled(true);
                    GlobalUIManager.Instance.SetSampleRate(cmd_name.Substring(4));
                }
                else if (_enabled_rows[3] != dataGridView2.Rows[row_index]) {
                    byte cmd = Convert.ToByte(cmd_str, 16);
                    _serial_manage.SerialSendCmd(filter_en, cmd);

                    _enabled_rows[3].Cells[1].Style.BackColor = Color.White;
                    dataGridView2.Rows[row_index].Cells[1].Style.BackColor = Color.LightGreen;
                    _enabled_rows[3] = dataGridView2.Rows[row_index];

                    GlobalSerialManager.Instance.SetIsSafEnabled(true);
                    GlobalUIManager.Instance.SetSampleRate(cmd_name.Substring(4));
                }
                else {
                    byte cmd = Convert.ToByte("0x4D", 16);
                    _serial_manage.SerialSendCmd(filter_dis, cmd);

                    GlobalSerialManager.Instance.SetIsSafEnabled(false);
                }
            }
            else if (cmd_name.StartsWith("LPF")) {
                if (_enabled_rows[4] == null) {
                    byte cmd = Convert.ToByte(cmd_str, 16);
                    _serial_manage.SerialSendCmd(filter_en,cmd);

                    dataGridView2.Rows[row_index].Cells[1].Style.BackColor = Color.LightGreen;
                    _enabled_rows[4] = dataGridView2.Rows[row_index];

                    GlobalSerialManager.Instance.SetIsLpfEnabled(true);
                }
                else {
                    byte cmd = Convert.ToByte("0xFD", 16);
                    _serial_manage.SerialSendCmd(filter_dis, cmd);

                    GlobalSerialManager.Instance.SetIsLpfEnabled(false);
                }
            }
            else if (cmd_name.StartsWith("MAF_")) {
                if ((_enabled_rows[5] == null)) {
                    byte cmd = Convert.ToByte(cmd_str, 16);
                    _serial_manage.SerialSendCmd(filter_en, cmd);

                    dataGridView2.Rows[row_index].Cells[1].Style.BackColor = Color.LightGreen;
                    _enabled_rows[5] = dataGridView2.Rows[row_index];

                    GlobalSerialManager.Instance.SetIsMafEnabled(true);
                }
                else if (_enabled_rows[5] != dataGridView2.Rows[row_index]) {
                    byte cmd = Convert.ToByte(cmd_str, 16);
                    _serial_manage.SerialSendCmd(filter_en, cmd);

                    _enabled_rows[5].Cells[1].Style.BackColor = Color.White;
                    dataGridView2.Rows[row_index].Cells[1].Style.BackColor = Color.LightGreen;
                    _enabled_rows[5] = dataGridView2.Rows[row_index];

                    GlobalSerialManager.Instance.SetIsMafEnabled(true);
                }
                else {
                    byte cmd = Convert.ToByte("0x2D", 16);
                    _serial_manage.SerialSendCmd(filter_dis, cmd);

                    GlobalSerialManager.Instance.SetIsMafEnabled(false);
                }
            }
        }

        private void FilterOff(int filter_no) {
            switch (filter_no)
            {
                case 0:
                    _enabled_rows[3].Cells[1].Style.BackColor = Color.White;
                    _enabled_rows[3] = null;
                    break;
                case 1:
                    _enabled_rows[4].Cells[1].Style.BackColor = Color.White;
                    _enabled_rows[4] = null;
                    break;
                case 2:
                    _enabled_rows[5].Cells[1].Style.BackColor = Color.White;
                    _enabled_rows[5] = null;
                    break;
                default:
                    break;
            }
        }

        private void play_button_Click(object sender, EventArgs e) {
            if (!GlobalUIManager.Instance.GetIsTxtLogging()) {
                play_button.Image = Resources.Stop;
                rec_state_label.BackColor = Color.IndianRed;
                rec_state_label.ForeColor = Color.White;
                GlobalUIManager.Instance.SetIsTxtLogging(true);
                GlobalLogManager.Instance.OpenDataLogFile();
            }
            else {
                play_button.Image = Resources.play;
                rec_state_label.BackColor = Color.White;
                rec_state_label.ForeColor = Color.DarkGray;
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
        /// Handle Auto Stop Options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void auto_stop_text_box_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                try {
                    GlobalLogManager.Instance.SetAutoStopCount(Int32.Parse(auto_stop_text_box.Text));
                }
                catch {
                    GlobalUIManager.Instance.SetDebugStat($"Setup Error - Invalid Counter Value {auto_stop_text_box.Text}");
                    using (ErrorForm error_form = new ErrorForm()) {
                        error_form.ShowDialog();
                    }
                }
                auto_stop_text_box.ForeColor = SystemColors.ControlDark;
            }
        }
        private void auto_stop_text_box_Click(object sender, EventArgs e) {
            auto_stop_text_box.ForeColor = Color.Black;
        }
        private void auto_stop_text_box_Leave(object sender, EventArgs e) {
            auto_stop_text_box.ForeColor = SystemColors.ControlDark;
        }

        private void auto_stop_check_box_CheckedChanged(object sender, EventArgs e) {
            if (!GlobalLogManager.Instance.GetAutoStopEnabled()) {
                GlobalLogManager.Instance.SetAutoStopEnabled(true);
            }
            else {
                GlobalLogManager.Instance.SetAutoStopEnabled(false);
                GlobalLogManager.Instance.SetCounter(0);
            }
            
        }
        private void auto_stop_text_box_MouseHover(object sender, EventArgs e) {
            auto_stop_count_tool_tip.SetToolTip(auto_stop_text_box, "단위: ms");
        }

        /// <summary>
        /// Modify Hydrogen Percent Written In Log File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hydrogen_percent_text_box_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                GlobalUIManager.Instance.SetHydrogenPercent(hydrogen_percent_text_box.Text);
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
