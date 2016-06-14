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
    public sealed class Font
    {
        private int CursorPosX;     //Current position on X-axis of cursor
        private int CursorPosY;     //Current position on Y-axis of cursor
        private byte CurrentRed, CurrentGreen, CurrentBlue; //Current values of colors used to draw

        private const int screenHeight = 128;   //Dimension of display
        private const int screenWidth = 128;    //Dimension of display

        internal Pixel[,] pixelArray = new Pixel[screenHeight, screenWidth];    //Container for drawing page to display
        private Dictionary<char, int> charWidth = new Dictionary<char, int>();  //Container for characters width

        public Font(byte red, byte green ,byte blue)
        {
            CursorPosY = 1;
            CursorPosX = 1;
            CurrentRed = red;
            CurrentGreen = green;
            CurrentBlue = blue;
            InitPixelArray();
            InitCharWidthDictionary();
            InitInterface();
        }

        private void InitCharWidthDictionary()  //Initializes dictionary of characters width
        {
            charWidth.Add('0', 5);
            charWidth.Add('1', 5);
            charWidth.Add('2', 5);
            charWidth.Add('3', 5);
            charWidth.Add('4', 5);
            charWidth.Add('5', 5);
            charWidth.Add('6', 5);
            charWidth.Add('7', 5);
            charWidth.Add('8', 5);
            charWidth.Add('9', 5);
            charWidth.Add('A', 5);
            charWidth.Add('B', 5);
            charWidth.Add('C', 6);
            charWidth.Add('D', 6);
            charWidth.Add('E', 5);
            charWidth.Add('F', 5);
            charWidth.Add('G', 6);
            charWidth.Add('H', 6);
            charWidth.Add('I', 3);
            charWidth.Add('J', 3);
            charWidth.Add('K', 5);
            charWidth.Add('L', 4);
            charWidth.Add('M', 9);
            charWidth.Add('N', 6);
            charWidth.Add('O', 7);
            charWidth.Add('P', 5);
            charWidth.Add('Q', 8);
            charWidth.Add('R', 5);
            charWidth.Add('r', 5);
            charWidth.Add('S', 5);
            charWidth.Add('T', 6);
            charWidth.Add('U', 5);
            charWidth.Add('W', 9);
            charWidth.Add('V', 7);
            charWidth.Add('X', 5);
            charWidth.Add('Y', 5);
            charWidth.Add('Z', 6);
            charWidth.Add('\\', 0);
            charWidth.Add(' ', 3);
            charWidth.Add('.', 4);
            charWidth.Add(',', 4);
            charWidth.Add(';', 4);
            charWidth.Add(':', 4);
        }

        public void InitPixelArray()   //Initializes array of pixels
        {
            for (int i = 0; i < screenHeight; i++)
            {
                for (int j = 0; j < screenWidth; j++)
                {
                    pixelArray[i, j] = new Pixel();
                }
            }
        }

        private void SetCurrentPosition(int x, int y) //Sets current cursor position for PixelArray
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

        public void WriteOnScreen(string text) //Writes to PixelArray given string starting from current cursor position
        {
            char[] textArray = text.ToCharArray();
            for (int i = 0; i < textArray.Length; i++)
            {
                if ((CursorPosX > screenWidth) || (CursorPosX + charWidth[textArray[i]] > screenWidth)) NewLine();
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
                    case '.':
                        WriteDot();
                        break;
                    case ',':
                        WriteComma();
                        break;
                    case ';':
                        WriteSemicolon();
                        break;
                    case ':':
                        WriteColon();
                        break;
                }
            }
        }

        private void DrawPixel(int y, int x) //Set single pixel in PixelArray
        {
            try
            {
                if ((x < 0) || (x >= screenWidth) || (y < 0) || (y >= screenHeight)) throw new System.IndexOutOfRangeException();
                else
                {
                    pixelArray[y, x].Red = CurrentRed;
                    pixelArray[y, x].Green = CurrentGreen;
                    pixelArray[y, x].Blue = CurrentBlue;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                Debug.WriteLine("Writing out of range!\n{0}", e.ToString());
            }
        }

        private void DrawHorizontalLine(int length, int startPosY, int startPosX) //Draw horizontal line from given position
        {
            for (int i = 0; i < length; i++)
            {
                DrawPixel(startPosY, startPosX + i);
            }
        }

        private void DrawVerticalLine(int length, int startPosY, int startPosX) //Draw vertical line from given position
        {
            for (int i = 0; i < length; i++)
            {
                DrawPixel(startPosY + i, startPosX);
            }
        }

        private void DrawFrame(int length, int height, int borderThickness, int startPosX, int startPosY) //Draw frame from given position with given thickness
        {
            for (int i = 0; i < borderThickness; i++)
            {
                DrawHorizontalLine(length, startPosY + i, startPosX);
                DrawHorizontalLine(length, startPosY + height - i - 1, startPosX);
                DrawVerticalLine(height, startPosY, startPosX + i);
                DrawVerticalLine(height, startPosY, startPosX + length - i - 1);
            }
        }

        private void DrawRectangle(int length, int height, int startPosX, int startPosY) //Draw rectangle from given position
        {
            for (int i = 0; i < height; i++)
            {
                DrawVerticalLine(length, startPosY, startPosX + i);
            }
        }

        private void WriteSpace() //Moves current cursor position by 3 pixels to the right
        {
            CursorPosX += 3;
        }

        public void NewLine() //Moves current cursir oisutuion to new line
        {
            CursorPosX = 1;
            CursorPosY += 15;
        }

        private void WriteZero() //Writes '0' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 1);
            DrawVerticalLine(6, CursorPosY + 1, CursorPosX);
            DrawVerticalLine(6, CursorPosY + 1, CursorPosX + 4);
            CursorPosX += 7;
        }

        private void WriteOne() //Writes '1' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(2, CursorPosY + 1, CursorPosX);
            DrawHorizontalLine(5, CursorPosY + 7, CursorPosX);
            DrawVerticalLine(7, CursorPosY, CursorPosX + 2);
            CursorPosX += 7;
        }

        private void WriteTwo() //Writes '2' to PixelArray starting from current cursor position
        {
            DrawPixel(CursorPosY + 1, CursorPosX);
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawVerticalLine(3, CursorPosY + 1, CursorPosX + 4);
            DrawPixel(CursorPosY + 4, CursorPosX + 3);
            DrawPixel(CursorPosY + 5, CursorPosX + 2);
            DrawPixel(CursorPosY + 6, CursorPosX + 1);
            DrawHorizontalLine(5, CursorPosY + 7, CursorPosX);
            CursorPosX += 7;
        }

        private void WriteThree() //Writes '3' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(5, CursorPosY, CursorPosX);
            DrawHorizontalLine(5, CursorPosY + 3, CursorPosX);
            DrawHorizontalLine(5, CursorPosY + 7, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX + 4);
            DrawVerticalLine(3, CursorPosY + 4, CursorPosX + 4);
            CursorPosX += 7;
        }

        private void WriteFour() //Writes '4' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(5, CursorPosY + 5, CursorPosX);
            DrawVerticalLine(8, CursorPosY, CursorPosX + 3);
            DrawPixel(CursorPosY + 1, CursorPosX + 2);
            DrawPixel(CursorPosY + 2, CursorPosX + 1);
            DrawPixel(CursorPosY + 3, CursorPosX);
            DrawPixel(CursorPosY + 4, CursorPosX);
            CursorPosX += 7;
        }

        private void WriteFive() //Writes '5' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(5, CursorPosY, CursorPosX);
            DrawHorizontalLine(4, CursorPosY + 3, CursorPosX);
            DrawHorizontalLine(4, CursorPosY + 7, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX);
            DrawVerticalLine(3, CursorPosY + 4, CursorPosX + 4);
            CursorPosX += 7;
        }

        private void WriteSix() //Writes '6' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 3, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 1);
            DrawVerticalLine(6, CursorPosY + 1, CursorPosX);
            DrawVerticalLine(3, CursorPosY + 4, CursorPosX + 4);
            DrawPixel(CursorPosY + 1, CursorPosX + 4);
            CursorPosX += 7;
        }

        private void WriteSeven() //Writes '7' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(5, CursorPosY, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX + 4);
            DrawVerticalLine(2, CursorPosY + 4, CursorPosX + 2);
            DrawVerticalLine(2, CursorPosY + 6, CursorPosX + 1);
            DrawPixel(CursorPosY + 3, CursorPosX + 3);
            CursorPosX += 7;
        }

        private void WriteEight() //Writes '8' to PixelArray starting from current cursor position
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

        private void WriteNine() //Writes '9' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 3, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 1);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX);
            DrawVerticalLine(6, CursorPosY + 1, CursorPosX + 4);
            DrawPixel(CursorPosY + 6, CursorPosX);
            CursorPosX += 7;
        }

        private void WriteDegreeSymbol() //Writes degree symbol to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(2, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(2, CursorPosY + 3, CursorPosX + 1);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX + 3);
            CursorPosX += 5;
        }

        private void WriteA() //Writes 'A' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 3, CursorPosX + 1);
            DrawVerticalLine(7, CursorPosY + 1, CursorPosX);
            DrawVerticalLine(7, CursorPosY + 1, CursorPosX + 4);
            CursorPosX += 7;
        }

        private void WriteB() //Writes 'B' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 3, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 1);
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX + 4);
            DrawVerticalLine(3, CursorPosY + 4, CursorPosX + 4);
            CursorPosX += 7;
        }

        private void WriteC() //Writes 'C' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 2);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 2);
            DrawVerticalLine(4, CursorPosY + 2, CursorPosX);
            DrawPixel(CursorPosY + 1, CursorPosX + 1);
            DrawPixel(CursorPosY + 6, CursorPosX + 1);
            DrawPixel(CursorPosY + 1, CursorPosX + 5);
            DrawPixel(CursorPosY + 6, CursorPosX + 5);
            CursorPosX += 8;
        }

        private void WriteD() //Writes 'D' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 1);
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            DrawVerticalLine(4, CursorPosY + 2, CursorPosX + 5);
            DrawPixel(CursorPosY + 1, CursorPosX + 4);
            DrawPixel(CursorPosY + 6, CursorPosX + 4);
            CursorPosX += 8;
        }

        private void WriteE() //Writes 'E' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(4, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 3, CursorPosX + 1);
            DrawHorizontalLine(4, CursorPosY + 7, CursorPosX + 1);
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            CursorPosX += 7;
        }

        private void WriteF() //Writes 'F' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(4, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 3, CursorPosX + 1);
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            CursorPosX += 7;
        }

        private void WriteG() //Writes 'G' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 2);
            DrawHorizontalLine(3, CursorPosY + 4, CursorPosX + 3);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 2);
            DrawVerticalLine(4, CursorPosY + 2, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 5, CursorPosX + 5);
            DrawPixel(CursorPosY + 1, CursorPosX + 1);
            DrawPixel(CursorPosY + 6, CursorPosX + 1);
            DrawPixel(CursorPosY + 1, CursorPosX + 5);
            CursorPosX += 8;
        }

        private void WriteH() //Writes 'H' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(4, CursorPosY + 3, CursorPosX + 1);
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            DrawVerticalLine(8, CursorPosY, CursorPosX + 5);
            CursorPosX += 8;
        }

        private void WriteI() //Writes 'I' to PixelArray starting from current cursor position
        {
            DrawVerticalLine(8, CursorPosY, CursorPosX + 2);
            CursorPosX += 5;
        }

        private void WriteJ() //Writes 'J' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(2, CursorPosY + 7, CursorPosX);
            DrawVerticalLine(7, CursorPosY, CursorPosX + 2);
            CursorPosX += 5;
        }

        private void WriteK() //Writes 'K' to PixelArray starting from current cursor position
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

        private void WriteL() //Writes 'L' to PixelArray starting from current cursor position
        {
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 1);
            CursorPosX += 6;
        }

        private void WriteM() //Writes 'M' to PixelArray starting from current cursor position
        {
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            DrawVerticalLine(2, CursorPosY, CursorPosX + 1);
            DrawVerticalLine(2, CursorPosY + 2, CursorPosX + 2);
            DrawVerticalLine(2, CursorPosY + 4, CursorPosX + 3);
            DrawVerticalLine(2, CursorPosY + 6, CursorPosX + 4);
            DrawVerticalLine(2, CursorPosY + 4, CursorPosX + 5);
            DrawVerticalLine(2, CursorPosY + 2, CursorPosX + 6);
            DrawVerticalLine(2, CursorPosY, CursorPosX + 7);
            DrawVerticalLine(8, CursorPosY, CursorPosX + 8);
            CursorPosX += 11;
        }

        private void WriteN() //Writes 'N' to PixelArray starting from current cursor position
        {
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            DrawVerticalLine(2, CursorPosY, CursorPosX + 1);
            DrawVerticalLine(2, CursorPosY + 2, CursorPosX + 2);
            DrawVerticalLine(2, CursorPosY + 4, CursorPosX + 3);
            DrawVerticalLine(2, CursorPosY + 6, CursorPosX + 4);
            DrawVerticalLine(8, CursorPosY, CursorPosX + 5);
            CursorPosX += 8;
        }

        private void WriteO() //Writes 'O' to PixelArray starting from current cursor position
        {
            DrawVerticalLine(4, CursorPosY + 2, CursorPosX);
            DrawVerticalLine(4, CursorPosY + 2, CursorPosX + 6);
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 2);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 2);
            DrawPixel(CursorPosY + 1, CursorPosX + 1);
            DrawPixel(CursorPosY + 1, CursorPosX + 5);
            DrawPixel(CursorPosY + 6, CursorPosX + 1);
            DrawPixel(CursorPosY + 6, CursorPosX + 5);
            CursorPosX += 9;
        }

        private void WriteP() //Writes 'P' to PixelArray starting from current cursor position
        {
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 4, CursorPosX + 1);
            DrawVerticalLine(3, CursorPosY + 1, CursorPosX + 4);
            CursorPosX += 7;
        }

        private void WriteQ() //Writes 'Q' to PixelArray starting from current cursor position
        {
            DrawVerticalLine(4, CursorPosY + 2, CursorPosX);
            DrawVerticalLine(4, CursorPosY + 2, CursorPosX + 6);
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 2);
            DrawHorizontalLine(5, CursorPosY + 7, CursorPosX + 2);
            DrawPixel(CursorPosY + 1, CursorPosX + 1);
            DrawPixel(CursorPosY + 1, CursorPosX + 5);
            DrawPixel(CursorPosY + 6, CursorPosX + 1);
            DrawPixel(CursorPosY + 6, CursorPosX + 5);
            DrawPixel(CursorPosY + 8, CursorPosX + 7);
            CursorPosX += 10;
        }

        private void WriteR() //Writes 'R' to PixelArray starting from current cursor position
        {
            DrawVerticalLine(8, CursorPosY, CursorPosX);
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 4, CursorPosX + 1);
            DrawVerticalLine(3, CursorPosY + 1, CursorPosX + 4);
            DrawVerticalLine(2, CursorPosY + 6, CursorPosX + 4);
            DrawPixel(CursorPosY + 5, CursorPosX + 3);
            CursorPosX += 7;
        }

        private void WriteS() //Writes 'S' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 3, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX + 1);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX);
            DrawVerticalLine(3, CursorPosY + 4, CursorPosX + 4);
            DrawPixel(CursorPosY + 1, CursorPosX + 4);
            DrawPixel(CursorPosY + 6, CursorPosX);
            CursorPosX += 7;
        }

        private void WriteT() //Writes 'T' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(5, CursorPosY, CursorPosX + 1);
            DrawVerticalLine(7, CursorPosY + 1, CursorPosX + 3);
            CursorPosX += 8;
        }

        private void WriteU() //Writes 'U' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(4, CursorPosY + 7, CursorPosX + 1);
            DrawVerticalLine(7, CursorPosY, CursorPosX);
            DrawVerticalLine(7, CursorPosY, CursorPosX + 5);
            CursorPosX += 7;
        }

        private void WriteW() //Writes 'W' to PixelArray starting from current cursor position
        {
            DrawVerticalLine(3, CursorPosY, CursorPosX);
            DrawVerticalLine(3, CursorPosY + 3, CursorPosX + 1);
            DrawVerticalLine(2, CursorPosY + 6, CursorPosX + 2);
            DrawVerticalLine(3, CursorPosY + 3, CursorPosX + 3);
            DrawVerticalLine(3, CursorPosY, CursorPosX + 4);
            DrawVerticalLine(3, CursorPosY + 3, CursorPosX + 5);
            DrawVerticalLine(2, CursorPosY + 6, CursorPosX + 6);
            DrawVerticalLine(3, CursorPosY + 3, CursorPosX + 7);
            DrawVerticalLine(3, CursorPosY, CursorPosX + 8);
            CursorPosX += 11;
        }

        private void WriteV() //Writes 'V' to PixelArray starting from current cursor position
        {
            DrawVerticalLine(3, CursorPosY, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 3, CursorPosX + 1);
            DrawVerticalLine(2, CursorPosY + 5, CursorPosX + 2);
            DrawPixel(CursorPosY + 7, CursorPosX + 3);
            DrawVerticalLine(2, CursorPosY + 5, CursorPosX + 4);
            DrawVerticalLine(2, CursorPosY + 3, CursorPosX + 5);
            DrawVerticalLine(3, CursorPosY, CursorPosX + 6);
            CursorPosX += 9;
        }

        private void WriteX() //Writes 'X' to PixelArray starting from current cursor position
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
            CursorPosX += 7;
        }

        private void WriteY() //Writes 'Y' to PixelArray starting from current cursor position
        {
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX + 1);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX + 3);
            DrawVerticalLine(5, CursorPosY + 3, CursorPosX + 2);
            DrawPixel(CursorPosY, CursorPosX);
            DrawPixel(CursorPosY, CursorPosX + 4);
            CursorPosX += 7;
        }

        private void WriteZ() //Writes 'Z' to PixelArray starting from current cursor position
        {
            DrawHorizontalLine(6, CursorPosY, CursorPosX);
            DrawHorizontalLine(6, CursorPosY + 7, CursorPosX);
            DrawPixel(CursorPosY + 6, CursorPosX);
            DrawPixel(CursorPosY + 5, CursorPosX + 1);
            DrawPixel(CursorPosY + 4, CursorPosX + 2);
            DrawPixel(CursorPosY + 3, CursorPosX + 3);
            DrawPixel(CursorPosY + 2, CursorPosX + 4);
            DrawPixel(CursorPosY + 1, CursorPosX + 5);
            CursorPosX += 8;
        }

        private void WriteDot() //Writes '.' to PixelArray starting from current cursor position
        {
            DrawPixel(CursorPosY + 7, CursorPosX);
            DrawPixel(CursorPosY + 6, CursorPosX);
            DrawPixel(CursorPosY + 7, CursorPosX + 1);
            DrawPixel(CursorPosY + 6, CursorPosX + 1);
            CursorPosX += 4;
        }

        private void WriteComma() //Writes ',' to PixelArray starting from current cursor position
        {
            DrawPixel(CursorPosY + 7, CursorPosX);
            DrawPixel(CursorPosY + 6, CursorPosX);
            DrawPixel(CursorPosY + 7, CursorPosX + 1);
            DrawPixel(CursorPosY + 6, CursorPosX + 1);
            DrawPixel(CursorPosY + 8, CursorPosX + 1);
            DrawPixel(CursorPosY + 9, CursorPosX);
            CursorPosX += 4;
        }

        private void WriteSemicolon() //Writes ';' to PixelArray starting from current cursor position
        {
            DrawPixel(CursorPosY + 2, CursorPosX);
            DrawPixel(CursorPosY + 3, CursorPosX);
            DrawPixel(CursorPosY + 2, CursorPosX + 1);
            DrawPixel(CursorPosY + 3, CursorPosX + 1);

            DrawPixel(CursorPosY + 7, CursorPosX);
            DrawPixel(CursorPosY + 6, CursorPosX);
            DrawPixel(CursorPosY + 7, CursorPosX + 1);
            DrawPixel(CursorPosY + 6, CursorPosX + 1);
            DrawPixel(CursorPosY + 8, CursorPosX + 1);
            DrawPixel(CursorPosY + 9, CursorPosX);
            CursorPosX += 4;
        }

        private void WriteColon() //Writes ':' to PixelArray starting from current cursor position
        {
            DrawPixel(CursorPosY + 2, CursorPosX);
            DrawPixel(CursorPosY + 3, CursorPosX);
            DrawPixel(CursorPosY + 2, CursorPosX + 1);
            DrawPixel(CursorPosY + 3, CursorPosX + 1);

            DrawPixel(CursorPosY + 7, CursorPosX);
            DrawPixel(CursorPosY + 6, CursorPosX);
            DrawPixel(CursorPosY + 7, CursorPosX + 1);
            DrawPixel(CursorPosY + 6, CursorPosX + 1);
            CursorPosX += 4;
        }

        private void ErrorSymbol() //Writes error symbol to PixelArray on predefined position
        {
            DrawVerticalLine(2, 111, 9);
            DrawVerticalLine(2, 113, 8);
            DrawVerticalLine(2, 113, 10);
            DrawVerticalLine(2, 115, 7);
            DrawVerticalLine(2, 115, 11);
            DrawVerticalLine(2, 117, 6);
            DrawVerticalLine(2, 117, 12);
            DrawVerticalLine(2, 119, 5);
            DrawVerticalLine(2, 119, 13);
            DrawVerticalLine(2, 121, 4);
            DrawVerticalLine(2, 121, 14);
            DrawHorizontalLine(11, 123, 4);
            DrawPixel(121, 9);
            DrawVerticalLine(4, 116, 9);
        }

        public void ErrorSignal(string message) //Writes massage with error symbol to PixelArray on predefined position
        {
            int LastPosX = CursorPosX;
            int LastPosY = CursorPosY;
            ErrorSymbol();
            SetCurrentPosition(16, 114);
            WriteOnScreen(message);
            SetCurrentPosition(LastPosX, LastPosY);
        }

        public void InitInterface() //Initializes standard interface for application
        {
            InitPixelArray();
            DrawFrame(126, 126, 2, 1, 1);
            SetCurrentPosition(10, 4);
            WriteOnScreen("WEATHER  STATION");
            SetCurrentPosition(8, 14);
            WriteOnScreen("ON  RASPBERRY  PI  2");
            DrawHorizontalLine(122, 23, 3);
            DrawHorizontalLine(122, 24, 3);
            SetCurrentPosition(16, 32);
            WriteOnScreen("CURRENT  STATE:");
        }
    }
}