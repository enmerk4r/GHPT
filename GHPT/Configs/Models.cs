namespace GHPT.Configs
{
    public static class Models
    {
        public static Dictionary<string, GPTVersion> ModelOptions = new()
        {
            { "gpt-4", GPTVersion.GPT4 },
            { "gpt-3.5-turbo", GPTVersion.GPT3_5 },
            { "gpt-3.5-turbo-0301", GPTVersion.GPT3_5 },
            { "gpt-3.5-turbo-0613", GPTVersion.GPT3_5 },
            { "gpt-3.5-turbo-16k", GPTVersion.GPT3_5 },
            { "gpt-3.5-turbo-16k-0613", GPTVersion.GPT3_5 },
            { "mistralai/Mixtral-8x7B-Instruct-v0.1", GPTVersion.GPT3_5 }
        };
    }
}
