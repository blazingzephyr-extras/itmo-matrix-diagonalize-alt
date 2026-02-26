
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
        //
        // Edit 26.02:
        // Предыдущая версия:
        // alphas[0] = -1
        // alphas[rang] = A0.DetTri();
        //
        alphas[0] = Math.Pow(-1, rang);
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

            // Edit 26.02:
            // Исправлена ошибка, было: A = A.Product(B);
            A = A0.Product(B);
            alphas[n] = Math.Pow(-1.0, rang - 1) * alpha;
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

        Console.WriteLine("// 0. Характеристическое уравнение:");
        var builder = new System.Text.StringBuilder();
        for (int i = 0; i < alphas.Length; i++)
        {
            builder.Append($"{alphas[i]}*L^({rang - i}) + ");
        }

        Console.Write(builder.Remove(builder.Length - 3, 3));
        Console.WriteLine(" = 0");
        Console.WriteLine();

        Console.WriteLine("// 1. Обратная матрица");
        Console.WriteLine(inverse.Print());
        Console.WriteLine($"* 1 / {Math.Pow(-1, rang) * Math.Pow(-1, rang - 1) * alphas[rang]}");

        Console.WriteLine();
        Console.WriteLine("// 2. Введите степень, в которую необходимо возвести матрицу");

        string? nextLine = Console.ReadLine();
        if (nextLine is null) return;

        double[] c = [..alphas];

        int pow = int.Parse(nextLine);
        for (int i = 0; i < pow - rang; i++)
        {
            double[] t = new double[rang + 1];

            t[rang] = -alphas[0] * alphas[rang] * c[1];

            for (int j = 1; j < rang; j++)
            {
                t[j] = -alphas[0] * alphas[j] * c[1] + c[j + 1];
                Console.WriteLine($"c[{j}] = {-alphas[0]} * {alphas[j]} * {c[1]} + {c[j + 1]} = {t[j]}");
            }

            Console.WriteLine($"c[{rang}] = {-alphas[0]} * {alphas[rang]} * {c[1]} = {t[rang]}");
            Console.WriteLine();

            c = t;
        }

        double[,] result = Matrix.Zero<double>(A0.I());
        for (int i = 0; i < rang; i++)
        {
            result = Matrix.Add(result,
                                Matrix.Power(A0, rang - i - 1)
                                    .Scalar(c[i + 1]));
        }

        Console.WriteLine(result.Scalar(alphas[0] * -1).Print());
    }
}
