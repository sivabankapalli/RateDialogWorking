using ChatBot.VirtualAssistant.Responses.RateAnswer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using MyAssistant1.Models;
using MyAssistant1.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyAssistant1.Dialogs
{
	public class RateAnswerDialog : ExceptionHandlingComponentDialog
	{
		private RateAnswerResponses _responder = new RateAnswerResponses();

		private IStatePropertyAccessor<RateAnswerState> _accessor;
		private RateAnswerState _state;

		private readonly IHostingEnvironment _environment;
		private readonly IBotTelemetryClient _telemetryClient;

		public RateAnswerDialog(BotServices botServices, IHostingEnvironment environment, IBotTelemetryClient telemetryClient, UserState userState)
			: base(nameof(RateAnswerDialog), telemetryClient)
		{
			_environment = environment;
			_telemetryClient = telemetryClient;

			_accessor = userState.CreateProperty<RateAnswerState>(nameof(RateAnswerState));

			InitialDialogId = nameof(RateAnswerDialog);

			var askToRate = new WaterfallStep[]
			{
				AskRating,
				FinishDialog
			};

			AddDialog(new WaterfallDialog(InitialDialogId, askToRate));
			AddDialog(new ConfirmPrompt(RateAnswerDialogIds.AskRatingPrompt));
		}

		private async Task<DialogTurnResult> AskRating(WaterfallStepContext sc, CancellationToken cancellationToken)
		{
			// Get passed in options, need to serialise the object before we deserialise because calling .ToString on the object is unreliable
			AnswerRatingDto answerRatingDto = JsonConvert.DeserializeObject<AnswerRatingDto>(JsonConvert.SerializeObject(sc.Options));

			// Read out data from the state
			_state = await GetRateAnswerState(sc.Context);

			// Set state value
			_state.Question = answerRatingDto.Question;

			// Save state
			await SaveRateAnswerState(sc.Context, cancellationToken);

			return await sc.PromptAsync(RateAnswerDialogIds.AskRatingPrompt, new PromptOptions()
			{
				Prompt = await _responder.RenderTemplate(sc.Context, sc.Context.Activity.Locale, RateAnswerResponses.ResponseIds.AskRating),
			});
		}

		private async Task<DialogTurnResult> FinishDialog(WaterfallStepContext sc, CancellationToken cancellationToken)
		{
			return await sc.EndDialogAsync((bool)sc.Result);
		}

		protected override async Task<DialogTurnResult> EndComponentAsync(DialogContext outerDc, object result, CancellationToken cancellationToken)
		{
			var isInfoUseful = (bool)result;

			_state = await GetRateAnswerState(outerDc.Context);

			if (isInfoUseful)
			{
				// If user said yes
				await _responder.ReplyWith(outerDc.Context, RateAnswerResponses.ResponseIds.SayThanks);
				LogBotTelemetryEvent("PositiveFeedback", new Dictionary<string, string> { { $"Bot_PositiveFeedbackReceived", $"Question was: {_state.Question}" } });
			}
			else
			{
				// else if user said no
				await _responder.ReplyWith(outerDc.Context, RateAnswerResponses.ResponseIds.SaySorry);
				LogBotTelemetryEvent("NegativeFeedback", new Dictionary<string, string> { { $"Bot_NegativeFeedbackReceived", $"Question was: {_state.Question}" } });
			}

			return await outerDc.EndDialogAsync();
		}

		public class RateAnswerDialogIds
		{
			public const string AskRatingPrompt = "askRatingPrompt";
		}

		private void LogBotTelemetryEvent(string eventName, Dictionary<string, string> eventProperties)
		{
			if (_environment.IsProduction())
			{
				_telemetryClient.TrackEvent(eventName, eventProperties);
			}
		}

		private async Task<RateAnswerState> GetRateAnswerState(ITurnContext turnContext)
		{
			return await _accessor.GetAsync(turnContext, () => new RateAnswerState());
		}

		private async Task SaveRateAnswerState(ITurnContext turnContext, CancellationToken cancellationToken)
		{
			await _accessor.SetAsync(turnContext, _state, cancellationToken);
		}
	}
}
