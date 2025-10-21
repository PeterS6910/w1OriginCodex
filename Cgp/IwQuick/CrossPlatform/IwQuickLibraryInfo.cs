using System;
using System.Reflection;

namespace Contal.IwQuick
{
// ReSharper disable once PartialTypeWithSinglePart
    [LibraryInfoRootClass]
    public partial class IwQuickLibraryInfo : ALibraryInfo
    {
        public static readonly IwQuickLibraryInfo Singleton = new IwQuickLibraryInfo();

        private IwQuickLibraryInfo()
        {
            
        }

        protected override void Process(Action<ALibraryInfo> lambda)
        {
            
        }

        

        public override Assembly GetExecutingAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }
    }

    
}
