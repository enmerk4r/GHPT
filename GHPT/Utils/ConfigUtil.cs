using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using GHPT.IO;
using Newtonsoft.Json;

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
            using (StreamReader reader = new StreamReader(ConfigurationFilePath))
            {
                string contents = reader.ReadToEnd();
                _config = JsonConvert.DeserializeObject<GPTConfig>(contents);
            }
        }

        public static void SaveConfig()
        {
            using (StreamWriter writer = new StreamWriter(ConfigurationFilePath))
            {
                string config = JsonConvert.SerializeObject(_config);
                writer.Write(config);
            }
        }

        public static void PromptUserForConfig()
        {
            ConfigPromptWindow window = new ConfigPromptWindow();
            window.ShowDialog();

            _config = window.Config;

            SaveConfig();
        }
    }
}
