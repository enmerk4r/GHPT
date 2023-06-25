using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;

namespace GHPT.Prompts
{

    public struct PromptData
    {
        public string Advice { get; set; }
        public IEnumerable<Addition> Additions { get; set; }
        public IEnumerable<ConnectionPairing> Connections { get; set; }

        public void ComputeTiers()
        {
            List<Addition> additions = this.Additions.ToList();
            for (int i=0; i < additions.Count(); i++)
            {
                Addition addition = additions[i];
                int tier = FindParentsRecursive(addition, new List<Addition>());
                addition.Tier = tier;

                additions[i] = addition;
            }
            this.Additions = additions;
        }

        public int FindParentsRecursive(Addition child, List<Addition> ancestors)
        {
            ConnectionPairing pairing = Connections.FirstOrDefault(c => c.To.Id == child.Id);
            if (pairing.IsValid())
            {
                Addition parent = Additions.FirstOrDefault(a => pairing.From.Id == a.Id);
                ancestors.Add(parent);
                FindParentsRecursive(parent, ancestors);
            }
            return ancestors.Count();
        }
    }

    public struct Addition
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string? Value { get; set; }
        public int Tier { get; set; }
    }

    public struct ConnectionPairing
    {
        public Connection To { get; set; }
        public Connection From { get; set; }

        public bool IsValid()
        {
            return To.IsValid() && From.IsValid();
        }
    }

    public struct Connection
    {
        private int? _id;
        public int Id { get { return _id ?? -1; } set { _id = value; } }
        public string ParameterName { get; set; }

        public bool IsValid()
        {
            return Id > 0 && !string.IsNullOrEmpty(ParameterName);
        }
    }


}
