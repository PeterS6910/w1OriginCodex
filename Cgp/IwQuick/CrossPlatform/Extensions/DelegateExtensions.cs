using System;
using JetBrains.Annotations;

namespace Contal.IwQuick
{
    public static class DelegateExtensions
    {
        public static string GetTargetDescription([CanBeNull] this Delegate d)
        {
            if (ReferenceEquals(d,null))
                return string.Empty;

            return "{ " + d.Method.Name + " @ " + d.Target + " }";
        }

    }

    public static class ActionFuncExtensions
    {
        /// <summary>
        /// tries to call the delegate's snapshot, if not null
        /// </summary>
        /// <param name="lambda"></param>
        public static void Call([CanBeNull] this Action lambda)
        {
            if (ReferenceEquals(lambda,null))
                return;

            lambda();
        }

        /// <summary>
        /// tries to call the delegate's snapshot, if not null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lambda"></param>
        /// <param name="arg1"></param>
        public static void Call<T>([CanBeNull] this Action<T> lambda,T arg1)
        {
            if (ReferenceEquals(lambda, null))
                return;

            lambda(arg1);
        }

        /// <summary>
        /// tries to call the delegate's snapshot, if not null
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="lambda"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public static void Call<T1, T2>([CanBeNull] this Action<T1, T2> lambda, T1 arg1, T2 arg2)
        {
            if (ReferenceEquals(lambda, null))
                return;

            lambda(arg1,arg2);
        }

        /// <summary>
        /// tries to call the delegate's snapshot, if not null
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <param name="lambda"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public static void Call<T1, T2, T3>([CanBeNull] this Action<T1, T2, T3> lambda, T1 arg1, T2 arg2, T3 arg3)
        {
            if (ReferenceEquals(lambda, null))
                return;

            lambda(arg1, arg2, arg3);
        }

        /// <summary>
        /// tries to call the delegate's snapshot, if not null
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <param name="lambda"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        public static void Call<T1, T2, T3, T4>([CanBeNull] this Action<T1, T2, T3, T4> lambda, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (ReferenceEquals(lambda, null))
                return;

            lambda(arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// tries to call the delegate's snapshot, if not null
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="lambda"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool Call<TResult>([CanBeNull] this Func<TResult> lambda, out TResult result)
        {
            if (ReferenceEquals(lambda, null))
            {
                result = default (TResult);
                return false;
            }

            result = lambda();
            return true;
        }

        /// <summary>
        /// tries to call the delegate's snapshot, if not null
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="lambda"></param>
        /// <param name="arg1"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool Call<T1, TResult>([CanBeNull] this Func<T1, TResult> lambda, T1 arg1, out TResult result)
        {
            if (ReferenceEquals(lambda, null))
            {
                result = default(TResult);
                return false;
            }

            result = lambda(arg1);
            return true;
        }

        /// <summary>
        /// tries to call the delegate's snapshot, if not null
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="lambda"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool Call<T1, T2, TResult>([CanBeNull] this Func<T1, T2, TResult> lambda, T1 arg1, T2 arg2, out TResult result)
        {
            if (ReferenceEquals(lambda, null))
            {
                result = default(TResult);
                return false;
            }

            result = lambda(arg1,arg2);
            return true;
        }

        /// <summary>
        /// tries to call the delegate's snapshot, if not null
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="lambda"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool Call<T1, T2, T3, TResult>([CanBeNull] this Func<T1, T2, T3, TResult> lambda, T1 arg1, T2 arg2, T3 arg3, out TResult result)
        {
            if (ReferenceEquals(lambda, null))
            {
                result = default(TResult);
                return false;
            }

            result = lambda(arg1, arg2, arg3);
            return true;
        }

        /// <summary>
        /// tries to call the delegate's snapshot, if not null
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="T3"></typeparam>
        /// <typeparam name="T4"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="lambda"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool Call<T1, T2, T3, T4, TResult>([CanBeNull] this Func<T1, T2, T3, T4, TResult> lambda, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out TResult result)
        {
            if (ReferenceEquals(lambda, null))
            {
                result = default(TResult);
                return false;
            }

            result = lambda(arg1, arg2, arg3, arg4);
            return true;
        }
    }
}
