using System;
using JetBrains.Annotations;

namespace Contal.IwQuick
{
    public class CreateOnDemandGetter<T> : AGetter<T>
        where T:class 
    {
        private readonly Func<T> _instantiationLambda;

        public CreateOnDemandGetter([NotNull] Func<T> instantiationLambda) : base(null)
        {
            Validator.CheckForNull(instantiationLambda,"instantiationLambda");

            _instantiationLambda = instantiationLambda;
        }

        private readonly object _syncRoot = new object();

        protected override void GetOrModifyValue(ref T valueToBePassedAndOrModified)
        {
            if (ReferenceEquals(valueToBePassedAndOrModified,null))
                lock(_syncRoot)
                    if (ReferenceEquals(valueToBePassedAndOrModified, null))
                        valueToBePassedAndOrModified = _instantiationLambda();

        }

        protected override void DisposeValue(ref T valueToBeDisposed, bool isExplicitDispose)
        {
            var castAdisposable = valueToBeDisposed as ADisposable;
            if (!ReferenceEquals(castAdisposable, null))
            {
                castAdisposable.Dispose(isExplicitDispose);
                return;
            }
            
            var castIdisposable = valueToBeDisposed as IDisposable;
            if (!ReferenceEquals(castIdisposable,null)) 
                castIdisposable.Dispose();
                
        }
    }

    
}