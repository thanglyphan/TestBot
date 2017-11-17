using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestBot.ObjectsFromWit;

namespace TestBot.Dialogs
{
    [Serializable]
    public class EconomyDialog : IDialog<object>
    {
        private string message;
        private string replyCheck = 
            "Lønn får du inn på konto den 20. hver måned. " +
            "Det finnes dog to unntak: dersom 20. faller på en helgedag vil lønnen komme fredagen før. " +
            "Dersom 20. faller på en rød dag, vil du få lønnen utbetalt i forkant, på siste ordinære arbeidsdag.";
        private string replyVacationMoney =
            "Feriepengene kommer aldri";
        private string replyAskForMoreHelp = "Var det noe mer jeg kan hjelpe deg med?";
        private WitOkonomi[] okonomi;
     
        public EconomyDialog(IMessageActivity message, WitOkonomi[] okonomi)
        {
            this.message = message.Text.ToLower();
            this.okonomi = okonomi;
        }

        public async Task StartAsync(IDialogContext context)
        {
            foreach(var item in okonomi)
            {
                //foreach(var lonn in item.WitLonn)
                //{
                //    if (message.Contains(lonn))
                //    {
                //        await context.PostAsync(ReplyToUser(context, this.replyCheck));
                //        await context.PostAsync(ReplyToUser(context, this.replyAskForMoreHelp));
                //    }
                //}
                //foreach (var feriepenger in item.Feriepenger)
                //{
                //    if (message.Contains(feriepenger))
                //    {
                //        await context.PostAsync(ReplyToUser(context, this.replyVacationMoney));
                //        await context.PostAsync(ReplyToUser(context, this.replyAskForMoreHelp));
                //    }
                //}
                //TODO: Broken. Replace with new structure
            }
            context.Done<object>(new Object());
        }
        private IMessageActivity ReplyToUser(IDialogContext context, string replyText)
        {
            var reply = context.MakeMessage();
            reply.Text = replyText;
            return reply;
        }
    }
}
