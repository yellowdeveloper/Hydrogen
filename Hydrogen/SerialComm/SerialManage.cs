using Hydrogen.Forms;
using Hydrogen.GlobalManagers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms.VisualStyles;


namespace Hydrogen.SerialComm {
    public class SerialManage {

        private SerialPort sp = new SerialPort();
        Stopwatch stopwatch = new Stopwatch();

        private List<byte> received_buffer = new List<byte>();
        private bool is_processing = false;
        private int num_filters = 0;

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

        public void SerialSendCmd(byte cmd, byte op) {
            // HEADER = 0x09, 0x0D, 0x09, 0x0D
            // FOOTER = 0x27, 0x22, 0x27, 0x22
            byte[] buffer = { 0x09, 0x0D, 0x09, 0x0D, cmd, op, 0x27, 0x22, 0x27, 0x22 };

            sp.Write(buffer, 0, 10);

            GlobalLogManager.Instance.ConsoleLog("OK", $"Sent Command : 0x{cmd:X2} | 0x{op:X2}");
            GlobalLogManager.Instance.AddLogToFile("DEBUG", $"Sent Command : 0x{cmd:X2} | 0x{op:X2}");
        }

        private void SerialReceivedDebug(object s, SerialDataReceivedEventArgs e) {
            if (!sp.IsOpen) return;

            try
            {
                int bytes_to_read = sp.BytesToRead; // Test with tx change to rx later
                byte[] buffer = new byte[bytes_to_read];
                sp.Read(buffer, 0, bytes_to_read);  // Test with tx change to rx later

                GlobalLogManager.Instance.ConsoleLog("OK", $"Received Bytes Length: {buffer.Length}\n");
                GlobalLogManager.Instance.ConsoleLog("OK", $"Received: ");
                for (int i = 0; i < buffer.Length; i++)
                {
                    Console.Write($"{buffer[i]:X2}  ");
                }
                Console.Write(": ");

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
                if (GlobalUIManager.Instance.GetIsTxtLogging() && GlobalLogManager.Instance.GetNowSpent() == 0) stopwatch.Start();
                else if (!GlobalUIManager.Instance.GetIsTxtLogging() && GlobalLogManager.Instance.GetNowSpent() != 0) stopwatch.Reset();

                int bytes_to_read = sp.BytesToRead; // Test with tx change to rx later
                byte[] buffer = new byte[bytes_to_read];
                int actually_read = sp.Read(buffer, 0, bytes_to_read);  // Test with tx change to rx later

                if (actually_read > 0) received_buffer.AddRange(buffer.Take(actually_read));

                ProcessReceivedData();

                GlobalLogManager.Instance.SetNowSpent(stopwatch.Elapsed.TotalMilliseconds);
                GlobalLogManager.Instance.ConsoleLog("COM", $"{GlobalLogManager.Instance.GetAutoStopEnabled()}");
                if (GlobalLogManager.Instance.GetCounter() == 0 && GlobalLogManager.Instance.GetAutoStopEnabled()) {
                    GlobalLogManager.Instance.ConsoleLog("COM", $"CONDITION GRANT");
                    GlobalLogManager.Instance.SetCounter(GlobalLogManager.Instance.GetNowSpent() + GlobalLogManager.Instance.GetAutoStopCount()*1000);
                    GlobalLogManager.Instance.ConsoleLog("COM", $"NOWSPENT: {GlobalLogManager.Instance.GetNowSpent()} COUNT: {GlobalLogManager.Instance.GetCounter()}");
                }
                GlobalLogManager.Instance.ConsoleLog("COM", $"NOWSPENT: {GlobalLogManager.Instance.GetNowSpent()} COUNT: {GlobalLogManager.Instance.GetCounter()}");

            }

            catch (Exception ex) {
                GlobalLogManager.Instance.ConsoleLog("ERROR", $"Error occured while receiving {ex}");
                GlobalLogManager.Instance.AddLogToFile("ERROR", $"Error occured while receiving {ex}");
            }
        }

