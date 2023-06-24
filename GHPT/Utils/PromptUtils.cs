using GHPT.Prompts;
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
            PromptData result = JsonSerializer.Deserialize<PromptData>(chatGPTJson);

            return result;
        }

        public static async Task<PromptData> AskQuestion(string question)
        {
            var payload = await ClientUtil.Ask(Prompt.GetPrompt(question));
            string payloadJson = payload.Choices.FirstOrDefault().Message.Content;

            var returnValue = GetPromptDataFromResponse(GetChatGPTJson(payloadJson));

            return returnValue;
        }

    }

}
