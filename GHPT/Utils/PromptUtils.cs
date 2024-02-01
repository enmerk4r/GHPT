using GHPT.IO;
using GHPT.Prompts;
using System.Text.Json;
using System.Threading.Tasks;

namespace GHPT.Utils
{

	public static class PromptUtils
	{

		private static readonly string[] SPLITTER = { "```json", "```" };

		public static string GetChatGPTJson(string chatGPTResponse)
		{
			string[] jsons = chatGPTResponse.Split( SPLITTER,
										System.StringSplitOptions.RemoveEmptyEntries);

			string latestJson = jsons.Last();

			return latestJson;
		}

		public static PromptData GetPromptDataFromResponse(string chatGPTJson)
		{
			JsonSerializerOptions options = new()
			{
				AllowTrailingCommas = true,
				PropertyNameCaseInsensitive = true,
				IgnoreReadOnlyFields = true,
				IgnoreReadOnlyProperties = true,
				ReadCommentHandling = JsonCommentHandling.Skip,
				WriteIndented = true,
				IncludeFields = true,
				NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
				Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
			};

			if (chatGPTJson.ToLowerInvariant().Contains(Prompt.TOO_COMPLEX))
			{
				return new PromptData()
				{
					Additions = new List<Addition>(),
					Connections = new List<ConnectionPairing>(),
					Advice = Prompt.TOO_COMPLEX
				};
			}

			try
			{
				PromptData result = JsonSerializer.Deserialize<PromptData>(chatGPTJson, options);
				result.ComputeTiers();
				return result;
			}
			catch (Exception ex)
			{
				return new PromptData()
				{
					Additions = new List<Addition>(),
					Connections = new List<ConnectionPairing>(),
					Advice = "Exception: " + ex.Message
				};
			}
		}

		public static async Task<PromptData> AskQuestion(GPTConfig config, string question, double temperature)
		{
			try
			{
				string prompt = Prompt.GetPrompt(question);
				var payload = await ClientUtil.Ask(config, prompt, temperature);
				string payloadJson = payload.Choices.FirstOrDefault().Message.Content;

				if (payloadJson.ToLowerInvariant().Contains(Prompt.TOO_COMPLEX.ToLowerInvariant()))
				{
					return new PromptData()
					{
						Advice = Prompt.TOO_COMPLEX
					};
				}

				var json = GetChatGPTJson(payloadJson);
                var returnValue = GetPromptDataFromResponse(json);

				return returnValue;
			}
			catch (Exception ex)
			{
				return new PromptData()
				{
					Advice = ex.Message
				};
			}
		}

	}

}
