using CelsiusTax.Models.Interests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CelsiusTax.Models
{
    public class TaxReportViewModel
    {
        public TaxReportViewModel()
        {
            InterestsPerCoin = new List<InterestsPerCoin>();
        }
        public IEnumerable<string> AvailableCurrencies { get; set; }

        public IEnumerable<InterestsPerCoin> InterestsPerCoin { get; set; }
        public int TaxYear { get; internal set; }
        public string SelectedCurrency { get; internal set; }
    }   
}
