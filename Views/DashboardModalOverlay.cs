using DamageMeterRebuilt.Infrastructure;
using Godot;

namespace DamageMeterRebuilt.Views;

internal sealed partial class DashboardModalOverlay : CanvasLayer
{
    private readonly ColorRect _background;
    private readonly Label _titleLabel;
    private readonly GridContainer _grid;

    public event Action? CloseRequested;
    public event Action<DashboardView>? PanelTitleRequested;
    public event Action<DashboardView, string>? PanelBarActivated;

    public DashboardModalOverlay()
    {
        Name = "DamageMeterRebuiltDashboard";
        Layer = 110;
        ProcessMode = ProcessModeEnum.Always;

        _background = new ColorRect
        {
            Color = new Color(0f, 0f, 0f, 0.6f),
            MouseFilter = Control.MouseFilterEnum.Stop
        };
        _background.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        _background.GuiInput += OnBackgroundGuiInput;
        AddChild(_background);

        var margin = new MarginContainer();
        margin.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        margin.AddThemeConstantOverride("margin_left", 40);
        margin.AddThemeConstantOverride("margin_right", 40);
        margin.AddThemeConstantOverride("margin_top", 30);
        margin.AddThemeConstantOverride("margin_bottom", 30);
        _background.AddChild(margin);

        var shell = new PanelContainer();
        shell.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        shell.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        shell.AddThemeStyleboxOverride("panel", CreateShellStyle());
        margin.AddChild(shell);

        var root = new VBoxContainer();
        root.AddThemeConstantOverride("separation", 10);
        shell.AddChild(root);

        var header = new HBoxContainer();
        header.AddThemeConstantOverride("separation", 8);
        root.AddChild(header);

        _titleLabel = new Label
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        _titleLabel.AddThemeColorOverride("font_color", GoldColor);
        _titleLabel.AddThemeFontSizeOverride("font_size", 18);
        header.AddChild(_titleLabel);

        var closeButton = new Button
        {
            Text = "✕",
            Flat = true,
            CustomMinimumSize = new Vector2(32f, 28f)
        };
        closeButton.AddThemeFontSizeOverride("font_size", 16);
        closeButton.AddThemeColorOverride("font_color", DimText);
        closeButton.AddThemeColorOverride("font_hover_color", new Color(1f, 0.4f, 0.4f, 1f));
        closeButton.Pressed += () => CloseRequested?.Invoke();
        header.AddChild(closeButton);

        var separator = new HSeparator();
        separator.AddThemeStyleboxOverride("separator", new StyleBoxFlat
        {
            BgColor = new Color(1f, 0.84f, 0f, 0.2f)
        });
        separator.AddThemeConstantOverride("separation", 4);
        root.AddChild(separator);

        var scroll = new ScrollContainer
        {
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
            VerticalScrollMode = ScrollContainer.ScrollMode.Auto,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };
        root.AddChild(scroll);

        _grid = new GridContainer
        {
            Columns = 4,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        _grid.AddThemeConstantOverride("h_separation", 12);
        _grid.AddThemeConstantOverride("v_separation", 12);
        scroll.AddChild(_grid);
    }

    public void SetView(DashboardModalView view)
    {
        _titleLabel.Text = view.Title;
        ClearGrid();

        foreach (var panel in view.Panels)
        {
            _grid.AddChild(CreateMiniPanel(panel));
        }
    }

    private void OnBackgroundGuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Right })
        {
            return;
        }

        CloseRequested?.Invoke();
        GetViewport()?.SetInputAsHandled();
    }

    private void ClearGrid()
    {
        foreach (var child in _grid.GetChildren())
        {
            child.QueueFree();
        }
    }

    private PanelContainer CreateMiniPanel(DashboardMiniPanelView view)
    {
        var panel = new PanelContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            CustomMinimumSize = new Vector2(280f, 0f)
        };
        panel.AddThemeStyleboxOverride("panel", CreateMiniPanelStyle());

        var root = new VBoxContainer();
        root.AddThemeConstantOverride("separation", 3);
        panel.AddChild(root);

        if (view.TitleClickable)
        {
            var titleButton = new Button
            {
                Text = view.Title,
                Flat = true,
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                MouseDefaultCursorShape = Control.CursorShape.PointingHand
            };
            titleButton.AddThemeColorOverride("font_color", GoldColor);
            titleButton.AddThemeColorOverride("font_hover_color", new Color(1f, 0.95f, 0.5f, 1f));
            titleButton.AddThemeFontSizeOverride("font_size", 13);
            titleButton.Pressed += () => PanelTitleRequested?.Invoke(view.View);
            root.AddChild(titleButton);
        }
        else
        {
            var titleLabel = new Label
            {
                Text = view.Title,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            titleLabel.AddThemeColorOverride("font_color", GoldColor);
            titleLabel.AddThemeFontSizeOverride("font_size", 13);
            root.AddChild(titleLabel);
        }

        var separator = new HSeparator();
        separator.AddThemeStyleboxOverride("separator", new StyleBoxFlat
        {
            BgColor = new Color(1f, 0.84f, 0f, 0.15f)
        });
        separator.AddThemeConstantOverride("separation", 2);
        root.AddChild(separator);

        if (view.Bars.Count == 0)
        {
            var waiting = new Label
            {
                Text = I18n.WaitingForCombat,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            waiting.AddThemeColorOverride("font_color", DimText);
            waiting.AddThemeFontSizeOverride("font_size", 11);
            root.AddChild(waiting);
            return panel;
        }

        var bars = view.Bars.Take(8).ToList();
        var maxValue = bars.Max(bar => bar.Value);
        var totalValue = bars.Sum(bar => bar.Value);
        for (var index = 0; index < bars.Count; index++)
        {
            var bar = bars[index];
            root.AddChild(CreateBarNode(
                view.View,
                bar,
                DetailColors[index % DetailColors.Length],
                maxValue,
                totalValue,
                view.BarsClickable));
        }

        return panel;
    }

    private Panel CreateBarNode(
        DashboardView view,
        DashboardBarItem data,
        Color color,
        int maxValue,
        int totalValue,
        bool clickable)
    {
        var resolvedColor = data.ColorOverride ?? color;
        var fillRatio = maxValue > 0 ? Mathf.Clamp((float)data.Value / maxValue, 0f, 1f) : 0f;
        var percent = totalValue > 0 ? (float)data.Value / totalValue * 100f : 0f;

        var panel = new Panel
        {
            CustomMinimumSize = new Vector2(0f, 18f),
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            MouseFilter = clickable && data.Clickable ? Control.MouseFilterEnum.Stop : Control.MouseFilterEnum.Ignore
        };
        panel.AddThemeStyleboxOverride("panel", new StyleBoxFlat
        {
            BgColor = BarBgColor,
            CornerRadiusTopLeft = 2,
            CornerRadiusTopRight = 2,
            CornerRadiusBottomLeft = 2,
            CornerRadiusBottomRight = 2
        });

        var fill = new ColorRect
        {
            Color = new Color(resolvedColor.R, resolvedColor.G, resolvedColor.B, 0.35f),
            AnchorRight = fillRatio,
            AnchorBottom = 1f,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        panel.AddChild(fill);

        var label = new Label
        {
            Text = "  " + data.Label,
            AnchorRight = 0.55f,
            AnchorBottom = 1f,
            VerticalAlignment = VerticalAlignment.Center,
            ClipText = true,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        label.AddThemeFontSizeOverride("font_size", 11);
        label.AddThemeColorOverride("font_color", Colors.White);
        panel.AddChild(label);

        var valueLabel = new Label
        {
            Text = data.DisplayText ?? $"{data.Value}  ({percent:F1}%)  ",
            AnchorLeft = 0.5f,
            AnchorRight = 1f,
            AnchorBottom = 1f,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            ClipText = true,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        valueLabel.AddThemeFontSizeOverride("font_size", 11);
        valueLabel.AddThemeColorOverride("font_color", TextColor);
        panel.AddChild(valueLabel);

        if (clickable && data.Clickable)
        {
            panel.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
            panel.GuiInput += @event =>
            {
                if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
                {
                    return;
                }

                PanelBarActivated?.Invoke(view, data.Key);
                GetViewport()?.SetInputAsHandled();
            };
        }

        return panel;
    }

    private static StyleBoxFlat CreateShellStyle()
    {
        return new StyleBoxFlat
        {
            BgColor = new Color(0.04f, 0.04f, 0.1f, 0.95f),
            BorderWidthLeft = 1,
            BorderWidthTop = 1,
            BorderWidthRight = 1,
            BorderWidthBottom = 1,
            BorderColor = GoldBorder,
            CornerRadiusTopLeft = 10,
            CornerRadiusTopRight = 10,
            CornerRadiusBottomLeft = 10,
            CornerRadiusBottomRight = 10,
            ContentMarginLeft = 16f,
            ContentMarginRight = 16f,
            ContentMarginTop = 12f,
            ContentMarginBottom = 16f
        };
    }

    private static StyleBoxFlat CreateMiniPanelStyle()
    {
        return new StyleBoxFlat
        {
            BgColor = new Color(0.06f, 0.06f, 0.14f, 0.9f),
            BorderWidthLeft = 1,
            BorderWidthTop = 1,
            BorderWidthRight = 1,
            BorderWidthBottom = 1,
            BorderColor = new Color(1f, 0.84f, 0f, 0.15f),
            CornerRadiusTopLeft = 6,
            CornerRadiusTopRight = 6,
            CornerRadiusBottomLeft = 6,
            CornerRadiusBottomRight = 6,
            ContentMarginLeft = 8f,
            ContentMarginRight = 8f,
            ContentMarginTop = 6f,
            ContentMarginBottom = 8f
        };
    }

    private static readonly Color GoldColor = new(1f, 0.84f, 0f, 1f);
    private static readonly Color GoldBorder = new(1f, 0.84f, 0f, 0.3f);
    private static readonly Color TextColor = new(0.85f, 0.85f, 0.85f, 1f);
    private static readonly Color DimText = new(0.5f, 0.5f, 0.5f, 1f);
    private static readonly Color BarBgColor = new(0.06f, 0.06f, 0.12f, 0.95f);
    private static readonly Color[] DetailColors =
    {
        new Color(0.95f, 0.45f, 0.35f, 1f),
        new Color(0.95f, 0.7f, 0.2f, 1f),
        new Color(0.3f, 0.75f, 0.95f, 1f),
        new Color(0.6f, 0.85f, 0.35f, 1f),
        new Color(0.85f, 0.5f, 0.85f, 1f),
        new Color(0.45f, 0.8f, 0.75f, 1f),
        new Color(0.95f, 0.6f, 0.45f, 1f),
        new Color(0.6f, 0.6f, 0.95f, 1f)
    };
}
