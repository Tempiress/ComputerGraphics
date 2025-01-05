//form1.cs
using Lab6;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
//1264
//1275
//1277
namespace Lab8
{
    public partial class Afins3D : Form
    {
        private static int _spin = 0;
        fileaction f = new fileaction();
        private Polyhedron _polyhedron;
        double d = 5;
        Projection projectionFunction = new Projection();
        private double _cameraAngle = 0;
        ZBufferRenderer renderer;
        private Camera _camera = new Camera
        {
            Position = new Vertex(1, 1, 0.1), // Камера расположена в пространстве
            Target = new Vertex(0, 0, 0),   // Камера смотрит на центр объекта
            FieldOfView = Math.PI / 4,      // Угол обзора
            AspectRatio = 16.0 / 9.0,       // Соотношение сторон экрана
            NearPlane = 0.1,
            FarPlane = 200.0
        };



        int _fi;
        double _l;
        double _m;
        double _n;

        private Matrix _reflection;
        //private Matrix _rotation;
        private double _translationX , _translationY, _translationZ;
        private double _scaleX, _scaleY, _scaleZ;
        private double _rotationX, _rotationY, _rotationZ;

        private double _inittranslationX, _inittranslationY, _inittranslationZ;
        private double _initscaleX, _initscaleY, _initscaleZ;
        private double _initrotationX, _initrotationY, _initrotationZ;

        public Afins3D()
        {
            InitializeComponent();
            Initialize();
            projectionFunction.setProjection(Projection.enumprojection.perspective);
           

            //pictureBox1.MouseMove += Form_MouseMove;
        }
        //private void Form_MouseMove(object sender, MouseEventArgs e)
        //{
        //    MousepositionLabel.Text = $@"X:{e.X}, Y:{e.Y}";
        //}

       
        private void Initialize()
        {
            _translationX = 0; _translationY = 0; _translationZ = 0;
            _scaleX = 1; _scaleY = 1; _scaleZ = 1;
            _rotationX = 0; _rotationY = 0; _rotationZ = 0;

            _inittranslationX = 0; _inittranslationY = 0; _inittranslationZ = 0;
            _initscaleX = 1; _initscaleY = 1; _initscaleZ = 1;
            _initrotationX = 0; _initrotationY = 0; _initrotationZ = 0;

            _reflection =  new Matrix( new double[,]
            {
                {1, 0, 0, 0 },
                {0, 1, 0, 0 },
                {0, 0, 1, 0 },
                {0, 0, 0, 1 }
            });

          //  _rotation = new Matrix (new double[,]
          // {
          //      {1, 0, 0, 0 },
          //      {0, 1, 0, 0 },
          //      {0, 0, 1, 0 },
          //      {0, 0, 0, 1 }
          //});

          //  _lrotationIs = new Matrix(new double[,]
          // {
          //      {1, 0, 0, 0 },
          //      {0, 1, 0, 0 },
          //      {0, 0, 1, 0 },
          //      {0, 0, 0, 1 }
          //});
        }

        // Возвращает геометрический центр полигона
        // Возвращает геометрический центр всего многогранника

        void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
           var radionButton = sender as RadioButton;

            if (radionButton != null && radionButton.Checked)
            {
                if (radionButton.Text == "Перспектива") projectionFunction.setProjection(Projection.enumprojection.perspective);
                if (radionButton.Text == "Изометрия") projectionFunction.setProjection(Projection.enumprojection.isometric);
            }

            pictureBox1.Invalidate();
        }

       

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (_polyhedron == null)
                return;

            _inittranslationX += _translationX;
            _inittranslationY += _translationY;
            _inittranslationZ += _translationZ;
            _initscaleX += _scaleX;
            _initscaleY += _scaleY;
            _initscaleZ += _scaleZ;
            _initrotationX += _rotationX;
            _initrotationY += _rotationY;
            _initrotationZ += _rotationZ;

            var viewMatrix = _camera.GetViewMatrix();
            var projectionMatrix = _camera.GetProjectionMatrix();

            var transformationMatrix = TranslationMatrix(_inittranslationX, _inittranslationY, _inittranslationZ) *
                                       ScalingMatrix(_initscaleX, _initscaleY, _initscaleZ) *
                                       RotationMatrix(_initrotationX, _initrotationY, _initrotationZ);

            int clientWidth = e.ClipRectangle.Width;
            int clientHeight = e.ClipRectangle.Height;

