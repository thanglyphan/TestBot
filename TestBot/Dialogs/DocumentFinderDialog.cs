using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
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
            await context.PostAsync("Alle Creunas ansatte har sin Creuna CV lagret på filserveren. Ønsker du at jeg skal finne en spesifikk CV for deg?");
            context.Wait(FindSpecificDocument);
        }

        private async Task FindSpecificDocument(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            if (message.Text.ToLower().Equals("ja"))
            {
                await context.PostAsync("Bruker du PC eller MAC?");
                context.Wait(null);
                await context.PostAsync("Hvem sin CV ønsker du å finne?");
                context.Wait(this.SearchQuery);
            }
            else
                context.Wait(MessageReceived);

        }

        private async Task SearchQuery(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var searchQuery = await result;
            string cvLocation = @"S:\Creuna internt\CV og maler\CV\CVer ORGANISERT PER ANSATT";
            string searchResult = Path.Combine(cvLocation, searchQuery.Text.ToLower());
            if (searchQuery != null)
            {
                await context.PostAsync("Jeg tror CVen du leter etter ligger her:");
                await context.PostAsync(searchResult);
                await ShowFileDialog(context, searchResult);
            }
            else
            {
                await context.PostAsync(searchResult);
                await context.PostAsync("Jeg fant ikke den du leter etter, er du sikker på at du har skrevet navnet riktig? Prøv igjen.");
                context.Wait(SearchQuery);
            }
        }

        private async Task AfterRepeatAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {

            var confirm = await result;
            if (confirm.Text.ToLower().Contains("nei"))
            {
                await context.PostAsync("Supert!");
                context.Done<object>(new Object());
            }
            else
            {
                await context.PostAsync("Hvilken CV skal jeg finne denne gangen?");
                context.Wait(SearchQuery);
            }
        }

        private async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            bool requestHandled = false;
            var message = await result as Activity;

            requestHandled = true;
            context.Done<bool>(requestHandled);
        }
        private async Task ShowFileDialog(IDialogContext context, string argument)
        {
            var path = argument;
            var images = new List<CardImage>();
            var actions = new List<CardAction>();
            var documentButton = CreateButton("openUrl", "Åpne mappe", path, "ButtonText", "DisplayText");
            actions.Add(documentButton);

            var card = CreateThumbnailCard("Er dette riktig?", "", "", images, actions);
            var attachment = ComposeAttachment(card, ThumbnailCard.ContentType);
            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);
            await context.PostAsync(reply, cancellationToken: CancellationToken.None);
            await context.PostAsync("Skal jeg finne en annen CV til deg?");
            context.Wait(AfterRepeatAsync);
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
