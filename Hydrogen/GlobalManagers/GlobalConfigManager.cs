using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Hydrogen.GlobalManagers {
    internal class GlobalConfigManager {
        private static readonly GlobalConfigManager _instance = new GlobalConfigManager();
        public static GlobalConfigManager Instance => _instance;

        public static event Action OnOptionSaved;

        private GlobalConfigManager() {
            initialize();
        }

        private Dictionary<string, string> _data_cmds = new Dictionary<string, string>();
        private Dictionary<string, string> _ctrl_cmds = new Dictionary<string, string>();

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
        private string _debug_log_folder_path = @"DEBUG";
        private string _error_log_folder_path = @"ERROR";

        private string _now_log_file_name;

        private int same_file_cnt = 0;
        public string GetLogFolderPath() {
            return _log_folder_path ?? string.Empty;
        }

        public void SetLogFolderPath(string log_folder_path) {
            _log_folder_path = log_folder_path;
        }

        public string GetNowLogDefaultFileName() {
            DateTime now = DateTime.Now;
            _now_log_file_name = $"Hydrogen_Sense_{now.ToString("yyyyMMdd_HH")}_H2-{GlobalUIManager.Instance.GetHydrogenPercent()}_{same_file_cnt}.txt";
            string now_log_file_path = Path.Combine(_log_folder_path, _now_log_file_name);
            if (!File.Exists(now_log_file_path)) {
                same_file_cnt = 0;
                return _now_log_file_name ?? string.Empty;
            }
            else {
                same_file_cnt++;
                return GetNowLogDefaultFileName();
            }
        }

        public string GetNowLogFileName() {
            return _now_log_file_name;
        }

        public void SetLogFileName(string log_file_name) {
            _now_log_file_name = log_file_name;
        }

        public string GetDebugLogFolderPath() {
            string log_folder_path = Path.Combine(_log_folder_path, _debug_log_folder_path);
            return log_folder_path ?? string.Empty;
        }

        public string GetDebugLogFilePath() {
            string log_file_name = "DEBUG_Log.txt";
            string now_dt = DateTime.Now.ToString("yyyy.MM.dd_");
            string path = GetDebugLogFolderPath();

            log_file_name = now_dt + log_file_name;

            string log_file_path = Path.Combine(path, log_file_name);

            return log_file_path ?? string.Empty;
        }

        public string GetErrorLogFolderPath() {
            string log_folder_path = Path.Combine(_log_folder_path, _error_log_folder_path);
            return log_folder_path ?? string.Empty;
        }


        public string GetErrorLogFilePath() {
            string log_file_name = "ERROR_Log.txt";
            string now_dt = DateTime.Now.ToString("yyyy.MM.dd_");
            string path = GetErrorLogFolderPath();

            log_file_name = now_dt + log_file_name;

            string log_file_path = Path.Combine(path, log_file_name);

            return log_file_path ?? string.Empty;
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
                    { "DTREnable", "False" }
                }
            },
            {
                "DataCommandInfo", new Dictionary<string, string> {
                    { "Raw", "-" },
                    { "LPF", "0xFA" },
                    { "AF_2", "0x2A" },
                    { "AF_4", "0x4A" },
                    { "AF_8", "0x8A" }
                }
            },
            { 
                "CtrlCommandInfo", new Dictionary<string, string> {
                    { "ReadStart", "0x10" },
                    { "ReadStop", "0x02" },
                    { "Gain1", "0x30" },
                    { "Gain2", "0x32" },
                    { "Gain4", "0x34" },
                    { "Gain8", "0x36" },
                    { "Gain16", "0x38" },
                    { "Gain32", "0x3A" },
                    { "Gain64", "0x3C" },
                    { "Gain128", "0x3E" },
                    { "DR45", "0x20" },
                    { "DR90", "0x40" },
                    { "DR175", "0x60" },
                    { "DR330", "0x80" },
                    { "DR600", "0xA0" },
                    { "DR1000", "0xC0" }
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

            OnOptionSaved?.Invoke();
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
                break; // Only serialize the first section (SerialInfo)
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
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileSection")]
        private static extern int GetPrivateProfileSection(string lpAppName, byte[] lpReturnedString, int nSize, string lpFileName);

        public void WriteConfig(string path, string section, string key, string val) { WritePrivateProfileString(section, key, val, path); }
        public string ReadConfig(string path, string Section, string Key, string Default) {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, Default, temp, 255, path);
            if (temp != null && temp.Length > 0) return temp.ToString();
            else return Default;
        }
        public Dictionary<string, string> ReadConfigSection(string path, string section) {
            var result = new Dictionary<string, string>();
            byte[] buffer = new byte[8192];
            int bytesRead = GetPrivateProfileSection(section, buffer, buffer.Length, path);

            if (bytesRead > 0) {
                string sectionData = Encoding.Default.GetString(buffer, 0, bytesRead).Trim('\0');

                string[] pairs = sectionData.Split('\0');

                foreach (string pair in pairs) {
                    if (string.IsNullOrWhiteSpace(pair)) continue;

                    int equalIndex = pair.IndexOf('=');
                    if (equalIndex > 0) {
                        string key = pair.Substring(0, equalIndex);
                        string value = pair.Substring(equalIndex + 1);
                        result[key] = value;
                    }
                }
            }
            return result;
        }
        public void CmdSectionInit(string config_file_path) {

            if(File.Exists(config_file_path)) {
                _data_cmds = ReadConfigSection(config_file_path, "DataCommandInfo");
                //Console.WriteLine($"\n Num of controls :: {_data_cmds.Count}");
            }

            if (File.Exists(config_file_path)) {
                _ctrl_cmds = ReadConfigSection(config_file_path, "CtrlCommandInfo");
            }
        }

        public Dictionary<string, string> GetDataCmdSection() {
            if (_data_cmds.Count > 0) {
                Dictionary<string, string> cmds = new Dictionary<string, string>(_data_cmds);
                foreach (var item in _data_cmds) {
                    cmds[item.Key] = item.Value;
                }
                return cmds;
            }
            else {
                return null;
            }
        }

        public Dictionary<string, string> GetCtrlCmdSection() {
            if (_ctrl_cmds.Count > 0) {
                Dictionary<string, string> cmds = new Dictionary<string, string>(_ctrl_cmds);
                foreach (var item in _ctrl_cmds) {
                    cmds[item.Key] = item.Value;
                }
                return cmds;
            }
            else {
                return null;
            }
        }
    }
}
