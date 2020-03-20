using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using LuccaDevises.Models;

namespace LuccaDevises.Converter
{
    public class CurrencyConverter
    {
        private KeyValuePair<string, float> GetNewNode(ExchangeRate matchingRate, string lastCurrency)
        {
            if (matchingRate.InitialCurrency == lastCurrency)
            {
                return new KeyValuePair<string, float>(matchingRate.TargetCurrency, matchingRate.Rate);
            }
            else
            {
                return new KeyValuePair<string, float>(matchingRate.InitialCurrency, (float)Math.Round(1 / matchingRate.Rate, 4));
            }
        }

        private List<ConversionPath> AddPathsFromMatches(List<ConversionPath> paths, ConversionPath currentAnalyzedPath, List<ExchangeRate> matchingRates)
        {
            var lastNode = currentAnalyzedPath.Nodes.Last();
            var currentPathState = new ConversionPath { Nodes = currentAnalyzedPath.Nodes.ToList() };
            // add a node to the iterated path
            paths.First(p => p == currentAnalyzedPath)
                .Nodes.Add(
                        GetNewNode(matchingRates.First(), lastNode.Key
                    ));

            // multiple matches, create new possible paths
            if (matchingRates.Count > 1)
            {
                int remainingCount = matchingRates.Count - 1;
                var duplicates = Enumerable.Repeat(currentPathState, remainingCount).ToList();
                for (int i = 0; i < remainingCount; i++)
                {
                    duplicates[i].Nodes.Add(GetNewNode(matchingRates[i + 1], lastNode.Key));
                }
                paths.AddRange(duplicates);
            }

            return paths;
        }

        private ConversionPath GetShortestPath(ToConvert toConvert, List<ExchangeRate> exchangeRates)
        {
            // keys are currencies, values are for the chained calculations
            // first node: <initial currency, amount>
            // other nodes: <currency, exchange rate>
            var firstNode = new KeyValuePair<string, float>(toConvert.InitialCurrency, toConvert.Amount);

            // init with a first path with the currency to convert from 
            var possibilities = new List<ConversionPath> {
                new ConversionPath { Nodes = new List<KeyValuePair<string, float>> {firstNode}}
            };

            while(exchangeRates.Any())
            {
                foreach(var possibility in possibilities.ToList())
                {
                    var lastNode = possibility.Nodes.Last();

                    var matchingRates = exchangeRates
                        .Where(e => lastNode.Key == e.InitialCurrency || lastNode.Key == e.TargetCurrency)
                        .ToList();

                    if (matchingRates.Any())
                    {
                        var firstMatch = matchingRates.First();
                        possibilities = AddPathsFromMatches(possibilities, possibility, matchingRates);

                        var shortest = possibilities.FirstOrDefault(p => p.Nodes.Last().Key == toConvert.TargetCurrency);
                        // found the shortest path
                        if (shortest != null)
                        {
                            return shortest;
                        }

                        exchangeRates.RemoveAll(e => matchingRates.Contains(e));
                    }
                    else
                    {
                        // no match, useless path
                        possibilities.Remove(possibility);
                        // remaining exchange rates but no path to solution 
                        if (!possibilities.Any()) return null;
                        break;
                    }
                }
            }
            // empty list but no solution found
            return null;
        }

        public double ConvertCurrency(ConversionPath path)
        {
            float amount = path.Nodes[0].Value;
            foreach(var node in path.Nodes.Skip(1))
            {
                amount *= node.Value;
            }
            return amount;
        }

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

                ConversionPath shortestPath = GetShortestPath(toConvert, exchangeRates.ToList());
                if (shortestPath == null)
                {
                    Console.WriteLine("No solution found for this exchange rates list");
                    return;
                }

                Console.WriteLine(Math.Round(ConvertCurrency(shortestPath)));
            }
            catch (IndexOutOfRangeException)
            {
                Console.Error.WriteLine($"Incorrect file: doesn't respect the exchange rates format");
                Environment.Exit(1);
            }
            catch (FormatException)
            {
                Console.Error.WriteLine($"Incorrect file: number of lines not provided");
                Environment.Exit(1);
            }
        }
    }
}