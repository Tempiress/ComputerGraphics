using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GrehemAlgorithm
{
    public partial class Form1 : Form
    {
        List<Point> points  = new List<Point>();
        List<Point> hull = new List<Point>(); //Точки выпуклой оболочки
        
        public Form1()
        {
            InitializeComponent();

            pictureBox1.MouseClick += drawpoints;
            pictureBox1.Paint += PictureBox1_Paint;

        }


        private void PictureBox1_Paint(object sender, PaintEventArgs e) 
        {
            if (points.Count != 0) 
            {
                Brush brush = new SolidBrush(Color.Black);


                for (int i = 0; i < points.Count; i++)
                {
                    e.Graphics.FillRectangle(brush, points[i].X, points[i].Y, 4, 4);
                }

                if (hull.Count > 1) 
                {
                    e.Graphics.DrawPolygon(Pens.Red, hull.ToArray());
                }

            }


        }


        private void drawpoints(object sender, MouseEventArgs e) 
        {
            
            points.Add(e.Location);
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (points.Count != 0) 
            {
                Brush brush = new SolidBrush(Color.Black);
                for (int i = 0; i < points.Count; i++) 
                {
                    e.Graphics.FillRectangle(brush, points[i].X, points[i].Y, 2, 2);
                }                            
                
            }

        }

        private void SortByPolarAngle(List<Point> points, Point minPoint) 
        {
            for (int i = 0; i < points.Count; i++) 
            {
                int minIndex = i;
                double minAngle = Math.Atan2(points[i].Y - minPoint.Y, points[i].X - minPoint.X);

                for (int j = i + 1; j < points.Count; j++) 
                {
                    double angle = Math.Atan2(points[j].Y - minPoint.Y, points[j].X - minPoint.X);

                    if (angle < minAngle) 
                    {
                        minIndex = j;
                        minAngle = angle;
                    }
                }

                Point temp = points[i];
                points[i] = points[minIndex];
                points[minIndex] = temp;
            }
        }

        private double rotate(Point A, Point B, Point C)
        {
            return (B.X - A.X) * (C.Y - B.Y) - (B.Y - A.Y) * (C.X - B.X);
        }

        // Сканирование методом Грэхема
        private void Grahamscan(List<Point> pnts)
        {
            if (pnts.Count < 3) return; // Оболочка невозможна, если точек меньше 3

            // Находим точку с минимальной координатой Y (если несколько, то с минимальной X)
            Point minPoint = pnts.Aggregate((min, p) => p.Y < min.Y || (p.Y == min.Y && p.X < min.X) ? p : min);
            pnts.Remove(minPoint); // Убираем эту точку из списка для сортировки

            // Сортировка точек по полярному углу относительно минимальной точки
            SortByPolarAngle(pnts, minPoint);


            // Добавляем минимальную точку в выпуклую оболочку и первую отсортированную точку
            hull.Clear();
            hull.Add(minPoint);
            hull.Add(pnts[0]);

            // Строим выпуклую оболочку
            for (int i = 1; i < pnts.Count; i++)
            {
                while (hull.Count >= 2 && rotate(hull[hull.Count - 2], hull[hull.Count - 1], pnts[i]) <= 0)
                {
                    hull.RemoveAt(hull.Count - 1); // Удаляем последнюю точку, если поворот не против часовой
                }
                hull.Add(pnts[i]);
            }

            pictureBox1.Invalidate(); // Перерисовываем с новой оболочкой
        }


        // Метод для запуска алгоритма при нажатии кнопки
        private void button1_Click(object sender, EventArgs e)
        {
            if (points.Count < 3)
            {
                MessageBox.Show("Необходимо добавить как минимум 3 точки.");
                return;
            }

            Grahamscan(new List<Point>(points)); // Запуск алгоритма
        }
    }
}
