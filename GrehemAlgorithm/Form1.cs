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
        List<int> P;
        List<int> po = new List<int>();
       
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

                //if (hull.Count > 1) 
                //{
                //    e.Graphics.DrawPolygon(Pens.Red, hull.ToArray());
                //}
                if (po.Count > 2) 
                {
                    for (int k = 0; k < po.Count - 1; k++)
                    {
                        e.Graphics.DrawLine(Pens.Red, points[po[k]].X, points[po[k]].Y, points[po[k + 1]].X, points[po[k + 1]].Y);
                    }
                    e.Graphics.DrawLine(Pens.Red, points[po[0]].X, points[po[0]].Y, points[po[po.Count - 1]].X, points[po[po.Count - 1]].Y);
                   
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
        private void Grahamscan()
        {
            if (points.Count < 3) return; // Оболочка невозможна, если точек меньше 3

            int n = points.Count;
            P = Enumerable.Range(0, n).ToList();

            for (int i = 0; i < n; i++) 
            {
                if (points[P[i]].X < points[P[0]].X) 
                {
                    (P[i], P[0]) = (P[0], P[i]);
                }
            }


            for (int i = 1; i < n; i++) 
            {
                int j = i;
                while (j > 1 && (rotate(points[P[0]], points[P[j - 1]], points[P[j]] ) < 0)) 
                {
                    (P[j], P[j - 1]) = (P[j - 1], P[j]);
                    j -= 1;
                }
            }
            po.Clear();
            po.Add(P[0]);
            po.Add(P[1]);
        
            for (int q = 1; q < n; q++) 
            {
                while (rotate(points[po[po.Count - 2]], points[po[po.Count - 1]], points[P[q]]) < 0) 
                {
                    po.RemoveAt(po.Count - 1);
                }
                po.Add(P[q]);

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

            Grahamscan(); // Запуск алгоритма
           
        }
    }
}
