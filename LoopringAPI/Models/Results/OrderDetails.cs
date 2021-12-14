using System;

namespace LoopringAPI
{
    public class OrderDetails
    {
        public OrderDetails(ApiOrderGetResult result)
        {
            hash = result.hash;
            clientOrderId = result.clientOrderId;
            side = (Side)Enum.Parse(typeof(Side), result.side, true);
            market = result.market;
            price = result.price;
            volumes = result.volumes;
            validity= result.validity;
            orderType = (OrderType)Enum.Parse(typeof(OrderType), result.orderType, true);
            tradeChannel = (TradeChannel)Enum.Parse(typeof(TradeChannel), result.tradeChannel, true);
            status = (OrderStatus)Enum.Parse(typeof(OrderStatus), result.status, true);
        }
        public string hash { get; set; }
        public string clientOrderId { get; set; }
        public Side side { get; set; }
        public string market { get; set; }
        public string price { get; set; }
        public Volumes volumes { get; set; }
        public Validity validity { get; set; }
        public OrderType orderType { get; set; }
        public TradeChannel tradeChannel { get; set; }
        public OrderStatus status { get; set; }
    }
}
