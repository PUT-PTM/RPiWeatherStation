using System;
using System.IO;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;

namespace Display
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral deferral;
        private ThreadPoolTimer _timer;
        private Display _display;
        private PressureSensor _pressureSensor;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();

            //Initialize display
            _display = new Display();
            _display.InitGpio();
            await _display.InitSpi();
            await _display.InitChip(); 

            //Initialize pressure sensor
            _pressureSensor = new PressureSensor();

            //Periodic timer, which read and display data
            _timer = ThreadPoolTimer.CreatePeriodicTimer(TimerTick, TimeSpan.FromMilliseconds(30000));
        }

        private async void TimerTick(ThreadPoolTimer timer)
        {
            try
            {
                if(_pressureSensor.IsInitialized)
                {
                    //Read data from sensor
                    await _pressureSensor.ReadRawData();
                    _pressureSensor.CalculateTemperatureAndPressure();
                }

                else
                {
                    //Try again
                    await _pressureSensor.InitializeAsync();
                }    
            }

            catch(FileNotFoundException e)
            {
                _display.DisplayError("SENSOR PROBLEM");
            }

            finally
            {
                var temperature = _pressureSensor.Temperature;
                var pressure = _pressureSensor.Pressure;

                //Write data to screen
                _display.DisplayMeasurements(temperature, pressure);
            }
        }

    }
}
