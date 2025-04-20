using Godot;
using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

[JsonDerivedType(typeof(EnemyBase), typeDiscriminator: "base")]
[JsonDerivedType(typeof(EnemyWithSpike), typeDiscriminator: "withSpike")]
public class EnemyBase
{
    [JsonPropertyName("id")]
	public required string Id { get; set; }
	
	[JsonPropertyName("name")]
	public required string Name { get; set; }
	
	[JsonPropertyName("description")]
	public string? Description { get; set; }

    [JsonPropertyName("controllerNode")]
    public required string ControllerNode { get; set; }

    [JsonPropertyName("hp")]
    public int Hp { get; set; }

    [JsonPropertyName("contactDamage")]
    public int ContactDamage { get; set; }

	[JsonPropertyName("rarityTier")]
	public required string RarityTier { get; set; }

    [JsonPropertyName("spawnsAfterWhatDepth")]
    public int SpawnsAfterWhatDepth { get; set; }

	public SpawnRarity? Rarity { get; set; }
}

public class EnemyWithSpike : EnemyBase
{
    [JsonPropertyName("spikeContactDamage")]
    public int SpikeContactDamage { get; set; }

    [JsonPropertyName("spikeHeight")]
    public int SpikeHeight { get; set; }

    [JsonPropertyName("spikeWidth")]
    public int SpikeWidth { get; set; }
}