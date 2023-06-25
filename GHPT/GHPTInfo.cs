using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace GHPT
{

    public sealed class GHPTInfo : GH_AssemblyInfo
    {

        /// <inheritdoc/>
        public override string Name => "GHPT";

        /// <inheritdoc/>
        public override Bitmap Icon => GHPT.Resources.Icons.dark_logo_24x24;

        /// <inheritdoc/>
        public override string Description => "The last plug-in you’ll ever need!";

        public override Guid Id => new("eb88b742-c2cf-4efb-8005-4e3729e66881");

        /// <inheritdoc/>
        public override string AuthorName => "Callum Sykes, Jo Kam, Quoc Dang, Ryan Erbert, Sergey Pigach";

        /// <inheritdoc/>
        public override string AuthorContact => "https://github.com/enmerk4r/GHPT";

        /// <inheritdoc/>
        public override Bitmap AssemblyIcon => GHPT.Resources.Icons.dark_logo_24x24;

    }
}