using AppModules.Core;

namespace AppModule1
{
    public class FactorialModule : IModule
    {
        public string CallAlias { get; } = "fact";
        public string Name { get; } = "factorial";
        public string Description { get; } = "Calculate simple factorial";

        public void Work()
        {
            Console.Write("Input integer number:");
            var input = Console.ReadLine();
            if (!int.TryParse(input, out var number)) throw new ArgumentException("Not a integer", nameof(input));
            int result = 1;
            for (var i = 1; i <= number; i++)
            {
                result *= i;
            }
            Console.WriteLine($"Factorial of number {number} is {result}");
        }
    }
}