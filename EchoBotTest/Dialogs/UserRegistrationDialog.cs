using LegalBotTest.Models;
using LegalBotTest.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LegalBotTest.Dialogs;
//using LegalBotTest.Models.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Microsoft.Bot.Schema;
using System.Collections;

namespace LegalBotTest.Dialogs
{
    public class UserRegistrationDialog : ComponentDialog
    {
        private readonly BotStateService _botStateService;
        //private readonly IFileUtility _fileUtility;
        //private readonly IConfiguration _configuration;
        private string jsonFile = @"Resources/county.json";
        public string county = "";
        public string sub_county = "";

        public UserRegistrationDialog(string dialogId, BotStateService botStateService) : base(dialogId)
            //IFileUtility fileUtility, IConfiguration configuration
        {

            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(botStateService));
           // _configuration = configuration;
           // _fileUtility = fileUtility;

            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            var waterfallSteps = new WaterfallStep[]
            { 
                //Create Waterfall steps
                LanguageStepAsync,
                DescriptionStepAsync,
                CountyStepAsync,
                SubCountyStepAsync,
                WardStepAsync,
                MainMenuStepAsync,
                SelectedOptionStepAsync,
                SubMenuStepAsync,
                StartSurveyStepAsync
            };

            //Add Named Dialogs
            AddDialog(new WaterfallDialog($"{nameof(UserRegistrationDialog)}.mainFlow", waterfallSteps));
            AddDialog(new ChoicePrompt($"{nameof(UserRegistrationDialog)}.Language"));
            AddDialog(new TextPrompt($"{nameof(UserRegistrationDialog)}.Name"));
            AddDialog(new ChoicePrompt($"{nameof(UserRegistrationDialog)}.county"));
            AddDialog(new ChoicePrompt($"{nameof(UserRegistrationDialog)}.subCounty"));
            AddDialog(new ChoicePrompt($"{nameof(UserRegistrationDialog)}.ward"));
            AddDialog(new ChoicePrompt($"{nameof(UserRegistrationDialog)}.mainMenu"));
            AddDialog(new ChoicePrompt($"{nameof(UserRegistrationDialog)}.selectedOption"));
            AddDialog(new ChoicePrompt($"{nameof(UserRegistrationDialog)}.subMenu"));
            AddDialog(new SurveyDialog($"{nameof(UserRegistrationDialog)}.survey", _botStateService));
            

            //set the starting Dialog
            InitialDialogId = $"{nameof(UserRegistrationDialog)}.mainFlow";

        }

        //waterfall steps

        //Welcome message

