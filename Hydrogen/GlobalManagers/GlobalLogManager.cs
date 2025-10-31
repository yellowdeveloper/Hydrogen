using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen.GlobalManagers
{
    internal class GlobalLogManager {
        private static readonly GlobalLogManager _instance = new GlobalLogManager();
        public static GlobalLogManager Instance => _instance;

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
        }
    }
}
