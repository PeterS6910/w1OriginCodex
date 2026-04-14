using System;
using Contal.IwQuick;
using Contal.IwQuick.Sys.Microsoft;

namespace Contal.IwQuick.Sys
{
    /// <summary>
    /// 
    /// </summary>
    public class TimeManager
    {
        private static volatile TimeManager _singleton = null;
        private static readonly object _syncRoot = new object();

        private TimeManager()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public static TimeManager Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new TimeManager();
                    }
                
                return _singleton;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        public void SetTime(DateTime date)
        {
            if (!SystemTime.UseDateTimeNow)
            {
                SystemTime.SetLocalTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second,
                    date.Millisecond);
            }
            else
            {
                date = date.ToUniversalTime();
                SystemTime.SetSystemTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second,
                    date.Millisecond);
            }

            FireTimeHasChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        public void SetUtcTime(DateTime date)
        {
            if (!SystemTime.UseDateTimeNow)
            {
                date = date.ToLocalTime();
                SystemTime.SetLocalTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second,
                    date.Millisecond);
            }
            else
            {
                SystemTime.SetSystemTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second,
                    date.Millisecond);
            }

            FireTimeHasChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        public event DVoid2Void TimeHasBeenChanged = null;

        private void FireTimeHasChanged()
        {
            if (TimeHasBeenChanged != null)
                TimeHasBeenChanged();
        }
    }
}
