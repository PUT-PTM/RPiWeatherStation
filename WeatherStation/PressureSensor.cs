using System;
using Windows.Devices.I2c;
using Windows.Devices.Enumeration;
using System.Threading.Tasks;

namespace WeatherStation
{
    public static class PressureSensor
    {
        public static bool IsInitialized { get; private set; } = false;
        private static I2cDevice _sensor;

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

        //Rejestry kalibracyjne
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

            //TODO Odczytać dane kalibracyjne z czujnika
       
            IsInitialized = true;
        }
    }
}
