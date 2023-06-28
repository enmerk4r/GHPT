using GHPT.Configs;

namespace GHPT.IO
{
    public readonly struct GPTConfig
    {
        public readonly string Name;

        public readonly string Token;

        public readonly string Model;

        public readonly GPTVersion Version;

        public GPTConfig(string name, GPTVersion version, string token, string model)
        {
            this.Name = name;
            this.Version = version;
            this.Token = token;
            this.Model = model;
        }
    }
}
