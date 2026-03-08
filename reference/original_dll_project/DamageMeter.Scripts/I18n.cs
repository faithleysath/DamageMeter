using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using Godot;
using MegaCrit.Sts2.Core.Localization;

namespace DamageMeter.Scripts;

public static class I18n
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static LocaleChangeCallback _003C0_003E__OnLocaleChanged;
	}

	[CompilerGenerated]
	private sealed class _003CGetLanguageCandidates_003Ed__108 : global::System.Collections.Generic.IEnumerable<string>, global::System.Collections.IEnumerable, global::System.Collections.Generic.IEnumerator<string>, global::System.Collections.IEnumerator, global::System.IDisposable
	{
		private int _003C_003E1__state;

		private string _003C_003E2__current;

		private int _003C_003El__initialThreadId;

		private string language;

		public string _003C_003E3__language;

		private HashSet<string> _003Cseen_003E5__1;

		private int _003Csep_003E5__2;

		private string _003Cprefix_003E5__3;

		string global::System.Collections.Generic.IEnumerator<string>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		object global::System.Collections.IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CGetLanguageCandidates_003Ed__108(int _003C_003E1__state)
		{
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		[DebuggerHidden]
		void global::System.IDisposable.Dispose()
		{
			_003Cseen_003E5__1 = null;
			_003Cprefix_003E5__3 = null;
			_003C_003E1__state = -2;
		}

		private bool MoveNext()
		{
			switch (_003C_003E1__state)
			{
			default:
				return false;
			case 0:
				_003C_003E1__state = -1;
				_003Cseen_003E5__1 = new HashSet<string>((IEqualityComparer<string>)(object)StringComparer.OrdinalIgnoreCase);
				if (_003Cseen_003E5__1.Add(language))
				{
					_003C_003E2__current = language;
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_008a;
			case 1:
				_003C_003E1__state = -1;
				goto IL_008a;
			case 2:
				_003C_003E1__state = -1;
				goto IL_00f4;
			case 3:
				_003C_003E1__state = -1;
				goto IL_0146;
			case 4:
				_003C_003E1__state = -1;
				goto IL_0190;
			case 5:
				{
					_003C_003E1__state = -1;
					break;
				}
				IL_0190:
				if (_003Cseen_003E5__1.Add("en"))
				{
					_003C_003E2__current = "en";
					_003C_003E1__state = 5;
					return true;
				}
				break;
				IL_008a:
				_003Csep_003E5__2 = language.IndexOf('_');
				if (_003Csep_003E5__2 > 0)
				{
					_003Cprefix_003E5__3 = language.Substring(0, _003Csep_003E5__2);
					if (_003Cseen_003E5__1.Add(_003Cprefix_003E5__3))
					{
						_003C_003E2__current = _003Cprefix_003E5__3;
						_003C_003E1__state = 2;
						return true;
					}
					goto IL_00f4;
				}
				goto IL_00fc;
				IL_0146:
				if (language.StartsWith("en", (StringComparison)5) && _003Cseen_003E5__1.Add("en"))
				{
					_003C_003E2__current = "en";
					_003C_003E1__state = 4;
					return true;
				}
				goto IL_0190;
				IL_00f4:
				_003Cprefix_003E5__3 = null;
				goto IL_00fc;
				IL_00fc:
				if (language.StartsWith("zh", (StringComparison)5) && _003Cseen_003E5__1.Add("zhs"))
				{
					_003C_003E2__current = "zhs";
					_003C_003E1__state = 3;
					return true;
				}
				goto IL_0146;
			}
			return false;
		}

		bool global::System.Collections.IEnumerator.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			return this.MoveNext();
		}

		[DebuggerHidden]
		void global::System.Collections.IEnumerator.Reset()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			throw new NotSupportedException();
		}

		[DebuggerHidden]
		global::System.Collections.Generic.IEnumerator<string> global::System.Collections.Generic.IEnumerable<string>.GetEnumerator()
		{
			_003CGetLanguageCandidates_003Ed__108 _003CGetLanguageCandidates_003Ed__;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = 0;
				_003CGetLanguageCandidates_003Ed__ = this;
			}
			else
			{
				_003CGetLanguageCandidates_003Ed__ = new _003CGetLanguageCandidates_003Ed__108(0);
			}
			_003CGetLanguageCandidates_003Ed__.language = _003C_003E3__language;
			return _003CGetLanguageCandidates_003Ed__;
		}

		[DebuggerHidden]
		global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
		{
			return (global::System.Collections.IEnumerator)((global::System.Collections.Generic.IEnumerable<string>)this).GetEnumerator();
		}
	}

	private const string DefaultLanguage = "en";

	private const string ResourcePrefix = "DamageMeter.localization.";

	private static Dictionary<string, string> _translations = new Dictionary<string, string>((IEqualityComparer<string>)(object)StringComparer.OrdinalIgnoreCase);

	private static string? _loadedLanguage;

	private static bool _subscribed;

	[CompilerGenerated]
	[DebuggerBrowsable(/*Could not decode attribute arguments.*/)]
	private static Action? m_Changed;

	public static string CatDamageDealt => Get("cat_damage_dealt", "Damage Dealt");

	public static string CatDamageTaken => Get("cat_damage_taken", "Damage Taken");

	public static string CatDpt => Get("cat_dpt", "DPT");

	public static string CatCardUsage => Get("cat_card_usage", "Cards Played");

	public static string CatBlock => Get("cat_block", "Block");

	public static string CatEnergy => Get("cat_energy", "Energy");

	public static string CatEfficiency => Get("cat_efficiency", "DMG/Energy");

	public static string CatOverkill => Get("cat_overkill", "Overkill");

	public static string CatPotions => Get("cat_potions", "Potions");

	public static string CatDebuffs => Get("cat_debuffs", "Debuffs Applied");

	public static string Turn => Get("turn", "Turn");

	public static string Wasted => Get("wasted", "wasted");

	public static string TotalWasted => Get("total_wasted", "Total Wasted");

	public static string Blocked => Get("blocked", "Blocked");

	public static string WaitingForCombat => Get("waiting", "Waiting...");

	public static string SegmentCurrent => Get("segment_current", "Current");

	public static string SegmentOverall => Get("segment_overall", "Overall");

	public static string SegmentFight => Get("segment_fight", "Fight");

	public static string CatDeathLog => Get("cat_death_log", "Death Log");

	public static string CatCardFlow => Get("cat_card_flow", "Card Flow");

	public static string DeathLog => Get("death_log", "Death Log");

	public static string Dead => Get("dead", "DEAD");

	public static string Alive => Get("alive", "Alive");

	public static string Drawn => Get("drawn", "Drawn");

	public static string Discarded => Get("discarded", "Discarded");

	public static string Exhausted => Get("exhausted", "Exhausted");

	public static string CatCombatLog => Get("cat_combat_log", "Combat Log");

	public static string CatRecords => Get("cat_records", "Records");

	public static string Events => Get("events", "events");

	public static string RecHighestHit => Get("rec_highest_hit", "Highest Hit");

	public static string RecMostFightDmg => Get("rec_most_fight_dmg", "Most Fight DMG");

	public static string RecBestTurnDmg => Get("rec_best_turn_dmg", "Best Turn DMG");

	public static string RecMostCards => Get("rec_most_cards", "Most Cards Played");

	public static string RecMostBlock => Get("rec_most_block", "Most Block Gained");

	public static string RecTotalDamage => Get("rec_total_damage", "Total Damage");

	public static string RecTotalFights => Get("rec_total_fights", "Fights Completed");

	public static string RecNoRecords => Get("rec_no_records", "No records yet");

	public static string Settings => Get("settings", "Settings");

	public static string SettingsScale => Get("settings_scale", "Scale");

	public static string SettingsOpacity => Get("settings_opacity", "Opacity");

	public static string SettingsMaxBars => Get("settings_max_bars", "Max Bars");

	public static string SettingsResetPos => Get("settings_reset_pos", "Reset Position");

	public static string ResetData => Get("reset_data", "Reset Data");

	public static string SettingsAutoReset => Get("settings_auto_reset", "Auto-reset on new run");

	public static string Dashboard => Get("dashboard", "Dashboard");

	public static event Action Changed
	{
		[CompilerGenerated]
		add
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			Action val = I18n.m_Changed;
			Action val2;
			do
			{
				val2 = val;
				Action val3 = (Action)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
				val = Interlocked.CompareExchange<Action>(ref I18n.m_Changed, val3, val2);
			}
			while (val != val2);
		}
		[CompilerGenerated]
		remove
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			Action val = I18n.m_Changed;
			Action val2;
			do
			{
				val2 = val;
				Action val3 = (Action)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
				val = Interlocked.CompareExchange<Action>(ref I18n.m_Changed, val3, val2);
			}
			while (val != val2);
		}
	}

	public static string Get(string key, string fallback)
	{
		EnsureLoaded();
		return CollectionExtensions.GetValueOrDefault<string, string>((IReadOnlyDictionary<string, string>)(object)_translations, key) ?? fallback;
	}

	public static void Initialize()
	{
		ForceReload();
		TrySubscribe();
	}

	private static void TrySubscribe()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		if (_subscribed)
		{
			return;
		}
		try
		{
			LocManager instance = LocManager.Instance;
			if (instance != null)
			{
				object obj = _003C_003EO._003C0_003E__OnLocaleChanged;
				if (obj == null)
				{
					LocaleChangeCallback val = OnLocaleChanged;
					_003C_003EO._003C0_003E__OnLocaleChanged = val;
					obj = (object)val;
				}
				instance.SubscribeToLocaleChange((LocaleChangeCallback)obj);
				_subscribed = true;
				MainFile.Log.Info("Subscribed to LocManager locale change", 1);
			}
			else
			{
				MainFile.Log.Info("LocManager.Instance is null, will use lazy language detection", 1);
			}
		}
		catch (global::System.Exception ex)
		{
			MainFile.Log.Error("Failed to subscribe to locale change: " + ex.Message, 1);
		}
	}

	private static void OnLocaleChanged()
	{
		string text = ResolveLanguage();
		MainFile.Log.Info("Locale changed callback fired, resolved language: " + text, 1);
		_loadedLanguage = null;
		ForceReload();
		Action? changed = I18n.Changed;
		if (changed != null)
		{
			changed.Invoke();
		}
	}

	private static void EnsureLoaded()
	{
		if (!_subscribed)
		{
			TrySubscribe();
		}
		string text = ResolveLanguage();
		if (!string.Equals(_loadedLanguage, text, (StringComparison)5))
		{
			_translations = LoadTranslations(text);
			_loadedLanguage = text;
			MainFile.Log.Info($"Lazy-loaded localization: {text} ({_translations.Count} keys)", 1);
		}
	}

	private static void ForceReload()
	{
		string text = ResolveLanguage();
		_translations = LoadTranslations(text);
		_loadedLanguage = text;
		MainFile.Log.Info($"Localization loaded: {text} ({_translations.Count} keys)", 1);
	}

	private static Dictionary<string, string> LoadTranslations(string language)
	{
		global::System.Collections.Generic.IEnumerator<string> enumerator = GetLanguageCandidates(language).GetEnumerator();
		try
		{
			while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
			{
				string current = enumerator.Current;
				Dictionary<string, string> val = TryLoadEmbedded(current);
				if (val != null && val.Count > 0)
				{
					return val;
				}
				string path = "res://DamageMeter/localization/" + current + ".json";
				val = TryLoadFromPck(path);
				if (val != null && val.Count > 0)
				{
					return val;
				}
			}
		}
		finally
		{
			((global::System.IDisposable)enumerator)?.Dispose();
		}
		return new Dictionary<string, string>((IEqualityComparer<string>)(object)StringComparer.OrdinalIgnoreCase);
	}

	private static Dictionary<string, string>? TryLoadEmbedded(string language)
	{
		//IL_0038: Expected O, but got Unknown
		string text = "DamageMeter.localization." + language + ".json";
		Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(text);
		try
		{
			if (manifestResourceStream == null)
			{
				return null;
			}
			try
			{
				return JsonSerializer.Deserialize<Dictionary<string, string>>(manifestResourceStream, (JsonSerializerOptions)null);
			}
			catch (JsonException ex)
			{
				JsonException ex2 = ex;
				MainFile.Log.Error("Failed to parse embedded localization '" + text + "': " + ((global::System.Exception)(object)ex2).Message, 1);
				return null;
			}
		}
		finally
		{
			((global::System.IDisposable)manifestResourceStream)?.Dispose();
		}
	}

	private static Dictionary<string, string>? TryLoadFromPck(string path)
	{
		//IL_003b: Expected O, but got Unknown
		if (!FileAccess.FileExists(path))
		{
			return null;
		}
		FileAccess val = FileAccess.Open(path, (ModeFlags)1);
		try
		{
			if (val == null)
			{
				return null;
			}
			try
			{
				return JsonSerializer.Deserialize<Dictionary<string, string>>(val.GetAsText(false), (JsonSerializerOptions)null);
			}
			catch (JsonException ex)
			{
				JsonException ex2 = ex;
				MainFile.Log.Error("Failed to parse localization file '" + path + "': " + ((global::System.Exception)(object)ex2).Message, 1);
				return null;
			}
		}
		finally
		{
			((global::System.IDisposable)val)?.Dispose();
		}
	}

	private static string ResolveLanguage()
	{
		string text = null;
		try
		{
			LocManager instance = LocManager.Instance;
			text = ((instance != null) ? instance.Language : null);
		}
		catch
		{
		}
		if (string.IsNullOrWhiteSpace(text))
		{
			try
			{
				text = TranslationServer.GetLocale();
			}
			catch
			{
			}
		}
		return NormalizeLanguageCode(text);
	}

	[IteratorStateMachine(typeof(_003CGetLanguageCandidates_003Ed__108))]
	private static global::System.Collections.Generic.IEnumerable<string> GetLanguageCandidates(string language)
	{
		HashSet<string> seen = new HashSet<string>((IEqualityComparer<string>)(object)StringComparer.OrdinalIgnoreCase);
		if (seen.Add(language))
		{
			yield return language;
		}
		int sep = language.IndexOf('_');
		if (sep > 0)
		{
			string prefix = language.Substring(0, sep);
			if (seen.Add(prefix))
			{
				yield return prefix;
			}
		}
		if (language.StartsWith("zh", (StringComparison)5) && seen.Add("zhs"))
		{
			yield return "zhs";
		}
		if (language.StartsWith("en", (StringComparison)5) && seen.Add("en"))
		{
			yield return "en";
		}
		if (seen.Add("en"))
		{
			yield return "en";
		}
	}

	private static string NormalizeLanguageCode(string? language)
	{
		if (string.IsNullOrWhiteSpace(language))
		{
			return "en";
		}
		string text = language.Trim().Replace('-', '_').ToLowerInvariant();
		if (1 == 0)
		{
		}
		string result = ((!(text == "zh_cn") && !(text == "zh_hans") && !(text == "zh_sg") && !(text == "zh")) ? ((!(text == "en_us") && !(text == "en_gb")) ? text : "en") : "zhs");
		if (1 == 0)
		{
		}
		return result;
	}
}
