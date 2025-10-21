using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Contal.IwQuick;
using JetBrains.Annotations;

namespace Contal.Cgp.BaseLib
{
    public class CgpModuleFactory
    {
        public static void CreateModules([NotNull] IEnumerable<CgpModuleConfiguration> moduleConfigurations)
        {
// ReSharper disable once PossibleMultipleEnumeration
            Validator.CheckForNull(moduleConfigurations,"moduleConfigurations");

            
            ACgpModule module;
            string typeFullName;
            Type moduleType;

            LinkedList<ACgpModule> modules = new LinkedList<ACgpModule>();

            foreach (CgpModuleConfiguration config in moduleConfigurations)
            {
                typeFullName = ACgpModule.CGP_NAMESPACE_VALIDATION + config.CgpType;


                try
                {
                    moduleType = Type.GetType(typeFullName);
                    if (null == moduleType)
                        throw new ArgumentException();

                    if (!moduleType.IsSubclassOf(typeof(ACgpModule)))
                        throw new ArgumentException();

                    ConstructorInfo ci = moduleType.GetConstructor(new Type[] { typeof(CgpModuleConfiguration) });
                    if (ci == null)
                        throw new ArgumentException();

                    module = (ACgpModule) ci.Invoke(new object[] { config });
                    if (null != module)
                        modules.AddLast(module);
                    
                }
                catch
                {
                    
                }

            }

            ACgpModule.ResolveReferrences();
        }
    }
}
