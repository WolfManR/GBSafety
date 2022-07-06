namespace AppModules.ConsoleApp.AssemblyHelpers;

public class ModuleHandlers : List<ModuleHandler>, IDisposable
{
    public void Dispose()
    {
        foreach (var moduleHandler in this)
        {
            moduleHandler.Dispose();
        }
    }
}