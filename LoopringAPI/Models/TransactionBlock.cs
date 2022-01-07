using System;
using System.Collections.Generic;
using System.Text;

namespace LoopringAPI
{
    public class TransactionBlock
    {
        public string txType { get; set; }
        public int accountId { get; set; }
        public string owner { get; set; }

        public OrderBlock orderA { get; set; }
        public OrderBlock orderB { get; set; }
        public bool valid { get; set; }
    }
}
