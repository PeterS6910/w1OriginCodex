using System;

#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF
#else
namespace CrSceneFramework
#endif
{
    public class DelayedInitReference<TInstance> : IInstanceProvider<TInstance>
        where TInstance : class
    {
        private TInstance _instance;
        private bool _isSet;

        public DelayedInitReference()
        {
        }

        public DelayedInitReference(TInstance instance)
        {
            _instance = instance;
            _isSet = true;
        }

        public TInstance Instance
        {
            get
            {
                if (!_isSet)
                    throw new Exception();

                return _instance;
            }

            set
            {
                if (_isSet)
                    throw new Exception();

                _isSet = true;
                _instance = value;
            }
        }
    }
}
