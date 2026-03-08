namespace DamageMeterRebuilt.Views;

internal sealed record DashboardMiniPanelView(
    DashboardView View,
    string Title,
    bool TitleClickable,
    bool BarsClickable,
    IReadOnlyList<DashboardBarItem> Bars);
