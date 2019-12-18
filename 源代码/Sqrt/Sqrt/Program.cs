using System;

namespace Sqrt
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello!");
            for(int k=0;k<12;k++)
            {
                double x = 0.5;
                double.TryParse(Console.ReadLine(), out double delta);
                if ((x > 1) || (x < 0) || (delta < 0))
                {
                    Console.WriteLine("x is out of range [0, 1].");
                }
                else if (x == 1)
                {
                    Console.WriteLine("Answer: 1");
                    Console.WriteLine("IterTimes: 0");
                }
                else if (x == 0)
                {
                    Console.WriteLine("Answer: 0");
                    Console.WriteLine("IterTimes: 0");
                }
                else
                {
                    int i = 0;
                    double x0 = x;
                    double x1 = (x0 / 2) + (x / (2 * x0));
                    while (Math.Abs(x0 - x1) > delta)
                    {
                        x0 = x1;
                        x1 = (x0 / 2) + (x / (2 * x0));
                        i++;
                    }
                    Console.WriteLine("Answer: " + x1.ToString());
                    Console.WriteLine("IterTimes: " + i.ToString());
                }
            }
        }
    }
}
