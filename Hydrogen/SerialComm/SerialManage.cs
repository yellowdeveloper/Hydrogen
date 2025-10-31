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

            //sp.DataReceived += new SerialDataReceivedEventHandler(SerialReceived);
            sp.DataReceived += SerialReceivedDebug;

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
            if (sp.IsOpen) {
                try {
                    //sp.DataReceived -= new SerialDataReceivedEventHandler(SerialReceived);
                    sp.DataReceived -= SerialReceivedDebug;
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

                Console.Write($"{Encoding.UTF8.GetString(buffer)}");
            }
            catch (Exception ex)
            {
                GlobalLogManager.Instance.ConsoleLog("ERROR", $"Error occured while receiving {ex}");
            }
        }

        private void SerialReceived(object s, SerialDataReceivedEventArgs e) {
            //GlobalUIManager.Instance.SetDebugStat($"{GlobalSerialManager.Instance.GetPortName()} {sp.BytesToRead} bytes received");
            
            while (sp.BytesToRead > 0) {
                int tmp = sp.ReadByte();

                // Board & Sensor Error
                if (tmp == 0x01) {
                    GlobalUIManager.Instance.SetDebugStat($"Sensor is not responding");
                    Console.WriteLine($"Sensor is not responding");
                }
                else if (tmp == 0x00) {
                    GlobalUIManager.Instance.SetDebugStat($"Board -> Sensor Transfer Error");
                    Console.WriteLine($"Board -> Sensor Transfer Error");
                }
                else {
                    GlobalUIManager.Instance.SetDebugStat($"Undefined Error in Hydrogen Board");
                }

                // Receiving Sensor Data
                if (tmp == 0x18) {
                    GlobalUIManager.Instance.SetDebugStat($"Header Status OK : {tmp}");
                    GlobalSerialManager.Instance.SetIsMeasurementTriggered(true);
                }

                if (GlobalSerialManager.Instance.GetIsMeasurementTriggered()) {
                    try {
                        GlobalSerialManager.Instance.AddToReceiveBuffer(tmp);
                        GlobalSerialManager.Instance.SetBufferIndex(GlobalSerialManager.Instance.GetBufferIndex() + 1);
                    }
                    catch {
                        Console.WriteLine($"RECEIVE BUFFER ERROR");
                    }

                    if (GlobalSerialManager.Instance.GetBufferIndex() == 5) {
                        data_array_tmp = ProcessReceivedData(GlobalSerialManager.Instance.GetReceiveBuffer());
                        GlobalSerialManager.Instance.SetSerialReceivedData(data_array_tmp[0].ToString());
                        GlobalSerialManager.Instance.SetBufferIndex(0);
                        GlobalSerialManager.Instance.SetIsMeasurementTriggered(false);
                    }
                }

                //string str_receiveData = receiveData.ToString();
                GlobalUIManager.Instance.SetDebugStat($"received data = {data_array_tmp[0]}");
            }
            if (sp.BytesToRead <= 0) {
                //GlobalUIManager.Instance.SetDebugStat($"{GlobalSerialManager.Instance.GetPortName()} {sp.BytesToRead} bytes received");
                //Console.WriteLine($"no Bytes to Read");
            }
        }

        private int[] ProcessReceivedData(int[] received_buffer) {
            string data = String.Empty;
            int [] data_array = new int [2];

            for (int i = 1; i < received_buffer.Length; i++) {
                data += Convert.ToString(received_buffer[i], 2).PadLeft(8, '0');
            }

            data_array[0] = Convert.ToInt32(data.Substring(0, 20), 2);
            data_array[1] = Convert.ToInt32(data.Substring(20), 2);

            //Console.WriteLine($"Now Status : 0x{received_buffer[0]:X2}");
            //Console.WriteLine($"2bit exp integrated (Humid+Temp) : {data}");
            Console.WriteLine($"Humid : {data.Substring(0, 20)} :: {data_array[0]}");
            //Console.WriteLine($"Temp : {data.Substring(20)} :: {data_array[1]}");

            return data_array;
        }
    }
}
