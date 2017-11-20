using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using TestBot.ObjectsFromWit;
using System.Collections.Generic;
using System.Threading;
using TestBot.Cards;
using Newtonsoft.Json;
using TestBot.Models;

namespace TestBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.PostAsync("[RootDialog]");
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result as Activity;

            var api = new Networking();
            var response = api.GetResponseForMessage(activity?.Text);
            if (string.IsNullOrEmpty(response))
            {
                await context.PostAsync("Something is wrong. I was unable to process your request");
            }
            Console.WriteLine("Data from Wit.Ai:");
            Console.WriteLine(response);


            var witObject = new WitObjectStructure(response);
            if (witObject.Data == null)
            {
                await context.PostAsync("Something is wrong. I was unable to process your request");
            }
            var data = new MessageData(witObject.Data);
            Console.WriteLine("MessageData:");
            Console.WriteLine(JsonConvert.SerializeObject(data));

            var messageIntent = witObject.Data?.Entities?.Intent;
            if (messageIntent == null)
            {
                await context.PostAsync("Jeg forstår ikke hva du vil. Kan du omformulere spørsmålet?");
            }
            else
            {
                foreach (var item in messageIntent)
                {
                    if (item.Value.ToLower() == "hjelp")
                    {
                        await ShowOptionsAsync(context);
                    }
                    else if (item.Value.ToLower() == "hilsen")
                    {
                        await ShowWelcomeModuleAsync(context);
                    }
                    else if (item.Value.ToLower() == "plassering")
                    {
                        foreach (var entity in witObject.Data.Entities.Gjenstand)
                        {
                            if (entity.Value.ToLower().Equals("cv"))
                                context.Call(new DocumentFinderDialog(), this.ResumeAfterDocumentFinderDialog);
                        }
                    }
                    else if (item.Value.ToLower() == "tidspunkt")
                    {
                        var economyDialog = new EconomyDialog();
                        context.Call(economyDialog, this.ResumeAfterChildDialog);
                    }
                    else
                        context.Wait(MessageReceivedAsync);
                }
            }
        }

        private async Task ResumeAfterChildDialog(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("[RootDialog]");
            context.Wait(this.MessageReceivedAsync);
        }
        private async Task ResumeAfterDocumentFinderDialog(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("[RootDialog]");
            await context.PostAsync("Hva annet kan jeg hjelpe deg med?");
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task ShowWelcomeModuleAsync(IDialogContext context)
        {
            var images = new List<CardImage>();
            var actions = new List<CardAction>();
            var ThumbnailCardGenerator = new ThumbnailCardGenerator();
            var thumbnailImage = ThumbnailCardGenerator
                .CreateImage("https://avatars3.githubusercontent.com/u/6422482?s=400&v=4", "CreunaBot");
            var organizationButton = ThumbnailCardGenerator
                .CreateButton("imBack", "Organisasjon", "Jeg har et spørsmål om Creunas organisasjon.", "ButtonText", "DisplayText");
            var economyButton = ThumbnailCardGenerator
                .CreateButton("imBack", "Økonomi", "Jeg har et spørsmål om lønn, feriepenger, utlegg etc..", "ButtonText", "DisplayText");
            var cvButton = ThumbnailCardGenerator
                .CreateButton("imBack", "Hvor finner jeg CVer", "Jeg har spørsmål om hvor jeg finner en Creuna CV", "ButtonText", "DisplayText");
            var itemButton = ThumbnailCardGenerator
                .CreateButton("imBack", "Printer eller Nøkkelkort", "Jeg har spørsmål angående printer eller nøkkelkort.", "ButtonText", "DisplayText");
            images.Add(thumbnailImage);
            actions.Add(organizationButton);
            actions.Add(economyButton);
            actions.Add(cvButton);
            actions.Add(itemButton);

            var card = ThumbnailCardGenerator
                .CreateThumbnailCard("Hei, mitt navn er CreunaBot.", "Hva kan jeg hjelpe deg med?", "Jeg er programmert til å kunne gi noen svar på følgende temaer:", images, actions);
            var attachment = ThumbnailCardGenerator
                .ComposeAttachment(card, ThumbnailCard.ContentType);
            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);
            await context.PostAsync(reply, cancellationToken: CancellationToken.None);
        }
        private async Task ShowOptionsAsync(IDialogContext context)
        {
            var images = new List<CardImage>();
            var actions = new List<CardAction>();
            var ThumbnailCardGenerator = new ThumbnailCardGenerator();
            var thumbnailImage = ThumbnailCardGenerator
                .CreateImage("https://avatars3.githubusercontent.com/u/6422482?s=400&v=4", "nice picture of a bot");
            var pictureButton = ThumbnailCardGenerator
                .CreateButton("openUrl", "GitBot", "https://avatars3.githubusercontent.com/u/6422482?s=400&v=4", "ButtonText", "DisplayText");
            var wikiButton = ThumbnailCardGenerator
                .CreateButton("openUrl", "Se hvem som er best", "https://www.creuna.com/no/", "ButtonText", "DisplayText");
            var contactHumanButton = ThumbnailCardGenerator
                .CreateButton("imBack", "Få tak i et menneske", "Jeg trenger et menneske som kan hjelpe meg.", "ButtonText", "DisplayText");

            images.Add(thumbnailImage);
            actions.Add(pictureButton);
            actions.Add(wikiButton);
            actions.Add(contactHumanButton);

            var card = ThumbnailCardGenerator
                .CreateThumbnailCard("Hjelp", "Hva sliter du med?", "text", images, actions);
            var attachment = ThumbnailCardGenerator
                .ComposeAttachment(card, ThumbnailCard.ContentType);
            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);
            await context.PostAsync(reply, cancellationToken: CancellationToken.None);
        }
    }
}