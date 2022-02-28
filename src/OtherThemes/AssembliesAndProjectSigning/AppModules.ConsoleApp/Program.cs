using AppModules.ConsoleApp.AssemblyHelpers;

using ModuleHandlers moduleHandlers = new();

while (true)
{
    Console.WriteLine("Write path of module library or \"work\" to start working with modules");
    var input = Console.ReadLine();

    if (input is "work" or null) break;

    var file = new FileInfo(input);
    if (file.Exists && file.Extension == ".dll")
    {
        moduleHandlers.Add(new(input));
    }
}

Console.WriteLine("Loading modules..");
var modules = moduleHandlers.SelectMany(m => m.LoadAssembly()).ToList();
Console.WriteLine("Modules loaded");
Console.WriteLine($"Count of loaded modules: {modules.Count}");

var menu = modules.Select((m, index) => new MenuItem(m.CallAlias, m.Name, m.Description, Id: index)).ToList();
var quit = new MenuItem("quit", "Exit", "Exit from program", menu.Count + 1);
menu.Add(quit);

while (true)
{
    foreach (var item in menu)
    {
        Console.WriteLine($"{item.CallAlias} : {item.Name}");
    }

    var selectedAlias = Console.ReadLine();
    if (selectedAlias == quit.CallAlias) return;

    var selected = menu.FirstOrDefault(m => m.CallAlias == selectedAlias);
    if (selected is null) return;

    Console.WriteLine(selected.Description);

    modules[selected.Id].Work();
}

record MenuItem(string CallAlias, string Name, string Description, int Id);