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

    public class EchoBot<T> : ActivityHandler where T : Dialog
    {       
        protected readonly BotState ConversationState;
        protected readonly BotState UserState;
        protected readonly Dialog Dialog;
        protected readonly ILogger Logger;        

        
        public EchoBot(ConversationState conversationState, UserState userState, T dialog, ILogger<EchoBot<T>> logger)
        {
            ConversationState = conversationState;
            UserState = userState;
            Dialog = dialog;
            Logger = logger;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
          
            await base.OnTurnAsync(turnContext, cancellationToken);

            // GUARDA LOS CAMBIOS DE ESTADO QUE HAN DURANTE LOS TURNOS
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {            
                        
            Logger.LogInformation("Running dialog with Message Activity.");

            // EJECUTA EL DIALOGO CON EL NUEVO MENSAJE DE ACTIVIDAD
            await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }       
    }
}
