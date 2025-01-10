using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ray
{
    public partial class Form1 : Form
    {
        private const int ImageWidth = 800;
        private const int ImageHeight = 600;

        public Form1()
        {
            InitializeComponent();
            this.Text = "RayTracing Demo";
            this.ClientSize = new Size(ImageWidth, ImageHeight);

            var pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                Image = RenderScene()
            };

            this.Controls.Add(pictureBox);
        }

        private Bitmap RenderScene()
        {
            Bitmap bitmap = new Bitmap(ImageWidth, ImageHeight);
            Vector3 cameraPos = new Vector3(0, 0, 6); // Позиция камеры
            float viewportSize = 1; // Размер виртуального экрана
            float projectionPlaneZ = 0; // Z-координата проекционной плоскости
            Cube cube = new Cube(new Vector3(0, 0, 0), 1, Color.Red); // Куб в центре

            for (int y = 0; y < ImageHeight; y++)
            {
                for (int x = 0; x < ImageWidth; x++)
                {
                    // Нормализуем координаты пикселя
                    float px = (x - ImageWidth / 2f) * viewportSize / ImageWidth;
                    float py = -(y - ImageHeight / 2f) * viewportSize / ImageHeight;

                    // Создаём луч из камеры через пиксель
                    Ray ray = new Ray(cameraPos, new Vector3(px, py, projectionPlaneZ) - cameraPos);

                    // Трассировка луча
                    Color pixelColor = TraceRay(ray, cube);
                    bitmap.SetPixel(x, y, pixelColor);
                }
            }

            return bitmap;
        }

        private Color TraceRay(Ray ray, Cube cube)
        {
            if (cube.Intersect(ray, out float t))
            {
                // Освещение: Чем ближе точка, тем ярче
                float brightness = 1 - Math.Min(t / 10f, 1f);
                return ScaleColor(cube.Color, brightness);
            }

            return Color.Black; // Фон
        }

        private Color ScaleColor(Color color, float factor)
        {
            return Color.FromArgb(
                (int)(color.R * factor),
                (int)(color.G * factor),
                (int)(color.B * factor)
            );
        }
    }

    public class Vector3
    {
        public float X, Y, Z;

        public Vector3(float x, float y, float z) => (X, Y, Z) = (x, y, z);



        // Добавляем индексатор для доступа к компонентам через [0], [1], [2]
        public float this[int index]
        {
            get
            {
                switch (index) 
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;

                    case 2:
                        return Z;

                    default:
                        throw new IndexOutOfRangeException();
                       
                }
            }
            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    default: throw new IndexOutOfRangeException("Index must be 0, 1, or 2.");
                }
            }
        }



        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3 operator *(Vector3 a, float scalar) => new Vector3(a.X * scalar, a.Y * scalar, a.Z * scalar);
        public float Dot(Vector3 v) => X * v.X + Y * v.Y + Z * v.Z;
        public float Magnitude() => (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        public Vector3 Normalize() => this * (1 / Magnitude());
    }

    public class Ray
    {
        public Vector3 Origin, Direction;
        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction.Normalize();
        }
    }

    public class Cube
    {
        public Vector3 Center;
        public float Size;
        public Color Color;

        public Cube(Vector3 center, float size, Color color)
        {
            Center = center;
            Size = size;
            Color = color;
        }

        public bool Intersect(Ray ray, out float t)
        {
            // Алгоритм проверки пересечения с каждой гранью куба
            float tMin = float.NegativeInfinity, tMax = float.PositiveInfinity;
            Vector3 min = Center - new Vector3(Size / 2, Size / 2, Size / 2);
            Vector3 max = Center + new Vector3(Size / 2, Size / 2, Size / 2);

            for (int i = 0; i < 3; i++) // По всем осям (X, Y, Z)
            {
                float t1 = (min[i] - ray.Origin[i]) / ray.Direction[i];
                float t2 = (max[i] - ray.Origin[i]) / ray.Direction[i];

                if (t1 > t2) (t1, t2) = (t2, t1);

                tMin = Math.Max(tMin, t1);
                tMax = Math.Min(tMax, t2);

                if (tMin > tMax)
                {
                    t = 0;
                    return false;
                }
            }

            t = tMin;
            return true;
        }
    }


}
