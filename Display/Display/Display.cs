using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;

namespace Display
{
    public sealed class Display
    {
        private const string SPI_CONTROLLER_NAME = "SPI0";
        private const int SPI_CHIP_SELECT_LINE = 0;
        private const int A0_PIN = 23;
        private const int RESET_PIN = 24;
        private GpioController _gpioController;
        private SpiDevice _spiDisplay;
        private GpioPin _configurationPin;
        private GpioPin _resetPin;
        private Font _fontManager;

        public Display()
        {
            _fontManager = new Font(5);
        }
        public void InitGpio()
        {
            _gpioController = GpioController.GetDefault();
            _configurationPin = _gpioController.OpenPin(A0_PIN);
            _configurationPin.SetDriveMode(GpioPinDriveMode.Output);
            _configurationPin.Write(GpioPinValue.High);

            _resetPin = _gpioController.OpenPin(RESET_PIN);
            _resetPin.SetDriveMode(GpioPinDriveMode.Output);
            _resetPin.Write(GpioPinValue.High);
        }
        internal async Task InitSpi()
        {
            var settings = new SpiConnectionSettings(SPI_CHIP_SELECT_LINE);
            settings.ClockFrequency = 8000000;
            settings.Mode = SpiMode.Mode0;
            string spiAqs = SpiDevice.GetDeviceSelector(SPI_CONTROLLER_NAME);
            var devicesInfo = await DeviceInformation.FindAllAsync(spiAqs);
            _spiDisplay = await SpiDevice.FromIdAsync(devicesInfo[0].Id, settings);
        }
        internal async Task InitChip()
        {
            SendCommand(new byte[] { 0x11 }); //Exit sleep
            await Task.Delay(5);

            SendCommand(new byte[] { 0x13 }); //Normal mode
            await Task.Delay(5);

            SendCommand(new byte[] { 0x38 }); //Iddle mode off
            await Task.Delay(5);

            SendCommand(new byte[] { 0x29 }); //Display on
            await Task.Delay(5);
        }
        public void DisplayMeasurements(double temperature, double pressure)
        {
            _fontManager.InitPixelArray();
            _fontManager.SetCurrentColor(63, 63, 63);
            _fontManager.SetCurrentPosition(5, 5);
            DisplayTemperature(temperature);
            _fontManager.NewLine();
            DisplayPressure(pressure);
            SendToDisplay();
        }
        private void SendCommand(byte[] command)
        {
            _configurationPin.Write(GpioPinValue.Low);
            _spiDisplay.Write(command);
            _configurationPin.Write(GpioPinValue.High);
        }
        private void SendData(byte[] data)
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
        private void DisplayTemperature(double temperature)
        {
            var tempString = $"TEMPERATURA: {temperature}";
            _fontManager.WriteOnScreen(tempString);
        }
        private void DisplayPressure(double pressure)
        {
            var tempString = $"CISNIENIE: {pressure}";
            _fontManager.WriteOnScreen(tempString);
        }
        private void SendToDisplay()
        {
            SendCommand(new byte[] { 0x2C }); //Ram Write

            foreach (var pixel in _fontManager.pixelArray)
            {
                SendPixelData(pixel.Red, pixel.Green, pixel.Blue);
            }

            SendCommand(new byte[] { 0x00 }); //Interrupt Ram writing
        }
    }
}
