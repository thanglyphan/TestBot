using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading;
using System.Linq;
using System.Web.Script.Serialization;
using TestBot.ObjectsFromWit;
using Newtonsoft.Json;

namespace TestBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            //if (ContainsGreeting(message.Text)) { }

            if (message.Text.ToLower().Contains("repeat after me") || message.Text.ToLower().Contains("gjenta etter meg"))
            {
                context.Call<object>(new EchoDialog(), this.ResumeAfterChildDialog);
            }
            else if (message.Text.ToLower() == "picture")
            {
                context.Call<object>(new ImageDialog(), this.ResumeAfterChildDialog);
            }
            else
            {
                var activity = await result as Activity;
                int length = (activity.Text ?? string.Empty).Length;
                Networking api = new Networking();
                api.ConnectToWit(activity.Text);
                Locations location = new Locations(api.response);

                foreach (var item in location.data.entities.intent)
                {
                    await context.PostAsync("Intent: " + item.value + ". Confidence: " + item.confidence);
                    if (item.value.ToLower() == "hilsen")
                    {
                        await context.PostAsync("Hello, my name is TechBot. How can I help you?");

                    }
                }
                //foreach (var item in location.data.entities.lol)
                //{
                //    await context.PostAsync("Entity: " + item.value + ". Confidence: " + item.confidence);
                //}
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterChildDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(this.MessageReceivedAsync);
        }
        public bool ContainsGreeting(string result)
        {
            var message = result;
            bool isGreeting = false;
            string[] greetings = new string[] { "hello", "hi", "greetings", "hey", "hei", "hallo", "halla", "yo", "heisann" };

            if (greetings.Any(message.ToLower().Contains))
                isGreeting = true;
            return isGreeting;
        }
    }
}