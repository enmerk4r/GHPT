using GHPT.Prompts;
using System;
using System.Collections.Generic;
using System.Linq;
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
                IncludeFields = true
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
                return result;
            }
            catch (Exception ex)
            {
                return new PromptData()
                {
                    Additions = new List<Addition>(),
                    Connections = new List<ConnectionPairing>(),
                    Advice = ex.Message
                };
            }
        }

        public static async Task<PromptData> AskQuestion(string question)
        {
            var payload = await ClientUtil.Ask(Prompt.GetPrompt(question));
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

    }

}
