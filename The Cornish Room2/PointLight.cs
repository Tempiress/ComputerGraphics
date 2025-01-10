using Lab8;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6
{
    internal class PointLight
    {

        public Vertex Position { get; set; } // Позиция источника света
        public Color Color { get; set; }     // Цвет света
        public double Intensity { get; set; } // Интенсивность света

    }
}
