using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;

namespace WeatherStation
{
    /*  PIN MAP
     *  VCC     ->  3,3V PWR    = RPi2 Pin 1 or 17
     *  GND     ->  GND         = RPi2 Pin 6 0r 9 or 14 or 20 or 25 or 30 or 34 or 39
     *  CS      ->  SPI0 CS0    = RPi2 Pin 24
     *  RESET   ->  GPIO Pin 23 = RPi2 Pin 16
     *  A0      ->  GPIO Pin 22 = RPi2 Pin 15
     *  SDA     ->  SPI0 MOSI   = RPi2 Pin 19
     *  SCK     ->  SPI0 SCLK   = RPi2 Pin 23
     *  LED     ->  5V PWR      = RPi2 Pin 2 or 4 */

    class Display
    {
        private const string SPI_CONTROLLER_NAME = "SPI0";
        private const Int32 SPI_CHIP_SELECT_LINE = 0;       /* Line 0   = RPi2 Pin 34*/
        private const Int32 DATA_COMMAND_PIN = 22;          /* GPIO 22  = RPi2 Pin 15 */
        private const Int32 RESET_PIN = 23;                 /* GPIO 23  = RPi2 Pin 16 */

        GpioController IoController;
        GpioPin DataCommandPin;
        GpioPin ResetPin;
        SpiDevice SpiDisplay;

        /* Initialize the GPIO */
        private void InitGpio()
        {
            try
            {
                IoController = GpioController.GetDefault(); /* Get the default GPIO controller on the system */

                /* Initialize a pin as output for the Data/Command line on the display  */
                DataCommandPin = IoController.OpenPin(DATA_COMMAND_PIN);
                DataCommandPin.Write(GpioPinValue.High);
                DataCommandPin.SetDriveMode(GpioPinDriveMode.Output);

                /* Initialize a pin as output for the hardware Reset line on the display */
                ResetPin = IoController.OpenPin(RESET_PIN);
                ResetPin.Write(GpioPinValue.High);
                ResetPin.SetDriveMode(GpioPinDriveMode.Output);
            }
            /* If initialization fails, throw an exception */
            catch (Exception ex)
            {
                throw new Exception("GPIO initialization failed", ex);
            }
        }

        /* Initialize the SPI bus */
        private async Task InitSpi()
        {
            try
            {
                var settings = new SpiConnectionSettings(SPI_CHIP_SELECT_LINE); /* Create SPI initialization settings                               */
                settings.ClockFrequency = 10000000;                             /* Datasheet specifies maximum SPI clock frequency of 10MHz         */
                settings.Mode = SpiMode.Mode3;                                  /* The display expects an idle-high clock polarity, we use Mode3
                                                                         * to set the clock polarity and phase to: CPOL = 1, CPHA = 1
                                                                         */

                string spiAqs = SpiDevice.GetDeviceSelector(SPI_CONTROLLER_NAME);       /* Find the selector string for the SPI bus controller          */
                var devicesInfo = await DeviceInformation.FindAllAsync(spiAqs);         /* Find the SPI bus controller device with our selector string  */
                SpiDisplay = await SpiDevice.FromIdAsync(devicesInfo[0].Id, settings);  /* Create an SpiDevice with our bus controller and SPI settings */

            }
            /* If initialization fails, display the exception and stop running */
            catch (Exception ex)
            {
                throw new Exception("SPI Initialization Failed", ex);
            }
        }

        /* Send SPI commands to power up and initialize the display */
        private async Task InitDisplay()
        {
            /* Initialize the display */
            try
            {
                /* See the datasheet for more details on these commands: http://www.adafruit.com/datasheets/SSD1306.pdf             */
                await ResetDisplay();                   /* Perform a hardware reset on the display                                  */
                DisplaySendCommand(CMD_CHARGEPUMP_ON);  /* Turn on the internal charge pump to provide power to the screen          */
                DisplaySendCommand(CMD_MEMADDRMODE);    /* Set the addressing mode to "horizontal"                                  */
                DisplaySendCommand(CMD_SEGREMAP);       /* Flip the display horizontally, so it's easier to read on the breadboard  */
                DisplaySendCommand(CMD_COMSCANDIR);     /* Flip the display vertically, so it's easier to read on the breadboard    */
                DisplaySendCommand(CMD_DISPLAY_ON);     /* Turn the display on                                                      */
            }
            catch (Exception ex)
            {
                throw new Exception("Display Initialization Failed", ex);
            }
        }

