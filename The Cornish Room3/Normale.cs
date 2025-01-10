using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6
{
    public class Normale
    {
        public double NX { get; set; } //Нормаль по X
        public double NY { get; set; } //Нормаль по Y
        public double NZ { get; set; } //Нормаль по Z
        Normale(double nx, double ny, double nz) 
        {

            NX = nx;
            NY = ny;
            NZ = nz;

        }

    }
}
