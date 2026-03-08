using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

namespace DamageMeter.Scripts;

[ScriptPath("res://Scripts/InputHandler.cs")]
public class InputHandler : Node
{
	public class MethodName : MethodName
	{
		public static readonly StringName _UnhandledKeyInput = StringName.op_Implicit("_UnhandledKeyInput");
	}

	public class PropertyName : PropertyName
	{
	}

	public class SignalName : SignalName
	{
	}

	public override void _UnhandledKeyInput(InputEvent @event)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I8
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Invalid comparison between Unknown and I8
		InputEventKey val = (InputEventKey)(object)((@event is InputEventKey) ? @event : null);
		if (val != null && val.Pressed && (long)val.Keycode == 4194338)
		{
			DamageMeterUI.ToggleVisibility();
			((Node)this).GetViewport().SetInputAsHandled();
			return;
		}
		val = (InputEventKey)(object)((@event is InputEventKey) ? @event : null);
		if (val != null && val.Pressed && (long)val.Keycode == 4194305 && DamageMeterUI.IsDashboardVisible)
		{
			DamageMeterUI.CloseDashboard();
			((Node)this).GetViewport().SetInputAsHandled();
		}
	}

	[EditorBrowsable(/*Could not decode attribute arguments.*/)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		List<MethodInfo> val = new List<MethodInfo>(1);
		StringName unhandledKeyInput = MethodName._UnhandledKeyInput;
		PropertyInfo val2 = new PropertyInfo((Type)0, StringName.op_Implicit(""), (PropertyHint)0, "", (PropertyUsageFlags)6, false);
		long num = 1L;
		List<PropertyInfo> obj = new List<PropertyInfo>();
		obj.Add(new PropertyInfo((Type)24, StringName.op_Implicit("event"), (PropertyHint)0, "", (PropertyUsageFlags)6, new StringName("InputEvent"), false));
		val.Add(new MethodInfo(unhandledKeyInput, val2, (MethodFlags)num, obj, (List<Variant>)null));
		return val;
	}

	[EditorBrowsable(/*Could not decode attribute arguments.*/)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if ((ref method) == MethodName._UnhandledKeyInput && ((NativeVariantPtrArgs)(ref args)).Count == 1)
		{
			((Node)this)._UnhandledKeyInput(VariantUtils.ConvertTo<InputEvent>(ref ((NativeVariantPtrArgs)(ref args))[0]));
			ret = default(godot_variant);
			return true;
		}
		return ((Node)this).InvokeGodotClassMethod(ref method, args, ref ret);
	}

	[EditorBrowsable(/*Could not decode attribute arguments.*/)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if ((ref method) == MethodName._UnhandledKeyInput)
		{
			return true;
		}
		return ((Node)this).HasGodotClassMethod(ref method);
	}

	[EditorBrowsable(/*Could not decode attribute arguments.*/)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).SaveGodotObjectData(info);
	}

	[EditorBrowsable(/*Could not decode attribute arguments.*/)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		((GodotObject)this).RestoreGodotObjectData(info);
	}
}
