using System;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;

namespace DamageMeter.Scripts.Patches;

[HarmonyPatch(typeof(CombatManager), "Reset")]
public static class CombatResetPatch
{
	[HarmonyPrefix]
	public static void Prefix()
	{
		try
		{
			CombatManager.Instance.TurnStarted -= CombatDataCollector.OnTurnStarted;
			CombatManager.Instance.TurnEnded -= CombatDataCollector.OnTurnEnded;
			CombatDataCollector.StopTracking();
		}
		catch (global::System.Exception ex)
		{
			MainFile.Log.Error($"CombatResetPatch failed: {ex}", 1);
		}
	}
}
