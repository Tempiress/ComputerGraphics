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
