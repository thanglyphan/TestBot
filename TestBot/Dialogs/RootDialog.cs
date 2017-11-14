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
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var activity = await result as Activity;
            if (message.Text.ToLower().Contains("hjelp"))
            {
                await ShowOptionsAsync(context);
            }
            Networking api = new Networking();
            api.ConnectToWit(activity.Text);
            WitObjectStructure witObjectStructure = new WitObjectStructure(api.response);

            var messageIntent = witObjectStructure.data.entities.intent;
            if (messageIntent == null)
            {
                await context.PostAsync("Jeg forstår ikke hva du vil. Kan du omformulere spørsmålet?");
            }
            else
            {
                foreach (var item in witObjectStructure.data.entities.intent)
                {
                    if (item.value.ToLower() == "hilsen")
                    {
                        await context.PostAsync("Hei, mitt navn er CreunaBot, hva kan jeg hjelpe deg med?");
                    }
                    else if (item.value.ToLower() == "plassering")
                    {
                    }
                    else if (item.value.ToLower() == "tidspunkt")
                    {
                        foreach (var entity in witObjectStructure.data.entities.okonomi)
                        {
                            if (entity.value.ToLower() == "lønn")
                            {
                                context.Call<Object>(new EconomyDialog(), this.ResumeAfterChildDialog);
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

        private async Task ShowOptionsAsync(IDialogContext context)
        {
            var images = new List<CardImage>();
            var actions = new List<CardAction>();
            var button = createButton("openUrl", "CVer", @"S:\Creuna internt\CV og maler\CV\CVer ORGANISERT PER ANSATT", "ButtonText", "DisplayText");
            actions.Add(button);

            var card = CreateThumbnailCard("Hjelp", "Hva sliter du med?","text", images, actions);
            var attachment = ComposeAttachment(card, ThumbnailCard.ContentType);

            var reply = context.MakeMessage();
            reply.Attachments.Add(attachment);
            await context.PostAsync(reply, cancellationToken: CancellationToken.None);
            context.Wait(MessageReceivedAsync);
        }

        public CardAction createButton(string type, string title, object value, string text, string displayText)
        {

            return new CardAction()
            {
                Type = type,
                Title = title,
                Value = value,
                Text = text,
                DisplayText = displayText
            };
        }

        public ThumbnailCard CreateThumbnailCard(string title, string subtitle, string text, List<CardImage> images, List<CardAction> actions)
        {
            return new ThumbnailCard()
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = images,
                Buttons = actions
            };
        }

        public ThumbnailCard MakeThumbnailCard()
        {
            ThumbnailCard card = new ThumbnailCard()
            {
                Title = "Thumbnail card.",
                Subtitle = "Wiki",
                Images = new List<CardImage>(),
                Buttons = new List<CardAction>()
            };
            return card;
        }

        public Attachment ComposeAttachment(ThumbnailCard card, string contentType)
        {
            Attachment attachment = new Attachment()
            {
                ContentType = contentType,
                Content = card
            };
            return attachment;
        }
    }
}