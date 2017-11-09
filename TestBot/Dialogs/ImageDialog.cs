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
            await context.PostAsync("I can display pictures for you.");
            context.Wait(GifDialog);
        }

        public async Task GifDialog(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var inboundMessage = await argument;
            var outboundMessage = context.MakeMessage();

            if (inboundMessage.Text.ToLower().Equals("gif"))
            {
                var client = new HttpClient() { BaseAddress = new Uri("https://api.giphy.com/") };
                var result = client.GetStringAsync("v1/gifs/trending?api_key=gvShFLoFcNDqyDxuMQUQpFSmY5Ydr6R4&limit=25&rating=G").Result;
                var data = ((dynamic)JObject.Parse(result)).data;
                var gif = data[(int)Math.Floor(new Random().NextDouble() * data.Count)];
                var gifUrl = gif.images.fixed_height.url.Value;
                var slug = gif.slug.Value;

                outboundMessage.Attachments = new List<Attachment>();
                outboundMessage.Attachments.Add(new Attachment()
                {
                    ContentUrl = "gifUrl",
                    ContentType = "image/gif",
                    Name = slug + ".gif"
                });
                await context.PostAsync(outboundMessage);
                context.Done<object>(new Object());
            }
            else if (inboundMessage.Text.ToLower().Equals("bender"))
            {
                outboundMessage.Attachments.Add(new Attachment()
                {
                    ContentUrl = "https://upload.wikimedia.org/wikipedia/en/a/a6/Bender_Rodriguez.png",
                    ContentType = "image/png",
                    Name = "Bender_Rodriguez.png"
                });
                await context.PostAsync($"Here is the only picture I have of '{inboundMessage.Text}': \n");
                await context.PostAsync(outboundMessage);
                context.Done<object>(new Object());
            }
            else
            {
                await context.PostAsync("Didn't get that, can you repeat the request?");
                context.Wait(GifDialog);
            }
        }
    }
}
