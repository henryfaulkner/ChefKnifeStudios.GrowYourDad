using System;
using System.Runtime.Caching;
using Godot;

public static class XpTableHelper
{
	private static readonly MemoryCache _cache = MemoryCache.Default;
	private static readonly string CacheKey = "XpTableCache";
	private static readonly object _cacheLock = new object();

	public static int GetLevelsXpGoal(int level)
	{
		// Try to get the XP table from the cache
		if (!_cache.Contains(CacheKey))
		{
			lock (_cacheLock)
			{
				if (!_cache.Contains(CacheKey)) // Double-check locking for thread safety
				{
					var xpTable = GenerateXpTable();
					_cache.Set(CacheKey, xpTable, DateTimeOffset.UtcNow.AddMinutes(30)); // Cache for 30 minutes
				}
			}
		}

		var cachedTable = (int[])_cache.Get(CacheKey);
		return cachedTable[level];
	}

	public static int[] GenerateXpTable(int maxLevel = 20, int baseXP = 100, float multiplier = 1.5f)
	{
		int[] result = new int[maxLevel + 1];  // Array to store XP required for each level

		// Generate the XP table
		for (int level = 1; level <= maxLevel; level++)
		{
			if (level == 1)
			{
				result[level] = baseXP;
			}
			else
			{
				// Calculate XP for the next level based on the previous level
				result[level] = (int)(result[level - 1] * multiplier);
			}
		}

		return result;
	}
}
