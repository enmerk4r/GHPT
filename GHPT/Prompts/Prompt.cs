using System;
using System.IO;
using System.Linq;

namespace GHPT.Prompts
{

    public static class Prompt
    {
        private const string QUESTION_FORMAT = "{QUESTION}";

        private const string TOO_COMPLEX_FORMAT = "{TOO_COMPLEX}";

        public const string TOO_COMPLEX = "This question is too complex for me to answer. Please try again.";

        public static string GetPrompt(string question)
        {
            if (string.IsNullOrEmpty(question))
                throw new ArgumentNullException("Please give a non-null or empty question");

            string resourceName = typeof(Prompt).Assembly.GetManifestResourceNames()
                .First(str => str.EndsWith("prompt.txt"));

            string prompt_string = string.Empty;
            using (Stream stream = typeof(Prompt).Assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new(stream))
                {
                    prompt_string = reader.ReadToEnd();
                }
            }

            return prompt_string.Replace(QUESTION_FORMAT, question)
                                .Replace(TOO_COMPLEX_FORMAT, TOO_COMPLEX);
        }

    }
}
