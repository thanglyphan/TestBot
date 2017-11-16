using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace TestBot.Dialogs
{
    public class Networking
    {
        private const string AccessToken = "YHOYLGOGY2ZF2ONV465TXJDD4RBL5OC2";

        public string GetResponseForMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return string.Empty;
            var url = CreateUrlToWit(message, DateTime.UtcNow);
            Console.WriteLine(url);
            var request = CreatePostRequest(url);
            try
            {
                return GetResponse(request);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return string.Empty;
            }
        }

        private static string CreateUrlToWit(string message, DateTime timeNow)
        {
            return $"https://api.wit.ai/message?v={timeNow.Day}.{timeNow.Month}.{timeNow.Year}&q={message}";
        }

        private static string CreateEntitiesUrlToWit(DateTime timeNow)
        {
            return $"https://api.wit.ai/entities?v={timeNow.Day}.{timeNow.Month}.{timeNow.Year}";
        }

        private static HttpWebRequest CreatePostRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.PreAuthenticate = true;
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
            request.Accept = "application/json";
            return request;
        }

        private static HttpWebRequest CreateGetRequest(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.PreAuthenticate = true;
            request.Headers.Add("Authorization", "Bearer " + AccessToken);
            request.Accept = "application/json";
            return request;
        }

        private static string GetResponse(HttpWebRequest request)
        {
            var responseStream = request
                .GetResponse()
                .GetResponseStream();
            if (responseStream == null)
                return string.Empty;

            using (var reader = new StreamReader(responseStream))
            {
                return reader.ReadToEnd();
            }
        }

        public List<string> GetEntities()
        {
            var url = CreateEntitiesUrlToWit(DateTime.UtcNow);
            var request = CreateGetRequest(url);
            try
            {
                var response = GetResponse(request);
                var entities = GetEntitiesFromResponse(response);
                return entities?.ToList() ?? new List<string>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<string>();
            }
        }

        private static IEnumerable<string> GetEntitiesFromResponse(string entityString)
        {
            if (string.IsNullOrEmpty(entityString))
                return new List<string>();
            var trimmedEntityString = entityString
                .Replace("\"", "")
                .Replace("[", "")
                .Replace("]", "");
            var entities = trimmedEntityString
                .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return entities
                .Where(e => !e.Contains("$"));
        }
    }
}
