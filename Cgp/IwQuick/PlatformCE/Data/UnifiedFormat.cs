using Contal.IwQuick.Sys.Microsoft;


namespace Contal.IwQuick.Data
{
    public static class UnifiedFormat
    {
        public const string DATETIME_FORMAT = "dd.MM.yy HH:mm:ss";
        public const string DATETIME_FORMAT_SORTABLE = "yyMMdd HH:mm:ss";
        public const string DATETIME_FORMAT_WITH_MS = "dd.MM.yy HH:mm:ss.fff";
        public const string DATETIME_FORMAT_SORTABLE_WITH_MS = "yyMMdd HH:mm:ss.fff";


        public static string DateTime
        {
            get
            {
                System.DateTime now = SystemTime.GetLocalTime();

                return now.ToString(DATETIME_FORMAT);
            }
        }

        public static string DateTimePrecise
        {
            get
            {
                System.DateTime dt = SystemTime.GetLocalTime(); //System.DateTime.Now;
                if (SystemTime.MilisecondsSynchronized)
                    return dt.ToString(DATETIME_FORMAT_WITH_MS);
                else
                    return dt.ToString(DATETIME_FORMAT);// +":" + string.Format("{0:000}", (dt.Ticks % 1000));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string DateTimePreciseSortable
        {
            get
            {
                System.DateTime dt = SystemTime.GetLocalTime(); //System.DateTime.Now;
                return dt.ToString(DATETIME_FORMAT_SORTABLE_WITH_MS);
            }
        }

        public static string GetDateTime(System.DateTime dateTime)
        {
            return dateTime.ToString(DATETIME_FORMAT);
        }

        public static string GetDateTimePrecise(System.DateTime dateTime)
        {
            return dateTime.ToString(DATETIME_FORMAT_WITH_MS);
        }

        public static string GetDateTimeSortable(System.DateTime dateTime)
        {
            return dateTime.ToString(DATETIME_FORMAT_SORTABLE);
        }
    }
}
