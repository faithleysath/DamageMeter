using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DamageMeter.Scripts.Categories;

public class EnergyCategory : IStatCategory
{
	public string Name => I18n.CatEnergy;

	public List<BarData> GetPlayerBars()
	{
		List<BarData> val = new List<BarData>();
		global::System.Collections.Generic.IEnumerator<CombatDataCollector.PlayerStats> enumerator = ((global::System.Collections.Generic.IEnumerable<CombatDataCollector.PlayerStats>)Enumerable.OrderByDescending<CombatDataCollector.PlayerStats, int>(CombatDataCollector.Players.Values, (Func<CombatDataCollector.PlayerStats, int>)((CombatDataCollector.PlayerStats p) => p.TotalEnergySpent))).GetEnumerator();
		try
		{
			while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
			{
				CombatDataCollector.PlayerStats current = enumerator.Current;
				if (current.TotalEnergySpent > 0)
				{
					val.Add(new BarData
					{
						Key = current.Key,
						Label = current.Name,
						Value = current.TotalEnergySpent
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
		int currentTurn = CombatDataCollector.CurrentTurn;
		int num = default(int);
		for (int i = 1; i <= currentTurn; i++)
		{
			playerStats.EnergyWastedPerTurn.TryGetValue(i, ref num);
			if (num > 0)
			{
				val.Add(new BarData
				{
					Key = $"turn_{i}",
					Label = $"{I18n.Turn} {i}",
					Value = num,
					DisplayText = $"{num} {I18n.Wasted}"
				});
			}
		}
		if (playerStats.TotalEnergyWasted > 0 && val.Count == 0)
		{
			val.Add(new BarData
			{
				Key = "total_wasted",
				Label = I18n.TotalWasted,
				Value = playerStats.TotalEnergyWasted
			});
		}
		return val;
	}

	public string GetDetailTitle(string playerKey)
	{
		CombatDataCollector.PlayerStats playerStats = default(CombatDataCollector.PlayerStats);
		if (CombatDataCollector.Players.TryGetValue(playerKey, ref playerStats))
		{
			return $"{playerStats.Name} — {I18n.Wasted}: {playerStats.TotalEnergyWasted}";
		}
		return "";
	}
}
