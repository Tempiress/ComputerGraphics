using Lab8;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab8
{
    internal class fileaction
    {

        public Polyhedron LoadFromOBJ(string filePath)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<Face> faces = new List<Face>();
            List<Vertex> normales = new List<Vertex>();
            
            foreach (var line in File.ReadLines(filePath))
            {
                if (line.StartsWith("v "))
                {
                    var parts = line.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                    double x = double.Parse(parts[1], CultureInfo.InvariantCulture);
                    double y = double.Parse(parts[2], CultureInfo.InvariantCulture);
                    double z = double.Parse(parts[3], CultureInfo.InvariantCulture);
                    vertices.Add(new Vertex(x, y, z));
                }

                else if (line.StartsWith("vn "))
                {
                    var normalesParts = line.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                    double nx = double.Parse(normalesParts[1], CultureInfo.InvariantCulture);
                    double ny = double.Parse(normalesParts[2], CultureInfo.InvariantCulture);
                    double nz = double.Parse(normalesParts[3], CultureInfo.InvariantCulture);
                    normales.Add(new Vertex(nx, ny, nz));
                }

                //else if (line.StartsWith("vt ")) 
                //{
                //    var texturesParts = line.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                //    double n1 = double.Parse(texturesParts[1], CultureInfo.InvariantCulture);
                //    double n2 = double.Parse(texturesParts[2], CultureInfo.InvariantCulture);
                //    double n3 = double.Parse(texturesParts[3], CultureInfo.InvariantCulture);
                //    normales.Add(new Vertex(n1, n2, n3));
                //}

                else if (line.StartsWith("f "))
                {
                    var partsFace = line.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);

                    var faceVetices = new List<Vertex>();
                    var faceNormales = new List<Vertex>(); // для нормалей
                    //foreach (var p in parts) 
                    //{
                    //    vv[i] = vertices[int.Parse(p)];
                    //    i++;

                    //}
                  
                    for (int j = 1; j < partsFace.Length; j++)
                    {
                        var indices  = partsFace[j].Split('/', (char)StringSplitOptions.RemoveEmptyEntries);
                        // Извлечение индекса вершины
                        int vertexIndex = int.Parse(indices[0]) - 1;
                        faceVetices.Add(vertices[vertexIndex]);

                        //извлечение индекса нормали, если есть
                        if (indices.Length > 1) 
                        {
                            faceNormales.Add(normales[int.Parse(indices[1]) - 1]);
                        }
                        //tempListVertex.Add(vertices[int.Parse(partsFace[j]) - 1]);
                       

                    }
                    var face = new Face(faceVetices);
                    if (faceNormales.Count > 0) face.Normales = faceNormales; // Добавляем нормали в грань
                    faces.Add(face);


                    // List<Face> faceVertices = parts.Skip(1).Select(index => vertices[int.Parse(index)]);
                    //                                 //.Select(index => vertices[int.Parse(index) - 1])
                    //                                 //.ToList();
                    //faces.Add(new Face(faceVertices));
                }
            }
           
            return new Polyhedron(vertices, faces);
        }

        public void SaveToOBJ(Polyhedron polyhedron, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var vertex in polyhedron.Vertices)
                {
                    writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "v {0:0.###} {1:0.###} {2:0.###}", vertex.X, vertex.Y, vertex.Z));
                    //writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "v {0:0.###} {1:0.###} {2:0.###}", vertex.NX, vertex.NY, vertex.NZ));

                }

                foreach (var face in polyhedron.Faces)
                {
                    var indices = face.Vertices.Select(v => polyhedron.Vertices.IndexOf(v) + 1);
                    writer.WriteLine("f " + string.Join(" ", indices));
                }
            }
        }
    }
}

//private void LoadFile_Click(object sender, EventArgs e)
//{
//    OpenFileDialog openFileDialog = new OpenFileDialog();
//    if (openFileDialog.ShowDialog() == DialogResult.OK)
//    {

//        pop = LoadFromOBJ(openFileDialog.FileName);
//        pnts = pop.Faces;
//        pictureBox1.Invalidate();
//    }
//}





//    }
//}
