using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Collections.Generic;
using Godot;
using FileAccess = Godot.FileAccess;
using System.Linq;
using System;

using JsonSerializer = System.Text.Json.JsonSerializer;

public static class EnemyHelper
{
	// method: get enemy list from file
    public static IEnumerable<EnemyBase> GetEnemies()
    {
        string filePath = "res://Core/Enemies/enemy-schemas.json";
        using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
        string jsonContent = file.GetAsText();
        var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,  // Ensures case-insensitive matching
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(), // Required for polymorphic deserialization
			WriteIndented = true
		};

        var result = JsonSerializer.Deserialize<IEnumerable<EnemyBase>>(jsonContent, options);
		if (result == null) throw new JsonException("enemy-schemas.json did not deserialize into an item list.");
		
        PopulateRarityProperty(result);
		return result;
    }

    public static IEnumerable<SpawnRarity> GetSpawnRarities()
	{
		string filePath = "res://Core/Enemies/rarity-schemas.json";
		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
		string jsonContent = file.GetAsText();
		var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,  // Ensures case-insensitive matching
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(), // Required for polymorphic deserialization
			WriteIndented = true
		};
		var result = JsonSerializer.Deserialize<IEnumerable<SpawnRarity>>(jsonContent, options);
		if (result == null) throw new JsonException("rarity-schemas.json did not deserialize into an item list.");
		return result;
	}
    
    // method: get random item
	public static EnemyBase GetRandomItem()
    {
        var enemyList = GetEnemies().ToList();
        var rand = new Random();

        // Step 1: Compute total weight
        int totalWeight = enemyList.Sum(item => item.Rarity!.Chance);

        // Step 2: Generate random number within total weight range
        int randomValue = rand.Next(totalWeight);

        // Step 3: Select item using cumulative probability
        int cumulative = 0;
        foreach (var enemy in enemyList)
        {
            cumulative += enemy.Rarity!.Chance;
            if (randomValue < cumulative)
            {
                return enemy;
            }
        }

        return enemyList[0]; // Fallback (should never be reached)
    }

	// method: get random n items
	public static IEnumerable<EnemyBase> GetRandomNItems(int n)
	{
		var enemyList = GetEnemies().ToList();
		var rand = new Random();
		List<EnemyBase> result = new();

		// If n is greater than or equal to the number of items, shuffle and take all unique items
		if (n >= enemyList.Count)
		{
			// Shuffle the list to get a random order of items
			enemyList = enemyList.OrderBy(x => rand.Next()).ToList();
			return enemyList;
		}
		else
		{
			// Get n unique random items
			while (result.Count < n)
			{
				var randItem = GetRandomItem();
				if (result.Where(x => x.Id == randItem.Id).Any())
					continue;
				result.Add(randItem);
			}
		}

		return result;
	}

    static void PopulateRarityProperty(IEnumerable<EnemyBase> enemies)
	{
		var enemyRarities = GetSpawnRarities();
		var rarityDict = enemyRarities.ToDictionary(x => x.Tier);

		foreach (var enemy in enemies)
			enemy.Rarity = rarityDict[enemy.RarityTier];
	}
}