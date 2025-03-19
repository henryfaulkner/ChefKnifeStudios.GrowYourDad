using System;
using System.Linq;
using System.Collections.Generic;
using Godot;

public partial class ShopKeeperPanel : BaseMenuPanel
{
	[Export]
	BaseButton BackBtn = null!;
	[Export]
	OptionButton ItemOptionBtn = null!;
	[Export]
	BaseButton BuyBtn = null!;
	
	public override Enumerations.PauseMenuPanels Id => Enumerations.PauseMenuPanels.ShopKeeper;
	
	ILoggerService _logger = null!;
	PauseMenuService _pauseMenuService = null!;
	IShopKeeperService _shopKeeperService = null!;
	IPcInventoryService _pcInventoryService = null!;
	IPcWalletService _pcWalletService = null!;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_pauseMenuService = GetNode<PauseMenuService>(Constants.SingletonNodes.PauseMenuService);
		_shopKeeperService = GetNode<IShopKeeperService>(Constants.SingletonNodes.ShopKeeperService);
		_pcInventoryService = GetNode<IPcInventoryService>(Constants.SingletonNodes.PcInventoryService);
		_pcWalletService = GetNode<IPcWalletService>(Constants.SingletonNodes.PcWalletService);

		FocusIndex = 0;
		Buttons = new List<BaseButton>();
		Buttons.Add(BackBtn);
		Buttons.Add(ItemOptionBtn);
		Buttons.Add(BuyBtn);
		
		BackBtn.Pressed += HandleBack;	
		PopulateItemOptions();
		BuyBtn.Pressed += HandleBuy;

		_shopKeeperService.GetShopItems();
	}

	public override void _ExitTree()
	{
		if (BackBtn != null)
		{
			BackBtn.Pressed -= HandleBack;
		}
	}

	void HandleBack()
	{
		var resultPanel = _pauseMenuService.PopPanel();
		EmitSignal(SignalName.Open, (int)resultPanel.Id);
	}

	void HandleBuy()
	{
		var selectedItem = _shopKeeperService.GetShopItems().ToArray()[ItemOptionBtn.GetSelectedId()];
		if (_pcWalletService.ProteinInWallet < selectedItem.Price) 
		{
			_logger.LogInfo("Not enough protein to buy.");
			return;
		}

		_pcWalletService.ProteinInWallet -= selectedItem.Price;
		_pcInventoryService.AddToInventory(selectedItem);
	}
	
	void PopulateItemOptions()
	{
		var items = _shopKeeperService.GetShopItems().ToList();
		int len = items.Count();

		for (int i = 0; i < len; i += 1)
		{
			ItemOptionBtn.AddItem($"{items[i].Name} ${items[i].Price}", i);
		}
	}
}
