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
                    Advice = PostprocessErrorMessage(ex.Message)
                };
            }
        }

        public static async Task<PromptData> AskQuestion(string question)
        {
            try
            {
                string prompt = Prompt.GetPrompt(question);
                var payload = await ClientUtil.Ask(prompt);
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
                string msg = ex.Message;



                return new PromptData()
                {
                    Advice = PostprocessErrorMessage(ex.Message)
                };
            }
        }

        public static string PostprocessErrorMessage(string msg)
        {
            string l = msg.ToLower();

            if (l.Contains("429 too many requests"))
            {
                return "Error: Looks like you've exceeded the maximum number of requests per minute "
                    + "for your account. Please, consult the OpenAI API documentation regarding "
                    + "rate limits and API quotas: \n\nhttps://platform.openai.com/docs/guides/rate-limits/overview\n\n Also, refer "
                    + "to the Rate Limits section of your personal page on OpenAI's Developer portal: \n\nhttps://platform.openai.com/account/rate-limits\n\n"
                    + msg;
            }
            else if (l.Contains("the model") && l.Contains("does not exist"))
            {
                return "Error: Looks like your account does not have access to the requested model. Please, take a look "
                    + "at your OpenAI Developer account to view the list of available models and rate limits: \n\nhttps://platform.openai.com/account/rate-limits\n\n"
                    + msg;
            }
            else return msg;
        }

    }

}
