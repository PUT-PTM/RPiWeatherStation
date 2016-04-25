using System;
using Windows.Devices.I2c;
using Windows.Devices.Enumeration;
using System.Threading.Tasks;

namespace WeatherStation
{
    public static class PressureSensor
    {
        public static bool IsInitialized { get; private set; } = false;
        public static double Temperature { get; private set; } = 0;
        public static double Pressure { get; private set; } = 0;
        private static I2cDevice _sensor;
        private static CalibrationData _calibrationData;
        private static byte[] _uncompestatedTemperature;
        private static byte[] _uncompestatedPressure;

        //Adres czujnika
        private const byte MP180_ADDR = 0x77;
        //Adres odczytu
        private const byte BMP180_REG_CHIPID = 0xD0;
        //Adres kontrolny
        private const byte BMP180_REG_CONTROL = 0xF4;
        //Adres software'owego resetu
        private const byte BMP180_COM_SOFTRESET = 0xE0;
        //Adres wynikowy
        private const byte BMP180_REG_RESULT = 0xF6;

        //Rejestr temperatury 
        private const byte BMP180_COM_TEMPERATURE = 0x2E;
        //Rejestr ciśnienia w trybie Ultra Low Power
        private const byte BMP180_COM_PRESSURE0 = 0x34;
        //Rejestr ciśnienia w trybie Standard
        private const byte BMP180_COM_PRESSURE1 = 0x74;
        //Rejestr ciśnienia w trybie High Resolution
        private const byte BMP180_COM_PRESSURE2 = 0xB4;
        //Rejestr ciśnienia w trybie Ultra High Resolution
        private const byte BMP180_COM_PRESSURE3 = 0xF4;

        //Adresy rejestrow kalibracyjnych
        private const byte BMP180_CAL_AC1 = 0xAA;
        private const byte BMP180_CAL_AC2 = 0xAC;
        private const byte BMP180_CAL_AC3 = 0xAE;
        private const byte BMP180_CAL_AC4 = 0xB0;
        private const byte BMP180_CAL_AC5 = 0xB2;
        private const byte BMP180_CAL_AC6 = 0xB4;
        private const byte BMP180_CAL_B1 = 0xB6;
        private const byte BMP180_CAL_B2 = 0xB8;
        private const byte BMP180_CAL_MB = 0xBA;
        private const byte BMP180_CAL_MC = 0xBC;
        private const byte BMP180_CAL_MD = 0xBE;
  
        internal static async Task InitializeAsync()
        {
            var selectorString = I2cDevice.GetDeviceSelector();
            var devices = await DeviceInformation.FindAllAsync(selectorString);
            var deviceId = devices[0].Id;
            var settings = new I2cConnectionSettings(MP180_ADDR);
            settings.BusSpeed = I2cBusSpeed.StandardMode;
            _sensor = await I2cDevice.FromIdAsync(deviceId, settings);
            _calibrationData = new CalibrationData();
            ReadCalibrationData();
            IsInitialized = true;
        }
   
        private static void ReadCalibrationData()
        {
            var data = WriteRead(BMP180_CAL_AC1, 2);
            Array.Reverse(data);
            _calibrationData.AC1 = BitConverter.ToInt16(data, 0);

            data = WriteRead(BMP180_CAL_AC2, 2);
            Array.Reverse(data);
            _calibrationData.AC2 = BitConverter.ToInt16(data, 0);

            data = WriteRead(BMP180_CAL_AC3, 2);
            Array.Reverse(data);
            _calibrationData.AC3 = BitConverter.ToInt16(data, 0);

            data = WriteRead(BMP180_CAL_AC4, 2);
            Array.Reverse(data);
            _calibrationData.AC4 = BitConverter.ToUInt16(data, 0);

            data = WriteRead(BMP180_CAL_AC5, 2);
            Array.Reverse(data);
            _calibrationData.AC5 = BitConverter.ToUInt16(data, 0);

            data = WriteRead(BMP180_CAL_AC6, 2);
            Array.Reverse(data);
            _calibrationData.AC6 = BitConverter.ToUInt16(data, 0);

            data = WriteRead(BMP180_CAL_B1, 2);
            Array.Reverse(data);
            _calibrationData.B1 = BitConverter.ToInt16(data, 0);

            data = WriteRead(BMP180_CAL_B2, 2);
            Array.Reverse(data);
            _calibrationData.B2 = BitConverter.ToInt16(data, 0);

            data = WriteRead(BMP180_CAL_MB, 2);
            Array.Reverse(data);
            _calibrationData.MB = BitConverter.ToInt16(data, 0);

            data = WriteRead(BMP180_CAL_MC, 2);
            Array.Reverse(data);
            _calibrationData.MC = BitConverter.ToInt16(data, 0);

            data = WriteRead(BMP180_CAL_MD, 2);
            Array.Reverse(data);
            _calibrationData.MD = BitConverter.ToInt16(data, 0);
        }

