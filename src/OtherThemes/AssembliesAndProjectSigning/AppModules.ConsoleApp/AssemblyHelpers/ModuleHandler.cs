using AppModules.Core;

using System.Reflection;

namespace AppModules.ConsoleApp.AssemblyHelpers;

public class ModuleHandler : IDisposable
{
    private bool _isDisposed;
    public ModuleHandler(string modulePath) => ModulePath = modulePath;
    private string ModulePath { get; set; }
    private Lazy<AssemblyModuleContext> Context { get; } = new(() => new());

    public IEnumerable<IModule> LoadAssembly()
    {
        if (_isDisposed) throw new ObjectDisposedException("Assembly already disposed");

        if (!File.Exists(ModulePath)) yield break;
        
        Assembly assembly = Context.Value.LoadFromAssemblyPath(ModulePath);

        var moduleTypes = assembly.DefinedTypes.Where(i => i.ImplementedInterfaces.Contains(typeof(IModule))).ToList();

        foreach (var moduleType in moduleTypes)
        {
            var instance = (IModule)Activator.CreateInstance(moduleType)!;
            yield return instance;
        }
    }

    public void Dispose()
    {
        if(_isDisposed) return;
        _isDisposed = true;
        if(Context.IsValueCreated) Context.Value.Unload();
    }
}