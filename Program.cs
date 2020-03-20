using System;
using LuccaDevises.Converter;

namespace LuccaDevises
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath;

            if (args.Length < 1)
            {
                Console.Error.WriteLine("File path argument is missing");
                return;
            }
            else
            {
                filePath = args[0];
            }

            CurrencyConverter converterApp = new CurrencyConverter();
            converterApp.Run(filePath);
        }
    }
}
