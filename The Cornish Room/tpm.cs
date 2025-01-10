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
        private Camera _camera = new Camera
        {
            Position = new Vertex(0, 1, 1), // Камера расположена в пространстве
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

        List<SceneObject> sceneObjects = new List<SceneObject>
        {
        // Комната (куб)
        new SceneObject
        {
            Polyhedron = Polyhedron.Tetrahedron(50), // Комната размером 10x10x10
            TranslationX = 5,            // Центр комнаты в начале координат
            TranslationY = 5,
            TranslationZ = 5,
            RotationX = 2,
            RotationY = 2,
            RotationZ = 2,
            ScaleX = 1,
            ScaleY = 1,
            ScaleZ = 1,
            Color = Color.Red,

        },
        // Объект внутри комнаты (например, сфера или куб)
        new SceneObject
        {
            Polyhedron = Polyhedron.Tetrahedron(40), // Пример объекта (тетраэдр)
            TranslationX = 2,                     // Объект в центре комнаты
            TranslationY = 2,
            TranslationZ = 2,
            RotationX = 0,
            RotationY = 0,
            RotationZ = 0,
            ScaleX = 0.5,                         // Масштабируем объект
            ScaleY = 0.5,
            ScaleZ = 0.5,
            Color = Color.Beige,
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


            //sceneObjects[0].TranslationX += centerX;
            //sceneObjects[0].TranslationY += centerY;

            //sceneObjects[1].TranslationX += centerX - 100;
            //sceneObjects[1].TranslationY += centerY - 100;
            //sceneObjects[1].TranslationZ = -5;

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

            sceneObjects[0].CheckObjectGeometry();
            Console.WriteLine($"Object Center: ({sceneObjects[0].TranslationX}, {sceneObjects[0].TranslationY}, {sceneObjects[0].TranslationZ})");

            Console.WriteLine($"Camera Position: {_camera.Position}");


            Vertex rayDirection2 = CalculateRayDirection(0, 0);
            Console.WriteLine($"Top-left ray direction: ({rayDirection2.X}, {rayDirection2.Y}, {rayDirection2.Z})");

            rayDirection2 = CalculateRayDirection(pictureBox1.Width - 1, pictureBox1.Height - 1);
            Console.WriteLine($"Bottom-right ray direction: ({rayDirection2.X}, {rayDirection2.Y}, {rayDirection2.Z})");

            // Создаем Bitmap для отрисовки
            Bitmap renderBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            RayTracer rayTracer = new RayTracer(sceneObjects, light);

            // Расчет освещения с использованием RayTracer
            for (int y = 0; y < pictureBox1.Height; y++)
            {
                for (int x = 0; x < pictureBox1.Width; x++)
                {


                    Vertex rayOrigin = _camera.Position;
                    Vertex rayDirection = CalculateRayDirection(x, y);

                    Color pixelColor = rayTracer.TraceRay(rayOrigin, rayDirection, _camera);

                    // Устанавливаем цвет пикселя на Bitmap
                    renderBitmap.SetPixel(x, y, pixelColor);

                }
            }

            // Отрисовка результата на экран
            e.Graphics.DrawImage(renderBitmap, 0, 0);

        }


        private Vertex CalculateRayDirection(int x, int y)
        {
            double fov = Math.PI / 4; // Угол обзора
            double aspectRatio = (double)pictureBox1.Width / (double)pictureBox1.Height;

            double px = (2 * ((x + 0.5) / (double)pictureBox1.Width) - 1) * Math.Tan(fov / 2) * aspectRatio;
            double py = (1 - 2 * ((y + 0.5) / (double)pictureBox1.Height)) * Math.Tan(fov / 2);

            Vertex dir = new Vertex(px, py, -1).Normalize();
            //------Console.WriteLine($"Ray Direction3: ({dir.X}, {dir.Y}, {dir.Z})");

            return dir; // Направление луча
           
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

        //Скалярное произведение
        private static double DotProduct(Vertex a, Vertex b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }



    }

}


//camera.cs
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

//matrix.cs
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


//normale.cs
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


        public static Polyhedron Tetrahedron(int index_size)
        {
            var v1 = new Vertex(1 * index_size, 1 * index_size, 1 );
            var v2 = new Vertex(1 * index_size  , -1 * index_size, -1  );
            var v3 = new Vertex(-1 * index_size, 1 * index_size, -1  );
            var v4 = new Vertex(-1 * index_size, -1 * index_size, 1  ) ;

            Face firstPol = new Face(new List<Vertex> { v1, v2, v3 });
            Face secondPol = new Face(new List<Vertex> { v1, v2, v4 });
            Face thirdPol = new Face(new List<Vertex> {v1, v3, v4 });
            Face fourthPol = new Face(new List<Vertex> {v2, v3, v4 });

            return new Polyhedron(new List<Vertex> { v1, v2, v3, v4 }, new List<Face> {firstPol, secondPol, thirdPol, fourthPol });
        }

        public static Polyhedron CreateRoom(double size)
        {
            double halfSize = size / 2;
            var vertices = new List<Vertex>
        {
            new Vertex(-halfSize, -halfSize, -halfSize), // 0
            new Vertex(halfSize, -halfSize, -halfSize),  // 1
            new Vertex(-halfSize, halfSize, -halfSize),  // 2
            new Vertex(halfSize, halfSize, -halfSize),   // 3
            new Vertex(-halfSize, -halfSize, halfSize),  // 4
            new Vertex(halfSize, -halfSize, halfSize),   // 5
            new Vertex(-halfSize, halfSize, halfSize),   // 6
            new Vertex(halfSize, halfSize, halfSize)     // 7
        };

                var faces = new List<Face>
        {
            //new Face(new List<Vertex> { vertices[0], vertices[1], vertices[3], vertices[2] }), // Задняя грань
            new Face(new List<Vertex> { vertices[4], vertices[5], vertices[7], vertices[6] }), // Передняя грань
            new Face(new List<Vertex> { vertices[0], vertices[1], vertices[5], vertices[4] }), // Нижняя грань
            new Face(new List<Vertex> { vertices[2], vertices[3], vertices[7], vertices[6] }), // Верхняя грань
            new Face(new List<Vertex> { vertices[0], vertices[2], vertices[6], vertices[4] }), // Левая грань
            new Face(new List<Vertex> { vertices[1], vertices[3], vertices[7], vertices[5] })  // Правая грань
        };

            return new Polyhedron(vertices, faces);
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
        public Color TraceRay(Vertex rayOrigin, Vertex rayDirection, Camera camera)
        {
            // Поиск ближайшего пересечения
            SceneObject closestObject = null;
            double closestDistance = double.MaxValue;
            Vertex intersectionPoint = null;




            foreach (var obj in sceneObjects)
            {
                var intersection = obj.Intersect(rayOrigin, rayDirection, camera);
                if(intersection != null) Console.WriteLine($"Intersection found with object: {obj.Color} at distance {intersection.Distance}");
                if (intersection != null && intersection.Distance < closestDistance)
                {

                    

                    closestDistance = intersection.Distance;
                    closestObject = obj;
                    intersectionPoint = intersection.Point;
                }
            }


            // Если луч не пересекает объекты, возвращаем фоновый цвет
            if (closestObject == null) 
            {
                //Console.WriteLine($"No intersection. Returning background color.");
                return Color.Black;
            }

            //Console.WriteLine($"Intersection with object of color {closestObject.Color}");


            // Расчет освещения
            return CalculateLighting(intersectionPoint, closestObject, camera);
        }

        private bool IsInShadow(Vertex point, SceneObject currentObject, Camera camera)
        {
            Vertex lightDirection = (light.Position - point).Normalize();

            foreach (var obj in sceneObjects)
            {
                if (obj != currentObject)
                {
                    var intersection = obj.Intersect(point, lightDirection, camera);
                    if (intersection != null)
                    {
                        return true; // Тень есть
                    }
                }
            }

            return false; // Тени нет
        }

        private Color CalculateLighting(Vertex point, SceneObject obj, Camera camera)
        {
            Vertex lightDirection = (light.Position - point).Normalize();
            Vertex normal = obj.GetNormal(point);

            // Диффузное освещение
            double diffuse = Math.Max(0, Vertex.Dot(normal, lightDirection));

            // Тени
            if (IsInShadow(point, obj, camera))
            {
                diffuse *= 0.2; // Уменьшаем интенсивность света в тени
            }
            

            // Цвет с учетом освещения
            int r = (int)(obj.Color.R * diffuse * light.Intensity);
            int g = (int)(obj.Color.G * diffuse * light.Intensity);
            int b = (int)(obj.Color.B * diffuse * light.Intensity);

            Console.WriteLine($"Lighting for point ({point.X}, {point.Y}, {point.Z}): {Color.FromArgb(r, g, b)}");

            return Color.FromArgb(r, g, b);
        }

    }
}




