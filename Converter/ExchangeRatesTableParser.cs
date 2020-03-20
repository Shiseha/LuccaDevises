using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System;
using LuccaDevises.Models;

namespace LuccaDevises.Converter
{
    public static class ExchangeRatesTableParser
    {
        public static Tuple<ToConvert, int> GetConversionInstructions(string request, string exchangeRateCount)
        {
            string[] requestData = request.Split(';');

            if (requestData.Length != 3) throw new IndexOutOfRangeException();
            else
            {
                ToConvert toConvert = new ToConvert {
                    InitialCurrency = requestData[0],
                    Amount = Convert.ToInt32(requestData[1]),
                    TargetCurrency = requestData[2]
                };
                int count = Convert.ToInt32(exchangeRateCount);
                return Tuple.Create(toConvert, count);
            }
        }

        public static IEnumerable<ExchangeRate> ParseRates(IEnumerable<string> lines, int exchangeRatesCount)
        {
            return lines.Skip(2).Take(exchangeRatesCount)
                .Select(l => {
                    var sections = l.Split(';');

                    if (sections.Length != 3) throw new IndexOutOfRangeException();
                    else
                    {
                        return new ExchangeRate {
                            InitialCurrency = sections[0],
                            TargetCurrency = sections[1],
                            Rate = float.Parse(sections[2], CultureInfo.InvariantCulture)
                        };
                    }
                });
        }

    }
}