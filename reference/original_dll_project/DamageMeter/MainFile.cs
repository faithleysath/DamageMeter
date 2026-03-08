using System.Reflection;
using DamageMeter.Scripts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace DamageMeter;

[ModInitializer("Initialize")]
public class MainFile
{
	internal const string ModId = "sts2.piyixiajiuhenfen.damagemeter";

	internal static readonly Logger Log = new Logger("sts2.piyixiajiuhenfen.damagemeter", (LogType)0);

	public static void Initialize()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		Harmony val = new Harmony("sts2.piyixiajiuhenfen.damagemeter");
		val.PatchAll(Assembly.GetExecutingAssembly());
		DamageMeterSettings.Load();
		I18n.Initialize();
		DamageMeterUI.Initialize();
		Log.Info("DamageMeter v1.0.3 initialized!", 1);
	}
}
