using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen.GlobalManagers
{
    internal class GlobalSerialManager {
        private static readonly GlobalSerialManager _instance = new GlobalSerialManager();
        public static GlobalSerialManager Instance => _instance;

        private string _model_name;
        private string _port_name;
        private int _baudrate;
        private int _databits;
        private Parity _parity;
        private StopBits _stop_bits;
        private bool _rts_enable;
        private bool _dtr_enable;

        public string GetModelName() { return _model_name; }
        public void SetModelName(string model_name) { _model_name = model_name; }
        public string GetPortName() { return _port_name; }
        public void SetPortName(string port_name) { _port_name = port_name; }
        public int GetBaudrate() { return _baudrate; }
        public void SetBaudrate(int baudrate) { _baudrate = baudrate; }
        public int GetDataBits() { return _databits; }
        public void SetDataBits(int databits) { _databits = databits; }

        private string _serial_received_data;
        private int[] receive_buffer = new int[6];
        private int _buffer_index = 0;
        private bool _is_measurement_triggered = false;

        public string GetSerialReceivedData() { return _serial_received_data; }
        public void SetSerialReceivedData(string serial_received_data) { _serial_received_data = serial_received_data; }
        public bool GetIsMeasurementTriggered() { return _is_measurement_triggered; }
        public void SetIsMeasurementTriggered(bool is_measurement_triggered) { _is_measurement_triggered = is_measurement_triggered; }
        public int GetBufferIndex() { return _buffer_index; }
        public void SetBufferIndex(int buffer_index) { _buffer_index = buffer_index; }
        public void AddToReceiveBuffer(int received_data) { receive_buffer[_buffer_index] = received_data; }
        public int[] GetReceiveBuffer () { return receive_buffer; }
    }
}
