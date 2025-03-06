using Godot;
using System;
using System.Collections.Generic;

public interface IPcInventoryService
{
    List<ItemBase> ItemInventory { get; set; }
}

public partial class PcInventoryService : Node
{
    public List<ItemBase> ItemInventory { get; set; } = new();
}
