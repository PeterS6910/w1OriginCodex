namespace Contal.Cgp.Globals
{
    public interface IShortObject
    {
        ObjectType ObjectType { get; }
        string GetSubTypeImageString(object value);
        string Name { get; }
        object Id { get; }
    }
}
