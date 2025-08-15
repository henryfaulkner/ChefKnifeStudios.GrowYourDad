using System.Text.Json;
using System.Collections.Generic;
using Godot;
using FileAccess = Godot.FileAccess;

public static class ThemeHelper
{
    private static readonly string ThemesFilePath = "res://src/Core/Themes/themes.json";

    public static Theme? GetThemeByIndex(int index)
    {
        var themes = LoadThemes();
        return themes[index % themes.Count];
    }

    // Method to load themes from a JSON file
    public static List<Theme> LoadThemes()
    {
        var file = FileAccess.Open(ThemesFilePath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            GD.PrintErr($"Failed to open themes file at {ThemesFilePath}");
            return new List<Theme>();
        }

        string json = file.GetAsText();
        return JsonSerializer.Deserialize<List<Theme>>(json) ?? new List<Theme>();
    }
}