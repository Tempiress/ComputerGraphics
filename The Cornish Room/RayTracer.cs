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

        private bool IsInShadow(Vertex point, SceneObject currentObject)
        {
            Vertex lightDirection = (light.Position - point).Normalize();

            foreach (var obj in sceneObjects)
            {
                if (obj != currentObject)
                {
                    var intersection = obj.Intersect(point, lightDirection);
                    if (intersection != null)
                    {
                        return true; // Тень есть
                    }
                }
            }

            return false; // Тени нет
        }

        private Color CalculateLighting(Vertex point, SceneObject obj)
        {
            Vertex lightDirection = (light.Position - point).Normalize();
            Vertex normal = obj.GetNormal(point);

            // Диффузное освещение
            double diffuse = Math.Max(0, Vertex.Dot(normal, lightDirection));

            // Тени
            if (IsInShadow(point, obj))
            {
                diffuse *= 0.2; // Уменьшаем интенсивность света в тени
            }

            // Цвет с учетом освещения
            int r = (int)(obj.Color.R * diffuse * light.Intensity);
            int g = (int)(obj.Color.G * diffuse * light.Intensity);
            int b = (int)(obj.Color.B * diffuse * light.Intensity);

            return Color.FromArgb(r, g, b);
        }

    }
}
