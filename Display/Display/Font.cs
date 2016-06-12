using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

/*  [0,0]   ... [0,127]
 *               
 *  ...  [y,x]  ...
 *  
 *  [127,0]     [127,127]
 */

namespace Display
{
    class Font
    {
        int CursorPosX;
        int CursorPosY;
        int DisplayMargin;
        byte CurrentRed, CurrentGreen, CurrentBlue;

        Pixel[,] pixelArray = new Pixel[128, 128];
        int screenHeight = 128;
        int screenWidth = 128;

        public Font(int margin = 0) 
        {
            DisplayMargin = margin;
            CursorPosY = margin+1;
            CursorPosX = margin+1;
            CurrentRed = 0;
            CurrentGreen = 0;
            CurrentBlue = 0;
            InitPixelArray();
        }

        private void InitPixelArray()   //Initializes array of pixels
        {
            for (int i = 0; i < screenHeight; i++)
            {
                for (int j = 0; j < screenWidth; j++)
                {
                    pixelArray[i, j] = new Pixel();
                }
            }
        }

        public void SetCurrentPosition(int x, int y) //Sets current cursor position for PixelArray
        {
            CursorPosY = y;
            CursorPosX = x;
        }

        public void SetCurrentColor(byte SetRed, byte SetGreen, byte SetBlue) //Sets current color for pixels in PixelArray
        {
            CurrentRed = SetRed;
            CurrentGreen = SetGreen;
            CurrentBlue = SetBlue;
        }

        private Pixel[,] WriteOnScreen(string text) //Writes to PixelArray given string starting from current cursor position
        {
            char[] textArray = text.ToCharArray();
            for (int i = 0; i < textArray.Length; i++)
            {
                if (CursorPosX > screenWidth - DisplayMargin) NewLine();
                switch (textArray[i])
                {
                    case ' ':
                        WriteSpace();
                        break;
                    case '0':
                        WriteZero();
                        break;
                    case '1':
                        WriteOne();
                        break;
                    case '2':
                        WriteTwo();
                        break;
                    case '3':
                        WriteThree();
                        break;
                    case '4':
                        WriteFour();
                        break;
                    case '5':
                        WriteFive();
                        break;
                    case '6':
                        WriteSix();
                        break;
                    case '7':
                        WriteSeven();
                        break;
                    case '8':
                        WriteEight();
                        break;
                    case '9':
                        WriteNine();
                        break;
                    case '\\':
                        i++;
                        if (textArray[i].Equals('r')) WriteDegreeSymbol();
                        break;
                    case 'A':
                        WriteA();
                        break;
                    case 'B':
                        WriteB();
                        break;
                    case 'C':
                        WriteC();
                        break;
                    case 'D':
                        WriteD();
                        break;
                    case 'E':
                        WriteE();
                        break;
                    case 'F':
                        WriteF();
                        break;
                    case 'G':
                        WriteG();
                        break;
                    case 'H':
                        WriteH();
                        break;
                    case 'I':
                        WriteI();
                        break;
                    case 'J':
                        WriteJ();
                        break;
                    case 'K':
                        WriteK();
                        break;
                    case 'L':
                        WriteL();
                        break;
                    case 'M':
                        WriteM();
                        break;
                    case 'N':
                        WriteN();
                        break;
                    case 'O':
                        WriteO();
                        break;
                    case 'P':
                        WriteP();
                        break;
                    case 'Q':
                        WriteQ();
                        break;
                    case 'R':
                        WriteR();
                        break;
                    case 'S':
                        WriteS();
                        break;
                    case 'T':
                        WriteT();
                        break;
                    case 'U':
                        WriteU();
                        break;
                    case 'W':
                        WriteW();
                        break;
                    case 'V':
                        WriteV();
                        break;
                    case 'Y':
                        WriteY();
                        break;
                    case 'X':
                        WriteX();
                        break;
                    case 'Z':
                        WriteZ();
                        break;
                }
            }
            return pixelArray;
        }

