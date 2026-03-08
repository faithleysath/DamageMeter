using System;
using System.Collections;
using System.Collections.Generic;

namespace DamageMeter.Scripts.Categories;

public class CardEfficiencyCategory : IStatCategory
{
	public string Name => I18n.CatEfficiency;

	public List<BarData> GetPlayerBars()
	{
		List<BarData> val = new List<BarData>();
		global::System.Collections.Generic.IEnumerator<CombatDataCollector.PlayerStats> enumerator = CombatDataCollector.Players.Values.GetEnumerator();
		try
		{
			while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
			{
				CombatDataCollector.PlayerStats current = enumerator.Current;
				if (current.TotalEnergySpent > 0)
				{
					float num = (float)current.DamageDealt / (float)current.TotalEnergySpent;
					int num2 = (int)(num * 10f);
					val.Add(new BarData
					{
						Key = current.Key,
						Label = current.Name,
						Value = Math.Max(1, num2),
						DisplayText = $"{num:F1} DMG/E"
					});
				}
			}
		}
		finally
		{
			((global::System.IDisposable)enumerator)?.Dispose();
		}
		val.Sort((Comparison<BarData>)((BarData a, BarData b) => b.Value.CompareTo(a.Value)));
		return val;
	}

	public List<BarData> GetDetailBars(string playerKey)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		CombatDataCollector.PlayerStats playerStats = default(CombatDataCollector.PlayerStats);
		if (!CombatDataCollector.Players.TryGetValue(playerKey, ref playerStats))
		{
			return new List<BarData>();
		}
		List<BarData> val = new List<BarData>();
		Enumerator<string, int> enumerator = playerStats.DamageByCard.GetEnumerator();
		try
		{
			int num = default(int);
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, int> current = enumerator.Current;
				string key = current.Key;
				int value = current.Value;
				playerStats.EnergySpentByCard.TryGetValue(key, ref num);
				if (num > 0)
				{
					float num2 = (float)value / (float)num;
					int num3 = (int)(num2 * 10f);
					val.Add(new BarData
					{
						Key = key,
						Label = CombatDataCollector.ResolveCardName(key),
						Value = Math.Max(1, num3),
						DisplayText = $"{num2:F1} ({value}/{num}E)"
					});
				}
			}
		}
		finally
		{
			((global::System.IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		val.Sort((Comparison<BarData>)((BarData a, BarData b) => b.Value.CompareTo(a.Value)));
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
