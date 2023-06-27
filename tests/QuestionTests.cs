using GHPT.Serialization;
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

            if (!configured)
            {
                ConfigUtil.PromptUserForConfig();
            }
            else
            {
                ConfigUtil.LoadConfig();
            }

            Assert.That(ConfigUtil.CheckConfiguration(), Is.True);
        }

        [TestCaseSource(nameof(Queries))]
        public async Task GetResponseDataTest(string question)
        {
            PromptData data = await PromptUtils.AskQuestion(question);
            Assert.That(data.Connections, Is.Not.Empty);
            Assert.That(data.Additions, Is.Not.Empty);
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
