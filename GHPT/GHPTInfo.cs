using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace GHPT
{
    public class GHPTInfo : GH_AssemblyInfo
    {
        public override string Name => "GHPT";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("eb88b742-c2cf-4efb-8005-4e3729e66881");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}