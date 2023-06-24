using GHPT.IO;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace GHPT.Utils
{
    public static class ConfigUtil
    {
        private static GPTConfig _config;
        public static string ConfigFileName => "ghpt.json";

        public static string AssemblyLocation => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string ConfigurationFilePath => Path.Combine(AssemblyLocation, ConfigFileName);

        public static string GptToken => _config?.Token;
        public static string GptModel => _config.Model;

        public static bool CheckConfiguration()
        {
            if (File.Exists(ConfigurationFilePath))
            {
                return true;
            }

            return false;
        }

        public static void LoadConfig()
        {
            using (StreamReader reader = new(ConfigurationFilePath))
            {
                string contents = reader.ReadToEnd();
                _config = JsonConvert.DeserializeObject<GPTConfig>(contents);
            }
        }

        public static void SaveConfig()
        {
            using (StreamWriter writer = new(ConfigurationFilePath))
            {
                string config = JsonConvert.SerializeObject(_config);
                writer.Write(config);
            }
        }

        public static void PromptUserForConfig()
        {
            ConfigPromptWindow window = new();
            window.ShowDialog();

            _config = window.Config;

            SaveConfig();
        }
    }
}
