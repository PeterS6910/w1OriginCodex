using System;

namespace Contal.IwQuick
{
    public abstract class AProgram<TProgramChild> : IDisposable
        where TProgramChild:AProgram<TProgramChild>,new()
    {
        protected abstract void Run(string[] args);

        protected static void RunFromMain(string[] args)
        {
            using(var program = new TProgramChild() )
                program.Run(args);
        }

        public void Dispose()
        {
            
        }

        
    }
}
