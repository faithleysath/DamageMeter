using Godot;

namespace DamageMeterRebuilt.Views;

internal sealed record DashboardBarItem(
    string Key,
    string Label,
    int Value,
    string? DisplayText = null,
    bool Clickable = false,
    Color? ColorOverride = null,
    Texture2D? Icon = null);
