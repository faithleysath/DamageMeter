using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DamageMeter.Scripts.Categories;

public class CardFlowCategory : IStatCategory
{
	public string Name => I18n.CatCardFlow;

	public List<BarData> GetPlayerBars()
	{
		List<BarData> val = new List<BarData>();
		global::System.Collections.Generic.IEnumerator<CombatDataCollector.PlayerStats> enumerator = ((global::System.Collections.Generic.IEnumerable<CombatDataCollector.PlayerStats>)Enumerable.OrderByDescending<CombatDataCollector.PlayerStats, int>(CombatDataCollector.Players.Values, (Func<CombatDataCollector.PlayerStats, int>)((CombatDataCollector.PlayerStats p) => p.CardsDrawn))).GetEnumerator();
		try
		{
			while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
			{
				CombatDataCollector.PlayerStats current = enumerator.Current;
				int num = current.CardsDrawn + current.CardsDiscarded + current.CardsExhausted;
				if (num > 0)
				{
					val.Add(new BarData
					{
						Key = current.Key,
						Label = current.Name,
						Value = num,
						DisplayText = $"{current.CardsDrawn}D {current.CardsDiscarded}d {current.CardsExhausted}E"
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
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		CombatDataCollector.PlayerStats playerStats = default(CombatDataCollector.PlayerStats);
		if (!CombatDataCollector.Players.TryGetValue(playerKey, ref playerStats))
		{
			return new List<BarData>();
		}
		List<BarData> val = new List<BarData>();
		if (playerStats.CardsDrawn > 0)
		{
			val.Add(new BarData
			{
				Key = "drawn",
				Label = I18n.Drawn,
				Value = playerStats.CardsDrawn
			});
		}
		if (playerStats.CardsDiscarded > 0)
		{
			val.Add(new BarData
			{
				Key = "discarded",
				Label = I18n.Discarded,
				Value = playerStats.CardsDiscarded
			});
		}
		if (playerStats.CardsExhausted > 0)
		{
			val.Add(new BarData
			{
				Key = "exhausted",
				Label = I18n.Exhausted,
				Value = playerStats.CardsExhausted
			});
		}
		global::System.Collections.Generic.IEnumerator<KeyValuePair<string, int>> enumerator = Enumerable.Take<KeyValuePair<string, int>>((global::System.Collections.Generic.IEnumerable<KeyValuePair<string, int>>)Enumerable.OrderByDescending<KeyValuePair<string, int>, int>((global::System.Collections.Generic.IEnumerable<KeyValuePair<string, int>>)playerStats.ExhaustCount, (Func<KeyValuePair<string, int>, int>)((KeyValuePair<string, int> k) => k.Value)), 5).GetEnumerator();
		try
		{
			while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
			{
				KeyValuePair<string, int> current = enumerator.Current;
				val.Add(new BarData
				{
					Key = "exhaust_" + current.Key,
					Label = "  " + CombatDataCollector.ResolveCardName(current.Key),
					Value = current.Value,
					DisplayText = $"{current.Value}x {I18n.Exhausted}"
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
