using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects.Models.Spot;
using CoinAPI.Coin;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace CoinAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoinAPIController : ControllerBase
    {
        private static bool init = false;
        private readonly string _apiKey;
        private readonly string _secretKey;
        private readonly ILogger<CoinAPIController> _logger;

        public CoinAPIController(ILogger<CoinAPIController> logger)
        {
            _logger = logger;
            _apiKey = AppSettings.apiKey;
            _secretKey = AppSettings.secretKey;
        }

        // GET methods for BINANCE

        [HttpGet("getAccount")]
        public async Task<IActionResult> getAccount()
        {
            //var optionsApiCredentials = new ApiCredentials(_apiKey, _secretKey); 

            using (var binanceClient = new BinanceRestClient())
            {
                var balance = await binanceClient.SpotApi.Account.GetAccountInfoAsync(omitZeroBalances: true);

                if (!balance.Success)
                {
                    return BadRequest(balance.Error);
                }

                return Ok(balance.Data);
            }
        }

        [HttpGet("getBalances")]
        public async Task<IActionResult> getBalances(string? pAsset)
        {
            using (var binanceClient = new BinanceRestClient())
            {
                var balance = await binanceClient.SpotApi.Account.GetBalancesAsync(pAsset);

                if (!balance.Success)
                {
                    return BadRequest(balance.Error);
                }

                return Ok(balance.Data);
            }
        }

        [HttpGet("getExchangeInfo")]
        public async Task<IActionResult> getExchangeInfo(string? pSymbols)
        {
            using (var binanceClient = new BinanceRestClient())
            {
                string token = "056cdbce3d6c424d87415aca776f4f5543685934887";
string url = WebUtility.UrlEncode("https://bigpara.hurriyet.com.tr/borsa/canli-borsa/tum-hisseler/?harf=P");
var client = new HttpClient();
var requestURL = $"https://api.scrape.do/?token={token}&url={url}";        
var request = new HttpRequestMessage(HttpMethod.Get, requestURL);
var response = client.SendAsync(request).Result;
var content = response.Content.ReadAsStringAsync().Result;
Console.WriteLine(content);
                //pSymbols = pSymbols.Replace("\"", "").Replace(" ", "").Replace("[", "").Replace("]", "");
                //List<string> symbols = pSymbols.Split(',').ToList<string>();
                var balance = await binanceClient.SpotApi.ExchangeData.GetExchangeInfoAsync(""); //binanceClient.SpotApi.ExchangeData.(pSymbols);

                if (!balance.Success)
                {
                    return BadRequest(balance.Error);
                }

                return Ok(balance.Data);
            }
        }

        [HttpGet("getWalletBalances")]
        public async Task<IActionResult> getWalletBalances()
        {
            //var optionsApiCredentials = new ApiCredentials(_apiKey, _secretKey); 

            using (var binanceClient = new BinanceRestClient())
            {
                var balance = await binanceClient.SpotApi.Account.GetWalletBalancesAsync();

                if (!balance.Success)
                {
                    return BadRequest(balance.Error);
                }

                return Ok(balance.Data);
            }
        }

        [HttpGet("getOpenOrders")]
        public async Task<IActionResult> getOpenOrders(string? pSymbol)
        {
            using (var binanceClient = new BinanceRestClient())
            {
                var sonuc = await binanceClient.SpotApi.Trading.GetOpenOrdersAsync(pSymbol);
                
                if (!sonuc.Success)
                {
                    return BadRequest(sonuc.Error);
                }

                return Ok(sonuc.Data);
            }
        }

        [HttpGet("getMerhabaIdo")]
        public IActionResult getMerhabaIdo()
        {
            return Ok("Merhaba KARDEŞ!!! " + DateTime.Now.ToString());
        }

        // POST methods for BINANCE

        [HttpPost("postPlaceBuyOrder")]
        public async Task<IActionResult> postPlaceBuyOrder(string pSymbol = "USDT", decimal pQuantity =1, decimal? pLimitPrice=null)
        {
            var sonuc = await cCoinWorker.PostPlaceBuyOrder2(pSymbol, pQuantity, pLimitPrice);
            //if (!sonuc.Result.Success)
            //    return BadRequest(sonuc.Result.Error);
            return Ok(""); //sonuc.Result.Data);  
        }

        [HttpPost("postCancelOrder")]
        public async Task<IActionResult> postCancelOrder(string pSymbol, long? pOrderId)
        {
            using (var binanceClient = new BinanceRestClient())
            {
                try
                {
                    var sonuc = await binanceClient.SpotApi.Trading.CancelOrderAsync(pSymbol, pOrderId);
                    if (!sonuc.Success)
                    {
                        return BadRequest(sonuc.Error);
                    }

                    return Ok(sonuc.Data);
                }
                catch (Exception pExc)
                {
                    Exception exc = pExc;
                    return BadRequest();
                }
            }
        }

        [HttpPost("postPlaceSellOrder")]
        public async Task<IActionResult> postPlaceSellOrder(string pSymbol, decimal? pQuantity, decimal? pStopPrice)
        {
            using (var binanceClient = new BinanceRestClient())
            {
                string? newClientOrderId = null;
                //long? orderId = null;
                var sonuc = await binanceClient.SpotApi.Trading.PlaceOrderAsync(pSymbol, OrderSide.Sell, SpotOrderType.StopLoss, quantity: pQuantity
                    , stopPrice: pStopPrice
                    //, price: pPrice
                    //, timeInForce: TimeInForce.GoodTillCanceled
                    , newClientOrderId: newClientOrderId);

                if (!sonuc.Success)
                {
                    return BadRequest(sonuc.Error);
                }

                return Ok(sonuc.Data);
            }
        }

        // POST methods for CoinAPI

        [HttpPost("postInitMotor")]
        public IActionResult postInitMotor()
        {
            if (!init)
            {
                init = true;
                new cCoinMotor().fnInit();
            }
            
            return Ok();
        }

        [HttpPost("postSendingEmail")]
        public IActionResult postSendingEmail(string pSubject, string pBody)
        {
            //cCoinMotor.fnSendingEmail(pSubject, pBody);
            return Ok();
        }

        [HttpPost("postStartMotor")]
        public IActionResult postStartMotor()
        {
            cCoinMotor.fnStart();
            return Ok();
        }

        [HttpPost("postStopMotor")]
        public IActionResult postStopMotor()
        {
            cCoinMotor.fnStop();
            return Ok();
        }
    }
}
