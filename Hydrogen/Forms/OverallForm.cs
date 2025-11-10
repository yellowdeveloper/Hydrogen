using Hydrogen.GlobalManagers;
using Hydrogen.SerialComm;
using Hydrogen.UserControls;
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
using System.Windows.Forms.DataVisualization.Charting;

namespace Hydrogen {
    public partial class OverallForm : Form {
        private Point mousePoint;

        MainPanel main_panel = new MainPanel();
        SidePanel side_panel = new SidePanel();
        HeaderPanel header_panel = new HeaderPanel();
        private readonly SerialManage serial_manage = new SerialManage();


        public OverallForm() {
            InitializeComponent();
            InitializePanels();

            header_panel.SetInterfaceLabelText($"Interface   :   {GlobalSerialManager.Instance.GetPortName()}");
            header_panel.SetBaudrateLabelText($"Baudrate   :   {GlobalSerialManager.Instance.GetBaudrate()}");
            header_panel.SetModelLabelText($"Model   :   {GlobalSerialManager.Instance.GetModelName()}");

            timer1.Start();

            GlobalLogManager.AutoEnd += AutoEndEvent;

            this.Disposed += (s, e) => {
                GlobalLogManager.AutoEnd -= AutoEndEvent;
            };
        }

        private void InitializePanels() {

            this.main_cont.Controls.Add(this.main_panel);
            this.main_panel.Dock = DockStyle.Fill;

            this.side_cont.Controls.Add(this.side_panel);
            this.side_panel.Dock = DockStyle.Fill;
            this.side_panel.InitializeSerialManager(this.serial_manage);

            this.header_cont.Controls.Add(this.header_panel);
            this.header_panel.Dock = DockStyle.Fill;
            this.header_panel.InitializeSerialManager(this.serial_manage);
        }

        private void timer1_Tick(object sender, EventArgs e) {
            header_panel.SetDebugDataText($"{GlobalUIManager.Instance.GetDebugStat()}");
            string[] series;

            if (GlobalSerialManager.Instance.GetFilter() == GlobalSerialManager.Filter.LPF)
                series = new string[] { "Raw", "LPF" };
            else if (GlobalSerialManager.Instance.GetFilter() == GlobalSerialManager.Filter.AVG)
                series = new string[] { "Raw", "LPF", "AVG" };
            else series = new string[] { "Raw" };
            try {
                this.main_panel.UpdateChart(series);
            }
            catch (Exception ex) {
                GlobalLogManager.Instance.ConsoleLog("ERROR", $"Error Occured while Updating Chart{ex}");
            }
        }

        private void AutoEndEvent() {
            if (this.side_panel.InvokeRequired) {
                this.side_panel.Invoke(new Action(() => { this.side_panel.AutoCheckChange(); }));
            }
            else {
                this.side_panel.AutoCheckChange();
            }
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