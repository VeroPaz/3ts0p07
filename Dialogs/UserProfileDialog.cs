using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace EchoBotTest.Dialogs
{
    public class UserProfileDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;

        public UserProfileDialog(UserState userState)
            : base(nameof(UserProfileDialog))
        {
            _userProfileAccessor = userState.CreateProperty<UserProfile>("UserProfile");
                        
            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {
                TransportStepAsync,
                NameStepAsync,
                NameConfirmStepAsync,
                //AgeStepAsync,
                //ConfirmStepAsync,
                SummaryStepAsync,
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>))); /*AgePromptValidatorAsync));*/
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> TransportStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {            
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Bienvenido a Bot Crear Ticket Service"), cancellationToken);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Por favor ingresa tu nombre.") }, cancellationToken);                   

        }

        private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["name"] = (string)stepContext.Result;

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("Por favor ingresa tu email") }, cancellationToken);
        }

        private async Task<DialogTurnResult> NameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["mail"] = (string)stepContext.Result;

            //PARA ENVIAR UN MENSAJE AL USUARIO........
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Gracias "), cancellationToken);
                                    
            var msg = $"Tu mail es: {stepContext.Result} está correcto?";
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(msg) }, cancellationToken);
        }


        private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                // Get the current profile object from user state.
                var userProfile = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

                userProfile.Mail = (string)stepContext.Values["mail"];                
                
                var msg = $"Tu nombre es: {userProfile.Name} y tu mail es: {userProfile.Mail}.";
               

                await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Los datos no serán guardados"), cancellationToken);
            }

            // ESTA PARTE INDICA EL FINAL DEL DIALOGO (ÚLTIMO INDICADO EN LA WATERFALL
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
                       
    }
}