            int offsetX = clientWidth / 2;
            int offsetY = clientHeight / 2;

            foreach (var face in _polyhedron.Faces)
            {
                var points2D = new List<Point>();
                foreach (var vertex in face.Vertices)
                {
                    // Трансформация вершины
                    var transformedVertex = Transformer.TransformToWorld(vertex, transformationMatrix * viewMatrix * projectionMatrix, projectionFunction);

                    Console.WriteLine($"Before normalization: X={transformedVertex.X}, Y={transformedVertex.Y}, Z={transformedVertex.Z}, W={transformedVertex.W}");
                    // Нормализация в экранные координаты
                    double w = transformedVertex.W;
                    double x = transformedVertex.X / w;
                    double y = transformedVertex.Y / w;
                    
                    // Преобразование в пиксельные координаты
                    points2D.Add(new Point(
                        (int)(x * offsetX + offsetX),
                        (int)(y * offsetY + offsetY)
                    ));
                }

                // Отрисовка полигона
                if (points2D.Count >= 3)
                {
                    e.Graphics.DrawPolygon(Pens.Black, points2D.ToArray());
                }
            }




            //BEGIN---------------------------ZBUFFER
            //if (renderer == null) 
            //{
            //    renderer = new ZBufferRenderer(e.ClipRectangle.Width, e.ClipRectangle.Height, projectionFunction);
            //}

           

            //Matrix transformationMatrix = TranslationMatrix(_inittranslationX, _inittranslationY, _inittranslationZ) *
            //                              ScalingMatrix(_initscaleX , _initscaleY , _initscaleZ) *
            //                              RotationMatrix(_initrotationX, _initrotationY , _initrotationZ);

            //// Рендерим все грани
            //foreach (var face in _polyhedron.Faces)
            //{
            //    renderer.RenderFace(face, transformationMatrix, Pens.Black);
            //    renderer.DrawToGraphics(e.Graphics);
            //}

            //renderer.ClearBuffer();



            //Направление обзора
            //var viewDirection = new Vertex(1, 0, -1);


            ////END--------------------------- ZBUFFER


            //Matrix translationMatrix = TranslationMatrix(_translationX, _translationY, _translationZ);
            //Matrix scalingMatrix = ScalingMatrix(_scaleX, _scaleY, _scaleZ);
            //Matrix rotationMatrix = RotationMatrix(_rotationX, _rotationY, _rotationZ);
            //Matrix lrotation = LRotation(_fi, _l, _m, _n);
            //Vertex centroid = _polyhedron.Centroid(_polyhedron.LocalToWorld);

            //Matrix toCenter = TranslationMatrix(-centroid.X, -centroid.Y, -centroid.Z);
            //Matrix fromCenter = TranslationMatrix(centroid.X, centroid.Y, centroid.Z);

            //// Матрица преобразования (только поворот и масштабирование, без переноса)
            //Matrix trasformationMatrixWithoutTranslation = RotationMatrix(_rotationX, _rotationX, _rotationZ) * ScalingMatrix(_scaleX, _scaleY, _scaleZ);
            //Matrix worldMatrix;
            //if (!IsCentroid)
            //{
            //    IsCentroid = true;
            //    worldMatrix = translationMatrix * scalingMatrix * rotationMatrix * lrotation * _reflection;
            //}
            //else
            //{
            //    worldMatrix = toCenter * translationMatrix * scalingMatrix * rotationMatrix * lrotation * _reflection * fromCenter;
            //}
            //_polyhedron.LocalToWorld *= worldMatrix;

            //int clientWidth = e.ClipRectangle.Width;
            //int clientHeight = e.ClipRectangle.Height;

            //int offsetX = clientWidth / 2;
            //int offsetY = clientHeight / 2;

            //centroid = _polyhedron.Centroid(_polyhedron.LocalToWorld);
            //e.Graphics.FillRectangle(Brushes.Red, (int)centroid.X + offsetX, (int)centroid.Y + offsetY, 2, 2);

            //var points2D = new List<Point>(10);

            //// Выполняем отсечение
            //Polyhedron visibleFaces = BackfaceCulling(_polyhedron, viewDirection, trasformationMatrixWithoutTranslation);
            //foreach (Face face in visibleFaces.Faces)
            //{
            //    foreach (Vertex vertex in face.Vertices)
            //    {
            //        Vertex worldVertex = Transformer.TransformToWorld(vertex, _polyhedron.LocalToWorld * projectionFunction.getProjection(), projectionFunction);
            //        if (worldMatrix == null) throw new InvalidOperationException("Матрица преобразования некорректна.");
            //        points2D.Add(new Point((int)worldVertex.X, (int)worldVertex.Y));
            //    }

