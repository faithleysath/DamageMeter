using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;

namespace DamageMeter.Scripts;

public static class CombatDataCollector
{
	public class CombatSegment
	{
		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public Dictionary<string, PlayerStats> Players
		{
			[CompilerGenerated]
			get;
		} = new Dictionary<string, PlayerStats>();

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int TurnCount
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public string EncounterKey
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		} = "";
	}

	public class PlayerStats
	{
		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public string Name
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		} = "";

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public string Key
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		} = "";

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public string CharacterId
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		} = "";

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public Color CharacterColor
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		} = new Color(0.95f, 0.55f, 0.15f, 1f);

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int DamageDealt
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int DamageTaken
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int BlockedByTarget
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int OverkillDealt
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int HitCount
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public Dictionary<int, int> DamagePerTurn
		{
			[CompilerGenerated]
			get;
		} = new Dictionary<int, int>();

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public Dictionary<string, int> DamageByCard
		{
			[CompilerGenerated]
			get;
		} = new Dictionary<string, int>();

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public Dictionary<string, int> DamageBySource
		{
			[CompilerGenerated]
			get;
		} = new Dictionary<string, int>();

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int CardsPlayed
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public Dictionary<string, int> CardPlayCount
		{
			[CompilerGenerated]
			get;
		} = new Dictionary<string, int>();

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public Dictionary<CardType, int> CardTypeCount
		{
			[CompilerGenerated]
			get;
		} = new Dictionary<CardType, int>();

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int TotalBlockGained
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public Dictionary<string, int> BlockByCard
		{
			[CompilerGenerated]
			get;
		} = new Dictionary<string, int>();

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int TotalEnergySpent
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int TotalEnergyWasted
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public Dictionary<string, int> EnergySpentByCard
		{
			[CompilerGenerated]
			get;
		} = new Dictionary<string, int>();

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public Dictionary<int, int> EnergyWastedPerTurn
		{
			[CompilerGenerated]
			get;
		} = new Dictionary<int, int>();

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int PotionsUsed
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public Dictionary<string, int> PotionUseCount
		{
			[CompilerGenerated]
			get;
		} = new Dictionary<string, int>();

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public Dictionary<string, int> DebuffsApplied
		{
			[CompilerGenerated]
			get;
		} = new Dictionary<string, int>();

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int CardsDrawn
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int CardsDiscarded
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int CardsExhausted
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public Dictionary<string, int> DrawCount
		{
			[CompilerGenerated]
			get;
		} = new Dictionary<string, int>();

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public Dictionary<string, int> DiscardCount
		{
			[CompilerGenerated]
			get;
		} = new Dictionary<string, int>();

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public Dictionary<string, int> ExhaustCount
		{
			[CompilerGenerated]
			get;
		} = new Dictionary<string, int>();

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int MaxSingleHit
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public string MaxSingleHitCard
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		} = "";

		public PlayerStats Clone()
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			PlayerStats playerStats = new PlayerStats
			{
				Name = Name,
				Key = Key,
				CharacterId = CharacterId,
				CharacterColor = CharacterColor,
				DamageDealt = DamageDealt,
				DamageTaken = DamageTaken,
				BlockedByTarget = BlockedByTarget,
				OverkillDealt = OverkillDealt,
				HitCount = HitCount,
				CardsPlayed = CardsPlayed,
				TotalBlockGained = TotalBlockGained,
				TotalEnergySpent = TotalEnergySpent,
				TotalEnergyWasted = TotalEnergyWasted,
				PotionsUsed = PotionsUsed,
				CardsDrawn = CardsDrawn,
				CardsDiscarded = CardsDiscarded,
				CardsExhausted = CardsExhausted,
				MaxSingleHit = MaxSingleHit,
				MaxSingleHitCard = MaxSingleHitCard
			};
			CopyDict<int>(DamagePerTurn, playerStats.DamagePerTurn);
			CopyDict<string>(DamageByCard, playerStats.DamageByCard);
			CopyDict<string>(DamageBySource, playerStats.DamageBySource);
			CopyDict<string>(CardPlayCount, playerStats.CardPlayCount);
			CopyDict<CardType>(CardTypeCount, playerStats.CardTypeCount);
			CopyDict<string>(BlockByCard, playerStats.BlockByCard);
			CopyDict<string>(EnergySpentByCard, playerStats.EnergySpentByCard);
			CopyDict<int>(EnergyWastedPerTurn, playerStats.EnergyWastedPerTurn);
			CopyDict<string>(PotionUseCount, playerStats.PotionUseCount);
			CopyDict<string>(DebuffsApplied, playerStats.DebuffsApplied);
			CopyDict<string>(DrawCount, playerStats.DrawCount);
			CopyDict<string>(DiscardCount, playerStats.DiscardCount);
			CopyDict<string>(ExhaustCount, playerStats.ExhaustCount);
			return playerStats;
		}

		public void MergeFrom(PlayerStats other)
		{
			DamageDealt += other.DamageDealt;
			DamageTaken += other.DamageTaken;
			BlockedByTarget += other.BlockedByTarget;
			OverkillDealt += other.OverkillDealt;
			HitCount += other.HitCount;
			CardsPlayed += other.CardsPlayed;
			TotalBlockGained += other.TotalBlockGained;
			TotalEnergySpent += other.TotalEnergySpent;
			TotalEnergyWasted += other.TotalEnergyWasted;
			PotionsUsed += other.PotionsUsed;
			CardsDrawn += other.CardsDrawn;
			CardsDiscarded += other.CardsDiscarded;
			CardsExhausted += other.CardsExhausted;
			if (other.MaxSingleHit > MaxSingleHit)
			{
				MaxSingleHit = other.MaxSingleHit;
				MaxSingleHitCard = other.MaxSingleHitCard;
			}
			MergeDict<int>(other.DamagePerTurn, DamagePerTurn);
			MergeDict<string>(other.DamageByCard, DamageByCard);
			MergeDict<string>(other.DamageBySource, DamageBySource);
			MergeDict<string>(other.CardPlayCount, CardPlayCount);
			MergeDict<CardType>(other.CardTypeCount, CardTypeCount);
			MergeDict<string>(other.BlockByCard, BlockByCard);
			MergeDict<string>(other.EnergySpentByCard, EnergySpentByCard);
			MergeDict<int>(other.EnergyWastedPerTurn, EnergyWastedPerTurn);
			MergeDict<string>(other.PotionUseCount, PotionUseCount);
			MergeDict<string>(other.DebuffsApplied, DebuffsApplied);
			MergeDict<string>(other.DrawCount, DrawCount);
			MergeDict<string>(other.DiscardCount, DiscardCount);
			MergeDict<string>(other.ExhaustCount, ExhaustCount);
		}

		private static void CopyDict<TKey>(Dictionary<TKey, int> src, Dictionary<TKey, int> dst) where TKey : notnull
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			Enumerator<TKey, int> enumerator = src.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<TKey, int> current = enumerator.Current;
					dst[current.Key] = current.Value;
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}

		private static void MergeDict<TKey>(Dictionary<TKey, int> src, Dictionary<TKey, int> dst) where TKey : notnull
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			Enumerator<TKey, int> enumerator = src.GetEnumerator();
			try
			{
				int num = default(int);
				while (enumerator.MoveNext())
				{
					KeyValuePair<TKey, int> current = enumerator.Current;
					dst.TryGetValue(current.Key, ref num);
					dst[current.Key] = num + current.Value;
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}
	}

	public enum CombatEventType
	{
		DamageDealt,
		DamageTaken,
		BlockGained,
		CardPlayed,
		PotionUsed,
		DebuffApplied
	}

	public record CombatEvent(CombatEventType Type, int Turn, string PlayerKey, string Label, int Value)
	{
		[CompilerGenerated]
		protected virtual global::System.Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(CombatEvent);
			}
		}

		[CompilerGenerated]
		public override string ToString()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			StringBuilder val = new StringBuilder();
			val.Append("CombatEvent");
			val.Append(" { ");
			if (PrintMembers(val))
			{
				val.Append(' ');
			}
			val.Append('}');
			return ((object)val).ToString();
		}

		[CompilerGenerated]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("Type = ");
			builder.Append(((object)Type/*cast due to .constrained prefix*/).ToString());
			builder.Append(", Turn = ");
			builder.Append(((object)Turn/*cast due to .constrained prefix*/).ToString());
			builder.Append(", PlayerKey = ");
			builder.Append((object)PlayerKey);
			builder.Append(", Label = ");
			builder.Append((object)Label);
			builder.Append(", Value = ");
			builder.Append(((object)Value/*cast due to .constrained prefix*/).ToString());
			return true;
		}

		[CompilerGenerated]
		public virtual bool Equals(CombatEvent? other)
		{
			return (object)this == other || ((object)other != null && EqualityContract == other.EqualityContract && EqualityComparer<CombatEventType>.Default.Equals(Type, other.Type) && EqualityComparer<int>.Default.Equals(Turn, other.Turn) && EqualityComparer<string>.Default.Equals(PlayerKey, other.PlayerKey) && EqualityComparer<string>.Default.Equals(Label, other.Label) && EqualityComparer<int>.Default.Equals(Value, other.Value));
		}
	}

	public const int ViewCurrent = -1;

	public const int ViewOverall = -2;

	private static readonly Dictionary<string, PlayerStats> _players = new Dictionary<string, PlayerStats>();

	private static readonly List<CombatSegment> _segments = new List<CombatSegment>();

	private static bool _isTracking;

	private static int _currentTurn;

	private static int _viewIndex = -1;

	private static string _currentEncounterKey = "";

	private static Dictionary<string, PlayerStats>? _overallCache;

	private static bool _overallDirty = true;

	private static readonly List<CombatEvent> _eventLog = new List<CombatEvent>();

	private static readonly Dictionary<string, List<CombatEvent>> _deathLogs = new Dictionary<string, List<CombatEvent>>();

	private const int MaxEventLogSize = 200;

	[CompilerGenerated]
	[DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
	private static Action? m_StatsChanged;

	public static bool IsTracking => _isTracking;

	public static int SegmentCount => _segments.Count;

	public static int ViewIndex => _viewIndex;

	public static int CurrentTurn
	{
		get
		{
			int viewIndex = _viewIndex;
			if (1 == 0)
			{
			}
			int result = viewIndex switch
			{
				-1 => _currentTurn, 
				-2 => (_segments.Count > 0) ? Enumerable.Max<CombatSegment>((global::System.Collections.Generic.IEnumerable<CombatSegment>)_segments, (Func<CombatSegment, int>)((CombatSegment s) => s.TurnCount)) : _currentTurn, 
				_ => (_viewIndex < 0 || _viewIndex >= _segments.Count) ? _currentTurn : _segments[_viewIndex].TurnCount, 
			};
			if (1 == 0)
			{
			}
			return result;
		}
	}

	public static IReadOnlyDictionary<string, PlayerStats> Players
	{
		get
		{
			int viewIndex = _viewIndex;
			if (1 == 0)
			{
			}
			IReadOnlyDictionary<string, PlayerStats> result = (IReadOnlyDictionary<string, PlayerStats>)(viewIndex switch
			{
				-1 => _players, 
				-2 => GetOverallView(), 
				_ => (_viewIndex < 0 || _viewIndex >= _segments.Count) ? _players : _segments[_viewIndex].Players, 
			});
			if (1 == 0)
			{
			}
			return result;
		}
	}

	public static global::System.Collections.Generic.IReadOnlyCollection<string> DeadPlayers => (global::System.Collections.Generic.IReadOnlyCollection<string>)_deathLogs.Keys;

	public static global::System.Collections.Generic.IReadOnlyList<CombatEvent> EventLog => (global::System.Collections.Generic.IReadOnlyList<CombatEvent>)_eventLog;

	public static event Action StatsChanged
	{
		[CompilerGenerated]
		add
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			Action val = CombatDataCollector.m_StatsChanged;
			Action val2;
			do
			{
				val2 = val;
				Action val3 = (Action)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
				val = Interlocked.CompareExchange<Action>(ref CombatDataCollector.m_StatsChanged, val3, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			Action val = CombatDataCollector.m_StatsChanged;
			Action val2;
			do
			{
				val2 = val;
				Action val3 = (Action)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
				val = Interlocked.CompareExchange<Action>(ref CombatDataCollector.m_StatsChanged, val3, val2);
			}
			while (val != val2);
		}
	}

	public static bool HasDeathLog(string playerKey)
	{
		return _deathLogs.ContainsKey(playerKey);
	}

	public static global::System.Collections.Generic.IReadOnlyList<CombatEvent> GetDeathLog(string playerKey)
	{
		List<CombatEvent> val = default(List<CombatEvent>);
		global::System.Collections.Generic.IReadOnlyList<CombatEvent> result;
		if (!_deathLogs.TryGetValue(playerKey, ref val))
		{
			global::System.Collections.Generic.IReadOnlyList<CombatEvent> readOnlyList = global::System.Array.Empty<CombatEvent>();
			result = readOnlyList;
		}
		else
		{
			global::System.Collections.Generic.IReadOnlyList<CombatEvent> readOnlyList = (global::System.Collections.Generic.IReadOnlyList<CombatEvent>)val;
			result = readOnlyList;
		}
		return result;
	}

	public static global::System.Collections.Generic.IReadOnlyList<CombatEvent> GetPlayerEvents(string playerKey)
	{
		return (global::System.Collections.Generic.IReadOnlyList<CombatEvent>)Enumerable.ToList<CombatEvent>(Enumerable.Where<CombatEvent>((global::System.Collections.Generic.IEnumerable<CombatEvent>)_eventLog, (Func<CombatEvent, bool>)((CombatEvent e) => e.PlayerKey == playerKey)));
	}

	public static int GetPlayerEventCount(string playerKey)
	{
		return Enumerable.Count<CombatEvent>((global::System.Collections.Generic.IEnumerable<CombatEvent>)_eventLog, (Func<CombatEvent, bool>)((CombatEvent e) => e.PlayerKey == playerKey));
	}

	public static void ArchiveAndStartNew(string encounterKey)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (_players.Count > 0 && HasMeaningfulData())
		{
			CombatSegment combatSegment = new CombatSegment
			{
				TurnCount = _currentTurn,
				EncounterKey = _currentEncounterKey
			};
			Enumerator<string, PlayerStats> enumerator = _players.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, PlayerStats> current = enumerator.Current;
					combatSegment.Players[current.Key] = current.Value.Clone();
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			_segments.Add(combatSegment);
			MainFile.Log.Info($"Archived segment #{_segments.Count}: {_currentEncounterKey} ({_currentTurn} turns)", 1);
			DamageMeterSettings.UpdateRecords((global::System.Collections.Generic.IEnumerable<PlayerStats>)_players.Values);
		}
		_players.Clear();
		_eventLog.Clear();
		_deathLogs.Clear();
		_isTracking = true;
		_currentTurn = 1;
		_currentEncounterKey = encounterKey;
		_viewIndex = -1;
		_overallDirty = true;
	}

	public static void StopTracking()
	{
		_isTracking = false;
	}

	public static void ResetAll()
	{
		bool isTracking = _isTracking;
		_players.Clear();
		_segments.Clear();
		_eventLog.Clear();
		_deathLogs.Clear();
		_currentTurn = 1;
		_currentEncounterKey = "";
		_viewIndex = -1;
		_overallCache = null;
		_overallDirty = true;
		_isTracking = isTracking;
		MainFile.Log.Info($"All combat data reset (tracking={isTracking})", 1);
		NotifyChanged();
	}

	public static void CycleViewForward()
	{
		if (_segments.Count == 0)
		{
			_viewIndex = ((_viewIndex == -1) ? (-2) : (-1));
			NotifyChanged();
			return;
		}
		if (_viewIndex == -1)
		{
			_viewIndex = 0;
		}
		else if (_viewIndex == -2)
		{
			_viewIndex = -1;
		}
		else if (_viewIndex >= _segments.Count - 1)
		{
			_viewIndex = -2;
		}
		else
		{
			_viewIndex++;
		}
		NotifyChanged();
	}

	public static void CycleViewBackward()
	{
		if (_segments.Count == 0)
		{
			_viewIndex = ((_viewIndex == -1) ? (-2) : (-1));
			NotifyChanged();
			return;
		}
		if (_viewIndex == -1)
		{
			_viewIndex = -2;
		}
		else if (_viewIndex == -2)
		{
			_viewIndex = _segments.Count - 1;
		}
		else if (_viewIndex <= 0)
		{
			_viewIndex = -1;
		}
		else
		{
			_viewIndex--;
		}
		NotifyChanged();
	}

	public static void SetView(int viewIndex)
	{
		if (viewIndex == -1 || viewIndex == -2 || (viewIndex >= 0 && viewIndex < _segments.Count))
		{
			_viewIndex = viewIndex;
			NotifyChanged();
		}
	}

	public static string GetSegmentLabel(int index)
	{
		if (index < 0 || index >= _segments.Count)
		{
			return "";
		}
		CombatSegment combatSegment = _segments[index];
		string text = ResolveEncounterName(combatSegment.EncounterKey);
		string text2 = $"{I18n.SegmentFight} {index + 1}";
		return string.IsNullOrEmpty(text) ? text2 : (text2 + ": " + text);
	}

	public static string GetViewLabel()
	{
		if (_viewIndex == -1)
		{
			return I18n.SegmentCurrent;
		}
		if (_viewIndex == -2)
		{
			return I18n.SegmentOverall;
		}
		if (_viewIndex >= 0 && _viewIndex < _segments.Count)
		{
			CombatSegment combatSegment = _segments[_viewIndex];
			string text = ResolveEncounterName(combatSegment.EncounterKey);
			string text2 = $"{I18n.SegmentFight} {_viewIndex + 1}";
			return string.IsNullOrEmpty(text) ? text2 : (text2 + ": " + text);
		}
		return "";
	}

	public static void OnTurnStarted(CombatState state)
	{
		if (_isTracking)
		{
			_currentTurn = state.RoundNumber;
		}
	}

	public static void OnTurnEnded(CombatState state)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		if (!_isTracking || (int)state.CurrentSide != 1)
		{
			return;
		}
		global::System.Collections.Generic.IEnumerator<Player> enumerator = ((global::System.Collections.Generic.IEnumerable<Player>)state.Players).GetEnumerator();
		try
		{
			int num = default(int);
			while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
			{
				Player current = enumerator.Current;
				PlayerCombatState playerCombatState = current.PlayerCombatState;
				if (playerCombatState != null)
				{
					int energy = playerCombatState.Energy;
					if (energy > 0)
					{
						PlayerStats orCreate = GetOrCreate(current.Creature);
						orCreate.TotalEnergyWasted += energy;
						orCreate.EnergyWastedPerTurn.TryGetValue(_currentTurn, ref num);
						orCreate.EnergyWastedPerTurn[_currentTurn] = num + energy;
					}
				}
			}
		}
		finally
		{
			((global::System.IDisposable)enumerator)?.Dispose();
		}
		NotifyChanged();
	}

	public static void RecordDamage(Creature? dealer, Creature receiver, DamageResult result, CardModel? cardSource)
	{
		if (!_isTracking)
		{
			return;
		}
		if (dealer != null && dealer.IsPlayer && receiver.IsEnemy)
		{
			PlayerStats orCreate = GetOrCreate(dealer);
			int unblockedDamage = result.UnblockedDamage;
			orCreate.DamageDealt += unblockedDamage;
			orCreate.OverkillDealt += result.OverkillDamage;
			orCreate.BlockedByTarget += result.BlockedDamage;
			orCreate.HitCount++;
			int num = default(int);
			orCreate.DamagePerTurn.TryGetValue(_currentTurn, ref num);
			orCreate.DamagePerTurn[_currentTurn] = num + unblockedDamage;
			string text = ((cardSource != null) ? ((AbstractModel)cardSource).Id.Entry : null) ?? "Other";
			int num2 = default(int);
			orCreate.DamageByCard.TryGetValue(text, ref num2);
			orCreate.DamageByCard[text] = num2 + unblockedDamage;
			if (unblockedDamage > orCreate.MaxSingleHit)
			{
				orCreate.MaxSingleHit = unblockedDamage;
				orCreate.MaxSingleHitCard = text;
			}
			if (unblockedDamage > 0)
			{
				string text2 = ResolveCardName(text);
				LogEvent(new CombatEvent(CombatEventType.DamageDealt, _currentTurn, orCreate.Key, $"{text2} → {unblockedDamage}", unblockedDamage));
			}
		}
		if (receiver.IsPlayer)
		{
			PlayerStats orCreate2 = GetOrCreate(receiver);
			int unblockedDamage2 = result.UnblockedDamage;
			orCreate2.DamageTaken += unblockedDamage2;
			object obj;
			if (dealer == null || !dealer.IsMonster)
			{
				obj = null;
			}
			else
			{
				MonsterModel monster = dealer.Monster;
				obj = ((monster != null) ? ((AbstractModel)monster).Id.Entry : null);
			}
			if (obj == null)
			{
				obj = "Unknown";
			}
			string text3 = (string)obj;
			int num3 = default(int);
			orCreate2.DamageBySource.TryGetValue(text3, ref num3);
			orCreate2.DamageBySource[text3] = num3 + unblockedDamage2;
			if (unblockedDamage2 > 0)
			{
				string text4 = ResolveMonsterName(text3);
				LogEvent(new CombatEvent(CombatEventType.DamageTaken, _currentTurn, orCreate2.Key, $"{text4} → {unblockedDamage2}", unblockedDamage2));
			}
			if (result.WasTargetKilled)
			{
				SnapshotDeathLog(orCreate2.Key);
				MainFile.Log.Info($"Death detected for {orCreate2.Name} at turn {_currentTurn}", 1);
			}
		}
		NotifyChanged();
	}

	public static void RecordCardPlay(CardPlay cardPlay)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		if (!_isTracking)
		{
			return;
		}
		Player owner = cardPlay.Card.Owner;
		Creature val = ((owner != null) ? owner.Creature : null);
		if (val != null && val.IsPlayer)
		{
			PlayerStats orCreate = GetOrCreate(val);
			string entry = ((AbstractModel)cardPlay.Card).Id.Entry;
			CardType type = cardPlay.Card.Type;
			orCreate.CardsPlayed++;
			int num = default(int);
			orCreate.CardPlayCount.TryGetValue(entry, ref num);
			orCreate.CardPlayCount[entry] = num + 1;
			int num2 = default(int);
			orCreate.CardTypeCount.TryGetValue(type, ref num2);
			orCreate.CardTypeCount[type] = num2 + 1;
			ResourceInfo resources = cardPlay.Resources;
			int energySpent = ((ResourceInfo)(ref resources)).EnergySpent;
			orCreate.TotalEnergySpent += energySpent;
			if (energySpent > 0)
			{
				int num3 = default(int);
				orCreate.EnergySpentByCard.TryGetValue(entry, ref num3);
				orCreate.EnergySpentByCard[entry] = num3 + energySpent;
			}
			string label = ResolveCardName(entry);
			LogEvent(new CombatEvent(CombatEventType.CardPlayed, _currentTurn, orCreate.Key, label, energySpent));
			NotifyChanged();
		}
	}

	public static void RecordBlockGained(Creature receiver, int amount, CardPlay? cardPlay)
	{
		if (_isTracking && receiver.IsPlayer && amount > 0)
		{
			PlayerStats orCreate = GetOrCreate(receiver);
			orCreate.TotalBlockGained += amount;
			string text = ((cardPlay != null) ? ((AbstractModel)cardPlay.Card).Id.Entry : null) ?? "Other";
			int num = default(int);
			orCreate.BlockByCard.TryGetValue(text, ref num);
			orCreate.BlockByCard[text] = num + amount;
			string text2 = ResolveCardName(text);
			LogEvent(new CombatEvent(CombatEventType.BlockGained, _currentTurn, orCreate.Key, $"+{amount} ({text2})", amount));
			NotifyChanged();
		}
	}

	public static void RecordPotionUsed(PotionModel potion, Creature? target)
	{
		if (_isTracking)
		{
			Player owner = potion.Owner;
			Creature val = ((owner != null) ? owner.Creature : null);
			if (val != null && val.IsPlayer)
			{
				PlayerStats orCreate = GetOrCreate(val);
				string entry = ((AbstractModel)potion).Id.Entry;
				orCreate.PotionsUsed++;
				int num = default(int);
				orCreate.PotionUseCount.TryGetValue(entry, ref num);
				orCreate.PotionUseCount[entry] = num + 1;
				string label = ResolvePotionName(entry);
				LogEvent(new CombatEvent(CombatEventType.PotionUsed, _currentTurn, orCreate.Key, label, 1));
				NotifyChanged();
			}
		}
	}

	public static void RecordPowerReceived(PowerModel power, decimal amount, Creature? applier)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		if (_isTracking && (int)power.Type == 2 && power.Owner.IsEnemy && applier != null && applier.IsPlayer)
		{
			PlayerStats orCreate = GetOrCreate(applier);
			string entry = ((AbstractModel)power).Id.Entry;
			int num = (int)Math.Max(1m, amount);
			int num2 = default(int);
			orCreate.DebuffsApplied.TryGetValue(entry, ref num2);
			orCreate.DebuffsApplied[entry] = num2 + num;
			string text = ResolvePowerName(entry);
			LogEvent(new CombatEvent(CombatEventType.DebuffApplied, _currentTurn, orCreate.Key, $"{text} x{num}", num));
			NotifyChanged();
		}
	}

	public static void RecordCardDrawn(CardModel card)
	{
		if (_isTracking)
		{
			Player owner = card.Owner;
			Creature val = ((owner != null) ? owner.Creature : null);
			if (val != null && val.IsPlayer)
			{
				PlayerStats orCreate = GetOrCreate(val);
				string entry = ((AbstractModel)card).Id.Entry;
				orCreate.CardsDrawn++;
				int num = default(int);
				orCreate.DrawCount.TryGetValue(entry, ref num);
				orCreate.DrawCount[entry] = num + 1;
				NotifyChanged();
			}
		}
	}

	public static void RecordCardDiscarded(CardModel card)
	{
		if (_isTracking)
		{
			Player owner = card.Owner;
			Creature val = ((owner != null) ? owner.Creature : null);
			if (val != null && val.IsPlayer)
			{
				PlayerStats orCreate = GetOrCreate(val);
				string entry = ((AbstractModel)card).Id.Entry;
				orCreate.CardsDiscarded++;
				int num = default(int);
				orCreate.DiscardCount.TryGetValue(entry, ref num);
				orCreate.DiscardCount[entry] = num + 1;
				NotifyChanged();
			}
		}
	}

	public static void RecordCardExhausted(CardModel card)
	{
		if (_isTracking)
		{
			Player owner = card.Owner;
			Creature val = ((owner != null) ? owner.Creature : null);
			if (val != null && val.IsPlayer)
			{
				PlayerStats orCreate = GetOrCreate(val);
				string entry = ((AbstractModel)card).Id.Entry;
				orCreate.CardsExhausted++;
				int num = default(int);
				orCreate.ExhaustCount.TryGetValue(entry, ref num);
				orCreate.ExhaustCount[entry] = num + 1;
				NotifyChanged();
			}
		}
	}

	private static void LogEvent(CombatEvent evt)
	{
		_eventLog.Add(evt);
		if (_eventLog.Count > 200)
		{
			_eventLog.RemoveAt(0);
		}
	}

	private static void SnapshotDeathLog(string playerKey)
	{
		if (!_deathLogs.ContainsKey(playerKey))
		{
			List<CombatEvent> val = Enumerable.ToList<CombatEvent>(Enumerable.TakeLast<CombatEvent>(Enumerable.Where<CombatEvent>((global::System.Collections.Generic.IEnumerable<CombatEvent>)_eventLog, (Func<CombatEvent, bool>)((CombatEvent e) => e.PlayerKey == playerKey)), 8));
			_deathLogs[playerKey] = val;
		}
	}

	private static string GetPlayerKey(Creature creature)
	{
		Player player = creature.Player;
		return ((player != null) ? player.NetId.ToString() : null) ?? creature.Name;
	}

	public static string ResolveCardName(string key)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		if (key == "Other")
		{
			return key;
		}
		try
		{
			LocString val = new LocString("cards", key + ".title");
			if (val.Exists())
			{
				return val.GetFormattedText();
			}
		}
		catch
		{
		}
		return key;
	}

	public static string ResolvePotionName(string key)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		try
		{
			LocString val = new LocString("potions", key + ".title");
			if (val.Exists())
			{
				return val.GetFormattedText();
			}
		}
		catch
		{
		}
		return key;
	}

	public static string ResolvePowerName(string key)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		try
		{
			LocString val = new LocString("powers", key + ".title");
			if (val.Exists())
			{
				return val.GetFormattedText();
			}
		}
		catch
		{
		}
		return key;
	}

	public static string ResolveMonsterName(string key)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		if (key == "Unknown")
		{
			return key;
		}
		try
		{
			LocString val = new LocString("monsters", key + ".title");
			if (val.Exists())
			{
				return val.GetFormattedText();
			}
		}
		catch
		{
		}
		return key;
	}

	public static string ResolveEncounterName(string key)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		if (string.IsNullOrEmpty(key))
		{
			return "";
		}
		try
		{
			LocString val = new LocString("encounters", key + ".title");
			if (val.Exists())
			{
				return val.GetFormattedText();
			}
		}
		catch
		{
		}
		return key;
	}

	private static bool HasMeaningfulData()
	{
		return Enumerable.Any<PlayerStats>((global::System.Collections.Generic.IEnumerable<PlayerStats>)_players.Values, (Func<PlayerStats, bool>)((PlayerStats p) => p.DamageDealt > 0 || p.DamageTaken > 0 || p.CardsPlayed > 0 || p.TotalBlockGained > 0 || p.PotionsUsed > 0 || p.CardsDrawn > 0));
	}

	private static void NotifyChanged()
	{
		_overallDirty = true;
		Action? statsChanged = CombatDataCollector.StatsChanged;
		if (statsChanged != null)
		{
			statsChanged.Invoke();
		}
	}

	private static IReadOnlyDictionary<string, PlayerStats> GetOverallView()
	{
		if (_overallCache == null || _overallDirty)
		{
			_overallCache = BuildOverallView();
			_overallDirty = false;
		}
		return (IReadOnlyDictionary<string, PlayerStats>)(object)_overallCache;
	}

	private static Dictionary<string, PlayerStats> BuildOverallView()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<string, PlayerStats> val = new Dictionary<string, PlayerStats>();
		Enumerator<CombatSegment> enumerator = _segments.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				CombatSegment current = enumerator.Current;
				MergePlayersInto(current.Players, val);
			}
		}
		finally
		{
			((global::System.IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		MergePlayersInto(_players, val);
		return val;
	}

	private static void MergePlayersInto(Dictionary<string, PlayerStats> source, Dictionary<string, PlayerStats> target)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<string, PlayerStats> enumerator = source.GetEnumerator();
		try
		{
			PlayerStats playerStats = default(PlayerStats);
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, PlayerStats> current = enumerator.Current;
				if (!target.TryGetValue(current.Key, ref playerStats))
				{
					playerStats = new PlayerStats
					{
						Name = current.Value.Name,
						Key = current.Value.Key,
						CharacterId = current.Value.CharacterId,
						CharacterColor = current.Value.CharacterColor
					};
					target[current.Key] = playerStats;
				}
				playerStats.MergeFrom(current.Value);
			}
		}
		finally
		{
			((global::System.IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private static PlayerStats GetOrCreate(Creature creature)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		string playerKey = GetPlayerKey(creature);
		PlayerStats playerStats = default(PlayerStats);
		if (!_players.TryGetValue(playerKey, ref playerStats))
		{
			string playerDisplayName = GetPlayerDisplayName(creature);
			Player player = creature.Player;
			object obj;
			if (player == null)
			{
				obj = null;
			}
			else
			{
				CharacterModel character = player.Character;
				obj = ((character != null) ? ((AbstractModel)character).Id.Entry : null);
			}
			if (obj == null)
			{
				obj = "";
			}
			string characterId = (string)obj;
			Player player2 = creature.Player;
			Color? obj2;
			if (player2 == null)
			{
				obj2 = null;
			}
			else
			{
				CharacterModel character2 = player2.Character;
				obj2 = ((character2 != null) ? new Color?(character2.NameColor) : ((Color?)null));
			}
			Color characterColor = (Color)(((_003F?)obj2) ?? new Color(0.95f, 0.55f, 0.15f, 1f));
			playerStats = new PlayerStats
			{
				Name = playerDisplayName,
				Key = playerKey,
				CharacterId = characterId,
				CharacterColor = characterColor
			};
			_players[playerKey] = playerStats;
		}
		return playerStats;
	}

	private static string GetPlayerDisplayName(Creature creature)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Player player = creature.Player;
			if (player != null)
			{
				RunManager instance = RunManager.Instance;
				PlatformType? obj;
				if (instance == null)
				{
					obj = null;
				}
				else
				{
					INetGameService netService = instance.NetService;
					obj = ((netService != null) ? new PlatformType?(netService.Platform) : ((PlatformType?)null));
				}
				PlatformType? val = obj;
				if (val.HasValue)
				{
					ulong num = (RunManager.Instance.IsSinglePlayerOrFakeMultiplayer ? PlatformUtil.GetLocalPlayerId(val.Value) : player.NetId);
					string playerName = PlatformUtil.GetPlayerName(val.Value, num);
					if (!string.IsNullOrEmpty(playerName))
					{
						return playerName;
					}
				}
			}
			return creature.Name;
		}
		catch
		{
			Player player2 = creature.Player;
			object obj3;
			if (player2 == null)
			{
				obj3 = null;
			}
			else
			{
				CharacterModel character = player2.Character;
				obj3 = ((character != null) ? ((AbstractModel)character).Id.Entry : null);
			}
			if (obj3 == null)
			{
				obj3 = "Unknown";
			}
			return (string)obj3;
		}
	}
}
