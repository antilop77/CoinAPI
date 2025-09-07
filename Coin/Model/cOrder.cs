namespace CoinAPI.Coin.Model
{
    public class cOrder
    {
        public long orderId;
        public decimal quantity;
        public decimal closePrice;
        public decimal tickSize;
        public decimal lotSize;
        public string? buySymbolCode;
        public string? baseAsset;
    }
}
