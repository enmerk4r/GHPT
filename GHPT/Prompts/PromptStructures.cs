using System.Collections.Generic;

namespace GHPT.Prompts
{

    public struct Response
    {
        public string Question { get; set; }
        public string Reasoning { get; set; }
        public string Advice { get; set; }
        public string Payload { get; set; }
    }

    public struct PromptData
    {
        public string Advice { get; set; }
        public IEnumerable<Addition> Additions { get; set; }
        public IEnumerable<ConnectionPairing> Connections { get; set; }
    }

    public struct Addition
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string? Value { get; set; }
    }

    public struct ConnectionPairing
    {
        public Connection To { get; set; }
        public Connection From { get; set; }
    }

    public struct Connection
    {
        public int Id { get; set; }
        public string ParameterName { get; set; }
    }

}
