using Godot;
using System;
using System.Linq;

public partial class NewGameSavePopupMenu : PopupMenu
{
	[Signal]
	public delegate void SubmittedEventHandler();

	[Export]
	TextEdit TextBox { get; set; } = null!;
	[Export] 
	Button SubmitBtn { get; set; } = null!;

	IUnitOfWork _unitOfWork = null!;
	ILoggerService _logger = null!;
	CrawlStatsService _crawlStatsService = null!;

	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.WhenPaused;
		_unitOfWork = GetNode<IUnitOfWork>(Constants.SingletonNodes.UnitOfWork);
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_crawlStatsService = GetNode<CrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);

		SubmitBtn.Pressed += HandleSubmitClicked;    
	}

	public override void _ExitTree()
	{
		SubmitBtn.Pressed -= HandleSubmitClicked;
	}

	public void Open()
	{
		Visible = true;
		GetTree().Paused = true;
	}

	public void Close()
	{
		Visible = false;
		GetTree().Paused = false;
	}

	void HandleSubmitClicked()
	{
		// create new gamer
		if (string.IsNullOrEmpty(TextBox.Text))
		{
			_logger.LogInfo("Username cannot be empty.");
			return;
		}
		string username = TextBox.Text;

		// don't allow duplicate names
		var gameSaves = _unitOfWork.GameSaveRepository.GetAll();
		if (gameSaves.Any(x => x.Username == username))
		{
			_logger.LogError("Duplicate usernames are not allowed.");
			return;
		}

		GameSave newGameSave = new() 
		{ 
			Username = username,
		};
		_unitOfWork.GameSaveRepository.Add(newGameSave);
		_unitOfWork.SaveChanges();
		_crawlStatsService.GameSave = newGameSave;

		EmitSignal(SignalName.Submitted);
		this.Close();
	}
}