            //    var centeredPoints = points2D.Select(p => new Point(p.X + offsetX, p.Y + offsetY)).ToArray();
            //    if (centeredPoints.Length > 0)
            //    {
            //        e.Graphics.DrawPolygon(Pens.Black, centeredPoints);
            //    }
            //    points2D.Clear();
            //}
        }

        public bool IsCentroid { get; private set; }


        //Матрица перемещения
        private Matrix TranslationMatrix(double tx, double ty, double tz)
        {
            return new Matrix(new[,]
            {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { tx, ty, tz, 1 }
            });
        }

        //Матрица Масштабирования
        private Matrix ScalingMatrix(double sx, double sy, double sz)
        {

            return new Matrix( new[,]
            {
                {sx, 0, 0 ,0 },
                {0, sy, 0, 0 },
                {0, 0, sz, 0 },
                {0, 0, 0, 1 }
            });

        }

        private Matrix RotationMatrix(double angleX, double angleY, double angleZ)
        {
            //return MultiplyMarices(MultiplyMarices(zRotation(angleZ), yRotation(angleY)), xRotationMatrix(angleX));
            //return MultiplyMarices(MultiplyMarices(xRotationMatrix(angleX), yRotation(angleY)), zRotation(angleZ));
            return XRotationMatrix(angleX) * YRotation(angleY) * ZRotation(angleZ);
            //return MultiplyMarices(zRotation(angleZ), MultiplyMarices(yRotation(angleY), xRotationMatrix(angleX)));
        }

        private Matrix XRotationMatrix(double a)
        {

            double radians = (Math.PI / 180) * a;
            return new Matrix(new[,] {
            { 1,0,0,0 },
            {0, Math.Cos(radians), - Math.Sin(radians), 0 },
            { 0, Math.Sin(radians), Math.Cos(radians ), 0},
            {0, 0 ,0, 1 }
            });

        }

        private Matrix YRotation(double a)
        {
            double radians = (Math.PI / 180) * a;
            var cosx = Math.Cos(radians);
            var sinx = Math.Sin(radians);

            return new Matrix(new double[,] {
            {cosx, 0 , sinx, 0 },
            {0, 1, 0, 0 },
            {-sinx, 0, cosx, 0 },
            {0, 0, 0, 1 }
            });

        }

        private Matrix ZRotation(double a)
        {
            double radians = (Math.PI / 180) * a;
            var cosx = Math.Cos(radians);
            var sinx = Math.Sin(radians);

            return new Matrix( new double[,] {
            {cosx, -sinx, 0, 0  },
            {sinx, cosx, 0, 0  },
            {0, 0, 1, 0 },
            {0, 0, 0, 1 }
            });
        }

        private Matrix LRotation(int fi, double l, double m, double n)
        {
            double fiRad = (Math.PI / 180) * fi;
            double cosFi = Math.Cos(fiRad);
            double sinFI = Math.Sin(fiRad);

            return new Matrix( new double[,]
            {
                {Math.Pow(l, 2) + cosFi * (1 - Math.Pow(l, 2)), l * (1 - cosFi) * m + n * sinFI,               l * (1 - cosFi) * n - m * sinFI,              0},
                {l * (1 - cosFi) * m - n * sinFI,               Math.Pow(m, 2) + cosFi * (1 - Math.Pow(m, 2)), m * (1 - cosFi ) *  n  + l * sinFI,           0},
                {l * (1 - cosFi) * n + m * sinFI,               m * (1 - cosFi) * n - l * sinFI,               Math.Pow(n,2) + cosFi * (1 - Math.Pow(n, 2)), 0  },
                {0, 0, 0 ,1 }

            });

        }

        private void button1_Click(object sender, EventArgs e)
        {

            switch (comboBox1.Text)
            {
                case "Тетраэдр":
                    _polyhedron = Polyhedron.Tetrahedron();
                    pictureBox1.Invalidate();
                    break;
                case "Гексаэдр":
                    _polyhedron = Polyhedron.Hexahedron();
                    pictureBox1.Invalidate();
                    break;
                case "Октаэдр":
                    _polyhedron = Polyhedron.Octahedron();
                    pictureBox1.Invalidate();
                    break;

                case "Икосаэдр":
                    _polyhedron = Polyhedron.Icosahedron();
                    pictureBox1.Invalidate();
                    break;

                case "Додекаэдр":
                    _polyhedron = Polyhedron.Dodecahedron();
                    pictureBox1.Invalidate();
                    break;
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            _translationX = Convert.ToDouble(dxBox.Text, new NumberFormatInfo() { NumberDecimalSeparator = "." });
            _translationY = Convert.ToDouble(dyBox.Text, new NumberFormatInfo() { NumberDecimalSeparator = "." });
            _translationZ = Convert.ToDouble(dzBox.Text, new NumberFormatInfo() { NumberDecimalSeparator = "." });

            pictureBox1.Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var dxScale = Convert.ToDouble(textBox1.Text, new NumberFormatInfo() { NumberDecimalSeparator = "."});
            var dyScale = Convert.ToDouble(textBox2.Text, new NumberFormatInfo() { NumberDecimalSeparator = "." });
            var dzScale = Convert.ToDouble(textBox3.Text, new NumberFormatInfo() { NumberDecimalSeparator = "." });

            _scaleX = dxScale;
            _scaleY = dyScale;
            _scaleZ = dzScale;

            IsCentroid = false;

            pictureBox1.Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
            {
                switch(_spin)
                {
                    case 0:
                        //xRotation(3);
                        break;
                    case 1:
                        YRotation(3);
                        break;
                    case 2:
                        ZRotation(3);
                        break;
                }
            }

        private void button8_Click(object sender, EventArgs e)
        {
            _camera.Position = new Vertex(Convert.ToDouble(dxBox.Text), Convert.ToDouble(dyBox.Text), Convert.ToDouble(dzBox.Text));
            pictureBox1.Invalidate();
        }

        private void ButtonLoad_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel) 
            {
                return;
            }
            string filename = openFileDialog1.FileName;
            //string filetext = System.IO.File.ReadAllText(filename);
            _polyhedron = f.LoadFromOBJ(filename);
            pictureBox1.Invalidate();
            
            //MessageBox.Show("asd");
        }

        private void Save_Click(object sender, EventArgs e)
        {
            // Пример фигуры вращения
            var profile = new List<Vertex> {
                new Vertex(0, 0, 0),
                new Vertex(0, 1, 0),
                new Vertex(0, 2, 1),
            };

            var revolutionFigure = FigureBuilder.BuildRevolutionFigure(profile, "z", 500);
            var fileAction = new fileaction();
            fileAction.SaveToOBJ(revolutionFigure, "revolution.obj");

            // Пример графика функции
            Func<double, double, double> func = (x, y) => Math.Sin(Math.Sqrt(x * x + y * y));
            var surface = FigureBuilder.BuildSurface(func, -5, 5, -5, 5, 20);
            fileAction.SaveToOBJ(surface, "surface.obj");
        }

        public void radioButtonSwitch(object sender, EventArgs e)
        {
            var rbt = sender as RadioButton;

            if (rbt != null && rbt.Checked)
            {
                switch (rbt.Text)
                {

                    case "XAxis":
                       _spin = 0;
                        break;
                    case "YAxis":
                        _spin = 1;
                        break;
                    case "ZAxis":
                        _spin = 2;
                        break;

                }

            }

        }

        private void RotateCamera()
        {
            _cameraAngle += 0.01; // Угол поворота
            _camera.Position = new Vertex(
                5 * Math.Cos(_cameraAngle), // X
                5,                          // Y
                5 * Math.Sin(_cameraAngle)  // Z
            );
            pictureBox1.Invalidate(); // Перерисовать сцену
        }

        private void rotationButton_Click(object sender, EventArgs e)
        {
            RotateCamera();
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {

                if (radioButton6.Checked)
                {

                    var reflXYMatr = new Matrix( new double[,]
                    {
                            {1, 0, 0, 0 },
                            {0, 1, 0, 0 },
                            {0, 0, -1, 0 },
                            {0, 0, 0, 1 }
                    });

                    //reflMatr = MultiplyMarices(reflMatr, reflXYMatr);
                    MessageBox.Show("Hy");
                    _reflection = _reflection * reflXYMatr;

                    pictureBox1.Invalidate();


                }
                else if (radioButton7.Checked)
                {

                    var reflXZMatr = new Matrix( new double[,]
                    {
                            {1, 0, 0, 0 },
                            {0, -1, 0, 0 },
                            {0, 0, 1, 0 },
                            {0, 0, 0, 1 }
                    });

                    //reflMatr = MultiplyMarices(reflMatr, reflXZMatr);
                    _reflection = _reflection * reflXZMatr;
                    pictureBox1.Invalidate();
                }
                else if (radioButton8.Checked)
                {

                    var reflYZMatr = new Matrix( new double[,]
                    {
                            {-1, 0, 0, 0 },
                            {0, 1, 0, 0 },
                            {0, 0, 1, 0 },
                            {0, 0, 0, 1 }
                    });

                    //reflMatr = MultiplyMarices(reflMatr, reflYZMatr);
                    _reflection = _reflection * reflYZMatr;

                    pictureBox1.Invalidate();
                }

        }

        private void button7_Click(object sender, EventArgs e)
        {
 
            _fi = Convert.ToInt32(textBox4.Text);
                _l = Convert.ToInt32(textBox5.Text);
                _m = Convert.ToInt32(textBox6.Text);
                _n = Convert.ToInt32(textBox7.Text);
            //    Matrix lRotMatr1 = LRotation(Convert.ToInt32(textBox4.Text), Convert.ToDouble(textBox5.Text), Convert.ToDouble(textBox6.Text), Convert.ToDouble(textBox7.Text));
            //_lrotationIs = lRotMatr1;
                //_rotation *= lRotMatr1;
            pictureBox1.Invalidate();
        }

        private void switchReflectionRadioButton(object sender, EventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            try
            {
                    switch (_spin) 
                    {

                        case 0:
                            _rotationX = Convert.ToInt32(rotationBox.Text);
                            _rotationY = 0;
                            _rotationZ = 0;
                            break;
                        case 1:
                            _rotationY = Convert.ToInt32(rotationBox.Text);
                            _rotationX = 0;
                            _rotationZ = 0;
                            break;
                        case 2:
                            _rotationZ = Convert.ToInt32(rotationBox.Text);
                            _rotationX = 0;
                            _rotationY = 0;
                            break;


                    }
                pictureBox1.Invalidate();
            }
            catch (Exception)
            {
                MessageBox.Show("Введите корректные значения!");
            }

        }
        private void button6_Click_1(object sender, EventArgs e)
        {
            var dxScale = Convert.ToDouble(textBox1.Text, new NumberFormatInfo() { NumberDecimalSeparator = "." });
            var dyScale = Convert.ToDouble(textBox2.Text, new NumberFormatInfo() { NumberDecimalSeparator = "." });
            var dzScale = Convert.ToDouble(textBox3.Text, new NumberFormatInfo() { NumberDecimalSeparator = "." });

            _scaleX = dxScale;
            _scaleY = dyScale;
            _scaleZ = dzScale;

            //(scaleXCenter, scaleYCenter, scaleZCenter) = Centroid();

            pictureBox1.Invalidate();
        }


        public static Polyhedron BackfaceCulling(Polyhedron polyhedron, Vertex viewDirection, Matrix transformationMatrix)
        {
            //Если нормалей нет, возвращаем исходный polyhedron
            if (polyhedron.Faces[0].Normales.Count == 0) return polyhedron;

            var visibleFaces = new List<Face>();
            var visibleVertices = new List<Vertex>();

            foreach (var face in polyhedron.Faces)
            {
                var normal = face.Normales[0];

                var transformedNormal = Transformer.TransformNormal(face.Normales[0], transformationMatrix);
                double dotProduct = transformedNormal.X * viewDirection.X +
                                    transformedNormal.Y * viewDirection.Y +
                                    transformedNormal.Z * viewDirection.Z;

                //Если грань "лицевая", добавляем её в список видимых
                if (dotProduct < 0) 
                {
                    visibleFaces.Add(face);
                    foreach (var vertex in face.Vertices) 
                    {
                        if(!visibleVertices.Contains(vertex))
                            visibleVertices.Add(vertex);
                    }
                }
                  

            }
            return new Polyhedron(visibleVertices.ToList(), visibleFaces);
        }




        


    }

    public class Transformer
    {   
        
        public static Vertex TransformToWorld(Vertex vertex, Matrix matrix, Projection projection)
        {
            var point = new Matrix(new[,] { { vertex.X, vertex.Y, vertex.Z, vertex.W } });
            point = point *  matrix;

            //var result = new Vertex(point[0, 0], point[0, 1], point[0, 2]);

            double w = point[0, 3]; // Извлечение W после преобразования
            if (projection.getEnumProjection() == Projection.enumprojection.perspective)
            {
                //return result / point[0, 3];
                return new Vertex(point[0, 0], point[0, 1], point[0, 2], w);
            }

            //return result;
            return new Vertex(point[0, 0], point[0, 1], point[0, 2], w);
        }

        public static Vertex TransformNormal(Vertex normal, Matrix transformationMatrix)
        {


            // Используем только 3x3 подматрицу для нормали
            var normalMatrix = new Matrix(new[,]
            {
        { transformationMatrix[0, 0], transformationMatrix[0, 1], transformationMatrix[0, 2] },
        { transformationMatrix[1, 0], transformationMatrix[1, 1], transformationMatrix[1, 2] },
        { transformationMatrix[2, 0], transformationMatrix[2, 1], transformationMatrix[2, 2] }
        });

            var point = new Matrix(new[,] { { normal.X, normal.Y, normal.Z } }); // 1x3
            point *= normalMatrix; // Умножение 1x3 на 3x3

            //var transformedNormal = TransformToWorld(normal, normalMatrix, new Projection());

            // Нормализация нормали после преобразования
            double length = Math.Sqrt(point[0, 0] * point[0, 0] + point[0, 1] * point[0, 1] + point[0, 2] * point[0, 2]);
            return new Vertex(point[0, 0] / length, point[0, 1] / length, point[0, 2] / length);
        }

    }


    public class Projection
    {
        //private Func<Matrix> _projectionFunction { get; set; }
        private Matrix _projMatrix;
        public enum enumprojection
        {
            none,
            perspective,
            isometric
        }
        enumprojection en = enumprojection.perspective;
        public void setProjection(enumprojection proj)
        {

            switch (proj)
            {
                case enumprojection.perspective:
                    _projMatrix = PerspectiveMatrix();
                    break;
                case enumprojection.isometric:
                    _projMatrix = IsometricMatrix();
                    break;
            }
        }
        public enumprojection getEnumProjection()
        {
            return en;
        }

        public Matrix getProjection() { return _projMatrix; }

        //Перспективная проекция
        private Matrix PerspectiveMatrix()
        {
            return new Matrix(new[,]
            {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, -1/-300d },
                { 0, 0, 0 ,1 }
            });
        }

        private Matrix IsometricMatrix()
        {
            //return new Matrix(new[,]
            //{
            //    { Math.Sqrt(3), 0, -Math.Sqrt(3), 0 },
            //    { 1, 2, 1, 0 },
            //    { Math.Sqrt(2), -Math.Sqrt(2), Math.Sqrt(2), 0 },
            //    { 0, 0, 0, 1 }
            //});

            return new Matrix(new[,]
{
                {Math.Sqrt(3), 1, Math.Sqrt(2), 0 },
                {0, 2, -Math.Sqrt(2), 0 },
                {-Math.Sqrt(3), 1, Math.Sqrt(2), 0 },
                {0, 0 ,0, 1 }
            });


        }


    }

}



