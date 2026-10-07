using Hydrogen.GlobalManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hydrogen.Forms
{
    public partial class ErrorForm : Form
    {
        public ErrorForm(string msg = null) {
            InitializeComponent();
            if (msg != null)
            {
                this.label1.Text = msg;
                return;
            }
            this.label1.Text = GlobalUIManager.Instance.GetDebugStat();
        }

        private void button1_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
