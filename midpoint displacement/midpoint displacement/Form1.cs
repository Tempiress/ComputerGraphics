using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace midpoint_displacement
{
    public partial class Form1 : Form
    {
        List<Point> polygonPoints = new List<Point>();
        Queue<Tuple<Point, int>> pointQueue = new Queue<Tuple<Point, int>>();
        double roughness;
        double hX;
        double hY;
        List<Point> initPoints = new List<Point>();
        bool End = false;
        Image backgroundImage;

        public Form1()
        {
            InitializeComponent();
            pictureBox1.Paint += new PaintEventHandler(PictureBox1_Paint);
            pictureBox1.MouseClick += new MouseEventHandler(pictureBox1_Click);
            backgroundImage = Image.FromFile("sky.jpg");
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            midpointAlg();
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e) 
        {
            if (polygonPoints.Count > 1) 
            {   

                e.Graphics.DrawLines(Pens.Black, polygonPoints.ToArray());

            }


            if (End == false)
                return;


            List<Point> closedPolygonpoints = new List<Point>(polygonPoints);
            closedPolygonpoints.Add(new Point(closedPolygonpoints.Last().X, pictureBox1.Height));
            closedPolygonpoints.Insert(0, new Point(closedPolygonpoints.First().X, pictureBox1.Height));
            e.Graphics.FillPolygon(Brushes.Black, closedPolygonpoints.ToArray());

            // Ограничиваем картинку "неба" ломаной
            Region region = new Region(new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
            region.Exclude(new Region(new System.Drawing.Drawing2D.GraphicsPath(closedPolygonpoints.ToArray(), Enumerable.Repeat((byte)1, closedPolygonpoints.Count).ToArray())));

            e.Graphics.SetClip(region, System.Drawing.Drawing2D.CombineMode.Replace);

            // Отрисовка изображения неба
            if (backgroundImage != null)
            {
                e.Graphics.DrawImage(backgroundImage, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
            }

            //e.Graphics.DrawPolygon(Pens.Black, polygonPoints.ToArray());
            //g.DrawLine(Pens.Black, polygonPoints polygonPoints.ToArray());
        }


        private void midpointAlg() 
        {
            if (pointQueue.Count == 0)
                return;
            
            var rand = new Random();
            roughness = 0.09;
            Tuple<Point, int> curT = pointQueue.Dequeue();

            if (curT.Item2 == 1) 
            {
                hX = (initPoints[0].X + curT.Item1.X) / 2;
                hY = (initPoints[0].Y + curT.Item1.Y) / 2;
            }
            if (curT.Item2 == 0) 
            {
                hX = (initPoints[1].X + curT.Item1.X) / 2;
                hY = (initPoints[1].Y + curT.Item1.Y) / 2;
            }
            
            //Вычисление длины отрезка
            int l = (int)(Math.Sqrt(Math.Pow(polygonPoints[1].X - polygonPoints[0].X, 2) + Math.Pow(polygonPoints[1].Y - polygonPoints[0].Y, 2)));

            //Сглаживание высоты на каждой итерации
            double amplitude = roughness * l;
            double newH = hY - rand.Next((int)(-roughness * l), (int)(roughness * l));

            //Добавляем новую точку
            polygonPoints.Add(new Point((int)hX, (int)newH));

            roughness *= 0.6;

            pointQueue.Enqueue(new Tuple<Point, int>(new Point( (int)hX, (int)newH), 0 ));
            pointQueue.Enqueue(new Tuple<Point, int>(new Point((int)hX, (int)newH), 1));

            polygonPoints.Sort((p1, p2) => 
            {
                int res = p1.X.CompareTo(p2.X);
                if (res == 0) 
                {
                    res = p1.Y.CompareTo(p2.Y); 
                }
                return res;
            });
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {
            if (polygonPoints.Count > 1) return;

            polygonPoints.Add(e.Location);
            initPoints.Add(e.Location);

            if (polygonPoints.Count == 2)
            {
                if (polygonPoints[0].X < polygonPoints[1].X)
                    pointQueue.Enqueue(new Tuple<Point, int>((polygonPoints[1]), 1));
                else
                    pointQueue.Enqueue(new Tuple<Point, int>((polygonPoints[0]), 1));
            }

            pictureBox1.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            End = true;
            pictureBox1.Invalidate();
        }
    }
}