        private static byte [] WriteRead(byte register, int length)
        {
            var buffer = new byte[length];
            _sensor.WriteRead(new[] { register }, buffer);
            return buffer;
        }

        private static async Task ReadUncompestatedTemperature()
        {
            var command = new[] { BMP180_REG_CONTROL, BMP180_COM_TEMPERATURE };
            _sensor.Write(command);
            await Task.Delay(5);
            _uncompestatedTemperature= WriteRead(BMP180_REG_RESULT, 2); 
        }

        private static async Task ReadUncompestatedPressure()
        {
            var command = new[] { BMP180_REG_CONTROL, BMP180_COM_PRESSURE1 };
            _sensor.Write(command);
            await Task.Delay(8);
            _uncompestatedPressure = WriteRead(BMP180_REG_RESULT, 3);
        }

        internal static async Task ReadRawData()
        {
            await ReadUncompestatedTemperature();
            await ReadUncompestatedPressure();
        }

        internal static void CalculateTemperatureAndPressure()
        {
            //Niezbędne przesunięcia bitowe
            var temperature = (_uncompestatedTemperature[0] << 8) + _uncompestatedTemperature[1];
            var pressure = ((_uncompestatedPressure[0] <<16) + (_uncompestatedPressure[1]<<8 )+ _uncompestatedPressure[2])>>7;

            //Wyliczenie temperatury zgodnie z algorytmem
            var X1 = (temperature - _calibrationData.AC6) * (_calibrationData.AC5) >> 15;
            var X2 = (_calibrationData.MC << 11) / (X1 + _calibrationData.MD);
            var B5 = X1 + X2;
            var t = (B5 + 8) >> 4;
            Temperature = t / 10.0;

            //Wyliczenie ciśnienia zgodnie z algorytmem
            //FIX 
            long B6 = B5 - 4000;
            long X1_l = (BMP180_CAL_B2 * (B6 * (B6 >> 12)) >> 11);
            long X2_l = BMP180_CAL_AC2 * (B6 >> 11);
            long X3 = X1_l + X2_l;
            long B3 = (((BMP180_CAL_AC1 * 4 + X3) << 1) + 2) >>2;
            X1_l = BMP180_CAL_AC3 * B6 >> 13;
            X2_l = (BMP180_CAL_B1 * (B6 * B6 >> 12)) >> 16;
            X3 = ((X1_l + X2_l) + 2) >> 2;
            UInt64 B4 = BMP180_CAL_AC4 * ((UInt64)X3 + 32768) >> 15;
            UInt64 B7 = ((UInt64)(pressure - B3)) * (50000 >> 1);
            long p;

            if(B7 < 0x80000000)
            {
                p = ((long)B7 * 2) / (long)B4;
            }

            else
            {
                p = ((long)B7 / (long)B4) * 2;
            }
            long x = (p >> 8) * (p >> 8);
            x = (x * 3038) >> 16;
            long y = ((-7357) * (int)p) >> 16;
            long resultPressure = (int)p + ((int)x + y + 3791) >> 4;
            Pressure = resultPressure/100.0;

        }
    }

    public sealed class CalibrationData
    {
        public short AC1 { get; set; }
        public short AC2 { get; set; }
        public short AC3 { get; set; }
        public ushort AC4 { get; set; }
        public ushort AC5 { get; set; }
        public ushort AC6 { get; set; }
        public short B1 { get; set; }
        public short B2 { get; set; }
        public short MB { get; set; }
        public short MC { get; set; }
        public short MD { get; set; }
    }
}
