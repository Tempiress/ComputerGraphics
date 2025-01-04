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
            return new Matrix(new[,]
            {
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
