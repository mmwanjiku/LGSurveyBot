using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using LegalBotTest.Services;
using System.Text.RegularExpressions;
//using LegalBotTest.Models.Interfaces;
using Microsoft.Extensions.Configuration;


namespace LegalBotTest.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly BotStateService _botStateService;
        //private readonly IConfiguration _configuration;
        //private readonly IFileUtility _fileUtility;


        public MainDialog(BotStateService botStateService)
                          //IConfiguration configuration,
                          //IFileUtility fileUtility)
        {
            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));
            //_configuration = configuration;
            //_fileUtility = fileUtility;

            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            //Create Waterfall Steps
            var waterfallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                FinallStepAsync
            };

            //Add Named Dialogs
            AddDialog(new SurveyDialog($"{nameof(MainDialog)}.survey", _botStateService));
            AddDialog(new UserRegistrationDialog($"{nameof(MainDialog)}.userRegistration", _botStateService));

            AddDialog(new WaterfallDialog(nameof(MainDialog), waterfallSteps));

            //Set the starting Dialog
            InitialDialogId = nameof(MainDialog);
           
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
          
                return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.userRegistration", null, cancellationToken);
         
        }

        private async Task<DialogTurnResult> FinallStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }


    }
}
