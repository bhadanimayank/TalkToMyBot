using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Extensions.Options;
using ChatBot.Models;
using ChatBot.Services;
using Microsoft.Bot.Builder.Ai.LUIS;

namespace ChatBot
{
    public class EchoBot : IBot
    {
        /// <summary>
        /// Every Conversation turn for our EchoBot will call this method. In here
        /// the bot checks the Activty type to verify it's a message, bumps the 
        /// turn conversation 'Turn' count, and then echoes the users typing
        /// back to them. 
        /// </summary>
        /// <param name="context">Turn scoped context containing all the data needed
        /// for processing this conversation turn. </param>        
        public async Task OnTurn(ITurnContext context)
        {
            // This bot is only handling Messages
            if (context.Activity.Type == ActivityTypes.Message)
            {
                // Get the conversation state from the turn context
                // var state = context.GetConversationState<EchoState>();

                // Bump the turn count. 
                // state.TurnCount++;

                // Echo back to the user whatever they typed.
                // await context.SendActivity($"Turn {state.TurnCount}: You sent '{context.Activity.Text}'");

                if (!context.Responded)
                {
                    var result = context.Services.Get<RecognizerResult>(LuisRecognizerMiddleware.LuisRecognizerResultKey);
                    var topIntent = result?.GetTopScoringIntent();

                    switch (topIntent != null ? topIntent.Value.intent : null)
                    {
                        case "TodaysSpecialty":
                            await context.SendActivity($"For today we have the following options: {string.Join(", ", BotConstants.Specialties)}");
                            break;
                        default:
                            await context.SendActivity("Sorry, I didn't understand that.");
                            break;
                    }
                }
            }
            else if (context.Activity.Type == ActivityTypes.ConversationUpdate && context.Activity.MembersAdded.FirstOrDefault()?.Id == context.Activity.Recipient.Id)
            {
                var msg = "Hi! I'm a restaurant assistant bot. I can help you with your reservation.";

                await context.SendActivity(msg);
            }
        }
    }    
}
