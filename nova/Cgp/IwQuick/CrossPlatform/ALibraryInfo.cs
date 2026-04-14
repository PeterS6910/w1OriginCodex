using System;
using System.Collections.Generic;
using System.Reflection;
using Contal.IwQuick.Data;
using JetBrains.Annotations;

namespace Contal.IwQuick
{
    public abstract class ALibraryInfo
    {
        private ALibraryInfo _nextLibraryInfo;

        protected ALibraryInfo()
        {
            Register(this);
        }

        private void Register([NotNull] ALibraryInfo libraryInfo)
        {
            if (ReferenceEquals(libraryInfo, Root) ||
                ReferenceEquals(Root,null))
                return;

            ALibraryInfo libInfo = Root;
            bool resolved = false;

            while (!resolved)
            {
                if (libInfo._nextLibraryInfo == null)
                {
                    libInfo._nextLibraryInfo = libraryInfo;
                    resolved = true;
                }
                else
                {
                    libInfo = _nextLibraryInfo;
                }
            }

            
        }

        /// <summary>
        /// 
        /// </summary>
        public void Iterate([CanBeNull] Action<ALibraryInfo> lambda)
        {
            Process(lambda);
            if (_nextLibraryInfo != null)
                _nextLibraryInfo.Iterate(lambda);
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void Process([CanBeNull] Action<ALibraryInfo> lambda);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="cached"></param>
        /// <returns></returns>
        public IEnumerable<Type> GetClassesWithAttribute(Type attribute, bool cached)
        {
            if (cached)
                return GetCachedClassesWithAttribute(attribute);

            var ret = GetClassesWithAttributeInRuntime(attribute);

            return ret;


        }

        private IEnumerable<Type> GetClassesWithAttributeInRuntime(Type attribute)
        {
            var a = GetExecutingAssembly();

            var enumerator = a.GetTypes().GetEnumerator();

            bool movedForwad;

            do
            {
                movedForwad = enumerator.MoveNext();
                

                if (movedForwad)
                {
                    var t = enumerator.Current as Type;
                    if (t == null)
                        yield break;

                    yield return t;
                    var attrCheckResult = t.GetCustomAttributes(attribute, false);

                    if (!attrCheckResult.IsEmpty())
                        yield return t;
                    
                }
                else
                {
                    yield break;
                }
// ReSharper disable once ConditionIsAlwaysTrueOrFalse
            } while (movedForwad);
           
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        protected virtual IEnumerable<Type> GetCachedClassesWithAttribute(Type attribute)
        {
            if (attribute == typeof (LwSerializeAttribute))
            {
                return GetLwSerializableClasses();
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<Type> GetLwSerializableClasses()
        {
            yield break;
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Assembly GetExecutingAssembly();

        [PublicAPI]
        public static readonly IwQuickLibraryInfo Root = IwQuickLibraryInfo.Singleton;

        /// <summary>
        /// for marking the root class in reflection ; 
        /// internal to disallow the marking for other classes
        /// </summary>
        internal class LibraryInfoRootClassAttribute : Attribute
        {
            
        }
    }
}
