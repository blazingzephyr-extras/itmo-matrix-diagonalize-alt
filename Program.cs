
namespace ITMO;

internal partial class Program
{
    static void Main(string[] args)
    {
        // Логика чтения консоли взята из предыдущих программ,
        // у меня не было желания её переписывать понапрасну.
        var A0 = Input();
        if (A0 is null) return;

        int rang = A0.Rg();
        if (rang != A0.I() || A0.DetTri() == 0)
        {
            throw new InvalidDataException(
                "Матрица вырождена и/или" +
                "её ранг меньше положенного.");
        }

        // Находим коэффициенты при характеристическом уравнении
        // при помощи метода Фаддеева-Леверрье.
        double[] alphas = new double[rang + 1];

        // Не до конца уверен, почему здесь всегда -1.
        // Дальше тоже нужно разобраться, почему так.
        alphas[0] = -1;
        alphas[rang] = A0.DetTri();

        // A_1 = A
        // a_n = (1/n) * tr(A_n),
        // B_n = A_n - a_n * E,
        // A_n = A * B_n-1
        //
        double[,] A = A0;
        for (int n = 1; n < rang; n++)
        {
            double alpha = 1.0 / n * A.Tr();
            double[,] B = Matrix.E<double>(rang)
                                .Scalar(-alpha)
                                .Add(A);

            A = A.Product(B);
            alphas[n] = alpha;
        }

        // Находим по формуле обратную матрицу.
        double[,] inverse = Matrix.Zero<double>(rang);
        for (int n = rang; n > 0; n--)
        {
            inverse = A0
                .Power(n - 1)
                .Scalar(alphas[rang - n])
                .Add(inverse);
        }

        Console.WriteLine(inverse.Print());
        Console.WriteLine($"* ({Math.Pow(-1, rang)}) / {alphas[rang]}");
    }
}
