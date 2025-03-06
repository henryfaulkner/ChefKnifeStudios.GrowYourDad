using Godot;
using System;
using System.Collections.Generic;

public interface IPcInventoryService
{
    List<ItemBase> ItemInventory { get; }
    void AddToInventory(ItemBase item);
}

public partial class PcInventoryService : Node, IPcInventoryService
{
    public List<ItemBase> ItemInventory { get; private set; } = new();

    public void AddToInventory(ItemBase item)
    {
        ItemInventory.Add(item);
    }
}
