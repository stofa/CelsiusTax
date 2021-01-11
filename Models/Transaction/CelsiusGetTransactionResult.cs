using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CelsiusTax.Models.Transaction
{
    public class CelsiusGetTransactionResult
    {
        public CelsisuGetTransactionPaginationResult pagination { get; set; }
        public IEnumerable<CelsiusTransactionModel> record { get; set; }
    }
}
