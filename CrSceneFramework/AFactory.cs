using JetBrains.Annotations;

#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF
#else
namespace CrSceneFramework
#endif
{
    public abstract class AFactory<TInstance> : IInstanceProvider<TInstance>
            where TInstance : class
    {
        protected abstract TInstance CreateInstance();

        public virtual TInstance Instance
        {
            get
            {
                return CreateInstance();
            }
        }
    }

    public abstract class AFactory<TInstance, TParam1> : AFactory<TInstance>
        where TInstance : class
        where TParam1 : class
    {
        private readonly IInstanceProvider<TParam1> _param1Provider;

        protected AFactory(
            [NotNull]
            IInstanceProvider<TParam1> param1Provider)
        {
            _param1Provider = param1Provider;
        }

        protected abstract TInstance CreateInstance(TParam1 param1);

        protected sealed override TInstance CreateInstance()
        {
            return CreateInstance(_param1Provider.Instance);
        }
    }

    public abstract class AFactory<TInstance, TParam1, TParam2> : AFactory<TInstance>
        where TInstance : class
        where TParam1 : class
        where TParam2 : class
    {
        private readonly IInstanceProvider<TParam1> _param1;
        private readonly IInstanceProvider<TParam2> _param2;

        protected AFactory(
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

    public abstract class AFactory<TInstance, TParam1, TParam2, TParam3> : AFactory<TInstance>
        where TInstance : class
        where TParam1 : class
        where TParam2 : class
        where TParam3 : class
    {
        private readonly IInstanceProvider<TParam1> _param1;
        private readonly IInstanceProvider<TParam2> _param2;
        private readonly IInstanceProvider<TParam3> _param3;

        protected AFactory(
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
