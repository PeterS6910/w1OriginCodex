using System;
using JetBrains.Annotations;

namespace Contal.IwQuick
{
    public class StaticDestructor: ADisposable
    {
        private readonly Action _destructor = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destructor"></param>
        /// <param name="bindToUnhandledException"></param>
        public StaticDestructor(
            [NotNull] Action destructor,
            bool bindToUnhandledException)
        {
            Validator.CheckForNull(destructor,"destructor");

            _destructor = destructor;

            if (bindToUnhandledException) 
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            EnsureDestructorCalled();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destructorRoutine"></param>
        /// <param name="bindToUnhandledException"></param>
        /// <returns></returns>
        public static StaticDestructor Bind(
            [NotNull] Action destructorRoutine,
            bool bindToUnhandledException)
        {
            return new StaticDestructor(destructorRoutine, bindToUnhandledException);
        }

        private bool _destructorCalled = false;
        private void EnsureDestructorCalled()
        {
            lock (this)
            {
                if (!_destructorCalled)
                {
                    _destructorCalled = true;
                    if (null != _destructor)
                        try
                        {
                            _destructor();
                        }
                        catch
                        {
                        }
                }

            }
        }

        
        #region ADisposable Members

        protected override void InternalDispose(bool isExplicitDispose)
        {
            EnsureDestructorCalled();
        }

        

        #endregion
    }
}
