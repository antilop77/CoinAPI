using CoinAPI.Betting.Model;
using CoinAPI.Coin.Model;
using CryptoExchange.Net.CommonObjects;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices.JavaScript;

namespace CoinAPI.Betting
{
    public class cBettingWorker
    {
        public static async Task doBettingProcesses()
        {
            Console.WriteLine(DateTime.Now.ToString()  + " started.");
            Int64 y = -1;
            var options = new RestClientOptions("https://api.betting-api.com");
            var client = new RestClient(options);
            string ratio = AppSettings.ratio; //"1.5";
            DateTime preTime = DateTime.Now;
            DateTime now;
            while (true)
            {
                Console.WriteLine("preTime = " + preTime.ToString());
                now = DateTime.Now;
                Console.WriteLine("now     = " + now.ToString());
                double diff = (now - preTime).TotalSeconds;
                
                Console.WriteLine("diff = " + Convert.ToInt32(diff).ToString() + " " + (72-Convert.ToInt32(diff)).ToString() + " sn bekle");
                if ((72 - Convert.ToInt32(diff)) > 0)
                {
                    Console.WriteLine((72-Convert.ToInt32(diff)).ToString() + " sn uyuyorum.");
                    Thread.Sleep((72-Convert.ToInt32(diff))*1000); //Free Plan >>> 50 requests / hour >>> 60*60/50 = 72 seconds
                }
                preTime = DateTime.Now;
                
                Console.WriteLine(DateTime.Now.ToString()  + " Scores are being gotten.");
                var request = new RestRequest("/1xbet/football/live/all?token=021c3c2fb6044e20b8b5758487427e6ecaa430b9f9fd497c862e390306d83837", Method.Get);
                Console.WriteLine(DateTime.Now.ToString()  + " Scores were gotten.");
                RestResponse response = await client.ExecuteAsync(request);
                string content = response.Content ?? "";
                List<cBetting>? bettings = JsonConvert.DeserializeObject<List<cBetting>>(content);
                Int64 x = Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmmmss"));
                Console.WriteLine(DateTime.Now.ToString()  + " Match results are being saved.");
                fnSaveLiveScores(bettings, x);
                Console.WriteLine(DateTime.Now.ToString()  + " Match results were saved.");
                Console.WriteLine(DateTime.Now.ToString()  + " ratio = " + ratio.ToString() + " Scores were just saved. X = " + x.ToString() + " Y = " + y.ToString() + "\n");
                
                if (y != -1 && 1==1)
                {
                    string sql = "exec  [dbo].[spGetBettableMatches2] " + ratio;
                    Console.WriteLine(DateTime.Now.ToString()  + " Match results are being reported.");
                    DataTable dt = cCommon.executeReader(sql);
                    Console.WriteLine(DateTime.Now.ToString()  + " Match results were reported.");
                    //await cCoinMotor.fnSendingEmail(ratio, "oha beee");
                    if (dt.Rows.Count != 0)
                        Console.WriteLine(DateTime.Now.ToString()  + " Potential results are being notified.");
                        foreach (DataRow dr in dt.Rows)
                        {
                            string body = dr["Rule"].ToString() + " : " + dr["x"].ToString() + " : " + dr["match_Id"].ToString() + " : " + (string)dr["league"] + " >>> " + (string)dr["team1"] + " --- " + (string)dr["team2"] + " >>> " + (string)dr["favourite"] 
                                + " >>> minute : " + dr["MINUTE"]+ " >>> WIN1 = " + dr["WIN1"]+ " >>> WIN2 = " + dr["WIN2"];
                            Console.WriteLine(body);
                            await cCoinMotor.fnSendingEmail(ratio, body);
                            sql = $@" insert into Ocean.dbo.NOTIFICATION (X, TOPIC, MESSAGE) 
                                                values ({x}, 'BET', '{ratio.ToString()} - {body}');";
                            int ret = cCommon.executeNonQuery(sql);
                        }
                        Console.WriteLine(DateTime.Now.ToString()  + " Potential results were notified.");
                }
                y = x;
                Console.WriteLine("bitti");
            }
        }

        public static void fnSaveLiveScores(List<cBetting>? pBettings, Int64 x)
        {
            if (pBettings == null) { return; }
            string sql = "";
            foreach (cBetting betting in pBettings)
            {
                if (betting == null) { return; }
                if (betting?.league?.name == "FIFA 25. Cyber League"
                    || betting?.league?.name == "Indonesia. Liga 4"
                    || betting?.league?.name == "Kazakhstan. League 2"
                    || betting?.league?.name == "Student League 2"
                    || betting?.league?.name == "Subsoccer"
                    || betting?.league?.name == "Student League"
                    || betting?.league?.name == "Spain. Tercera Division RFEF"
                    )
                    continue;
                if (!betting?.team1?.Contains('/') ?? false)
                {
                    if (betting?.markets?.win1 == null || betting?.markets.winX == null || betting?.markets.win2 == null)
                        continue;
                    try
                    {
                        sql = $@" insert into Ocean.dbo.SCORE (x, MATCH_ID, LEAGUE, TEAM1, TEAM2, SCORE1, SCORE2, IS_LIVE, HALF_ORDER_INDEX, MINUTE, SECONDS, WIN1, WINX, WIN2) 
                                               values ({x}, {betting?.id}, '{betting?.league?.name}', '{betting?.team1}', '{betting?.team2}', {betting?.score1}, {betting?.score2}
                                                        , {(betting?.isLive == false ? 0 : 1)}, {betting?.half_order_index}, {betting?.minute}, {betting?.seconds}
                                                        , {Convert.ToDecimal(betting?.markets.win1.v.ToString())}
                                                        , {Convert.ToDecimal(betting?.markets.winX.v.ToString())}
                                                        , {Convert.ToDecimal(betting?.markets.win2.v.ToString())});";

                        int ret = cCommon.executeNonQuery(sql);
                    }
                    catch (Exception pExc)
                    {
                        Exception exc = pExc;
                    }
                }
            }
        }
    }
}
