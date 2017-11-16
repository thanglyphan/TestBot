using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace TestBot.Dialogs
{
    [Serializable]
    public class EconomyDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("[EconomyDialog]");
            context.Wait(MessageReceived);
        }

        private async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var userInput = message.Text.ToLower();
            if (userInput.Contains("tilbake") || userInput.Contains("avbryt"))
            {
                context.Done<object>(new Object());
            }
            if ((message.Text != null) && (message.Text.Trim().Length > 0))
            {
                if (message.Text.ToLower().Contains("når"))
                {
                    var replyMessage = context.MakeMessage();
                    replyMessage.Text = "Lønn får du inn på konto den 20. hver måned. " +
                        "Det finnes dog to unntak: dersom 20. faller på en helgedag vil lønnen komme fredagen før. " +
                        "Dersom 20. faller på en rød dag, vil du få lønnen utbetalt i forkant, på siste ordinære arbeidsdag.";
                    await context.PostAsync(replyMessage);
                }
                context.Done<object>(new Object());
            }
            else
            {
                context.Fail(new Exception("Message was not a string or was an empty string."));
            }
        }
    }
}
