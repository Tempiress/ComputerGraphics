//form1.cs

using Lab6;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
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
        private Polyhedron _polyhedron2;
        double d = 5;
        Projection projectionFunction = new Projection();
        private double _cameraAngle = 0;
        ZBufferRenderer renderer;
        private Camera _camera = new Camera
        {
            Position = new Vertex(1, 1, 10), // Камера расположена в пространстве
            Target = new Vertex(0, 0, 0),   // Камера смотрит на центр объекта
            FieldOfView = Math.PI / 4,      // Угол обзора
            AspectRatio = 16.0 / 9.0,       // Соотношение сторон экрана
            NearPlane = 0.1,
            FarPlane = 200.0
        };

        // Центр PictureBox
        static int centerX;
        static int centerY;

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


        RayTracer rayTracer;// Поле для Ray Tracer
        private PointLight light; // Точечный источник света
        //List<SceneObject> sceneObjects = new List<SceneObject>
        //{
        //new SceneObject
        //{
        //    Polyhedron = Polyhedron.Tetrahedron(),
        //    TranslationX = 0,
        //    TranslationY = 0,
        //    TranslationZ = 0,
        //    RotationX = 0,
        //    RotationY = 0,
        //    RotationZ = 0,
        //    ScaleX = 1,
        //    ScaleY = 1,
        //    ScaleZ = 1
        //},
        //new SceneObject
        //{
        //    Polyhedron = Polyhedron.Hexahedron(),
        //    TranslationX = 5,
        //    TranslationY = 0,
        //    TranslationZ = 0,
        //    RotationX = 45,
        //    RotationY = 0,
        //    RotationZ = 0,
        //    ScaleX = 1,
        //    ScaleY = 1,
        //    ScaleZ = 1
        //}\


        List<SceneObject> sceneObjects = new List<SceneObject>
        {
        // Комната (куб)
        new SceneObject
        {
            Polyhedron = Polyhedron.CreateRoom(450), // Комната размером 10x10x10
            TranslationX = 3,            // Центр комнаты в начале координат
            TranslationY = 3,
            TranslationZ = 3,
            RotationX = 0,
            RotationY = 0,
            RotationZ = 0,
            ScaleX = 1,
            ScaleY = 1,
            ScaleZ = 1,

        },
        // Объект внутри комнаты (например, сфера или куб)
        new SceneObject
        {
            Polyhedron = Polyhedron.Tetrahedron(40), // Пример объекта (тетраэдр)
            TranslationX = 3,                     // Объект в центре комнаты
            TranslationY = 3,
            TranslationZ = 3,
            RotationX = 0,
            RotationY = 0,
            RotationZ = 0,
            ScaleX = 0.5,                         // Масштабируем объект
            ScaleY = 0.5,
            ScaleZ = 0.5,
        }
        };
        

    

        public Afins3D()
        {
            InitializeComponent();
            Initialize();
            projectionFunction.setProjection(Projection.enumprojection.perspective);

            light = new PointLight
            {
                Position = new Vertex(0, 5, 0),
                Color = Color.White,
                Intensity = 1.0
            };

            // Инициализация Ray Tracer
            rayTracer = new RayTracer(sceneObjects, light);


            //pictureBox1.MouseMove += Form_MouseMove;
        }
        //private void Form_MouseMove(object sender, MouseEventArgs e)
        //{
        //    MousepositionLabel.Text = $@"X:{e.X}, Y:{e.Y}";
        //}

       
        private void Initialize()
        {
            // Центр PictureBox
            centerX = pictureBox1.Width / 2;
            centerY = pictureBox1.Height / 2;

            //_translationX = 0; _translationY = 0; _translationZ = 0;
            //_scaleX = 1; _scaleY = 1; _scaleZ = 1;
            //_rotationX = 0; _rotationY = 0; _rotationZ = 0;

            //_inittranslationX = 0; _inittranslationY = 0; _inittranslationZ = 0;
            //_initscaleX = 1; _initscaleY = 1; _initscaleZ = 1;
            //_initrotationX = 0; _initrotationY = 0; _initrotationZ = 0;


            sceneObjects[0].TranslationX += centerX;
            sceneObjects[0].TranslationY += centerY;

            sceneObjects[1].TranslationX += centerX - 100;
            sceneObjects[1].TranslationY += centerY - 100;
            sceneObjects[1].TranslationZ = -5;

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

            // Создаем Ray Tracer (если он еще не создан)
            if (rayTracer == null)
            {
                rayTracer = new RayTracer(sceneObjects, light);
            }

            // Очистка PictureBox (если нужно)
            e.Graphics.Clear(Color.Black);

            // Рендеринг сцены
            for (int y = 0; y < pictureBox1.Height; y++)
            {
                for (int x = 0; x < pictureBox1.Width; x++)
                {
                    // Выпускаем луч из камеры через пиксель (x, y)
                    Vertex rayOrigin = _camera.Position;
                    Vertex rayDirection = CalculateRayDirection(x, y);

                    // Трассируем луч и получаем цвет пикселя
                    Color pixelColor = rayTracer.TraceRay(rayOrigin, rayDirection);

                    // Рисуем пиксель на PictureBox
                    using (var brush = new SolidBrush(pixelColor))
                    {
                        e.Graphics.FillRectangle(brush, x, y, 1, 1);
                    }
                }
            }









            if (_polyhedron == null)
                return;

            //_inittranslationX += _translationX;
            //_inittranslationY += _translationY;
            //_inittranslationZ += _translationZ;
            //_initscaleX *= _scaleX;
            //_initscaleY *= _scaleY;
            //_initscaleZ *= _scaleZ;
            //_initrotationX += _rotationX;
            //_initrotationY += _rotationY;
            //_initrotationZ += _rotationZ;

            //var viewMatrix = _camera.GetViewMatrix();
            //var projectionMatrix = _camera.GetProjectionMatrix();

            //var transformationMatrix = TranslationMatrix(_translationX, _translationY, _translationZ) *
            //                           ScalingMatrix(_scaleX, _scaleY, _scaleZ) *
            //                           RotationMatrix(_initrotationX, _initrotationY, _initrotationZ);

            //int clientWidth = e.ClipRectangle.Width;
            //int clientHeight = e.ClipRectangle.Height;

            //int offsetX = clientWidth / 2;
            //int offsetY = clientHeight / 2;

            //foreach (var face in _polyhedron.Faces)
            //{
            //    var points2D = new List<Point>();
            //    foreach (var vertex in face.Vertices)
            //    {
            //        // Трансформация вершины
            //        var transformedVertex = Transformer.TransformToWorld(vertex, transformationMatrix * viewMatrix * projectionMatrix, projectionFunction);

            //        Console.WriteLine($"Before normalization: X={transformedVertex.X}, Y={transformedVertex.Y}, Z={transformedVertex.Z}, W={transformedVertex.W}");
            //        // Нормализация в экранные координаты
            //        //double w = transformedVertex.W;
            //        //double x = transformedVertex.X / w;
            //        //double y = transformedVertex.Y / w;

            //        // Преобразование в пиксельные координаты
            //        points2D.Add(new Point(
            //            (int)(transformedVertex.X * offsetX + offsetX),
            //            (int)(transformedVertex.Y * offsetY + offsetY)
            //        ));
            //    }

            //    // Отрисовка полигона
            //    if (points2D.Count >= 3)
            //    {
            //        e.Graphics.DrawPolygon(Pens.Black, points2D.ToArray());
            //    }
            //}




            //BEGIN---------------------------ZBUFFER
            if (renderer == null)
            {
                renderer = new ZBufferRenderer(e.ClipRectangle.Width, e.ClipRectangle.Height, projectionFunction);
            }
            //finalTransformationMatrix = TranslationMatrix(_translationX, _translationY, _translationZ) *
            //                                  ScalingMatrix(_scaleX, _scaleY, _scaleZ) *
            //                                  RotationMatrix(_initrotationX, _initrotationY, _initrotationZ);

            renderer.ClearBuffer();

            foreach (var sceneObject in sceneObjects)
            {
                // Вычисляем центроид фигуры
                Vertex centroid = sceneObject.Polyhedron.Centroid(sceneObject.Polyhedron.LocalToWorld);

                // Матрица перемещения в центр
                Matrix toCenter = TranslationMatrix(-centroid.X, -centroid.Y , -centroid.Z);

                // Матрица преобразований (вращение, масштабирование и т.д.)
                Matrix transformationMatrix = RotationMatrix(sceneObject.RotationX, sceneObject.RotationY,sceneObject.RotationZ) *
                                              ScalingMatrix(sceneObject.ScaleX, sceneObject.ScaleY, sceneObject.RotationZ);
                
                // Матрица возврата на место
                Matrix fromCenter = TranslationMatrix(centroid.X, centroid.Y, centroid.Z);

                // Матрица отдельного перемещения
                Matrix translationMatrix = TranslationMatrix( sceneObject.TranslationX, sceneObject.TranslationY, sceneObject.TranslationZ);

                // Финальная матрица преобразования
                Matrix finalTransformationMatrix = toCenter * transformationMatrix * fromCenter * translationMatrix;

                int q = 1;
                // Рендерим все грани
                foreach (var face in sceneObject.Polyhedron.Faces)
                {


                    if (q == 1) renderer.RenderFace(face, finalTransformationMatrix, Pens.Yellow);
                    else if (q == 2) renderer.RenderFace(face, finalTransformationMatrix, Pens.Black);
                    else if (q == 3) renderer.RenderFace(face, finalTransformationMatrix, Pens.Azure);
                    else if (q == 4) renderer.RenderFace(face, finalTransformationMatrix, Pens.Red);
                    else if (q == 5) renderer.RenderFace(face, finalTransformationMatrix, Pens.Green);
                    else renderer.RenderFace(face, finalTransformationMatrix, Pens.Brown);
                    q++;
                    //renderer.RenderFace(face, transformationMatrix, Pens.Black);
                    //renderer.DrawToGraphics(e.Graphics);
                }
            }

            renderer.DrawToGraphics(e.Graphics);




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


        private Vertex CalculateRayDirection(int x, int y)
        {
            double fov = Math.PI / 4; // Угол обзора
            double aspectRatio = (double)pictureBox1.Width / pictureBox1.Height;

            double px = (2 * ((x + 0.5) / pictureBox1.Width) - 1) * Math.Tan(fov / 2) * aspectRatio;
            double py = (1 - 2 * ((y + 0.5) / pictureBox1.Height)) * Math.Tan(fov / 2);

            return new Vertex(px, py, -1).Normalize(); // Направление луча
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
                    _polyhedron = Polyhedron.Tetrahedron(40);
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
            double _transX = Convert.ToDouble(dxBox.Text, new NumberFormatInfo() { NumberDecimalSeparator = "." });
            double _transY = Convert.ToDouble(dyBox.Text, new NumberFormatInfo() { NumberDecimalSeparator = "." });
            double _transZ = Convert.ToDouble(dzBox.Text, new NumberFormatInfo() { NumberDecimalSeparator = "." });


            sceneObjects[0].TranslationX += _transX;
            sceneObjects[0].TranslationY += _transY;
            sceneObjects[0].TranslationZ += _transZ;



            pictureBox1.Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var dxScale = Convert.ToDouble(textBox1.Text, new NumberFormatInfo() { NumberDecimalSeparator = "."});
            var dyScale = Convert.ToDouble(textBox2.Text, new NumberFormatInfo() { NumberDecimalSeparator = "." });
            var dzScale = Convert.ToDouble(textBox3.Text, new NumberFormatInfo() { NumberDecimalSeparator = "." });

            sceneObjects[0].ScaleX *=  dxScale;
            sceneObjects[0].ScaleY *= dyScale;
            sceneObjects[0].ScaleZ *= dzScale;

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

        private void button9_Click(object sender, EventArgs e)
        {
            string file_name = "D:\\Programming2024\\Computer Graphics\\GitComputerGraphics\\The Cornish Room\\bin\\cubeq2.obj";
            string file_name2 = "D:\\Programming2024\\Computer Graphics\\GitComputerGraphics\\The Cornish Room\\bin\\cubeq2.obj";

            _polyhedron = f.LoadFromOBJ(file_name);
            _polyhedron2 = f.LoadFromOBJ(file_name2);

            //sceneObjects = new List<SceneObject> {

            //    new SceneObject
            //    {
            //        Polyhedron =  _polyhedron,
            //        TranslationX = 0,
            //        TranslationY = 0,
            //        TranslationZ = 0,
            //        RotationX = 0,
            //        RotationY = 0,
            //        RotationZ = 0,
            //        ScaleX = 1,
            //        ScaleY = 1,
            //        ScaleZ = 1
            //    },

            //    new SceneObject
            //    {
            //        Polyhedron =  _polyhedron2,
            //        TranslationX = 0,
            //        TranslationY = 0,
            //        TranslationZ = 0,
            //        RotationX = 0,
            //        RotationY = 0,
            //        RotationZ = 0,
            //        ScaleX = 1,
            //        ScaleY = 1,
            //        ScaleZ = 1
            //    }

            //};
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
            MessageBox.Show(filename);
            _polyhedron = f.LoadFromOBJ(filename);
            _polyhedron2 = _polyhedron;
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
                            
                            sceneObjects.ForEach(q => { q.RotationX += Convert.ToInt32(rotationBox.Text); q.RotationY = 0; q.RotationZ = 0; });
                        break;
                        case 1:
                            _rotationY = Convert.ToInt32(rotationBox.Text);
                            _rotationX = 0;
                            _rotationZ = 0;
                            sceneObjects.ForEach(q=> { q.RotationY += Convert.ToInt32(rotationBox.Text);  q.RotationX = 0; q.RotationZ = 0; });
                            break;
                        case 2:
                            _rotationZ = Convert.ToInt32(rotationBox.Text);
                            _rotationX = 0;
                            _rotationY = 0;
                            sceneObjects.ForEach(q => { q.RotationX = 0; q.RotationY = 0; q.RotationZ += Convert.ToInt32(rotationBox.Text); });
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
            return new Vertex(point[0, 0] / w, point[0, 1] / w, point[0, 2]/ w, w);
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


//Normale.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6
{
    public class Normale
    {
        public double NX { get; set; } //Нормаль по X
        public double NY { get; set; } //Нормаль по Y
        public double NZ { get; set; } //Нормаль по Z
        Normale(double nx, double ny, double nz) 
        {

            NX = nx;
            NY = ny;
            NZ = nz;

        }

    }
}


//RayTracer.cs
using Lab8;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6
{
    internal class RayTracer
    {


        private List<SceneObject> sceneObjects;
        private PointLight light;

        public RayTracer(List<SceneObject> objects, PointLight light)
        {
            this.sceneObjects = objects;
            this.light = light;
        }

        // Метод для выпуска луча и расчета цвета пикселя
        public Color TraceRay(Vertex rayOrigin, Vertex rayDirection)
        {
            // Поиск ближайшего пересечения
            SceneObject closestObject = null;
            double closestDistance = double.MaxValue;
            Vertex intersectionPoint = null;

            foreach (var obj in sceneObjects)
            {
                var intersection = obj.Intersect(rayOrigin, rayDirection);
                if (intersection != null && intersection.Distance < closestDistance)
                {
                    closestDistance = intersection.Distance;
                    closestObject = obj;
                    intersectionPoint = intersection.Point;
                }
            }

            // Если луч не пересекает объекты, возвращаем фоновый цвет
            if (closestObject == null)
                return Color.Black;

            // Расчет освещения
            return CalculateLighting(intersectionPoint, closestObject);
        }

        // Метод для расчета освещения в точке пересечения
        private Color CalculateLighting(Vertex point, SceneObject obj)
        {
            // Вектор от точки пересечения к источнику света
            Vertex lightDirection = (light.Position - point).Normalize();

            // Проверка на тени (если луч от точки к свету пересекает другие объекты)
            bool inShadow = false;
            foreach (var otherObj in sceneObjects)
            {
                if (otherObj != obj && otherObj.Intersect(point, lightDirection) != null)
                {
                    inShadow = true;
                    break;
                }
            }

            // Если точка в тени, возвращаем темный цвет
            if (inShadow)
                return Color.FromArgb((int)(obj.Color.R * 0.2), (int)(obj.Color.G * 0.2), (int)(obj.Color.B * 0.2));

            // Расчет диффузного освещения
            Vertex normal = obj.GetNormal(point); // Нормаль к поверхности
            double diffuse = Math.Max(0, Vertex.Dot(normal, lightDirection));

            // Цвет с учетом освещения
            int r = (int)(obj.Color.R * diffuse * light.Intensity);
            int g = (int)(obj.Color.G * diffuse * light.Intensity);
            int b = (int)(obj.Color.B * diffuse * light.Intensity);

            return Color.FromArgb(r, g, b);
        }

    }
}


//Vertex.cs
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

        //Скалярное произведение
        public static double Dot(Vertex a, Vertex b) 
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }


        // Метод для векторного произведения
        public static Vertex Cross(Vertex a, Vertex b)
        {
            return new Vertex(
                a.Y * b.Z - a.Z * b.Y, // X
                a.Z * b.X - a.X * b.Z, // Y
                a.X * b.Y - a.Y * b.X  // Z
            );
        }

        // Метод для нормализации вектора
        public  Vertex Normalize()
        {
            double length = Math.Sqrt(X * X + Y * Y + Z * Z); // Длина вектора

            if (length == 0)
                throw new InvalidOperationException("Невозможно нормализовать нулевой вектор.");

            // Возвращаем новый нормализованный вектор
            return new Vertex(X / length, Y / length, Z / length);
        }

        public static Vertex operator *(Vertex v, double d) 
        {

            return new Vertex(v.X * d, v.Y * d, v.Z * d);
        }

        public static Vertex operator -(Vertex v1, Vertex v2) 
        {
            return new Vertex(v1.X - v2.X, v1.Y - v2.Y, v2.Z - v2.Z);
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


//SceneObject.cs
using Lab8;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6
{
    internal class SceneObject
    {
        public Polyhedron Polyhedron { get; set; } // Геометрия объекта
        public double TranslationX { get; set; }   // Перемещение по X
        public double TranslationY { get; set; }   // Перемещение по Y
        public double TranslationZ { get; set; }   // Перемещение по Z
        public double RotationX { get; set; }      // Вращение вокруг X
        public double RotationY { get; set; }      // Вращение вокруг Y
        public double RotationZ { get; set; }      // Вращение вокруг Z
        public double ScaleX { get; set; }         // Масштабирование по X
        public double ScaleY { get; set; }         // Масштабирование по Y
        public double ScaleZ { get; set; }         // Масштабирование по Z
        public Color Color { get; set; }


        public IntersectionResult Intersect(Vertex rayOrigin, Vertex rayDirection)
        {
            IntersectionResult closestIntersection = null;
            double closestDistance = double.MaxValue;

            // Перебираем все грани куба
            foreach (var face in Polyhedron.Faces)
            {
                // Получаем нормаль к грани
                Vertex normal = GetFaceNormal(face);

                // Вычисляем пересечение луча с плоскостью грани
                double denominator = Vertex.Dot(normal, rayDirection);

                // Если луч параллелен грани, пересечения нет
                if (Math.Abs(denominator) < 1e-6)
                    continue;

                // Вычисляем расстояние до плоскости
                Vertex pointOnPlane = face.Vertices[0]; // Любая точка на грани
                double t = Vertex.Dot(pointOnPlane - rayOrigin, normal) / denominator;

                // Если пересечение за лучом, игнорируем
                if (t < 0)
                    continue;

                // Точка пересечения
                Vertex intersectionPoint = rayOrigin + rayDirection * t;

                // Проверяем, лежит ли точка внутри грани
                if (IsPointInsideFace(intersectionPoint, face))
                {
                    // Если это ближайшее пересечение, сохраняем его
                    if (t < closestDistance)
                    {
                        closestDistance = t;
                        closestIntersection = new IntersectionResult
                        {
                            Point = intersectionPoint,
                            Distance = t
                        };
                    }
                }
            }

            return closestIntersection;
        }

        // Метод для вычисления нормали к грани
        private Vertex GetFaceNormal(Face face)
        {
            Vertex v0 = face.Vertices[0];
            Vertex v1 = face.Vertices[1];
            Vertex v2 = face.Vertices[2];

            Vertex edge1 = v1 - v0;
            Vertex edge2 = v2 - v0;

            // Вычисляем векторное произведение
            Vertex norml = Vertex.Cross(edge1, edge2);

            // Проверяем, не является ли вектор нулевым
            double length = Math.Sqrt(norml.X * norml.X + norml.Y * norml.Y + norml.Z * norml.Z);
            if (length < 1e-6) // Пороговое значение для проверки нулевого вектора
            {
                // Возвращаем вектор по умолчанию или выбрасываем исключение
                return new Vertex(0, 0, 1);
                //throw new InvalidOperationException("Невозможно вычислить нормаль: ребра грани коллинеарны.");
                // Или вернуть вектор по умолчанию:
                // return new Vertex(0, 0, 1);
            }

            // Нормализуем вектор
            return norml.Normalize();
        }

        // Метод для проверки, лежит ли точка внутри грани
        private bool IsPointInsideFace(Vertex point, Face face)
        {
            // Простейшая реализация: проверяем, лежит ли точка внутри выпуклого многоугольника
            // (Этот метод можно улучшить для более сложных случаев)
            Vertex normal = GetFaceNormal(face);

            for (int i = 0; i < face.Vertices.Count; i++)
            {
                Vertex v1 = face.Vertices[i];
                Vertex v2 = face.Vertices[(i + 1) % face.Vertices.Count];

                Vertex edge = v2 - v1;
                Vertex toPoint = point - v1;

                Vertex cross = Vertex.Cross(edge, toPoint);

                if (Vertex.Dot(cross, normal) < 0)
                    return false; // Точка находится за пределами грани
            }

            return true; // Точка внутри грани
        }

        public Vertex GetNormal(Vertex point)
        {
            // Нормаль к поверхности сферы
            Vertex center = Polyhedron.Centroid(Polyhedron.LocalToWorld);
            return (point - center).Normalize();
        }
    }

        public class IntersectionResult
        {
            public Vertex Point { get; set; }
            public double Distance { get; set; }
        }



}

//ZBufferRenderer.cs
using Lab8;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6
{
    public class ZBufferRenderer
    {
        private readonly int width;
        private readonly int height;
        private readonly double[,] zBuffer;
        private readonly Bitmap renderBitmap;
        Projection projectionFunction;
        public ZBufferRenderer(int width, int height, Projection projectionFunc)
        {
            this.width = width;
            this.height = height;
            projectionFunction = projectionFunc;
            zBuffer = new double[width, height];
            this.renderBitmap = new Bitmap(width, height);
            ClearBuffer();
        }

        public void ClearBuffer()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    zBuffer[x, y] = double.MaxValue;
                    renderBitmap.SetPixel(x, y, Color.White); // Очистка Bitmap 
                }
            }
        }

        public void RenderFace(Face face, Matrix transformationMatrix, Pen pen)
        {
            // Преобразование вершин в мировые координаты
            var transformedVertices = face.Vertices.Select(v => Transformer.TransformToWorld(v, transformationMatrix, projectionFunction)).ToList();

            // Растеризация треугольников (каждая грань разбивается на треугольники)
            for (int i = 1; i < transformedVertices.Count - 1; i++)
            {
                RasterizeTriangle( transformedVertices[0], transformedVertices[i], transformedVertices[i + 1], pen);
            }
        }

        private void RasterizeTriangle(Vertex v1, Vertex v2, Vertex v3, Pen pen)
        {
            // Найти границы треугольника на экране
            int minX = (int)Math.Min(Math.Min(v1.X, v2.X), v3.X);
            int maxX = (int)Math.Max(Math.Max(v1.X, v2.X), v3.X);
            int minY = (int)Math.Min(Math.Min(v1.Y, v2.Y), v3.Y);
            int maxY = (int)Math.Max(Math.Max(v1.Y, v2.Y), v3.Y);

            // Ограничить область отрисовки экраном
            minX = Math.Max(minX, 0);
            maxX = Math.Min(maxX, width - 1);
            minY = Math.Max(minY, 0);
            maxY = Math.Min(maxY, height - 1);

            // Растеризация пикселей в треугольнике
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (IsPointInTriangle(x, y, v1, v2, v3, out double z))
                    {
                        // Z-буферная проверка
                        if (z < zBuffer[x, y])
                        {
                            zBuffer[x, y] = z;
                            //graphics.DrawRectangle(pen, x, y, 1, 1); // Отрисовка пикселя
                            renderBitmap.SetPixel(x, y, pen.Color); // Устанавливаем цвет пикселя
                        }
                    }
                }
            }
        }

        private bool IsPointInTriangle(int px, int py, Vertex v1, Vertex v2, Vertex v3, out double z)
        {
            // Проверка, находится ли точка внутри треугольника, и интерполяция Z
            z = double.MaxValue;
            var a = v1;
            var b = v2;
            var c = v3;

            double alpha = ((b.Y - c.Y) * (px - c.X) + (c.X - b.X) * (py - c.Y)) /
                           ((b.Y - c.Y) * (a.X - c.X) + (c.X - b.X) * (a.Y - c.Y));
            double beta = ((c.Y - a.Y) * (px - c.X) + (a.X - c.X) * (py - c.Y)) /
                          ((b.Y - c.Y) * (a.X - c.X) + (c.X - b.X) * (a.Y - c.Y));
            double gamma = 1.0 - alpha - beta;

            if (alpha >= 0 && beta >= 0 && gamma >= 0)
            {
                z = alpha * v1.Z + beta * v2.Z + gamma * v3.Z;
                return true;
            }

            return false;
        }

        // Метод для отрисовки Bitmap на целевую поверхность
        public void DrawToGraphics(Graphics graphics)
        {
            graphics.DrawImage(renderBitmap, 0, 0); // Отрисовка Bitmap на Graphics
        }

    }

}


//Camera.cs
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

        //Скалярное произведение
        private static double DotProduct(Vertex a, Vertex b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }



    }

}


//Face.cs
using Lab6;
using System.Collections.Generic;

namespace Lab8
{
    public class Face
    {

        public List<Vertex> Vertices { get; private set; }
        public List<Vertex> Normales{get; set;}

        public Face(List<Vertex> vertices)
        {
            Vertices = vertices;
            Normales = new List<Vertex>();
        }


    }



}

//Normale.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6
{
    public class Normale
    {
        public double NX { get; set; } //Нормаль по X
        public double NY { get; set; } //Нормаль по Y
        public double NZ { get; set; } //Нормаль по Z
        Normale(double nx, double ny, double nz) 
        {

            NX = nx;
            NY = ny;
            NZ = nz;

        }

    }
}


