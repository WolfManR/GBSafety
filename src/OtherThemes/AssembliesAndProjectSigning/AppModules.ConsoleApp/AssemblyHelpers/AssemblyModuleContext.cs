using System.Reflection;
using System.Runtime.Loader;

namespace AppModules.ConsoleApp.AssemblyHelpers;

public class AssemblyModuleContext : AssemblyLoadContext
{
    public AssemblyModuleContext() : base(isCollectible: true) { }

    protected override Assembly? Load(AssemblyName assemblyName) => base.Load(assemblyName);
}