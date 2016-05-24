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

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace Display
{
    public sealed class StartupTask : IBackgroundTask
    {
        private GpioController _gpioController;
         private SpiDevice _spiDisplay;
        private GpioPin _configurationPin;
        private GpioPin _resetPin;
        private const string SPI_CONTROLLER_NAME = "SPI0";
        private const int SPI_CHIP_SELECT_LINE = 0; 
        private const int A0_PIN = 23;
        private const int RESET_PIN = 24;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            InitGpio();
            InitSpi();
            InitChip();
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
        private void InitChip()
        {
            SendCommand(new byte[] { 0x01 }); //Software reset
            Task.Delay(5);

            SendCommand(new byte[] { 0x11 }); //Exit sleep
            Task.Delay(120);

            SendCommand(new byte[] { 0x3A }); //Set Color format 16 bit
            SendData(new byte[] { 0x05 });
            Task.Delay(5);

            SendCommand(new byte[] { 0x13 }); //Normal mode
            Task.Delay(5);

            SendCommand(new byte[] { 0x38 }); //Iddle mode off
            Task.Delay(5);

            SendCommand(new byte[] { 0x2A }); //Column adress
            SendData(new byte[] { 0x00 });
            SendData(new byte[] { 128 });
            Task.Delay(5);

            SendCommand(new byte[] { 0x2B }); //Page adress
            SendData(new byte[] { 0x00 });
            SendData(new byte[] {128}); //160
            Task.Delay(5);

            SendCommand(new byte[] { 0x29 }); //Display on
            Task.Delay(5);
            SendCommand(new byte[] { 0x2C }); //Ram Write

            for (int j = 0; j< 16384; j++)
            {
                SendData(new byte[] { 0x64 });
                SendData(new byte[] { 0x64 });
            }
            SendCommand(new byte[] { 0x00 });//Interrupt Ram writing
            int i = 5;
        }
    }
}
