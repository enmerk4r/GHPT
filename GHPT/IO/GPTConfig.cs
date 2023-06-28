using GHPT.Configs;

namespace GHPT.IO
{
    public struct GPTConfig
    {
        public string Name { get; set; }

        public string Token { get; set; }

        public string Model { get; set; }

        public GPTVersion Version { get; set; }

        public GPTConfig(string name, GPTVersion version, string token, string model)
        {
            this.Name = name;
            this.Version = version;
            this.Token = token;
            this.Model = model;
        }

        internal bool IsValid() =>
            !string.IsNullOrEmpty(Name) &&
            !string.IsNullOrEmpty(Token) &&
            !string.IsNullOrEmpty(Model) &&
            Version != GPTVersion.None;

    }
}
