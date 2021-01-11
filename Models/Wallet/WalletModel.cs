using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CelsiusTax.Models.Wallet
{
    public class WalletModel
    {
        public decimal TotalValueInUsd => Balances.Sum(t => t.ValueInUsd);
        public decimal TotalValueInCurrencyToDisplay => Balances.Sum(t => t.ValueInCurrencyToDisplay);

        public IEnumerable<BalanceModel> Balances { get; set; }
        public decimal? CurrentCelBalance { get; internal set; }
    }
}
