using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
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
            string log_folder_path = GlobalConfigManager.Instance.GetLogFolderPath();

            try {
                Directory.CreateDirectory(config_folder_path);
                Directory.CreateDirectory(log_folder_path);
            }
            catch {
                Debug.WriteLine("Folder Create Error");
            }
        }

        private static void InitializeSettings () {
            string config_folder_path = GlobalConfigManager.Instance.GetConfigFolderPath();
            string config_file_name = GlobalConfigManager.Instance.GetConfigFileName();
            string config_file_path = Path.Combine(config_folder_path, config_file_name);

            if (!File.Exists(config_file_path)) {
                CreateInitConfigFile(config_file_path);
            }
            else {
                string parity = GlobalConfigManager.Instance.ReadConfig(config_file_path, "SerialInfo", "Parity", "Default");
                string stop_bits = GlobalConfigManager.Instance.ReadConfig(config_file_path, "SerialInfo", "StopBits", "Default");

                GlobalSerialManager.Instance.SetPortName(GlobalConfigManager.Instance.ReadConfig(config_file_path, "SerialInfo", "PortName", "Default"));
                GlobalSerialManager.Instance.SetBaudrate(Int32.Parse(GlobalConfigManager.Instance.ReadConfig(config_file_path, "SerialInfo", "Baudrate", "Default")));
                GlobalSerialManager.Instance.SetDataBits(Int32.Parse(GlobalConfigManager.Instance.ReadConfig(config_file_path, "SerialInfo", "DataBits", "Default")));
                GlobalSerialManager.Instance.SetRtsEnable(Boolean.Parse(GlobalConfigManager.Instance.ReadConfig(config_file_path, "SerialInfo", "RTSEnable", "Default")));
                GlobalSerialManager.Instance.SetDtrEnable(Boolean.Parse(GlobalConfigManager.Instance.ReadConfig(config_file_path, "SerialInfo", "DTREnable", "Default")));

                switch (parity) {
                    case "None":
                        GlobalSerialManager.Instance.SetParity(System.IO.Ports.Parity.None);
                        break;
                    case "Odd":
                        GlobalSerialManager.Instance.SetParity(System.IO.Ports.Parity.Odd);
                        break;
                    case "Even":
                        GlobalSerialManager.Instance.SetParity(System.IO.Ports.Parity.Even);
                        break;
                    case "Mark":
                        GlobalSerialManager.Instance.SetParity(System.IO.Ports.Parity.Mark);
                        break;
                    case "Space":
                        GlobalSerialManager.Instance.SetParity(System.IO.Ports.Parity.Space);
                        break;
                }

                switch (stop_bits) {
                    case "None":
                        GlobalSerialManager.Instance.SetStopBits(System.IO.Ports.StopBits.None);
                        break;
                    case "One":
                        GlobalSerialManager.Instance.SetStopBits(System.IO.Ports.StopBits.One);
                        break;
                    case "Two":
                        GlobalSerialManager.Instance.SetStopBits(System.IO.Ports.StopBits.Two);
                        break;
                    case "OnePointFive":
                        GlobalSerialManager.Instance.SetStopBits(System.IO.Ports.StopBits.OnePointFive);
                        break;
                }
            }
        }
    }
}
