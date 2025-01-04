private void pictureBox1_Paint(object sender, PaintEventArgs e)
{
    if (_polyhedron == null)
        return;

    // Направление обзора
    var viewDirection = new Vertex(0, 0, -1);

    // Получаем только видимые грани
    var _visiblePolyhedron = BackfaceCulling(_polyhedron, viewDirection);

    Matrix translationMatrix = TranslationMatrix(_translationX, _translationY, _translationZ);
    Matrix scalingMatrix = ScalingMatrix(_scaleX, _scaleY, _scaleZ);
    Matrix rotationMatrix = RotationMatrix(_rotationX, _rotationY, _rotationZ);
    Matrix lrotation = LRotation(_fi, _l, _m, _n);
    Vertex centroid = _polyhedron.Centroid(_polyhedron.LocalToWorld);

    Matrix toCenter = TranslationMatrix(-centroid.X, -centroid.Y, -centroid.Z);
    Matrix fromCenter = TranslationMatrix(centroid.X, centroid.Y, centroid.Z);

    Matrix worldMatrix;
    if (!IsCentroid)
    {
        IsCentroid = true;
        worldMatrix = translationMatrix * scalingMatrix * rotationMatrix * lrotation * _reflection;
    }
    else
    {
        worldMatrix = toCenter * translationMatrix * scalingMatrix * rotationMatrix * lrotation * _reflection * fromCenter;
    }
    _polyhedron.LocalToWorld *= worldMatrix;

    int clientWidth = e.ClipRectangle.Width;
    int clientHeight = e.ClipRectangle.Height;

    int offsetX = clientWidth / 2;
    int offsetY = clientHeight / 2;

    centroid = _polyhedron.Centroid(_polyhedron.LocalToWorld);
    e.Graphics.FillRectangle(Brushes.Red, (int)centroid.X + offsetX, (int)centroid.Y + offsetY, 2, 2);

    var points2D = new List<Point>(10);

    foreach (Face face in _visiblePolyhedron.Faces) // Рендерим только видимые грани
    {
        foreach (Vertex vertex in face.Vertices)
        {
            Vertex worldVertex = Transformer.TransformToWorld(vertex, _polyhedron.LocalToWorld * projectionFunction.getProjection(), projectionFunction);
            if (worldMatrix == null) throw new InvalidOperationException("Матрица преобразования некорректна.");
            points2D.Add(new Point((int)worldVertex.X, (int)worldVertex.Y));
        }

        var centeredPoints = points2D.Select(p => new Point(p.X + offsetX, p.Y + offsetY)).ToArray();
        if (centeredPoints.Length > 0)
        {
            e.Graphics.DrawPolygon(Pens.Black, centeredPoints);
        }
        points2D.Clear();
    }
}
