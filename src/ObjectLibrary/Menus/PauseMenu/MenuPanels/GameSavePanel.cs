using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class GameSavePanel : BaseMenuPanel
{
	[Export]
	BaseButton BackBtn { get; set; } = null!;

	[ExportGroup("Existing Game Saves")]
	[Export]
	HSeparator GameSaveOptionsSeparator { get; set; } = null!;
	[Export]
	HBoxContainer GameSaveOptions { get; set; } = null!;
	[Export]
	Button[] UsernameBtns { get; set; } = null!;

	[ExportGroup("New Game Save")]
	[Export]
	HSeparator NewGameSaveSeparator { get; set; } = null!;
	[Export]
	TextEdit NewGameSaveTextBox { get; set; } = null!;
	[Export]
	HSeparator SubmitBtnSeparator { get; set; } = null!;
	[Export]
	BaseButton SubmitBtn { get; set; } = null!;
	
	public override int Id => (int)Enumerations.PauseMenuPanels.GameSave;
	
	IUnitOfWork _unitOfWork = null!;
	ILoggerService _logger = null!;
	ICrawlStatsService _crawlStatsService = null!;

	public MenuBusiness MenuBusiness { get; set; } = null!;
	
	public void Init(MenuBusiness menuBusiness)
	{
		MenuBusiness = menuBusiness;
	}

	public override void _Ready()
	{
		_unitOfWork = GetNode<IUnitOfWork>(Constants.SingletonNodes.UnitOfWork);
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_crawlStatsService = GetNode<ICrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);
		
		IEnumerable<GameSave> gameSaves = _unitOfWork.GameSaveRepository.GetAll();
		Controls = [BackBtn];
		BackBtn.Pressed += HandleBack;
		
		if (gameSaves.Count() > 0)
		{
			RegisterExistingGameSaves(gameSaves);
		}
		if (gameSaves.Count() < UsernameBtns.Length)
		{
			RegisterNewGameSave();
		}
		
		ToggleVisibility(gameSaves);
	}

	public override void _ExitTree()
	{
		if (BackBtn != null)
		{
			BackBtn.Pressed -= HandleBack;
		}

		foreach (var btn in UsernameBtns)
		{
			if (IsInstanceValid(btn) && SubmitBtn.IsConnected("pressed", Callable.From(() => HandleExistingGameSavePressed(btn))))
			{
				btn.Pressed -= () => HandleExistingGameSavePressed(btn);
			}
		}

		if (IsInstanceValid(SubmitBtn) && SubmitBtn.IsConnected("pressed", Callable.From(HandleSubmit)))
		{
			SubmitBtn.Pressed -= HandleSubmit;
		}
	}
	
	void ShowExistingGameSaves()
	{
		GameSaveOptionsSeparator.Visible = true;
		GameSaveOptions.Visible = true;
	}

	void HideExistingGameSaves()
	{
		GameSaveOptionsSeparator.Visible = false;
		GameSaveOptions.Visible = false;
	}

	void RegisterExistingGameSaves(IEnumerable<GameSave> gameSaves)
	{
		var gameSaveList = gameSaves.ToList();
		for (int i = 0; i < UsernameBtns.Count() && i < gameSaves.Count(); i += 1)
		{
			var btn = UsernameBtns[i];
			var gameSave = gameSaveList[i];
	
			GD.Print($"Add {gameSave.Username}");
			btn.Text = gameSave.Username;
			btn.Pressed += () => HandleExistingGameSavePressed(btn);
			Controls.Add(btn);
		}
	}

	void ToggleVisibility(IEnumerable<GameSave> gameSaves)
	{
		if (gameSaves.Count() == UsernameBtns.Length)
		{
			HideNewGameSave();
		}
		else 
		{
			ShowNewGameSave();
		}
		if (gameSaves.Count() == 0)
		{
			HideExistingGameSaves();
		} 
		else 
		{
			ShowExistingGameSaves();
		}
	}

	void ShowNewGameSave()
	{
		NewGameSaveSeparator.Visible = true;
		NewGameSaveTextBox.Visible = true;
		SubmitBtnSeparator.Visible = true;
		SubmitBtn.Visible = true;
	}
	
	void HideNewGameSave()
	{
		NewGameSaveSeparator.Visible = false;
		NewGameSaveTextBox.Visible = false;
		SubmitBtnSeparator.Visible = false;
		SubmitBtn.Visible = false;
	}
	
	void RegisterNewGameSave()
	{
		Controls.Add(NewGameSaveTextBox);
		SubmitBtn.Pressed += HandleSubmit;
		Controls.Add(SubmitBtn);
	}

	void HandleBack()
	{
		var resultPanel = MenuBusiness.PopPanel();
		EmitSignal(SignalName.Open, (int)resultPanel.Id);
	}

	void HandleExistingGameSavePressed(Button btn)
	{
		var gameSave = _unitOfWork.GameSaveRepository.GetAll()
			.Where(x => x.Username == btn.Text).First();
		if (gameSave == null)
		{
			_logger.LogError("GameSave cannot be changed to empty save.");
			return;
		}

		_crawlStatsService.GameSave = gameSave;
	}

	void HandleSubmit()
	{
		if (NewGameSaveTextBox == null)
		{
			_logger.LogError("HandleSubmit relies on NewGameSaveTextBox, which is null.");
			throw new Exception("HandleSubmit relies on NewGameSaveTextBox, which is null.");
		}
		if (string.IsNullOrEmpty(NewGameSaveTextBox.Text))
		{
			_logger.LogInfo("Username cannot be empty.");
			return;
		}
		string username = NewGameSaveTextBox.Text;

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
		
		NewGameSaveTextBox.Text = string.Empty;
		HandleBack();
		
		gameSaves = _unitOfWork.GameSaveRepository.GetAll();
		var newExistingBtn = UsernameBtns[gameSaves.Count() - 1];
		newExistingBtn.Text = newGameSave.Username;
		newExistingBtn.Pressed += () => HandleExistingGameSavePressed(newExistingBtn);
		Controls.Add(newExistingBtn);

		ToggleVisibility(gameSaves);
	}
}
