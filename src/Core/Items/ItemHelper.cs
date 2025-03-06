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

		if (result == null) throw new JsonException("schemas.json did not deserialize into an item list.");

		return result;
	}

	// method: get random item
	public static ItemBase GetRandomItem()
	{
		var itemList = GetItems().ToList();
		var rand = new Random();

		int randIndex = rand.Next(itemList.Count());
		return itemList[randIndex];
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
				var randIndex = rand.Next(itemList.Count);
				var randomItem = itemList[randIndex];

				// Add the item to the result
				result.Add(randomItem);

				// Remove the selected item from the list to avoid duplicates
				itemList.RemoveAt(randIndex);
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
}
