using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

[JsonDerivedType(typeof(ItemBase), typeDiscriminator: "base")]
[JsonDerivedType(typeof(ItemWithHealingEffect), typeDiscriminator: "withHealingEffect")]
[JsonDerivedType(typeof(ItemWithDamagingEffect), typeDiscriminator: "withDamagingEffect")]
[JsonDerivedType(typeof(ItemWithPassiveEffect), typeDiscriminator: "withPassiveEffect")]
public class ItemBase
{

    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("Price")]
    public int Price { get; set; }

    public override string ToString()
    {
        return $"ItemBase (ID: {Id}, Name: {Name}, Description: {Description ?? "No description"}, Price: {Price})";
    }
}

public class ItemWithHealingEffect : ItemBase
{
    [JsonPropertyName("oneTimeHpBenefit")]
    public int OneTimeHpBenefit { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()}, OneTimeHpBenefit: {OneTimeHpBenefit}";
    }
}

public class ItemWithDamagingEffect : ItemBase
{
    [JsonPropertyName("damageBase")]
    public int DamageBase { get; set; }

    [JsonPropertyName("ammoConsumed")]
    public int AmmoConsumed { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()}, DamageBase: {DamageBase}, AmmoConsumed: {AmmoConsumed}";
    }
}

public class ItemWithPassiveEffect : ItemBase
{
    [JsonPropertyName("baseHpBenefit")]
    public int BaseHpBenefit { get; set; }
    
    [JsonPropertyName("baseSpBenefit")]
    public int BaseSpBenefit { get; set; }

    [JsonPropertyName("damageBenefit")]
    public int DamageBenefit { get; set; }
    
    [JsonPropertyName("magnetRadiusBenefit")]
    public int MagnetRadiusBenefit { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()}, BaseHpBenefit: {BaseHpBenefit}, BaseSpBenefit: {BaseSpBenefit}, " +
               $"DamageBenefit: {DamageBenefit}, MagnetRadiusBenefit: {MagnetRadiusBenefit}";
    }
}