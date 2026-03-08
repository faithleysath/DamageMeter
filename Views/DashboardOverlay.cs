using DamageMeterRebuilt.Infrastructure;
using Godot;

namespace DamageMeterRebuilt.Views;

internal sealed partial class DashboardOverlay : CanvasLayer
{
    private const int ToggleAutoResetMenuId = 20_000;
    private const int ResetPositionMenuId = 20_001;
    private const float AnchoredOffsetX = -10f;
    private const float AnchoredOffsetY = 10f;

    private readonly PanelContainer _panel;
    private readonly Button _resetButton;
    private readonly Button _dashboardButton;
    private readonly Button _leftButton;
    private readonly Button _titleButton;
    private readonly Button _rightButton;
    private readonly Button _segmentLeftButton;
    private readonly Button _segmentLabelButton;
    private readonly Button _segmentRightButton;
    private readonly ScrollContainer _scrollContainer;
    private readonly VBoxContainer _contentBox;
    private readonly Label _footerLabel;
    private readonly ConfirmationDialog _resetDialog;
    private readonly PopupPanel _categoryPopup;
    private readonly GridContainer _categoryGrid;
    private readonly PopupMenu _segmentMenu;
    private readonly PopupMenu _settingsMenu;
    private readonly PopupMenu _scaleSubmenu;
    private readonly PopupMenu _opacitySubmenu;
    private readonly PopupMenu _maxBarsSubmenu;
    private readonly StyleBoxFlat _panelStyle;
    private bool _dragging;
    private bool _anchored;
    private bool _titleActsAsBack;
    private int _selectedSegmentMenuIndex;
    private DashboardView _selectedView;
    private float _selectedScale;
    private float _selectedOpacity;
    private int _selectedMaxBars;
    private bool _selectedAutoReset;
    private Vector2 _dragOffset;

    public event Action<bool>? CycleViewRequested;
    public event Action? ResetRequested;
    public event Action? DashboardRequested;
    public event Action? TitleRequested;
    public event Action<bool>? SegmentCycleRequested;
    public event Action<string>? BarActivated;
    public event Action<Vector2>? PositionChanged;
    public event Action<DashboardView>? ViewSelected;
    public event Action<int>? SegmentSelected;
    public event Action<float>? ScaleSelected;
    public event Action<float>? OpacitySelected;
    public event Action<int>? MaxBarsSelected;
    public event Action<bool>? AutoResetChanged;
    public event Action? ResetPositionRequested;

