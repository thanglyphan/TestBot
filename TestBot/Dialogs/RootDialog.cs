using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using TestBot.ObjectsFromWit;
using System.Collections.Generic;
using System.Threading;

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
            if (activity.Text.ToLower().Equals("hjelp"))
            {
                await ShowOptionsAsync(context);
            }
            var api = new Networking();
            api.ConnectToWit(activity.Text);
            var witObjectStructure = new WitObjectStructure(api.response);
            var messageIntent = witObjectStructure.data.entities.intent;
            if (messageIntent == null)
            {
                await context.PostAsync("Jeg forstår ikke hva du vil. Kan du omformulere spørsmålet?");
            }
            else
            {
                foreach (var item in messageIntent)
                {
                    if (item.value.ToLower() == "hilsen")
                    {
                        await ShowWelcomeModuleAsync(context);
                    }
                    else if (item.value.ToLower() == "plassering")
                    {
                        foreach (var entity in witObjectStructure.data.entities.gjenstand)
                        {
                            if (entity.value.ToLower().Equals("cv"))
                                await context.Forward(new DocumentFinderDialog(), this.ResumeAfterDocumentFinderDialog, messageIntent, CancellationToken.None);
                        }
                    }
                    else if (item.value.ToLower() == "tidspunkt")
                    {
                        foreach (var entity in witObjectStructure.data.entities.okonomi)
                        {
                            if (entity.value.ToLower() == "lønn")
                            {
                                context.Call(new EconomyDialog(), this.ResumeAfterChildDialog);
                            }
                        }
                    }
                    else
                        context.Wait(MessageReceivedAsync);
                }
            }
        }

        private async Task ResumeAfterChildDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(this.MessageReceivedAsync);
        }
        private async Task ResumeAfterDocumentFinderDialog(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result as Activity;
            await context.PostAsync(message.Text);
            context.Wait(this.MessageReceivedAsync);
        }
        private async Task ShowWelcomeModuleAsync(IDialogContext context)
        {
            var images = new List<CardImage>();
            var actions = new List<CardAction>();
            var thumbnailImage = CreateImage("https://avatars3.githubusercontent.com/u/6422482?s=400&v=4", "CreunaBot");
            var organizationButton = CreateButton("imBack", "Organisasjon", "Jeg har et spørsmål om Creunas organisasjon.", "ButtonText", "DisplayText");
            var economyButton = CreateButton("imBack", "Økonomi", "Jeg har et spørsmål om lønn, feriepenger, utlegg etc..", "ButtonText", "DisplayText");
            var cvButton = CreateButton("imBack", "Hvor finner jeg CVer", "Jeg har spørsmål om hvor jeg finner en Creuna CV", "ButtonText", "DisplayText");
            var itemButton = CreateButton("imBack", "Printer eller Nøkkelkort", "Jeg har spørsmål angående printer eller nøkkelkort.", "ButtonText", "DisplayText");
            images.Add(thumbnailImage);
            actions.Add(organizationButton);
            actions.Add(economyButton);
            actions.Add(cvButton);
            actions.Add(itemButton);

            var card = CreateThumbnailCard("Hei, mitt navn er CreunaBot.", "Hva kan jeg hjelpe deg med?", "Jeg er programmert til å kunne gi noen svar på følgende temaer:", images, actions);
            var attachment = ComposeAttachment(card, ThumbnailCard.ContentType);
            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);
            await context.PostAsync(reply, cancellationToken: CancellationToken.None);
            context.Wait(MessageReceivedAsync);
        }
        private async Task ShowOptionsAsync(IDialogContext context)
        {
            var images = new List<CardImage>();
            var actions = new List<CardAction>();
            var thumbnailImage = CreateImage("https://avatars3.githubusercontent.com/u/6422482?s=400&v=4", "nice picture of a bot");
            var pictureButton = CreateButton("openUrl", "GitBot", "https://avatars3.githubusercontent.com/u/6422482?s=400&v=4", "ButtonText", "DisplayText");
            var wikiButton = CreateButton("openUrl", "Se hvem som er best", "https://www.creuna.com/no/", "ButtonText", "DisplayText");
            var contactHumanButton = CreateButton("imBack", "Få tak i et menneske", "Jeg trenger et menneske som kan hjelpe meg.", "ButtonText", "DisplayText");
            images.Add(thumbnailImage);
            actions.Add(pictureButton);
            actions.Add(wikiButton);
            actions.Add(contactHumanButton);

            var card = CreateThumbnailCard("Hjelp", "Hva sliter du med?", "text", images, actions);
            var attachment = ComposeAttachment(card, ThumbnailCard.ContentType);
            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);
            await context.PostAsync(reply, cancellationToken: CancellationToken.None);
            context.Wait(MessageReceivedAsync);
        }

        public CardImage CreateImage(string url, string alt)
            => new CardImage()
            {
                Url = url,
                Alt = alt
            };

        public CardAction CreateButton(string type, string title, object value, string text, string displayText)
            => new CardAction()
            {
                Type = type,
                Title = title,
                Value = value,
                Text = text,
                DisplayText = displayText
            };

        public ThumbnailCard CreateThumbnailCard(string title, string subtitle, string text, List<CardImage> images, List<CardAction> actions)
        => new ThumbnailCard()
        {
            Title = title,
            Subtitle = subtitle,
            Text = text,
            Images = images,
            Buttons = actions
        };

        public Attachment ComposeAttachment(ThumbnailCard card, string contentType)
            => new Attachment()
            {
                ContentType = contentType,
                Content = card
            };
    }
}