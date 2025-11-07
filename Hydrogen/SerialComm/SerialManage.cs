using Hydrogen.Forms;
using Hydrogen.GlobalManagers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;


namespace Hydrogen.SerialComm {
    public class SerialManage {

        private SerialPort sp = new SerialPort();
        private int[] data_array_tmp = new int[2];

        private List<byte> received_buffer = new List<byte>();

        public int SerialConnect() {
            sp.PortName = GlobalSerialManager.Instance.GetPortName();
            sp.BaudRate = GlobalSerialManager.Instance.GetBaudrate();
            sp.DataBits = GlobalSerialManager.Instance.GetDataBits();
            sp.Parity = Parity.None;
            sp.StopBits = StopBits.One;

            sp.RtsEnable = false;
            sp.DtrEnable = false;

            sp.DataReceived += new SerialDataReceivedEventHandler(SerialReceived);
            //sp.DataReceived += SerialReceivedDebug;

            if (!sp.IsOpen) {
                try {
                    sp.Open();
                    GlobalUIManager.Instance.SetDebugStat($"Opening Port : {GlobalSerialManager.Instance.GetPortName()}");
                    GlobalLogManager.Instance.AddLogToFile("DEBUG", $"Opening Port : {GlobalSerialManager.Instance.GetPortName()}");
                }
                catch (Exception ex) {
                    GlobalUIManager.Instance.SetDebugStat($"Port : {GlobalSerialManager.Instance.GetPortName()} Serial Connnection Error - {ex.Message}");
                    GlobalLogManager.Instance.AddLogToFile("ERROR", $"Port : {GlobalSerialManager.Instance.GetPortName()} Serial Connnection Error - {ex.Message}");
                    using (ErrorForm error_form = new ErrorForm()) {
                        error_form.ShowDialog();
                    }
                    return 1;
                }
            }

            if (sp.IsOpen) {
                GlobalUIManager.Instance.SetDebugStat($"Port : {GlobalSerialManager.Instance.GetPortName()} Serial Connnected");
                return 0;
            }
            else {
                GlobalUIManager.Instance.SetDebugStat($"Port : {GlobalSerialManager.Instance.GetPortName()} Serial Connnection Error While Opening Port");
                return 1;
            }
        }

        public void SerialDisconnect() {
            if (received_buffer.Count != 0) received_buffer.Clear();
            if (sp.IsOpen) {
                try {
                    sp.DataReceived -= new SerialDataReceivedEventHandler(SerialReceived);
                    //sp.DataReceived -= SerialReceivedDebug;
                    sp.Close();
                    GlobalUIManager.Instance.SetDebugStat($"Port : {GlobalSerialManager.Instance.GetPortName()} Serial Disconnnected");
                    // Console.WriteLine($"Serial Disconnected");
                }
                catch (Exception ex) {
                    Console.WriteLine($"Serial Disconnect Error : {ex.Message}");
                }
            }
        }

        public void InterfaceCheck() {
            string[] accessable_ports = SerialPort.GetPortNames();
            if (accessable_ports.Length > 0) {
                GlobalSerialManager.Instance.SetPortName(accessable_ports[0]);
            }
            else {
                GlobalSerialManager.Instance.SetPortName("None");
            }
        }

        public void SerialSendCmd(byte cmd) {
            // HEADER = 0x09, 0x0D, 0x09, 0x0D
            // FOOTER = 0x27, 0x22, 0x27, 0x22
            byte[] buffer = { 0x09, 0x0D, 0x09, 0x0D, cmd, 0x27, 0x22, 0x27, 0x22 };

            sp.Write(buffer, 0, 9);

            GlobalLogManager.Instance.ConsoleLog("OK", $"Sent Command : 0x{cmd:X2}");
            GlobalLogManager.Instance.AddLogToFile("DEBUG", $"Sent Command : 0x{cmd:X2}");
        }

        private void SerialReceivedDebug(object s, SerialDataReceivedEventArgs e) {
            if (!sp.IsOpen) return;

            try
            {
                int bytes_to_read = sp.BytesToRead; // Test with tx change to rx later
                byte[] buffer = new byte[bytes_to_read];
                sp.Read(buffer, 0, bytes_to_read);  // Test with tx change to rx later

                //GlobalLogManager.Instance.ConsoleLog("OK", $"Received Bytes Length: {buffer.Length}\n");
                //GlobalLogManager.Instance.ConsoleLog("OK", $"Received: ");
                //for (int i = 0; i < buffer.Length; i++)
                //{
                //    Console.Write($"{buffer[i]:X2}  ");
                //}
                //Console.Write(": ");

                Console.Write($"{Encoding.UTF8.GetString(buffer)}\n");
            }
            catch (Exception ex)
            {
                GlobalLogManager.Instance.ConsoleLog("ERROR", $"Error occured while receiving {ex}");
            }
        }

