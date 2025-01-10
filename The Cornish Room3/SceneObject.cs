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


                      
    }

   
}

