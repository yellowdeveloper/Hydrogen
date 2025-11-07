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
        private double _y_scale = 100.0f;     // %
        private string _hydrogen_percent = "1";
        private bool _is_graph_logging = false;
        private bool _is_txt_logging = false;
        private int _max_raw = 0;
        private int _min_raw = 0;
        private int _min_max_diff = 0;

        public string GetDebugStat() { return _debug_stat; }
        public void SetDebugStat(string debug_stat) { _debug_stat = debug_stat; }

        public int GetTimeTick() { return _time_tick; }
        public void SetTimeTick(int time_tick) { _time_tick = time_tick; }

        public int GetXScale() { return _x_scale; }
        public void SetXScale(int x_scale) { _x_scale = x_scale; }

        public double GetYScale() { return _y_scale; }
        public void SetYScale(double y_scale) { _y_scale = y_scale; }

        public string GetHydrogenPercent() { return _hydrogen_percent; }
        public void SetHydrogenPercent(string hydrogen_percent) { _hydrogen_percent = hydrogen_percent; }

        public bool GetIsGraphLogging() { return _is_graph_logging; }
        public void SetIsGraphLogging(bool is_graph_logging) { _is_graph_logging = is_graph_logging; }

        public bool GetIsTxtLogging() { return _is_txt_logging; }
        public void SetIsTxtLogging(bool is_txt_logging) { _is_txt_logging = is_txt_logging; }

        public int GetMaxRaw() { return _max_raw; }
        public void SetMaxRaw(int max_raw) { _max_raw = max_raw; }

        public int GetMinRaw() { return _min_raw; }
        public void SetMinRaw(int min_raw) { _min_raw = min_raw; }

        public int GetDiffRaw() { return _min_max_diff; }
        public void SetDiffRaw(int min_max_diff) { _min_max_diff = min_max_diff; }


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
