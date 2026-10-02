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

        private void ProcessReceivedData()
        {
            // 패킷의 최소 크기는 헤더(4) + 길이(1) + RAW데이터(3) + CRC(1) + 푸터(4) = 13바이트
            while (received_buffer.Count >= 13)
            {
                // 1. 헤더(0x09 0x0D 0x09 0x0D) 시작 위치 찾기
                int headerIdx = -1;
                for (int i = 0; i <= received_buffer.Count - 4; i++)
                {
                    if (received_buffer[i] == 0x09 && received_buffer[i + 1] == 0x0D && received_buffer[i + 2] == 0x09 && received_buffer[i + 3] == 0x0D)
                    {
                        headerIdx = i;
                        break;
                    }
                }

                // 헤더를 못 찾으면 가비지 데이터이므로 삭제 후 대기
                if (headerIdx == -1)
                {
                    received_buffer.Clear();
                    break;
                }

                // 헤더 앞의 가비지 데이터 제거
                if (headerIdx > 0)
                {
                    received_buffer.RemoveRange(0, headerIdx);
                }

                // 길이를 읽을 수 있을 만큼 버퍼가 찼는지 확인
                if (received_buffer.Count < 5) break;

                int data_length = received_buffer[4];
                int required_length = 4 + 1 + data_length + 1 + 4; // 헤더(4) + 길이(1) + 데이터길이 + CRC(1) + 푸터(4)

                // 핵심: 패킷 전체가 시리얼 버퍼에 안 들어왔다면 루프를 탈출하고 다음 이벤트를 기다림
                if (received_buffer.Count < required_length)
                {
                    break;
                }

                // 전체 패킷이 들어왔으므로 검증 시작
                if (!ValidityCheck(data_length, required_length))
                {
                    // 유효하지 않은 패킷(잘못된 헤더)일 경우 1바이트만 지워서 다음 헤더를 찾도록 함
                    received_buffer.RemoveAt(0);
                    continue;
                }

                // --- 여기부터는 유효한 데이터 파싱 (기존 로직 동일) ---
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
            if (num_filters <= 0) return;

            if (received_buffer[0] == 0) {
                if (!GlobalSerialManager.Instance.GetIsSafEnabled()) GlobalSerialManager.Instance.SetIsSafEnabled(true);
                received_buffer.RemoveRange(0, 1);

                string buff = ConvertByteArray(received_buffer.GetRange(0, 4).ToArray());
                if (Int32.Parse(buff) != 0) GlobalSerialManager.Instance.SetSerialReceivedDataSAF(buff);
                GlobalLogManager.Instance.ConsoleLog("OK", $"Received Data (SAF) :: {GlobalSerialManager.Instance.GetSerialReceivedDataSAF()}");
                received_buffer.RemoveRange(0, 4);

                num_filters--;
            }
            else {
                if (GlobalSerialManager.Instance.GetIsSafEnabled()) GlobalSerialManager.Instance.SetIsSafEnabled(false);
            }

            if (num_filters <= 0) return;
            if (received_buffer[0] == 1)
            {
                if (!GlobalSerialManager.Instance.GetIsLpfEnabled()) GlobalSerialManager.Instance.SetIsLpfEnabled(true);
                received_buffer.RemoveRange(0, 1);

                GlobalSerialManager.Instance.SetSerialReceivedDataLPF(ConvertByteArray(received_buffer.GetRange(0, 4).ToArray()));
                GlobalLogManager.Instance.ConsoleLog("OK", $"Received Data (LPF) :: {GlobalSerialManager.Instance.GetSerialReceivedDataLPF()}");
                received_buffer.RemoveRange(0, 4);

                num_filters--;
            }
            else {
                if (GlobalSerialManager.Instance.GetIsLpfEnabled()) GlobalSerialManager.Instance.SetIsLpfEnabled(false);
            }

            if (num_filters <= 0) return;
            if (received_buffer[0] == 2) {
                if (!GlobalSerialManager.Instance.GetIsMafEnabled()) GlobalSerialManager.Instance.SetIsMafEnabled(true);
                received_buffer.RemoveRange(0, 1);

                GlobalSerialManager.Instance.SetSerialReceivedDataMAF(ConvertByteArray(received_buffer.GetRange(0, 4).ToArray()));
                GlobalLogManager.Instance.ConsoleLog("OK", $"Received Data (MAF) :: {GlobalSerialManager.Instance.GetSerialReceivedDataMAF()}");
                received_buffer.RemoveRange(0, 4);

                num_filters--;
            }
            else {
                if (GlobalSerialManager.Instance.GetIsMafEnabled()) GlobalSerialManager.Instance.SetIsMafEnabled(false);
            }
        }

        private bool ValidityCheck(int data_length, int required_length)
        {
            // 1. 푸터 확인 (끝에서 4바이트)
            if (received_buffer[required_length - 4] != 0x27 ||
                received_buffer[required_length - 3] != 0x22 ||
                received_buffer[required_length - 2] != 0x27 ||
                received_buffer[required_length - 1] != 0x22)
            {

                GlobalLogManager.Instance.ConsoleLog("ERROR", "Invalid Footer!");
                GlobalLogManager.Instance.AddLogToFile("ERROR", "Invalid Footer!");
                return false;
            }

            // 2. CRC 확인 (끝에서 5번째 바이트)
            byte received_crc = received_buffer[required_length - 5];
            byte[] data_payload = received_buffer.GetRange(5, data_length).ToArray();
            byte calculated_crc = CalCRC(data_payload);

            if (received_crc != calculated_crc)
            {
                GlobalLogManager.Instance.ConsoleLog("ERROR", $"CRC BAD :: Calc: {calculated_crc:X2}, Recv: {received_crc:X2}");
                return false;
            }

            // 검증 성공! 파싱을 위해 버퍼의 앞뒤 껍데기 제거하고 순수 데이터만 남김
            received_buffer.RemoveRange(required_length - 5, 5); // 뒤에서부터 CRC(1) + 푸터(4) 제거
            received_buffer.RemoveRange(0, 5);                   // 앞에서부터 헤더(4) + 길이(1) 제거

            num_filters = data_length / 5; // RAW 데이터(3바이트)를 제외하고 필터는 5바이트씩 차지하므로 /5 연산 유지

            return true;
        }

        private string ConvertByteArray(byte[] val) {
            string result =  (BitConverter.ToInt32(val, 0)).ToString();
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
            return dc;
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
