using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DamageMeter.Scripts.Categories;

public class OverkillCategory : IStatCategory
{
	public string Name => I18n.CatOverkill;

	public List<BarData> GetPlayerBars()
	{
		List<BarData> val = new List<BarData>();
		global::System.Collections.Generic.IEnumerator<CombatDataCollector.PlayerStats> enumerator = ((global::System.Collections.Generic.IEnumerable<CombatDataCollector.PlayerStats>)Enumerable.OrderByDescending<CombatDataCollector.PlayerStats, int>(CombatDataCollector.Players.Values, (Func<CombatDataCollector.PlayerStats, int>)((CombatDataCollector.PlayerStats p) => p.OverkillDealt))).GetEnumerator();
		try
		{
			while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
			{
				CombatDataCollector.PlayerStats current = enumerator.Current;
				if (current.OverkillDealt > 0)
				{
					val.Add(new BarData
					{
						Key = current.Key,
						Label = current.Name,
						Value = current.OverkillDealt
					});
				}
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
		CombatDataCollector.PlayerStats playerStats = default(CombatDataCollector.PlayerStats);
		if (!CombatDataCollector.Players.TryGetValue(playerKey, ref playerStats))
		{
			return new List<BarData>();
		}
		List<BarData> val = new List<BarData>();
		if (playerStats.DamageDealt > 0)
		{
			val.Add(new BarData
			{
				Key = "effective",
				Label = I18n.CatDamageDealt,
				Value = playerStats.DamageDealt
			});
		}
		if (playerStats.OverkillDealt > 0)
		{
			val.Add(new BarData
			{
				Key = "overkill",
				Label = I18n.CatOverkill,
				Value = playerStats.OverkillDealt
			});
		}
		if (playerStats.BlockedByTarget > 0)
		{
			val.Add(new BarData
			{
				Key = "blocked",
				Label = I18n.Blocked,
				Value = playerStats.BlockedByTarget
			});
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