        private async Task<DialogTurnResult> LanguageStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.Language",

                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Hujambo, karibu katika huduma yetu. Tafadhali chagua lugha inayokufaa(1.KISWAHILI),(2.KINGEREZA)\n " +
                                                  "Hello, welccome to our service. Please choose your preferred language (1.KISWAHILI),(2.ENGLISH)//"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "KISWAHILI", "ENGLISH" }),
                }, cancellationToken);
        }


        //Get User Name

        private async Task<DialogTurnResult> DescriptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Language"] = ((FoundChoice)stepContext.Result).Value;

            if (stepContext.Values["Language"] == "KISWAHILI")
            {
                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.Name", new PromptOptions
                {
                    Prompt = MessageFactory.Text("Karibu katika huduma yetu, tungependa kukuuliza maswali machache kwa madhumuni ya usajili. Jina lako kamili ni nani?"),
                }, cancellationToken);
            }
            else
            {
                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.Name", new PromptOptions
                {
                    Prompt = MessageFactory.Text("Welcome to our service, we would like to ask you a few questions for the purpose of registration. What is your full name?"),
                }, cancellationToken);
            }

        }
        
        //Get county Location

        private async Task<DialogTurnResult> CountyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //get counties

            //reading data from the json file
            var json = File.ReadAllText(jsonFile);
            try
            {
                var counties = JsonConvert.DeserializeObject<List<County>>(json);

                string options = "";


                for (int i = 0; i < counties.Count; i++)
                {

                    options += $"\n{i + 1}.{counties[i].CountyName}\n";
                }

                if ((string)stepContext.Values["Language"] == "KISWAHILI")
                {

                    var promptOptions = new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Unaishi kaunti gani?) ?\n" + options),
                        RetryPrompt = MessageFactory.Text("Please choose an option from the list."),

                    };
                    return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.county", promptOptions, cancellationToken);
                }

                else
                {
                    var promptOptions = new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Which county do you live in?) ?\n" + options),
                        RetryPrompt = MessageFactory.Text("Please choose an option from the list."),

                    };
                    return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.county", promptOptions, cancellationToken);
                }
            }
            catch (Exception)
            {

                throw;
            }

            //end get counties




            ////Get location information
            //var counties = GetLocations("county");

            ////Filter for administrative location 

            ////We'll need present list to user

            //stepContext.Values["Name"] = (string)stepContext.Result;

            //if (stepContext.Values["Language"] == "KISWAHILI")
            //{
            //    return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.county", new PromptOptions
            //    {
            //        Prompt = MessageFactory.Text("Unaishi kaunti gani? "),
            //        Choices = ChoiceFactory.ToChoices(counties),
            //    }, cancellationToken);
            //}
            //else
            //{
            //    return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.county", new PromptOptions
            //    {
            //        Prompt = MessageFactory.Text("Which county do you live in? "),
            //        Choices = ChoiceFactory.ToChoices(counties),
            //    }, cancellationToken);
            //}
        }

        //get sub-county location

        private async Task<DialogTurnResult> SubCountyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //string result = ((FoundChoice)stepContext.Result).Value;
            //stepContext.Values["county"] = result;

            //var subCounties = GetLocations("constituency", "county", result);

            //if (stepContext.Values["Language"] == "KISWAHILI")
            //{

            //    return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.subCounty", new PromptOptions
            //    {
            //        Prompt = MessageFactory.Text("Je! Uko kaunti gani ndogo ? "),
            //        Choices = ChoiceFactory.ToChoices(subCounties),
            //    }, cancellationToken);
            //}
            //else
            //{
            //    return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.subCounty", new PromptOptions
            //    {
            //        Prompt = MessageFactory.Text("Which sub-county are you located at?"),
            //        Choices = ChoiceFactory.ToChoices(subCounties),
            //    }, cancellationToken);
            //}

            stepContext.Values["county"] = (int)stepContext.Result;
            var json = File.ReadAllText(jsonFile);
            var counties = JsonConvert.DeserializeObject<List<County>>(json);


            int id = (int)stepContext.Values["county"];

            string sub_counties = "";


            for (int i = 0; i < counties.Count; i++)
            {


                if (counties[i].CountyId == id)
                {
                    county += counties[i].CountyName;

                    for (int j = 0; j < counties[i].SubCounties.Count; j++)
                    {
                        sub_counties += $"\n{j + 1}.{counties[i].SubCounties[j].Name}\n";

                    }
                }

            }



            if ((string)stepContext.Values["Language"] == "ENGLISH")
            {

                var promptOptions = new PromptOptions
                {

                    Prompt = MessageFactory.Text("Which sub-county are you located at?\n" + sub_counties),
                    RetryPrompt = MessageFactory.Text("Please choose an option from the list."),


                };
                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.subcounty", promptOptions, cancellationToken);
            }
            else
            {

                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text("Unaishi Katika sub-kaunti gani ?:\n" + sub_counties),
                    RetryPrompt = MessageFactory.Text("Tafadhali chagua kutoka kwenye list"),

                };

                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.subcounty", promptOptions, cancellationToken);

            }
        }

        //get ward location

        private async Task<DialogTurnResult> WardStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //string result = ((FoundChoice)stepContext.Result).Value;
            //stepContext.Values["subCounty"] = result;

            //var wards = GetLocations("ward", "constituency", result);
            //if (stepContext.Values["Language"] == "KISWAHILI")
            //{
            //    return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.ward", new PromptOptions
            //    {
            //        Prompt = MessageFactory.Text("Unaishi katika wadi gani ? "),
            //        Choices = ChoiceFactory.ToChoices(wards),
            //    }, cancellationToken);
            //}
            //else
            //{
            //    return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.ward", new PromptOptions
            //    {
            //        Prompt = MessageFactory.Text("Which ward do you live in?"),
            //        Choices = ChoiceFactory.ToChoices(wards),
            //    }, cancellationToken);
            //}

            // value we get back from the choice prompt
            stepContext.Values["subcounty"] = (int)stepContext.Result;

            int sub_id = (int)stepContext.Values["subcounty"];
            var json = File.ReadAllText(jsonFile);
            var counties = JsonConvert.DeserializeObject<List<County>>(json);
            string[] ward_arr = new string[] { };
            string ward_str = "";

            int county_id = (int)stepContext.Values["county"];

            for (int i = 0; i < counties.Count; i++)
            {


                if (counties[i].CountyId == county_id)
                {

                    for (int j = 0; j < counties[i].SubCounties.Count; j++)
                    {
                        sub_county = counties[i].SubCounties[j].Name;

                        if (counties[i].SubCounties[j].Id == sub_id)
                        {
                            ward_arr = counties[i].SubCounties[j].Wards;
                        }

                    }
                }
            }

            for (int k = 0; k < ward_arr.Length; k++)
            {
                ward_str += $"\n {k + 1}. {ward_arr[k]}\n";
            }





            if ((string)stepContext.Values["Language"] == "ENGLISH")
            {

                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text("Which ward do you live? :\n" + ward_str),
                    RetryPrompt = MessageFactory.Text("Please choose an option from the list."),

                };
                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.ward", promptOptions, cancellationToken);
            }
            else
            {


                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text("Unaishi Katika Ward gani ?\n" + ward_str),
                    RetryPrompt = MessageFactory.Text(" Tafadhali chagua option kutoka list."),

                };

                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.ward", promptOptions
                , cancellationToken);
            }
        }

        

        private async Task<DialogTurnResult> MainMenuStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //var userProfilesFilePath = _configuration["UsersFilePathSource"];
            // Fetch list of all user profile
            //string userProfileJson = await _fileUtility.ReadFromFile(userProfilesFilePath);
            //List<UserProfile> userProfiles = JsonConvert.DeserializeObject<List<UserProfile>>(userProfileJson);

            stepContext.Values["ward"] = ((FoundChoice)stepContext.Result).Value;

            if (stepContext.Values["Language"] == "KISWAHILI")
            {
                //Get the current profile object from user state
                var userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);


                //save all of the data inside the user profile
                userProfile.Name = (string)stepContext.Values["Name"];
                userProfile.County = (string)stepContext.Values["county"];
                userProfile.SubCounty = (string)stepContext.Values["subCounty"];
                userProfile.Ward = (string)stepContext.Values["ward"];
               


                await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format(" Hongera {0}!Umesajiliwa kutumia huduma yetu.Tafadhali chagua(1.MAIN MENU) kuendelea kutumia huduma", userProfile.Name)), cancellationToken);

                await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
                //userProfiles.Add(userProfile);

                // Save user profiles
                //var userProfilesString = JsonConvert.SerializeObject(userProfiles, Formatting.Indented);

                //await _fileUtility.WriteToFile(userProfilesString, userProfilesFilePath);

                ////save data in userstate
                //await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);

                ////Write user details to a json file
                //var userDetails = JsonConvert.SerializeObject(userProfile, Formatting.Indented);
                //var filePath = @".\Resources\userDetails.json";

                //if (!File.Exists(filePath))
                //{
                //    File.WriteAllText(filePath, userDetails);
                //}
                //else
                //{
                //    File.AppendAllText(filePath, userDetails);
                //}

                //display main menu

                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.mainMenu",

                   new PromptOptions
                   {
                       Prompt = MessageFactory.Text("Ukitaka kuendelea chagua MAIN MENU. Ukitaka kuondoka chagua Exit"),
                       Choices = ChoiceFactory.ToChoices(new List<string> { "MAIN MENU ", "EXIT" }),
                   }, cancellationToken);

                //waterfallStep always finishes with the end of the waterfall or with another dialog here it is the end
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
            else
            {
                //Get the current profile object from user state
                var userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);


                //save all of the data inside the user profile
                userProfile.Name = (string)stepContext.Values["Name"];
                userProfile.County = (string)stepContext.Values["county"];
                userProfile.SubCounty = (string)stepContext.Values["subCounty"];
                userProfile.Ward = (string)stepContext.Values["ward"];
                
                await stepContext.Context.SendActivityAsync(MessageFactory.Text(string.Format("Congratulations {0}! You are now registered to use our service. Please choose (1.MAIN MENU ) to continue using the service.", userProfile.Name)), cancellationToken);

                ////save data in userstate
                //await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);

                ////Write user details to a json file
                //var userDetails = JsonConvert.SerializeObject(userProfile, Formatting.Indented);
                //var filePath = @".\Resources\userDetails.json"; ;

                //if (!File.Exists(filePath))
                //{
                //    File.WriteAllText(filePath, userDetails);
                //}
                //else
                //{
                //    File.AppendAllText(filePath, userDetails);
                //}

                // Save data in userstate



                await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
                //userProfiles.Add(userProfile);

                // Save user profiles
                //var userProfilesString = JsonConvert.SerializeObject(userProfiles, Formatting.Indented);

                //await _fileUtility.WriteToFile(userProfilesString, userProfilesFilePath);

                //display main menu

                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.mainMenu",

                   new PromptOptions
                   {
                       Prompt = MessageFactory.Text("Select Main menu if you want to continue. Click on Exit to leave the application"),
                       Choices = ChoiceFactory.ToChoices(new List<string> { "MAIN MENU", "EXIT"}),
                   }, cancellationToken);

            }
        }

        private async Task<DialogTurnResult> SelectedOptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["mainMenu"] = ((FoundChoice)stepContext.Result).Value;


           if (stepContext.Values["mainMenu"] == "MAIN MENU")
            {
                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.selectedOption",
 

                new PromptOptions
                {
                    Prompt = MessageFactory.Text(" "),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "GO BACK", "SUB MENU" }),
                }, cancellationToken) ;
            }
            else
            {
                var exit = "Exited successfully.";

                await stepContext.Context.SendActivityAsync(exit);

                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }



        }

        private async Task<DialogTurnResult> SubMenuStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values["selectedOption"] = ((FoundChoice)stepContext.Result).Value;


            if (stepContext.Values["selectedOption"] == "SUB MENU")
            {
              
                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text("Choose from the available services "),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "INFORMATION", "NEWS", "REFERAL", "SURVEY", "UPDATE PROFILE", "SHARE" }),
                };
                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.subMenu", promptOptions, cancellationToken);
            }
            else
            {
                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text(" Go back to the main menu"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "INFORMATION", "NEWS", "REFERAL", "SURVEY", "UPDATE PROFILE", "SHARE" }),
                };

                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.subMenu",promptOptions, cancellationToken);
            }



        }
       

        //Start Survey Dialog
        private async Task<DialogTurnResult> StartSurveyStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["subMenu"] = ((FoundChoice)stepContext.Result).Value;

            if (stepContext.Values["subMenu"] == "SURVEY")
            {
              

                return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.survey", null, cancellationToken);



            }
            else
            {

                var promptOptions = new PromptOptions
                {
                    Prompt = MessageFactory.Text(" Please select a service")
                };

                return await stepContext.PromptAsync($"{nameof(UserRegistrationDialog)}.survey", promptOptions, cancellationToken);
            }
            
        }

        private string ReadUserDetails()
        {
            var filePath = @"C:\Users\Tech Jargon\source\repos\FeedBackLegalBot\FeedBackLegalBot\Data\UserDetails.json";
            var profile = JsonConvert.DeserializeObject<UserProfile>(File.ReadAllText(filePath));
            return profile.ToString();
        }

        private List<string> GetLocations(string administrativeKey, string filterKey = null, string filterValue = null)
        {
            //Source

            var file = @".\Resources\locations.json";

            //Read values and convert to json

            var locations = JsonConvert.DeserializeObject<JArray>(File.ReadAllText(file));

            var items = new List<string>();

            //Loop through array

            foreach (var obj in locations)
            {
                if (filterKey != null && obj.Value<string>(filterKey) != filterValue)
                {
                    continue;
                }

                items.Add(obj.Value<string>(administrativeKey));
            }

            return items.Distinct().ToList();

        }

    }

}