        private void SerialReceived(object s, SerialDataReceivedEventArgs e) {
            if (!sp.IsOpen) return;

            try {
                int bytes_to_read = sp.BytesToRead; // Test with tx change to rx later
                byte[] buffer = new byte[bytes_to_read];
                int actually_read = sp.Read(buffer, 0, bytes_to_read);  // Test with tx change to rx later

                if (actually_read > 0) received_buffer.AddRange(buffer.Take(actually_read));

                if (GlobalSerialManager.Instance.GetFilter() == GlobalSerialManager.Filter.Raw) ProcessReceivedData_Raw();
                else if (GlobalSerialManager.Instance.GetFilter() == GlobalSerialManager.Filter.LPF) ProcessReceivedData_LPF();
                else if (GlobalSerialManager.Instance.GetFilter() == GlobalSerialManager.Filter.AVG) ProcessReceivedData_AVG();


            }

            catch (Exception ex) {
                GlobalLogManager.Instance.ConsoleLog("ERROR", $"Error occured while receiving {ex}");
                GlobalLogManager.Instance.AddLogToFile("ERROR", $"Error occured while receiving {ex}");
            }
        }

        private void ProcessReceivedData_Raw() {
            while(received_buffer.Count >= 11) {
                //Check if Header is OK
                if (!HeaderCheck()) continue;

                string dc = get_digital_count(received_buffer[0], received_buffer[1], received_buffer[2]).ToString();

                GlobalSerialManager.Instance.SetSerialReceivedDataRaw(dc);
                received_buffer.RemoveRange(0, 3);
                GlobalLogManager.Instance.ConsoleLog("OK", $"Received Data (RAW) :: {GlobalSerialManager.Instance.GetSerialReceivedDataRaw()}");

                CalMinMaxDiff(Int32.Parse(dc));

                if (!FooterCheck()) continue;

                if (GlobalUIManager.Instance.GetIsTxtLogging()) GlobalLogManager.Instance.DataValueLog();
            }
        }
        private void ProcessReceivedData_LPF() {
            while (received_buffer.Count >= 19) {
                //Check if Header is OK
                if (!HeaderCheck()) continue;

                string dc = get_digital_count(received_buffer[0], received_buffer[1], received_buffer[2]).ToString();

                GlobalSerialManager.Instance.SetSerialReceivedDataRaw(dc);
                received_buffer.RemoveRange(0, 3);
                GlobalLogManager.Instance.ConsoleLog("OK", $"Received Data (RAW) :: {GlobalSerialManager.Instance.GetSerialReceivedDataRaw()}");

                CalMinMaxDiff(Int32.Parse(dc));

                GlobalSerialManager.Instance.SetSerialReceivedDataLPF(ConvertByteArray(received_buffer.GetRange(0, 8).ToArray()));
                received_buffer.RemoveRange(0, 8);
                GlobalLogManager.Instance.ConsoleLog("OK", $"Received Data (LPF) :: {GlobalSerialManager.Instance.GetSerialReceivedDataLPF()}");

                if (!FooterCheck()) continue;

                if (GlobalUIManager.Instance.GetIsTxtLogging()) GlobalLogManager.Instance.DataValueLog();
            }
        }

