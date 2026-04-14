using System;
using System.Collections.Generic;
using Contal.Cgp.Globals;
using Contal.IwQuick;

namespace Contal.Cgp.NCAS.CCU.LogicalConfigurator
{
    public sealed partial class LogicalConfigurator :
        ASingleton<LogicalConfigurator>
    {
        private readonly Dictionary<ObjectType, SpecialOutputsConfigurator> _specialOutputsConfigurators;
        private readonly object _lockObject = new object();

        private LogicalConfigurator()
            : base(null)
        {
            _specialOutputsConfigurators = new Dictionary<ObjectType, SpecialOutputsConfigurator>
            {
                {ObjectType.DCU, new SpecialOutputsConfiguratorForDcu()},
                {ObjectType.AlarmArea, new SpecialOutputsConfiguratorForAlarmArea()}
            };
        }

        public void Configure(IEnumerable<IdAndObjectType> recentlySavedObjects)
        {
            lock (_lockObject)
            {
                foreach (var recentlySavedObject in recentlySavedObjects)
                {
                    SpecialOutputsConfigurator configurator;

                    if (!_specialOutputsConfigurators.TryGetValue(recentlySavedObject.ObjectType, out configurator))
                    {
                        continue;
                    }

                    configurator.Configure(recentlySavedObject);
                }
            }
        }

        public void Unconfigure(
            IdAndObjectType idAndObjectType,
            DB.IDbObject newDbObject)
        {
            lock (_lockObject)
            {
                SpecialOutputsConfigurator configurator;

                if (!_specialOutputsConfigurators.TryGetValue(idAndObjectType.ObjectType, out configurator))
                {
                    return;
                }

                configurator.Unconfigure(
                    (Guid) idAndObjectType.Id,
                    newDbObject);
            }
        }
    }
}
