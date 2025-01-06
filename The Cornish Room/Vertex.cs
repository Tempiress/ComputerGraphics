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