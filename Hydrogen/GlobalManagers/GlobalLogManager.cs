using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Hydrogen.GlobalManagers
{
    internal class GlobalLogManager {
        private static readonly GlobalLogManager _instance = new GlobalLogManager();
        public static GlobalLogManager Instance => _instance;

        private FileStream fs;
        private StreamWriter sw;
        private static readonly object _logLock = new object();

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

            if (type == "ERROR")
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (type == "WARN")
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }
            else if (type == "OK")
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine(logMessage);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void DataValueLog() {
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
            if (fs != null && sw != null) {
                sw.Close();
                fs.Close();
                
                sw = null;
                fs = null;
            }
        }
    }
}
