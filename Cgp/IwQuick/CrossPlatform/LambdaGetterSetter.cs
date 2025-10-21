using System;
using JetBrains.Annotations;

namespace Contal.IwQuick
{
    /// <summary>
    /// supplement for .NET 3.5 missing language-included getter/setter with default value definition
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LambdaGetterSetter<T> : AGetterSetter<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueToBePassedAndOrModified"></param>
        public delegate void DGetterLambda(ref T valueToBePassedAndOrModified);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueToBeModified"></param>
        public delegate void DSetterLambda(ref T valueToBeModified);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valueToBeDisposed"></param>
        public delegate void DDisposalLambda(ref T valueToBeDisposed, bool isExplicitDispose);

        private DGetterLambda _getterLambda;
        private DSetterLambda _setterLambda;
        private DDisposalLambda _disposalLambda;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <param name="getterLambda"></param>
        /// <param name="setterLambda"></param>
        /// <param name="disposalLambda"></param>
        public LambdaGetterSetter(
            T defaultValue,
            [CanBeNull] DGetterLambda getterLambda,
            [CanBeNull] DSetterLambda setterLambda,
            [CanBeNull] DDisposalLambda disposalLambda )
            :base(defaultValue)
        {
            _getterLambda = getterLambda;
            _setterLambda = setterLambda;
            _disposalLambda = disposalLambda;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override void GetOrModifyValue(ref T valueToBePassedAndOrModified)
        {
            if (_getterLambda == null)
                throw new InvalidOperationException("Getter not available");

            _getterLambda(ref valueToBePassedAndOrModified);
        }

        protected override void SetValue(ref T valueToBeModified)
        {
            if (_setterLambda == null)
                throw new InvalidOperationException("Setter not available");

            _setterLambda(ref valueToBeModified);
        }

        protected override void DisposeValue(ref T valueToBeDisposed, bool isExplicitDispose)
        {
            if (null != _disposalLambda)
                _disposalLambda(ref valueToBeDisposed,isExplicitDispose);
            else
                valueToBeDisposed = default(T);


            _getterLambda = null;
            _setterLambda = null;
            _disposalLambda = null;
        }



    }
}
