using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace TestBot.Dialogs
{
    [Serializable]
    public class DocumentFinderDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var message = context.Activity;
            await context.PostAsync("[DocumentFinderDialog]");

            context.Wait(this.MessageReceived);
        }
        private async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            bool requestHandled = false;
            var message = await result as Activity;
            requestHandled = true;
            context.Done<bool>(requestHandled);
        }
    }
}
