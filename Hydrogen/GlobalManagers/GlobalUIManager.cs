using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hydrogen.GlobalManagers
{
    internal class GlobalUIManager {            
        private static readonly GlobalUIManager _instance = new GlobalUIManager();
        public static GlobalUIManager Instance => _instance;

        private string _debug_stat;

        public string GetDebugStat() { return _debug_stat; }
        public void SetDebugStat(string debug_stat) { _debug_stat = debug_stat; }

        public void DrawRectangle(Color color, TableLayoutCellPaintEventArgs e) {
            using (Pen pen = new Pen(color, 10)){
                e.Graphics.DrawRectangle(pen, e.CellBounds);
            }
            using (Brush brush = new SolidBrush(color)) {
                e.Graphics.FillRectangle(brush, e.CellBounds);
            }
        }
    }
}
