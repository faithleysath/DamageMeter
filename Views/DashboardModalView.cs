namespace DamageMeterRebuilt.Views;

internal sealed record DashboardModalView(
    string Title,
    IReadOnlyList<DashboardMiniPanelView> Panels);
