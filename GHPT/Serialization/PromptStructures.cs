using System.Collections.Generic;
using System.Linq;

namespace GHPT.Serialization
{

    public struct PromptData
    {
        public string Advice { get; set; }
        public IEnumerable<Addition> Additions { get; set; }
        public IEnumerable<ConnectionPairing> Connections { get; set; }

        public void ComputeTiers()
        {
            List<Addition> additions = Additions.ToList();
            if (additions == null)
                return;

            for (int i = 0; i < additions.Count(); i++)
            {
                Addition addition = additions[i];
                int tier = FindParentsRecursive(addition);
                addition.Tier = tier;

                additions[i] = addition;
            }
            Additions = additions;
        }

        public int FindParentsRecursive(Addition child, int depth = 0)
        {
            try
            {
                List<ConnectionPairing> pairings = Connections.Where(c => c.To.Id == child.Id).ToList();
                List<int> depths = new();
                foreach (ConnectionPairing pairing in pairings)
                {
                    if (pairing.IsValid())
                    {
                        Addition parent = Additions.FirstOrDefault(a => pairing.From.Id == a.Id);
                        int maxDepth = FindParentsRecursive(parent, depth + 1);
                        depths.Add(maxDepth);
                    }
                }

                if (depths.Count == 0) return depth;
                else if (depths.Count == 1) return depths[0];
                else return depths.Max();
            }
            catch
            {
                return depth;
            }
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
        private int _id;
        public int Id { get { return _id == default ? -1 : _id; } set { _id = value; } }
        public string ParameterName { get; set; }

        public bool IsValid()
        {
            return Id > 0 && !string.IsNullOrEmpty(ParameterName);
        }
    }


}
