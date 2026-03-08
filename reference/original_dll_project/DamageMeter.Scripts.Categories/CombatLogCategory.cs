using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DamageMeter.Scripts.Categories;

public class CombatLogCategory : IStatCategory
{
	public string Name => I18n.CatCombatLog;

	public List<BarData> GetPlayerBars()
	{
		List<BarData> val = new List<BarData>();
		global::System.Collections.Generic.IEnumerator<CombatDataCollector.PlayerStats> enumerator = CombatDataCollector.Players.Values.GetEnumerator();
		try
		{
			while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
			{
				CombatDataCollector.PlayerStats current = enumerator.Current;
				int playerEventCount = CombatDataCollector.GetPlayerEventCount(current.Key);
				val.Add(new BarData
				{
					Key = current.Key,
					Label = current.Name,
					Value = playerEventCount,
					DisplayText = $"{playerEventCount} {I18n.Events}"
				});
			}
		}
		finally
		{
			((global::System.IDisposable)enumerator)?.Dispose();
		}
		return Enumerable.ToList<BarData>((global::System.Collections.Generic.IEnumerable<BarData>)Enumerable.OrderByDescending<BarData, int>((global::System.Collections.Generic.IEnumerable<BarData>)val, (Func<BarData, int>)((BarData b) => b.Value)));
	}

	public List<BarData> GetDetailBars(string playerKey)
	{
		global::System.Collections.Generic.IReadOnlyList<CombatDataCollector.CombatEvent> playerEvents = CombatDataCollector.GetPlayerEvents(playerKey);
		List<BarData> val = new List<BarData>();
		int num = Math.Max(0, ((global::System.Collections.Generic.IReadOnlyCollection<CombatDataCollector.CombatEvent>)playerEvents).Count - 20);
		for (int num2 = ((global::System.Collections.Generic.IReadOnlyCollection<CombatDataCollector.CombatEvent>)playerEvents).Count - 1; num2 >= num; num2--)
		{
			CombatDataCollector.CombatEvent combatEvent = playerEvents[num2];
			CombatDataCollector.CombatEventType type = combatEvent.Type;
			if (1 == 0)
			{
			}
			string text = type switch
			{
				CombatDataCollector.CombatEventType.DamageDealt => $"T{combatEvent.Turn} [+]", 
				CombatDataCollector.CombatEventType.DamageTaken => $"T{combatEvent.Turn} [-]", 
				CombatDataCollector.CombatEventType.BlockGained => $"T{combatEvent.Turn} [B]", 
				CombatDataCollector.CombatEventType.CardPlayed => $"T{combatEvent.Turn} [C]", 
				CombatDataCollector.CombatEventType.PotionUsed => $"T{combatEvent.Turn} [P]", 
				CombatDataCollector.CombatEventType.DebuffApplied => $"T{combatEvent.Turn} [D]", 
				_ => $"T{combatEvent.Turn}", 
			};
			if (1 == 0)
			{
			}
			string text2 = text;
			val.Add(new BarData
			{
				Key = $"evt_{val.Count}",
				Label = text2 + " " + combatEvent.Label,
				Value = Math.Max(1, combatEvent.Value),
				DisplayText = ((combatEvent.Value > 0) ? combatEvent.Value.ToString() : "")
			});
		}
		return val;
	}

	public string GetDetailTitle(string playerKey)
	{
		CombatDataCollector.PlayerStats playerStats = default(CombatDataCollector.PlayerStats);
		if (CombatDataCollector.Players.TryGetValue(playerKey, ref playerStats))
		{
			return playerStats.Name + " — " + I18n.CatCombatLog;
		}
		return I18n.CatCombatLog;
	}
}
