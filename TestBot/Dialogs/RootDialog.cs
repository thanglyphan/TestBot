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
        private string uncertainReply = "Nå forstod jeg ikke hva du mente. Kan du omformulere spørsmålet?";
        private string additionalHelpReply = "Hva annet kan jeg hjelpe deg med?";
        private string questionOrganization = "Jeg har et spørsmål om hvordan Creuna er organisert.";
        private string questionEconomy = "Jeg har et spørsmål om min lønn, feriepenger, utlegg etc.";
        private string questionDocumentCV = "Jeg lurer på hvor jeg finner Creuna CV’er.";
        private string questionPrinterAndKeycard = "Jeg lurer på hvor jeg finner printer eller når jeg må bruke nøkkelkortet mitt.";
        private string buttonTextOrganization = "Organisasjon";
        private string thumbnailCardImageUrl = "https://avatars3.githubusercontent.com/u/6422482?s=400&v=4";
        private string thumbnailCardImageAltText = "Nice picture of a bot. Stolen.";

        private string welcomeButtonTextEconomy = "Ansatte økonomi";
        private string welcomeButtonTextDocumentCV = "CV plassering";
        private string welcomeButtonTextPrinterAndKeycard = "Printerlokasjon og nøkkelkortbruk";
        private string welcomeCardTitle = "Hei, mitt navn er CreunaBot.";
        private string welcomeCardSubtitle = "Hva kan jeg hjelpe deg med?";
        private string welcomeCardText = "Jeg er programmert til å kunne gi noen svar på følgende temaer:";

        private string helpCardTitle = "Hjelp";
        private string helpCardSubtitle = "Hva sliter du med?";
        private string helpCardText = "";
        private string helpButtonTextCreuna = "Se hvem som er best";
        private string helpButtonTextHumanHelp = "Få tak i et menneske";
        private string helpButtonCreunaUrl = "https://www.creuna.com/no/";
        private string helpButtonHumanOutput = "Jeg trenger et menneske som kan hjelpe meg.";
        




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
                await context.PostAsync(uncertainReply);
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
                        foreach (var entity in witObject.Data.Entities.Okonomi)
                        {
                            var economyDialog = new EconomyDialog(item.Value.ToLower(), entity.Value.ToLower());
                            context.Call(economyDialog, this.ResumeAfterChildDialog);
                        }
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
            await context.PostAsync(additionalHelpReply);
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task ShowWelcomeModuleAsync(IDialogContext context)
        { 
            var images = new List<CardImage>();
            var actions = new List<CardAction>();
            var ThumbnailCardGenerator = new ThumbnailCardGenerator();
            var thumbnailImage = ThumbnailCardGenerator
                .CreateImage(thumbnailCardImageUrl, "CreunaBot");
            var organizationButton = ThumbnailCardGenerator
                .CreateButton("imBack", buttonTextOrganization, questionOrganization, "ButtonText", "DisplayText");
            var economyButton = ThumbnailCardGenerator
                .CreateButton("imBack", welcomeButtonTextEconomy, questionEconomy, "ButtonText", "DisplayText");
            var cvButton = ThumbnailCardGenerator
                .CreateButton("imBack", welcomeButtonTextDocumentCV, questionDocumentCV, "ButtonText", "DisplayText");
            var itemButton = ThumbnailCardGenerator
                .CreateButton("imBack", welcomeButtonTextPrinterAndKeycard, questionPrinterAndKeycard, "ButtonText", "DisplayText");
            images.Add(thumbnailImage);
            actions.Add(organizationButton);
            actions.Add(economyButton);
            actions.Add(cvButton);
            actions.Add(itemButton);

            var card = ThumbnailCardGenerator
                .CreateThumbnailCard(welcomeCardTitle,welcomeCardSubtitle,welcomeCardText, images, actions);
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
                .CreateImage(thumbnailCardImageUrl, thumbnailCardImageAltText);
            var pictureButton = ThumbnailCardGenerator
                .CreateButton("openUrl", "GitBot", thumbnailCardImageUrl, "ButtonText", "DisplayText");
            var wikiButton = ThumbnailCardGenerator
                .CreateButton("openUrl", helpButtonTextCreuna, helpButtonCreunaUrl, "ButtonText", "DisplayText");
            var contactHumanButton = ThumbnailCardGenerator
                .CreateButton("imBack", helpButtonTextHumanHelp, helpButtonHumanOutput, "ButtonText", "DisplayText");

            images.Add(thumbnailImage);
            actions.Add(pictureButton);
            actions.Add(wikiButton);
            actions.Add(contactHumanButton);

            var card = ThumbnailCardGenerator
                .CreateThumbnailCard(helpCardTitle,helpCardSubtitle,helpCardText, images, actions);
            var attachment = ThumbnailCardGenerator
                .ComposeAttachment(card, ThumbnailCard.ContentType);
            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);
            await context.PostAsync(reply, cancellationToken: CancellationToken.None);
        }
    }
}