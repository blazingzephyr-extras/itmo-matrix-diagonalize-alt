
namespace ITMO;

internal partial class Program
{
    private static double[,]? Input()
    {
        int y = 0;
        int x = 0;

        List<string[]> lines = new List<string[]>();
        string[] lastLine = [];
        bool valid = true;

        Console.ReadKey();
        Console.Clear();
        Console.WriteLine("Введите матрицу:");

        do
        {
            string? line = Console.ReadLine();
            if (string.IsNullOrEmpty(line)) break;

            lastLine = String.IsNullOrEmpty(line) ? [] : line.Split();
            lines.Add(lastLine);
            y++;

            if (lastLine.Length == 0) valid = false;
            else if (x == 0) x = lastLine.Length;
            else if (x != lastLine.Length) valid = false;
        }
        while (valid);
        if (!valid || y == 0)
        {
            if (y > 0)
            {
                Console.WriteLine("Неправильный размер матрицы");
                Console.WriteLine("————————————————————");
                Console.WriteLine();
            }
            return null;
        }

        double[,] A = new double[y, x];
        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < lines[0].Length; j++)
            {
                A[i, j] = Convert.ToDouble(lines[i][j]);
            }
        }

        return A;
    }
}
