using System;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;
using Windows.System.Threading;
using Rinsen.IoT.OneWire;
using System.Collections.Generic;
using System.Diagnostics;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace WeatherStation
{
    public sealed class StartupTask : IBackgroundTask
    {

        BackgroundTaskDeferral deferral;
        private GpioPinValue value = GpioPinValue.High;
        private const int LED_PIN = 47;
        private GpioPin pin;
        private ThreadPoolTimer timer;
        private OneWireDeviceHandler _handler;
        private IEnumerable<DS18B20> _devices;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();

            InitGPIO();
            await PressureSensor.InitializeAsync();
            if (PressureSensor.IsInitialized)
            {
                await PressureSensor.ReadRawData();
                PressureSensor.CalculateTemperatureAndPressure();
            }
            //timer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick, TimeSpan.FromMilliseconds(5000));     
        }

        private void InitGPIO()
        {
            pin = GpioController.GetDefault().OpenPin(LED_PIN);
            pin.Write(GpioPinValue.High);
            pin.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void Timer_Tick(ThreadPoolTimer timer)
        {
            value = (value == GpioPinValue.High) ? GpioPinValue.Low : GpioPinValue.High;
            pin.Write(value);
        }
    
    }
}