    public DashboardOverlay(Vector2 initialPosition, bool useAnchoredPosition)
    {
        Name = "DamageMeterRebuiltOverlay";
        Layer = 50;
        ProcessMode = ProcessModeEnum.Always;
        _anchored = useAnchoredPosition;

        _panel = new PanelContainer
        {
            Name = "Panel",
            CustomMinimumSize = new Vector2(280f, 0f),
            MouseFilter = Control.MouseFilterEnum.Stop
        };

        if (_anchored)
        {
            ApplyAnchoredPosition();
        }
        else
        {
            _panel.Position = initialPosition;
        }

        var root = new VBoxContainer();
        root.AddThemeConstantOverride("separation", 6);
        root.MouseFilter = Control.MouseFilterEnum.Ignore;

        var topRow = new HBoxContainer();
        topRow.AddThemeConstantOverride("separation", 2);
        topRow.MouseFilter = Control.MouseFilterEnum.Ignore;

        _resetButton = CreateNavButton("↺", 14, DimText);
        _resetButton.TooltipText = I18n.ResetData;
        _resetButton.Pressed += ShowResetDialog;
        topRow.AddChild(_resetButton);

        _dashboardButton = CreateNavButton("≡", 14, DimText);
        _dashboardButton.TooltipText = I18n.Dashboard;
        _dashboardButton.Pressed += () => DashboardRequested?.Invoke();
        topRow.AddChild(_dashboardButton);

        _leftButton = CreateNavButton("◀");
        _leftButton.Pressed += () => CycleViewRequested?.Invoke(false);
        topRow.AddChild(_leftButton);

        _titleButton = new Button
        {
            Flat = true,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        _titleButton.AddThemeColorOverride("font_color", GoldColor);
        _titleButton.AddThemeColorOverride("font_hover_color", new Color(1f, 0.95f, 0.5f, 1f));
        _titleButton.AddThemeColorOverride("font_pressed_color", GoldColor);
        _titleButton.AddThemeFontSizeOverride("font_size", 15);
        _titleButton.Pressed += OnTitleButtonPressed;
        _titleButton.GuiInput += OnTitleGuiInput;
        topRow.AddChild(_titleButton);

        _rightButton = CreateNavButton("▶");
        _rightButton.Pressed += () => CycleViewRequested?.Invoke(true);
        topRow.AddChild(_rightButton);

        root.AddChild(topRow);
        root.AddChild(CreateSeparator(0.25f));

        var segmentRow = new HBoxContainer();
        segmentRow.AddThemeConstantOverride("separation", 2);
        segmentRow.MouseFilter = Control.MouseFilterEnum.Ignore;

        _segmentLeftButton = CreateNavButton("◀", 11, SegmentTextColor);
        _segmentLeftButton.Pressed += () => SegmentCycleRequested?.Invoke(false);
        segmentRow.AddChild(_segmentLeftButton);

        _segmentLabelButton = new Button
        {
            Flat = true,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        _segmentLabelButton.AddThemeColorOverride("font_color", SegmentTextColor);
        _segmentLabelButton.AddThemeColorOverride("font_hover_color", new Color(0.85f, 0.85f, 0.95f, 1f));
        _segmentLabelButton.AddThemeColorOverride("font_pressed_color", SegmentTextColor);
        _segmentLabelButton.AddThemeFontSizeOverride("font_size", 12);
        _segmentLabelButton.Pressed += ShowSegmentMenu;
        _segmentLabelButton.GuiInput += OnSegmentLabelGuiInput;
        segmentRow.AddChild(_segmentLabelButton);

        _segmentRightButton = CreateNavButton("▶", 11, SegmentTextColor);
        _segmentRightButton.Pressed += () => SegmentCycleRequested?.Invoke(true);
        segmentRow.AddChild(_segmentRightButton);

        root.AddChild(segmentRow);

        _scrollContainer = new ScrollContainer
        {
            HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled,
            VerticalScrollMode = ScrollContainer.ScrollMode.Auto,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };
        _scrollContainer.AddThemeStyleboxOverride("scroll", new StyleBoxEmpty());

        _contentBox = new VBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        _contentBox.AddThemeConstantOverride("separation", 2);
        _scrollContainer.AddChild(_contentBox);
        root.AddChild(_scrollContainer);

        _footerLabel = new Label
        {
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        _footerLabel.AddThemeColorOverride("font_color", DimText);
        _footerLabel.AddThemeFontSizeOverride("font_size", 10);
        root.AddChild(_footerLabel);

        _panel.AddChild(root);
        _panel.GuiInput += OnPanelGuiInput;
        AddChild(_panel);

        _resetDialog = new ConfirmationDialog
        {
            DialogText = $"{I18n.ResetData}?",
            Title = I18n.ResetData
        };
        _resetDialog.Confirmed += () => ResetRequested?.Invoke();
        AddChild(_resetDialog);

        _categoryPopup = new PopupPanel
        {
            Name = "CategoryPopup",
            Transparent = true
        };
        _categoryPopup.TransparentBg = true;
        _categoryPopup.AddThemeStyleboxOverride("panel", CreateCategoryPopupStyle());
        _categoryGrid = new GridContainer
        {
            Columns = 3
        };
        _categoryGrid.AddThemeConstantOverride("h_separation", 4);
        _categoryGrid.AddThemeConstantOverride("v_separation", 4);
        _categoryPopup.AddChild(_categoryGrid);
        AddChild(_categoryPopup);

        _segmentMenu = CreateStyledPopup("SegmentMenu");
        _segmentMenu.IdPressed += OnSegmentMenuPressed;
        AddChild(_segmentMenu);

        _settingsMenu = CreateStyledPopup("SettingsMenu");
        _settingsMenu.IdPressed += OnSettingsMenuPressed;
        AddChild(_settingsMenu);

        _scaleSubmenu = CreateStyledPopup("ScaleMenu");
        _scaleSubmenu.IdPressed += OnSettingsMenuPressed;
        _settingsMenu.AddChild(_scaleSubmenu);

        _opacitySubmenu = CreateStyledPopup("OpacityMenu");
        _opacitySubmenu.IdPressed += OnSettingsMenuPressed;
        _settingsMenu.AddChild(_opacitySubmenu);

        _maxBarsSubmenu = CreateStyledPopup("MaxBarsMenu");
        _maxBarsSubmenu.IdPressed += OnSettingsMenuPressed;
        _settingsMenu.AddChild(_maxBarsSubmenu);

        _panelStyle = new StyleBoxFlat
        {
            BgColor = new Color(0.05f, 0.05f, 0.12f, 0.95f),
            BorderColor = GoldBorder,
            BorderWidthLeft = 1,
            BorderWidthTop = 1,
            BorderWidthRight = 1,
            BorderWidthBottom = 1,
            CornerRadiusTopLeft = 8,
            CornerRadiusTopRight = 8,
            CornerRadiusBottomLeft = 8,
            CornerRadiusBottomRight = 8,
            ContentMarginLeft = 10f,
            ContentMarginRight = 10f,
            ContentMarginTop = 8f,
            ContentMarginBottom = 8f
        };
        ApplyPanelTheme();
    }

    public void SetView(DashboardBarView view, string footer, int maxBars)
    {
        _titleButton.Text = view.Title;
        _titleActsAsBack = view.TitleClickable;
        _titleButton.MouseDefaultCursorShape = view.TitleClickable
            ? Control.CursorShape.PointingHand
            : Control.CursorShape.Arrow;
        _segmentLabelButton.Text = view.SegmentLabel;
        _footerLabel.Text = footer;
        _footerLabel.Visible = !string.IsNullOrWhiteSpace(footer);

        ClearBars();

        if (view.Bars.Count == 0)
        {
            AddNoDataLabel();
            UpdateScrollHeight(1);
            return;
        }

        var clampedBars = view.Bars.Take(maxBars).ToList();
        var maxValue = clampedBars.Max(bar => bar.Value);
        var totalValue = clampedBars.Sum(bar => bar.Value);

        for (var index = 0; index < clampedBars.Count; index++)
        {
            var bar = clampedBars[index];
            var color = DetailColors[index % DetailColors.Length];
            _contentBox.AddChild(CreateBarNode(bar, color, maxValue, totalValue));
        }

        UpdateScrollHeight(clampedBars.Count);
    }

    public void ApplyStyle(float scale, float opacity)
    {
        var safeScale = Mathf.Clamp(scale, 0.5f, 2.0f);
        var safeOpacity = Mathf.Clamp(opacity, 0.3f, 1.0f);
        _panel.Scale = new Vector2(safeScale, safeScale);
        _panelStyle.BgColor = new Color(_panelStyle.BgColor.R, _panelStyle.BgColor.G, _panelStyle.BgColor.B, safeOpacity);
        _panel.AddThemeStyleboxOverride("panel", _panelStyle);
    }

    public void SetDashboardAvailable(bool enabled)
    {
        _dashboardButton.Disabled = !enabled;
        _dashboardButton.Modulate = enabled ? Colors.White : new Color(1f, 1f, 1f, 0.4f);
    }

    public void ConfigureMenus(
        DashboardView currentView,
        IReadOnlyList<string> segmentLabels,
        int selectedSegmentIndex,
        float scale,
        float opacity,
        int maxBars,
        bool autoReset)
    {
        _selectedView = currentView;
        _selectedSegmentMenuIndex = Mathf.Clamp(selectedSegmentIndex, 0, Math.Max(segmentLabels.Count - 1, 0));
        _selectedScale = scale;
        _selectedOpacity = opacity;
        _selectedMaxBars = maxBars;
        _selectedAutoReset = autoReset;
        RebuildSegmentMenu(segmentLabels);
        RebuildSettingsMenu();
    }

    public void ShowOverlay()
    {
        _panel.Visible = true;
    }

    public void HideOverlay()
    {
        _panel.Visible = false;
    }

    private void OnPanelGuiInput(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouseButton { ButtonIndex: MouseButton.Right, Pressed: true }:
                ShowCategoryMenu();
                GetViewport()?.SetInputAsHandled();
                break;
            case InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true }:
                StartDragging();
                _dragging = true;
                if (@event is InputEventMouseButton mouseButton)
                {
                    _dragOffset = _panel.Position - mouseButton.GlobalPosition;
                }
                GetViewport()?.SetInputAsHandled();
                break;
            case InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: false }:
                _dragging = false;
                PositionChanged?.Invoke(_panel.Position);
                break;
            case InputEventMouseMotion motion when _dragging:
                _panel.Position = motion.GlobalPosition + _dragOffset;
                GetViewport()?.SetInputAsHandled();
                break;
        }
    }

