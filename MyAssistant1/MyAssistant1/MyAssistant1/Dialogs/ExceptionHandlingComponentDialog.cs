using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using MyAssistant1.Responses.Main;
using MyAssistant1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyAssistant1.Dialogs
{
	/// <summary>
	/// A special dialog that will catch internal exceptions, log telemetry,
	/// inform the user that the dialog flow will be ended, then end the dialog
	/// </summary>
	public class ExceptionHandlingComponentDialog : ComponentDialog
	{
		private readonly IBotTelemetryClient _botTelemetryClient;

		public ExceptionHandlingComponentDialog(
			string dialogId,
			IBotTelemetryClient botTelemetryClient)
			: base(dialogId)
		{
			_botTelemetryClient = botTelemetryClient;
		}

		public async override Task<DialogTurnResult> ContinueDialogAsync(DialogContext outerDc, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				return await base.ContinueDialogAsync(outerDc, cancellationToken);
			}
			catch (Exception exception)
			{
				return await HandleException(outerDc, exception);
			}
		}

		public async override Task<DialogTurnResult> ResumeDialogAsync(DialogContext outerDc, DialogReason reason, object result = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				return await base.ResumeDialogAsync(outerDc, reason, result, cancellationToken);
			}
			catch (Exception exception)
			{
				return await HandleException(outerDc, exception);
			}
		}

		private async Task<DialogTurnResult> HandleException(DialogContext outerDc, Exception exception)
		{
			Dictionary<string, string> telemetryData = new Dictionary<string, string>
			{
				{ "ActivityText", outerDc.Context.Activity?.Text?.ToString() },
				{ "ActivityValue", outerDc.Context.Activity?.Value?.ToString() },
			};

			if (outerDc.ActiveDialog != null)
			{
				await outerDc.Context.SendActivityAsync(MainStrings.ERROR);
				await outerDc.Context.SendActivityAsync("The current dialog flow will be ended.");

				telemetryData.Add("ActiveDialog", "True");
				telemetryData.Add("ActiveDialogId", $"{outerDc.ActiveDialog.Id}");
				telemetryData.Add("State", $"{StateConversionHelper.ConvertDialogStateToString(outerDc.ActiveDialog.State)}");
			}
			else
			{
				await outerDc.Context.SendActivityAsync($"An issue was encountered.");

				telemetryData.Add("ActiveDialog", "False");
				telemetryData.Add("State", $"{StateConversionHelper.ConvertTurnStateToString(outerDc.Context.TurnState)}");
			}

			_botTelemetryClient.TrackException(exception, telemetryData);

			return await outerDc.EndDialogAsync();
		}
	}
}
