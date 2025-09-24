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
    public partial class HeaderPanel : UserControl {
        SerialManage serial_manage = new SerialManage();
        public HeaderPanel() {
            InitializeComponent();
        }
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

        private void tableLayoutPanel6_CellPaint(object sender, TableLayoutCellPaintEventArgs e) {
            GlobalUIManager.Instance.DrawRectangle(Color.FromArgb(163, 199, 249), e);
        }

        private void tableLayoutPanel2_CellPaint(object sender, TableLayoutCellPaintEventArgs e) {
            GlobalUIManager.Instance.DrawRectangle(Color.FromArgb(163, 199, 249), e);
        }

        private void cnt_but_Click(object sender, EventArgs e) {
            serial_manage.SerialConnect();
            Debug.Write("Coneect clicked");
        }
    }
}
