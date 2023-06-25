public class GHPTCategoryIcon : Grasshopper.Kernel.GH_AssemblyPriority
{
    public override Grasshopper.Kernel.GH_LoadingInstruction PriorityLoad()
    {
        Grasshopper.Instances.ComponentServer.AddCategoryIcon("GHPT", GHPT.Resources.Icons.dark_logo_24x24);
        Grasshopper.Instances.ComponentServer.AddCategorySymbolName("GHPT", 'G');
        return Grasshopper.Kernel.GH_LoadingInstruction.Proceed;
    }
}
