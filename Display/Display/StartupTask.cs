using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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
        private int _ticksCounter = 0;
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
            _timer = ThreadPoolTimer.CreatePeriodicTimer(TimerTick, TimeSpan.FromMilliseconds(10000));
        }

        private async void TimerTick(ThreadPoolTimer timer)
        {
            _display.InitInterface();
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

            catch(Exception e)
            {
                _display.DisplayError("SENSOR PROBLEM");
            }

            finally
            {
                var temperature = _pressureSensor.Temperature;
                var pressure = _pressureSensor.Pressure;

                //Write data to screen
                _display.DisplayMeasurements(temperature, pressure);

                //Once per minute, sent measurements to Api
                _ticksCounter++;
                if (_ticksCounter % 6 == 0)
                {
                    SendToApi(temperature, pressure);
                }

            }
        }

        private void SendToApi(double temperature, double pressure)
        {
            var measurements = new Measurement();
            var json = JsonConvert.SerializeObject(measurements);
            //Send serialized result
        }

    }
}
