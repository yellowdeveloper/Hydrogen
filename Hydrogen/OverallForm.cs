using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hydrogen.UserControls;
using Hydrogen.SerialComm;
using Hydrogen.GlobalManagers;

namespace Hydrogen {
    public partial class OverallForm : Form {
        private Point mousePoint;

        MainPanel main_panel = new MainPanel();
        SidePanel side_panel = new SidePanel();
        HeaderPanel header_panel = new HeaderPanel();
        SerialManage serial_manage = new SerialManage();

        public OverallForm() {
            InitializeComponent();
            InitializePanels();

            //serial_manage.SerialConnect();

            header_panel.SetInterfaceLabelText($"Interface   :   {GlobalSerialManager.Instance.GetPortName()}");
            header_panel.SetBaudrateLabelText($"Baudrate   :   {GlobalSerialManager.Instance.GetBaudrate()}");
            header_panel.SetModelLabelText($"Model   :   {GlobalSerialManager.Instance.GetModelName()}");

            timer1.Start();
        }

        private void InitializePanels() {

            this.main_cont.Controls.Add(this.main_panel);
            this.main_panel.Dock = DockStyle.Fill;
            //this.main_panel.BorderStyle = BorderStyle.FixedSingle;

            this.side_cont.Controls.Add(this.side_panel);
            this.side_panel.Dock = DockStyle.Fill;
            //this.side_panel.BorderStyle = BorderStyle.FixedSingle;

            this.header_cont.Controls.Add(this.header_panel);
            this.header_panel.Dock = DockStyle.Fill;
            //this.header_panel.BorderStyle = BorderStyle.FixedSingle;
        }

        private void timer1_Tick(object sender, EventArgs e) {
            header_panel.SetDebugDataText($"{GlobalUIManager.Instance.GetDebugStat()}");
            this.main_panel.AddValueToChart();
        }

        private void tableLayoutPanel4_MouseDown(object sender, MouseEventArgs e) {
            mousePoint = new Point(e.X, e.Y);
        }

        private void tableLayoutPanel4_MouseMove(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                this.Location = new Point(this.Left - (mousePoint.X - e.X), this.Top - (mousePoint.Y - e.Y));
            }
        }

        private void minimize_button_Click(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
        }

        private void maximize_button_Click(object sender, EventArgs e) {
            if (this.WindowState == FormWindowState.Normal) {
                this.WindowState = FormWindowState.Maximized;
            }

            else if (this.WindowState == FormWindowState.Maximized) {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void close_button_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}