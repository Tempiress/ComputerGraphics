using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Brezenhaim_And_Wu
{
    public partial class Form1 : Form
    {

        private Bitmap bmp;

        public Form1()
        {
            InitializeComponent();

            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            //pictureBox1.Image = bmp;        
        }


        


        public void BresenhaimLine(int x0, int y0, int x1, int y1, Color color, PictureBox picturebox)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int error = dx - dy;

            while (true)
            {

                bmp.SetPixel(x0, y0, color);
                if (x0 == x1 && y0 == y1) break;

                int e2 = 2 * error;
                if (e2 >= -dy)
                {
                    error -= dy;
                    x0 += sx;
                }
                if (e2 <= dx) 
                {
                    error += dx;
                    y0 += sy;
                }
            }

            picturebox.Image = bmp;
            picturebox.Invalidate();
            picturebox.Refresh();

        }


        private void swap(ref int a, ref int b) 
        {
            int t = a;
            a = b;
            b = t;
        }

        private float fpart(float x) 
        {
            return x - (float)Math.Floor(x);//(float)(x - Math.Truncate(x));
        }

        private int ipart(float x) 
        {
            return (int)Math.Floor(x);  //(int)Math.Truncate(x);
        }


        public static float Clamp(float value, float min, float max)
        {
            if (min > max)
            {
                throw new Exception("Max < Min!");
            }

            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }

            return value;
        }


        private void plot(Bitmap btp, int x, int y, float brightness, Color color) 
        {
            Color newColor = Color.FromArgb((int)(color.A * Clamp(brightness, 0f, 1f)), color.R, color.G, color.B);
            btp.SetPixel(x, y, newColor);
        }


        public void WuLine(int x1, int y1, int x2, int y2, Color color, PictureBox picturebox) 
        {
            
            if (x1 == x2 && y1 == y2) 
            {
                plot(bmp, x1, y1, 1, Color.Black);
                return;
            }
           

            bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);


            if ( steep)
            {
                swap(ref x1, ref y1);
                swap(ref x2, ref y2);
            }

            if (x1 > x2) 
            {
                swap(ref x1, ref x2);
                swap(ref y1, ref y2);
            }



            int dx = x2 - x1;
            int dy = y2 - y1;
            float gradient = (float)dy / (float)dx;

            //Рисуем первую точку
            int xend = x1;
            float yend = y1 + gradient * (xend - x1);
            float xgap = 1 - fpart((float)(x1 + 0.5f));
            int xpxl1 = xend;
            int ypxl1 = ipart(yend);


            plot(bmp, xpxl1, ypxl1, (1 - fpart(yend)) * xgap, color);
            plot(bmp, xpxl1, ypxl1 + 1, fpart(yend) * xgap, color);


            var intery = yend + gradient;

            //Рисуем последнюю точку
            xend = x2;
            yend = y2 + gradient * (xend - x2);
            xgap = fpart(x2 + 0.5f);
            int xpxl2 = xend;
            int ypxl2 = ipart(yend);
            plot(bmp, xpxl2, ypxl2, (1 - fpart(yend)) * xgap, color);
            plot(bmp, xpxl2, ypxl2 + 1, fpart(yend) * xgap, color);


            if (dx == 0) 
            {
                // Линия вертикальная
                for (int y = y1; y <= y2; y++) 
                {

                    plot(bmp, x1, y, 1.0f, color); 
                }
                return;
            }

            //Основной цикл
            for (int x = xpxl1 + 1; x <= xpxl2 - 1; x++) 
            {



                plot(bmp, x, ipart(intery), 1 - fpart(intery), color);
                plot(bmp, x, ipart(intery) + 1, fpart(intery) , color);
                intery += gradient;
            }

            pictureBox1.Image = bmp;
            pictureBox1.Invalidate();
            pictureBox1.Refresh();
        }


        private void DrawButton1_Click(object sender, EventArgs e)
        {
            BresenhaimLine(10, 10, 40, 100, Color.Black, this.pictureBox1);
        }

        private void DrawButton2_Click(object sender, EventArgs e)
        {
            WuLine(200, 200, 10, 10, Color.Black, pictureBox1);
            WuLine(10, 10, 110, 200, Color.Black, pictureBox1);
            WuLine(300, 100, 160, 150, Color.Black, pictureBox1);
            WuLine(10, 10, 140, 10, Color.Black, pictureBox1);
            //WuLine(0,0,0,0, Color.Black, pictureBox1);
        }


        private static Bitmap resizeBitmap(Bitmap sourceBMP, int width, int height) 
        {
            Bitmap result = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(result);
            g.DrawImage(sourceBMP, 0, 0, width, height);
            return result;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            pictureBox1.Image = resizeBitmap(bmp, bmp.Width * (trackBar1.Value + 1), bmp.Height * (trackBar1.Value + 1));
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            pictureBox1.Invalidate();
            pictureBox1.Refresh();            
        }
    }




}
