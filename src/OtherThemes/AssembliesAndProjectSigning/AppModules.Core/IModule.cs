namespace AppModules.Core
{
    public interface IModule
    {
        string CallAlias { get; }
        string Name { get; }
        string Description { get; }
        void Work();
    }
}