        private void DrawPixel(int x, int y, byte red = 0, byte green = 0, byte blue = 0) //Set single pixel in PixelArray
        {
            try
            {
                if ((x < 0 + DisplayMargin) || (x >= 128 - DisplayMargin) || (y < 0 + DisplayMargin) || (y >= 128 - DisplayMargin)) throw new System.IndexOutOfRangeException();
                else
                {
                    pixelArray[y, x].Red = red;
                    pixelArray[y, x].Green = green;
                    pixelArray[y, x].Blue = blue;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.WriteLine("Writing out of range!\n{0}", e.ToString());
            }
        }  

        private void DrawHorizontalLine(int length, int startPosX, int startPosY) //Draw horizontal line from given position
        {
            for (int i = 0; i < length; i++) {
                DrawPixel(startPosY, startPosX + i, CurrentRed, CurrentGreen, CurrentBlue);
            }
        }

        private void DrawVerticalLine(int length, int startPosX, int startPosY) //Draw vertical line from given position
        {
            for (int i = 0; i < length; i++)
            {
                DrawPixel(startPosY + i, startPosX, CurrentRed, CurrentGreen, CurrentBlue);
            }
        }

        private void WriteSpace() //Moves current cursor position by 3 pixels to the right
        {
            CursorPosX += 3;
        }

        private void NewLine() //Moves current cursir oisutuion to new line
        {
            CursorPosX = DisplayMargin + 1;
            CursorPosY += 15;
        }

        private void WriteZero() //Writes '0' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 1);
            DrawVerticalLine(6, CursorPosY + 1, CursorPosX);
            DrawVerticalLine(6, CursorPosY + 1, CursorPosX + 5);
            CursorPosX += 7;
        }

        private void WriteOne() //Writes '1' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(2, CursorPosY+1, CursorPosX);
            DrawHorizontalLine(5, CursorPosY + 7, CursorPosX);
            DrawVerticalLine(7, CursorPosY, CursorPosX + 3);
            CursorPosX += 7;
        }

        private void WriteTwo() //Writes '2' to PixelArray string starting from current cursor position
        {
            DrawPixel(CursorPosY + 1, CursorPosX);
            DrawHorizontalLine(2, CursorPosY, CursorPosX+1);
            DrawHorizontalLine(3, CursorPosY + 1, CursorPosX + 4);
            DrawPixel(CursorPosY + 4, CursorPosX + 3);
            DrawPixel(CursorPosY + 5, CursorPosX + 2);
            DrawPixel(CursorPosY + 6, CursorPosX + 1);
            DrawHorizontalLine(5, CursorPosY + 7, CursorPosX);
            CursorPosX += 7;
        }

