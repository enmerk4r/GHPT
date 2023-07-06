using GHPT.Configs;
using GHPT.IO;
using Rhino;

namespace GHPT.Utils
{
	public static class ConfigUtil
	{
		private static List<GPTConfig> ConfigList = new();
		public static IReadOnlyList<GPTConfig> Configs => ConfigList;

		public static bool CheckConfiguration()
		{
			return PersistentSettings.RhinoAppSettings.TryGetChild(nameof(GPTConfig), out var allSettings) &&
				allSettings.ChildKeys.Count > 0;
		}

		public static void LoadConfigs()
		{
			PersistentSettings allSettings = null;
			if (!PersistentSettings.RhinoAppSettings.TryGetChild(nameof(GPTConfig), out allSettings))
			{
				allSettings = PersistentSettings.RhinoAppSettings.AddChild(nameof(GPTConfig));
				allSettings.HiddenFromUserInterface = true;
			}

			ConfigList = new(allSettings.ChildKeys.Count);

			var childKeys = allSettings.ChildKeys;
			foreach (string childKey in allSettings.ChildKeys)
			{
				if (!allSettings.TryGetChild(childKey, out PersistentSettings childSettings))
					continue;

				GPTConfig config = GetConfigFromSettings(childKey, childSettings);
				if (!ConfigList.Contains(config))
				{
					ConfigList.Add(config);
					ConfigAdded?.Invoke(null, new ConfigArgs(config));
				}
			}
		}

		private static GPTConfig GetConfigFromSettings(string name, PersistentSettings childSettings)
		{
			childSettings.TryGetEnumValue(nameof(GPTConfig.Version), out GPTVersion version);
			childSettings.TryGetString(nameof(GPTConfig.Token), out string token);
			childSettings.TryGetString(nameof(GPTConfig.Model), out string model);

			return new(name, version, token, model);
		}

		public static void SaveConfig(GPTConfig config)
		{
			if (!ConfigList.Contains(config))
			{
				ConfigList.Add(config);
				ConfigAdded?.Invoke(null, new ConfigArgs(config));
			}

			PersistentSettings allSettings = null;
			if (!PersistentSettings.RhinoAppSettings.TryGetChild(nameof(GPTConfig), out allSettings))
			{
				allSettings = PersistentSettings.RhinoAppSettings.AddChild(nameof(GPTConfig));
				allSettings.HiddenFromUserInterface = true;
			}

			PersistentSettings configSettings = null;
			if (!allSettings.TryGetChild(config.Name, out configSettings))
			{
				configSettings = allSettings.AddChild(config.Name);
				configSettings.HiddenFromUserInterface = true;
			}

			configSettings.SetString(nameof(GPTConfig.Token), config.Token);
			configSettings.SetString(nameof(GPTConfig.Model), config.Model);
			configSettings.SetEnumValue(nameof(GPTConfig.Version), config.Version);
		}

		public static void RemoveConfig(GPTConfig config)
		{
			ConfigList.Remove(config);
			ConfigRemoved?.Invoke(null, new ConfigArgs(config));

			PersistentSettings allSettings = null;
			if (!PersistentSettings.RhinoAppSettings.TryGetChild(nameof(GPTConfig), out allSettings))
			{
				allSettings = PersistentSettings.RhinoAppSettings.AddChild(nameof(GPTConfig));
				allSettings.HiddenFromUserInterface = true;
			}

			allSettings.DeleteChild(config.Name);
		}

		internal static EventHandler<ConfigArgs> ConfigAdded;
		internal static EventHandler<ConfigArgs> ConfigRemoved;

		internal class ConfigArgs : EventArgs
		{
			internal readonly GPTConfig Config;
			internal ConfigArgs(GPTConfig config)
			{
				Config = config;
			}
		}

	}
}
