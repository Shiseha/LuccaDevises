using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using LuccaDevises.Models;

namespace LuccaDevises.Converter
{
    public class CurrencyConverter
    {
        public void Run(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.Error.WriteLine("File path doesn't exist");
                Environment.Exit(1);
            }

            var lines = File.ReadLines(filePath).ToList();

            try
            {
                var instructions = ExchangeRatesTableParser.GetConversionInstructions(lines[0], lines[1]);

                ToConvert toConvert = instructions.Item1;
                int exchangeRatesCount = instructions.Item2;

                var exchangeRates = ExchangeRatesTableParser.ParseRates(lines, exchangeRatesCount);
            }
            catch (IndexOutOfRangeException e)
            {
                Console.Error.WriteLine($"Incorrect file: {e.Message}");
                Environment.Exit(1);
            }
        }
    }
}