using GHPT.IO;
using GHPT.Prompts;
using System.Text.Json;
using System.Threading.Tasks;

namespace GHPT.Utils
{

	public static class PromptUtils
	{

		private const string SPLITTER = "// JSON: ";

		public static string GetChatGPTJson(string chatGPTResponse)
		{
			string[] jsons = chatGPTResponse.Split(new string[] { SPLITTER },
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

		public static async Task<PromptData> AskQuestion(GPTConfig config, string question)
		{
			try
			{
				string prompt = Prompt.GetPrompt(question);
				var payload = await ClientUtil.Ask(config, prompt);
				string payloadJson = payload.Choices.FirstOrDefault().Message.Content;

				if (payloadJson.ToLowerInvariant().Contains(Prompt.TOO_COMPLEX.ToLowerInvariant()))
				{
					return new PromptData()
					{
						Advice = Prompt.TOO_COMPLEX
					};
				}

				var returnValue = GetPromptDataFromResponse(GetChatGPTJson(payloadJson));

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
