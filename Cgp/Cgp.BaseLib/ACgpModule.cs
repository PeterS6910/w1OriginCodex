using System;
using System.Collections.Generic;

using Contal.IwQuick;
using JetBrains.Annotations;

namespace Contal.Cgp.BaseLib
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ACgpModule:IComparable
    {
        // namespace validation must end with dot
        public const string CGP_NAMESPACE_VALIDATION = "Contal.Cgp.";

        /// <summary>
        /// list of all modules registered in runtime
        /// </summary>
        private static Dictionary<string, ACgpModule> _allModules = new Dictionary<string, ACgpModule>();

        /// <summary>
        /// referrence to module's main configuration
        /// </summary>
        private CgpModuleConfiguration _generalConfig = null;

        /// <summary>
        /// unique name in the set of the same type
        /// </summary>
        public string Name
        {
            get { return _generalConfig.Name; }
        }

        /// <param name="typeName"></param>
        /// <param name="name"></param>
        /// <returns>return CGP designation in format "short_type_name/module_name"</returns>
        /// <exception cref="InvalidOperationException">if the module is not from the ACgpModule.CGP_NAMESPACE_VALIDATION namespace</exception>
        protected internal static string GetCgpDesignation(
            [NotNull] ref string typeName, 
            [NotNull] string name)
        {
            Validator.CheckForNull(typeName,"typeName");
            Validator.CheckNullString(name,"name");

// ReSharper disable once StringIndexOfIsCultureSpecific.2
            if (typeName.IndexOf(CGP_NAMESPACE_VALIDATION, 0) != 0)
                throw new InvalidOperationException("CGP module " + name + " is not from " + CGP_NAMESPACE_VALIDATION + " namespace");
            
            typeName = typeName.Substring(CGP_NAMESPACE_VALIDATION.Length);

            string designation = typeName + "/" + name;
            
            return designation;
        }

        private string _cgpDesignation = null;
        /// <summary>
        /// unique CGP designation of the module
        /// </summary>
        public string CgpDesignation
        {
            get
            {
                if (null == _generalConfig.CgpType || null == _cgpDesignation)
                {
                    string typeName = this.GetType().FullName;
                    _cgpDesignation = GetCgpDesignation(ref typeName, _generalConfig.Name);
                    _generalConfig.CgpType = typeName;
                }

                return _cgpDesignation;
            }
        }

        /// <summary>
        /// CGP type name
        /// </summary>
        public string CgpType
        {
            get
            {
                if (null == _generalConfig.CgpType)
                {
                    string typeName = this.GetType().FullName;
                    GetCgpDesignation(ref typeName, _generalConfig.Name);
                    _generalConfig.CgpType = typeName;
                }

                return _generalConfig.CgpType;
            }
        }

        
        /// <summary>
        /// optional description of the module
        /// </summary>
        public string Description
        {
            get { return _generalConfig.Description; }
        }

        /// <summary>
        /// constructor with initial configuration
        /// </summary>
        /// <param name="generalConfig"></param>
        public ACgpModule([NotNull] CgpModuleConfiguration generalConfig)
        {
            Validator.CheckForNull(generalConfig,"generalConfig");
            _generalConfig = generalConfig;

            RegisterInstance();
        }

        private void RegisterInstance()
        {
            lock (_allModules)
            {
                if (_allModules.ContainsKey(CgpDesignation))
                    throw new AlreadyExistsException(_generalConfig.Name,"Module with designation " + CgpDesignation + " already exists");

                _allModules[CgpDesignation] = this;
            }
        }

        /// <summary>
        /// searches for the CGP module in the general module namespace
        /// </summary>
        /// <param name="cgpDesignation">CGP designation of the module</param>
        /// <returns>instance of the module, or null if not found</returns>
        public static ACgpModule FindInstance(string cgpDesignation)
        {
            if (Validator.IsNullString(cgpDesignation))
                return null;

            if (_allModules.ContainsKey(cgpDesignation))
                return _allModules[cgpDesignation];
            else
                return null;
        }

        /// <summary>
        /// searches for the CGP module in the general module namespace
        /// </summary>
        /// <param name="name">name of the module</param>
        /// <param name="type">real declaration type of the module</param>
        /// <returns>instance of the module, or null if not found</returns>
        public static ACgpModule FindInstance(Type type, string name)
        {
            string typeName = type.FullName;
            string designation = GetCgpDesignation(ref typeName, name);

            if (_allModules.ContainsKey(designation))
                return _allModules[designation];
            else
                return null;
        }

        private int _referrenceCounter = 0;
        private Dictionary<short, LinkedList<ACgpModule>> _referrencedModules = new Dictionary<short, LinkedList<ACgpModule>>();
        private LinkedList<ACgpModule> _referrencedByModules = new LinkedList<ACgpModule>();

        /// <summary>
        /// adds, by whom the module is referrenced
        /// </summary>
        /// <param name="module">module to be registered by internal ACgpModule mechanism</param>
        protected internal void AddReferencedBy(ACgpModule module)
        {
            if (null == module)
                return;
            
            lock (_referrencedByModules)
            {
                if (!_referrencedByModules.Contains(module))
                    _referrencedByModules.AddLast(module);
            }
        }

        /// <summary>
        /// removes, by whom the module is referrenced
        /// </summary>
        /// <param name="module">module to be unregisterred by internal ACgpModule mechanism</param>
        protected internal void RemoveReferencedBy(ACgpModule module)
        {
            if (null == module)
                return;

            lock (_referrencedByModules)
            {
                LinkedListNode<ACgpModule> node = _referrencedByModules.Find(module);
                if (null != node)
                    _referrencedByModules.Remove(node);
            }
        }

        /// <summary>
        /// referrences another ACgpModule within set of relationshipType
        /// </summary>
        /// <param name="relationshipType">custom relationShipType</param>
        /// <param name="module">module to be referrenced</param>
        /// <exception cref="AlreadyExistsException">if such module is already referrenced</exception>
        public virtual void Referrence(
            short relationshipType,
            [NotNull] ACgpModule module)
        {
            Validator.CheckForNull(module,"module");

            lock (_referrencedModules)
            {
                LinkedList<ACgpModule> list = null;
                bool newList = false;
                if (!_referrencedModules.TryGetValue(relationshipType,out list))
                {
                    list = new LinkedList<ACgpModule>();
                    newList = true;
                }

                if (list.Contains(module))
                    throw new AlreadyExistsException(module, "Module " + module.Name + " is already referrenced");

                _referrenceCounter++;
                list.AddLast(module);
                module.AddReferencedBy(this);

                if (newList)
                    _referrencedModules[relationshipType] = list;
            }
        }

        /// <summary>
        /// dereferrences another ACgpModule within set of relationshipType
        /// </summary>
        /// <param name="relationshipType">custom relationship</param>
        /// <param name="module">module to be referrenced</param>
        /// <exception cref="DoesNotExistException">if such module is not referrenced by this module,
        /// or such relationship does not exist</exception>
        public virtual void Dereferrence(
            short relationshipType,
            [NotNull] ACgpModule module)
        {
            Validator.CheckForNull(module,"module");

            lock (_referrencedModules)
            {
                LinkedList<ACgpModule> list;
                if (!_referrencedModules.TryGetValue(relationshipType, out list))
                    throw new DoesNotExistException(relationshipType, "Relationship with id " + relationshipType + " does not exist");

                LinkedListNode<ACgpModule> node = list.Find(module);
                if (!list.Contains(module))
                    throw new DoesNotExistException(module, "Module " + module.Name + " does not exist");
                else
                {
                    _referrenceCounter--;
                    list.Remove(module);
                    module.RemoveReferencedBy(this);
                }

                if (list.Count == 0)
                    _referrencedModules.Remove(relationshipType);
            }

        }

        /// <summary>
        /// referrences another ACgpModule within set of relationshipType
        /// </summary>
        /// <param name="relationshipType">custom relationship</param>
        /// <param name="cgpDesignation">CGP designation of the module</param>
        /// <exception cref="AlreadyExistsException">if such module is already referrenced</exception>
        public virtual void Referrence(short relationshipType, string cgpDesignation)
        {
            ACgpModule module = FindInstance(cgpDesignation);
            if (null == module)
                throw new DoesNotExistException(cgpDesignation);

            Referrence(relationshipType, module);
        }

        /// <summary>
        /// dereferrences another ACgpModule within set of relationshipType
        /// </summary>
        /// <param name="relationshipType">custom relationship</param>
        /// <param name="cgpDesignation">CGP designation of the module</param>
        /// <exception cref="DoesNotExistException">if such module is not referrenced by this module,
        /// or such relationship does not exist</exception>
        public virtual void Dereferrence(short relationshipType, string cgpDesignation)
        {
            ACgpModule module = FindInstance(cgpDesignation);
            if (null == module)
                throw new DoesNotExistException(cgpDesignation);

            Dereferrence(relationshipType, module);
        }

        /// <summary>
        /// returns references of one relationship type
        /// </summary>
        /// <param name="relationshipType">relationship type</param>
        /// <returns>array of referrenced modules</returns>
        public ACgpModule[] GetReferrences(short relationshipType)
        {
            lock (_referrencedModules)
            {
                LinkedList<ACgpModule> list;
                if (!_referrencedModules.TryGetValue(relationshipType, out list))
                    return new ACgpModule[0];

                ACgpModule[] array = new ACgpModule[list.Count];
                list.CopyTo(array, 0);
                return array;
            }
        }

        /// <summary>
        /// returns all referrenced modules
        /// </summary>
        /// <returns></returns>
        public ACgpModule[] GetReferrences()
        {
            lock (_referrencedModules)
            {
                ACgpModule[] array = new ACgpModule[_referrenceCounter];

                int pos = 0;
                foreach (KeyValuePair<short, LinkedList<ACgpModule>> pair in _referrencedModules)
                {
                    pair.Value.CopyTo(array, pos);
                    pos += pair.Value.Count;
                }

                return array;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ACgpModule[] GetReferrencedBy()
        {
            lock (_referrencedByModules)
            {
                ACgpModule[] array = new ACgpModule[_referrencedByModules.Count];
                _referrencedByModules.CopyTo(array, 0);
                return array;
            }
        }

        public void ClearReferrences()
        {
            lock (_referrencedModules)
            {
                if (_referrencedModules.Count == 0)
                    return;

                foreach (LinkedList<ACgpModule> list in _referrencedModules.Values)
                {
                    foreach (ACgpModule module in list)
                    {
                        module.RemoveReferencedBy(this);
                    }

                    list.Clear();
                }

                _referrencedModules.Clear();
            }
        }

        public void ClearReferrences(short relationshipId)
        {
            lock (_referrencedModules)
            {
                LinkedList<ACgpModule> list;
                if (!_referrencedModules.TryGetValue(relationshipId, out list))
                    throw new DoesNotExistException(relationshipId);
               
                foreach (ACgpModule module in list)
                {
                    module.RemoveReferencedBy(this);
                }

                list.Clear();
            }
        }

#if DEBUG
        public
#else
        protected
#endif
        void StoreReferrences()
        {
            if (_referrenceCounter == 0)
                _generalConfig._referrenceStorage = null;
            else
            {
                lock (_referrencedModules)
                {
                    int overhead = _referrencedModules.Count;

                    
                    
                    foreach (KeyValuePair<short, LinkedList<ACgpModule>> pair in _referrencedModules)
                    {
                        string[] designations = new string[pair.Value.Count];
                        

                        int i = 0;
                        foreach (ACgpModule module in pair.Value)
                        {
                           designations[i++] = module.CgpDesignation;
                        }
                        _generalConfig._referrenceStorage[pair.Key] = designations;
                    }
                }
            }
        }

#if DEBUG
        public
#else
        protected
#endif
        void LoadReferrences()
        {
            lock (_referrencedModules)
            {
                ClearReferrences();

                if (_generalConfig._referrenceStorage != null &&
                    _generalConfig._referrenceStorage.Count > 0)
                {
                    foreach (KeyValuePair<short,string[]> pair in _generalConfig._referrenceStorage)
                    {

                        foreach (string designation in pair.Value)
                        {
                            Referrence(pair.Key, designation);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// intended for initialization of the referrence mesh
        /// </summary>
        public static void ResolveReferrences()
        {
            foreach (ACgpModule module in _allModules.Values)
            {
                try
                {
                    module.LoadReferrences();
                }
                catch
                {
                }
            }
        }


        public override string ToString()
        {
            return _generalConfig.Name;
        }

        public override int GetHashCode()
        {
            // let the property to be called to ensure _cgpDesignation being not null
            return CgpDesignation.GetHashCode();
        }

        #region IComparable Members

        /// <summary>
        /// returns :
        /// -2 if object null; 
        /// -1 if object is not of ACgpModule type; 
        /// 0 if the CGP designation is equal
        /// 1 if the CGP designation is NOT equal
        /// </summary>
        /// <param name="obj">other ACgpModule instance</param>
        public int CompareTo(object obj)
        {
            if (null == obj)
                return -2;

            if (obj is ACgpModule)
            {
                if (((ACgpModule)obj).CgpDesignation == this.CgpDesignation)
                    return 0;
                else
                    return 1;
            }
            else
                return -1;

            
            
        }

        #endregion
    }
}
