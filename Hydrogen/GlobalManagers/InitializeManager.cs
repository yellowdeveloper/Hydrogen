using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Hydrogen.GlobalManagers
{
    internal class InitializeManager {
        public static void InitializeProgram() {
            InitializePaths();
            InitializeSettings();
        }
        private static void CreateInitConfigFile(string configFilePath) {
            string DefaultConfig = GlobalConfigManager.Instance.ConvertConfigToString();
            File.WriteAllText(configFilePath, DefaultConfig);
        }
        private static void InitializePaths () {
            string config_folder_path = GlobalConfigManager.Instance.GetConfigFolderPath();
            string config_file_name = GlobalConfigManager.Instance.GetConfigFileName();
            string log_folder_path = GlobalConfigManager.Instance.GetDefaultLogFolderPath();

            try {
                Directory.CreateDirectory(config_folder_path);
                Directory.CreateDirectory(log_folder_path);
            }
            catch {
                Debug.WriteLine("Folder Create Error");
            }

            //if (!File.Exists(Path.Combine(config_folder_path, config_file_name))) {
            CreateInitConfigFile(Path.Combine(config_folder_path, config_file_name));
            //}
        }

        private static void InitializeSettings () {
            string config_folder_path = GlobalConfigManager.Instance.GetConfigFolderPath();
            string config_file_name = GlobalConfigManager.Instance.GetConfigFileName();
            string config_file_path = Path.Combine(config_folder_path, config_file_name);

            if (!File.Exists(config_file_path)) {
                CreateInitConfigFile(config_file_path);
            }
            else {
                GlobalSerialManager.Instance.SetModelName(GlobalConfigManager.Instance.ReadConfig(config_file_path, "SystemInfo", "ModelName", "Default"));
                GlobalSerialManager.Instance.SetPortName(GlobalConfigManager.Instance.ReadConfig(config_file_path, "SystemInfo", "PortName", "Default"));
                GlobalSerialManager.Instance.SetBaudrate(Int32.Parse(GlobalConfigManager.Instance.ReadConfig(config_file_path, "SystemInfo", "Baudrate", "Default")));
                GlobalSerialManager.Instance.SetDataBits(Int32.Parse(GlobalConfigManager.Instance.ReadConfig(config_file_path, "SystemInfo", "DataBits", "Default")));
            }
        }
    }
}
