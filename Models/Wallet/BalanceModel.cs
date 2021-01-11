using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CelsiusTax.Models.Wallet
{
    public class BalanceModel
    {
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public decimal ValueInUsd { get; set; }
        public decimal ValueInCurrencyToDisplay { get; internal set; }
        public string FiatCurrencyToDisplay { get; internal set; }
    }
}
