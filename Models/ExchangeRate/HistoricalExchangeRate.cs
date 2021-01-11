using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CelsiusTax.Models.ExchangeRate
{
    public class HistoricalExchangeRate
    {
        public DateTime Date { get; set; }
        public decimal ExchangeRate { get; set; }
        public string FiatSymbol { get; set; }
    }
}
