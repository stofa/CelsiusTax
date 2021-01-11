using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CelsiusTax.Models.Transaction
{
    public class CelsiusTransactionModel
    {
        public decimal amount_precise { get; set; }
        public decimal amount_usd { get; set; }
        public string original_interest_coin { get; set; }
        public string coin { get; set; }
        public string state { get; set; }
        public string nature { get; set; }
        public DateTime time { get; set; }
        public decimal AmountInSelectedFiat { get; internal set; }
    }
}