        /* Update the SPI display to mirror the textbox contents */
        private void DisplayTextBoxContents()
        {
            try
            {
                ClearDisplayBuf();  /* Blank the display buffer             */
                WriteLineDisplayBuf(Display_TextBoxLine0.Text, 0, 0);
                WriteLineDisplayBuf(Display_TextBoxLine1.Text, 0, 1);
                WriteLineDisplayBuf(Display_TextBoxLine2.Text, 0, 2);
                WriteLineDisplayBuf(Display_TextBoxLine3.Text, 0, 3);
                DisplayUpdate();    /* Write our changes out to the display */
            }
            /* Show an error if we can't update the display */
            catch (Exception ex)
            {
                Text_Status.Text = "Status: Failed to update display";
                Text_Status.Text = "\nException: " + ex.Message;
            }
        }

        /* Updates the display when the textbox contents change */
        private void Display_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayTextBoxContents();
        }

        /*
 * NAME:        WriteCharDisplayBuf
 * DESCRIPTION: Writes one character to the display screen buffer (DisplayUpdate() needs to be called subsequently to output the buffer to the screen)
 * INPUTS:
 *
 * Character: The character we want to draw. In this sample, special characters like tabs and newlines are not supported.
 * Col:       The horizontal column we want to start drawing at. This is equivalent to the 'X' axis pixel position.
 * Row:       The vertical row we want to write to. The screen is divided up into 4 rows of 16 pixels each, so valid values for Row are 0,1,2,3.
 *
 * RETURN VALUE:
 * We return the number of horizontal pixels used. This value is 0 if Row/Col are out-of-bounds, or if the character isn't available in the font.
 */
        private UInt32 WriteCharDisplayBuf(Char Chr, UInt32 Col, UInt32 Row)
        {
            /* Check that we were able to find the font corresponding to our character */
            FontCharacterDescriptor CharDescriptor = DisplayFontTable.GetCharacterDescriptor(Chr);
            if (CharDescriptor == null)
            {
                return 0;
            }

            /* Make sure we're drawing within the boundaries of the screen buffer */
            UInt32 MaxRowValue = (SCREEN_HEIGHT_PAGES / DisplayFontTable.FontHeightBytes) - 1;
            UInt32 MaxColValue = SCREEN_WIDTH_PX;
            if (Row > MaxRowValue)
            {
                return 0;
            }
            if ((Col + CharDescriptor.CharacterWidthPx + DisplayFontTable.FontCharSpacing) > MaxColValue)
            {
                return 0;
            }

            UInt32 CharDataIndex = 0;
            UInt32 StartPage = Row * 2;
            UInt32 EndPage = StartPage + CharDescriptor.CharacterHeightBytes;
            UInt32 StartCol = Col;
            UInt32 EndCol = StartCol + CharDescriptor.CharacterWidthPx;
            UInt32 CurrentPage = 0;
            UInt32 CurrentCol = 0;

            /* Copy the character image into the display buffer */
            for (CurrentPage = StartPage; CurrentPage < EndPage; CurrentPage++)
            {
                for (CurrentCol = StartCol; CurrentCol < EndCol; CurrentCol++)
                {
                    DisplayBuffer[CurrentCol, CurrentPage] = CharDescriptor.CharacterData[CharDataIndex];
                    CharDataIndex++;
                }
            }

            /* Pad blank spaces to the right of the character so there exists space between adjacent characters */
            for (CurrentPage = StartPage; CurrentPage < EndPage; CurrentPage++)
            {
                for (; CurrentCol < EndCol + DisplayFontTable.FontCharSpacing; CurrentCol++)
                {
                    DisplayBuffer[CurrentCol, CurrentPage] = 0x00;
                }
            }

            /* Return the number of horizontal pixels used by the character */
            return CurrentCol - StartCol;
        }

        /* Writes the Display Buffer out to the physical screen for display */
        private void DisplayUpdate()
        {
            int Index = 0;
            /* We convert our 2-dimensional array into a serialized string of bytes that will be sent out to the display */
            for (int PageY = 0; PageY < SCREEN_HEIGHT_PAGES; PageY++)
            {
                for (int PixelX = 0; PixelX < SCREEN_WIDTH_PX; PixelX++)
                {
                    SerializedDisplayBuffer[Index] = DisplayBuffer[PixelX, PageY];
                    Index++;
                }
            }

            /* Write the data out to the screen */
            DisplaySendCommand(CMD_RESETCOLADDR);         /* Reset the column address pointer back to 0 */
            DisplaySendCommand(CMD_RESETPAGEADDR);        /* Reset the page address pointer back to 0   */
            DisplaySendData(SerializedDisplayBuffer);     /* Send the data over SPI                     */
        }

        /* Send graphics data to the screen */
        private void DisplaySendData(byte[] Data)
        {
            /* When the Data/Command pin is high, SPI data is treated as graphics data  */
            DataCommandPin.Write(GpioPinValue.High);
            SpiDisplay.Write(Data);
        }

        public async Task ResetDisplay(){
            ResetPin.Write(GpioPinValue.High);
        }

        public void DisplaySendCommand(byte[] command) {
            SpiDisplay.Write(command);
        }
    }
}
