﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.AspNetCore.Mvc;
using TestBot.Dialogs;
using System;

namespace TestBot.Controllers
{
    [Route("api/[controller]")]
    [BotAuthentication]
    public class MessagesController : Controller
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
  
            if (activity?.Type == ActivityTypes.Message)
            {

                //if (ActivityTypes.Message.Contains("Repeat after me"))
                //{
                //    await Conversation.SendAsync(activity, () => new EchoDialog());
                //}
                //else
                await Conversation.SendAsync(activity, () => new RootDialog());

                await Conversation.SendAsync(activity, () => new EchoDialog());

            }
            else
            {
                HandleSystemMessage(activity);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {

            }

            return null;
        }
        [Serializable]
        public class EchoDialog : IDialog<object>
        {
            public async Task StartAsync(IDialogContext context)
            {
                context.Wait(MessageReceivedAsync);
            }

            public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
            {
                var message = await argument;
                await context.PostAsync("You said: " + message.Text);
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}