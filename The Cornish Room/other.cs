//Face.cs
using System.Collections.Generic;

namespace Lab7
{
    public class Face
    {
        public List<Vertex> Vertices { get; private set; }
        public Face(List<Vertex> vertices)
        {
            Vertices = vertices;
        }
    }
}

//fileaction.cs
using Lab7;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab7
{
    internal class fileaction
    {

        public Polyhedron LoadFromOBJ(string filePath)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<Face> faces = new List<Face>();

            foreach (var line in File.ReadLines(filePath))
            {
                if (line.StartsWith("v "))
                {
                    var parts = line.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                    double x = double.Parse(parts[1], CultureInfo.InvariantCulture);
                    double y = double.Parse(parts[2], CultureInfo.InvariantCulture);
                    double z = double.Parse(parts[3], CultureInfo.InvariantCulture);
                    vertices.Add(new Vertex(x, y, z));
                }
                else if (line.StartsWith("f "))
                {
                    var partsFace = line.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                    int i = 0;
                    List<Vertex> tempListVertex = new List<Vertex>();
                    //foreach (var p in parts) 
                    //{
                    //    vv[i] = vertices[int.Parse(p)];
                    //    i++;

                    //}
                    for (int j = 1; j < partsFace.Length; j++) 
                    {

                        tempListVertex.Add(vertices[int.Parse(partsFace[j]) - 1]);
                    }
                    faces.Add(new Face(tempListVertex));
                   // List<Face> faceVertices = parts.Skip(1).Select(index => vertices[int.Parse(index)]);
                    //                                 //.Select(index => vertices[int.Parse(index) - 1])
                    //                                 //.ToList();
                    //faces.Add(new Face(faceVertices));
                }
            }
           
            return new Polyhedron(vertices, faces);
        }

        public void SaveToOBJ(Polyhedron polyhedron, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var vertex in polyhedron.Vertices)
                {
                    writer.WriteLine($"v {vertex.X} {vertex.Y} {vertex.Z}");
                }

                foreach (var face in polyhedron.Faces)
                {
                    var indices = face.Vertices.Select(v => polyhedron.Vertices.IndexOf(v) + 1);
                    writer.WriteLine("f " + string.Join(" ", indices));
                }
            }
        }
    }
}

//private void LoadFile_Click(object sender, EventArgs e)
//{
//    OpenFileDialog openFileDialog = new OpenFileDialog();
//    if (openFileDialog.ShowDialog() == DialogResult.OK)
//    {

//        pop = LoadFromOBJ(openFileDialog.FileName);
//        pnts = pop.Faces;
//        pictureBox1.Invalidate();
//    }
//}





//    }
//}


//
//Matrix.cs
namespace Lab7
{
    public class Matrix
    {
        private readonly double[,] _data;
        private readonly int _rows;
        private readonly int _cols;

        public Matrix(double[,] data)
        {
            _data = data;
            _rows = data.GetLength(0);
            _cols = data.GetLength(1);
        }
        public double this[int x, int y] => _data[x, y];

        public static Matrix operator* (Matrix a, Matrix b)
        {
            int rows = a._rows;
            int cols = b._cols;

            var result = new double[rows, cols];

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    result[i, j] = 0;
                    for (var k = 0; k < a._cols; k++)
                    {
                        result[i, j] += a._data[i, k] * b._data[k, j];
                    }
                }
            }
            return new Matrix(result);
        }
    }
}


//polyhedron.cs
using System;
using System.Collections.Generic;

namespace Lab7
{
    public class Polyhedron
    {
        //Transformer transformer;
        Projection p = new Projection(); //Заглушка
        public Polyhedron(List<Vertex> vertices, List<Face> faces)
        {
            Vertices = vertices;
            Faces = faces;
        }
        public  List<Vertex> Vertices { get; }
        public List<Face> Faces { get; private set; }

        public Matrix LocalToWorld { get; set; } = new Matrix( new double[,]
        {
            { 1, 0, 0, 0 },
            { 0, 1, 0, 0 },
            { 0, 0, 1, 0 },
            { 0, 0, 0, 1 }
        });

        public static Polyhedron Tetrahedron()
        {
            var v1 = new Vertex(1 , 1 , 1 );
            var v2 = new Vertex(1  , -1  , -1  );
            var v3 = new Vertex(-1  , 1  , -1  );
            var v4 = new Vertex(-1  , -1  , 1  ) ;

            Face firstPol = new Face(new List<Vertex> { v1, v2, v3 });
            Face secondPol = new Face(new List<Vertex> { v1, v2, v4 });
            Face thirdPol = new Face(new List<Vertex> {v1, v3, v4 });
            Face fourthPol = new Face(new List<Vertex> {v2, v3, v4 });

            return new Polyhedron(new List<Vertex> { v1, v2, v3, v4 }, new List<Face> {firstPol, secondPol, thirdPol, fourthPol });
        }

