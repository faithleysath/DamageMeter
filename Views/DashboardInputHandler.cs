using DamageMeterRebuilt.Infrastructure;
using Godot;

namespace DamageMeterRebuilt.Views;

internal sealed partial class DashboardInputHandler : Node
{
    private bool _f7Down;
    private bool _f8Down;
    private bool _f9Down;
    private bool _escapeDown;

    public event Action? ToggleRequested;
    public event Action<bool>? CycleViewRequested;
    public event Action? ToggleScopeRequested;
    public event Action? CloseDashboardRequested;

    public DashboardInputHandler()
    {
        Name = "DamageMeterRebuiltInputHandler";
        ProcessMode = ProcessModeEnum.Always;
        TreeEntered += OnTreeEntered;
        TreeExiting += OnTreeExiting;
    }

    private void OnTreeEntered()
    {
        var tree = GetTree();
        if (tree is not null)
        {
            tree.ProcessFrame += OnProcessFrame;
        }

        LoggerAdapter.Info("Dashboard input handler attached to scene tree.");
    }

    private void OnTreeExiting()
    {
        var tree = GetTree();
        if (tree is not null)
        {
            tree.ProcessFrame -= OnProcessFrame;
        }
    }

    private void OnProcessFrame()
    {
        PollKey(Key.F7, ref _f7Down, "F7", () => ToggleRequested?.Invoke());
        PollKey(
            Key.F8,
            ref _f8Down,
            IsShiftPressed() ? "Shift+F8" : "F8",
            () => CycleViewRequested?.Invoke(!IsShiftPressed()));
        PollKey(Key.F9, ref _f9Down, "F9", () => ToggleScopeRequested?.Invoke());
        PollKey(Key.Escape, ref _escapeDown, "Escape", () => CloseDashboardRequested?.Invoke());
    }

    private static bool IsShiftPressed()
    {
        return Input.IsPhysicalKeyPressed(Key.Shift) || Input.IsKeyPressed(Key.Shift);
    }

    private static bool IsPressed(Key key)
    {
        return Input.IsPhysicalKeyPressed(key) || Input.IsKeyPressed(key);
    }

    private void PollKey(Key key, ref bool wasDown, string label, Action onPressed)
    {
        var isDown = IsPressed(key);
        if (isDown && !wasDown)
        {
            LoggerAdapter.Info($"Hotkey received: {label}");
            onPressed();
        }

        wasDown = isDown;
    }
}
