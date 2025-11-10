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

        public enum Filter {
            Raw,
            LPF,
            AVG
        }

        private string _model_name = "H2Sense01";
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
        public Parity GetParity() { return _parity; }
        public void SetParity(Parity parity) { _parity = parity; }
        public StopBits GetStopBits() { return _stop_bits; }
        public void SetStopBits(StopBits stop_bits) { _stop_bits = stop_bits; }
        public bool GetRtsEnable() { return _rts_enable; }
        public void SetRtsEnable(bool rts_enable) { _rts_enable = rts_enable; }
        public bool GetDtrEnable() { return _dtr_enable; }
        public void SetDtrEnable(bool dtr_enable) { _dtr_enable = dtr_enable; }

        private string _serial_received_data_raw;
        private string _serial_received_data_lpf;
        private string _serial_received_data_avg;
        private bool _is_connected = false;
        private Filter _filter = Filter.Raw;


        public string GetSerialReceivedDataRaw() { return _serial_received_data_raw; }
        public void SetSerialReceivedDataRaw(string serial_received_data_raw) { _serial_received_data_raw = serial_received_data_raw; }
        public string GetSerialReceivedDataLPF() { return _serial_received_data_lpf; }
        public void SetSerialReceivedDataLPF(string serial_received_data_lpf) { _serial_received_data_lpf = serial_received_data_lpf; }
        public string GetSerialReceivedDataAVG() { return _serial_received_data_avg; }
        public void SetSerialReceivedDataAVG(string serial_received_data_avg) { _serial_received_data_avg = serial_received_data_avg; }
        public bool GetIsConnected() { return _is_connected; }
        public void SetIsConnected(bool is_connected) { _is_connected = is_connected; }
        public Filter GetFilter() { return _filter; }
        public void SetFilter(Filter filter) { _filter = filter; }
    }
}
