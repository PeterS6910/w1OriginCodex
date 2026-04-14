namespace Contal.IwQuick
{
    public static class ObjectExtensions
    {
        public static bool IsNull(this object o)
        {
            return ReferenceEquals(o, null);
        }
    }
}
