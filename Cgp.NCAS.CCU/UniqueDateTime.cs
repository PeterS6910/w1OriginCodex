using System;

using Contal.Drivers.LPC3250;

namespace Contal.Cgp.NCAS.CCU
{
    public static class UniqueDateTime
    {
        private static DateTime? _lastDateTime;
        private static int _miliseconds;
        private static readonly object _lockGetDateTime = new object();

        /// <summary>
        /// Returns unique DateTime
        /// </summary>
        public static DateTime GetDateTime
        {
            get
            {
                if (WindowsCE.Build >= Definitions.NCASConstants.FW_VERSION_GET_LOCAL_TIME_WITH_MILISCONDS)
                    return CcuCore.LocalTime;

                lock (_lockGetDateTime)
                {
                    DateTime currentDateTime = CcuCore.LocalTime;

                    if (_lastDateTime == null || currentDateTime > _lastDateTime.Value.AddMilliseconds(_miliseconds))
                    {
                        _miliseconds = 0;
                        _lastDateTime = currentDateTime;
                    }
                    else
                    {
                        _miliseconds++;
                        currentDateTime = currentDateTime.AddMilliseconds(_miliseconds);
                    }

                    return currentDateTime;
                }
            }
        }
    }
}
