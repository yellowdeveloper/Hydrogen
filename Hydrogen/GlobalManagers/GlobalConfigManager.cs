using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen.GlobalManagers {
    internal class GlobalConfigManager {
        private static readonly GlobalConfigManager _instance = new GlobalConfigManager();
        public static GlobalConfigManager Instance => _instance;

        private GlobalConfigManager() {
            initialize();
        }

        // Config Folder Path
        private string _config_folder_path = @"Config\";
        private string _config_file_name = "init_config.ini";

        public string GetConfigFolderPath() {
            return _config_folder_path ?? string.Empty; ;
        }


        public string GetConfigFileName() {
            return _config_file_name ?? string.Empty;
        }

        // Log Folder Path
        private string _log_folder_path = @"Log\";
        private string _now_log_file_name;
        private int same_file_cnt = 0;
        public string GetLogFolderPath() {
            return _log_folder_path ?? string.Empty;
        }

        public void SetLogFolderPath(string log_folder_path) {
            _log_folder_path = log_folder_path;
        }

        public string GetNowLogFileName() {
            DateTime now = DateTime.Now;
            _now_log_file_name = $"Hydrogen_Sense_{now.ToString("yyyyMMdd_HH")}_{same_file_cnt}.csv";
            string now_log_file_path = Path.Combine(_log_folder_path, _now_log_file_name);
            if (!File.Exists(now_log_file_path)) {
                same_file_cnt = 0;
                return _now_log_file_name ?? string.Empty;
            }
            else {
                same_file_cnt++;
                return GetNowLogFileName();
            }
        }

        public void SetLogFileName(string log_file_name) {
            _now_log_file_name = log_file_name;
        }

        private Dictionary<string, Dictionary<string, string>> _config = new Dictionary<string, Dictionary<string, string>> {
            {
                "SerialInfo", new Dictionary<string, string> {
                    { "PortName", "COM6" },
                    { "Baudrate",  "115200" },
                    { "DataBits", "8" },
                    { "Parity", "None" },
                    { "StopBits", "One" },
                    { "RTSEnable", "False" },
                    { "DTREnable", "False" },
                }
            }
        };
        public void SetConfig() {
            _config["SerialInfo"]["PortName"] = GlobalSerialManager.Instance.GetPortName();
            _config["SerialInfo"]["Baudrate"] = GlobalSerialManager.Instance.GetBaudrate().ToString();
            _config["SerialInfo"]["DataBits"] = GlobalSerialManager.Instance.GetDataBits().ToString();
            _config["SerialInfo"]["Parity"] = GlobalSerialManager.Instance.GetParity().ToString();
            _config["SerialInfo"]["StopBits"] = GlobalSerialManager.Instance.GetStopBits().ToString();
            _config["SerialInfo"]["RTSEnable"] = GlobalSerialManager.Instance.GetRtsEnable().ToString();
            _config["SerialInfo"]["DTREnable"] = GlobalSerialManager.Instance.GetDtrEnable().ToString();
        }
        public Dictionary<string, Dictionary<string, string>> GetInitConfig() {
            return _config;
        }
        public string ConvertConfigToString() {
            StringBuilder sb = new StringBuilder();

            foreach (var section in _config) {
                sb.AppendLine($"[{section.Key}]");
                foreach (var key in section.Value) {
                    sb.AppendLine($"{key.Key}={key.Value}");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
        private void initialize() {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string exeDirectory = Path.GetDirectoryName(exePath);

            _config_folder_path = Path.Combine(exeDirectory, _config_folder_path);
            _log_folder_path = Path.Combine(exeDirectory, _log_folder_path);
        }

        [DllImport("Kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("Kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public void WriteConfig(string path, string section, string key, string val) { WritePrivateProfileString(section, key, val, path); }
        public string ReadConfig(string path, string Section, string Key, string Default) {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, Default, temp, 255, path);
            if (temp != null && temp.Length > 0) return temp.ToString();
            else return Default;
        }
    }
}
