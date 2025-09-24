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
    internal class SerialManage {

        private SerialPort sp = new SerialPort();
        private int[] data_array_tmp = new int[2];

        public void SerialConnect() {
            sp.PortName = GlobalSerialManager.Instance.GetPortName();
            sp.BaudRate = GlobalSerialManager.Instance.GetBaudrate();
            sp.DataBits = GlobalSerialManager.Instance.GetDataBits();
            sp.Parity = Parity.None;
            sp.StopBits = StopBits.One;

            sp.RtsEnable = true;
            sp.DtrEnable = true;

            sp.DataReceived += new SerialDataReceivedEventHandler(SerialReceived);

            if (!sp.IsOpen) {
                sp.Open();
                GlobalUIManager.Instance.SetDebugStat($"Opening Port : {GlobalSerialManager.Instance.GetPortName()}");
            }

            if (sp.IsOpen) {
                GlobalUIManager.Instance.SetDebugStat($"Port : {GlobalSerialManager.Instance.GetPortName()} Serial Connnected");
            }
            else {

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

        private void SerialReceived(object s, SerialDataReceivedEventArgs e) {
            //GlobalUIManager.Instance.SetDebugStat($"{GlobalSerialManager.Instance.GetPortName()} {sp.BytesToRead} bytes received");
            
            while (sp.BytesToRead > 0) {
                int tmp = sp.ReadByte();

                // Board & Sensor Error
                if (tmp == 0x01) {
                    GlobalUIManager.Instance.SetDebugStat($"Sensor is not responding");
                    Debug.WriteLine($"Sensor is not responding");
                }
                else if (tmp == 0x00) {
                    GlobalUIManager.Instance.SetDebugStat($"Board -> Sensor Transfer Error");
                    Debug.WriteLine($"Board -> Sensor Transfer Error");
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
                        Debug.WriteLine($"RECEIVE BUFFER ERROR");
                    }

                    if (GlobalSerialManager.Instance.GetBufferIndex() == 5) {
                        data_array_tmp = ProcessReceivedData(GlobalSerialManager.Instance.GetReceiveBuffer());
                        GlobalSerialManager.Instance.SetBufferIndex(0);
                        GlobalSerialManager.Instance.SetIsMeasurementTriggered(false);
                    }
                }

                GlobalSerialManager.Instance.SetSerialReceivedData(data_array_tmp[0].ToString());
                //string str_receiveData = receiveData.ToString();
                GlobalUIManager.Instance.SetDebugStat($"received data = {data_array_tmp[0]}");
            }
            if (sp.BytesToRead <= 0) {
                //GlobalUIManager.Instance.SetDebugStat($"{GlobalSerialManager.Instance.GetPortName()} {sp.BytesToRead} bytes received");
                Debug.WriteLine($"no Bytes to Read");
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

            Debug.WriteLine($"Now Status : 0x{received_buffer[0]:X2}");
            Debug.WriteLine($"2bit exp integrated (Humid+Temp) : {data}");
            Debug.WriteLine($"Humid : {data.Substring(0, 20)} :: {data_array[0]}");
            Debug.WriteLine($"Temp : {data.Substring(20)} :: {data_array[1]}");

            return data_array;
        }
    }
}
