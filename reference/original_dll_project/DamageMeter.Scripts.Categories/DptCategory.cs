using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DamageMeter.Scripts.Categories;

public class DptCategory : IStatCategory
{
	public string Name => I18n.CatDpt;

	public List<BarData> GetPlayerBars()
	{
		List<BarData> val = new List<BarData>();
		int currentTurn = CombatDataCollector.CurrentTurn;
		global::System.Collections.Generic.IEnumerator<CombatDataCollector.PlayerStats> enumerator = ((global::System.Collections.Generic.IEnumerable<CombatDataCollector.PlayerStats>)Enumerable.OrderByDescending<CombatDataCollector.PlayerStats, int>(CombatDataCollector.Players.Values, (Func<CombatDataCollector.PlayerStats, int>)((CombatDataCollector.PlayerStats p) => p.DamageDealt))).GetEnumerator();
		try
		{
			while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
			{
				CombatDataCollector.PlayerStats current = enumerator.Current;
				int value = ((currentTurn > 0) ? (current.DamageDealt / currentTurn) : 0);
				val.Add(new BarData
				{
					Key = current.Key,
					Label = current.Name,
					Value = value
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
		global::System.Collections.Generic.IEnumerator<KeyValuePair<int, int>> enumerator = ((global::System.Collections.Generic.IEnumerable<KeyValuePair<int, int>>)Enumerable.OrderBy<KeyValuePair<int, int>, int>((global::System.Collections.Generic.IEnumerable<KeyValuePair<int, int>>)playerStats.DamagePerTurn, (Func<KeyValuePair<int, int>, int>)((KeyValuePair<int, int> k) => k.Key))).GetEnumerator();
		try
		{
			while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
			{
				KeyValuePair<int, int> current = enumerator.Current;
				val.Add(new BarData
				{
					Key = current.Key.ToString(),
					Label = $"{I18n.Turn} {current.Key}",
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
