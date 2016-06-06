using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.Devices.Gpio;
using System.Threading.Tasks;
using Windows.Devices.Spi;
using Windows.Devices.Enumeration;
using System.Threading;
using Windows.System.Threading;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Display
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral deferral;
        private GpioController _gpioController;
        private SpiDevice _spiDisplay;
        private GpioPin _configurationPin;
        private GpioPin _resetPin;
        private ThreadPoolTimer _timer;
        private const string SPI_CONTROLLER_NAME = "SPI0";
        private const int SPI_CHIP_SELECT_LINE = 0; 
        private const int A0_PIN = 23;
        private const int RESET_PIN = 24;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            InitGpio();
            InitSpi();
            InitChip();
            await PressureSensor.InitializeAsync();
            await PressureSensor.ReadRawData();
            PressureSensor.CalculateTemperatureAndPressure();
            var temperature = PressureSensor.Temperature;
            var pressure = PressureSensor.Pressure;
            WriteTemperature(temperature);
            // _timer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick, TimeSpan.FromMilliseconds(10000));
        }

        private async void Timer_Tick(ThreadPoolTimer timer)
        {
            await PressureSensor.ReadRawData();
            PressureSensor.CalculateTemperatureAndPressure();
            var temperature = PressureSensor.Temperature;
            var pressure = PressureSensor.Pressure;
            WriteTemperature(temperature);
        }
        private void InitGpio()
        {
            _gpioController = GpioController.GetDefault();
            _configurationPin = _gpioController.OpenPin(A0_PIN);
            _configurationPin.SetDriveMode(GpioPinDriveMode.Output);
            _configurationPin.Write(GpioPinValue.High);

            _resetPin = _gpioController.OpenPin(RESET_PIN);
            _resetPin.SetDriveMode(GpioPinDriveMode.Output);
            _resetPin.Write(GpioPinValue.High);
        }
        private async void InitSpi() 
        {
            var settings = new SpiConnectionSettings(SPI_CHIP_SELECT_LINE);
            settings.ClockFrequency = 8000000;                           
            settings.Mode = SpiMode.Mode0;                                                                                                          
            string spiAqs = SpiDevice.GetDeviceSelector(SPI_CONTROLLER_NAME);       
            var devicesInfo = await DeviceInformation.FindAllAsync(spiAqs);         
            _spiDisplay = await SpiDevice.FromIdAsync(devicesInfo[0].Id, settings);  
        }
        private void SendCommand(byte [] command)
        {
            _configurationPin.Write(GpioPinValue.Low);
            _spiDisplay.Write(command);
            _configurationPin.Write(GpioPinValue.High);
        }
        private void SendData(byte [] data)
        {
            _configurationPin.Write(GpioPinValue.High);
            _spiDisplay.Write(data);
            _configurationPin.Write(GpioPinValue.Low);
        }
        private void SendPixelData(Int32 red, Int32 green, Int32 blue)
        {
            var redByteArray = BitConverter.GetBytes(red);
            var greenByteArray = BitConverter.GetBytes(green);
            var blueByteArray = BitConverter.GetBytes(blue);

            var finalRedArray = new byte[1];
            finalRedArray[0] = redByteArray.First();

            var finalGreenArray = new byte[1];
            finalGreenArray[0] = greenByteArray.First();

            var finalBlueArray = new byte[1];
            finalBlueArray[0] = blueByteArray.First();

            SendData(finalBlueArray);
            SendData(finalGreenArray);
            SendData(finalRedArray);
        }

        private void InitChip()
        {
            SendCommand(new byte[] { 0x11 }); //Exit sleep
            Task.Delay(1);

            SendCommand(new byte[] { 0x13 }); //Normal mode
            Task.Delay(1);

            SendCommand(new byte[] { 0x38 }); //Iddle mode off
            Task.Delay(1);
            
            SendCommand(new byte[] { 0x29 }); //Display on
            Task.Delay(1);
        }

        public void WriteTemperature(double temperature)
        {
            SendCommand(new byte[] { 0x2C }); //Ram Write

            var pixelArray = GetTemperatureScreen(temperature);

            foreach (var pixel in pixelArray)
            {
                SendPixelData(pixel.Red, pixel.Green, pixel.Blue);
            }

            SendCommand(new byte[] { 0x00 }); //Interrupt Ram writing
        }

        private Pixel[,] GetTemperatureScreen(double temp)
        {
            var pixelArray = new Pixel[128, 128];
            var screenHeight = 128;
            var screenWidth = 128;

            var x = screenWidth / 2;
            var y = screenHeight / 2;

            for (int i=0; i<=127; i++)
            {
                for( int j=0; j<=127; j++)
                {
                    pixelArray[i, j] = new Pixel();
                }
            }

            foreach(var pixel in pixelArray)
            {
                pixel.Red = 63;
                pixel.Green = 63;
                pixel.Blue = 63;
            }

            WriteZero(pixelArray, 0, 0, 0, x-8, y);
            WriteZero(pixelArray, 0, 0, 0, x+8, y);
            WriteOne(pixelArray, 0, 0, 0, x, y);
            WriteOne(pixelArray, 0, 0, 0, x+16, y);
            WriteOne(pixelArray, 0, 0, 0, x-16, y);
            return pixelArray;
        }

        private void WriteZero(Pixel[,] pixelArray, byte red,byte Green, byte blue, int x, int y)
        {
            #region Red
            //Górna połowa
            pixelArray[y, x].Red = red;
            pixelArray[y + 1, x].Red = red;
            pixelArray[y + 2, x].Red = red;
            pixelArray[y + 3, x].Red = red;
            pixelArray[y + 4, x].Red = red;
            pixelArray[y, x + 1].Red = red;
            pixelArray[y + 1, x + 1].Red = red;
            pixelArray[y + 2, x + 1].Red = red;
            pixelArray[y + 3, x + 1].Red = red;
            pixelArray[y + 4, x + 1].Red = red;

            pixelArray[y, x + 2].Red = red;
            pixelArray[y, x + 3].Red = red;
            pixelArray[y + 1, x + 2].Red = red;
            pixelArray[y + 1, x + 3].Red = red;

            pixelArray[y, x + 4].Red = red;
            pixelArray[y + 1, x + 4].Red = red;
            pixelArray[y + 2, x + 4].Red = red;
            pixelArray[y + 3, x + 4].Red = red;
            pixelArray[y + 4, x + 4].Red = red;
            pixelArray[y, x + 5].Red = red;
            pixelArray[y + 1, x + 5].Red = red;
            pixelArray[y + 2, x + 5].Red = red;
            pixelArray[y + 3, x + 5].Red = red;
            pixelArray[y + 4, x + 5].Red = red;

            //Dolna połowa
            pixelArray[y + 5, x].Red = red;
            pixelArray[y + 6, x].Red = red;
            pixelArray[y + 7, x].Red = red;
            pixelArray[y + 8, x].Red = red;
            pixelArray[y + 9, x].Red = red;
            pixelArray[y + 5, x + 1].Red = red;
            pixelArray[y + 6, x + 1].Red = red;
            pixelArray[y + 7, x + 1].Red = red;
            pixelArray[y + 8, x + 1].Red = red;
            pixelArray[y + 9, x + 1].Red = red;

            pixelArray[y + 5, x + 4].Red = red;
            pixelArray[y + 6, x + 4].Red = red;
            pixelArray[y + 7, x + 4].Red = red;
            pixelArray[y + 8, x + 4].Red = red;
            pixelArray[y + 9, x + 4].Red = red;
            pixelArray[y + 5, x + 5].Red = red;
            pixelArray[y + 6, x + 5].Red = red;
            pixelArray[y + 7, x + 5].Red = red;
            pixelArray[y + 8, x + 5].Red = red;
            pixelArray[y + 9, x + 5].Red = red;

            pixelArray[y + 8, x + 2].Red = red;
            pixelArray[y + 8, x + 3].Red = red;
            pixelArray[y + 9, x + 2].Red = red;
            pixelArray[y + 9, x + 3].Red = red;
            #endregion

            #region Green
            //Górna połowa
            pixelArray[y, x].Green = Green;
            pixelArray[y + 1, x].Green = Green;
            pixelArray[y + 2, x].Green = Green;
            pixelArray[y + 3, x].Green = Green;
            pixelArray[y + 4, x].Green = Green;
            pixelArray[y, x + 1].Green = Green;
            pixelArray[y + 1, x + 1].Green = Green;
            pixelArray[y + 2, x + 1].Green = Green;
            pixelArray[y + 3, x + 1].Green = Green;
            pixelArray[y + 4, x + 1].Green = Green;

            pixelArray[y, x + 2].Green = Green;
            pixelArray[y, x + 3].Green = Green;
            pixelArray[y + 1, x + 2].Green = Green;
            pixelArray[y + 1, x + 3].Green = Green;

            pixelArray[y, x + 4].Green = Green;
            pixelArray[y + 1, x + 4].Green = Green;
            pixelArray[y + 2, x + 4].Green = Green;
            pixelArray[y + 3, x + 4].Green = Green;
            pixelArray[y + 4, x + 4].Green = Green;
            pixelArray[y, x + 5].Green = Green;
            pixelArray[y + 1, x + 5].Green = Green;
            pixelArray[y + 2, x + 5].Green = Green;
            pixelArray[y + 3, x + 5].Green = Green;
            pixelArray[y + 4, x + 5].Green = Green;

            //Dolna połowa
            pixelArray[y + 5, x].Green = Green;
            pixelArray[y + 6, x].Green = Green;
            pixelArray[y + 7, x].Green = Green;
            pixelArray[y + 8, x].Green = Green;
            pixelArray[y + 9, x].Green = Green;
            pixelArray[y + 5, x + 1].Green = Green;
            pixelArray[y + 6, x + 1].Green = Green;
            pixelArray[y + 7, x + 1].Green = Green;
            pixelArray[y + 8, x + 1].Green = Green;
            pixelArray[y + 9, x + 1].Green = Green;

            pixelArray[y + 5, x + 4].Green = Green;
            pixelArray[y + 6, x + 4].Green = Green;
            pixelArray[y + 7, x + 4].Green = Green;
            pixelArray[y + 8, x + 4].Green = Green;
            pixelArray[y + 9, x + 4].Green = Green;
            pixelArray[y + 5, x + 5].Green = Green;
            pixelArray[y + 6, x + 5].Green = Green;
            pixelArray[y + 7, x + 5].Green = Green;
            pixelArray[y + 8, x + 5].Green = Green;
            pixelArray[y + 9, x + 5].Green = Green;

            pixelArray[y + 8, x + 2].Green = Green;
            pixelArray[y + 8, x + 3].Green = Green;
            pixelArray[y + 9, x + 2].Green = Green;
            pixelArray[y + 9, x + 3].Green = Green;
            #endregion

            #region Blue

            //Górna połowa
            pixelArray[y, x].Blue = blue;
            pixelArray[y + 1, x].Blue = blue;
            pixelArray[y + 2, x].Blue = blue;
            pixelArray[y + 3, x].Blue = blue;
            pixelArray[y + 4, x].Blue = blue;
            pixelArray[y, x + 1].Blue = blue;
            pixelArray[y + 1, x + 1].Blue = blue;
            pixelArray[y + 2, x + 1].Blue = blue;
            pixelArray[y + 3, x + 1].Blue = blue;
            pixelArray[y + 4, x + 1].Blue = blue;

            pixelArray[y, x + 2].Blue = blue;
            pixelArray[y, x + 3].Blue = blue;
            pixelArray[y + 1, x + 2].Blue = blue;
            pixelArray[y + 1, x + 3].Blue = blue;

            pixelArray[y, x + 4].Blue = blue;
            pixelArray[y + 1, x + 4].Blue = blue;
            pixelArray[y + 2, x + 4].Blue = blue;
            pixelArray[y + 3, x + 4].Blue = blue;
            pixelArray[y + 4, x + 4].Blue = blue;
            pixelArray[y, x + 5].Blue = blue;
            pixelArray[y + 1, x + 5].Blue = blue;
            pixelArray[y + 2, x + 5].Blue = blue;
            pixelArray[y + 3, x + 5].Blue = blue;
            pixelArray[y + 4, x + 5].Blue = blue;

            //Dolna połowa
            pixelArray[y + 5, x].Blue = blue;
            pixelArray[y + 6, x].Blue = blue;
            pixelArray[y + 7, x].Blue = blue;
            pixelArray[y + 8, x].Blue = blue;
            pixelArray[y + 9, x].Blue = blue;
            pixelArray[y + 5, x + 1].Blue = blue;
            pixelArray[y + 6, x + 1].Blue = blue;
            pixelArray[y + 7, x + 1].Blue = blue;
            pixelArray[y + 8, x + 1].Blue = blue;
            pixelArray[y + 9, x + 1].Blue = blue;

            pixelArray[y + 5, x + 4].Blue = blue;
            pixelArray[y + 6, x + 4].Blue = blue;
            pixelArray[y + 7, x + 4].Blue = blue;
            pixelArray[y + 8, x + 4].Blue = blue;
            pixelArray[y + 9, x + 4].Blue = blue;
            pixelArray[y + 5, x + 5].Blue = blue;
            pixelArray[y + 6, x + 5].Blue = blue;
            pixelArray[y + 7, x + 5].Blue = blue;
            pixelArray[y + 8, x + 5].Blue = blue;
            pixelArray[y + 9, x + 5].Blue = blue;

            pixelArray[y + 8, x + 2].Blue = blue;
            pixelArray[y + 8, x + 3].Blue = blue;
            pixelArray[y + 9, x + 2].Blue = blue;
            pixelArray[y + 9, x + 3].Blue = blue;
            #endregion
        }

        private void WriteOne(Pixel[,] pixelArray, byte red, byte Green, byte blue, int x, int y)
        {
            #region Red
            //Górna połowa
            pixelArray[y, x + 3].Red = red;
            pixelArray[y + 1, x + 3].Red = red;
            pixelArray[y + 2, x + 3].Red = red;
            pixelArray[y + 3, x + 3].Red = red;
            pixelArray[y + 4, x + 3].Red = red;
            pixelArray[y, x + 2].Red = red;
            pixelArray[y + 1, x + 2].Red = red;
            pixelArray[y + 2, x + 2].Red = red;
            pixelArray[y + 3, x + 2].Red = red;
            pixelArray[y + 4, x + 2].Red = red;

            //Dolna połowa
            pixelArray[y + 5, x + 3].Red = red;
            pixelArray[y + 6, x + 3].Red = red;
            pixelArray[y + 7, x + 3].Red = red;
            pixelArray[y + 8, x + 3].Red = red;
            pixelArray[y + 9, x + 3].Red = red;
            pixelArray[y + 5, x + 2].Red = red;
            pixelArray[y + 6, x + 2].Red = red;
            pixelArray[y + 7, x + 2].Red = red;
            pixelArray[y + 8, x + 2].Red = red;
            pixelArray[y + 9, x + 2].Red = red;

            #endregion

            #region Green
            //Górna połowa
            pixelArray[y, x + 2].Green = Green;
            pixelArray[y + 1, x + 2].Green = Green;
            pixelArray[y + 2, x + 2].Green = Green;
            pixelArray[y + 3, x + 2].Green = Green;
            pixelArray[y + 4, x + 2].Green = Green;
            pixelArray[y, x + 3].Green = Green;
            pixelArray[y + 1, x + 3].Green = Green;
            pixelArray[y + 2, x + 3].Green = Green;
            pixelArray[y + 3, x + 3].Green = Green;
            pixelArray[y + 4, x + 3].Green = Green;

            //Dolna połowa
            pixelArray[y + 5, x + 2].Green = Green;
            pixelArray[y + 6, x + 2].Green = Green;
            pixelArray[y + 7, x + 2].Green = Green;
            pixelArray[y + 8, x + 2].Green = Green;
            pixelArray[y + 9, x + 2].Green = Green;
            pixelArray[y + 5, x + 3].Green = Green;
            pixelArray[y + 6, x + 3].Green = Green;
            pixelArray[y + 7, x + 3].Green = Green;
            pixelArray[y + 8, x + 3].Green = Green;
            pixelArray[y + 9, x + 3].Green = Green;
            #endregion

            #region Blue

            //Górna połowa
            pixelArray[y, x + 2].Blue = blue;
            pixelArray[y + 1, x + 2].Blue = blue;
            pixelArray[y + 2, x + 2].Blue = blue;
            pixelArray[y + 3, x + 2].Blue = blue;
            pixelArray[y + 4, x + 2].Blue = blue;
            pixelArray[y, x + 3].Blue = blue;
            pixelArray[y + 1, x + 3].Blue = blue;
            pixelArray[y + 2, x + 3].Blue = blue;
            pixelArray[y + 3, x + 3].Blue = blue;
            pixelArray[y + 4, x + 3].Blue = blue;

            //Dolna połowa
            pixelArray[y + 5, x + 2].Blue = blue;
            pixelArray[y + 6, x + 2].Blue = blue;
            pixelArray[y + 7, x + 2].Blue = blue;
            pixelArray[y + 8, x + 2].Blue = blue;
            pixelArray[y + 9, x + 2].Blue = blue;
            pixelArray[y + 5, x + 3].Blue = blue;
            pixelArray[y + 6, x + 3].Blue = blue;
            pixelArray[y + 7, x + 3].Blue = blue;
            pixelArray[y + 8, x + 3].Blue = blue;
            pixelArray[y + 9, x + 3].Blue = blue;
            #endregion
        }
    }
}
