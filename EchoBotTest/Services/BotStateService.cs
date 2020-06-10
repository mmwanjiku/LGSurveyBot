﻿using LegalBotTest.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LegalBotTest.Services
{
    public class BotStateService
    {
        #region Variables
        //State Variables
        public ConversationState ConversationState { get; }
        public UserState UserState { get; }

        //IDs
        public static string UserProfileId { get; } = $"{nameof(BotStateService)}.UserProfile";
        //public static string ConversationDataId { get; } = $"{nameof(BotStateService)}.ConversationData";
        public static string DialogStateId { get; } = $"{nameof(BotStateService)}.DialogState";
        public static string SurveyDataId { get; } = $"{nameof(BotStateService)}.SurveyData";

        //Accessors
        public IStatePropertyAccessor<UserProfile> UserProfileAccessor { get; set; }
        //public IStatePropertyAccessor<ConversationData> ConversationDataAccessor { get; set; }
        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }
        public IStatePropertyAccessor<SurveyData> SurveyDataAccessor { get; set; }

        public BotStateService(ConversationState conversationState, UserState userState)
        {
            //ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
            InitializeAccessors();

        }

        public void InitializeAccessors()
        {
            //Initialize Conversation State Accessorrs
            //ConversationDataAccessor = ConversationState.CreateProperty<ConversationData>(ConversationDataId);

            //Initialize User State
            UserProfileAccessor = UserState.CreateProperty<UserProfile>(UserProfileId);

            //Initialize Dialog accessor
            DialogStateAccessor = UserState.CreateProperty<DialogState>(DialogStateId);

            //Initialize Dialog accessor
            SurveyDataAccessor = UserState.CreateProperty<SurveyData>(SurveyDataId);
        }

        #endregion

    }
}
