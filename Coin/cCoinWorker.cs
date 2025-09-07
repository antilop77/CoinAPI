using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Models.Spot;
using CoinAPI.Coin.Model;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices.JavaScript;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CoinAPI.Coin
{
    public class cCoinWorker : BackgroundService
    {
        private static Dictionary<string, DateTime> symbolDict = new Dictionary<string, DateTime>();
        private readonly ILogger<cCoinWorker> _logger;
        public static bool fNewOrder = true;
        public static bool fStop = false;
        public static int activeCoinCnt = 0;
        public static int maxCoin = 1;
        public static int cAMOUNT = 100;
        private static decimal CONST_LOW_DIFF = 1;
        private static readonly object obj = new object();
        private static readonly object threadObj = new object();
        private static readonly object coinObj = new object();
        private static int threadCnt = 0;
        public cCoinWorker(ILogger<cCoinWorker> logger)
        {
            _logger = logger;
        }

        static Action<DataEvent<IBinanceStreamKlineData>> fCoinStream = (data) =>
        {
            //Console.WriteLine(data.Symbol + " : " + $"{data.Data.TradeTime}: {data.Data.Quantity} @ {data.Data.Price}");

            Console.WriteLine(DateTime.Now.ToString() + " ... " + data.Symbol + " : " + data.Data.Data.Interval.ToString() + " : " + $"O:{data.Data.Data.OpenPrice} H: {data.Data.Data.HighPrice} L: {data.Data.Data.LowPrice} C: {data.Data.Data.ClosePrice}");
            //DateTime value;
            //symbolDict.TryGetValue(data?.Symbol, out value);
            //double diffInSeconds = (DateTime.Now - value).TotalSeconds;
            //if (diffInSeconds >= 1)
            //{
            //    symbolDict[data.Symbol] = DateTime.Now;
            //    Console.WriteLine(data.Symbol + @$" : {data.Data.TradeTime} " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + @$": {data.Data.Quantity} @ {data.Data.Price}");
            //    string sql = $@"insert into SYMBOLPRICE (Symbol, Price, BinanceDate, InsertDate, Quantity) 
            //                    values('{data.Symbol}', {data.Data.Price.ToString().Replace(",", ".")}, Convert(DATETIME, '{data.Data.TradeTime.ToString("yyyy.MM.dd HH:mm:ss")}', 120), getdate(), {data.Data.Quantity.ToString().Replace(",", ".")}); ";
            //    cCommon.executeNonQuery(sql);
            //}

            //DateTime value;
            //symbolDict.TryGetValue(data.Symbol?? "---", out value);
            //double diffInSeconds = (DateTime.Now - value).TotalSeconds;
            //if (diffInSeconds >= 1)
            //{
            //    symbolDict[data.Symbol?? "---"] = DateTime.Now;
            //    Console.WriteLine(data.Symbol + @$" : {data.Data.TradeTime} " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + @$": {data.Data.Quantity} @ {data.Data.Price}");
            //    string sql = $@"insert into SYMBOLPRICE (Symbol, Price, BinanceDate, InsertDate, Quantity) 
            //                    values('{data.Symbol}', {data.Data.Price.ToString().Replace(",", ".")}, Convert(DATETIME, '{data.Data.TradeTime.ToString("yyyy.MM.dd HH:mm:ss")}', 120), getdate(), {data.Data.Quantity.ToString().Replace(",", ".")}); ";
            //    cCommon.executeNonQuery(sql);
            //}

        };

        public static void doCoinProcesses()
        {
            //string sql = @$"select Symbol 
                //                from SYMBOL 
                //                where 1=1 
                //                and MarketType = 'SPOT' 
                //                and active = 1 ";


                //DataTable dataTable = cCommon.executeReader(sql);
                List<cCoin> items = new List<cCoin>();
                //items = dataTable.AsEnumerable().Select(m => new cCoin()
                //{
                //    Symbol = m.Field<string?>("Symbol"),
                //}).ToList();
                cCoin cCoin = new cCoin();
                cCoin.Symbol = "BTCUSDT";
                //items.Add(cCoin);
                //cCoin = new cCoin();
                //cCoin.Symbol = "LTCUSDT";
                items.Add(cCoin);

                BinanceSocketClient.SetDefaultOptions(options =>
                {
                    options.ApiCredentials = new ApiCredentials("yFviDUaU85iSjExhKbODs7mm0BX05KEZqFwHLF7BQzJF9C40kdMLpR7Y106CHueW"
                                                                , "ODDz6ZI65IBYot5sZ8SrPCsTS4bsjphR1nxujVIOhqu8WxjLEPpXa9O6wDHaAllZ");
                });

                

                var socketClient = new BinanceSocketClient();
                foreach (cCoin item in items)
                {

                    string symbol = item.Symbol ?? "";
                    symbolDict.Add(symbol, DateTime.Now);
                    List<string> symbols = new List<string>();
                    symbols.Add(symbol);
                    List<KlineInterval> klineIntervals = new List<KlineInterval>();
                    klineIntervals.Add(KlineInterval.OneSecond);
                    //klineIntervals.Add(KlineInterval.OneMinute);
                    //klineIntervals.Add(KlineInterval.FifteenMinutes);

                    //var subscription = await socketClient.SpotApi.ExchangeData.SubscribeToTradeUpdatesAsync(symbol, data => { fCoinStream(data); });
                    var subscription = socketClient.SpotApi.ExchangeData.SubscribeToKlineUpdatesAsync(symbols, klineIntervals, data => { fCoinStream(data); });

                    //try
                    //{
                    //    string? symbol = item.Symbol?.Trim();
                    //    symbolDict.Add(symbol??"...", DateTime.Now);

                    //    var subscription = await socketClient.SpotApi.ExchangeData.SubscribeToTradeUpdatesAsync(symbol ?? "...", data => { fCoinStream(data); });


                    //    subscription.Data.ConnectionLost += () => Console.WriteLine("Connection lost, trying to reconnect..");
                    //    subscription.Data.ConnectionRestored += (t) => Console.WriteLine("Connection restored");
                    //}
                    //catch (Exception pExc)
                    //{
                    //    Exception exc = pExc;
                    //}
                }

                //if (DateTime.Now.Minute % 15 == 0)
                //{
                //    Thread thread = new Thread(fnWaitSignal);
                //    thread.IsBackground = true;
                //    thread.Start();
                //    Thread.Sleep(61000);
                //}

                //if (DateTime.Now.Second == 0)
                //{
                //    Thread thread = new Thread(fnCheckCoinStatus);
                //    thread.IsBackground = true;
                //    thread.Start();
                //    Thread.Sleep(1000);
                //}
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Dictionary<string, cOrder> orders = new Dictionary<string, cOrder>();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;
            //Thread t = new Thread(tryToFindNewCoin);
            //t.Start();
            while (!stoppingToken.IsCancellationRequested)
            {
                if (fNewOrder)
                {
                    fNewOrder = false;
                    string sql = $@"select BINANCE_ORDER_ID, SYMBOL_CODE, BUY_QUANTITY, BUY_CLOSE_PRICE 
                                    from Ocean.dbo.[ORDER] 
                                    where 1=1 
                                    and BINANCE_SELL_ORDER_ID is null 
                                    and ORDER_SIDE = 'BUY' 
                                    and BINANCE_ORDER_ID != -1 ";

                    DataTable dt = cCommon.executeReader(sql);
                    orders.Clear();
                    List<string> symbols = new List<string>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        long buyOrderId = long.Parse((string)dr["BINANCE_ORDER_ID"]);
                        string buySymbolCode = (string)dr["SYMBOL_CODE"] ?? "";
                        decimal quantity = decimal.Parse((string)dr["BUY_QUANTITY"]);
                        decimal closePrice = decimal.Parse((string)dr["BUY_CLOSE_PRICE"]);
                        cOrder oOrder = new cOrder();
                        oOrder.orderId = buyOrderId;
                        oOrder.quantity = quantity;
                        oOrder.closePrice = closePrice;
                        //_= tryToSell(buyOrderId, buySymbolCode);
                        orders.Add(buySymbolCode, oOrder);
                        //symbols.Add(buySymbolCode);
                    }

                    if (orders.Count > 0)
                    {
                        Dictionary<string, decimal> dictSymbols = new Dictionary<string, decimal>();
                        //List<string> symbols = new List<string>();
                        using (var binanceClient = new BinanceRestClient())
                        {
                            var balance = await binanceClient.SpotApi.Account.GetBalancesAsync();
                            for (int i = 0; i < balance.Data.Count(); i++)
                            {
                                if (balance.Data.ToList()[i].Asset != "BTTC" && balance.Data.ToList()[i].Asset != "USDT" && balance.Data.ToList()[i].Asset != "BNB"
                                     && balance.Data.ToList()[i].Asset != "SPELL" && balance.Data.ToList()[i].Asset != "XRP")
                                {
                                    dictSymbols.Add(balance.Data.ToList()[i].Asset + "USDT", balance.Data.ToList()[i].Available);
                                    symbols.Add(balance.Data.ToList()[i].Asset + "USDT");
                                }
                            }

                            if (symbols.Count == 0)
                            {
                                //fFindNewCoin = true;
                                Thread.Sleep(5000);
                                fNewOrder = true;
                                continue;
                            }

                            var assets = await binanceClient.SpotApi.ExchangeData.GetExchangeInfoAsync(symbols); //binanceClient.SpotApi.ExchangeData.(pSymbols);

                            if (assets != null)
                            {
                                if (!assets.Success)
                                {
                                    //return BadRequest(balance.Error);
                                }
                                for (int x = 0; x++ < 30;) Console.WriteLine("");
                                Console.WriteLine(DateTime.Now.ToString() + " ****************** Sil Bastan ;-))))))) ");
                                Console.WriteLine("");
                                cancellationTokenSource.Cancel();
                                while (true)
                                {
                                    lock (threadObj)
                                    {
                                        if (threadCnt == 0)
                                            break;
                                    }
                                    Thread.Sleep(1000);
                                }
                                cancellationTokenSource = new CancellationTokenSource();
                                cancellationToken = cancellationTokenSource.Token;
                                Console.WriteLine("****************");
                                for (int i = 0; i < assets.Data.Symbols.Count(); i++)
                                {
                                    string buySymbolCode = assets.Data.Symbols.ToList()[i].Name;
                                    dictSymbols.TryGetValue(buySymbolCode, out decimal available);

                                    _ = orders.TryGetValue(buySymbolCode, out cOrder? oOrder);
                                    if (oOrder == null) oOrder = new cOrder();
                                    oOrder.buySymbolCode = buySymbolCode;
                                    BinanceSymbolPriceFilter? priceFilter = assets.Data.Symbols.ToList()[i].PriceFilter;
                                    if (priceFilter == null) priceFilter = new BinanceSymbolPriceFilter();
                                    oOrder.tickSize = priceFilter.TickSize;

                                    oOrder.baseAsset = assets.Data.Symbols.ToList()[i].BaseAsset;
                                    BinanceSymbol bs = new BinanceSymbol();
                                    if (assets.Data.Symbols.ToList()[i] != null)
                                        bs = assets.Data.Symbols.ToList()[i];

                                    BinanceSymbolLotSizeFilter? lotSizeFilter = bs.LotSizeFilter;
                                    decimal stepSize = (lotSizeFilter ?? new BinanceSymbolLotSizeFilter()).StepSize;
                                    oOrder.lotSize = stepSize;
                                    if (available < oOrder.lotSize) continue;
                                    decimal rnd = 1 / oOrder.tickSize;
                                    int num = int.Parse(Math.Round(rnd).ToString());
                                    int digit = num.ToString().Length - 1;
                                    oOrder.tickSize = Math.Round(oOrder.tickSize, digit);
                                    rnd = 1 / oOrder.lotSize;
                                    num = int.Parse(Math.Round(rnd).ToString());
                                    digit = num.ToString().Length - 1;
                                    available = Math.Round(available, digit);
                                    oOrder.lotSize = Math.Round(oOrder.lotSize, digit);

                                    Console.WriteLine(buySymbolCode + " quantity : " + available.ToString() + " oOrder.tickSize : " + oOrder.tickSize.ToString() + " oOrder.lotSize : " + oOrder.lotSize.ToString());
                                    _ = tryToSell(oOrder, cancellationToken);
                                }
                                Console.WriteLine("****************");
                            }
                        }
                    }
                }
                await Task.Delay(5000, stoppingToken);
                //fNewOrder = true;
            }
        }

        private static async void tryToFindNewCoin(string? code)
        {
            //string code = "";
            var binanceClient = new BinanceRestClient();
            while (true)
            {
                lock (coinObj)
                {
                    if (activeCoinCnt == maxCoin || fStop)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }
                }

                while (true)
                {
                    if (DateTime.Now.Second == 10)
                        break;
                    Thread.Sleep(500);
                }

                string sql = $@"  exec Ocean.dbo.spGet1Minute ";

                DataTable dt = cCommon.executeReader(sql);

                if (dt.Rows.Count == 0)
                    continue;

                foreach (DataRow dr in dt.Rows)
                {
                    code = ((string)dr["code"]).Replace(" ", "");
                    decimal? perc = decimal.Parse((string)dr["perc"]);

                    var assets = await binanceClient.SpotApi.ExchangeData.GetExchangeInfoAsync(code);
                    BinanceSymbol bs = new BinanceSymbol();
                    if (assets.Data.Symbols.ToList()[0] != null)
                        bs = assets.Data.Symbols.ToList()[0];

                    BinanceSymbolLotSizeFilter? lotSizeFilter = bs.LotSizeFilter;
                    decimal stepSize = (lotSizeFilter ?? new BinanceSymbolLotSizeFilter()).StepSize;
                    decimal lotSize = stepSize;
                    var price = await binanceClient.SpotApi.ExchangeData.GetPriceAsync(code ?? "");
                    decimal quantity = cAMOUNT / price.Data.Price;
                    quantity -= quantity % lotSize;
                    decimal rnd = 1 / lotSize;
                    int num = int.Parse(Math.Round(rnd).ToString());
                    int digit = num.ToString().Length - 1;
                    quantity = decimal.Round(quantity, digit);
                    _ = PostPlaceBuyOrder2(code ?? "", quantity);

                    lock (coinObj)
                    {
                        activeCoinCnt++;
                        if (activeCoinCnt == maxCoin)
                            break;
                    }
                }
            }
        }

        public static async Task<WebCallResult<BinancePlacedOrder>> PostPlaceBuyOrder2(string pSymbol = "USDT", decimal pQuantity = 1, decimal? pLimitPrice = null)
        {
            pSymbol = pSymbol.ToUpper();
            //update BUY sellOrderId = -1
            string sqlx = $@" update [Ocean].[dbo].[ORDER] set BINANCE_SELL_ORDER_ID = -1 where 1=1 and SYMBOL_CODE = '{pSymbol}' and ORDER_SIDE = 'BUY' and BINANCE_SELL_ORDER_ID is null";

            int ret1 = cCommon.executeNonQuery(sqlx);

            using (var binanceClient = new BinanceRestClient())
            {
                //if (pLimitPrice != null)
                //{
                //    string? newClientOrderId = null;
                //    //var sonuc = await binanceClient.SpotApi.Trading.PlaceOrderAsync(pSymbol, OrderSide.Buy, SpotOrderType.Market, quantity: pQuantity
                //    //    , newClientOrderId: newClientOrderId);

                //    var sonuc = await binanceClient.SpotApi.Trading.PlaceOrderAsync(pSymbol, OrderSide.Buy, SpotOrderType.Limit, quantity: pQuantity
                //        , newClientOrderId: newClientOrderId, price: pLimitPrice, timeInForce: TimeInForce.GoodTillCanceled);

                //    //if (!sonuc.Success)
                //    //{
                //    //    return BadRequest(sonuc.Error);
                //    //}
                //}
                //else

                string? newClientOrderId = null;

                var sonuc = await binanceClient.SpotApi.Trading.PlaceOrderAsync(pSymbol, OrderSide.Buy, SpotOrderType.Market, quantity: pQuantity
                    , newClientOrderId: newClientOrderId);

                //var sonuc = await binanceClient.SpotApi.Trading.PlaceOrderAsync(pSymbol, OrderSide.Buy, SpotOrderType.Limit, quantity: pQuantity
                //    , newClientOrderId: newClientOrderId, price: pLimitPrice, timeInForce: TimeInForce.GoodTillCanceled);

                //if (!sonuc.Success)
                //{
                //    return BadRequest(sonuc.Error);
                //}

                if (!sonuc.Success)
                {
                    lock (coinObj)
                    {
                        activeCoinCnt--;
                        return sonuc;
                    }
                }
                string sql = $@" insert into Ocean.dbo.[ORDER] (BINANCE_ORDER_ID, STATUS, SYMBOL_CODE, ORDER_SIDE, ORDER_TYPE, BUY_QUANTITY, BUY_CLOSE_PRICE, STOP_PRICE, BINANCE_SELL_ORDER_ID) 
                                                            values ({sonuc.Data.Id}, 'FILLED', '{pSymbol}', 'BUY', 'MARKET', {pQuantity}, {sonuc.Data.Price}, null, null);";

                int ret = cCommon.executeNonQuery(sql);
                if (ret > 0)
                    fNewOrder = true;
                //    cWorker.fFindNewCoin = true; 
                //return Ok(sonuc.Data);
                return sonuc;

            }
        }

        private async Task tryToSell(cOrder pOrder, CancellationToken pStoppingToken)
        {
            try
            {
                lock (threadObj)
                {
                    threadCnt++;
                }
                decimal currentPrice;

                var binanceClient = new BinanceRestClient();
                var balance = await binanceClient.SpotApi.Account.GetBalancesAsync(pOrder.baseAsset);
                var openOrders = await binanceClient.SpotApi.Trading.GetOrdersAsync(pOrder.buySymbolCode ?? "" );
                BinanceOrder bo = openOrders.Data.ToList()[openOrders.Data.Count() - 1];
                pOrder.closePrice = bo.Price;
                pOrder.orderId = bo.Id;
                pOrder.closePrice -= pOrder.closePrice % pOrder.tickSize;
                decimal minSellPrice = pOrder.closePrice - pOrder.tickSize * Math.Round(pOrder.closePrice * CONST_LOW_DIFF / 100 / pOrder.tickSize, 0);
                decimal available = 0;
                foreach (BinanceUserBalance bub in balance.Data)
                {
                    available = bub.Available;
                    break;
                }
                available -= available % pOrder.lotSize;
                decimal firstStep = pOrder.closePrice * (decimal)1.0029;
                firstStep -= firstStep % pOrder.tickSize;
                firstStep += pOrder.tickSize;
                decimal topPrice = firstStep;



                fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available, -1);
                //decimal i = (decimal) 0.5;
                while (!pStoppingToken.IsCancellationRequested)
                {
                    var price = await binanceClient.SpotApi.ExchangeData.GetPriceAsync(pOrder.buySymbolCode ?? "");

                    if (!price.Success)
                    {
                        lock (obj)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.BackgroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(DateTime.Now.ToString() + " Sorun var!!!!! : GetPriceAsync");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                        }

                        await Task.Delay(10000);
                        continue;
                    }

                    currentPrice = price.Data.Price;
                    currentPrice -= currentPrice % pOrder.tickSize;
                    decimal rnd = 1 / pOrder.tickSize;
                    int num = int.Parse(Math.Round(rnd).ToString());
                    int digit = num.ToString().Length - 1;
                    Console.Write(decimal.Round(currentPrice, digit).ToString() + "...");
                    //i += (decimal) 0.5;
                    //currentPrice = pOrder.closePrice * (100 + i)/100;
                    if (currentPrice <= minSellPrice)
                    {
                        string? newClientOrderId = null;

                        var sell = await binanceClient.SpotApi.Trading.PlaceOrderAsync(pOrder.buySymbolCode ?? "", OrderSide.Sell, SpotOrderType.Market, quantity: available, newClientOrderId: newClientOrderId);

                        if (sell.Success)
                        {
                            //cCoinMotor.fnSendingEmail(DateTime.Now.ToString() + " satıldı !!!! " + pOrder.buySymbolCode + " AverageFillPrice = " + sell.Data.AverageFillPrice.ToString() + " quantity : " + available.ToString()
                            //                        , " pOrder.orderId = " + pOrder.orderId.ToString() + " sell.Data.Id = " + sell.Data.Id.ToString());
                            lock (obj)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.BackgroundColor = ConsoleColor.Yellow;
                                Console.WriteLine(DateTime.Now.ToString() + " satıldı !!!! " + pOrder.buySymbolCode + " AverageFillPrice = " + sell.Data.AverageFillPrice.ToString() + " quantity : " + available.ToString() +
                                    " pOrder.orderId = " + pOrder.orderId.ToString() + " sell.Data.Id = " + sell.Data.Id.ToString());
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.BackgroundColor = ConsoleColor.Black;
                            }

                            lock (coinObj)
                            {
                                activeCoinCnt--;
                            }
                            //update BUY sellOrderId
                            string sql = $@"update [Ocean].[dbo].[ORDER] 
                                            set BINANCE_SELL_ORDER_ID = {sell.Data.Id} 
                                                , SELL_QUANTITY = {available} 
                                                , SELL_CLOSE_PRICE = {sell.Data.AverageFillPrice} 
                                                , UPDATED_DATE = getdate()
                                            where 1=1 and BINANCE_ORDER_ID =  {pOrder.orderId} ";

                            int ret = cCommon.executeNonQuery(sql);
                            break;
                        }
                    }

                    if (currentPrice >= firstStep)
                    {
                        if (currentPrice > topPrice) // - pOrder.tickSize))
                        {
                            topPrice = currentPrice;

                            //if (currentPrice >= pOrder.closePrice * (decimal)1.0009)
                            //{
                            //    minSellPrice = pOrder.closePrice * (decimal)1.0005;
                            //    topPrice = pOrder.closePrice * (decimal)1.0019;
                            //}

                            //if (currentPrice >= pOrder.closePrice * (decimal)1.0019)
                            //{
                            //    minSellPrice = pOrder.closePrice * (decimal)1.001;
                            //    topPrice = pOrder.closePrice * (decimal)1.0029;
                            //}

                            if (currentPrice >= pOrder.closePrice * (decimal)1.0029)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.002;
                                topPrice = pOrder.closePrice * (decimal)1.0099;
                            }

                            //if (currentPrice >= pOrder.closePrice * (decimal)1.0039)
                            //{
                            //    minSellPrice = pOrder.closePrice * (decimal)1.003;
                            //    topPrice = pOrder.closePrice * (decimal)1.0059;
                            //}

                            //if (currentPrice >= pOrder.closePrice * (decimal)1.0059)
                            //{
                            //    minSellPrice = pOrder.closePrice * (decimal)1.003;
                            //    topPrice = pOrder.closePrice * (decimal)1.009;
                            //}

                            //if (currentPrice >= pOrder.closePrice * (decimal)1.0079)
                            //{
                            //    minSellPrice = pOrder.closePrice * (decimal)1.007;
                            //    topPrice = pOrder.closePrice * (decimal)1.01;
                            //}

                            if (currentPrice >= pOrder.closePrice * (decimal)1.01)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.006;
                                topPrice = pOrder.closePrice * (decimal)1.0149;
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.015)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.01;
                                topPrice = pOrder.closePrice * (decimal)1.018;
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.019)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.015;
                                topPrice = pOrder.closePrice * (decimal)1.028;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.029)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.02;
                                topPrice = pOrder.closePrice * (decimal)1.038;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.039)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.025;
                                topPrice = pOrder.closePrice * (decimal)1.048;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.049)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.03;
                                topPrice = pOrder.closePrice * (decimal)1.058;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.059)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.04;
                                topPrice = pOrder.closePrice * (decimal)1.068;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.069)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.05;
                                topPrice = pOrder.closePrice * (decimal)1.078;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.079)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.055;
                                topPrice = pOrder.closePrice * (decimal)1.088;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.089)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.065;
                                topPrice = pOrder.closePrice * (decimal)1.098;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.099)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.07;
                                topPrice = pOrder.closePrice * (decimal)1.10;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.11)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.08;
                                topPrice = pOrder.closePrice * (decimal)1.119;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.12)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.085;
                                topPrice = pOrder.closePrice * (decimal)1.129;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.13)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.095;
                                topPrice = pOrder.closePrice * (decimal)1.139;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.14)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.105;
                                topPrice = pOrder.closePrice * (decimal)1.149;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.15)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.11;
                                topPrice = pOrder.closePrice * (decimal)1.159;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.16)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.12;
                                topPrice = pOrder.closePrice * (decimal)1.169;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.17)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.13;
                                topPrice = pOrder.closePrice * (decimal)1.179;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.18)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.14;
                                topPrice = pOrder.closePrice * (decimal)1.189;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.19)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.15;
                                topPrice = pOrder.closePrice * (decimal)1.199;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.20)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.15;
                                topPrice = pOrder.closePrice * (decimal)1.209;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.21)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.16;
                                topPrice = pOrder.closePrice * (decimal)1.219;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.22)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.17;
                                topPrice = pOrder.closePrice * (decimal)1.229;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.23)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.18;
                                topPrice = pOrder.closePrice * (decimal)1.24;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.24)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.19;
                                topPrice = pOrder.closePrice * (decimal)1.25;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.25)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.19;
                                topPrice = pOrder.closePrice * (decimal)1.26;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.26)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.20;
                                topPrice = pOrder.closePrice * (decimal)1.27;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.27)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.21;
                                topPrice = pOrder.closePrice * (decimal)1.28;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.28)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.22;
                                topPrice = pOrder.closePrice * (decimal)1.29;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.29)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.22;
                                topPrice = pOrder.closePrice * (decimal)1.30;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.30)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.23;
                                topPrice = pOrder.closePrice * (decimal)1.31;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.31)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.24;
                                topPrice = pOrder.closePrice * (decimal)1.32;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.32)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.25;
                                topPrice = pOrder.closePrice * (decimal)1.33;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.33)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.25;
                                topPrice = pOrder.closePrice * (decimal)1.34;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.34)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.26;
                                topPrice = pOrder.closePrice * (decimal)1.35;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.35)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.27;
                                topPrice = pOrder.closePrice * (decimal)1.36;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.36)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.27;
                                topPrice = pOrder.closePrice * (decimal)1.37;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.37)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.28;
                                topPrice = pOrder.closePrice * (decimal)1.38;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.38)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.29;
                                topPrice = pOrder.closePrice * (decimal)1.39;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.39)
                            {
                                minSellPrice = pOrder.closePrice * (decimal)1.30;
                                topPrice = pOrder.closePrice * (decimal)1.40;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }

                            if (currentPrice >= pOrder.closePrice * (decimal)1.40)
                            {
                                minSellPrice = currentPrice * 90 / 100;
                                topPrice = currentPrice; // * pOrder.tickSize;
                                //fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available);
                            }
                            fnWriteToScreen(pOrder, minSellPrice, topPrice, firstStep, available, currentPrice);
                        }
                    }
                    await Task.Delay(1000);
                }
            }
            catch (Exception pExc)
            {
                Exception exc = pExc;
            }
            finally
            {
                lock (threadObj)
                {
                    threadCnt--;
                }
            }
        }

        private static void fnWriteToScreen(cOrder pOrder, decimal pMinSellPrice, decimal pTopPrice, decimal pFirstStep, decimal pAvailable, decimal pCurrentPrice)
        {
            lock (obj)
            {
                decimal rnd = 1 / pOrder.tickSize;
                int num = int.Parse(Math.Round(rnd).ToString());
                int digit = num.ToString().Length - 1;
                pTopPrice = Math.Round(pTopPrice, digit);
                pMinSellPrice = Math.Round(pMinSellPrice, digit);
                pFirstStep = Math.Round(pFirstStep, digit);
                //pAvailable = Math.Round(pAvailable, digit);
                pCurrentPrice = Math.Round(pCurrentPrice, digit);
                pOrder.closePrice = Math.Round(pOrder.closePrice, digit);
                pOrder.tickSize = Math.Round(pOrder.tickSize, digit);
                Console.WriteLine("");
                Console.WriteLine("****************");
                Console.WriteLine(DateTime.Now.ToString() + " " + pOrder.buySymbolCode + " : " + pOrder.orderId.ToString() + " buy Price : " + pOrder.closePrice.ToString() + " current Price : " + pCurrentPrice.ToString() + " min sell price : " + pMinSellPrice.ToString() + " topPrice = " + pTopPrice.ToString()); // + " firstStep = " + pFirstStep.ToString());
                Console.WriteLine("****************");
            }
        }
    }
}
