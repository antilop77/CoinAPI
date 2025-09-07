using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Binance.Net.Objects.Models.Spot.Socket;
using CoinAPI.Betting;
using CoinAPI.Betting.Model;
using CoinAPI.Coin;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.CommonObjects;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Testing;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Net.Mail;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CoinAPI
{
    public class cCoinMotor
    {
        private static bool active = false;
        private static bool init = false;
        

       

        public static async Task fnSendingEmail(string pSubject, string pBody)
        {
            try {
                await cFirebase.fnGetFirebaseToken();
                var firebaseToken = cFirebase.fnGetFirebaseToken().Result;
                string token = firebaseToken.ToString();

                await cFirebase.fnSendNotification(token, "BET " + DateTime.Now.ToString() + " " + pSubject, pBody);
                return;
            }
            catch (Exception pExc)
            {
                Exception exc = pExc;
            }

            
            const string smtpHostAddress = "smtp.gmail.com"; //"smtp.mail.yahoo.com";
            const string adminEmailAddress = "pacificoceanido@gmail.com"; //"ocean_ido@yahoo.com";
            const string adminEmailPassword = "xinv grvy fskl oetw";

            //FINALLY LETS CREATE SMTP OBJECT TO SEND THE EMAILS TO ADMIN AND THE USER
            var smtp = new SmtpClient
            {
                Host = smtpHostAddress,
                Port = 587,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential
                    (adminEmailAddress, adminEmailPassword),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("pacificoceanido@gmail.com");
            //mailMessage.From = new MailAddress("ocean_ido@yahoo.com");
            mailMessage.Subject = "IDO-BETTing " + pSubject + " ("  + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + ")";
            mailMessage.Body = pBody; // "BUY filled";
            mailMessage.To.Add("ocean_ido@yahoo.com");
            //mailMessage.To.Add("baris.simsek@outlook.com");
            //mailMessage.To.Add("pacificoceanido@gmail.com");
            //SEND THE EMAILS OUT
            smtp.Send(mailMessage);
        }
        public static void fnStart()
        {
            active = true;
        }
        public static void fnStop()
        {
            active = false;
        }
        public void fnInit()
        {
            if (init) return;
            init = true;
            fnStart();
            Thread thread = new Thread(fnMotor);
            thread.IsBackground = true;
            thread.Start();
        }
        public void fnMotor()
        {
            while (1 == 1)
            {
                if (!active)
                {
                    Thread.Sleep(1000);
                    continue;
                }
                active = false;

                _= cBettingWorker.doBettingProcesses();
                //cCoinWorker.doCoinProcesses();
            }
        }      
    }
}
