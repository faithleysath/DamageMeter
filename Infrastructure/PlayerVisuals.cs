using System.Reflection;
using Godot;

namespace DamageMeterRebuilt.Infrastructure;

internal static class PlayerVisuals
{
    private static readonly Dictionary<string, string> CharacterIconMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["IRONCLAD"] = "铁甲战士",
        ["SILENT"] = "静默猎手",
        ["DEFECT"] = "故障机器人",
        ["NECROBINDER"] = "亡灵契约师",
        ["REGENT"] = "储君"
    };

    private static readonly Dictionary<string, Texture2D?> IconCache = new(StringComparer.OrdinalIgnoreCase);
    private static readonly List<string> PlayerColorOrder = new();
    private static readonly Color[] FallbackColors =
    {
        new Color(0.95f, 0.55f, 0.15f, 1f),
        new Color(0.3f, 0.6f, 1f, 1f),
        new Color(0.3f, 0.85f, 0.4f, 1f),
        new Color(0.7f, 0.4f, 1f, 1f)
    };

    public static Texture2D? LoadCharacterIcon(string? characterId)
    {
        if (string.IsNullOrWhiteSpace(characterId))
        {
            return null;
        }

        if (IconCache.TryGetValue(characterId, out var cached))
        {
            return cached;
        }

        Texture2D? icon = null;
        if (CharacterIconMap.TryGetValue(characterId, out var resourceKey))
        {
            try
            {
                using var stream = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream($"DamageMeterRebuilt.icons.{resourceKey}.png");
                if (stream is not null)
                {
                    using var memory = new MemoryStream();
                    stream.CopyTo(memory);
                    var image = new Image();
                    if (image.LoadPngFromBuffer(memory.ToArray()) == Error.Ok)
                    {
                        icon = ImageTexture.CreateFromImage(image);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAdapter.Error($"Failed to load icon for {characterId}.", ex);
            }
        }

        IconCache[characterId] = icon;
        return icon;
    }

    public static Color GetPlayerColor(string playerKey, Color? preferredColor = null)
    {
        if (preferredColor is { } color && color.A > 0f)
        {
            return color;
        }

        var index = PlayerColorOrder.IndexOf(playerKey);
        if (index < 0)
        {
            PlayerColorOrder.Add(playerKey);
            index = PlayerColorOrder.Count - 1;
        }

        return FallbackColors[index % FallbackColors.Length];
    }
}
