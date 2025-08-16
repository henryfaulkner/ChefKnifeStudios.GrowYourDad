using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Collections.Generic;
using Godot;
using FileAccess = Godot.FileAccess;
using System;
using System.Linq;

public static class ThemeHelper
{
	private static readonly string ThemesFilePath = "res://Core/Themes/themes.json";

	public static Theme? GetThemeByTileSetSourceId(int tileSetSourceId)
	{
		var themes = LoadThemes();
		foreach (var theme in themes) GD.Print(theme.TileSetSourceId);
		return themes.FirstOrDefault(x => x.TileSetSourceId == tileSetSourceId);
	}

	public static List<Theme> LoadThemes()
	{
		var file = FileAccess.Open(ThemesFilePath, FileAccess.ModeFlags.Read);
		if (file == null)
		{
			GD.PrintErr($"Failed to open themes file at {ThemesFilePath}");
			return new List<Theme>();
		}

		string json = file.GetAsText();
		var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,  // Ensures case-insensitive matching
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(), // Required for polymorphic deserialization
			WriteIndented = true
		};
		return JsonSerializer.Deserialize<List<Theme>>(json, options) ?? new List<Theme>();
	}

	public static void SetShaderTheme(Theme theme, ShaderMaterial material)
	{
		material.SetShaderParameter("colour_1", HexToColor(theme.BackgroundShaderHexColor1));
		material.SetShaderParameter("colour_2", HexToColor(theme.BackgroundShaderHexColor2));
		material.SetShaderParameter("colour_3", HexToColor(theme.BackgroundShaderHexColor3));
	}

	static Color HexToColor(string hex)
	{
		// Remove leading "#" if present
		if (hex.StartsWith("#"))
			hex = hex.Substring(1);

		// If no alpha provided, assume fully opaque
		if (hex.Length == 6)
			hex += "FF";

		// Parse hex as integer
		uint value = Convert.ToUInt32(hex, 16);

		// Extract RGBA
		float r = ((value >> 24) & 0xFF) / 255f;
		float g = ((value >> 16) & 0xFF) / 255f;
		float b = ((value >> 8) & 0xFF) / 255f;
		float a = (value & 0xFF) / 255f;

		return new Color(r, g, b, a);
	}
}
