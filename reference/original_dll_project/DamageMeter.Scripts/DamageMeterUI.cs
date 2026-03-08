using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DamageMeter.Scripts.Categories;
using Godot;

namespace DamageMeter.Scripts;

public static class DamageMeterUI
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static Action _003C0_003E__OnResetPressed;

		public static Action _003C1_003E__OnDashboardPressed;

		public static Action _003C2_003E__OnLeftPressed;

		public static Action _003C3_003E__OnTitlePressed;

		public static GuiInputEventHandler _003C4_003E__OnTitleGuiInput;

		public static Action _003C5_003E__OnRightPressed;

		public static Action _003C6_003E__OnSegmentLeftPressed;

		public static GuiInputEventHandler _003C7_003E__OnSegmentLabelGuiInput;

		public static Action _003C8_003E__OnSegmentRightPressed;

		public static GuiInputEventHandler _003C9_003E__OnPanelGuiInput;

		public static Action _003C10_003E__UpdateDisplay;

		public static Action _003C11_003E__OnLanguageChanged;

		public static IdPressedEventHandler _003C12_003E__OnSegmentSelected;

		public static IdPressedEventHandler _003C13_003E__OnSettingsAction;

		public static IdPressedEventHandler _003C14_003E__OnScaleSelected;

		public static IdPressedEventHandler _003C15_003E__OnOpacitySelected;

		public static IdPressedEventHandler _003C16_003E__OnMaxBarsSelected;

		public static Action _003C17_003E__OnResetConfirmed;

		public static GuiInputEventHandler _003C18_003E__OnDashboardBgInput;

		public static Action _003C19_003E__RemoveDashboard;
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Action _003C_003E9__56_0;

		public static Func<BarData, int> _003C_003E9__70_0;

		public static Func<BarData, int> _003C_003E9__70_1;

		public static Func<BarData, int> _003C_003E9__70_2;

		public static Func<BarData, int> _003C_003E9__70_3;

		public static Func<BarData, int> _003C_003E9__79_1;

		public static Func<BarData, int> _003C_003E9__79_2;

		internal void _003CShowCategoryMenu_003Eb__56_0()
		{
			PopupPanel? categoryPopup = _categoryPopup;
			if (categoryPopup != null)
			{
				((Window)categoryPopup).Hide();
			}
			ShowSettingsMenu();
		}

		internal int _003CUpdateDisplay_003Eb__70_0(BarData b)
		{
			return b.Value;
		}

		internal int _003CUpdateDisplay_003Eb__70_1(BarData b)
		{
			return b.Value;
		}

		internal int _003CUpdateDisplay_003Eb__70_2(BarData b)
		{
			return b.Value;
		}

		internal int _003CUpdateDisplay_003Eb__70_3(BarData b)
		{
			return b.Value;
		}

		internal int _003CCreateDashboardMiniPanel_003Eb__79_1(BarData b)
		{
			return b.Value;
		}

		internal int _003CCreateDashboardMiniPanel_003Eb__79_2(BarData b)
		{
			return b.Value;
		}
	}

	private static readonly Color[] DetailColors = (Color[])(object)new Color[8]
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

	private static readonly Color GoldColor = new Color(1f, 0.84f, 0f, 1f);

	private static readonly Color GoldBorder = new Color(1f, 0.84f, 0f, 0.3f);

	private static readonly Color TextColor = new Color(0.85f, 0.85f, 0.85f, 1f);

	private static readonly Color DimText = new Color(0.5f, 0.5f, 0.5f, 1f);

	private static readonly Color SegmentTextColor = new Color(0.7f, 0.7f, 0.8f, 1f);

	private static readonly Color BarBgColor = new Color(0.06f, 0.06f, 0.12f, 0.95f);

	private static readonly Dictionary<string, string> CharacterIconMap = new Dictionary<string, string>((IEqualityComparer<string>)(object)StringComparer.OrdinalIgnoreCase)
	{
		["IRONCLAD"] = "铁甲战士",
		["SILENT"] = "静默猎手",
		["DEFECT"] = "故障机器人",
		["NECROBINDER"] = "亡灵契约师",
		["REGENT"] = "储君"
	};

	private static readonly Dictionary<string, Texture2D?> _iconCache = new Dictionary<string, Texture2D>();

	private static readonly List<IStatCategory> _categories = new List<IStatCategory>();

	private static int _categoryIndex;

	private static string? _detailPlayerKey;

	private static readonly List<string> _playerColorOrder = new List<string>();

	private static CanvasLayer? _canvas;

	private static PanelContainer? _panel;

	private static Button? _titleBtn;

	private static Button? _resetBtn;

	private static Button? _leftBtn;

	private static Button? _rightBtn;

	private static HBoxContainer? _segmentRow;

	private static Button? _segmentLabel;

	private static VBoxContainer? _contentBox;

	private static InputHandler? _inputHandler;

	private static ScrollContainer? _scrollContainer;

	private static Button? _dashboardBtn;

	private static CanvasLayer? _dashboardCanvas;

	private static bool _dashboardVisible;

	private static readonly Dictionary<int, string?> _dashboardDetailState = new Dictionary<int, string>();

	private static PopupPanel? _categoryPopup;

	private static GridContainer? _categoryGrid;

	private static PopupMenu? _segmentPopup;

	private static PopupMenu? _settingsPopup;

	private static PopupMenu? _scaleSubmenu;

	private static PopupMenu? _opacitySubmenu;

	private static PopupMenu? _maxBarsSubmenu;

	private static StyleBoxFlat? _panelStyle;

	private static ConfirmationDialog? _resetConfirmDialog;

	private static bool _isDragging;

	private static Vector2 _dragOffset;

	private static bool _anchored = true;

	private static readonly float[] ScaleOptions;

	private static readonly float[] OpacityOptions;

	private static readonly int[] MaxBarsOptions;

	public static bool IsDashboardVisible => _dashboardVisible;

	public static void Initialize()
	{
		_categories.Clear();
		_categories.Add((IStatCategory)new DamageDealtCategory());
		_categories.Add((IStatCategory)new DamageTakenCategory());
		_categories.Add((IStatCategory)new DptCategory());
		_categories.Add((IStatCategory)new CardUsageCategory());
		_categories.Add((IStatCategory)new BlockCategory());
		_categories.Add((IStatCategory)new EnergyCategory());
		_categories.Add((IStatCategory)new CardEfficiencyCategory());
		_categories.Add((IStatCategory)new OverkillCategory());
		_categories.Add((IStatCategory)new PotionCategory());
		_categories.Add((IStatCategory)new DebuffsCategory());
		_categories.Add((IStatCategory)new CardFlowCategory());
		_categories.Add((IStatCategory)new DeathLogCategory());
		_categories.Add((IStatCategory)new CombatLogCategory());
		_categories.Add((IStatCategory)new RecordsCategory());
	}

	public static void CreateAndAttach()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Expected O, but got Unknown
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Expected O, but got Unknown
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Expected O, but got Unknown
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Expected O, but got Unknown
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Expected O, but got Unknown
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Expected O, but got Unknown
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Expected O, but got Unknown
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Expected O, but got Unknown
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Expected O, but got Unknown
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Expected O, but got Unknown
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Expected O, but got Unknown
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Expected O, but got Unknown
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Expected O, but got Unknown
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Expected O, but got Unknown
		//IL_0691: Unknown result type (might be due to invalid IL or missing references)
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Expected O, but got Unknown
		//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Expected O, but got Unknown
		//IL_0717: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Expected O, but got Unknown
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Expected O, but got Unknown
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Expected O, but got Unknown
		//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e7: Expected O, but got Unknown
		//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0802: Unknown result type (might be due to invalid IL or missing references)
		//IL_0808: Expected O, but got Unknown
		//IL_082f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0839: Expected O, but got Unknown
		//IL_0851: Unknown result type (might be due to invalid IL or missing references)
		//IL_0858: Expected O, but got Unknown
		//IL_086e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0931: Unknown result type (might be due to invalid IL or missing references)
		//IL_093b: Expected O, but got Unknown
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0823: Unknown result type (might be due to invalid IL or missing references)
		//IL_0829: Expected O, but got Unknown
		//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b0: Expected O, but got Unknown
		//IL_09fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a06: Expected O, but got Unknown
		//IL_0a40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4b: Expected O, but got Unknown
		//IL_0a98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa3: Expected O, but got Unknown
		//IL_0b27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b31: Expected O, but got Unknown
		//IL_0af0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afb: Expected O, but got Unknown
		//IL_0baf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8b: Expected O, but got Unknown
		//IL_0c0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c16: Unknown result type (might be due to invalid IL or missing references)
		if (_canvas == null)
		{
			_categoryIndex = 0;
			_detailPlayerKey = null;
			_playerColorOrder.Clear();
			_canvas = new CanvasLayer
			{
				Layer = 100,
				Name = StringName.op_Implicit("DamageMeterUI")
			};
			_panel = new PanelContainer();
			((Control)_panel).CustomMinimumSize = new Vector2(280f, 0f);
			if (DamageMeterSettings.HasSavedPosition)
			{
				((Control)_panel).Position = new Vector2(DamageMeterSettings.PanelX, DamageMeterSettings.PanelY);
				_anchored = false;
			}
			else
			{
				((Control)_panel).AnchorLeft = 1f;
				((Control)_panel).AnchorRight = 1f;
				((Control)_panel).GrowHorizontal = (GrowDirection)0;
				((Control)_panel).GrowVertical = (GrowDirection)1;
				((Control)_panel).OffsetLeft = -10f;
				((Control)_panel).OffsetTop = 10f;
			}
			_panelStyle = new StyleBoxFlat();
			_panelStyle.BgColor = new Color(0.05f, 0.05f, 0.12f, DamageMeterSettings.Opacity);
			StyleBoxFlat? panelStyle = _panelStyle;
			StyleBoxFlat? panelStyle2 = _panelStyle;
			StyleBoxFlat? panelStyle3 = _panelStyle;
			int num = (_panelStyle.BorderWidthRight = 1);
			int num3 = (panelStyle3.BorderWidthLeft = num);
			int borderWidthBottom = (panelStyle2.BorderWidthTop = num3);
			panelStyle.BorderWidthBottom = borderWidthBottom;
			_panelStyle.BorderColor = GoldBorder;
			StyleBoxFlat? panelStyle4 = _panelStyle;
			StyleBoxFlat? panelStyle5 = _panelStyle;
			StyleBoxFlat? panelStyle6 = _panelStyle;
			num = (_panelStyle.CornerRadiusBottomRight = 8);
			num3 = (panelStyle6.CornerRadiusBottomLeft = num);
			borderWidthBottom = (panelStyle5.CornerRadiusTopRight = num3);
			panelStyle4.CornerRadiusTopLeft = borderWidthBottom;
			StyleBoxFlat? panelStyle7 = _panelStyle;
			float contentMarginLeft = (((StyleBox)_panelStyle).ContentMarginRight = 10f);
			((StyleBox)panelStyle7).ContentMarginLeft = contentMarginLeft;
			StyleBoxFlat? panelStyle8 = _panelStyle;
			contentMarginLeft = (((StyleBox)_panelStyle).ContentMarginBottom = 8f);
			((StyleBox)panelStyle8).ContentMarginTop = contentMarginLeft;
			((Control)_panel).AddThemeStyleboxOverride(StringName.op_Implicit("panel"), (StyleBox)(object)_panelStyle);
			((Control)_panel).Scale = new Vector2(DamageMeterSettings.Scale, DamageMeterSettings.Scale);
			VBoxContainer val = new VBoxContainer();
			((Control)val).AddThemeConstantOverride(StringName.op_Implicit("separation"), 6);
			((Control)val).MouseFilter = (MouseFilterEnum)2;
			HBoxContainer val2 = new HBoxContainer();
			((Control)val2).AddThemeConstantOverride(StringName.op_Implicit("separation"), 2);
			((Control)val2).MouseFilter = (MouseFilterEnum)2;
			_resetBtn = CreateNavButton("↺", 14, DimText);
			Button? resetBtn = _resetBtn;
			object obj = _003C_003EO._003C0_003E__OnResetPressed;
			if (obj == null)
			{
				Action val3 = OnResetPressed;
				_003C_003EO._003C0_003E__OnResetPressed = val3;
				obj = (object)val3;
			}
			((BaseButton)resetBtn).Pressed += (Action)obj;
			((Control)_resetBtn).TooltipText = I18n.ResetData;
			((Node)val2).AddChild((Node)(object)_resetBtn, false, (InternalMode)0);
			_dashboardBtn = CreateNavButton("≡", 14, DimText);
			Button? dashboardBtn = _dashboardBtn;
			object obj2 = _003C_003EO._003C1_003E__OnDashboardPressed;
			if (obj2 == null)
			{
				Action val4 = OnDashboardPressed;
				_003C_003EO._003C1_003E__OnDashboardPressed = val4;
				obj2 = (object)val4;
			}
			((BaseButton)dashboardBtn).Pressed += (Action)obj2;
			((Control)_dashboardBtn).TooltipText = I18n.Dashboard;
			((Node)val2).AddChild((Node)(object)_dashboardBtn, false, (InternalMode)0);
			_leftBtn = CreateNavButton("◀");
			Button? leftBtn = _leftBtn;
			object obj3 = _003C_003EO._003C2_003E__OnLeftPressed;
			if (obj3 == null)
			{
				Action val5 = OnLeftPressed;
				_003C_003EO._003C2_003E__OnLeftPressed = val5;
				obj3 = (object)val5;
			}
			((BaseButton)leftBtn).Pressed += (Action)obj3;
			((Node)val2).AddChild((Node)(object)_leftBtn, false, (InternalMode)0);
			_titleBtn = new Button();
			_titleBtn.Flat = true;
			((Control)_titleBtn).SizeFlagsHorizontal = (SizeFlags)3;
			((Control)_titleBtn).AddThemeColorOverride(StringName.op_Implicit("font_color"), GoldColor);
			((Control)_titleBtn).AddThemeColorOverride(StringName.op_Implicit("font_hover_color"), new Color(1f, 0.95f, 0.5f, 1f));
			((Control)_titleBtn).AddThemeColorOverride(StringName.op_Implicit("font_pressed_color"), GoldColor);
			((Control)_titleBtn).AddThemeFontSizeOverride(StringName.op_Implicit("font_size"), 15);
			Button? titleBtn = _titleBtn;
			object obj4 = _003C_003EO._003C3_003E__OnTitlePressed;
			if (obj4 == null)
			{
				Action val6 = OnTitlePressed;
				_003C_003EO._003C3_003E__OnTitlePressed = val6;
				obj4 = (object)val6;
			}
			((BaseButton)titleBtn).Pressed += (Action)obj4;
			Button? titleBtn2 = _titleBtn;
			object obj5 = _003C_003EO._003C4_003E__OnTitleGuiInput;
			if (obj5 == null)
			{
				GuiInputEventHandler val7 = OnTitleGuiInput;
				_003C_003EO._003C4_003E__OnTitleGuiInput = val7;
				obj5 = (object)val7;
			}
			((Control)titleBtn2).GuiInput += (GuiInputEventHandler)obj5;
			((Node)val2).AddChild((Node)(object)_titleBtn, false, (InternalMode)0);
			_rightBtn = CreateNavButton("▶");
			Button? rightBtn = _rightBtn;
			object obj6 = _003C_003EO._003C5_003E__OnRightPressed;
			if (obj6 == null)
			{
				Action val8 = OnRightPressed;
				_003C_003EO._003C5_003E__OnRightPressed = val8;
				obj6 = (object)val8;
			}
			((BaseButton)rightBtn).Pressed += (Action)obj6;
			((Node)val2).AddChild((Node)(object)_rightBtn, false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)val2, false, (InternalMode)0);
			HSeparator val9 = new HSeparator();
			StyleBoxFlat val10 = new StyleBoxFlat
			{
				BgColor = new Color(1f, 0.84f, 0f, 0.25f)
			};
			((Control)val9).AddThemeStyleboxOverride(StringName.op_Implicit("separator"), (StyleBox)(object)val10);
			((Control)val9).AddThemeConstantOverride(StringName.op_Implicit("separation"), 4);
			((Control)val9).MouseFilter = (MouseFilterEnum)2;
			((Node)val).AddChild((Node)(object)val9, false, (InternalMode)0);
			_segmentRow = new HBoxContainer();
			((Control)_segmentRow).AddThemeConstantOverride(StringName.op_Implicit("separation"), 2);
			((Control)_segmentRow).MouseFilter = (MouseFilterEnum)2;
			Button val11 = CreateNavButton("◀", 11, SegmentTextColor);
			object obj7 = _003C_003EO._003C6_003E__OnSegmentLeftPressed;
			if (obj7 == null)
			{
				Action val12 = OnSegmentLeftPressed;
				_003C_003EO._003C6_003E__OnSegmentLeftPressed = val12;
				obj7 = (object)val12;
			}
			((BaseButton)val11).Pressed += (Action)obj7;
			((Node)_segmentRow).AddChild((Node)(object)val11, false, (InternalMode)0);
			_segmentLabel = new Button();
			_segmentLabel.Flat = true;
			((Control)_segmentLabel).SizeFlagsHorizontal = (SizeFlags)3;
			((Control)_segmentLabel).AddThemeColorOverride(StringName.op_Implicit("font_color"), SegmentTextColor);
			((Control)_segmentLabel).AddThemeColorOverride(StringName.op_Implicit("font_hover_color"), new Color(0.85f, 0.85f, 0.95f, 1f));
			((Control)_segmentLabel).AddThemeColorOverride(StringName.op_Implicit("font_pressed_color"), SegmentTextColor);
			((Control)_segmentLabel).AddThemeFontSizeOverride(StringName.op_Implicit("font_size"), 12);
			((Control)_segmentLabel).MouseDefaultCursorShape = (CursorShape)0;
			Button? segmentLabel = _segmentLabel;
			object obj8 = _003C_003EO._003C7_003E__OnSegmentLabelGuiInput;
			if (obj8 == null)
			{
				GuiInputEventHandler val13 = OnSegmentLabelGuiInput;
				_003C_003EO._003C7_003E__OnSegmentLabelGuiInput = val13;
				obj8 = (object)val13;
			}
			((Control)segmentLabel).GuiInput += (GuiInputEventHandler)obj8;
			((Node)_segmentRow).AddChild((Node)(object)_segmentLabel, false, (InternalMode)0);
			Button val14 = CreateNavButton("▶", 11, SegmentTextColor);
			object obj9 = _003C_003EO._003C8_003E__OnSegmentRightPressed;
			if (obj9 == null)
			{
				Action val15 = OnSegmentRightPressed;
				_003C_003EO._003C8_003E__OnSegmentRightPressed = val15;
				obj9 = (object)val15;
			}
			((BaseButton)val14).Pressed += (Action)obj9;
			((Node)_segmentRow).AddChild((Node)(object)val14, false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)_segmentRow, false, (InternalMode)0);
			_scrollContainer = new ScrollContainer();
			_scrollContainer.HorizontalScrollMode = (ScrollMode)0;
			_scrollContainer.VerticalScrollMode = (ScrollMode)1;
			((Control)_scrollContainer).CustomMinimumSize = new Vector2(0f, 0f);
			((Control)_scrollContainer).SizeFlagsVertical = (SizeFlags)3;
			((Control)_scrollContainer).AddThemeStyleboxOverride(StringName.op_Implicit("scroll"), (StyleBox)new StyleBoxEmpty());
			_contentBox = new VBoxContainer();
			((Control)_contentBox).AddThemeConstantOverride(StringName.op_Implicit("separation"), 2);
			((Control)_contentBox).SizeFlagsHorizontal = (SizeFlags)3;
			((Control)_contentBox).MouseFilter = (MouseFilterEnum)2;
			((Node)_scrollContainer).AddChild((Node)(object)_contentBox, false, (InternalMode)0);
			((Node)val).AddChild((Node)(object)_scrollContainer, false, (InternalMode)0);
			((Node)_panel).AddChild((Node)(object)val, false, (InternalMode)0);
			((Node)_canvas).AddChild((Node)(object)_panel, false, (InternalMode)0);
			PanelContainer? panel = _panel;
			object obj10 = _003C_003EO._003C9_003E__OnPanelGuiInput;
			if (obj10 == null)
			{
				GuiInputEventHandler val16 = OnPanelGuiInput;
				_003C_003EO._003C9_003E__OnPanelGuiInput = val16;
				obj10 = (object)val16;
			}
			((Control)panel).GuiInput += (GuiInputEventHandler)obj10;
			object obj11 = _003C_003EO._003C10_003E__UpdateDisplay;
			if (obj11 == null)
			{
				Action val17 = UpdateDisplay;
				_003C_003EO._003C10_003E__UpdateDisplay = val17;
				obj11 = (object)val17;
			}
			CombatDataCollector.StatsChanged += (Action?)obj11;
			object obj12 = _003C_003EO._003C11_003E__OnLanguageChanged;
			if (obj12 == null)
			{
				Action val18 = OnLanguageChanged;
				_003C_003EO._003C11_003E__OnLanguageChanged = val18;
				obj12 = (object)val18;
			}
			I18n.Changed += (Action?)obj12;
			_categoryPopup = new PopupPanel();
			((Window)_categoryPopup).Transparent = true;
			((Viewport)_categoryPopup).TransparentBg = true;
			StyleBoxFlat val19 = new StyleBoxFlat();
			val19.BgColor = new Color(0.08f, 0.08f, 0.15f, 0.95f);
			num = (val19.BorderWidthRight = 1);
			num3 = (val19.BorderWidthLeft = num);
			borderWidthBottom = (val19.BorderWidthTop = num3);
			val19.BorderWidthBottom = borderWidthBottom;
			val19.BorderColor = GoldBorder;
			num = (val19.CornerRadiusBottomRight = 6);
			num3 = (val19.CornerRadiusBottomLeft = num);
			borderWidthBottom = (val19.CornerRadiusTopRight = num3);
			val19.CornerRadiusTopLeft = borderWidthBottom;
			contentMarginLeft = (((StyleBox)val19).ContentMarginRight = 8f);
			((StyleBox)val19).ContentMarginLeft = contentMarginLeft;
			contentMarginLeft = (((StyleBox)val19).ContentMarginBottom = 8f);
			((StyleBox)val19).ContentMarginTop = contentMarginLeft;
			((Window)_categoryPopup).AddThemeStyleboxOverride(StringName.op_Implicit("panel"), (StyleBox)(object)val19);
			_categoryGrid = new GridContainer();
			_categoryGrid.Columns = 3;
			((Control)_categoryGrid).AddThemeConstantOverride(StringName.op_Implicit("h_separation"), 4);
			((Control)_categoryGrid).AddThemeConstantOverride(StringName.op_Implicit("v_separation"), 4);
			((Node)_categoryPopup).AddChild((Node)(object)_categoryGrid, false, (InternalMode)0);
			_segmentPopup = CreateStyledPopup();
			PopupMenu? segmentPopup = _segmentPopup;
			object obj13 = _003C_003EO._003C12_003E__OnSegmentSelected;
			if (obj13 == null)
			{
				IdPressedEventHandler val20 = OnSegmentSelected;
				_003C_003EO._003C12_003E__OnSegmentSelected = val20;
				obj13 = (object)val20;
			}
			segmentPopup.IdPressed += (IdPressedEventHandler)obj13;
			((Node)_canvas).AddChild((Node)(object)_categoryPopup, false, (InternalMode)0);
			((Node)_canvas).AddChild((Node)(object)_segmentPopup, false, (InternalMode)0);
			_settingsPopup = CreateStyledPopup();
			PopupMenu? settingsPopup = _settingsPopup;
			object obj14 = _003C_003EO._003C13_003E__OnSettingsAction;
			if (obj14 == null)
			{
				IdPressedEventHandler val21 = OnSettingsAction;
				_003C_003EO._003C13_003E__OnSettingsAction = val21;
				obj14 = (object)val21;
			}
			settingsPopup.IdPressed += (IdPressedEventHandler)obj14;
			_scaleSubmenu = CreateStyledPopup();
			((Node)_scaleSubmenu).Name = StringName.op_Implicit("ScaleMenu");
			PopupMenu? scaleSubmenu = _scaleSubmenu;
			object obj15 = _003C_003EO._003C14_003E__OnScaleSelected;
			if (obj15 == null)
			{
				IdPressedEventHandler val22 = OnScaleSelected;
				_003C_003EO._003C14_003E__OnScaleSelected = val22;
				obj15 = (object)val22;
			}
			scaleSubmenu.IdPressed += (IdPressedEventHandler)obj15;
			((Node)_settingsPopup).AddChild((Node)(object)_scaleSubmenu, false, (InternalMode)0);
			_opacitySubmenu = CreateStyledPopup();
			((Node)_opacitySubmenu).Name = StringName.op_Implicit("OpacityMenu");
			PopupMenu? opacitySubmenu = _opacitySubmenu;
			object obj16 = _003C_003EO._003C15_003E__OnOpacitySelected;
			if (obj16 == null)
			{
				IdPressedEventHandler val23 = OnOpacitySelected;
				_003C_003EO._003C15_003E__OnOpacitySelected = val23;
				obj16 = (object)val23;
			}
			opacitySubmenu.IdPressed += (IdPressedEventHandler)obj16;
			((Node)_settingsPopup).AddChild((Node)(object)_opacitySubmenu, false, (InternalMode)0);
			_maxBarsSubmenu = CreateStyledPopup();
			((Node)_maxBarsSubmenu).Name = StringName.op_Implicit("MaxBarsMenu");
			PopupMenu? maxBarsSubmenu = _maxBarsSubmenu;
			object obj17 = _003C_003EO._003C16_003E__OnMaxBarsSelected;
			if (obj17 == null)
			{
				IdPressedEventHandler val24 = OnMaxBarsSelected;
				_003C_003EO._003C16_003E__OnMaxBarsSelected = val24;
				obj17 = (object)val24;
			}
			maxBarsSubmenu.IdPressed += (IdPressedEventHandler)obj17;
			((Node)_settingsPopup).AddChild((Node)(object)_maxBarsSubmenu, false, (InternalMode)0);
			((Node)_canvas).AddChild((Node)(object)_settingsPopup, false, (InternalMode)0);
			_resetConfirmDialog = new ConfirmationDialog();
			((AcceptDialog)_resetConfirmDialog).DialogText = I18n.ResetData + "?";
			((AcceptDialog)_resetConfirmDialog).OkButtonText = I18n.ResetData;
			((Window)_resetConfirmDialog).Title = I18n.ResetData;
			ConfirmationDialog? resetConfirmDialog = _resetConfirmDialog;
			object obj18 = _003C_003EO._003C17_003E__OnResetConfirmed;
			if (obj18 == null)
			{
				Action val25 = OnResetConfirmed;
				_003C_003EO._003C17_003E__OnResetConfirmed = val25;
				obj18 = (object)val25;
			}
			((AcceptDialog)resetConfirmDialog).Confirmed += (Action)obj18;
			((Node)_canvas).AddChild((Node)(object)_resetConfirmDialog, false, (InternalMode)0);
			UpdateDisplay();
			Window root = ((SceneTree)Engine.GetMainLoop()).Root;
			((GodotObject)root).CallDeferred(MethodName.AddChild, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)_canvas) });
			if (_inputHandler == null)
			{
				_inputHandler = new InputHandler();
				((GodotObject)root).CallDeferred(MethodName.AddChild, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)_inputHandler) });
			}
		}
	}

	public static void Remove()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Expected O, but got Unknown
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Expected O, but got Unknown
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Expected O, but got Unknown
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Expected O, but got Unknown
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Expected O, but got Unknown
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Expected O, but got Unknown
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Expected O, but got Unknown
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Expected O, but got Unknown
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Expected O, but got Unknown
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Expected O, but got Unknown
		if (_canvas == null)
		{
			return;
		}
		object obj = _003C_003EO._003C10_003E__UpdateDisplay;
		if (obj == null)
		{
			Action val = UpdateDisplay;
			_003C_003EO._003C10_003E__UpdateDisplay = val;
			obj = (object)val;
		}
		CombatDataCollector.StatsChanged -= (Action?)obj;
		object obj2 = _003C_003EO._003C11_003E__OnLanguageChanged;
		if (obj2 == null)
		{
			Action val2 = OnLanguageChanged;
			_003C_003EO._003C11_003E__OnLanguageChanged = val2;
			obj2 = (object)val2;
		}
		I18n.Changed -= (Action?)obj2;
		if (_resetBtn != null)
		{
			Button? resetBtn = _resetBtn;
			object obj3 = _003C_003EO._003C0_003E__OnResetPressed;
			if (obj3 == null)
			{
				Action val3 = OnResetPressed;
				_003C_003EO._003C0_003E__OnResetPressed = val3;
				obj3 = (object)val3;
			}
			((BaseButton)resetBtn).Pressed -= (Action)obj3;
		}
		if (_dashboardBtn != null)
		{
			Button? dashboardBtn = _dashboardBtn;
			object obj4 = _003C_003EO._003C1_003E__OnDashboardPressed;
			if (obj4 == null)
			{
				Action val4 = OnDashboardPressed;
				_003C_003EO._003C1_003E__OnDashboardPressed = val4;
				obj4 = (object)val4;
			}
			((BaseButton)dashboardBtn).Pressed -= (Action)obj4;
		}
		RemoveDashboard();
		if (_leftBtn != null)
		{
			Button? leftBtn = _leftBtn;
			object obj5 = _003C_003EO._003C2_003E__OnLeftPressed;
			if (obj5 == null)
			{
				Action val5 = OnLeftPressed;
				_003C_003EO._003C2_003E__OnLeftPressed = val5;
				obj5 = (object)val5;
			}
			((BaseButton)leftBtn).Pressed -= (Action)obj5;
		}
		if (_rightBtn != null)
		{
			Button? rightBtn = _rightBtn;
			object obj6 = _003C_003EO._003C5_003E__OnRightPressed;
			if (obj6 == null)
			{
				Action val6 = OnRightPressed;
				_003C_003EO._003C5_003E__OnRightPressed = val6;
				obj6 = (object)val6;
			}
			((BaseButton)rightBtn).Pressed -= (Action)obj6;
		}
		if (_titleBtn != null)
		{
			Button? titleBtn = _titleBtn;
			object obj7 = _003C_003EO._003C3_003E__OnTitlePressed;
			if (obj7 == null)
			{
				Action val7 = OnTitlePressed;
				_003C_003EO._003C3_003E__OnTitlePressed = val7;
				obj7 = (object)val7;
			}
			((BaseButton)titleBtn).Pressed -= (Action)obj7;
			Button? titleBtn2 = _titleBtn;
			object obj8 = _003C_003EO._003C4_003E__OnTitleGuiInput;
			if (obj8 == null)
			{
				GuiInputEventHandler val8 = OnTitleGuiInput;
				_003C_003EO._003C4_003E__OnTitleGuiInput = val8;
				obj8 = (object)val8;
			}
			((Control)titleBtn2).GuiInput -= (GuiInputEventHandler)obj8;
		}
		if (_segmentLabel != null)
		{
			Button? segmentLabel = _segmentLabel;
			object obj9 = _003C_003EO._003C7_003E__OnSegmentLabelGuiInput;
			if (obj9 == null)
			{
				GuiInputEventHandler val9 = OnSegmentLabelGuiInput;
				_003C_003EO._003C7_003E__OnSegmentLabelGuiInput = val9;
				obj9 = (object)val9;
			}
			((Control)segmentLabel).GuiInput -= (GuiInputEventHandler)obj9;
		}
		if (_panel != null)
		{
			PanelContainer? panel = _panel;
			object obj10 = _003C_003EO._003C9_003E__OnPanelGuiInput;
			if (obj10 == null)
			{
				GuiInputEventHandler val10 = OnPanelGuiInput;
				_003C_003EO._003C9_003E__OnPanelGuiInput = val10;
				obj10 = (object)val10;
			}
			((Control)panel).GuiInput -= (GuiInputEventHandler)obj10;
		}
		if (_segmentPopup != null)
		{
			PopupMenu? segmentPopup = _segmentPopup;
			object obj11 = _003C_003EO._003C12_003E__OnSegmentSelected;
			if (obj11 == null)
			{
				IdPressedEventHandler val11 = OnSegmentSelected;
				_003C_003EO._003C12_003E__OnSegmentSelected = val11;
				obj11 = (object)val11;
			}
			segmentPopup.IdPressed -= (IdPressedEventHandler)obj11;
		}
		if (_settingsPopup != null)
		{
			PopupMenu? settingsPopup = _settingsPopup;
			object obj12 = _003C_003EO._003C13_003E__OnSettingsAction;
			if (obj12 == null)
			{
				IdPressedEventHandler val12 = OnSettingsAction;
				_003C_003EO._003C13_003E__OnSettingsAction = val12;
				obj12 = (object)val12;
			}
			settingsPopup.IdPressed -= (IdPressedEventHandler)obj12;
		}
		if (_scaleSubmenu != null)
		{
			PopupMenu? scaleSubmenu = _scaleSubmenu;
			object obj13 = _003C_003EO._003C14_003E__OnScaleSelected;
			if (obj13 == null)
			{
				IdPressedEventHandler val13 = OnScaleSelected;
				_003C_003EO._003C14_003E__OnScaleSelected = val13;
				obj13 = (object)val13;
			}
			scaleSubmenu.IdPressed -= (IdPressedEventHandler)obj13;
		}
		if (_opacitySubmenu != null)
		{
			PopupMenu? opacitySubmenu = _opacitySubmenu;
			object obj14 = _003C_003EO._003C15_003E__OnOpacitySelected;
			if (obj14 == null)
			{
				IdPressedEventHandler val14 = OnOpacitySelected;
				_003C_003EO._003C15_003E__OnOpacitySelected = val14;
				obj14 = (object)val14;
			}
			opacitySubmenu.IdPressed -= (IdPressedEventHandler)obj14;
		}
		if (_maxBarsSubmenu != null)
		{
			PopupMenu? maxBarsSubmenu = _maxBarsSubmenu;
			object obj15 = _003C_003EO._003C16_003E__OnMaxBarsSelected;
			if (obj15 == null)
			{
				IdPressedEventHandler val15 = OnMaxBarsSelected;
				_003C_003EO._003C16_003E__OnMaxBarsSelected = val15;
				obj15 = (object)val15;
			}
			maxBarsSubmenu.IdPressed -= (IdPressedEventHandler)obj15;
		}
		((Node)_canvas).QueueFree();
		_canvas = null;
		_panel = null;
		_titleBtn = null;
		_resetBtn = null;
		_dashboardBtn = null;
		_leftBtn = null;
		_rightBtn = null;
		_segmentRow = null;
		_segmentLabel = null;
		_contentBox = null;
		_scrollContainer = null;
		_categoryPopup = null;
		_categoryGrid = null;
		_segmentPopup = null;
		_settingsPopup = null;
		_scaleSubmenu = null;
		_opacitySubmenu = null;
		_maxBarsSubmenu = null;
		_panelStyle = null;
		if (_resetConfirmDialog != null)
		{
			ConfirmationDialog? resetConfirmDialog = _resetConfirmDialog;
			object obj16 = _003C_003EO._003C17_003E__OnResetConfirmed;
			if (obj16 == null)
			{
				Action val16 = OnResetConfirmed;
				_003C_003EO._003C17_003E__OnResetConfirmed = val16;
				obj16 = (object)val16;
			}
			((AcceptDialog)resetConfirmDialog).Confirmed -= (Action)obj16;
		}
		_resetConfirmDialog = null;
		_isDragging = false;
		_anchored = true;
	}

	public static void ToggleVisibility()
	{
		if (_canvas != null)
		{
			_canvas.Visible = !_canvas.Visible;
		}
	}

	public static void CloseDashboard()
	{
		RemoveDashboard();
	}

	private static void OnResetPressed()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (_resetConfirmDialog != null)
		{
			((AcceptDialog)_resetConfirmDialog).DialogText = I18n.ResetData + "?";
			((Window)_resetConfirmDialog).PopupCentered((Vector2I?)new Vector2I(250, 100));
		}
	}

	private static void OnResetConfirmed()
	{
		CombatDataCollector.ResetAll();
	}

	private static void OnLeftPressed()
	{
		if (_detailPlayerKey != null)
		{
			_detailPlayerKey = null;
		}
		else
		{
			_categoryIndex = (_categoryIndex - 1 + _categories.Count) % _categories.Count;
		}
		UpdateDisplay();
	}

	private static void OnRightPressed()
	{
		_detailPlayerKey = null;
		_categoryIndex = (_categoryIndex + 1) % _categories.Count;
		UpdateDisplay();
	}

	private static void OnTitlePressed()
	{
		if (_detailPlayerKey != null)
		{
			_detailPlayerKey = null;
			UpdateDisplay();
		}
	}

	private static void OnSegmentLeftPressed()
	{
		CombatDataCollector.CycleViewBackward();
	}

	private static void OnSegmentRightPressed()
	{
		CombatDataCollector.CycleViewForward();
	}

	private static void OnTitleGuiInput(InputEvent @event)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I8
		InputEventMouseButton val = (InputEventMouseButton)(object)((@event is InputEventMouseButton) ? @event : null);
		if (val != null && val.Pressed && (long)val.ButtonIndex == 2)
		{
			ShowCategoryMenu();
		}
	}

	private static void OnSegmentLabelGuiInput(InputEvent @event)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I8
		InputEventMouseButton val = (InputEventMouseButton)(object)((@event is InputEventMouseButton) ? @event : null);
		if (val != null && val.Pressed && (long)val.ButtonIndex == 2)
		{
			ShowSegmentMenu();
		}
	}

	private static void ShowCategoryMenu()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Expected O, but got Unknown
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		if (_categoryPopup == null || _categoryGrid == null || _panel == null)
		{
			return;
		}
		ClearContainer((Node)(object)_categoryGrid);
		for (int i = 0; i < _categories.Count; i++)
		{
			int catIdx = i;
			Button val = new Button();
			val.Text = _categories[i].Name;
			val.Flat = true;
			((Control)val).CustomMinimumSize = new Vector2(100f, 28f);
			((Control)val).AddThemeFontSizeOverride(StringName.op_Implicit("font_size"), 13);
			if (i == _categoryIndex)
			{
				((Control)val).AddThemeColorOverride(StringName.op_Implicit("font_color"), GoldColor);
				((Control)val).AddThemeColorOverride(StringName.op_Implicit("font_hover_color"), new Color(1f, 0.95f, 0.5f, 1f));
			}
			else
			{
				((Control)val).AddThemeColorOverride(StringName.op_Implicit("font_color"), TextColor);
				((Control)val).AddThemeColorOverride(StringName.op_Implicit("font_hover_color"), GoldColor);
			}
			((BaseButton)val).Pressed += (Action)delegate
			{
				_categoryIndex = catIdx;
				_detailPlayerKey = null;
				PopupPanel? categoryPopup = _categoryPopup;
				if (categoryPopup != null)
				{
					((Window)categoryPopup).Hide();
				}
				UpdateDisplay();
			};
			((Node)_categoryGrid).AddChild((Node)(object)val, false, (InternalMode)0);
		}
		Button val2 = new Button();
		val2.Text = "⚙ " + I18n.Settings;
		val2.Flat = true;
		((Control)val2).CustomMinimumSize = new Vector2(100f, 28f);
		((Control)val2).AddThemeFontSizeOverride(StringName.op_Implicit("font_size"), 13);
		((Control)val2).AddThemeColorOverride(StringName.op_Implicit("font_color"), DimText);
		((Control)val2).AddThemeColorOverride(StringName.op_Implicit("font_hover_color"), GoldColor);
		object obj = _003C_003Ec._003C_003E9__56_0;
		if (obj == null)
		{
			Action val3 = delegate
			{
				PopupPanel? categoryPopup = _categoryPopup;
				if (categoryPopup != null)
				{
					((Window)categoryPopup).Hide();
				}
				ShowSettingsMenu();
			};
			_003C_003Ec._003C_003E9__56_0 = val3;
			obj = (object)val3;
		}
		((BaseButton)val2).Pressed += (Action)obj;
		((Node)_categoryGrid).AddChild((Node)(object)val2, false, (InternalMode)0);
		((Window)_categoryPopup).Position = DisplayServer.MouseGetPosition();
		((Window)_categoryPopup).ResetSize();
		((Window)_categoryPopup).Popup((Rect2I?)null);
	}

	private static void ShowSegmentMenu()
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (_segmentPopup != null && _panel != null)
		{
			int segmentCount = CombatDataCollector.SegmentCount;
			_segmentPopup.Clear();
			_segmentPopup.AddRadioCheckItem(I18n.SegmentCurrent, 0, (Key)0);
			_segmentPopup.AddRadioCheckItem(I18n.SegmentOverall, 1, (Key)0);
			for (int i = 0; i < segmentCount; i++)
			{
				_segmentPopup.AddRadioCheckItem(CombatDataCollector.GetSegmentLabel(i), i + 2, (Key)0);
			}
			int viewIndex = CombatDataCollector.ViewIndex;
			_segmentPopup.SetItemChecked(viewIndex switch
			{
				-1 => 0, 
				-2 => 1, 
				_ => viewIndex + 2, 
			}, true);
			((Window)_segmentPopup).Position = DisplayServer.MouseGetPosition();
			((Window)_segmentPopup).ResetSize();
			((Window)_segmentPopup).Popup((Rect2I?)null);
		}
	}

	private static void ShowSettingsMenu()
	{
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		if (_settingsPopup != null && _scaleSubmenu != null && _opacitySubmenu != null && _maxBarsSubmenu != null)
		{
			_scaleSubmenu.Clear();
			for (int i = 0; i < ScaleOptions.Length; i++)
			{
				_scaleSubmenu.AddRadioCheckItem($"{(int)(ScaleOptions[i] * 100f)}%", i, (Key)0);
				_scaleSubmenu.SetItemChecked(i, Math.Abs(DamageMeterSettings.Scale - ScaleOptions[i]) < 0.01f);
			}
			_opacitySubmenu.Clear();
			for (int j = 0; j < OpacityOptions.Length; j++)
			{
				_opacitySubmenu.AddRadioCheckItem($"{(int)(OpacityOptions[j] * 100f)}%", j, (Key)0);
				_opacitySubmenu.SetItemChecked(j, Math.Abs(DamageMeterSettings.Opacity - OpacityOptions[j]) < 0.01f);
			}
			_maxBarsSubmenu.Clear();
			for (int k = 0; k < MaxBarsOptions.Length; k++)
			{
				_maxBarsSubmenu.AddRadioCheckItem(MaxBarsOptions[k].ToString(), k, (Key)0);
				_maxBarsSubmenu.SetItemChecked(k, DamageMeterSettings.MaxBars == MaxBarsOptions[k]);
			}
			_settingsPopup.Clear();
			_settingsPopup.AddSubmenuNodeItem(I18n.SettingsScale, _scaleSubmenu, -1);
			_settingsPopup.AddSubmenuNodeItem(I18n.SettingsOpacity, _opacitySubmenu, -1);
			_settingsPopup.AddSubmenuNodeItem(I18n.SettingsMaxBars, _maxBarsSubmenu, -1);
			_settingsPopup.AddSeparator("", -1);
			_settingsPopup.AddCheckItem(I18n.SettingsAutoReset, 200, (Key)0);
			_settingsPopup.SetItemChecked(_settingsPopup.GetItemIndex(200), DamageMeterSettings.AutoResetOnNewRun);
			_settingsPopup.AddSeparator("", -1);
			_settingsPopup.AddItem(I18n.SettingsResetPos, 100, (Key)0);
			((Window)_settingsPopup).Position = DisplayServer.MouseGetPosition();
			((Window)_settingsPopup).ResetSize();
			((Window)_settingsPopup).Popup((Rect2I?)null);
		}
	}

	private static void OnSettingsAction(long id)
	{
		switch (id)
		{
		case 200L:
			DamageMeterSettings.AutoResetOnNewRun = !DamageMeterSettings.AutoResetOnNewRun;
			DamageMeterSettings.Save();
			break;
		case 100L:
			DamageMeterSettings.PanelX = 0f / 0f;
			DamageMeterSettings.PanelY = 0f / 0f;
			DamageMeterSettings.Save();
			if (_panel != null)
			{
				((Control)_panel).AnchorLeft = 1f;
				((Control)_panel).AnchorRight = 1f;
				((Control)_panel).AnchorTop = 0f;
				((Control)_panel).AnchorBottom = 0f;
				((Control)_panel).GrowHorizontal = (GrowDirection)0;
				((Control)_panel).GrowVertical = (GrowDirection)1;
				((Control)_panel).OffsetLeft = -10f;
				((Control)_panel).OffsetTop = 10f;
				((Control)_panel).OffsetRight = 0f;
				((Control)_panel).OffsetBottom = 0f;
				_anchored = true;
			}
			break;
		}
	}

	private static void OnScaleSelected(long id)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)id;
		if (num >= 0 && num < ScaleOptions.Length)
		{
			DamageMeterSettings.Scale = ScaleOptions[num];
			DamageMeterSettings.Save();
			if (_panel != null)
			{
				((Control)_panel).Scale = new Vector2(DamageMeterSettings.Scale, DamageMeterSettings.Scale);
			}
		}
	}

	private static void OnOpacitySelected(long id)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)id;
		if (num >= 0 && num < OpacityOptions.Length)
		{
			DamageMeterSettings.Opacity = OpacityOptions[num];
			DamageMeterSettings.Save();
			if (_panelStyle != null)
			{
				_panelStyle.BgColor = new Color(0.05f, 0.05f, 0.12f, DamageMeterSettings.Opacity);
			}
		}
	}

	private static void OnMaxBarsSelected(long id)
	{
		int num = (int)id;
		if (num >= 0 && num < MaxBarsOptions.Length)
		{
			DamageMeterSettings.MaxBars = MaxBarsOptions[num];
			DamageMeterSettings.Save();
			UpdateDisplay();
		}
	}

	private static void OnSegmentSelected(long id)
	{
		int num = (int)id;
		switch (num)
		{
		case 0:
			CombatDataCollector.SetView(-1);
			break;
		case 1:
			CombatDataCollector.SetView(-2);
			break;
		default:
			CombatDataCollector.SetView(num - 2);
			break;
		}
	}

	private static PopupMenu CreateStyledPopup()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		PopupMenu val = new PopupMenu();
		StyleBoxFlat val2 = new StyleBoxFlat();
		val2.BgColor = new Color(0.08f, 0.08f, 0.15f, 0.95f);
		int num = (val2.BorderWidthRight = 1);
		int num3 = (val2.BorderWidthLeft = num);
		int borderWidthBottom = (val2.BorderWidthTop = num3);
		val2.BorderWidthBottom = borderWidthBottom;
		val2.BorderColor = GoldBorder;
		num = (val2.CornerRadiusBottomRight = 6);
		num3 = (val2.CornerRadiusBottomLeft = num);
		borderWidthBottom = (val2.CornerRadiusTopRight = num3);
		val2.CornerRadiusTopLeft = borderWidthBottom;
		float contentMarginLeft = (((StyleBox)val2).ContentMarginRight = 10f);
		((StyleBox)val2).ContentMarginLeft = contentMarginLeft;
		contentMarginLeft = (((StyleBox)val2).ContentMarginBottom = 6f);
		((StyleBox)val2).ContentMarginTop = contentMarginLeft;
		((Window)val).AddThemeStyleboxOverride(StringName.op_Implicit("panel"), (StyleBox)(object)val2);
		StyleBoxFlat val3 = new StyleBoxFlat();
		val3.BgColor = new Color(0.25f, 0.22f, 0.1f, 0.8f);
		num = (val3.CornerRadiusBottomRight = 3);
		num3 = (val3.CornerRadiusBottomLeft = num);
		borderWidthBottom = (val3.CornerRadiusTopRight = num3);
		val3.CornerRadiusTopLeft = borderWidthBottom;
		((Window)val).AddThemeStyleboxOverride(StringName.op_Implicit("hover"), (StyleBox)(object)val3);
		((Window)val).AddThemeColorOverride(StringName.op_Implicit("font_color"), TextColor);
		((Window)val).AddThemeColorOverride(StringName.op_Implicit("font_hover_color"), GoldColor);
		((Window)val).AddThemeFontSizeOverride(StringName.op_Implicit("font_size"), 13);
		return val;
	}

	private static void OnBarClicked(string playerKey)
	{
		_detailPlayerKey = playerKey;
		UpdateDisplay();
	}

	private static void OnLanguageChanged()
	{
		UpdateDisplay();
	}

	private static void UpdateDisplay()
	{
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		if (_contentBox == null || _titleBtn == null || _categories.Count == 0)
		{
			return;
		}
		IStatCategory statCategory = _categories[_categoryIndex];
		ClearContainer((Node)(object)_contentBox);
		if (_segmentRow != null)
		{
			((CanvasItem)_segmentRow).Visible = true;
			if (_segmentLabel != null)
			{
				_segmentLabel.Text = CombatDataCollector.GetViewLabel();
			}
		}
		int num = 0;
		if (_detailPlayerKey != null)
		{
			string detailTitle = statCategory.GetDetailTitle(_detailPlayerKey);
			_titleBtn.Text = "◀ " + detailTitle;
			((Control)_titleBtn).MouseDefaultCursorShape = (CursorShape)2;
			List<BarData> detailBars = statCategory.GetDetailBars(_detailPlayerKey);
			if (detailBars.Count == 0)
			{
				AddNoDataLabel();
				UpdateScrollHeight(1);
				return;
			}
			int maxVal = Enumerable.Max<BarData>((global::System.Collections.Generic.IEnumerable<BarData>)detailBars, (Func<BarData, int>)((BarData b) => b.Value));
			int totalVal = Enumerable.Sum<BarData>((global::System.Collections.Generic.IEnumerable<BarData>)detailBars, (Func<BarData, int>)((BarData b) => b.Value));
			int num2 = Math.Min(detailBars.Count, DamageMeterSettings.MaxBars);
			for (int num3 = 0; num3 < num2; num3++)
			{
				Color color = DetailColors[num3 % DetailColors.Length];
				AddBarNode(detailBars[num3], color, maxVal, totalVal, clickable: false);
			}
			num = num2;
		}
		else
		{
			_titleBtn.Text = statCategory.Name;
			((Control)_titleBtn).MouseDefaultCursorShape = (CursorShape)0;
			List<BarData> playerBars = statCategory.GetPlayerBars();
			if (playerBars.Count == 0)
			{
				AddNoDataLabel();
				UpdateScrollHeight(1);
				return;
			}
			bool hasDetail = statCategory.HasDetail;
			int maxVal2 = Enumerable.Max<BarData>((global::System.Collections.Generic.IEnumerable<BarData>)playerBars, (Func<BarData, int>)((BarData b) => b.Value));
			int totalVal2 = Enumerable.Sum<BarData>((global::System.Collections.Generic.IEnumerable<BarData>)playerBars, (Func<BarData, int>)((BarData b) => b.Value));
			int num4 = Math.Min(playerBars.Count, DamageMeterSettings.MaxBars);
			CombatDataCollector.PlayerStats playerStats = default(CombatDataCollector.PlayerStats);
			for (int num5 = 0; num5 < num4; num5++)
			{
				Color color2 = (hasDetail ? GetPlayerColor(playerBars[num5].Key) : DetailColors[num5 % DetailColors.Length]);
				string characterId = (CombatDataCollector.Players.TryGetValue(playerBars[num5].Key, ref playerStats) ? playerStats.CharacterId : playerBars[num5].Key);
				Texture2D icon = (hasDetail ? LoadCharacterIcon(characterId) : null);
				if (hasDetail)
				{
					playerBars[num5].Label = $"{num5 + 1}. {playerBars[num5].Label}";
				}
				AddBarNode(playerBars[num5], color2, maxVal2, totalVal2, hasDetail, icon);
			}
			num = num4;
		}
		UpdateScrollHeight(num);
		UpdateDashboard();
	}

	private static void UpdateScrollHeight(int barCount)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (_scrollContainer != null)
		{
			float num = (float)barCount * 24f;
			float num2 = 300f;
			((Control)_scrollContainer).CustomMinimumSize = new Vector2(0f, Math.Min(num, num2));
		}
	}

	private static void AddNoDataLabel()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Label val = new Label();
		val.Text = I18n.WaitingForCombat;
		val.HorizontalAlignment = (HorizontalAlignment)1;
		((Control)val).AddThemeColorOverride(StringName.op_Implicit("font_color"), DimText);
		((Control)val).AddThemeFontSizeOverride(StringName.op_Implicit("font_size"), 13);
		((Control)val).MouseFilter = (MouseFilterEnum)2;
		VBoxContainer? contentBox = _contentBox;
		if (contentBox != null)
		{
			((Node)contentBox).AddChild((Node)(object)val, false, (InternalMode)0);
		}
	}

	private static void AddBarNode(BarData data, Color color, int maxVal, int totalVal, bool clickable, Texture2D? icon = null)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Expected O, but got Unknown
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Expected O, but got Unknown
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Expected O, but got Unknown
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Expected O, but got Unknown
		if (_contentBox == null)
		{
			return;
		}
		float anchorRight = ((maxVal > 0) ? Math.Clamp((float)data.Value / (float)maxVal, 0f, 1f) : 0f);
		float num = ((totalVal > 0) ? ((float)data.Value / (float)totalVal * 100f) : 0f);
		Panel val = new Panel();
		((Control)val).CustomMinimumSize = new Vector2(0f, 22f);
		((Control)val).SizeFlagsHorizontal = (SizeFlags)3;
		((Control)val).MouseFilter = (MouseFilterEnum)0;
		StyleBoxFlat val2 = new StyleBoxFlat();
		val2.BgColor = BarBgColor;
		int num2 = (val2.CornerRadiusBottomRight = 3);
		int num4 = (val2.CornerRadiusBottomLeft = num2);
		int cornerRadiusTopLeft = (val2.CornerRadiusTopRight = num4);
		val2.CornerRadiusTopLeft = cornerRadiusTopLeft;
		((Control)val).AddThemeStyleboxOverride(StringName.op_Implicit("panel"), (StyleBox)(object)val2);
		ColorRect val3 = new ColorRect();
		val3.Color = new Color(color.R, color.G, color.B, 0.35f);
		((Control)val3).AnchorRight = anchorRight;
		((Control)val3).AnchorBottom = 1f;
		((Control)val3).OffsetLeft = 0f;
		((Control)val3).OffsetTop = 0f;
		((Control)val3).OffsetRight = 0f;
		((Control)val3).OffsetBottom = 0f;
		((Control)val3).MouseFilter = (MouseFilterEnum)2;
		((Node)val).AddChild((Node)(object)val3, false, (InternalMode)0);
		int num7 = 0;
		if (icon != null)
		{
			TextureRect val4 = new TextureRect();
			val4.Texture = icon;
			val4.ExpandMode = (ExpandModeEnum)1;
			val4.StretchMode = (StretchModeEnum)5;
			((Control)val4).AnchorBottom = 1f;
			((Control)val4).OffsetLeft = 3f;
			((Control)val4).OffsetTop = 2f;
			((Control)val4).OffsetRight = 21f;
			((Control)val4).OffsetBottom = -2f;
			((Control)val4).MouseFilter = (MouseFilterEnum)2;
			((Node)val).AddChild((Node)(object)val4, false, (InternalMode)0);
			num7 = 22;
		}
		Label val5 = new Label();
		val5.Text = ((icon != null) ? (" " + data.Label) : ("  " + data.Label));
		((Control)val5).AnchorRight = 0.55f;
		((Control)val5).AnchorBottom = 1f;
		((Control)val5).OffsetLeft = num7;
		((Control)val5).OffsetTop = 0f;
		((Control)val5).OffsetRight = 0f;
		((Control)val5).OffsetBottom = 0f;
		val5.VerticalAlignment = (VerticalAlignment)1;
		((Control)val5).AddThemeFontSizeOverride(StringName.op_Implicit("font_size"), 12);
		((Control)val5).AddThemeColorOverride(StringName.op_Implicit("font_color"), Colors.White);
		val5.ClipText = true;
		((Control)val5).MouseFilter = (MouseFilterEnum)2;
		((Node)val).AddChild((Node)(object)val5, false, (InternalMode)0);
		Label val6 = new Label();
		val6.Text = data.DisplayText ?? $"{data.Value}  ({num:F1}%)  ";
		((Control)val6).AnchorLeft = 0.5f;
		((Control)val6).AnchorRight = 1f;
		((Control)val6).AnchorBottom = 1f;
		((Control)val6).OffsetLeft = 0f;
		((Control)val6).OffsetTop = 0f;
		((Control)val6).OffsetRight = 0f;
		((Control)val6).OffsetBottom = 0f;
		val6.HorizontalAlignment = (HorizontalAlignment)2;
		val6.VerticalAlignment = (VerticalAlignment)1;
		((Control)val6).AddThemeFontSizeOverride(StringName.op_Implicit("font_size"), 12);
		((Control)val6).AddThemeColorOverride(StringName.op_Implicit("font_color"), TextColor);
		val6.ClipText = true;
		((Control)val6).MouseFilter = (MouseFilterEnum)2;
		((Node)val).AddChild((Node)(object)val6, false, (InternalMode)0);
		if (clickable)
		{
			string key = data.Key;
			((Control)val).GuiInput += (GuiInputEventHandler)delegate(InputEvent @event)
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Invalid comparison between Unknown and I8
				InputEventMouseButton val7 = (InputEventMouseButton)(object)((@event is InputEventMouseButton) ? @event : null);
				if (val7 != null && val7.Pressed && (long)val7.ButtonIndex == 1)
				{
					OnBarClicked(key);
				}
			};
			((Control)val).MouseDefaultCursorShape = (CursorShape)2;
		}
		((Node)_contentBox).AddChild((Node)(object)val, false, (InternalMode)0);
	}

	private static void OnDashboardPressed()
	{
		if (_dashboardVisible)
		{
			RemoveDashboard();
		}
		else
		{
			ShowDashboard();
		}
	}

	private static void ShowDashboard()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Expected O, but got Unknown
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Expected O, but got Unknown
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Expected O, but got Unknown
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Expected O, but got Unknown
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Expected O, but got Unknown
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Expected O, but got Unknown
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Expected O, but got Unknown
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Expected O, but got Unknown
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Expected O, but got Unknown
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Expected O, but got Unknown
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		if (_dashboardCanvas == null)
		{
			_dashboardCanvas = new CanvasLayer
			{
				Layer = 110,
				Name = StringName.op_Implicit("DamageMeterDashboard")
			};
			ColorRect val = new ColorRect();
			val.Color = new Color(0f, 0f, 0f, 0.6f);
			((Control)val).SetAnchorsPreset((LayoutPreset)15, false);
			((Control)val).MouseFilter = (MouseFilterEnum)0;
			object obj = _003C_003EO._003C18_003E__OnDashboardBgInput;
			if (obj == null)
			{
				GuiInputEventHandler val2 = OnDashboardBgInput;
				_003C_003EO._003C18_003E__OnDashboardBgInput = val2;
				obj = (object)val2;
			}
			((Control)val).GuiInput += (GuiInputEventHandler)obj;
			((Node)_dashboardCanvas).AddChild((Node)(object)val, false, (InternalMode)0);
			MarginContainer val3 = new MarginContainer();
			((Control)val3).SetAnchorsPreset((LayoutPreset)15, false);
			((Control)val3).AddThemeConstantOverride(StringName.op_Implicit("margin_left"), 40);
			((Control)val3).AddThemeConstantOverride(StringName.op_Implicit("margin_right"), 40);
			((Control)val3).AddThemeConstantOverride(StringName.op_Implicit("margin_top"), 30);
			((Control)val3).AddThemeConstantOverride(StringName.op_Implicit("margin_bottom"), 30);
			((Node)val).AddChild((Node)(object)val3, false, (InternalMode)0);
			PanelContainer val4 = new PanelContainer();
			StyleBoxFlat val5 = new StyleBoxFlat();
			val5.BgColor = new Color(0.04f, 0.04f, 0.1f, 0.95f);
			int num = (val5.BorderWidthRight = 1);
			int num3 = (val5.BorderWidthLeft = num);
			int borderWidthBottom = (val5.BorderWidthTop = num3);
			val5.BorderWidthBottom = borderWidthBottom;
			val5.BorderColor = GoldBorder;
			num = (val5.CornerRadiusBottomRight = 10);
			num3 = (val5.CornerRadiusBottomLeft = num);
			borderWidthBottom = (val5.CornerRadiusTopRight = num3);
			val5.CornerRadiusTopLeft = borderWidthBottom;
			float contentMarginLeft = (((StyleBox)val5).ContentMarginRight = 16f);
			((StyleBox)val5).ContentMarginLeft = contentMarginLeft;
			((StyleBox)val5).ContentMarginTop = 12f;
			((StyleBox)val5).ContentMarginBottom = 16f;
			((Control)val4).AddThemeStyleboxOverride(StringName.op_Implicit("panel"), (StyleBox)(object)val5);
			((Node)val3).AddChild((Node)(object)val4, false, (InternalMode)0);
			VBoxContainer val6 = new VBoxContainer();
			((Control)val6).AddThemeConstantOverride(StringName.op_Implicit("separation"), 10);
			((Node)val4).AddChild((Node)(object)val6, false, (InternalMode)0);
			HBoxContainer val7 = new HBoxContainer();
			((Control)val7).AddThemeConstantOverride(StringName.op_Implicit("separation"), 8);
			Label val8 = new Label();
			val8.Text = I18n.Dashboard + "  —  " + CombatDataCollector.GetViewLabel();
			((Control)val8).AddThemeColorOverride(StringName.op_Implicit("font_color"), GoldColor);
			((Control)val8).AddThemeFontSizeOverride(StringName.op_Implicit("font_size"), 18);
			((Control)val8).SizeFlagsHorizontal = (SizeFlags)3;
			((Node)val7).AddChild((Node)(object)val8, false, (InternalMode)0);
			Button val9 = new Button();
			val9.Text = "✕";
			val9.Flat = true;
			((Control)val9).CustomMinimumSize = new Vector2(32f, 28f);
			((Control)val9).AddThemeFontSizeOverride(StringName.op_Implicit("font_size"), 16);
			((Control)val9).AddThemeColorOverride(StringName.op_Implicit("font_color"), DimText);
			((Control)val9).AddThemeColorOverride(StringName.op_Implicit("font_hover_color"), new Color(1f, 0.4f, 0.4f, 1f));
			object obj2 = _003C_003EO._003C19_003E__RemoveDashboard;
			if (obj2 == null)
			{
				Action val10 = RemoveDashboard;
				_003C_003EO._003C19_003E__RemoveDashboard = val10;
				obj2 = (object)val10;
			}
			((BaseButton)val9).Pressed += (Action)obj2;
			((Node)val7).AddChild((Node)(object)val9, false, (InternalMode)0);
			((Node)val6).AddChild((Node)(object)val7, false, (InternalMode)0);
			HSeparator val11 = new HSeparator();
			StyleBoxFlat val12 = new StyleBoxFlat
			{
				BgColor = new Color(1f, 0.84f, 0f, 0.2f)
			};
			((Control)val11).AddThemeStyleboxOverride(StringName.op_Implicit("separator"), (StyleBox)(object)val12);
			((Control)val11).AddThemeConstantOverride(StringName.op_Implicit("separation"), 4);
			((Node)val6).AddChild((Node)(object)val11, false, (InternalMode)0);
			ScrollContainer val13 = new ScrollContainer();
			val13.HorizontalScrollMode = (ScrollMode)0;
			val13.VerticalScrollMode = (ScrollMode)1;
			((Control)val13).SizeFlagsVertical = (SizeFlags)3;
			((Node)val6).AddChild((Node)(object)val13, false, (InternalMode)0);
			GridContainer val14 = new GridContainer();
			val14.Columns = 4;
			((Control)val14).AddThemeConstantOverride(StringName.op_Implicit("h_separation"), 12);
			((Control)val14).AddThemeConstantOverride(StringName.op_Implicit("v_separation"), 12);
			((Control)val14).SizeFlagsHorizontal = (SizeFlags)3;
			((Node)val13).AddChild((Node)(object)val14, false, (InternalMode)0);
			for (int i = 0; i < _categories.Count; i++)
			{
				PanelContainer val15 = CreateDashboardMiniPanel(_categories[i], i);
				((Node)val14).AddChild((Node)(object)val15, false, (InternalMode)0);
			}
			_dashboardVisible = true;
			Window root = ((SceneTree)Engine.GetMainLoop()).Root;
			((GodotObject)root).CallDeferred(MethodName.AddChild, (Variant[])(object)new Variant[1] { Variant.op_Implicit((GodotObject)(object)_dashboardCanvas) });
		}
	}

	private static void RemoveDashboard()
	{
		if (_dashboardCanvas != null)
		{
			((Node)_dashboardCanvas).QueueFree();
			_dashboardCanvas = null;
		}
		_dashboardVisible = false;
		_dashboardDetailState.Clear();
	}

	private static void UpdateDashboard()
	{
		if (_dashboardVisible && _dashboardCanvas != null)
		{
			RemoveDashboard();
			ShowDashboard();
		}
	}

	private static void OnDashboardBgInput(InputEvent @event)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Invalid comparison between Unknown and I8
		InputEventMouseButton val = (InputEventMouseButton)(object)((@event is InputEventMouseButton) ? @event : null);
		if (val != null && val.Pressed && (long)val.ButtonIndex == 2)
		{
			RemoveDashboard();
		}
	}

	private static PanelContainer CreateDashboardMiniPanel(IStatCategory category, int categoryIndex)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Expected O, but got Unknown
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Expected O, but got Unknown
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Expected O, but got Unknown
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Expected O, but got Unknown
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Expected O, but got Unknown
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Expected O, but got Unknown
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		PanelContainer val = new PanelContainer();
		((Control)val).SizeFlagsHorizontal = (SizeFlags)3;
		((Control)val).CustomMinimumSize = new Vector2(280f, 0f);
		StyleBoxFlat val2 = new StyleBoxFlat();
		val2.BgColor = new Color(0.06f, 0.06f, 0.14f, 0.9f);
		int num = (val2.BorderWidthRight = 1);
		int num3 = (val2.BorderWidthLeft = num);
		int borderWidthBottom = (val2.BorderWidthTop = num3);
		val2.BorderWidthBottom = borderWidthBottom;
		val2.BorderColor = new Color(1f, 0.84f, 0f, 0.15f);
		num = (val2.CornerRadiusBottomRight = 6);
		num3 = (val2.CornerRadiusBottomLeft = num);
		borderWidthBottom = (val2.CornerRadiusTopRight = num3);
		val2.CornerRadiusTopLeft = borderWidthBottom;
		float contentMarginLeft = (((StyleBox)val2).ContentMarginRight = 8f);
		((StyleBox)val2).ContentMarginLeft = contentMarginLeft;
		((StyleBox)val2).ContentMarginTop = 6f;
		((StyleBox)val2).ContentMarginBottom = 8f;
		((Control)val).AddThemeStyleboxOverride(StringName.op_Implicit("panel"), (StyleBox)(object)val2);
		VBoxContainer val3 = new VBoxContainer();
		((Control)val3).AddThemeConstantOverride(StringName.op_Implicit("separation"), 3);
		string text = default(string);
		_dashboardDetailState.TryGetValue(categoryIndex, ref text);
		bool flag = text != null;
		int catIdx = categoryIndex;
		if (flag)
		{
			Button val4 = new Button();
			val4.Text = "◀ " + category.GetDetailTitle(text);
			val4.Flat = true;
			((Control)val4).SizeFlagsHorizontal = (SizeFlags)3;
			((Control)val4).AddThemeColorOverride(StringName.op_Implicit("font_color"), GoldColor);
			((Control)val4).AddThemeColorOverride(StringName.op_Implicit("font_hover_color"), new Color(1f, 0.95f, 0.5f, 1f));
			((Control)val4).AddThemeFontSizeOverride(StringName.op_Implicit("font_size"), 13);
			((Control)val4).MouseDefaultCursorShape = (CursorShape)2;
			((BaseButton)val4).Pressed += (Action)delegate
			{
				OnDashboardTitleClicked(catIdx);
			};
			((Node)val3).AddChild((Node)(object)val4, false, (InternalMode)0);
		}
		else
		{
			Label val5 = new Label();
			val5.Text = category.Name;
			((Control)val5).AddThemeColorOverride(StringName.op_Implicit("font_color"), GoldColor);
			((Control)val5).AddThemeFontSizeOverride(StringName.op_Implicit("font_size"), 13);
			val5.HorizontalAlignment = (HorizontalAlignment)1;
			((Node)val3).AddChild((Node)(object)val5, false, (InternalMode)0);
		}
		HSeparator val6 = new HSeparator();
		StyleBoxFlat val7 = new StyleBoxFlat
		{
			BgColor = new Color(1f, 0.84f, 0f, 0.15f)
		};
		((Control)val6).AddThemeStyleboxOverride(StringName.op_Implicit("separator"), (StyleBox)(object)val7);
		((Control)val6).AddThemeConstantOverride(StringName.op_Implicit("separation"), 2);
		((Node)val3).AddChild((Node)(object)val6, false, (InternalMode)0);
		List<BarData> val8;
		bool flag2;
		if (flag)
		{
			val8 = category.GetDetailBars(text);
			flag2 = false;
		}
		else
		{
			val8 = category.GetPlayerBars();
			flag2 = category.HasDetail;
		}
		if (val8.Count == 0)
		{
			Label val9 = new Label();
			val9.Text = I18n.WaitingForCombat;
			val9.HorizontalAlignment = (HorizontalAlignment)1;
			((Control)val9).AddThemeColorOverride(StringName.op_Implicit("font_color"), DimText);
			((Control)val9).AddThemeFontSizeOverride(StringName.op_Implicit("font_size"), 11);
			((Node)val3).AddChild((Node)(object)val9, false, (InternalMode)0);
		}
		else
		{
			int maxVal = Enumerable.Max<BarData>((global::System.Collections.Generic.IEnumerable<BarData>)val8, (Func<BarData, int>)((BarData b) => b.Value));
			int totalVal = Enumerable.Sum<BarData>((global::System.Collections.Generic.IEnumerable<BarData>)val8, (Func<BarData, int>)((BarData b) => b.Value));
			int num10 = Math.Min(val8.Count, 8);
			for (int num11 = 0; num11 < num10; num11++)
			{
				BarData barData = val8[num11];
				string label = ((!flag && flag2) ? $"{num11 + 1}. {barData.Label}" : barData.Label);
				Color color = ((!flag) ? (flag2 ? GetPlayerColor(barData.Key) : DetailColors[num11 % DetailColors.Length]) : DetailColors[num11 % DetailColors.Length]);
				string playerKey = barData.Key;
				Panel val10 = CreateDashboardBar(label, barData.Value, barData.DisplayText, color, maxVal, totalVal, flag2, flag2 ? ((Action)delegate
				{
					OnDashboardBarClicked(catIdx, playerKey);
				}) : ((Action)null));
				((Node)val3).AddChild((Node)(object)val10, false, (InternalMode)0);
			}
		}
		((Node)val).AddChild((Node)(object)val3, false, (InternalMode)0);
		return val;
	}

	private static void OnDashboardBarClicked(int categoryIndex, string playerKey)
	{
		_dashboardDetailState[categoryIndex] = playerKey;
		CanvasLayer dashboardCanvas = _dashboardCanvas;
		_dashboardCanvas = null;
		_dashboardVisible = false;
		if (dashboardCanvas != null)
		{
			((Node)dashboardCanvas).QueueFree();
		}
		ShowDashboard();
	}

	private static void OnDashboardTitleClicked(int categoryIndex)
	{
		_dashboardDetailState.Remove(categoryIndex);
		CanvasLayer dashboardCanvas = _dashboardCanvas;
		_dashboardCanvas = null;
		_dashboardVisible = false;
		if (dashboardCanvas != null)
		{
			((Node)dashboardCanvas).QueueFree();
		}
		ShowDashboard();
	}

	private static Panel CreateDashboardBar(string label, int value, string? displayText, Color color, int maxVal, int totalVal, bool clickable = false, Action? onClick = null)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Expected O, but got Unknown
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Expected O, but got Unknown
		float anchorRight = ((maxVal > 0) ? Math.Clamp((float)value / (float)maxVal, 0f, 1f) : 0f);
		float num = ((totalVal > 0) ? ((float)value / (float)totalVal * 100f) : 0f);
		Panel val = new Panel();
		((Control)val).CustomMinimumSize = new Vector2(0f, 18f);
		((Control)val).SizeFlagsHorizontal = (SizeFlags)3;
		((Control)val).MouseFilter = (MouseFilterEnum)(clickable ? 0 : 2);
		StyleBoxFlat val2 = new StyleBoxFlat();
		val2.BgColor = BarBgColor;
		int num2 = (val2.CornerRadiusBottomRight = 2);
		int num4 = (val2.CornerRadiusBottomLeft = num2);
		int cornerRadiusTopLeft = (val2.CornerRadiusTopRight = num4);
		val2.CornerRadiusTopLeft = cornerRadiusTopLeft;
		((Control)val).AddThemeStyleboxOverride(StringName.op_Implicit("panel"), (StyleBox)(object)val2);
		ColorRect val3 = new ColorRect();
		val3.Color = new Color(color.R, color.G, color.B, 0.35f);
		((Control)val3).AnchorRight = anchorRight;
		((Control)val3).AnchorBottom = 1f;
		((Control)val3).MouseFilter = (MouseFilterEnum)2;
		((Node)val).AddChild((Node)(object)val3, false, (InternalMode)0);
		Label val4 = new Label();
		val4.Text = "  " + label;
		((Control)val4).AnchorRight = 0.55f;
		((Control)val4).AnchorBottom = 1f;
		val4.VerticalAlignment = (VerticalAlignment)1;
		((Control)val4).AddThemeFontSizeOverride(StringName.op_Implicit("font_size"), 11);
		((Control)val4).AddThemeColorOverride(StringName.op_Implicit("font_color"), Colors.White);
		val4.ClipText = true;
		((Control)val4).MouseFilter = (MouseFilterEnum)2;
		((Node)val).AddChild((Node)(object)val4, false, (InternalMode)0);
		Label val5 = new Label();
		val5.Text = displayText ?? $"{value}  ({num:F1}%)  ";
		((Control)val5).AnchorLeft = 0.5f;
		((Control)val5).AnchorRight = 1f;
		((Control)val5).AnchorBottom = 1f;
		val5.HorizontalAlignment = (HorizontalAlignment)2;
		val5.VerticalAlignment = (VerticalAlignment)1;
		((Control)val5).AddThemeFontSizeOverride(StringName.op_Implicit("font_size"), 11);
		((Control)val5).AddThemeColorOverride(StringName.op_Implicit("font_color"), TextColor);
		val5.ClipText = true;
		((Control)val5).MouseFilter = (MouseFilterEnum)2;
		((Node)val).AddChild((Node)(object)val5, false, (InternalMode)0);
		if (clickable && onClick != null)
		{
			((Control)val).GuiInput += (GuiInputEventHandler)delegate(InputEvent @event)
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_001b: Invalid comparison between Unknown and I8
				InputEventMouseButton val6 = (InputEventMouseButton)(object)((@event is InputEventMouseButton) ? @event : null);
				if (val6 != null && val6.Pressed && (long)val6.ButtonIndex == 1)
				{
					onClick.Invoke();
				}
			};
			((Control)val).MouseDefaultCursorShape = (CursorShape)2;
		}
		return val;
	}

	private static Texture2D? LoadCharacterIcon(string characterId)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		Texture2D result = default(Texture2D);
		if (_iconCache.TryGetValue(characterId, ref result))
		{
			return result;
		}
		Texture2D val = null;
		string text = default(string);
		if (CharacterIconMap.TryGetValue(characterId, ref text))
		{
			try
			{
				Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DamageMeter.icons." + text + ".png");
				try
				{
					if (manifestResourceStream != null)
					{
						byte[] array = new byte[manifestResourceStream.Length];
						manifestResourceStream.ReadExactly(global::System.Span<byte>.op_Implicit(array));
						Image val2 = new Image();
						val2.LoadPngFromBuffer(array);
						val = (Texture2D)(object)ImageTexture.CreateFromImage(val2);
					}
				}
				finally
				{
					((global::System.IDisposable)manifestResourceStream)?.Dispose();
				}
			}
			catch (global::System.Exception ex)
			{
				MainFile.Log.Error("Failed to load icon for " + characterId + ": " + ex.Message, 1);
			}
		}
		_iconCache[characterId] = val;
		return val;
	}

	private static Color GetPlayerColor(string playerKey)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		CombatDataCollector.PlayerStats playerStats = default(CombatDataCollector.PlayerStats);
		if (CombatDataCollector.Players.TryGetValue(playerKey, ref playerStats))
		{
			return playerStats.CharacterColor;
		}
		int num = _playerColorOrder.IndexOf(playerKey);
		if (num < 0)
		{
			_playerColorOrder.Add(playerKey);
			num = _playerColorOrder.Count - 1;
		}
		Color[] array = (Color[])(object)new Color[4]
		{
			new Color(0.95f, 0.55f, 0.15f, 1f),
			new Color(0.3f, 0.6f, 1f, 1f),
			new Color(0.3f, 0.85f, 0.4f, 1f),
			new Color(0.7f, 0.4f, 1f, 1f)
		};
		return array[num % array.Length];
	}

	private static Button CreateNavButton(string text, int fontSize = 13, Color? color = null)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		Color val = (Color)(((_003F?)color) ?? GoldColor);
		Button val2 = new Button();
		val2.Text = text;
		val2.Flat = true;
		((Control)val2).CustomMinimumSize = new Vector2(24f, 22f);
		((Control)val2).AddThemeFontSizeOverride(StringName.op_Implicit("font_size"), fontSize);
		((Control)val2).AddThemeColorOverride(StringName.op_Implicit("font_color"), val);
		((Control)val2).AddThemeColorOverride(StringName.op_Implicit("font_hover_color"), new Color(val.R * 1.1f, val.G * 1.1f, val.B * 1.1f, 1f));
		return val2;
	}

	private static void ClearContainer(Node container)
	{
		for (int num = container.GetChildCount(false) - 1; num >= 0; num--)
		{
			Node child = container.GetChild(num, false);
			container.RemoveChild(child);
			child.QueueFree();
		}
	}

	private static void OnPanelGuiInput(InputEvent @event)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Invalid comparison between Unknown and I8
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Invalid comparison between Unknown and I8
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		if (_panel == null)
		{
			return;
		}
		InputEventMouseButton val = (InputEventMouseButton)(object)((@event is InputEventMouseButton) ? @event : null);
		if (val != null && val.Pressed && (long)val.ButtonIndex == 2)
		{
			ShowCategoryMenu();
			return;
		}
		InputEventMouseButton val2 = (InputEventMouseButton)(object)((@event is InputEventMouseButton) ? @event : null);
		if (val2 != null && (long)val2.ButtonIndex == 1)
		{
			if (val2.Pressed)
			{
				_isDragging = true;
				if (_anchored)
				{
					Vector2 globalPosition = ((Control)_panel).GlobalPosition;
					((Control)_panel).AnchorLeft = 0f;
					((Control)_panel).AnchorRight = 0f;
					((Control)_panel).AnchorTop = 0f;
					((Control)_panel).AnchorBottom = 0f;
					((Control)_panel).OffsetLeft = 0f;
					((Control)_panel).OffsetTop = 0f;
					((Control)_panel).OffsetRight = 0f;
					((Control)_panel).OffsetBottom = 0f;
					((Control)_panel).Position = globalPosition;
					_anchored = false;
				}
				_dragOffset = ((Control)_panel).Position - ((InputEventMouse)val2).GlobalPosition;
			}
			else
			{
				_isDragging = false;
				DamageMeterSettings.PanelX = ((Control)_panel).Position.X;
				DamageMeterSettings.PanelY = ((Control)_panel).Position.Y;
				DamageMeterSettings.Save();
			}
		}
		else
		{
			InputEventMouseMotion val3 = (InputEventMouseMotion)(object)((@event is InputEventMouseMotion) ? @event : null);
			if (val3 != null && _isDragging)
			{
				((Control)_panel).Position = ((InputEventMouse)val3).GlobalPosition + _dragOffset;
			}
		}
	}

	static DamageMeterUI()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		float[] array = new float[4];
		RuntimeHelpers.InitializeArray((global::System.Array)array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		ScaleOptions = array;
		float[] array2 = new float[4];
		RuntimeHelpers.InitializeArray((global::System.Array)array2, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		OpacityOptions = array2;
		int[] array3 = new int[5];
		RuntimeHelpers.InitializeArray((global::System.Array)array3, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		MaxBarsOptions = array3;
	}
}
