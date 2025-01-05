using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Lab8
{
    public static class FigureBuilder
    {
        // Часть 2: Построение фигуры вращения
        public static Polyhedron BuildRevolutionFigure(List<Vertex> profile, string axis, int divisions)
        {
            double angleStep = 360.0 / divisions;
            List<Vertex> vertices = new List<Vertex>();
            List<Face> faces = new List<Face>();

            foreach (var angle in Enumerable.Range(0, divisions + 1).Select(i => i * angleStep))
            {
                double radians = angle * Math.PI / 180;
                foreach (var vertex in profile)
                {
                    switch (axis.ToLower())
                    {
                        case "x":
                            vertices.Add(new Vertex(
                                Math.Round(vertex.X, 3),
                                Math.Round(vertex.Y * Math.Cos(radians) - vertex.Z * Math.Sin(radians), 3),
                                Math.Round(vertex.Y * Math.Sin(radians) + vertex.Z * Math.Cos(radians), 3)));
                            break;
                        case "y":
                            vertices.Add(new Vertex(
                                Math.Round(vertex.X * Math.Cos(radians) + vertex.Z * Math.Sin(radians), 3),
                                Math.Round(vertex.Y, 3),
                                Math.Round(-vertex.X * Math.Sin(radians) + vertex.Z * Math.Cos(radians), 3)));
                            break;
                        case "z":
                            vertices.Add(new Vertex(
                                Math.Round(vertex.X * Math.Cos(radians) - vertex.Y * Math.Sin(radians), 3),
                                Math.Round(vertex.X * Math.Sin(radians) + vertex.Y * Math.Cos(radians), 3),
                                Math.Round(vertex.Z, 3)));
                            break;
                    }
                }
            }

            // Создание граней
            int profileCount = profile.Count;
            for (int i = 0; i < divisions; i++)
            {
                for (int j = 0; j < profileCount - 1; j++)
                {
                    int current = i * profileCount + j;
                    int next = (i + 1) * profileCount + j;

                    faces.Add(new Face(new List<Vertex> { vertices[current], vertices[current + 1], vertices[next + 1], vertices[next] }));
                }
            }

            return new Polyhedron(vertices, faces);
        }

        // Часть 3: Построение графика функции
        public static Polyhedron BuildSurface(Func<double, double, double> func, double x0, double x1, double y0, double y1, int divisions)
        {
            double xStep = (x1 - x0) / divisions;
            double yStep = (y1 - y0) / divisions;
            List<Vertex> vertices = new List<Vertex>();
            List<Face> faces = new List<Face>();

            for (int i = 0; i <= divisions; i++)
            {
                for (int j = 0; j <= divisions; j++)
                {
                    double x = x0 + i * xStep;
                    double y = y0 + j * yStep;
                    double z = func(x, y);
                    vertices.Add(new Vertex(Math.Round(x, 3), Math.Round(y, 3), Math.Round(z, 3)));
                }
            }

            for (int i = 0; i < divisions; i++)
            {
                for (int j = 0; j < divisions; j++)
                {
                    int current = i * (divisions + 1) + j;
                    int next = (i + 1) * (divisions + 1) + j;

                    faces.Add(new Face(new List<Vertex> { vertices[current], vertices[current + 1], vertices[next + 1], vertices[next] }));
                }
            }

            return new Polyhedron(vertices, faces);
        }
    }

    // Пример использования
    //public class Example
    //{
    //    public static void Main()
    //    {
    //        // Пример фигуры вращения
    //        var profile = new List<Vertex> {
    //            new Vertex(0, 0, 0),
    //            new Vertex(0, 1, 0),
    //            new Vertex(0, 2, 1),
    //        };

    //        var revolutionFigure = FigureBuilder.BuildRevolutionFigure(profile, "z", 12);
    //        var fileAction = new fileaction();
    //        fileAction.SaveToOBJ(revolutionFigure, "revolution.obj");

    //        // Пример графика функции
    //        Func<double, double, double> func = (x, y) => Math.Sin(Math.Sqrt(x * x + y * y));
    //        var surface = FigureBuilder.BuildSurface(func, -5, 5, -5, 5, 20);
    //        fileAction.SaveToOBJ(surface, "surface.obj");
    //    }
    //}
}