        private void ProcessReceivedData() {
            while(received_buffer.Count >= 12) {
                //Check if Header is OK
                if (!ValidityCheck()) continue;

                string dc = get_digital_count(received_buffer[0], received_buffer[1], received_buffer[2]).ToString();

                GlobalSerialManager.Instance.SetSerialReceivedDataRaw(dc);
                received_buffer.RemoveRange(0, 3);
                GlobalLogManager.Instance.ConsoleLog("OK", $"Received Data (RAW) :: {GlobalSerialManager.Instance.GetSerialReceivedDataRaw()}");

                FilterCheck();

                CalMinMaxDiff(Int32.Parse(dc));

                if (GlobalSerialManager.Instance.GetIsSafEnabled() && Int32.Parse(GlobalSerialManager.Instance.GetSerialReceivedDataSAF()) == 0) return;

                if (GlobalUIManager.Instance.GetIsTxtLogging()) GlobalLogManager.Instance.DataValueLog();

                is_processing = false;
            }
        }
        private void FilterCheck() {
            if (num_filters == 0) return;
            if (received_buffer[0] == 0) {
                if (!GlobalSerialManager.Instance.GetIsSafEnabled()) GlobalSerialManager.Instance.SetIsSafEnabled(true);
                received_buffer.RemoveRange(0, 1);

                string buff = ConvertByteArray(received_buffer.GetRange(0, 4).ToArray());
                if (Int32.Parse(buff) != 0) GlobalSerialManager.Instance.SetSerialReceivedDataSAF(buff);
                GlobalLogManager.Instance.ConsoleLog("OK", $"Received Data (SAF) :: {GlobalSerialManager.Instance.GetSerialReceivedDataSAF()}");
                received_buffer.RemoveRange(0, 4);
            }
            else {
                if (GlobalSerialManager.Instance.GetIsSafEnabled()) GlobalSerialManager.Instance.SetIsSafEnabled(false);
            }

            if (num_filters == 1) return;
            if (received_buffer[0] == 1)
            {
                if (!GlobalSerialManager.Instance.GetIsLpfEnabled()) GlobalSerialManager.Instance.SetIsLpfEnabled(true);
                received_buffer.RemoveRange(0, 1);

                GlobalSerialManager.Instance.SetSerialReceivedDataLPF(ConvertByteArray(received_buffer.GetRange(0, 4).ToArray()));
                GlobalLogManager.Instance.ConsoleLog("OK", $"Received Data (LPF) :: {GlobalSerialManager.Instance.GetSerialReceivedDataLPF()}");
                received_buffer.RemoveRange(0, 4);
            }
            else {
                if (GlobalSerialManager.Instance.GetIsLpfEnabled()) GlobalSerialManager.Instance.SetIsLpfEnabled(false);
            }

            if (num_filters == 2) return;
            if (received_buffer[0] == 2) {
                if (!GlobalSerialManager.Instance.GetIsMafEnabled()) GlobalSerialManager.Instance.SetIsMafEnabled(true);
                received_buffer.RemoveRange(0, 1);

                GlobalSerialManager.Instance.SetSerialReceivedDataMAF(ConvertByteArray(received_buffer.GetRange(0, 4).ToArray()));
                GlobalLogManager.Instance.ConsoleLog("OK", $"Received Data (MAF) :: {GlobalSerialManager.Instance.GetSerialReceivedDataMAF()}");
                received_buffer.RemoveRange(0, 4);
            }
            else {
                if (GlobalSerialManager.Instance.GetIsMafEnabled()) GlobalSerialManager.Instance.SetIsMafEnabled(false);
            }
        }

        private bool ValidityCheck() {
            bool header_ok = false;
            bool footer_ok = false;
            int data_length = 0;
            byte crc = 0;

            if (received_buffer[0] == 0x09 && received_buffer[1] == 0x0D && received_buffer[2] == 0x09 && received_buffer[3] == 0x0D) {
                received_buffer.RemoveRange(0, 4);
                GlobalLogManager.Instance.ConsoleLog("OK", "Received Right Header :: 0x09 0x0D 0x09 0x0D Remove From Buffer");
                GlobalLogManager.Instance.AddLogToFile("DEBUG", "Received Right Header :: 0x09 0x0D 0x09 0x0D Remove From Buffer");

                is_processing = true;

                header_ok =  true;
            }

            if (header_ok) {
                data_length = received_buffer[0];
                received_buffer.RemoveRange(0, 1);

                if (received_buffer[data_length + 1] == 0x27 && received_buffer[data_length + 2] == 0x22 && received_buffer[data_length + 3] == 0x27 && received_buffer[data_length + 4] == 0x22) {
                    received_buffer.RemoveRange(data_length + 1, 4);
                    GlobalLogManager.Instance.ConsoleLog("OK", "Received Right Footer :: 0x22 0x27 0x22 0x27 Remove From Buffer");
                    GlobalLogManager.Instance.AddLogToFile("DEBUG", "Received Right Footer :: 0x22 0x27 0x22 0x27 Remove From Buffer");

                    footer_ok = true;
                }
            }

            if (footer_ok) {
                crc = CalCRC(received_buffer.GetRange(0, data_length).ToArray());
                if (crc == received_buffer[data_length])
                {
                    GlobalLogManager.Instance.ConsoleLog("OK", $"CRC OK :: {crc:X2}");
                    received_buffer.RemoveRange(data_length, 1);

                    num_filters = data_length / 5;

                    return true;
                }
                GlobalLogManager.Instance.ConsoleLog("OK", $"CRC BAD :: {crc:X2}, {received_buffer[data_length]:X2}");
            }

            GlobalLogManager.Instance.ConsoleLog("ERROR", $"Invalid Data :: Contents in Buffer ::");
            GlobalLogManager.Instance.AddLogToFile("ERROR", $"Wrong Header :: Erase First Byte in Buffer And Process");

            for (int i = 0; i < received_buffer.Count; i++) {
                Console.Write($"{received_buffer[i]:X2}  ");
            }

            Console.Write("Erase Buffer And Process\n");
            received_buffer.Clear();

            is_processing = false;

            return false;
        }

        private string ConvertByteArray(byte[] val) {
            string result =  (-BitConverter.ToInt32(val, 0)).ToString();
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

        private byte CalCRC(byte[] byte_array)
        {
            byte crc = 0x00;
            byte poly = 0x07;

            for (int i = 0; i < byte_array.Length; i++)
            {
                crc ^= byte_array[i];

                for (int bit = 0; bit < 8; bit++)
                {
                    if ((crc & 0x80) == 0x80) crc = (byte)(crc << 1 ^ poly);
                    else crc = (byte)(crc << 1);
                }
            }

            return crc;
        }
    }
}
