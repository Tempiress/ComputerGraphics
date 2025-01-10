namespace Lab8
{
    public class Matrix
    {
        private readonly double[,] _data;
        private readonly int _rows;
        private readonly int _cols;

        public Matrix(double[,] data)
        {
            _data = data;
            _rows = data.GetLength(0);
            _cols = data.GetLength(1);
        }
        public double this[int x, int y] => _data[x, y];

        public static Matrix operator* (Matrix a, Matrix b)
        {
            int rows = a._rows;
            int cols = b._cols;

            var result = new double[rows, cols];

            for (var i = 0; i < rows; i++)
            {
                for (var j = 0; j < cols; j++)
                {
                    result[i, j] = 0;
                    for (var k = 0; k < a._cols; k++)
                    {
                        result[i, j] += a._data[i, k] * b._data[k, j];
                    }
                }
            }
            return new Matrix(result);
        }
    }
}