using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Godot;

namespace DamageMeter.Scripts;

public static class DamageMeterSettings
{
	public class PersonalRecords
	{
		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int HighestHit
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public string HighestHitCard
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		} = "";

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int MostFightDamage
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int BestTurnDamage
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int MostCardsPlayed
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int MostBlockGained
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public long TotalDamage
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}

		[field: CompilerGenerated]
		[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
		public int TotalFights
		{
			[CompilerGenerated]
			get;
			[CompilerGenerated]
			set;
		}
	}

	private const string SettingsPath = "user://DamageMeter_settings.json";

	[field: CompilerGenerated]
	[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
	public static float PanelX
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = 0f / 0f;

	[field: CompilerGenerated]
	[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
	public static float PanelY
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = 0f / 0f;

	[field: CompilerGenerated]
	[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
	public static float Scale
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = 1f;

	[field: CompilerGenerated]
	[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
	public static float Opacity
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = 0.88f;

	[field: CompilerGenerated]
	[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
	public static int MaxBars
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = 10;

	[field: CompilerGenerated]
	[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
	public static bool AutoResetOnNewRun
	{
		[CompilerGenerated]
		get;
		[CompilerGenerated]
		set;
	} = true;

	[field: CompilerGenerated]
	[field: DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
	public static PersonalRecords Records
	{
		[CompilerGenerated]
		get;
	} = new PersonalRecords();

	public static bool HasSavedPosition => !float.IsNaN(PanelX) && !float.IsNaN(PanelY);

	public static void UpdateRecords(global::System.Collections.Generic.IEnumerable<CombatDataCollector.PlayerStats> allPlayers)
	{
		int num = 0;
		global::System.Collections.Generic.IEnumerator<CombatDataCollector.PlayerStats> enumerator = allPlayers.GetEnumerator();
		try
		{
			while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
			{
				CombatDataCollector.PlayerStats current = enumerator.Current;
				num += current.DamageDealt;
				if (current.MaxSingleHit > Records.HighestHit)
				{
					Records.HighestHit = current.MaxSingleHit;
					Records.HighestHitCard = current.MaxSingleHitCard;
				}
				int num2 = Enumerable.Max(Enumerable.DefaultIfEmpty<int>((global::System.Collections.Generic.IEnumerable<int>)current.DamagePerTurn.Values, 0));
				if (num2 > Records.BestTurnDamage)
				{
					Records.BestTurnDamage = num2;
				}
				if (current.CardsPlayed > Records.MostCardsPlayed)
				{
					Records.MostCardsPlayed = current.CardsPlayed;
				}
				if (current.TotalBlockGained > Records.MostBlockGained)
				{
					Records.MostBlockGained = current.TotalBlockGained;
				}
			}
		}
		finally
		{
			((global::System.IDisposable)enumerator)?.Dispose();
		}
		if (num > Records.MostFightDamage)
		{
			Records.MostFightDamage = num;
		}
		Records.TotalDamage += num;
		Records.TotalFights++;
		Save();
	}

	public static void Load()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Invalid comparison between Unknown and I4
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Invalid comparison between Unknown and I4
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Invalid comparison between Unknown and I4
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Invalid comparison between Unknown and I4
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Invalid comparison between Unknown and I4
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Invalid comparison between Unknown and I4
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Invalid comparison between Unknown and I4
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Invalid comparison between Unknown and I4
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Invalid comparison between Unknown and I4
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Invalid comparison between Unknown and I4
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Invalid comparison between Unknown and I4
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Invalid comparison between Unknown and I4
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Invalid comparison between Unknown and I4
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Invalid comparison between Unknown and I4
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Invalid comparison between Unknown and I4
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Invalid comparison between Unknown and I4
		try
		{
			if (!FileAccess.FileExists("user://DamageMeter_settings.json"))
			{
				return;
			}
			FileAccess val = FileAccess.Open("user://DamageMeter_settings.json", (ModeFlags)1);
			try
			{
				if (val == null)
				{
					return;
				}
				JsonElement val2 = JsonSerializer.Deserialize<JsonElement>(val.GetAsText(false), (JsonSerializerOptions)null);
				if ((int)((JsonElement)(ref val2)).ValueKind != 1)
				{
					return;
				}
				JsonElement val3 = default(JsonElement);
				if (((JsonElement)(ref val2)).TryGetProperty("panel_x", ref val3) && (int)((JsonElement)(ref val3)).ValueKind == 4)
				{
					PanelX = ((JsonElement)(ref val3)).GetSingle();
				}
				JsonElement val4 = default(JsonElement);
				if (((JsonElement)(ref val2)).TryGetProperty("panel_y", ref val4) && (int)((JsonElement)(ref val4)).ValueKind == 4)
				{
					PanelY = ((JsonElement)(ref val4)).GetSingle();
				}
				JsonElement val5 = default(JsonElement);
				if (((JsonElement)(ref val2)).TryGetProperty("scale", ref val5) && (int)((JsonElement)(ref val5)).ValueKind == 4)
				{
					Scale = Math.Clamp(((JsonElement)(ref val5)).GetSingle(), 0.5f, 2f);
				}
				JsonElement val6 = default(JsonElement);
				if (((JsonElement)(ref val2)).TryGetProperty("opacity", ref val6) && (int)((JsonElement)(ref val6)).ValueKind == 4)
				{
					Opacity = Math.Clamp(((JsonElement)(ref val6)).GetSingle(), 0.3f, 1f);
				}
				JsonElement val7 = default(JsonElement);
				if (((JsonElement)(ref val2)).TryGetProperty("max_bars", ref val7) && (int)((JsonElement)(ref val7)).ValueKind == 4)
				{
					MaxBars = Math.Clamp(((JsonElement)(ref val7)).GetInt32(), 3, 30);
				}
				JsonElement val8 = default(JsonElement);
				if (((JsonElement)(ref val2)).TryGetProperty("auto_reset", ref val8))
				{
					AutoResetOnNewRun = (int)((JsonElement)(ref val8)).ValueKind != 6;
				}
				JsonElement val9 = default(JsonElement);
				if (((JsonElement)(ref val2)).TryGetProperty("records", ref val9) && (int)((JsonElement)(ref val9)).ValueKind == 1)
				{
					JsonElement val10 = default(JsonElement);
					if (((JsonElement)(ref val9)).TryGetProperty("highest_hit", ref val10) && (int)((JsonElement)(ref val10)).ValueKind == 4)
					{
						Records.HighestHit = ((JsonElement)(ref val10)).GetInt32();
					}
					JsonElement val11 = default(JsonElement);
					if (((JsonElement)(ref val9)).TryGetProperty("highest_hit_card", ref val11) && (int)((JsonElement)(ref val11)).ValueKind == 3)
					{
						Records.HighestHitCard = ((JsonElement)(ref val11)).GetString() ?? "";
					}
					JsonElement val12 = default(JsonElement);
					if (((JsonElement)(ref val9)).TryGetProperty("most_fight_damage", ref val12) && (int)((JsonElement)(ref val12)).ValueKind == 4)
					{
						Records.MostFightDamage = ((JsonElement)(ref val12)).GetInt32();
					}
					JsonElement val13 = default(JsonElement);
					if (((JsonElement)(ref val9)).TryGetProperty("best_turn_damage", ref val13) && (int)((JsonElement)(ref val13)).ValueKind == 4)
					{
						Records.BestTurnDamage = ((JsonElement)(ref val13)).GetInt32();
					}
					JsonElement val14 = default(JsonElement);
					if (((JsonElement)(ref val9)).TryGetProperty("most_cards_played", ref val14) && (int)((JsonElement)(ref val14)).ValueKind == 4)
					{
						Records.MostCardsPlayed = ((JsonElement)(ref val14)).GetInt32();
					}
					JsonElement val15 = default(JsonElement);
					if (((JsonElement)(ref val9)).TryGetProperty("most_block_gained", ref val15) && (int)((JsonElement)(ref val15)).ValueKind == 4)
					{
						Records.MostBlockGained = ((JsonElement)(ref val15)).GetInt32();
					}
					JsonElement val16 = default(JsonElement);
					if (((JsonElement)(ref val9)).TryGetProperty("total_damage", ref val16) && (int)((JsonElement)(ref val16)).ValueKind == 4)
					{
						Records.TotalDamage = ((JsonElement)(ref val16)).GetInt64();
					}
					JsonElement val17 = default(JsonElement);
					if (((JsonElement)(ref val9)).TryGetProperty("total_fights", ref val17) && (int)((JsonElement)(ref val17)).ValueKind == 4)
					{
						Records.TotalFights = ((JsonElement)(ref val17)).GetInt32();
					}
				}
			}
			finally
			{
				((global::System.IDisposable)val)?.Dispose();
			}
		}
		catch (global::System.Exception ex)
		{
			MainFile.Log.Error("Failed to load settings: " + ex.Message, 1);
		}
	}

	public static void Save()
	{
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Expected O, but got Unknown
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Dictionary<string, object> val = new Dictionary<string, object>();
			if (!float.IsNaN(PanelX))
			{
				val["panel_x"] = PanelX;
			}
			if (!float.IsNaN(PanelY))
			{
				val["panel_y"] = PanelY;
			}
			val["scale"] = Scale;
			val["opacity"] = Opacity;
			val["max_bars"] = MaxBars;
			val["auto_reset"] = AutoResetOnNewRun;
			Dictionary<string, object> val2 = new Dictionary<string, object>
			{
				["highest_hit"] = Records.HighestHit,
				["highest_hit_card"] = Records.HighestHitCard,
				["most_fight_damage"] = Records.MostFightDamage,
				["best_turn_damage"] = Records.BestTurnDamage,
				["most_cards_played"] = Records.MostCardsPlayed,
				["most_block_gained"] = Records.MostBlockGained,
				["total_damage"] = Records.TotalDamage,
				["total_fights"] = Records.TotalFights
			};
			val["records"] = val2;
			FileAccess val3 = FileAccess.Open("user://DamageMeter_settings.json", (ModeFlags)2);
			try
			{
				if (val3 == null)
				{
					MainFile.Log.Error($"Failed to open settings for writing: {FileAccess.GetOpenError()}", 1);
				}
				else
				{
					val3.StoreString(JsonSerializer.Serialize<Dictionary<string, object>>(val, new JsonSerializerOptions
					{
						WriteIndented = true
					}));
				}
			}
			finally
			{
				((global::System.IDisposable)val3)?.Dispose();
			}
		}
		catch (global::System.Exception ex)
		{
			MainFile.Log.Error("Failed to save settings: " + ex.Message, 1);
		}
	}
}
