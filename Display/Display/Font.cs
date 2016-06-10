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

        Pixel[,] pixelArray = new Pixel[128, 128];
        int screenHeight = 128;
        int screenWidth = 128;

        public Font(int margin = 5) 
        {
            DisplayMargin = margin;
            CursorPosY = margin+1;
            CursorPosX = margin+1;
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

        private void DrawHorizontalLine(int length, int startPosX, int startPosY, byte red = 0, byte green = 0, byte blue = 0) //Draw horizontal line from given position
        {
            for (int i = 0; i < length; i++) {
                DrawPixel(startPosY, startPosX + i, red, green, blue);
            }
        }

        private void DrawVerticalLine(int length, int startPosX, int startPosY, byte red= 0, byte green = 0, byte blue = 0) //Draw vertical line from given position
        {
            for (int i = 0; i < length; i++)
            {
                DrawPixel(startPosY + i, startPosX, red, green, blue);
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
        }

        private void WriteSeven() //Writes '7' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(5, CursorPosY, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX + 4);
            DrawVerticalLine(2, CursorPosY + 4, CursorPosX + 2);
            DrawVerticalLine(2, CursorPosY + 6, CursorPosX + 1);
            DrawPixel(CursorPosY + 3, CursorPosX + 3);
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
        }

        private void WriteNine() //Writes '9' to PixelArray string starting from current cursor position
        {
            DrawHorizontalLine(3, CursorPosY, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 3, CursorPosX + 1);
            DrawHorizontalLine(3, CursorPosY + 7, CursorPosX);
            DrawVerticalLine(2, CursorPosY + 1, CursorPosX);
            DrawVerticalLine(6, CursorPosY + 1, CursorPosX + 4);
            DrawPixel(CursorPosY + 6, CursorPosX);
        }
    }
}