        public static Polyhedron Hexahedron()
        {
            var v1 = new Vertex(-20 - 100, -20, -20);
            var v2 = new Vertex(20 - 100, -20, -20);
            var v3 = new Vertex(-20- 100, 20, -20);
            var v4 = new Vertex(20- 100, 20, -20);
            var v5 = new Vertex(-20- 100, -20, 20);
            var v6 = new Vertex(20- 100, -20, 20);
            var v7 = new Vertex(-20- 100, 20, 20);
            var v8 = new Vertex(20- 100, 20, 20);

            var firstPol = new Face(new List<Vertex> { v1, v2, v4, v3 }); // Нижняя грань
            var secondPol = new Face(new List<Vertex> { v5, v6, v8, v7 }); // Верхняя грань
            var thirdPol = new Face(new List<Vertex> { v1, v3, v7, v5 }); // Левая грань
            var fourdPol = new Face(new List<Vertex> { v2, v4, v8, v6 }); // Правая грань
            var fivethPol = new Face(new List<Vertex> { v1, v2, v6, v5 }); // Передняя грань
            var sixthPol = new Face(new List<Vertex> { v3, v4, v8, v7 });  // Задняя грань

            return new Polyhedron(new List<Vertex> { v1, v2, v3, v4, v5, v6, v7, v8 }, new List<Face> {firstPol, secondPol, thirdPol, fourdPol, fivethPol, sixthPol });
        }
        public static Polyhedron Octahedron()
        {
            var v1 = new Vertex(1, 0, 1);
            var v2 = new Vertex(1, 0, -1);
            var v3 = new Vertex(-1, 0, -1);
            var v4 = new Vertex(-1, 0, 1);
            var v5 = new Vertex(0, 1, 0);
            var v6 = new Vertex(0, -1, 0);

            var firstPol = new Face(new List<Vertex> { v1, v2, v5 });
            var secondPol = new Face(new List<Vertex> { v2, v3, v5 });
            var thirdPol = new Face(new List<Vertex> { v3, v4, v5 });
            var fourdPol = new Face(new List<Vertex> { v4, v1, v5 });
            var fivethPol = new Face(new List<Vertex> { v1, v2, v6 });
            var sixthPol = new Face(new List<Vertex> { v2, v3, v6 });
            var sevenPol = new Face(new List<Vertex> { v3, v4, v6 });
            var eightPol = new Face(new List<Vertex> { v4, v1, v6 });

            return new Polyhedron(new List<Vertex> { v1, v2, v3, v4, v5, v6 }, new List<Face> { firstPol, secondPol, thirdPol, fourdPol, fivethPol, sixthPol, sevenPol, eightPol });
        }

        public static Polyhedron Icosahedron()
        {
            double phi = (1 + Math.Sqrt(5)) / 2;  // Золотое сечение
            double scale = 1;  // Масштаб для вершин
            List<Vertex> vertices = new List<Vertex>
            {
                new Vertex(-1 * scale,  phi * scale, 0),
                new Vertex( 1 * scale,  phi * scale, 0 * scale),
                new Vertex(-1 * scale, -phi * scale, 0),
                new Vertex( 1 * scale, -phi * scale, 0),
                new Vertex(0, -1 * scale,  phi * scale),
                new Vertex(0,  1 * scale,  phi * scale),
                new Vertex(0, -1 * scale, -phi * scale),
                new Vertex(0,  1 * scale, -phi * scale),
                new Vertex( phi * scale, 0, -1 * scale),
                new Vertex( phi * scale, 0,  1 * scale),
                new Vertex(-phi * scale, 0, -1 * scale),
                new Vertex(-phi * scale, 0,  1 * scale)
            };


            // Определение 20 треугольных граней, каждая из которых указывает на три вершины

            List<Face> faces = new List<Face>
            {
                new Face(new List<Vertex> { vertices[0], vertices[11], vertices[5] }),
                new Face(new List<Vertex> { vertices[0], vertices[5], vertices[1] }),
                new Face(new List<Vertex> { vertices[0], vertices[1], vertices[7] }),
                new Face(new List<Vertex> { vertices[0], vertices[7], vertices[10] }),
                new Face(new List<Vertex> { vertices[0], vertices[10], vertices[11] }),

                new Face(new List<Vertex> { vertices[1], vertices[5], vertices[9] }),
                new Face(new List<Vertex> { vertices[5], vertices[11], vertices[4] }),
                new Face(new List<Vertex> { vertices[11], vertices[10], vertices[2] }),
                new Face(new List<Vertex> { vertices[10], vertices[7], vertices[6] }),
                new Face(new List<Vertex> { vertices[7], vertices[1], vertices[8] }),

                new Face(new List<Vertex> { vertices[3], vertices[9], vertices[4] }),
                new Face(new List<Vertex> { vertices[3], vertices[4], vertices[2] }),
                new Face(new List<Vertex> { vertices[3], vertices[2], vertices[6] }),
                new Face(new List<Vertex> { vertices[3], vertices[6], vertices[8] }),
                new Face(new List<Vertex> { vertices[3], vertices[8], vertices[9] }),

                new Face(new List<Vertex> { vertices[4], vertices[9], vertices[5] }),
                new Face(new List<Vertex> { vertices[2], vertices[4], vertices[11] }),
                new Face(new List<Vertex> { vertices[6], vertices[2], vertices[10] }),
                new Face(new List<Vertex> { vertices[8], vertices[6], vertices[7] }),
                new Face(new List<Vertex> { vertices[9], vertices[8], vertices[1] })
            };
            return new Polyhedron(vertices, faces);
        }

