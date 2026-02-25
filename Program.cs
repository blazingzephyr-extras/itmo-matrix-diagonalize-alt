
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
        Console.WriteLine($"* ({Math.Pow(-1, rang)}) / {alphas[rang]}");

        Console.WriteLine();
        Console.WriteLine("// 2. Введите степень, в которую необходимо возвести матрицу");

        string? nextLine = Console.ReadLine();
        if (nextLine is null) return;

        // Пропускаем первый коэффициент.
        // На самом деле, возможно он и не нужен в I части.
        alphas = [..alphas.Skip(1)];
        double[] c = [..alphas];

        int pow = int.Parse(nextLine);
        for (int i = 0; i < pow - rang; i++)
        {
            double[] t = new double[rang];

            // Было неправильно: t[rang - 1] = alphas[0] * c[rang - 1];
            //
            // Здесь отсутствует умножение коэффициентов на -1, так что мб. ошибка в том, что берём
            // их отрицательными?
            t[rang - 1] = alphas[rang - 1] * c[0];

            for (int j = 0; j < rang - 1; j++)
            {
                t[j] = alphas[j] * c[0] + c[j + 1];
                Console.WriteLine($"c[{j}] = {alphas[j]} * {c[0]} + {c[j + 1]} = {t[j]}");
            }

            Console.WriteLine($"c[{rang - 1}] = {alphas[0]} * {c[rang - 1]} = {t[rang - 1]}");
            Console.WriteLine();

            c = t;
        }

        double[,] result = Matrix.Zero<double>(A0.I());
        for (int i = 0; i < rang; i++)
        {
            result = Matrix.Add(result,
                                Matrix.Power(A0, rang - i - 1)
                                    .Scalar(c[i]));
        }

        Console.WriteLine(result.Print());
    }
}
