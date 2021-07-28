using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAssistant1.Services
{
	public static class StateConversionHelper
	{
		public static string ConvertTurnStateToString(TurnContextStateCollection turnStateCollection)
		{
			StringBuilder stringBuilder = new StringBuilder();

			foreach (KeyValuePair<string, object> item in turnStateCollection)
			{
				object value = item.Value;

				if (value.GetType().Name == "CachedBotState")
				{
					dynamic botState = item.Value as dynamic;

					if (item.Key == "UserState")
					{
						stringBuilder.Append($"{item.Key}:{botState.GetType().GetProperty("Hash")?.GetValue(botState, null)}");
					}
					else if (item.Key == "ConversationState")
					{
						stringBuilder.Append($"{item.Key}:{botState.GetType().GetProperty("Hash")?.GetValue(botState, null)}");
					}
				}
				else
				{
					stringBuilder.Append($"{item.Key}:{value}");
				}

				stringBuilder.Append("|");
			}

			return stringBuilder.ToString();
		}

		public static string ConvertDialogStateToString(IDictionary<string, object> keyValuePairs)
		{
			StringBuilder stringBuilder = new StringBuilder();

			foreach (KeyValuePair<string, object> pair in keyValuePairs)
			{
				if (pair.Value is DialogState)
				{
					DialogState dialogState = (DialogState)pair.Value;

					foreach (DialogInstance dialogInstance in dialogState.DialogStack)
					{
						stringBuilder.Append($"dialogId:{dialogInstance.Id},");

						foreach (KeyValuePair<string, object> item in dialogInstance.State)
						{
							string state = dialogInstance.State.ToString();

							if (item.Value is IEnumerable && item.Value.GetType() != typeof(string))
							{
								IEnumerable collection = item.Value as IEnumerable;
								List<string> values = new List<string>();

								foreach (object entry in collection)
								{
									values.Add(entry.ToString());
								}

								stringBuilder.Append($"{item.Key}:{string.Join(",", values)};");
							}
							else
							{
								stringBuilder.Append($"{item.Key}:{item.Value};");
							}
						}

						stringBuilder.Append("|");
					}
				}
			}

			return stringBuilder.ToString();
		}
	}
}
