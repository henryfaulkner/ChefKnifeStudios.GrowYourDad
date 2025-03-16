using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public interface IShopKeeperService
{
	IEnumerable<ItemBase> GetShopItems();
}

public partial class ShopKeeperService : Node, IShopKeeperService
{
	public IEnumerable<ItemBase> GetShopItems()
	{
		var result = ItemHelper.GetItems()
			.GroupBy(item => item.Id)
			.Select(group => group.First());
		return result;
	}
}
