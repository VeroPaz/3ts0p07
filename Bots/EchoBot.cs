// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.3.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Connector;
using System.Text;
using System.Collections.Generic;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Extensions.Logging;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Bot.Connector.Teams.Models;

namespace EchoBotTest.Bots
{

    public class EchoBot : ActivityHandler
    {       
        protected readonly BotState ConversationState;
        protected readonly Dialog Dialog;
        protected readonly ILogger Logger;
        protected readonly BotState UserState;

        private const string WelcomeMessage = @"Holi";
        //private String nombre;
       

        public EchoBot(ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState;
            UserState = userState;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
                      
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var nombre = member.Name;
                    await turnContext.SendActivityAsync($"Hi there - {member.Name}. {WelcomeMessage}", cancellationToken: cancellationToken);                    
                }
            }

        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            var conversationStateAccessors = ConversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationStateAccessors.GetAsync(turnContext, () => new ConversationData());
            var userStateAccessors = ConversationState.CreateProperty<UserProfile>(nameof(UserProfile));
            var userProfile = await userStateAccessors.GetAsync(turnContext, () => new UserProfile());



            if (string.IsNullOrEmpty(userProfile.Name))//verificar que la variable Name se encuentra vacia
            {

                //Primera vez es falso, por lo que se solicita nombre del usuario
                if (userProfile.PromptedUserForName) //Variable boolean iniciada en false --> ConversationData.cs
                {
                    // Establece el nombre, variable nombre, clase conversationData
                    userProfile.Name = turnContext.Activity.Text?.Trim();

                    // Reconoce el nombre, y envía una nueva indicación
                    await turnContext.SendActivityAsync($"Gracias {userProfile.Name}. Por favor ahora ingresa tu E-Mail");

                    // Reinicia "flag"
                    userProfile.PromptedUserForName = false;
                }
                else
                {
                    await turnContext.SendActivityAsync($"Bienvenido a Bot Crear Ticket Service?");

                    // Al ser PromptedUserForName falso, pregunta su nombre
                    await turnContext.SendActivityAsync($"Ingresar nuevo ticket?");

                    // flag boolean true, inicia ciclo de if con el ingreso del nombre del usuario.
                    userProfile.PromptedUserForName = true; //cambiar a consultar por continuar
                }
            }
            else
            {
                // Add message details to the conversation data.
                // Convert saved Timestamp to local DateTimeOffset, then to string for display.
                var messageTimeOffset = (DateTimeOffset)turnContext.Activity.Timestamp;
                var localMessageTime = messageTimeOffset.ToLocalTime();
                conversationData.Timestamp = localMessageTime.ToString();
                conversationData.ChannelId = turnContext.Activity.ChannelId.ToString();
                //userProfile.Mail = turnContext.Activity.From.
                userProfile.Mail = turnContext.Activity.Text?.Trim();
                userProfile.Name = turnContext.Activity.From.Name;

                //var connector = new ConnectorClient(new Uri(turnContext.Activity.ServiceUrl));
                //var teamId = turnContext.Activity.GetChannelData<TeamsChannelData>().Team.Id;
                //var members = await connector.Conversations.GetConversationMembersAsync(teamId);
                //var member1 in members.AsTeamsChannelAccounts();
                //WindowsIdentity nombre = WindowsIdentity.GetCurrent();
                
                // Display state data.
                await turnContext.SendActivityAsync($"Tu Nombre es:{userProfile.Name}");
                await turnContext.SendActivityAsync($"Tu e-mail: {userProfile.Mail}");
                await turnContext.SendActivityAsync($"Tu e-mail: {userProfile.Mail2}");
                await turnContext.SendActivityAsync($"Message received at: {conversationData.Timestamp}");
                await turnContext.SendActivityAsync($"Message received from: {conversationData.ChannelId}");

                //acá agregar algo para pasar a otra clase para hacer un flujo de conversación....
            }

        }
        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {//manejo de la actividad entrante
            await base.OnTurnAsync(turnContext, cancellationToken);

            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken); //Guardar datos
            // await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

    }
}
