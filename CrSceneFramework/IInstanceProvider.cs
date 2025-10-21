#if COMPACT_FRAMEWORK
namespace CrSceneFrameworkCF
#else
namespace CrSceneFramework
#endif
{
    public interface IInstanceProvider<TInstance>
        where TInstance : class
    {
        TInstance Instance
        {
            get;
        }
    }

    public interface IInstanceProvider<TInstance, TParam>
        where TInstance : class
    {
        TInstance GetInstance(TParam menuItemsProvider);
    }

    public interface IInstanceProvider<TInstance, TParam1, TParam2>
        where TInstance : class
    {
        TInstance GetInstance(TParam1 param1, TParam2 param2);
    }

    public class CastInstanceProvider<TSourceType, TTargetType> : IInstanceProvider<TTargetType>
        where TTargetType : class
        where TSourceType : class, TTargetType
    {
        private readonly IInstanceProvider<TSourceType> _sourceInstanceProvider;

        public CastInstanceProvider(IInstanceProvider<TSourceType> sourceInstanceProvider)
        {
            _sourceInstanceProvider = sourceInstanceProvider;
        }

        public TTargetType Instance
        {
            get { return _sourceInstanceProvider.Instance; }
        }
    }
}