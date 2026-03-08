using Godot;

namespace DamageMeterRebuilt.Infrastructure;

internal static class LoggerAdapter
{
    private const string Prefix = "[DamageMeterRebuilt]";

    public static void Info(string message)
    {
        GD.Print($"{Prefix} {message}");
    }

    public static void Error(string message, Exception exception)
    {
        GD.PushError($"{Prefix} {message}: {exception}");
    }
}
