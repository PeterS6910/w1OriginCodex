using JetBrains.Annotations;


#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF
#else
namespace CrSceneFramework
#endif
{
    public abstract class ALazyInstance<TInstance> : IInstanceProvider<TInstance>
        where TInstance : class
    {
        private volatile TInstance _instance;
        private readonly object _instanceLock = new object();

        protected abstract TInstance CreateInstance();

        public virtual TInstance Instance
        {
            get
            {
                if (_instance == null)
                    lock (_instanceLock)
                        if (_instance == null)
                            _instance = CreateInstance();

                return _instance;
            }
        }
    }

    public abstract class ALazyInstance<TInstance, TParam1> : ALazyInstance<TInstance>
        where TInstance : class
        where TParam1 : class
    {
        private readonly IInstanceProvider<TParam1> _param1;

        protected ALazyInstance(
            [NotNull]
            IInstanceProvider<TParam1> param1)
        {
            _param1 = param1;
        }

        protected abstract TInstance CreateInstance(TParam1 param1);

        protected sealed override TInstance CreateInstance()
        {
            return CreateInstance(_param1.Instance);
        }
    }

    public abstract class ALazyInstance<TInstance, TParam1, TParam2> : ALazyInstance<TInstance>
        where TInstance : class
        where TParam1 : class
        where TParam2 : class
    {
        private readonly IInstanceProvider<TParam1> _param1;
        private readonly IInstanceProvider<TParam2> _param2;

        protected ALazyInstance(
            [NotNull]
            IInstanceProvider<TParam1> param1, 
            [NotNull]
            IInstanceProvider<TParam2> param2)
        {
            _param1 = param1;
            _param2 = param2;
        }

        protected abstract TInstance CreateInstance(
            TParam1 param1,
            TParam2 param2);

        protected sealed override TInstance CreateInstance()
        {
            return
                CreateInstance(
                    _param1.Instance,
                    _param2.Instance);
        }
    }

    public abstract class ALazyInstance<TInstance, TParam1, TParam2, TParam3> : ALazyInstance<TInstance>
        where TInstance : class
        where TParam1 : class
        where TParam2 : class
        where TParam3 : class
    {
        private readonly IInstanceProvider<TParam1> _param1;
        private readonly IInstanceProvider<TParam2> _param2;
        private readonly IInstanceProvider<TParam3> _param3;

        protected ALazyInstance(
            [NotNull]
            IInstanceProvider<TParam1> param1,
            [NotNull]
            IInstanceProvider<TParam2> param2,
            [NotNull]
            IInstanceProvider<TParam3> param3)
        {
            _param1 = param1;
            _param2 = param2;
            _param3 = param3;
        }

        protected abstract TInstance CreateInstance(
            TParam1 param1,
            TParam2 param2,
            TParam3 param3);

        protected sealed override TInstance CreateInstance()
        {
            return
                CreateInstance(
                    _param1.Instance,
                    _param2.Instance,
                    _param3.Instance);
        }
    }
}
