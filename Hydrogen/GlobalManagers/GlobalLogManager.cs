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
        private int _auto_stop_count = 1;
        private double _counter = 0;
        private double _now_spent = 0;

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
            if (Instance.GetAutoStopEnabled() && Instance.GetCounter() != 0 && (Instance.GetCounter() <= Instance.GetNowSpent())) {
                Instance.CloseDataLogFile();

                GlobalUIManager.Instance.SetIsTxtLogging(false);

                Instance.SetCounter(0);

                AutoEnd?.Invoke();
                return;
            }

            string now_time = DateTime.Now.ToString("yyMMdd_HH:mm:ss");
            string time_spent = Instance.GetNowSpent().ToString();
            string hydrogen_percent = GlobalUIManager.Instance.GetHydrogenPercent();

            string digital_count = GlobalSerialManager.Instance.GetSerialReceivedDataRaw();
            string saf = "Not Set";
            string lpf = "Not Set";
            string maf = "Not Set";

            string humidity;
            string temperature;

            if (GlobalSerialManager.Instance.GetIsSafEnabled()) saf = GlobalSerialManager.Instance.GetSerialReceivedDataSAF();
            if (GlobalSerialManager.Instance.GetIsLpfEnabled()) lpf = GlobalSerialManager.Instance.GetSerialReceivedDataLPF();
            if (GlobalSerialManager.Instance.GetIsMafEnabled()) maf = GlobalSerialManager.Instance.GetSerialReceivedDataMAF();

            string data_log = $"{now_time}\t{time_spent}\t{digital_count}\t{saf}\t{lpf}\t{maf}\n";

            lock (_logLock) {
                sw.Write(data_log);
            }
        }

        public void OpenDataLogFile() {
            if (fs != null) return;

            string log_file_path = Path.Combine(GlobalConfigManager.Instance.GetLogFolderPath(), GlobalConfigManager.Instance.GetNowLogFileName());

            fs = new FileStream(log_file_path, FileMode.Append, FileAccess.Write, FileShare.None);
            sw = new StreamWriter(fs);

            sw.Write($"date/time\ttime spent(ms)\traw\tSAF\tLPF\tMAF\n");
        }

        public void CloseDataLogFile() {
            string gain = GlobalUIManager.Instance.GetGain();
            string dr = GlobalUIManager.Instance.GetDataRate();
            string h2 = GlobalUIManager.Instance.GetHydrogenPercent();
            

            string log_fin = $"\n\tAVG\t=AVERAGE(C2:INDEX(C:C, ROW()-2))\t=AVERAGE(D2:INDEX(D:D, ROW()-2))\t=AVERAGE(E2:INDEX(E:E, ROW()-2))" +
                $"\n\tMIN\t=MIN(C2:INDEX(C:C, ROW()-3))\t=MIN(D2:INDEX(D:D, ROW()-3))\t=MIN(E2:INDEX(E:E, ROW()-3))" +
                $"\n\tMAX\t=MAX(C2:INDEX(C:C, ROW()-4))\t=MAX(D2:INDEX(D:D, ROW()-4))\t=MAX(E2:INDEX(E:E, ROW()-4))" +
                $"\n\tSTDEV\t=STDEV(C2:INDEX(C:C, ROW()-5))\t=STDEV(D2:INDEX(D:D, ROW()-5))\t=STDEV(E2:INDEX(E:E, ROW()-5))" +
                $"\n\tMIN-MAX DIFF\t=INDEX(C:C, ROW()-2)-INDEX(C:C, ROW()-3)\t=INDEX(D:D, ROW()-2)-INDEX(D:D, ROW()-3)\t=INDEX(E:E, ROW()-2)-INDEX(E:E, ROW()-3)" +
                $"\n\n=LET(SourceArray, A2:INDEX(A:A, ROW()-8), GROUPBY(SourceArray,SourceArray,COUNTA,0,0))\t\t\tGain\t{gain}" +
                $"\n\t\t\tData Rate\t{dr}\n\t\t\tH2%\t{h2}";

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
        public double GetNowSpent() { return _now_spent; }
        public void SetNowSpent(double now_spent) { _now_spent = now_spent; }
    }
}
