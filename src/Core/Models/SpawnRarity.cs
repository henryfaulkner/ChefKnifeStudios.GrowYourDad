using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

// NOTE: This could be extended to change spawn rates at certain depths.
public class SpawnRarity
{
	[JsonPropertyName("tier")]
	public required string Tier { get; set; }

	[JsonPropertyName("chance")]
	public int Chance { get; set; }

	[JsonPropertyName("color")]
	public required string Color { get; set; }
}