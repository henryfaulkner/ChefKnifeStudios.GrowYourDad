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
	
	public override int Id => (int)Enumerations.PauseMenuPanels.ShopKeeper;
	
	ILoggerService _logger = null!;
	IShopKeeperService _shopKeeperService = null!;
	IPcInventoryService _pcInventoryService = null!;
	IPcWalletService _pcWalletService = null!;
	ICrawlStatsService _crawlStatsService = null!;

	public MenuBusiness MenuBusiness { get; set; } = null!;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_shopKeeperService = GetNode<IShopKeeperService>(Constants.SingletonNodes.ShopKeeperService);
		_pcInventoryService = GetNode<IPcInventoryService>(Constants.SingletonNodes.PcInventoryService);
		_pcWalletService = GetNode<IPcWalletService>(Constants.SingletonNodes.PcWalletService);
		_crawlStatsService = GetNode<ICrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);

		Controls = [BackBtn, ItemOptionBtn, BuyBtn];
		
		BackBtn.Pressed += HandleBack;	
		PopulateItemOptions();
		BuyBtn.Pressed += HandleBuy;

		_shopKeeperService.GetShopItems();
	}

	public void Init(MenuBusiness menuBusiness)
	{
		MenuBusiness = menuBusiness;
	}

	public override void _ExitTree()
	{
		if (BackBtn != null)
		{
			BackBtn.Pressed -= HandleBack;
		}

		if (BuyBtn != null)
			BuyBtn.Pressed += HandleBuy;
	}

	void HandleBack()
	{
		var resultPanel = MenuBusiness.PopPanel();
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
		_crawlStatsService.CrawlStats.ItemsPurchased += 1;
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
