using System;

namespace LoopringAPI
{
    public class OrderResult
    {
        public OrderResult(ApiOrderSubmitResult aosr)
        {
            hash = aosr.hash;
            clientOrderId = aosr.clientOrderId;
            status = (OrderStatus)Enum.Parse(typeof(OrderStatus), aosr.status, true);
            isIdempotent = aosr.isIdempotent;
        }

        public string hash { get; set; }
        public string clientOrderId { get; set; }
        public OrderStatus status { get; set; }
        public bool isIdempotent { get; set; }
    }
}
