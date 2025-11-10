using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Hydrogen.GlobalManagers
{
    internal class GlobalLogManager {
        private static readonly GlobalLogManager _instance = new GlobalLogManager();
        public static GlobalLogManager Instance => _instance;

        public static event Action AutoEnd;

        private FileStream fs;
        private StreamWriter sw;
        private static readonly object _logLock = new object();

        private bool _auto_stop_enabled = false;
        private int _auto_stop_count = 100;
        private double _counter = 0;

        public void AddLogToFile(
            string type,
            string comment,
            string note = "-",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = "")
        {
            string log_file_path = GlobalConfigManager.Instance.GetDebugLogFilePath();
            try
            {
                string now_time = DateTime.Now.ToString("HH:mm:ss");

                if (type == "DEBUG" || type == "WARN") log_file_path = GlobalConfigManager.Instance.GetDebugLogFilePath();
                else if (type == "ERROR") log_file_path = GlobalConfigManager.Instance.GetErrorLogFilePath();

                string fileName = Path.GetFileName(filePath);

                // 소스 정보
                string source_line = $"Time: {now_time}: {fileName}: {memberName}(): Line {lineNumber}\n";

                string logMessage = source_line + note + comment + "\n";

                File.AppendAllText(log_file_path, logMessage);
            }
            catch (Exception ex) {
                Console.WriteLine($"Failed to write log: {ex.Message} {log_file_path}");
            }
        }

        public void ConsoleLog(
            string type,
            string comment,
            string note = " - ",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = "")
        {
            string now_time = DateTime.Now.ToString("HH:mm:ss");
            string fileName = Path.GetFileName(filePath);
            string source_line = $"[{now_time}]:{fileName}:{memberName}(): Line {lineNumber}";
            string logMessage = source_line + note + comment;

            if (type == "ERROR") {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (type == "WARN") {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }
            else if (type == "OK"){
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else {
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine(logMessage);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void DataValueLog() {
            if (Instance.GetAutoStopCount() <= Instance.GetCounter()) {
                Instance.CloseDataLogFile();

                GlobalUIManager.Instance.SetIsTxtLogging(false);

                Instance.SetCounter(0);

                AutoEnd?.Invoke();
                return;
            }

            string now_time = DateTime.Now.ToString("HH:mm:ssfff");
            string hydrogen_percent = GlobalUIManager.Instance.GetHydrogenPercent();

            string digital_count = GlobalSerialManager.Instance.GetSerialReceivedDataRaw();
            string lpf = "Not Set";
            string avg = "Not Set";

            string humidity;
            string temperature;

            GlobalSerialManager.Filter now_filter = GlobalSerialManager.Instance.GetFilter();

            if (now_filter == GlobalSerialManager.Filter.LPF) {
                lpf = GlobalSerialManager.Instance.GetSerialReceivedDataLPF();
            }
            else if (now_filter == GlobalSerialManager.Filter.AVG) {
                lpf = GlobalSerialManager.Instance.GetSerialReceivedDataLPF();
                avg = GlobalSerialManager.Instance.GetSerialReceivedDataAVG();
            }

            string data_log = $"{now_time}\t{digital_count}\t{lpf}\t{avg}\t{hydrogen_percent}\n";

            lock (_logLock) {
                sw.Write(data_log);
            }
        }

        public void OpenDataLogFile() {
            if (fs != null) return;

            string log_file_path = Path.Combine(GlobalConfigManager.Instance.GetLogFolderPath(), GlobalConfigManager.Instance.GetNowLogFileName());

            fs = new FileStream(log_file_path, FileMode.Append, FileAccess.Write, FileShare.None);
            sw = new StreamWriter(fs);

            sw.Write($"time\traw\tLPF\tAVG\tH2\n");
        }

        public void CloseDataLogFile() {
            string log_fin = $"\nAVG\t=AVERAGE(B2:INDEX(B:B, ROW()-2))\nMIN\t=MIN(B2:INDEX(B:B, ROW()-3))\nMAX\t=MAX(B2:INDEX(B:B, ROW()-4))" +
                $"\nSTDEV\t=STDEV(B2:INDEX(B:B, ROW()-5))\nMIN-MAX DIFF\t=INDEX(B:B, ROW()-2)-INDEX(B:B, ROW()-3)" +
                $"\n\n=LET(SourceArray, LEFT(A2:INDEX(A:A, ROW()-8), 8), GROUPBY(SourceArray,SourceArray,COUNTA,0,0))";

            lock (_logLock) {
                sw.Write(log_fin);
            }

            if (fs != null && sw != null) {
                sw.Close();
                fs.Close();
                
                sw = null;
                fs = null;
            }
        }

        public bool GetAutoStopEnabled() { return _auto_stop_enabled; }
        public void SetAutoStopEnabled(bool auto_stop_enabled) { _auto_stop_enabled = auto_stop_enabled; }

        public int GetAutoStopCount() { return _auto_stop_count; }
        public void SetAutoStopCount(int auto_stop_count) { _auto_stop_count = auto_stop_count; }

        public double GetCounter() { return _counter; }
        public void SetCounter(double counter) { _counter = counter; }
    }
}
