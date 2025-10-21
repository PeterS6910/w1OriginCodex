using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Contal.IwQuick
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISingleton
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class SingletonHelper
    {
        private static readonly object[] ConstructorParameterArray = {null};

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOfInstance"></typeparam>
        /// <returns></returns>
        internal static TOfInstance Instantiate<TOfInstance>()
            where TOfInstance:ISingleton
        {
            Type type = typeof (TOfInstance);
            if (ReferenceEquals(type,null))
                throw new ArgumentNullException("type");

            if (!type.IsSealed)
            {
                var e = new SingletonException(SingletonError.MustBeSealed, type);
                DebugHelper.TryBreak(e);
                throw e;
            }

            var constructors =
                type.GetConstructors(
                    BindingFlags.DeclaredOnly
                    | BindingFlags.Instance
                    | BindingFlags.NonPublic
                    | BindingFlags.Public);

            if (constructors == null)
            {
                var e = new SingletonException(SingletonError.NoConstructorFound, type);
                e.TryBreak();
                throw e;
            }

            if (constructors.Length != 1)
            {
                var e = new SingletonException(SingletonError.MustHaveOneConstructor, type);
                e.TryBreak();
                throw e;
            }

            var constructor = constructors[0];

            if (!constructor.IsPrivate)
            {
                var e = new SingletonException(SingletonError.ConstructorMustBePrivate, type);
                e.TryBreak();
                throw e;
            }

            var constructorParams = constructor.GetParameters();
            bool zeroParams = false;

            if (constructorParams.Length == 0)
                zeroParams = true;
            else
            {
                if (constructorParams.Length != 1
                    || !typeof(ISingleton).IsAssignableFrom(constructorParams[0].ParameterType))
                {
                    var e = new SingletonException(SingletonError.ConstructorMustHaveZeroOrOneParams, type);
                    e.TryBreak();
                    throw e;
                }
            }                
            
            if (zeroParams)
                return (TOfInstance)constructor.Invoke(null);

            return (TOfInstance)constructor.Invoke(ConstructorParameterArray);
        }

    }

    /// <summary>
    /// typical usage "public class SomeClassToBeSingleton : ASingleton&lt;SomeClassToBeSingleton&gt;"
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [UsedImplicitly]
    public abstract class ASingleton<T> : ISingleton
        where T : ASingleton<T>
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly T _singleton;

        // ReSharper disable once UnusedParameter.Local
        /// <summary>
        /// 
        /// </summary>
        /// <param name="singletonReserved">reserved to distinguish the constructor by type of the parameter</param>
        protected ASingleton(ISingleton singletonReserved)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public static T Singleton
        {
            get
            {
                if (ReferenceEquals(_singleton,null))
                    throw new SingletonException(SingletonError.RecursiveCall,typeof(T));

                return _singleton;
            }
        }

        static ASingleton()
        {
            _singleton = SingletonHelper.Instantiate<T>();
        }
    }

    /// <summary>
    /// typical usage "public class SomeMarshalByRefClassToBeSingleton : AMbrSingleton&lt;SomeMarshalByRefClassToBeSingleton&gt;"
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [UsedImplicitly]
    public abstract class AMbrSingleton<T> : MarshalByRefObject, ISingleton
        where T : AMbrSingleton<T>
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly T _singleton;

        // ReSharper disable once UnusedParameter.Local
        /// <summary>
        /// 
        /// </summary>
        /// <param name="singletonReserved">reserved to distinguish the constructor by type of the parameter</param>
        protected AMbrSingleton(ISingleton singletonReserved)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public static T Singleton
        {
            get
            {
                if (ReferenceEquals(_singleton,null))
                    throw new SingletonException(SingletonError.RecursiveCall,typeof(T));

                return _singleton;
            }
        }

        static AMbrSingleton()
        {
            _singleton = SingletonHelper
                .Instantiate<T>();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum SingletonError
    {
        MustBeSealed,
        NoConstructorFound,
        MustHaveOneConstructor,
        ConstructorMustBePrivate,
        RecursiveCall,
        ConstructorMustHaveZeroOrOneParams
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class SingletonException : Exception
    {
        private static string GetMessageByError(SingletonError error, Type type)
        {
            switch (error)
            {
                case SingletonError.MustBeSealed:
                    return string.Concat("A singleton type ", type, " must be sealed");
                case SingletonError.NoConstructorFound:
                    return string.Concat("No constructor found in type ", type);
                case SingletonError.MustHaveOneConstructor:
                    return string.Concat("A singleton type ", type, " must have exactly one private constructor declared");
                case SingletonError.ConstructorMustBePrivate:
                    return string.Concat("The constructor of a singleton type ", type, " must be private");
                case SingletonError.ConstructorMustHaveZeroOrOneParams:
                    return string.Concat("The constructor of a singleton type ", type, " must have zero or one parameter implementing ISingleton");
                case SingletonError.RecursiveCall:
                    return string.Concat("The recursive call of the constructor of a singleton type ", type, " detected");
                default:
                    return "Unknown error";
            }
        }

        public SingletonException(SingletonError error, Type type)
            : base(GetMessageByError(error, type))
        {
        }
    }
}