using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DamageMeter.Scripts.Categories;

public class DamageDealtCategory : IStatCategory
{
	public string Name => I18n.CatDamageDealt;

	public List<BarData> GetPlayerBars()
	{
		List<BarData> val = new List<BarData>();
		global::System.Collections.Generic.IEnumerator<CombatDataCollector.PlayerStats> enumerator = ((global::System.Collections.Generic.IEnumerable<CombatDataCollector.PlayerStats>)Enumerable.OrderByDescending<CombatDataCollector.PlayerStats, int>(CombatDataCollector.Players.Values, (Func<CombatDataCollector.PlayerStats, int>)((CombatDataCollector.PlayerStats p) => p.DamageDealt))).GetEnumerator();
		try
		{
			while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
			{
				CombatDataCollector.PlayerStats current = enumerator.Current;
				val.Add(new BarData
				{
					Key = current.Key,
					Label = current.Name,
					Value = current.DamageDealt
				});
			}
		}
		finally
		{
			((global::System.IDisposable)enumerator)?.Dispose();
		}
		return val;
	}

	public List<BarData> GetDetailBars(string playerKey)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		CombatDataCollector.PlayerStats playerStats = default(CombatDataCollector.PlayerStats);
		if (!CombatDataCollector.Players.TryGetValue(playerKey, ref playerStats))
		{
			return new List<BarData>();
		}
		List<BarData> val = new List<BarData>();
		global::System.Collections.Generic.IEnumerator<KeyValuePair<string, int>> enumerator = ((global::System.Collections.Generic.IEnumerable<KeyValuePair<string, int>>)Enumerable.OrderByDescending<KeyValuePair<string, int>, int>((global::System.Collections.Generic.IEnumerable<KeyValuePair<string, int>>)playerStats.DamageByCard, (Func<KeyValuePair<string, int>, int>)((KeyValuePair<string, int> k) => k.Value))).GetEnumerator();
		try
		{
			while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
			{
				KeyValuePair<string, int> current = enumerator.Current;
				val.Add(new BarData
				{
					Key = current.Key,
					Label = CombatDataCollector.ResolveCardName(current.Key),
					Value = current.Value
				});
			}
		}
		finally
		{
			((global::System.IDisposable)enumerator)?.Dispose();
		}
		return val;
	}

	public string GetDetailTitle(string playerKey)
	{
		CombatDataCollector.PlayerStats playerStats = default(CombatDataCollector.PlayerStats);
		if (CombatDataCollector.Players.TryGetValue(playerKey, ref playerStats))
		{
			return playerStats.Name;
		}
		return "";
	}
}
