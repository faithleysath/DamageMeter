namespace DamageMeterRebuilt.Views;

internal sealed record DashboardBarView(
    string Title,
    bool TitleClickable,
    string SegmentLabel,
    IReadOnlyList<DashboardBarItem> Bars);
