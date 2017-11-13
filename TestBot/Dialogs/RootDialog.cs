using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;
using TestBot.ObjectsFromWit;

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


                WitObjectStructure witObjectStructure = new WitObjectStructure(api.response);

                if(witObjectStructure.data.entities.intent == null)
                {
                    await context.PostAsync("Jeg forstår ikke hva du vil. Kan du omformulere spørsmålet?");
                }
                else
                //foreach (var item in witObjectStructure.data.entities.intent)
                //{
                //    await context.PostAsync("Intent: " + item.value + ". Confidence: " + item.confidence);
                //    if (item.value.ToLower() == "hilsen")
                //    {
                //        await context.PostAsync("Hello, my name is TechBot. How can I help you?");

                //    }
                //}
                //foreach (var item in witObjectStructure.data.entities.organisasjon)
                //{
                //    await context.PostAsync("Entity: " + location.data.entities.organisasjon.ToString() + ". Keyword: " + item.value + ". Confidence: " + item.confidence);
                //}

                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterChildDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(this.MessageReceivedAsync);
        }
    }
}