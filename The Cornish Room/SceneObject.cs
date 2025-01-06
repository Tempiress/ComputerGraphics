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

