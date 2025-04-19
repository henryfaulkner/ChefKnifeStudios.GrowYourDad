using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Collections.Generic;
using Godot;
using FileAccess = Godot.FileAccess;
using System.Linq;
using System;

using JsonSerializer = System.Text.Json.JsonSerializer;

public static class ItemHelper
{
	// method: get item list from file
	public static IEnumerable<ItemBase> GetItems()
	{
		string filePath = "res://Core/Items/item-schemas.json";
		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
		string jsonContent = file.GetAsText();
		var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,  // Ensures case-insensitive matching
			TypeInfoResolver = new DefaultJsonTypeInfoResolver(), // Required for polymorphic deserialization
			WriteIndented = true
		};

		IEnumerable<ItemBase>? result = JsonSerializer.Deserialize<IEnumerable<ItemBase>>(jsonContent, options);
		if (result == null) throw new JsonException("item-schemas.json did not deserialize into an item list.");
		
		PopulateRarityProperty(result);
		return result;
	}

	public static IEnumerable<SpawnRarity> GetSpawnRarities()
	{
		string filePath = "res://Core/Items/rarity-schemas.json";
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
	public static ItemBase GetRandomItem()
    {
        var itemList = GetItems().ToList();
        var rand = new Random();

        // Step 1: Compute total weight
        int totalWeight = itemList.Sum(item => item.Rarity!.Chance);

        // Step 2: Generate random number within total weight range
        int randomValue = rand.Next(totalWeight);

        // Step 3: Select item using cumulative probability
        int cumulative = 0;
        foreach (var item in itemList)
        {
            cumulative += item.Rarity!.Chance;
            if (randomValue < cumulative)
            {
                return item;
            }
        }

        return itemList[0]; // Fallback (should never be reached)
    }

	// method: get random n items
	public static IEnumerable<ItemBase> GetRandomNItems(int n)
	{
		var itemList = GetItems().ToList();
		var rand = new Random();
		List<ItemBase> result = new();

		// If n is greater than or equal to the number of items, shuffle and take all unique items
		if (n >= itemList.Count)
		{
			// Shuffle the list to get a random order of items
			itemList = itemList.OrderBy(x => rand.Next()).ToList();
			return itemList;
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

	// Method to use and log results of the three static methods
	public static void UseAndLogResults()
	{
		// 1. Get a random item
		var randomItem = GetRandomItem();
		GD.Print("Random Item:");
		GD.Print(randomItem);

		// 2. Get N random items (e.g., 3 random items)
		var randomNItems = GetRandomNItems(3);
		GD.Print("\nRandom N Items (3 items):");
		foreach (var item in randomNItems)
		{
			GD.Print(item);
		}

		// 3. Get all items from the GetItems method
		var allItems = GetItems();
		GD.Print("\nAll Items:");
		foreach (var item in allItems)
		{
			GD.Print(item);
		}
	}

	static void PopulateRarityProperty(IEnumerable<ItemBase> items)
	{
		var itemRarities = GetSpawnRarities();
		var rarityDict = itemRarities.ToDictionary(x => x.Tier);

		foreach (var item in items)
			item.Rarity = rarityDict[item.RarityTier];
	}
}
