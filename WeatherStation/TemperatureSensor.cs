using Windows.Devices.Gpio;
using Windows.System.Threading;

namespace WeatherStation
{
    public static class TemperatureSensor
    {
        public static GpioPin TemperaturePin { get; set; }
        private const int TEMPERATURE_PIN = 4;
        public static double Temperature { get; set; } = 0;

        public static void InitSensor()
        {
            TemperaturePin = GpioController.GetDefault().OpenPin(TEMPERATURE_PIN);
            TemperaturePin.SetDriveMode(GpioPinDriveMode.InputPullUp);
        }

        public static void ReadTemperature(ThreadPoolTimer timer)
        {
            var value = TemperaturePin.Read();
        }
    }
}