        private void WriteThree() //Writes '3' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(5, CursorPosY, CursorPosX);
            DrawHorizontalLine(5, CursorPosY + 3, CursorPosX);
            DrawHorizontalLine(5, CursorPosY + 7, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX + 4);
            DrawVerticalLine(3, CursorPosY + 4, CursorPosX + 4);
            CursorPosX += 7;
        }

        private void WriteFour() //Writes '4' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(5, CursorPosY + 7, CursorPosX);
            DrawVerticalLine(8, CursorPosY, CursorPosX + 3);
            DrawPixel(CursorPosY + 1, CursorPosX + 2);
            DrawPixel(CursorPosY + 2, CursorPosX + 1);
            DrawPixel(CursorPosY + 3, CursorPosX + 1);
            DrawPixel(CursorPosY + 4, CursorPosX);
            CursorPosX += 7;
        }

        private void WriteFive() //Writes '5' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(5, CursorPosY, CursorPosX);
            DrawHorizontalLine(4, CursorPosY + 3, CursorPosX);
            DrawHorizontalLine(4, CursorPosY + 7, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX);
            DrawVerticalLine(3, CursorPosY + 4, CursorPosX + 4);
            CursorPosX += 7;
        }

        private void WriteSix() //Writes '6' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 3, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX);
            DrawVerticalLine(6, CursorPosY + 1, CursorPosX);
            DrawVerticalLine(3, CursorPosY + 4, CursorPosX + 4);
            DrawPixel(CursorPosY + 1, CursorPosX + 4);
            CursorPosX += 7;
        }

        private void WriteSeven() //Writes '7' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(5, CursorPosY, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX + 4);
            DrawVerticalLine(2, CursorPosY + 4, CursorPosX + 2);
            DrawVerticalLine(2, CursorPosY + 6, CursorPosX + 1);
            DrawPixel(CursorPosY + 3, CursorPosX + 3);
            CursorPosX += 7;
        }

        private void WriteEight() //Writes '8' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 3, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 1);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX + 4);
            DrawVerticalLine(3, CursorPosY + 4, CursorPosX);
            DrawVerticalLine(3, CursorPosY + 4, CursorPosX + 4);
            CursorPosX += 7;
        }

        private void WriteNine() //Writes '9' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 3, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX);
            DrawVerticalLine(6, CursorPosY + 1, CursorPosX + 4);
            DrawPixel(CursorPosY + 6, CursorPosX);
            CursorPosX += 7;
        }

        private void WriteDegreeSymbol() //Writes degree symbol to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(2, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(2, CursorPosY + 3, CursorPosX + 1);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX + 1);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX + 4);
            CursorPosX += 7;
        }

        private void WriteA() //Writes 'A' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 3, CursorPosX + 1);
            DrawVerticalLine(7, CursorPosY + 1, CursorPosX);
            DrawVerticalLine(7, CursorPosY + 1, CursorPosX + 4);
            CursorPosX += 7;
        }

        private void WriteB() //Writes 'B' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 3, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 1);
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX + 4);
            DrawVerticalLine(2, CursorPosY + 4, CursorPosX + 4);
            CursorPosX += 7;
        }

        private void WriteC() //Writes 'C' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 2);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 2);
            DrawVerticalLine(4, CursorPosY + 2, CursorPosX);
            DrawPixel(CursorPosY + 1, CursorPosX + 1);
            DrawPixel(CursorPosY + 6, CursorPosX + 1);
            DrawPixel(CursorPosY + 1, CursorPosX + 5);
            DrawPixel(CursorPosY + 6, CursorPosX + 5);
            CursorPosX += 7;
        }

        private void WriteD() //Writes 'D' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 1);
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            DrawVerticalLine(4, CursorPosY + 2, CursorPosX + 5);
            DrawPixel(CursorPosY + 1, CursorPosX + 4);
            DrawPixel(CursorPosY + 6, CursorPosX + 4);
            CursorPosX += 7;
        }

        private void WriteE() //Writes 'E' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(4, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY+3, CursorPosX + 1);
            DrawHorizontalLine(4, CursorPosY+7, CursorPosX + 1);
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            CursorPosX += 7;
        }

        private void WriteF() //Writes 'F' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(4, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 3, CursorPosX + 1);
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            CursorPosX += 7;
        }

        private void WriteG() //Writes 'G' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 2);
            DrawHorizontalLine(3, CursorPosY + 4, CursorPosX + 3);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 2);
            DrawVerticalLine(4, CursorPosY + 2, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 5, CursorPosX + 5);
            DrawPixel(CursorPosY + 1, CursorPosX + 1);
            DrawPixel(CursorPosY + 6, CursorPosX + 1);
            DrawPixel(CursorPosY + 1, CursorPosX + 5);
            CursorPosX += 7;
        }

        private void WriteH() //Writes 'H' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY + 3, CursorPosX + 1);
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            DrawVerticalLine(8, CursorPosY, CursorPosX + 5);
            CursorPosX += 7;
        }

        private void WriteI() //Writes 'I' to PixelArray string starting from current cursor position
        {
            DrawVerticalLine(8, CursorPosY, CursorPosX + 2);
            CursorPosX += 4;
        }

        private void WriteJ() //Writes 'J' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(2, CursorPosY + 7, CursorPosX);
            DrawVerticalLine(8, CursorPosY, CursorPosX + 2);
            CursorPosX += 4;
        }

        private void WriteK() //Writes 'K' to PixelArray string starting from current cursor position
        {
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 6, CursorPosX + 4);
            DrawPixel(CursorPosY, CursorPosX + 4);
            DrawPixel(CursorPosY + 1, CursorPosX + 3);
            DrawPixel(CursorPosY + 2, CursorPosX + 2);
            DrawPixel(CursorPosY + 3, CursorPosX + 1);
            DrawPixel(CursorPosY + 4, CursorPosX + 2);
            DrawPixel(CursorPosY + 5, CursorPosX + 3);
            CursorPosX += 7;
        }

        private void WriteL() //Writes 'L' to PixelArray string starting from current cursor position
        {
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 1);
            CursorPosX += 6;
        }

        private void WriteM() //Writes 'M' to PixelArray string starting from current cursor position
        {
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            DrawVerticalLine(2, CursorPosY, CursorPosX + 1);
            DrawVerticalLine(2, CursorPosY + 2, CursorPosX + 2);
            DrawVerticalLine(2, CursorPosY + 4, CursorPosX + 3);
            DrawVerticalLine(2, CursorPosY + 6, CursorPosX + 4);
            DrawVerticalLine(2, CursorPosY + 4, CursorPosX + 5);
            DrawVerticalLine(2, CursorPosY + 2, CursorPosX + 6);
            DrawVerticalLine(8, CursorPosY, CursorPosX + 7);
            CursorPosX += 9;
        }

        private void WriteN() //Writes 'N' to PixelArray string starting from current cursor position
        {
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            DrawVerticalLine(2, CursorPosY, CursorPosX + 1);
            DrawVerticalLine(2, CursorPosY + 2, CursorPosX + 2);
            DrawVerticalLine(2, CursorPosY + 4, CursorPosX + 3);
            DrawVerticalLine(2, CursorPosY + 6, CursorPosX + 4);
            DrawVerticalLine(8, CursorPosY, CursorPosX + 5);
            CursorPosX += 7;
        }

        private void WriteO() //Writes 'O' to PixelArray string starting from current cursor position
        {
            DrawVerticalLine(4, CursorPosY + 2, CursorPosX);
            DrawVerticalLine(4, CursorPosY + 2, CursorPosX + 6);
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 1);
            DrawPixel(CursorPosY + 1, CursorPosX + 1);
            DrawPixel(CursorPosY + 1, CursorPosX + 5);
            DrawPixel(CursorPosY + 6, CursorPosX + 1);
            DrawPixel(CursorPosY + 6, CursorPosX + 5);
            CursorPosX += 8;
        }

        private void WriteP() //Writes 'P' to PixelArray string starting from current cursor position
        {
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 4, CursorPosX + 1);
            DrawVerticalLine(3, CursorPosY + 1, CursorPosX + 4);
            CursorPosX += 6;
        }

        private void WriteQ() //Writes 'Q' to PixelArray string starting from current cursor position
        {
            DrawVerticalLine(4, CursorPosY + 2, CursorPosX);
            DrawVerticalLine(4, CursorPosY + 2, CursorPosX + 6);
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(5, CursorPosY + 7, CursorPosX + 1);
            DrawPixel(CursorPosY + 1, CursorPosX + 1);
            DrawPixel(CursorPosY + 1, CursorPosX + 5);
            DrawPixel(CursorPosY + 6, CursorPosX + 1);
            DrawPixel(CursorPosY + 6, CursorPosX + 5);
            DrawPixel(CursorPosY + 7, CursorPosX + 7);
            CursorPosX += 9;
        }

        private void WriteR() //Writes 'R' to PixelArray string starting from current cursor position
        {
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 4, CursorPosX + 1);
            DrawVerticalLine(3, CursorPosY + 1, CursorPosX + 4);
            DrawVerticalLine(2, CursorPosY + 6, CursorPosX + 4);
            DrawPixel(CursorPosY + 5, CursorPosX + 3);
            CursorPosX += 6;
        }

        private void WriteS() //Writes 'S' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 3, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 1);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX);
            DrawVerticalLine(3, CursorPosY + 4, CursorPosX + 4);
            CursorPosX += 6;
        }

        private void WriteT() //Writes 'T' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(5, CursorPosY + 7, CursorPosX + 1);
            DrawVerticalLine(7, CursorPosY + 1, CursorPosX + 2);
            CursorPosX += 7;
        }

        private void WriteU() //Writes 'U' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(4, CursorPosY + 7, CursorPosX + 1);
            DrawVerticalLine(7, CursorPosY, CursorPosX);
            DrawVerticalLine(7, CursorPosY, CursorPosX + 5);
            CursorPosX += 7;
        }

        private void WriteW() //Writes 'W' to PixelArray string starting from current cursor position
        {
            DrawVerticalLine(3, CursorPosY, CursorPosX);
            DrawVerticalLine(3, CursorPosY + 3, CursorPosX +1);
            DrawVerticalLine(2, CursorPosY + 6, CursorPosX + 2);
            DrawVerticalLine(3, CursorPosY + 3, CursorPosX + 3);
            DrawVerticalLine(3, CursorPosY, CursorPosX + 4);
            DrawVerticalLine(3, CursorPosY + 3, CursorPosX + 5);
            DrawVerticalLine(2, CursorPosY + 6, CursorPosX + 6);
            DrawVerticalLine(3, CursorPosY + 3, CursorPosX + 7);
            DrawVerticalLine(3, CursorPosY, CursorPosX + 8);
            CursorPosX += 9;
        }

        private void WriteV() //Writes 'V' to PixelArray string starting from current cursor position
        {
            DrawVerticalLine(3, CursorPosY, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 3, CursorPosX + 1);
            DrawVerticalLine(2, CursorPosY + 5, CursorPosX + 2);
            DrawPixel(CursorPosY + 7, CursorPosX + 3);
            DrawVerticalLine(3, CursorPosY, CursorPosX + 4);
            DrawVerticalLine(3, CursorPosY + 3, CursorPosX + 5);
            DrawVerticalLine(2, CursorPosY + 6, CursorPosX + 6);
            CursorPosX += 8;
        }

        private void WriteX() //Writes 'X' to PixelArray string starting from current cursor position
        {
            DrawVerticalLine(2, CursorPosY, CursorPosX);
            DrawVerticalLine(2, CursorPosY, CursorPosX + 4);
            DrawVerticalLine(2, CursorPosY + 3, CursorPosX + 2);
            DrawVerticalLine(2, CursorPosY + 6, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 6, CursorPosX + 4);
            DrawPixel(CursorPosY + 2, CursorPosX + 1);
            DrawPixel(CursorPosY + 5, CursorPosX + 1);
            DrawPixel(CursorPosY + 2, CursorPosX + 3);
            DrawPixel(CursorPosY + 5, CursorPosX + 3);
            CursorPosX += 6;
        }

        private void WriteY() //Writes 'Y' to PixelArray string starting from current cursor position
        {
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX);
            DrawVerticalLine(5, CursorPosY + 3, CursorPosX + 2);
            DrawPixel(CursorPosY, CursorPosX);
            DrawPixel(CursorPosY, CursorPosX + 4);
            CursorPosX += 6;
        }

        private void WriteZ() //Writes 'Z' to PixelArray string starting from current cursor position
        {
            DrawVerticalLine(6, CursorPosY, CursorPosX);
            DrawVerticalLine(6, CursorPosY + 7, CursorPosX);
            DrawPixel(CursorPosY + 6, CursorPosX);
            DrawPixel(CursorPosY + 5, CursorPosX + 1);
            DrawPixel(CursorPosY + 4, CursorPosX + 2);
            DrawPixel(CursorPosY + 3, CursorPosX + 3);
            DrawPixel(CursorPosY + 2, CursorPosX + 4);
            DrawPixel(CursorPosY + 1, CursorPosX + 5);
            CursorPosX += 8;
        }
    }
}
