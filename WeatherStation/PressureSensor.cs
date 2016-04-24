using System;
using Windows.Devices.I2c;
using Windows.Devices.Enumeration;
using System.Threading.Tasks;

namespace WeatherStation
{
    public static class PressureSensor
    {
        public static bool IsInitialized { get; set; } = false;
        private const byte BMP180_ADDR = 0x77;
        private static I2cDevice _sensor;
        //TODO Dodac rejestry 

        internal static async Task InitializeAsync()
        {
            var selectorString = I2cDevice.GetDeviceSelector();
            var devices = await DeviceInformation.FindAllAsync(selectorString);
            var deviceId = devices[0].Id;
            var settings = new I2cConnectionSettings(BMP180_ADDR);
            settings.BusSpeed = I2cBusSpeed.StandardMode;
            _sensor = await I2cDevice.FromIdAsync(deviceId, settings);

            //TODO Odczytać dane kalibracyjne z czujnika
       
            IsInitialized = true;
        }
    }
}
