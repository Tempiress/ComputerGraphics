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
