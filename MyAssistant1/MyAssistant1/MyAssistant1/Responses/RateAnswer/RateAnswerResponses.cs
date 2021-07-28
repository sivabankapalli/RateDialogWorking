using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.TemplateManager;
using Microsoft.Bot.Schema;
using MyAssistant1.Responses.RateAnswer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBot.VirtualAssistant.Responses.RateAnswer
{
    public class RateAnswerResponses : TemplateManager
    {
        private LanguageTemplateDictionary _responseTemplates = new LanguageTemplateDictionary()
        {
            ["default"] = new TemplateIdMap
            {
                {
                    ResponseIds.SaySorry,
                    (context, data) =>
                    MessageFactory.Text(
                        text: RateAnswerStrings.SORRY,
                        ssml: RateAnswerStrings.SORRY,
                        inputHint: InputHints.AcceptingInput)
                },
                {
                    ResponseIds.SayThanks,
                    (context, data) =>
                    MessageFactory.Text(
                        text: RateAnswerStrings.THANK,
                        ssml: RateAnswerStrings.THANK,
                        inputHint: InputHints.AcceptingInput)
                },
                {
                    ResponseIds.AskRating,
                    (context, data) =>
                    MessageFactory.Text(
                        text: RateAnswerStrings.ASK_RATING,
                        ssml: RateAnswerStrings.ASK_RATING,
                        inputHint: InputHints.ExpectingInput)
                }
            }
        };

        public RateAnswerResponses()
        {
            Register(new DictionaryRenderer(_responseTemplates));
        }

        public class ResponseIds
        {
            public const string SaySorry = "saySorry";
            public const string SayThanks = "sayThanks";
            public const string AskRating = "askRating";
        }
    }
}
