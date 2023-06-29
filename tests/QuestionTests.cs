using GHPT.Configs;
using GHPT.IO;
using GHPT.Prompts;
using GHPT.Utils;
using NUnit.Framework;
using System.Collections;
using System.Threading.Tasks;

namespace UnitTests
{

	public sealed class QuestionTests
	{

		[SetUp]
		public static void SetupToken()
		{
			bool configured = ConfigUtil.CheckConfiguration();
			ConfigUtil.LoadConfigs();

			Assert.That(ConfigUtil.CheckConfiguration(), Is.True);
		}

		[TestCaseSource(nameof(Queries))]
		public async Task GetResponseDataTest(string question)
		{
			PromptData data = await PromptUtils.AskQuestion(GetTestConfig(), question);
			Assert.That(data.Connections, Is.Not.Empty);
			Assert.That(data.Additions, Is.Not.Empty);
		}

		private static GPTConfig GetTestConfig()
		{
			return new GPTConfig()
			{
				Model = "gpt-3.5-turbo",
				Name = "test",
				Version = GPTVersion.GPT3_5,
				Token = "invalid"
			};
		}

		public static IEnumerable Queries
		{
			get
			{
				yield return "How can I Create a Cylinder?";
			}
		}

	}
}