        public static Polyhedron Dodecahedron()
        {
            double phi = (1 + Math.Sqrt(5)) / 2; // Золотое сечение
            double scale = 1; // Масштаб для вершин

            List<Vertex> vertices = new List<Vertex>
            {
                new Vertex(-1 * scale, -1* scale , -1* scale), //0
                new Vertex(-1* scale, -1* scale, 1* scale), //1
                new Vertex(-1* scale, 1* scale, -1* scale), //2
                new Vertex(-1* scale, 1* scale, 1* scale), //3
                new Vertex( 1* scale, -1* scale, -1* scale), //4
                new Vertex( 1* scale, -1* scale, 1* scale), //5
                new Vertex( 1* scale, 1* scale, -1* scale), //6
                new Vertex( 1* scale, 1* scale, 1* scale),//7

                new Vertex(0, -1/phi * scale, -phi * scale), //8
                new Vertex(0, -1/phi * scale, phi * scale),//9
                new Vertex(0, 1/phi * scale, -phi * scale),//10
                new Vertex(0, 1/phi * scale, phi * scale),//11

                new Vertex(-1/phi * scale, -phi * scale, 0),//12
                new Vertex(-1/phi * scale, phi * scale, 0),//13
                new Vertex(1/phi * scale, -phi * scale, 0),//14
                new Vertex(1/phi * scale, phi * scale, 0),//15

                new Vertex(-phi * scale, 0, -1/phi * scale),//16
                new Vertex( phi * scale, 0, -1/phi * scale),//17
                new Vertex(-phi * scale, 0, 1/phi * scale),//18
                new Vertex( phi * scale, 0, 1/phi * scale)//19
            };

            // Определение 12 пятиугольных граней
            List<Face> faces = new List<Face>
            {
                //new polygon(new List<point> { vertices[0], vertices[1], vertices[2], vertices[3] }),
                new Face(new List<Vertex> { vertices[0], vertices[8], vertices[4], vertices[14], vertices[12] }),
                new Face(new List<Vertex> { vertices[15], vertices[7], vertices[11], vertices[3], vertices[13] }),
                new Face(new List<Vertex> { vertices[0], vertices[16], vertices[2], vertices[10], vertices[8] }),
                //new polygon(new List<point> { vertices[0], vertices[19] }),
                new Face(new List<Vertex> { vertices[14], vertices[4], vertices[17], vertices[19], vertices[5] }),
                new Face(new List<Vertex> { vertices[8], vertices[4], vertices[17], vertices[6], vertices[10] }),
                new Face(new List<Vertex> { vertices[0], vertices[12], vertices[1], vertices[18], vertices[16] }),
                new Face(new List<Vertex> { vertices[1], vertices[12], vertices[14], vertices[5], vertices[9] }),
                new Face(new List<Vertex> { vertices[3], vertices[13], vertices[2], vertices[16], vertices[18] }),
                new Face(new List<Vertex> { vertices[3], vertices[18], vertices[1], vertices[9], vertices[11] }),
                new Face(new List<Vertex> { vertices[7], vertices[11], vertices[9], vertices[5], vertices[19] }),
                new Face(new List<Vertex> { vertices[13], vertices[2], vertices[10], vertices[6], vertices[15] }),
                //new polygon(new List<point> { vertices[19], vertices[18], vertices[17], vertices[16], vertices[16] })
            };

            return new Polyhedron(vertices, faces);
        }

        public Vertex Centroid(Matrix matrix)
        {
            double centerX = 0; double centerY = 0; double centerZ = 0;
            foreach (Vertex vertex in Vertices)
            {
                var v = Transformer.TransformToWorld(vertex, matrix, p);
                centerX += v.X;
                centerY += v.Y;
                centerZ += v.Z;
            }
            int count = Vertices.Count;
            centerX /= count;
            centerY /= count;
            centerZ /= count;

            return new Vertex(centerX, centerY, centerZ);
        }
    }
}


//vertex.cs
using System;

namespace Lab7
{
    public class Vertex
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vertex(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }


        public static Vertex operator +(Vertex v1, Vertex v2) 
        {
            return new Vertex(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vertex operator /(Vertex v, double scalar) 
        {
            if (scalar == 0)throw new DivideByZeroException("Деление на ноль! В школе не учился? :)");

            return new Vertex(v.X / scalar, v.Y / scalar, v.Z / scalar);
        }

        //public static Vertex operator/=(Vertex v, double scalar)
        //{

        //    if (scalar == 0) 
        //    {
        //        throw new DivideByZeroException("Деление на ноль! В школе не учился? :)");
        //    }

        //    v.X /= scalar;
        //    v.Y /= scalar;
        //    v.Z /= scalar;
        //    return v;
        //}
    }
}