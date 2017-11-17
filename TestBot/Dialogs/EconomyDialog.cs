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
        private string replyCheck = 
            "Lønn får du inn på konto den 20. hver måned. " +
            "Det finnes dog to unntak: dersom 20. faller på en helgedag vil lønnen komme fredagen før. " +
            "Dersom 20. faller på en rød dag, vil du få lønnen utbetalt i forkant, på siste ordinære arbeidsdag.";
        private string replyVacationMoney =
            "Feriepengene kommer aldri";
        private string replyAskForMoreHelp = "Var det noe mer jeg kan hjelpe deg med?";
     
        public async Task StartAsync(IDialogContext context)
        {
           //TODO: implemented by Thang 
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
