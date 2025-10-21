namespace Contal.IwQuick.Data
{
    public static class UnifiedFormat
    {
        public const string DATETIME_FORMAT = "dd.MM.yyyy hh:mm:ss";
        public static string DateTime
        {
            get
            {
                return System.DateTime.Now.ToString();
            }
        }

        public static string GetDateTime(System.DateTime dateTime)
        {
            return dateTime.ToString(DATETIME_FORMAT);
        }
    }
}