//camera.cs
using Lab8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab6
{
    public class Camera
    {
        public Vertex Position { get; set; }
        public Vertex Target { get; set; }
        public Vertex Up { get; set; } = new Vertex(0, 1, 0); // Вектор "вверх" камеры


        public double FieldOfView { get; set; } = Math.PI / 4; // Угол обзора (рад)
        public double AspectRatio { get; set; } = 1.0; // Соотношение сторон экрана
        public double NearPlane { get; set; } = 0.1; // Ближняя плоскость отсечения
        public double FarPlane { get; set; } = 100.0; // Дальняя плоскость отсечения

        // Получение матрицы вида (View Matrix)
        public Matrix GetViewMatrix()
        {
            var zAxis = Normalize(new Vertex(
                Position.X - Target.X,
                Position.Y - Target.Y,
                Position.Z - Target.Z
            ));

            var xAxis = Normalize(CrossProduct(Up, zAxis));
            var yAxis = CrossProduct(zAxis, xAxis);

            return new Matrix(new[,]
            {
            { xAxis.X, xAxis.Y, xAxis.Z, -DotProduct(xAxis, Position) },
            { yAxis.X, yAxis.Y, yAxis.Z, -DotProduct(yAxis, Position) },
            { zAxis.X, zAxis.Y, zAxis.Z, -DotProduct(zAxis, Position) },
            { 0,       0,       0,       1 }
        });
        }

        // Получение матрицы проекции (Projection Matrix)
        public Matrix GetProjectionMatrix()
        {
            double f = 1.0 / Math.Tan(FieldOfView / 2.0);
            return new Matrix(new[,]{
            { f / AspectRatio, 0,  0,                                0 },
            { 0,               f,  0,                                0 },
            { 0,               0,  (FarPlane + NearPlane) / (NearPlane - FarPlane), (2 * FarPlane * NearPlane) / (NearPlane - FarPlane) },
            { 0,               0, -1,                                0 }
        });
        }

        // Вспомогательные методы
        private static Vertex Normalize(Vertex v)
        {
            double length = Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
            return new Vertex(v.X / length, v.Y / length, v.Z / length);
        }

        private static Vertex CrossProduct(Vertex a, Vertex b)
        {
            return new Vertex(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
            );
        }

        private static double DotProduct(Vertex a, Vertex b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }



    }

}

