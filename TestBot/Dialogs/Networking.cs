using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using TestBot.ObjectsFromWit;

namespace TestBot.Dialogs
{
    public class Networking
    {
        public string response { get; set; }
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
            } else
            {
                var reader = new StreamReader(responseStream);

                this.response = reader.ReadToEnd();

                //Locations location = JsonConvert.DeserializeObject<Locations>(this.response);

                Console.WriteLine(reader.ReadToEnd());
            }


        }
    }
}
