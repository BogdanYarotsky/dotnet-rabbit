using System.CommandLine;

namespace Rabbit;

public static class CommandLineExtensions
{
    public static void AddGlobalOptions(this RootCommand root, params Option[] options)
    {
        foreach (var option in options)
        {
            root.AddGlobalOption(option);
        }
    }

    public static void AddCommands(this RootCommand root, params Command[] commands)
    {
        foreach (var command in commands)
        {
            root.AddCommand(command);
        }
    }
}