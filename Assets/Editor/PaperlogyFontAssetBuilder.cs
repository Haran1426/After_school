using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

public static class PaperlogyFontAssetBuilder
{
    private const string SourceDirectory = "Assets/Font/Paperlogy-1.000";
    private const string OutputDirectory = "Assets/Font/Fonts/Paperlogy";

    [MenuItem("Tools/After School/Rebuild Paperlogy TMP Fonts")]
    public static void Rebuild()
    {
        Directory.CreateDirectory(OutputDirectory);

        string[] fontPaths =
        {
            "Paperlogy-1Thin.ttf",
            "Paperlogy-2ExtraLight.ttf",
            "Paperlogy-3Light.ttf",
            "Paperlogy-4Regular.ttf",
            "Paperlogy-5Medium.ttf",
            "Paperlogy-6SemiBold.ttf",
            "Paperlogy-7Bold.ttf",
            "Paperlogy-8ExtraBold.ttf",
            "Paperlogy-9Black.ttf"
        };

        string characters = BuildCharacterSet();
        foreach (string fileName in fontPaths)
        {
            string sourcePath = $"{SourceDirectory}/{fileName}";
            Font sourceFont = AssetDatabase.LoadAssetAtPath<Font>(sourcePath);
            if (sourceFont == null)
            {
                Debug.LogWarning($"[PaperlogyFontAssetBuilder] Missing font: {sourcePath}");
                continue;
            }

            string assetName = Path.GetFileNameWithoutExtension(fileName);
            string assetPath = $"{OutputDirectory}/{assetName} Full SDF.asset";
            TMP_FontAsset fontAsset = TMP_FontAsset.CreateFontAsset(
                sourceFont,
                72,
                9,
                GlyphRenderMode.SDFAA,
                4096,
                4096,
                AtlasPopulationMode.Dynamic,
                true);

            fontAsset.name = $"{assetName} Full SDF";
            fontAsset.TryAddCharacters(characters, out string missing);
            AssetDatabase.CreateAsset(fontAsset, assetPath);

            if (!string.IsNullOrEmpty(missing))
                Debug.LogWarning($"[PaperlogyFontAssetBuilder] {assetName} missing {missing.Length} chars.");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[PaperlogyFontAssetBuilder] Paperlogy TMP font assets rebuilt.");
    }

    private static string BuildCharacterSet()
    {
        StringBuilder builder = new StringBuilder(12000);

        AppendRange(builder, 0x0020, 0x007E); // English, digits, punctuation.
        AppendRange(builder, 0x3131, 0x318E); // Hangul compatibility jamo.
        AppendRange(builder, 0xAC00, 0xD7A3); // Modern Korean syllables.

        return builder.ToString();
    }

    private static void AppendRange(StringBuilder builder, int first, int last)
    {
        for (int code = first; code <= last; code++)
            builder.Append((char)code);
    }
}
