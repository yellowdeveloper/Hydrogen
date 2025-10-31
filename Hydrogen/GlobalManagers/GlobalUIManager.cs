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
        private int _time_tick = 100 ; // ms
        private int _x_scale = 1;     // sec
        private int _y_scale = 100;     // %
        private bool _is_graph_logging = false;
        private bool _is_csv_logging = false;

        public string GetDebugStat() { return _debug_stat; }
        public void SetDebugStat(string debug_stat) { _debug_stat = debug_stat; }

        public int GetTimeTick() { return _time_tick; }
        public void SetTimeTick(int time_tick) { _time_tick = time_tick; }

        public int GetXScale() { return _x_scale; }
        public void SetXScale(int x_scale) { _x_scale = x_scale; }

        public int GetYScale() { return _y_scale; }
        public void SetYScale(int y_scale) { _y_scale = y_scale; }

        public bool GetIsGraphLogging() { return _is_graph_logging; }
        public void SetIsGraphLogging(bool is_graph_logging) { _is_graph_logging = is_graph_logging; }


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
