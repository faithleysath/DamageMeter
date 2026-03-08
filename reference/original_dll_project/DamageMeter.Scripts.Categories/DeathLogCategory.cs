using System;
using System.Collections;
using System.Collections.Generic;

namespace DamageMeter.Scripts.Categories;

public class DeathLogCategory : IStatCategory
{
	public string Name => I18n.CatDeathLog;

	public List<BarData> GetPlayerBars()
	{
		List<BarData> val = new List<BarData>();
		global::System.Collections.Generic.IReadOnlyCollection<string> deadPlayers = CombatDataCollector.DeadPlayers;
		if (deadPlayers.Count == 0)
		{
			global::System.Collections.Generic.IEnumerator<CombatDataCollector.PlayerStats> enumerator = CombatDataCollector.Players.Values.GetEnumerator();
			try
			{
				while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
				{
					CombatDataCollector.PlayerStats current = enumerator.Current;
					val.Add(new BarData
					{
						Key = current.Key,
						Label = current.Name,
						Value = 0,
						DisplayText = I18n.Alive
					});
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator)?.Dispose();
			}
			return val;
		}
		global::System.Collections.Generic.IEnumerator<CombatDataCollector.PlayerStats> enumerator2 = CombatDataCollector.Players.Values.GetEnumerator();
		try
		{
			while (((global::System.Collections.IEnumerator)enumerator2).MoveNext())
			{
				CombatDataCollector.PlayerStats current2 = enumerator2.Current;
				bool flag = CombatDataCollector.HasDeathLog(current2.Key);
				val.Add(new BarData
				{
					Key = current2.Key,
					Label = current2.Name,
					Value = (flag ? 1 : 0),
					DisplayText = (flag ? I18n.Dead : I18n.Alive)
				});
			}
		}
		finally
		{
			((global::System.IDisposable)enumerator2)?.Dispose();
		}
		return val;
	}

	public List<BarData> GetDetailBars(string playerKey)
	{
		global::System.Collections.Generic.IReadOnlyList<CombatDataCollector.CombatEvent> deathLog = CombatDataCollector.GetDeathLog(playerKey);
		if (((global::System.Collections.Generic.IReadOnlyCollection<CombatDataCollector.CombatEvent>)deathLog).Count == 0)
		{
			return new List<BarData>();
		}
		List<BarData> val = new List<BarData>();
		global::System.Collections.Generic.IEnumerator<CombatDataCollector.CombatEvent> enumerator = ((global::System.Collections.Generic.IEnumerable<CombatDataCollector.CombatEvent>)deathLog).GetEnumerator();
		try
		{
			while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
			{
				CombatDataCollector.CombatEvent current = enumerator.Current;
				CombatDataCollector.CombatEventType type = current.Type;
				if (1 == 0)
				{
				}
				string text = type switch
				{
					CombatDataCollector.CombatEventType.DamageTaken => $"T{current.Turn} [-]", 
					CombatDataCollector.CombatEventType.DamageDealt => $"T{current.Turn} [+]", 
					CombatDataCollector.CombatEventType.BlockGained => $"T{current.Turn} [B]", 
					CombatDataCollector.CombatEventType.CardPlayed => $"T{current.Turn} [C]", 
					CombatDataCollector.CombatEventType.PotionUsed => $"T{current.Turn} [P]", 
					CombatDataCollector.CombatEventType.DebuffApplied => $"T{current.Turn} [D]", 
					_ => $"T{current.Turn}", 
				};
				if (1 == 0)
				{
				}
				string text2 = text;
				val.Add(new BarData
				{
					Key = $"evt_{val.Count}",
					Label = text2 + " " + current.Label,
					Value = Math.Max(1, current.Value),
					DisplayText = ((current.Value > 0) ? current.Value.ToString() : "")
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
			return playerStats.Name + " — " + I18n.DeathLog;
		}
		return I18n.DeathLog;
	}
}
