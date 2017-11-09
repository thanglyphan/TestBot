using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestBot.Dialogs
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        protected int count = 1;

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("I will repeat what you say until you tell me to 'stop repeating'.");
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            if (message.Text.Contains("stop repeating"))
            {
                PromptDialog.Confirm(
                    context,
                    AfterRepeatAsync,
                    "Are you sure you want me to stop repeating?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.None);
            }
            else
            {
                await context.PostAsync(message.Text);
                context.Wait(MessageReceivedAsync);
            }
        }

        public async Task AfterRepeatAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Stopping repeat.");
                context.Done<object>(new Object());
            }
            else
            {
                await context.PostAsync("Will continue to repeat.");
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}
