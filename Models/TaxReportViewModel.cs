using CelsiusTax.Models.Interests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        public DateTime From { get; set; }
        [Required]
        public DateTime To { get; set; }
        public string SelectedCurrency { get; internal set; }
    }   
}