//vertex.cs

using System;

namespace Lab8
{
    public class Vertex
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double W { get; set; }
       


        public Vertex(double x, double y, double z, double w = 1)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
           
        }

        public Vertex(Vertex a, Vertex b) 
        {
            a.X = X;
            a.Y = Y;
            a.Z = Z;

        }

        public Vertex(Vertex v) 
        {
            v.X = X;
            v.Y = Y;
            v.Z = Z;
        }




        public static Vertex operator +(Vertex v1, Vertex v2) 
        {
            return new Vertex(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vertex operator /(Vertex v, double scalar) 
        {
            if (scalar == 0)throw new DivideByZeroException("Деление на ноль!");

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

//polyhedron.cs

using System;
using System.Collections.Generic;

namespace Lab8
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

//Matrix.cs
namespace Lab8
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


//fileaction.cs

using Lab8;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab8
{
    internal class fileaction
    {

        public Polyhedron LoadFromOBJ(string filePath)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<Face> faces = new List<Face>();
            List<Vertex> normales = new List<Vertex>();
            
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

                else if (line.StartsWith("vn "))
                {
                    var normalesParts = line.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                    double nx = double.Parse(normalesParts[1], CultureInfo.InvariantCulture);
                    double ny = double.Parse(normalesParts[2], CultureInfo.InvariantCulture);
                    double nz = double.Parse(normalesParts[3], CultureInfo.InvariantCulture);
                    normales.Add(new Vertex(nx, ny, nz));
                }

                //else if (line.StartsWith("vt ")) 
                //{
                //    var texturesParts = line.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                //    double n1 = double.Parse(texturesParts[1], CultureInfo.InvariantCulture);
                //    double n2 = double.Parse(texturesParts[2], CultureInfo.InvariantCulture);
                //    double n3 = double.Parse(texturesParts[3], CultureInfo.InvariantCulture);
                //    normales.Add(new Vertex(n1, n2, n3));
                //}

                else if (line.StartsWith("f "))
                {
                    var partsFace = line.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);

                    var faceVetices = new List<Vertex>();
                    var faceNormales = new List<Vertex>(); // для нормалей
                    //foreach (var p in parts) 
                    //{
                    //    vv[i] = vertices[int.Parse(p)];
                    //    i++;

                    //}
                  
                    for (int j = 1; j < partsFace.Length; j++)
                    {
                        var indices  = partsFace[j].Split('/', (char)StringSplitOptions.RemoveEmptyEntries);
                        // Извлечение индекса вершины
                        int vertexIndex = int.Parse(indices[0]) - 1;
                        faceVetices.Add(vertices[vertexIndex]);

                        //извлечение индекса нормали, если есть
                        if (indices.Length > 1) 
                        {
                            faceNormales.Add(normales[int.Parse(indices[1]) - 1]);
                        }
                        //tempListVertex.Add(vertices[int.Parse(partsFace[j]) - 1]);
                       

                    }
                    var face = new Face(faceVetices);
                    if (faceNormales.Count > 0) face.Normales = faceNormales; // Добавляем нормали в грань
                    faces.Add(face);


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
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "v {0:0.###} {1:0.###} {2:0.###}", vertex.X, vertex.Y, vertex.Z));
                    //writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "v {0:0.###} {1:0.###} {2:0.###}", vertex.NX, vertex.NY, vertex.NZ));

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
