
using System.Numerics;

namespace ITMO;

internal static partial class Matrix
{
    // Умножение матрицы на скаляр.
    public static T[,] Scalar<T>(this T[,] matrix, T scalar) where
        T : IFloatingPointIeee754<T>
    {
        T[,] copy = (T[,])matrix.Clone();
        for (int i = 0; i < copy.I(); i++)
        {
            for (int j = 0; j < copy.J(); j++)
            {
                copy[i, j] *= scalar;
            }
        }

        return copy;
    }

    // Сложение матриц.
    public static T[,] Add<T>(this T[,] left, T[,] right) where
        T : IFloatingPointIeee754<T>
    {
        int lx = left.J();
        int ly = left.I();
        int rx = right.J();
        int ry = right.I();

        if (lx != rx || ly != ry)
        {
            throw new InvalidOperationException(
                "Сложение матриц, отличающихся по размеру," +
                "не поддерживается.");
        }

        T[,] matrix = new T[lx, ly];
        for (int i = 0; i < ly; i++)
        {
            for (int j = 0; j < lx; j++)
            {
                matrix[i, j] = left[i, j] + right[i, j];
            }
        }

        return matrix;
    }

    // Умножение матриц.
    public static T[,] Product<T>(this T[,] left, T[,] right) where
        T : IFloatingPointIeee754<T>
    {
        int j1 = I(left);
        int i2 = J(right);
        
        if (j1 != i2)
        {
            throw new InvalidOperationException(
                "Умножение матрицы с кол-вом столбцов j " +
                "на матрицу с кол-вом строк i невозможно," +
                "если i не равно j " +
                $"(j = {j1}, i = {i2})");
        }

        int i1 = I(left);
        int j2 = J(right);
        T[,] matrix = new T[i1, j2];

        for (int i = 0; i < i1; i++)
        {
            for (int j = 0; j < j2; j++)
            {
                for (int m = 0; m < j1; m++)
                {
                    matrix[i, j] += left[i, m] * right[m, j];
                }
            }
        }

        return matrix;
    }

    // Возведение матрицы в степень.
    public static T[,] Power<T>(this T[,] matrix, int power) where
        T : IFloatingPointIeee754<T>
    {
        switch (power)
        {
            case 0: return E<T>(matrix.I());
            case 1: return matrix;
            default:
                T[,] copy = (T[,])matrix.Clone();
                for (int i = 1; i < power; i++)
                {
                    copy = copy.Product(matrix);
                }

                return copy;
        }
    }

    // Умножает ряд матрицы на скаляр.
    public static T[,] ScalarRow<T>(this T[,] matrix, int i, T scal)
        where T : IFloatingPointIeee754<T>
    {
        T[,] copy = (T[,])matrix.Clone();
        int J = copy.J();

        for (int j = 0; j < J; j++)
        {
            copy[i, j] *= scal;
        }

        return copy;
    }

    // Складывает ряды матрицы путём добавления одного ряда в другой.
    public static T[,] AddRow<T>(this T[,] matrix, int a, int b, T coeff)
        where T : IFloatingPointIeee754<T>
    {
        T[,] copy = (T[,])matrix.Clone();
        int J = copy.J();

        for (int j = 0; j < J; j++)
        {
            copy[a, j] += coeff * copy[b, j];
        }

        return copy;
    }

    // Меняет местами строки матрицы.
    public static T[,] SwapRows<T>(this T[,] matrix, int a, int b)
        where T : IFloatingPointIeee754<T>
    {
        T[,] copy = (T[,])matrix.Clone();
        for (int j = 0; j < copy.J(); j++)
        {
            copy[a, j] = matrix[b, j];
            copy[b, j] = matrix[a, j];
        }

        return copy;
    }

    // Нахождение детерминанта (определителя) путём вычисления его для
    // соответствующей треугольной матрицы.
    //
    // Здесь используется свойство сохранения определителя матрицы после
    // элементарных преобразований.
    public static T DetTri<T>(this T[,] matrix)
        where T : IFloatingPointIeee754<T>
    {
        if (!matrix.IsQuadratic())
        {
            throw new InvalidOperationException(
                "Сведением к треугольной матрице определитель" +
                "возможно вычислить только для квадратной матрицы.");
        }

        T[,] copy = (T[,])matrix.Clone();
        T det = T.One;
        
        for (int j = 0; j < copy.J(); j++)
        {
            for (int i = j + 1; i < copy.I(); i++)
            {
                // Пробуем поменять ряды местами,
                // если обнаружен ноль на диагонали.
                if (copy[j, j] == T.Zero && copy[i, j] != T.Zero)
                {
                    copy = copy.SwapRows(i, j);
                }

                var coeff = -copy[i, j] / copy[j, j];
                copy = copy.AddRow(i, j, coeff);
            }

            det *= copy[j, j];
        }

        return det;
    }

    // Поиск ранга матрицы преимущественно взят из предыдущих РГР.
    // Используется слегка модифицированный метод Гаусса, но не уверен,
    // что есть варианты лучше.
    //
    // Вроде бы можно использовать Сингулярное разложение и форму Смита, но
    // мы ещё не дошли до формы Смита.
    public static int Rg<T>(this T[,] matrix)
        where T : IFloatingPointIeee754<T>
    {
        var identity = E<T>(matrix.I());
        var r = Math.Min(matrix.I(), matrix.J());
        int rank = 0;

        for (int i = 0; i < r; i++)
        {
            int pivotRow = -1;
            for (int k = i; k < matrix.I(); k++)
            {
                if (matrix[k, i] != T.Zero)
                {
                    pivotRow = k;
                    break;
                }
            }

            if (pivotRow == -1)
            {
                continue;
            }

            if (pivotRow != i)
            {
                matrix = matrix.SwapRows(i, pivotRow);
                identity = identity.SwapRows(i, pivotRow);
            }

            T diag = matrix[i, i];
            T factor = T.One / diag;

            matrix = matrix.ScalarRow(i, factor);
            identity = identity.ScalarRow(i, factor);
            rank++;

            for (int ii = 0; ii < matrix.I(); ii++)
            {
                if (i == ii) continue;

                T eliminationFactor = matrix[ii, i];
                if (eliminationFactor != T.Zero)
                {
                    matrix = matrix.AddRow(ii, i, -eliminationFactor);
                    identity = identity.AddRow(ii, i, -eliminationFactor);
                }
            }
        }

        return rank;
    }
}
