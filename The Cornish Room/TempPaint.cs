//ZBufferRenderer renderer = new ZBufferRenderer(pictureBox1.Width, pictureBox1.Height, projectionFunction);
            //RayTracer rayTracer = new RayTracer(sceneObjects, light);

            //// Отрисовка геометрии с использованием Z-буфера
            //foreach (var sceneObject in sceneObjects)
            //{
            //    renderer.RenderFace(sceneObject.Polyhedron, transformationMatrix, pen);
            //}

            //// Расчет освещения с использованием RayTracer
            //for (int y = 0; y < pictureBox1.Height; y++)
            //{
            //    for (int x = 0; x < pictureBox1.Width; x++)
            //    {
            //        if (renderer.IsPixelVisible(x, y)) // Проверка видимости пикселя
            //        {
            //            Vertex rayOrigin = camera.Position;
            //            Vertex rayDirection = CalculateRayDirection(x, y);

            //            Color pixelColor = rayTracer.TraceRay(rayOrigin, rayDirection);
            //            renderer.SetPixelColor(x, y, pixelColor);
            //        }
            //    }
            //}

            //// Отрисовка результата на экран
            //renderer.DrawToGraphics(e.Graphics);







            // Создаем Ray Tracer (если он еще не создан)
            //if (rayTracer == null)
            //{
            //    rayTracer = new RayTracer(sceneObjects, light);
            //}

            //// Создаем Bitmap для рендеринга
            //Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            //// Рендеринг сцены
            //for (int y = 0; y < bitmap.Height; y++)
            //{
            //    for (int x = 0; x < bitmap.Width; x++)
            //    {
            //        Vertex rayOrigin = _camera.Position;
            //        Vertex rayDirection = CalculateRayDirection(x, y);

            //        Color pixelColor = rayTracer.TraceRay(rayOrigin, rayDirection);
            //        bitmap.SetPixel(x, y, pixelColor);
            //    }
            //}
            //// Отображаем Bitmap на PictureBox
            //e.Graphics.DrawImage(bitmap, 0, 0);
            // Очистка PictureBox (если нужно)
            //e.Graphics.Clear(Color.Black);

            //// Рендеринг сцены
            //for (int y = 0; y < pictureBox1.Height; y++)
            //{
            //    for (int x = 0; x < pictureBox1.Width; x++)
            //    {
            //        // Выпускаем луч из камеры через пиксель (x, y)
            //        Vertex rayOrigin = _camera.Position;
            //        Vertex rayDirection = CalculateRayDirection(x, y);

            //        // Трассируем луч и получаем цвет пикселя
            //        Color pixelColor = rayTracer.TraceRay(rayOrigin, rayDirection);

            //        // Рисуем пиксель на PictureBox
            //        using (var brush = new SolidBrush(pixelColor))
            //        {
            //            e.Graphics.FillRectangle(brush, x, y, 1, 1);
            //        }
            //    }
            //}









            //if (_polyhedron == null)
            //    return;

            //_inittranslationX += _translationX;
            //_inittranslationY += _translationY;
            //_inittranslationZ += _translationZ;
            //_initscaleX *= _scaleX;
            //_initscaleY *= _scaleY;
            //_initscaleZ *= _scaleZ;
            //_initrotationX += _rotationX;
            //_initrotationY += _rotationY;
            //_initrotationZ += _rotationZ;

            //var viewMatrix = _camera.GetViewMatrix();
            //var projectionMatrix = _camera.GetProjectionMatrix();

            //var transformationMatrix = TranslationMatrix(_translationX, _translationY, _translationZ) *
            //                           ScalingMatrix(_scaleX, _scaleY, _scaleZ) *
            //                           RotationMatrix(_initrotationX, _initrotationY, _initrotationZ);

            //int clientWidth = e.ClipRectangle.Width;
            //int clientHeight = e.ClipRectangle.Height;

            //int offsetX = clientWidth / 2;
            //int offsetY = clientHeight / 2;

            //foreach (var face in _polyhedron.Faces)
            //{
            //    var points2D = new List<Point>();
            //    foreach (var vertex in face.Vertices)
            //    {
            //        // Трансформация вершины
            //        var transformedVertex = Transformer.TransformToWorld(vertex, transformationMatrix * viewMatrix * projectionMatrix, projectionFunction);

            //        Console.WriteLine($"Before normalization: X={transformedVertex.X}, Y={transformedVertex.Y}, Z={transformedVertex.Z}, W={transformedVertex.W}");
            //        // Нормализация в экранные координаты
            //        //double w = transformedVertex.W;
            //        //double x = transformedVertex.X / w;
            //        //double y = transformedVertex.Y / w;

            //        // Преобразование в пиксельные координаты
            //        points2D.Add(new Point(
            //            (int)(transformedVertex.X * offsetX + offsetX),
            //            (int)(transformedVertex.Y * offsetY + offsetY)
            //        ));
            //    }

            //    // Отрисовка полигона
            //    if (points2D.Count >= 3)
            //    {
            //        e.Graphics.DrawPolygon(Pens.Black, points2D.ToArray());
            //    }
            //}




            //BEGIN---------------------------ZBUFFER
            //if (renderer == null)
            //{
            //    renderer = new ZBufferRenderer(e.ClipRectangle.Width, e.ClipRectangle.Height, projectionFunction);
            //}
            ////finalTransformationMatrix = TranslationMatrix(_translationX, _translationY, _translationZ) *
            ////                                  ScalingMatrix(_scaleX, _scaleY, _scaleZ) *
            ////                                  RotationMatrix(_initrotationX, _initrotationY, _initrotationZ);

            //renderer.ClearBuffer();

            //foreach (var sceneObject in sceneObjects)
            //{
            //    // Вычисляем центроид фигуры
            //    Vertex centroid = sceneObject.Polyhedron.Centroid(sceneObject.Polyhedron.LocalToWorld);

            //    // Матрица перемещения в центр
            //    Matrix toCenter = TranslationMatrix(-centroid.X, -centroid.Y , -centroid.Z);

            //    // Матрица преобразований (вращение, масштабирование и т.д.)
            //    Matrix transformationMatrix = RotationMatrix(sceneObject.RotationX, sceneObject.RotationY,sceneObject.RotationZ) *
            //                                  ScalingMatrix(sceneObject.ScaleX, sceneObject.ScaleY, sceneObject.RotationZ);

            //    // Матрица возврата на место
            //    Matrix fromCenter = TranslationMatrix(centroid.X, centroid.Y, centroid.Z);

            //    // Матрица отдельного перемещения
            //    Matrix translationMatrix = TranslationMatrix( sceneObject.TranslationX, sceneObject.TranslationY, sceneObject.TranslationZ);

            //    // Финальная матрица преобразования
            //    Matrix finalTransformationMatrix = toCenter * transformationMatrix * fromCenter * translationMatrix;

            //    int q = 1;
            //    // Рендерим все грани
            //    foreach (var face in sceneObject.Polyhedron.Faces)
            //    {


            //        if (q == 1) renderer.RenderFace(face, finalTransformationMatrix, Pens.Yellow);
            //        else if (q == 2) renderer.RenderFace(face, finalTransformationMatrix, Pens.Black);
            //        else if (q == 3) renderer.RenderFace(face, finalTransformationMatrix, Pens.Azure);
            //        else if (q == 4) renderer.RenderFace(face, finalTransformationMatrix, Pens.Red);
            //        else if (q == 5) renderer.RenderFace(face, finalTransformationMatrix, Pens.Green);
            //        else renderer.RenderFace(face, finalTransformationMatrix, Pens.Brown);
            //        q++;
            //        //renderer.RenderFace(face, transformationMatrix, Pens.Black);
            //        //renderer.DrawToGraphics(e.Graphics);
            //    }
            //}

            //renderer.DrawToGraphics(e.Graphics);




            //renderer.ClearBuffer();

            //Направление обзора
            //var viewDirection = new Vertex(1, 0, -1);


            ////END--------------------------- ZBUFFER


            //Matrix translationMatrix = TranslationMatrix(_translationX, _translationY, _translationZ);
            //Matrix scalingMatrix = ScalingMatrix(_scaleX, _scaleY, _scaleZ);
            //Matrix rotationMatrix = RotationMatrix(_rotationX, _rotationY, _rotationZ);
            //Matrix lrotation = LRotation(_fi, _l, _m, _n);
            //Vertex centroid = _polyhedron.Centroid(_polyhedron.LocalToWorld);

            //Matrix toCenter = TranslationMatrix(-centroid.X, -centroid.Y, -centroid.Z);
            //Matrix fromCenter = TranslationMatrix(centroid.X, centroid.Y, centroid.Z);

            //// Матрица преобразования (только поворот и масштабирование, без переноса)
            //Matrix trasformationMatrixWithoutTranslation = RotationMatrix(_rotationX, _rotationX, _rotationZ) * ScalingMatrix(_scaleX, _scaleY, _scaleZ);
            //Matrix worldMatrix;
            //if (!IsCentroid)
            //{
            //    IsCentroid = true;
            //    worldMatrix = translationMatrix * scalingMatrix * rotationMatrix * lrotation * _reflection;
            //}
            //else
            //{
            //    worldMatrix = toCenter * translationMatrix * scalingMatrix * rotationMatrix * lrotation * _reflection * fromCenter;
            //}
            //_polyhedron.LocalToWorld *= worldMatrix;

            //int clientWidth = e.ClipRectangle.Width;
            //int clientHeight = e.ClipRectangle.Height;

            //int offsetX = clientWidth / 2;
            //int offsetY = clientHeight / 2;

            //centroid = _polyhedron.Centroid(_polyhedron.LocalToWorld);
            //e.Graphics.FillRectangle(Brushes.Red, (int)centroid.X + offsetX, (int)centroid.Y + offsetY, 2, 2);

            //var points2D = new List<Point>(10);

            //// Выполняем отсечение
            //Polyhedron visibleFaces = BackfaceCulling(_polyhedron, viewDirection, trasformationMatrixWithoutTranslation);
            //foreach (Face face in visibleFaces.Faces)
            //{
            //    foreach (Vertex vertex in face.Vertices)
            //    {
            //        Vertex worldVertex = Transformer.TransformToWorld(vertex, _polyhedron.LocalToWorld * projectionFunction.getProjection(), projectionFunction);
            //        if (worldMatrix == null) throw new InvalidOperationException("Матрица преобразования некорректна.");
            //        points2D.Add(new Point((int)worldVertex.X, (int)worldVertex.Y));
            //    }

            //    var centeredPoints = points2D.Select(p => new Point(p.X + offsetX, p.Y + offsetY)).ToArray();
            //    if (centeredPoints.Length > 0)
            //    {
            //        e.Graphics.DrawPolygon(Pens.Black, centeredPoints);
            //    }
            //    points2D.Clear();
            //}