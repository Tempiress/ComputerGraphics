using Lab6;
using System.Collections.Generic;

namespace Lab8
{
    public class Face
    {

        public List<Vertex> Vertices { get; private set; }
        public List<Vertex> Normales{get; set;}

        public Face(List<Vertex> vertices)
        {
            Vertices = vertices;
            Normales = new List<Vertex>();
        }


    }



}