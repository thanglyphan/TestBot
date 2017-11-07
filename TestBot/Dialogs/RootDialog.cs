using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading;

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
                await context.Forward(new EchoDialog() , this.EchoDialogResumeAfter, message, CancellationToken.None);
                context.Wait(this.MessageReceivedAsync);
            }
            else if (message.Text.ToLower().Contains("bender"))
            {
                await context.Forward(new ImageDialog(), this.ImageDialogResumeAfter, message, CancellationToken.None);
                context.Wait(this.MessageReceivedAsync);
            }
            var activity = await result as Activity;
            int length = (activity.Text ?? string.Empty).Length;
			Networking api = new Networking();
            api.ConnectToWit(activity.Text);            
			await context.PostAsync($"You sent '{activity.Text}' which was {length} characters long.");
            context.Wait(MessageReceivedAsync);
        }

        private async Task EchoDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            var message = await result;
            await context.PostAsync("You made the bot mimic your sentence, way to go!");
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task ImageDialogResumeAfter(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result;
            await context.PostAsync("Would you like me to find another, just type 'GIF' OR 'GIPHY'.");
            context.Wait(this.MessageReceivedAsync);
        }
    }
}