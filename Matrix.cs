
using System.Numerics;
using System.Text;

namespace ITMO;

internal static partial class Matrix
{
    // Нулевая матрица размерностью nxn.
    public static T[,] Zero<T>(int n) where
        T : IFloatingPointIeee754<T>
    {
        T[,] matrix = new T[n, n];
        return matrix;
    }

    // Единичная матрица размерностью nxn;
    public static T[,] E<T>(int n) where
        T : IFloatingPointIeee754<T>
    {
        T[,] matrix = new T[n, n];
        for (int i = 0; i < n; i++)
        {
            matrix[i, i] = T.One;
        }

        return matrix;
    }

    // i-ая координата матрицы.
    public static int I<T>(this T[,] matrix)
        where T : IFloatingPointIeee754<T>
    {
        return matrix.GetLength(0);
    }

    // j-ая координата матрицы.
    public static int J<T>(this T[,] matrix)
        where T : IFloatingPointIeee754<T>
    {
        return matrix.GetLength(1);
    }

    // Является ли матрица квадратной
    // (равенство кол-ва строк и столбцов).
    public static bool IsQuadratic<T>(this T[,] matrix)
        where T : IFloatingPointIeee754<T>
    {
        return I(matrix) == J(matrix);
    }

    // След матрицы.
    public static T Tr<T>(this T[,] matrix)
        where T : IFloatingPointIeee754<T>
    {
        if (!matrix.IsQuadratic())
        {
            throw new InvalidOperationException(
                "След можно найти только для квадратной матрицы");
        }

        T tr = T.Zero;
        for (int n = 0; n < matrix.I(); n++)
        {
            tr += matrix[n, n];
        }

        return tr;
    }

    // Выводит матрицу на экран.
    public static string Print<T>(this T[,] matrix)
        where T : IFloatingPointIeee754<T>
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < matrix.I(); i++)
        {
            builder.Append('(');
            for (int j = 0; j < matrix.J(); j++)
            {
                builder.Append(' ');
                builder.Append(matrix[i, j]);
            }

            builder.Append(' ');
            builder.Append(')');
            builder.AppendLine();
        }

        return builder.ToString();
    }
}
