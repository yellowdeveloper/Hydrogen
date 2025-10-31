using Hydrogen.GlobalManagers;
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

namespace Hydrogen.Forms
{
    public partial class SetupForm : Form {

        private Point mousePoint;
        public SetupForm() {
            InitializeComponent();
            port_text_box.Text = GlobalSerialManager.Instance.GetPortName();
            baud_rate_text_box.Text = GlobalSerialManager.Instance.GetBaudrate().ToString();
            data_bits_text_box.Text = GlobalSerialManager.Instance.GetDataBits().ToString();

            stop_bits_combo.SelectedIndexChanged -= new System.EventHandler(this.stop_bits_combo_SelectedIndexChanged);
            parity_combo.SelectedIndexChanged -= new System.EventHandler(this.parity_combo_SelectedIndexChanged);

            stop_bits_combo.DataSource = Enum.GetValues(typeof(System.IO.Ports.StopBits))
                                     .Cast<System.IO.Ports.StopBits>()
                                     .ToArray();

            parity_combo.DataSource = Enum.GetValues(typeof(System.IO.Ports.Parity))
                                     .Cast<System.IO.Ports.Parity>()
                                     .ToArray();

            stop_bits_combo.SelectedItem = GlobalSerialManager.Instance.GetStopBits();
            parity_combo.SelectedItem = GlobalSerialManager.Instance.GetParity();

            stop_bits_combo.SelectedIndexChanged += new System.EventHandler(this.stop_bits_combo_SelectedIndexChanged);
            parity_combo.SelectedIndexChanged += new System.EventHandler(this.parity_combo_SelectedIndexChanged);

            if (GlobalSerialManager.Instance.GetDtrEnable()){
                rts_check_box.Checked = true;
                rts_check_box.Text = "True";
            }
            else {
                rts_check_box.Checked = false;
                rts_check_box.Text = "False";
            }

            if (GlobalSerialManager.Instance.GetRtsEnable()) {
                dtr_check_box.Checked = true;
                dtr_check_box.Text = "True";
            }
            else {
                dtr_check_box.Checked = false;
                dtr_check_box.Text = "False";
            }
        }

        private void tableLayoutPanel3_MouseDown(object sender, MouseEventArgs e) {
            mousePoint = new Point(e.X, e.Y);
        }

        private void tableLayoutPanel3_MouseMove(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                this.Location = new Point(this.Left - (mousePoint.X - e.X), this.Top - (mousePoint.Y - e.Y));
            }
        }

        private void close_btn_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void text_box_Click(object sender, MouseEventArgs e) {
            var textBox = sender as TextBox;
            textBox.Text = "";
            textBox.ForeColor = Color.Black;
        }

        private void data_bits_text_box_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                if (data_bits_text_box.Text != "") {
                    try {
                        GlobalSerialManager.Instance.SetDataBits(int.Parse(data_bits_text_box.Text));
                        data_bits_text_box.ForeColor = SystemColors.ControlDark;
                    }
                    catch (Exception ex) {
                        GlobalUIManager.Instance.SetDebugStat($"Serial Setup Error - {ex.Message}");
                        using (ErrorForm error_form = new ErrorForm()) {
                            error_form.ShowDialog();
                        }
                    }
                }
                else{
                    data_bits_text_box.Text = GlobalSerialManager.Instance.GetDataBits().ToString();
                    data_bits_text_box.ForeColor = SystemColors.ControlDark;
                }
            }
        }

        private void baud_rate_text_box_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                if (baud_rate_text_box.Text != "") {
                    try {
                        GlobalSerialManager.Instance.SetBaudrate(int.Parse(baud_rate_text_box.Text));
                        baud_rate_text_box.ForeColor = SystemColors.ControlDark;
                    }
                    catch (Exception ex) {
                        GlobalUIManager.Instance.SetDebugStat($"Serial Setup Error - {ex.Message}");
                        using (ErrorForm error_form = new ErrorForm()) {
                            error_form.ShowDialog();
                        }
                    }
                }
                else {
                    baud_rate_text_box.Text = GlobalSerialManager.Instance.GetBaudrate().ToString();
                    baud_rate_text_box.ForeColor = SystemColors.ControlDark;
                }
            }
        }

        private void port_text_box_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                if (port_text_box.Text != "") {
                    if (port_text_box.TextLength < 3)
                    {
                        GlobalUIManager.Instance.SetDebugStat($"Serial Setup Error - Invalid Port Name {port_text_box.Text}");
                        using (ErrorForm error_form = new ErrorForm())
                        {
                            error_form.ShowDialog();
                        }
                    }
                    else if (port_text_box.Text.Substring(0, 3) != "COM") {
                        GlobalUIManager.Instance.SetDebugStat($"Serial Setup Error - Invalid Port Name {port_text_box.Text}");
                        using (ErrorForm error_form = new ErrorForm()) {
                            error_form.ShowDialog();

                        }
                    }
                    else {
                        GlobalSerialManager.Instance.SetPortName(port_text_box.Text);
                        port_text_box.ForeColor = SystemColors.ControlDark;
                    }
                }
                else {
                    port_text_box.Text = GlobalSerialManager.Instance.GetPortName();
                    port_text_box.ForeColor = SystemColors.ControlDark;
                }
            }
        }

        private void port_text_box_TextChanged(object sender, EventArgs e) {
            port_text_box.Text = port_text_box.Text.ToUpper();
            port_text_box.SelectionStart = port_text_box.Text.Length;
        }

        private void parity_combo_SelectedIndexChanged(object sender, EventArgs e) {
            var selected = (System.IO.Ports.Parity)parity_combo.SelectedItem;
            GlobalSerialManager.Instance.SetParity(selected);
        }

        private void stop_bits_combo_SelectedIndexChanged(object sender, EventArgs e) {
            var selected = (System.IO.Ports.StopBits)stop_bits_combo.SelectedItem;
            GlobalSerialManager.Instance.SetStopBits(selected);
        }

        private void rts_check_box_CheckedChanged(object sender, EventArgs e) {
            if (rts_check_box.Checked) {
                GlobalSerialManager.Instance.SetRtsEnable(true);
                rts_check_box.Text = "True";
            }
            else {
                GlobalSerialManager.Instance.SetRtsEnable(false);
                rts_check_box.Text = "False";
            }
        }

        private void dtr_check_box_CheckedChanged(object sender, EventArgs e) {
            if (dtr_check_box.Checked) {
                GlobalSerialManager.Instance.SetDtrEnable(true);
                dtr_check_box.Text = "True";
            }
            else {
                GlobalSerialManager.Instance.SetDtrEnable(false);
                dtr_check_box.Text = "False";
            }
        }

        private void port_text_box_Leave(object sender, EventArgs e) {
            port_text_box.Text = GlobalSerialManager.Instance.GetPortName();
            port_text_box.ForeColor = SystemColors.ControlDark;
        }

        private void baud_rate_text_box_Leave(object sender, EventArgs e) {
            baud_rate_text_box.Text = GlobalSerialManager.Instance.GetBaudrate().ToString();
            baud_rate_text_box.ForeColor = SystemColors.ControlDark;
        }

        private void data_bits_text_box_Leave(object sender, EventArgs e) {
            data_bits_text_box.Text = GlobalSerialManager.Instance.GetDataBits().ToString();
            data_bits_text_box.ForeColor = SystemColors.ControlDark;
        }
    }
}
