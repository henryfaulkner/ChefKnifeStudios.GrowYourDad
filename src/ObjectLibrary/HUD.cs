using Godot;
using System;

public partial class HUD : CanvasLayer
{
	[ExportGroup("Meters")]
	[Export]
	public Meter HpMeter { get; set; } = null!;
	[Export]
	public Meter SpMeter { get; set; } = null!;
	
	[ExportGroup("Crawl Depth")]
	[Export]
	public RichTextLabel CrawlDepthLabel { get; set; } = null!;

	[ExportGroup("Protein")]
	[Export]
	public RichTextLabel ProteinLabel { get; set; } = null!;

	Observables _observables = null!;
	ILoggerService _logger = null!;
	PcWalletService _pcWalletService = null!;
	ICrawlStatsService _crawlStatsService = null!;
	
	const string CRAWL_DEPTH_LABEL_TEXT = "[font_size=24][left]Level {0}[/left][/font_size]";
	const string PROTEIN_LABEL_TEXT = "[font_size=24][right]{0} proteins[/right][/font_size]";

	public override void _Ready()
	{
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_pcWalletService = GetNode<PcWalletService>(Constants.SingletonNodes.PcWalletService);
		_crawlStatsService = GetNode<ICrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);

		_observables.UpdateHpMeterValue += HpMeter.UpdateValue;
		_observables.UpdateHpMeterMax += HpMeter.UpdateMax;

		_observables.UpdateSpMeterValue += SpMeter.UpdateValue;
		_observables.UpdateSpMeterMax += SpMeter.UpdateMax;
		
		CrawlDepthLabel.Text = string.Format(CRAWL_DEPTH_LABEL_TEXT, _crawlStatsService.CrawlStats.CrawlDepth_ToString());

		_pcWalletService.RefreshWalletUI += HandleRefreshWalletUI;
		HandleRefreshWalletUI();
	}

	public override void _ExitTree()
	{
		if (_observables != null)
		{
			if (HpMeter != null)
			{
				_observables.UpdateHpMeterValue -= HpMeter.UpdateValue;
				_observables.UpdateHpMeterMax -= HpMeter.UpdateMax;
			}

			if (SpMeter != null)
			{
				_observables.UpdateSpMeterValue -= SpMeter.UpdateValue;
				_observables.UpdateSpMeterMax -= SpMeter.UpdateMax;
			}
		}
		
		if (_pcWalletService != null)
		{
			_pcWalletService.RefreshWalletUI -= HandleRefreshWalletUI;
		}
	}
	
	void HandleRefreshWalletUI()
	{
		ProteinLabel.Text = string.Format(PROTEIN_LABEL_TEXT, _pcWalletService.ProteinInWallet.ToString());
	}
}