        private void ProcessReceivedData_AVG() {
            while (received_buffer.Count >= 27) {
                //Check if Header is OK
                if (!HeaderCheck()) continue;

                string dc = get_digital_count(received_buffer[0], received_buffer[1], received_buffer[2]).ToString();

                GlobalSerialManager.Instance.SetSerialReceivedDataRaw(dc);
                received_buffer.RemoveRange(0, 3);
                GlobalLogManager.Instance.ConsoleLog("OK", $"Received Data (RAW) :: {GlobalSerialManager.Instance.GetSerialReceivedDataRaw()}");

                CalMinMaxDiff(Int32.Parse(dc));

                GlobalSerialManager.Instance.SetSerialReceivedDataLPF(ConvertByteArray(received_buffer.GetRange(0,8).ToArray()));
                received_buffer.RemoveRange(0, 8);
                GlobalLogManager.Instance.ConsoleLog("OK", $"Received Data (LPF) :: {GlobalSerialManager.Instance.GetSerialReceivedDataLPF()}");

                GlobalSerialManager.Instance.SetSerialReceivedDataAVG(ConvertByteArray(received_buffer.GetRange(0, 8).ToArray()));
                received_buffer.RemoveRange(0, 8);
                GlobalLogManager.Instance.ConsoleLog("OK", $"Received Data (AVG) :: {GlobalSerialManager.Instance.GetSerialReceivedDataAVG()}");

                if (!FooterCheck()) continue;

                if (GlobalUIManager.Instance.GetIsTxtLogging()) GlobalLogManager.Instance.DataValueLog();
            }
        }

        private bool HeaderCheck() {
            if (received_buffer[0] == 0x09 && received_buffer[1] == 0x0D && received_buffer[2] == 0x09 && received_buffer[3] == 0x0D) {
                received_buffer.RemoveRange(0, 4);
                GlobalLogManager.Instance.ConsoleLog("OK", "Received Right Header :: 0x09 0x0D 0x09 0x0D Remove From Buffer");
                GlobalLogManager.Instance.AddLogToFile("DEBUG", "Received Right Header :: 0x09 0x0D 0x09 0x0D Remove From Buffer");
                return true;
            }
            else {
                GlobalLogManager.Instance.ConsoleLog("ERROR", $"Wrong Header :: Contents in Buffer ::");
                GlobalLogManager.Instance.AddLogToFile("ERROR", $"Wrong Header :: Erase First Byte in Buffer And Process");

                for (int i = 0; i < received_buffer.Count; i++) {
                    Console.Write($"{received_buffer[i]:X2}  ");
                }

                Console.Write("Erase First Byte in Buffer And Process\n");
                received_buffer.RemoveAt(0);
                return false;
            }
        }

        private bool FooterCheck() {
            if (received_buffer[0] == 0x27 && received_buffer[1] == 0x22 && received_buffer[2] == 0x27 && received_buffer[3] == 0x22) {
                GlobalLogManager.Instance.ConsoleLog("OK", $"Received Right Footer :: 0x27 0x22 0x27 0x22 Remove From Buffer\n");
                received_buffer.RemoveRange(0, 4);
                //GlobalLogManager.Instance.ConsoleLog($"Received Right Footer :: {received_buffer[0]:X2} 0x0A 0x0D 0x0A Remove From Buffer");
                GlobalLogManager.Instance.AddLogToFile("DEBUG", "Received Right Footer :: 0x27 0x22 0x27 0x22 Remove From Buffer");
                return true;
            }
            else {
                GlobalLogManager.Instance.ConsoleLog("ERROR", $"Wrong Footer :: Contents in Buffer :: ");
                GlobalLogManager.Instance.AddLogToFile("ERROR", $"Wrong Footer :: Erase First Byte in Buffer And Process");

                for (int i = 0; i < received_buffer.Count; i++) {
                    Console.Write($"{received_buffer[i]:X2}  ");
                }

                Console.Write(" Erase First Byte in Buffer And Process\n");
                received_buffer.RemoveAt(0);
                return false;
            }
        }

        private string ConvertByteArray(byte[] val) {
            string result =  (-BitConverter.ToDouble(val, 0)).ToString("F3");
            return result;
        }

        private void CalMinMaxDiff(int val) {
            int max = GlobalUIManager.Instance.GetMaxRaw();
            int min = GlobalUIManager.Instance.GetMinRaw();
            int diff = 0;

            if (max == 0) GlobalUIManager.Instance.SetMaxRaw(val);
            if (min == 0) GlobalUIManager.Instance.SetMinRaw(val);

            if (max < val) GlobalUIManager.Instance.SetMaxRaw(val);
            if (min > val) GlobalUIManager.Instance.SetMinRaw(val);

            diff = max - min;

            GlobalUIManager.Instance.SetDiffRaw(diff);
        }

        private int get_digital_count(byte b1, byte b2, byte b3) {
            int dc;
            dc = (b1 << 16) |
                 (b2 << 8 ) |
                  b3;
            dc = dc << 8;
            dc = dc >> 8;
            return -dc;
        }
    }
}
