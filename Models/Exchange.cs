namespace LuccaDevises.Models
{
    public class Conversion
    {
        public string InitialCurrency {get; set;}
        public string TargetCurrency {get; set;}
    }

    public class ExchangeRate : Conversion
    {
        public float Rate {get; set;}
    }

    public class ToConvert : Conversion
    {
        public int Amount {get; set;}
    }
}