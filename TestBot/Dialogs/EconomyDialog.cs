using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestBot.ObjectsFromWit;
using TestBot.Dialogs.Assets;

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
        private string replyExpenses =
            "Du har lagt ut 900kr for en middag for 1";
        private string replyOldMoney =
            "Pensjon er oppskrytt";
        private string replyMoreHelp = "Var det noe mer jeg kan hjelpe deg med?";
        private string type;
        private Enums entityIndex;
        public EconomyDialog(string type, string entity)
        {
            this.type = type;
            this.entityIndex = InitializeEnum(entity);
        }

        private Enums InitializeEnum(string entity)
        {
            switch (entity)
            {
                case "utlegg": return Enums.Utlegg;
                case "pensjon": return Enums.Pensjon;
                case "feriepenger": return Enums.Feriepenger;
                case "lønn": return Enums.Lønn; 
                default: return Enums.Tom; 
            }
        }
        
        public async Task StartAsync(IDialogContext context)
        {
            switch(this.type)
            {
                case "tidspunkt": await GetPostAsync(context, this.entityIndex); break;
                default: await context.PostAsync("HÆ"); break;
            }
            context.Done<object>(new Object());
        }
        private async Task GetPostAsync(IDialogContext context, Enums entityType)
        {
            await context.PostAsync(ReplyToUser(context, entityType));
            await context.PostAsync(ReplyToUser(context, 0));

        }

        private IMessageActivity ReplyToUser(IDialogContext context, Enums entity)
        {
            var reply = context.MakeMessage();
            switch(entity)
            {
                case Enums.Utlegg:
                    reply.Text = replyExpenses;
                    break;
                case Enums.Lønn:
                    reply.Text = replyCheck;
                    break;
                case Enums.Pensjon:
                    reply.Text = replyOldMoney;
                    break;
                case Enums.Feriepenger:
                    reply.Text = replyVacationMoney;
                    break;
                default:
                    reply.Text = replyMoreHelp;
                    break;
            }
            return reply;
        }
    }
}
