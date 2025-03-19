using System;
using System.Collections.Generic;
using System.Text;

public static class DictionaryExtensions
{
	public static string ToString<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
	{
		if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));

		StringBuilder sb = new StringBuilder();
		sb.AppendLine("{");

		foreach (var kvp in dictionary)
		{
			sb.AppendLine($"  [{kvp.Key}] : {kvp.Value}");
		}

		sb.AppendLine("}");
		return sb.ToString();
	}
}