//sceneobject.cs
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


        public IntersectionResult Intersect(Vertex rayOrigin, Vertex rayDirection, Camera camera)
        {
            IntersectionResult closestIntersection = null;
            double closestDistance = double.MaxValue;

            // Перебираем все грани куба
            foreach (var face in Polyhedron.Faces)
            {
                // Получаем нормаль к грани
                Vertex normal = GetFaceNormal(face, camera);
                //------------Console.WriteLine($"Face Normal: ({normal.X}, {normal.Y}, {normal.Z})");

                //Console.WriteLine($"Normal: ({normal.X}, {normal.Y}, {normal.Z})");
                //Console.WriteLine($"Ray direction: ({rayDirection.X}, {rayDirection.Y}, {rayDirection.Z})");

                // Инвертируем нормаль, если она направлена не в сторону камеры
                if (Vertex.Dot(rayDirection, normal) > 0)
                {
                    normal = new Vertex(-normal.X, -normal.Y, -normal.Z);
                }

                // Вычисляем пересечение луча с плоскостью грани
                double denominator = Vertex.Dot(normal, rayDirection);

                //Console.WriteLine($"denominator is: {denominator}");
                // Если луч параллелен грани, пересечения нет
                if (Math.Abs(denominator) < 1e-6) 
                {
                    //------------Console.WriteLine("Ray is parallel to the plane");
                    continue;
                }
                    

                // Вычисляем расстояние до плоскости
                Vertex pointOnPlane = face.Vertices[0]; // Любая точка на грани
                double t = Vertex.Dot(pointOnPlane - rayOrigin, normal) / denominator;
                //-------Console.WriteLine($"RayOrigin: ({rayOrigin.X}, {rayOrigin.Y}, {rayOrigin.Z}), RayDirection: ({rayDirection.X}, {rayDirection.Y}, {rayDirection.Z}), t = {t}");
                // Если пересечение за лучом, игнорируем
                if (t <= 1e-6)
                    continue;

                // Точка пересечения
                Vertex intersectionPoint = rayOrigin + rayDirection * t;

                bool isInside = IsPointInsideFace(intersectionPoint, face, camera);
                //---------Console.WriteLine($"Intersection Point: ({intersectionPoint.X}, {intersectionPoint.Y}, {intersectionPoint.Z}), Is Inside: {isInside}");

                // Проверяем, лежит ли точка внутри грани
                if (IsPointInsideFace(intersectionPoint, face, camera))
                {
                    //Console.WriteLine($"Intersection found at distance {t}");
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

            if (closestIntersection == null)
            {
                //--------Console.WriteLine("No intersection found");
            }
            else
            {
                //----------Console.WriteLine($"Intersection found at distance {closestIntersection.Distance}");
            }


            return closestIntersection;
        }

        public void CheckObjectGeometry()
        {
            foreach (var face in Polyhedron.Faces)
            {
                Vertex v0 = face.Vertices[0];
                Vertex v1 = face.Vertices[1];
                Vertex v2 = face.Vertices[2];

                Vertex edge1 = v1 - v0;
                Vertex edge2 = v2 - v0;

                Vertex normal = Vertex.Cross(edge1, edge2);
                double length = Math.Sqrt(normal.X * normal.X + normal.Y * normal.Y + normal.Z * normal.Z);

                if (length < 1e-6)
                {
                    //---------Console.WriteLine($"Degenerate face detected: v0=({v0.X}, {v0.Y}, {v0.Z}), v1=({v1.X}, {v1.Y}, {v1.Z}), v2=({v2.X}, {v2.Y}, {v2.Z})");
                }
            }
        }

        // Метод для вычисления нормали к грани
        private Vertex GetFaceNormal(Face face, Camera _camera)
        {
            Vertex v0 = face.Vertices[0];
            Vertex v1 = face.Vertices[1];
            Vertex v2 = face.Vertices[2];

            //Console.WriteLine($"Face vertices: v0=({v0.X}, {v0.Y}, {v0.Z}), v1=({v1.X}, {v1.Y}, {v1.Z}), v2=({v2.X}, {v2.Y}, {v2.Z})");

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
            norml = norml.Normalize();
            //Console.WriteLine($"Computed normal: ({norml.X}, {norml.Y}, {norml.Z})");

            // Проверяем направление нормали
            Vertex centroid = (v0 + v1 + v2) / 3; // Центр грани
            Vertex toCentroid = centroid - _camera.Position; // Вектор от камеры к центру грани

            // Если нормаль направлена внутрь, инвертируем её
            if (Vertex.Dot(norml, toCentroid) < 0)
            {
                norml = new Vertex(-norml.X, -norml.Y, -norml.Z);
            }


            return norml;
        }

        // Метод для проверки, лежит ли точка внутри грани
        private bool IsPointInsideFace(Vertex point, Face face, Camera camera)
        {
            // Простейшая реализация: проверяем, лежит ли точка внутри выпуклого многоугольника
            // (Этот метод можно улучшить для более сложных случаев)
            Vertex normal = GetFaceNormal(face, camera);

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

        //��������� ������������
        public static double Dot(Vertex a, Vertex b) 
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }


        // ����� ��� ���������� ������������
        public static Vertex Cross(Vertex a, Vertex b)
        {
            return new Vertex(
                a.Y * b.Z - a.Z * b.Y, // X
                a.Z * b.X - a.X * b.Z, // Y
                a.X * b.Y - a.Y * b.X  // Z
            );
        }

        // ����� ��� ������������ �������
        public  Vertex Normalize()
        {
            double length = Math.Sqrt(X * X + Y * Y + Z * Z); // ����� �������

            if (length == 0)
                throw new InvalidOperationException("���������� ������������� ������� ������.");

            // ���������� ����� ��������������� ������
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
            if (scalar == 0)throw new DivideByZeroException("������� �� ����!");

            return new Vertex(v.X / scalar, v.Y / scalar, v.Z / scalar);
        }

        //public static Vertex operator/=(Vertex v, double scalar)
        //{

        //    if (scalar == 0) 
        //    {
        //        throw new DivideByZeroException("������� �� ����! � ����� �� ������? :)");
        //    }

        //    v.X /= scalar;
        //    v.Y /= scalar;
        //    v.Z /= scalar;
        //    return v;
        //}
    }
}