    private void ShowResetDialog()
    {
        _resetDialog.PopupCentered(new Vector2I(320, 0));
    }

    private void OnTitleButtonPressed()
    {
        if (_titleActsAsBack)
        {
            TitleRequested?.Invoke();
            return;
        }

        ShowCategoryMenu();
    }

    private void OnTitleGuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton { ButtonIndex: MouseButton.Right, Pressed: true })
        {
            return;
        }

        ShowCategoryMenu();
        GetViewport()?.SetInputAsHandled();
    }

    private void OnSegmentLabelGuiInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton { ButtonIndex: MouseButton.Right, Pressed: true })
        {
            return;
        }

        ShowSegmentMenu();
        GetViewport()?.SetInputAsHandled();
    }

    private void ShowCategoryMenu()
    {
        RebuildCategoryMenu();
        ShowPopupAtCursor(_categoryPopup);
    }

    private void ShowSegmentMenu()
    {
        ShowPopupAtCursor(_segmentMenu);
    }

    private void RebuildCategoryMenu()
    {
        foreach (var child in _categoryGrid.GetChildren())
        {
            child.QueueFree();
        }

        foreach (var view in Enum.GetValues<DashboardView>())
        {
            var category = view;
            var button = new Button
            {
                Text = GetCategoryLabel(view),
                Flat = true,
                CustomMinimumSize = new Vector2(100f, 28f)
            };
            button.AddThemeFontSizeOverride("font_size", 13);
            if (view == _selectedView)
            {
                button.AddThemeColorOverride("font_color", GoldColor);
                button.AddThemeColorOverride("font_hover_color", GoldColor);
            }
            else
            {
                button.AddThemeColorOverride("font_color", TextColor);
                button.AddThemeColorOverride("font_hover_color", GoldColor);
            }

            button.Pressed += () =>
            {
                _categoryPopup.Hide();
                ViewSelected?.Invoke(category);
            };
            _categoryGrid.AddChild(button);
        }

        var settingsButton = new Button
        {
            Text = "⚙ " + I18n.Settings,
            Flat = true,
            CustomMinimumSize = new Vector2(100f, 28f)
        };
        settingsButton.AddThemeFontSizeOverride("font_size", 13);
        settingsButton.AddThemeColorOverride("font_color", DimText);
        settingsButton.AddThemeColorOverride("font_hover_color", GoldColor);
        settingsButton.Pressed += () =>
        {
            _categoryPopup.Hide();
            ShowSettingsMenu();
        };
        _categoryGrid.AddChild(settingsButton);
    }

    private void RebuildSegmentMenu(IReadOnlyList<string> segmentLabels)
    {
        _segmentMenu.Clear();
        for (var index = 0; index < segmentLabels.Count; index++)
        {
            _segmentMenu.AddRadioCheckItem(segmentLabels[index], index);
            _segmentMenu.SetItemChecked(_segmentMenu.ItemCount - 1, index == _selectedSegmentMenuIndex);
        }
    }

    private void RebuildSettingsMenu()
    {
        _scaleSubmenu.Clear();
        for (var index = 0; index < ScaleOptions.Length; index++)
        {
            var option = ScaleOptions[index];
            _scaleSubmenu.AddRadioCheckItem($"{Mathf.RoundToInt(option * 100f)}%", GetScaleMenuId(option));
            _scaleSubmenu.SetItemChecked(index, Math.Abs(option - _selectedScale) < 0.01f);
        }

        _opacitySubmenu.Clear();
        for (var index = 0; index < OpacityOptions.Length; index++)
        {
            var option = OpacityOptions[index];
            _opacitySubmenu.AddRadioCheckItem($"{Mathf.RoundToInt(option * 100f)}%", GetOpacityMenuId(option));
            _opacitySubmenu.SetItemChecked(index, Math.Abs(option - _selectedOpacity) < 0.01f);
        }

        _maxBarsSubmenu.Clear();
        for (var index = 0; index < MaxBarsOptions.Length; index++)
        {
            var option = MaxBarsOptions[index];
            _maxBarsSubmenu.AddRadioCheckItem(option.ToString(), GetMaxBarsMenuId(option));
            _maxBarsSubmenu.SetItemChecked(index, option == _selectedMaxBars);
        }

        _settingsMenu.Clear();
        _settingsMenu.AddSubmenuNodeItem(I18n.SettingsScale, _scaleSubmenu, -1);
        _settingsMenu.AddSubmenuNodeItem(I18n.SettingsOpacity, _opacitySubmenu, -1);
        _settingsMenu.AddSubmenuNodeItem(I18n.SettingsMaxBars, _maxBarsSubmenu, -1);
        _settingsMenu.AddSeparator();
        _settingsMenu.AddCheckItem(I18n.SettingsAutoReset, ToggleAutoResetMenuId);
        _settingsMenu.SetItemChecked(_settingsMenu.GetItemIndex(ToggleAutoResetMenuId), _selectedAutoReset);
        _settingsMenu.AddSeparator();
        _settingsMenu.AddItem(I18n.SettingsResetPos, ResetPositionMenuId);
    }

    private void ShowSettingsMenu()
    {
        RebuildSettingsMenu();
        ShowPopupAtCursor(_settingsMenu);
    }

    private void OnSegmentMenuPressed(long id)
    {
        SegmentSelected?.Invoke((int)id);
    }

    private void OnSettingsMenuPressed(long id)
    {
        if (id == ToggleAutoResetMenuId)
        {
            var itemIndex = _settingsMenu.GetItemIndex((int)id);
            var nextValue = !_settingsMenu.IsItemChecked(itemIndex);
            AutoResetChanged?.Invoke(nextValue);
            return;
        }

        if (id == ResetPositionMenuId)
        {
            ResetPositionRequested?.Invoke();
            return;
        }

        if (TryDecodeScaleMenuId((int)id, out var scale))
        {
            ScaleSelected?.Invoke(scale);
            return;
        }

        if (TryDecodeOpacityMenuId((int)id, out var opacity))
        {
            OpacitySelected?.Invoke(opacity);
            return;
        }

        if (TryDecodeMaxBarsMenuId((int)id, out var maxBars))
        {
            MaxBarsSelected?.Invoke(maxBars);
        }
    }

    private static PopupMenu CreateStyledPopup(string name)
    {
        var popup = new PopupMenu
        {
            Name = name
        };
        popup.AddThemeStyleboxOverride("panel", CreateCategoryPopupStyle());
        return popup;
    }

    private static void ShowPopupAtCursor(Window popup)
    {
        popup.Position = DisplayServer.MouseGetPosition();
        popup.ResetSize();
        popup.Popup();
    }

    private static StyleBoxFlat CreateCategoryPopupStyle()
    {
        return new StyleBoxFlat
        {
            BgColor = new Color(0.08f, 0.08f, 0.15f, 0.95f),
            BorderWidthLeft = 1,
            BorderWidthTop = 1,
            BorderWidthRight = 1,
            BorderWidthBottom = 1,
            BorderColor = GoldBorder,
            CornerRadiusTopLeft = 6,
            CornerRadiusTopRight = 6,
            CornerRadiusBottomLeft = 6,
            CornerRadiusBottomRight = 6,
            ContentMarginLeft = 8f,
            ContentMarginRight = 8f,
            ContentMarginTop = 8f,
            ContentMarginBottom = 8f
        };
    }

    private void ApplyPanelTheme()
    {
        _panel.AddThemeStyleboxOverride("panel", _panelStyle);
    }

    private void ApplyAnchoredPosition()
    {
        _panel.AnchorLeft = 1f;
        _panel.AnchorRight = 1f;
        _panel.AnchorTop = 0f;
        _panel.AnchorBottom = 0f;
        _panel.GrowHorizontal = Control.GrowDirection.Begin;
        _panel.GrowVertical = Control.GrowDirection.End;
        _panel.OffsetLeft = AnchoredOffsetX;
        _panel.OffsetTop = AnchoredOffsetY;
        _panel.OffsetRight = AnchoredOffsetX;
        _panel.OffsetBottom = AnchoredOffsetY;
    }

    private void StartDragging()
    {
        if (!_anchored)
        {
            return;
        }

        var globalPosition = _panel.GlobalPosition;
        _panel.AnchorLeft = 0f;
        _panel.AnchorRight = 0f;
        _panel.AnchorTop = 0f;
        _panel.AnchorBottom = 0f;
        _panel.OffsetLeft = 0f;
        _panel.OffsetTop = 0f;
        _panel.OffsetRight = 0f;
        _panel.OffsetBottom = 0f;
        _panel.Position = globalPosition;
        _anchored = false;
    }

    private static Button CreateNavButton(string text, int fontSize = 13, Color? color = null)
    {
        var button = new Button
        {
            Text = text,
            Flat = true,
            CustomMinimumSize = new Vector2(24f, 24f)
        };
        button.AddThemeFontSizeOverride("font_size", fontSize);
        button.AddThemeColorOverride("font_color", color ?? GoldColor);
        button.AddThemeColorOverride("font_hover_color", GoldColor);
        button.AddThemeColorOverride("font_pressed_color", color ?? GoldColor);
        return button;
    }

    private static HSeparator CreateSeparator(float alpha)
    {
        var separator = new HSeparator();
        var style = new StyleBoxFlat
        {
            BgColor = new Color(1f, 0.84f, 0f, alpha)
        };
        separator.AddThemeStyleboxOverride("separator", style);
        separator.AddThemeConstantOverride("separation", 4);
        return separator;
    }

    private void ClearBars()
    {
        foreach (var child in _contentBox.GetChildren())
        {
            child.QueueFree();
        }
    }

    private void AddNoDataLabel()
    {
        var label = new Label
        {
            Text = I18n.WaitingForCombat,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        label.AddThemeColorOverride("font_color", DimText);
        label.AddThemeFontSizeOverride("font_size", 13);
        label.MouseFilter = Control.MouseFilterEnum.Ignore;
        _contentBox.AddChild(label);
    }

    private void UpdateScrollHeight(int barCount)
    {
        var desired = barCount * 24f;
        _scrollContainer.CustomMinimumSize = new Vector2(0f, Math.Min(desired, 300f));
    }

    private Panel CreateBarNode(DashboardBarItem data, Color color, int maxValue, int totalValue)
    {
        var resolvedColor = data.ColorOverride ?? color;
        var fillRatio = maxValue > 0 ? Mathf.Clamp((float)data.Value / maxValue, 0f, 1f) : 0f;
        var percent = totalValue > 0 ? (float)data.Value / totalValue * 100f : 0f;

        var panel = new Panel
        {
            CustomMinimumSize = new Vector2(0f, 22f),
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            MouseFilter = data.Clickable ? Control.MouseFilterEnum.Stop : Control.MouseFilterEnum.Ignore
        };

        var panelStyle = new StyleBoxFlat
        {
            BgColor = BarBgColor,
            CornerRadiusTopLeft = 3,
            CornerRadiusTopRight = 3,
            CornerRadiusBottomLeft = 3,
            CornerRadiusBottomRight = 3
        };
        panel.AddThemeStyleboxOverride("panel", panelStyle);

        var fill = new ColorRect
        {
            Color = new Color(resolvedColor.R, resolvedColor.G, resolvedColor.B, 0.35f),
            AnchorRight = fillRatio,
            AnchorBottom = 1f,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        panel.AddChild(fill);

        var labelOffset = 0f;
        if (data.Icon is not null)
        {
            var icon = new TextureRect
            {
                Texture = data.Icon,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                AnchorBottom = 1f,
                OffsetLeft = 3f,
                OffsetTop = 2f,
                OffsetRight = 21f,
                OffsetBottom = -2f,
                MouseFilter = Control.MouseFilterEnum.Ignore
            };
            panel.AddChild(icon);
            labelOffset = 22f;
        }

        var label = new Label
        {
            Text = data.Icon is not null ? " " + data.Label : "  " + data.Label,
            AnchorRight = 0.55f,
            AnchorBottom = 1f,
            OffsetLeft = labelOffset,
            VerticalAlignment = VerticalAlignment.Center,
            ClipText = true,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        label.AddThemeFontSizeOverride("font_size", 12);
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
        valueLabel.AddThemeFontSizeOverride("font_size", 12);
        valueLabel.AddThemeColorOverride("font_color", TextColor);
        panel.AddChild(valueLabel);

        if (data.Clickable)
        {
            panel.MouseDefaultCursorShape = Control.CursorShape.PointingHand;
            panel.GuiInput += @event =>
            {
                if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
                {
                    return;
                }

                BarActivated?.Invoke(data.Key);
                GetViewport()?.SetInputAsHandled();
            };
        }

        return panel;
    }

    private static readonly Color GoldColor = new(1f, 0.84f, 0f, 1f);
    private static readonly Color GoldBorder = new(1f, 0.84f, 0f, 0.3f);
    private static readonly Color TextColor = new(0.85f, 0.85f, 0.85f, 1f);
    private static readonly Color DimText = new(0.5f, 0.5f, 0.5f, 1f);
    private static readonly Color SegmentTextColor = new(0.7f, 0.7f, 0.8f, 1f);
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

    private static readonly float[] ScaleOptions = { 0.75f, 0.9f, 1f, 1.15f, 1.3f, 1.5f };
    private static readonly float[] OpacityOptions = { 0.45f, 0.6f, 0.75f, 0.88f, 1f };
    private static readonly int[] MaxBarsOptions = { 5, 8, 10, 12, 15, 20 };

    private static string GetCategoryLabel(DashboardView view)
    {
        return view switch
        {
            DashboardView.DamageDealt => I18n.CatDamageDealt,
            DashboardView.DamageTaken => I18n.CatDamageTaken,
            DashboardView.Dpt => I18n.CatDpt,
            DashboardView.CardUsage => I18n.CatCardUsage,
            DashboardView.Block => I18n.CatBlock,
            DashboardView.Energy => I18n.CatEnergy,
            DashboardView.Efficiency => I18n.CatEfficiency,
            DashboardView.Overkill => I18n.CatOverkill,
            DashboardView.Potions => I18n.CatPotions,
            DashboardView.Debuffs => I18n.CatDebuffs,
            DashboardView.DeathLog => I18n.CatDeathLog,
            DashboardView.CardFlow => I18n.CatCardFlow,
            DashboardView.CombatLog => I18n.CatCombatLog,
            DashboardView.Records => I18n.CatRecords,
            DashboardView.Orbs => I18n.CatOrbs,
            DashboardView.Threat => I18n.CatThreat,
            DashboardView.Advanced => I18n.CatAdvanced,
            _ => view.ToString()
        };
    }

    private static int GetScaleMenuId(float scale)
    {
        return 30_000 + Mathf.RoundToInt(scale * 100f);
    }

    private static int GetOpacityMenuId(float opacity)
    {
        return 40_000 + Mathf.RoundToInt(opacity * 100f);
    }

    private static int GetMaxBarsMenuId(int maxBars)
    {
        return 50_000 + maxBars;
    }

    private static bool TryDecodeScaleMenuId(int id, out float scale)
    {
        if (id is < 30_000 or >= 40_000)
        {
            scale = 0f;
            return false;
        }

        scale = (id - 30_000) / 100f;
        return true;
    }

    private static bool TryDecodeOpacityMenuId(int id, out float opacity)
    {
        if (id is < 40_000 or >= 50_000)
        {
            opacity = 0f;
            return false;
        }

        opacity = (id - 40_000) / 100f;
        return true;
    }

    private static bool TryDecodeMaxBarsMenuId(int id, out int maxBars)
    {
        if (id < 50_000)
        {
            maxBars = 0;
            return false;
        }

        maxBars = id - 50_000;
        return true;
    }
}
