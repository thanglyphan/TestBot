using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace TestBot.Dialogs
{
    public class Networking
    {
        public string response { get; set; }
        public string[] responseEntities { get; set; }
        public Networking()
        {

        }
        public void ConnectToWit(string message)
        {
            string URL = $"https://api.wit.ai/message?v={DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}&q={message}";
            Console.WriteLine(URL);
            //THANG string accessToken = "HIRXY32E3OLB6JVRFXNDCUMNU3MB55BN";
            string accessToken = "YHOYLGOGY2ZF2ONV465TXJDD4RBL5OC2";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "POST";
            request.PreAuthenticate = true;
            request.Headers.Add("Authorization", "Bearer " + accessToken);
            request.Accept = "application/json";

            var response = request.GetResponse();
            var responseStream = response.GetResponseStream();
            if (responseStream == null)
            {
                Console.WriteLine("ER NULL");
            }
            else
            {
                var reader = new StreamReader(responseStream);
                this.response = reader.ReadToEnd();
                Console.WriteLine(reader.ReadToEnd());
            }
        }
        public void GetEntities()
        {
            string URL = $"https://api.wit.ai/entities?v={DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}";
            string accessToken = "YHOYLGOGY2ZF2ONV465TXJDD4RBL5OC2";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";
            request.PreAuthenticate = true;
            request.Headers.Add("Authorization", "Bearer " + accessToken);
            request.Accept = "application/json";

            var response = request.GetResponse();
            var responseStream = response.GetResponseStream();
            if (responseStream == null)
            {
                Console.WriteLine("ER NULL");
            }
            else
            {
                var reader = new StreamReader(responseStream);
                var entityString = reader.ReadToEnd();
                var temp = entityString.Replace("\"", "").Replace("[", "").Replace("]", "");
                this.responseEntities = temp.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var finalArray = new List<string>();
                foreach (var item in this.responseEntities)
                {
                    if (!item.Contains("$"))
                    {
                        finalArray.Add(item);
                    }
                }
                finalArray.ForEach(a => Console.WriteLine(a));
            }
        }
    }
}
