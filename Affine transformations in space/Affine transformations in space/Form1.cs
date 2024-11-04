using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Affine_transformations_in_space
{
    public partial class Form1 : Form
    {

        polyhedron pop;
        public static int scale = 100;
        
        public static double focalLength = 200; //Глубина
        public Form1()
        {
            InitializeComponent();
               
        }


        public void DrawTetrahedron()
        {
            pop =  polyhedron.drawGexaedr();
            pictureBox1.Invalidate();
        
        }


        public class point 
        {
            public double X, Y, Z;

            public point(double x, double y, double z) 
            {
                X = x;
                Y = y;
                Z = z;
            }

        }
        public class polygon
        {
            public List<point> Vertices;

            public polygon(List<point> vertices) 
            {
                Vertices = vertices;
            }
        }


        public class polyhedron
        {
            public List<point> Verticles;
            public List<polygon> Faces;

            public polyhedron(List<point> verticles , List<polygon> faces)
            {
                Verticles = verticles;
                Faces = faces;
            }

            public static polyhedron drawTetraedr()
            {       
                point v1 = new point(1 * scale, 1 * scale, 1 * scale);
                point v2 = new point(1 * scale, -1 * scale, -1 * scale);
                point v3 = new point(-1 * scale, 1 * scale, -1 * scale);
                point v4 = new point(-1 * scale, -1 * scale, 1 * scale) ;

                polygon firstPol = new polygon(new List<point> { v1, v2, v3 });
                polygon secondPol = new polygon(new List<point> { v1, v2, v4 });
                polygon thirdPol = new polygon(new List<point> {v1, v3, v4 });
                polygon fourthPol = new polygon(new List<point> {v2, v3, v4 });

                return new polyhedron(new List<point> { v1, v2, v3, v4 }, new List<polygon> {firstPol, secondPol, thirdPol, fourthPol });             
            }

            public static polyhedron drawGexaedr() 
            {
                point v1 = new point(-1 * scale, -1 * scale, -1 * scale);
                point v2 = new point(1 * scale, -1 * scale, -1 * scale);
                point v3 = new point(-1 * scale, 1 * scale, -1 * scale);
                point v4 = new point(1 * scale, 1 * scale, -1 * scale);              
                point v5 = new point(-1 * scale, -1 * scale,  1 * scale);
                point v6 = new point(1 * scale, -1 * scale, 1 * scale);
                point v7 = new point(-1 * scale, 1 * scale, 1 * scale);
                point v8 = new point(1 * scale, 1 * scale, 1 * scale);

                polygon firstPol = new polygon(new List<point> { v1, v2, v4, v3 }); // Нижняя грань
                polygon secondPol = new polygon(new List<point> { v5, v6, v8, v7 }); // Верхняя грань
                polygon thirdPol = new polygon(new List<point> { v1, v3, v7, v5 }); // Левая грань
                polygon fourdPol = new polygon(new List<point> { v2, v4, v8, v6 }); // Правая грань
                polygon fivethPol = new polygon(new List<point> { v1, v2, v6, v5 }); // Передняя грань
                polygon sixthPol = new polygon(new List<point> { v3, v4, v8, v7 });  // Задняя грань

                return new polyhedron(new List<point> { v1, v2, v3, v4, v5, v6, v7, v8 }, new List<polygon> {firstPol, secondPol, thirdPol, fourdPol, fivethPol, sixthPol });
            }
        }

        //перспективная проекция
        private Point PointTo2D(point p) 
        {
            int x2D = (int)(focalLength * p.X / (p.Z + focalLength));
            int y2D = (int)(focalLength * p.Y / (p.Z + focalLength));

            return new Point(x2D, y2D);
        }

        //Изометрическая проекция
        private Point Isometric2DPoint(point p ) 
        {
            double angleCos = Math.Cos(Math.PI / 6);
            double angleSin = Math.Sin(Math.PI / 6);

            int x2D = (int)((p.X - p.Z) * angleCos);
            int y2D = (int)((p.Y - (p.X + p.Z) * angleSin) * -1);

            x2D += pictureBox1.Width / 2;
            y2D += pictureBox1.Height / 2;

            return new Point(x2D,y2D);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if(pop != null) 
            {
               
                for (int i = 0; i < pop.Faces.Count(); i++)
                {
                  
                    Point[] points2D = pop.Faces[i].Vertices.Select(v => Isometric2DPoint(v)).ToArray();
                    e.Graphics.DrawPolygon(Pens.Black, points2D);                   
                }
            }            
            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DrawTetrahedron();
        }
    }
}
