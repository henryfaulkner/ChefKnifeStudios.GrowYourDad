using System;
using System.Linq;
using System.Collections.Generic;
using Godot;
using System.Text;
using System.Collections;

public partial class CrawlStatsHistoryPanel : BaseMenuPanel
{
	[Export]
	BaseButton BackBtn = null!;

	[Export]
	RichTextLabel Table = null!;

	const string TABLE_LABEL_START = 
		"""
		[table=4,center]
		[cell]Gamer[/cell]
		[cell]Depth[/cell]
		[cell]Protein Collected[/cell]
		[cell]Foes Defeated[/cell]
		""";

	const string TABLE_LABEL_END = "[/table]";
	
	public override int Id => (int)Enumerations.PauseMenuPanels.CrawlStatsHistory;
	
	ILoggerService _logger = null!;
	ICrawlStatsService _crawlStatsService = null!;

	public MenuBusiness MenuBusiness { get; set; } = null!;

	public override void _Ready()
	{
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_crawlStatsService = GetNode<ICrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);

		Controls = [BackBtn];
		
		BackBtn.Pressed += HandleBack;

		var histories = _crawlStatsService.GetCrawlStatsHistory();

		StringBuilder sb = new();
		sb.Append(TABLE_LABEL_START);
		sb.Append(GetTableCellsText(histories));
		sb.Append(TABLE_LABEL_END);
		Table.Text = sb.ToString();
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
	}

	void HandleBack()
	{
		var resultPanel = MenuBusiness.PopPanel();	
		EmitSignal(SignalName.Open, (int)resultPanel.Id);
	}

	string GetTableCellsText(IEnumerable<CrawlStatsHistory> histories)
	{
		StringBuilder sb = new();
		foreach (var history in histories)
		{
			sb.Append(
				$"""
				[cell]{history.Gamer}[/cell]
				[cell]{history.FloorDepth}[/cell]
				[cell]{history.ProteinCollected}[/cell]
				[cell]{history.FoesDefeated}[/cell]
				"""
			);
		}
		return sb.ToString();
	} 
}
