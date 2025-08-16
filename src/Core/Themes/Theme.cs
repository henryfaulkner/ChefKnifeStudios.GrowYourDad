using System.Text.Json.Serialization;

public record Theme
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("tileSetSourceId")]
    public int TileSetSourceId { get; set; }

    [JsonPropertyName("shaderHexColor1")]
    public required string BackgroundShaderHexColor1 { get; set; }

    [JsonPropertyName("shaderHexColor2")]
    public required string BackgroundShaderHexColor2 { get; set; }

    [JsonPropertyName("shaderHexColor3")]
    public required string BackgroundShaderHexColor3 { get; set; }
}

