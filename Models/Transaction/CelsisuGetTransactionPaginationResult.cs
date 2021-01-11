﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CelsiusTax.Models.Transaction
{
    public class CelsisuGetTransactionPaginationResult
    {
        public int total { get; set; }
        public int pages { get; set; }
        public int per_page { get; set; }
        public string showing { get; set; }
    }
}
