using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace TestBot.Dialogs
{
    [Serializable]
    public class ImageDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(GifDialog);
        }

        public async Task GifDialog(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var inboundMessage = await argument;
            var outboundMessage = context.MakeMessage();

            //giphy api fail

            //var client = new HttpClient() { BaseAddress = new Uri("http://api.giphy.com") };
            //var result = client.GetStringAsync("/v1/gifs/trending").Result;
            //var data = ((dynamic)JObject.Parse(result)).data;
            //var gif = data[(int)Math.Floor(new Random().NextDouble() * data.Count)];
            //var gifUrl = gif.images.fixed_height.url.Value;
            //var slug = gif.slug.Value;

            //outboundMessage.Attachments = new List<Attachment>();
            outboundMessage.Attachments.Add(new Attachment()
            {
                ContentUrl = "https://upload.wikimedia.org/wikipedia/en/a/a6/Bender_Rodriguez.png",
                ContentType = "image/png",
                Name = "Bender_Rodriguez.png"
            });
            await context.PostAsync(outboundMessage);
            context.Done(outboundMessage);
        }
    }
}
