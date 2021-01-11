using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CelsiusTax.Models.Interests
{
    public class InterestsPerCoin
    {
        public string Coin { get; set; }
        public decimal UsdValue { get; set; }
        public decimal ValueInSelectedFiat { get; internal set; }
    }
}
