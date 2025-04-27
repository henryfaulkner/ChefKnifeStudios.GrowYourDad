using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class IntroTextBox : CanvasLayer
{
	[Export]
	MarginContainer IntroTextBoxContainer { get; set; } = null!;

	[Export]
	MarginContainer CreateProfileContainer { get; set; } = null!;

	IUnitOfWork _unitOfWork = null!;
	ILoggerService _logger = null!;
	CrawlStatsService _crawlStatsService = null!;

	public override void _EnterTree()
	{
		_unitOfWork = GetNode<IUnitOfWork>(Constants.SingletonNodes.UnitOfWork);
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);
		_crawlStatsService = GetNode<CrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);

		base._EnterTree();
	}

	public override void _Ready()
	{
		SubmitBtn.Pressed += HandleSubmitClicked; 
		HideTextBox();

		base._Ready();
	}

	public override void _ExitTree()
	{
		SubmitBtn.Pressed -= HandleSubmitClicked;
	}

	public override void _Process(double delta)
	{
		// ASSUMING INPUTMAP HAS A MAPPING FOR interact
		if (Input.IsActionJustPressed(INTERACT_INPUT))
		{
			if (_isReading)
			{
				_charReadRate = 5000f;
			}
			else
			{
				AdvanceTextBox();
			}
		}

		base._Process(delta);
	}

	#region TEXTBOX
	[Signal]
	public delegate void TextBoxCompletedEventHandler();

	[Export]
	RichTextLabel DialogueLabel { get; set; } = null!;

	const int DEFAULT_PAGE_LENGTH = 300;
	const float DEFAULT_READ_RATE = 70f;
	static readonly StringName INTERACT_INPUT = new StringName("shoot");
	static readonly StringName RIGHT_INPUT = new StringName("right");

	Queue<string> _dialogueQueue = new();

	bool _isReading = false;
	float _charReadRate = DEFAULT_READ_RATE;

	public void ExecuteProcess()
	{
		IntroTextBoxContainer.Visible = true;
		AdvanceTextBox();
	}

	public void HideTextBox()
	{
		Clear();
		IntroTextBoxContainer.Visible = false;
	}

	public void AddDialogue(string fullText)
	{
		var splitTextList = SplitText(fullText, DEFAULT_PAGE_LENGTH);
		foreach (var text in splitTextList)
		{
			_dialogueQueue.Enqueue(text);
		}
	}

	public bool IsOpen()
	{
		return Visible;
	}

	void AdvanceTextBox()
	{
		if (_dialogueQueue.TryDequeue(out string? dialogue))
		{
			if (dialogue == null) 
			{
				AdvanceTextBox();
				return;
			}
			
			Reset();
			_ = ReadDialogue(dialogue);
		}
		else
		{
			EmitSignal(SignalName.TextBoxCompleted);
		}
	}

	async Task ReadDialogue(string dialogue)
	{
		DialogueLabel.Text = dialogue;

		_isReading = true;
		int len = dialogue.Length;
		for (int i = 0; i < len; i++)
		{
			TimeSpan span = TimeSpan.FromSeconds((double)new decimal(1 / _charReadRate));
			DialogueLabel.VisibleCharacters += 1;
			await Task.Delay(span);
		}
		_isReading = false;
	}

	void Reset()
	{
		_charReadRate = DEFAULT_READ_RATE;
		DialogueLabel.Text = string.Empty;
		DialogueLabel.VisibleCharacters = 0;
	}

	void Clear()
	{
		Reset();
		_dialogueQueue.Clear();
		_isReading = false;
	}

	static List<string> SplitText(string fullText, int pageLength)
	{
		List<string> result = new ();
		int fullTextCharCount = fullText.Length;
		int fullPageCount = Convert.ToInt32(Math.Floor((double)(fullTextCharCount / pageLength)));
		int excessPageCharCount = fullTextCharCount - (fullPageCount * pageLength);

		int offset = 0;
		for (int i = 0; i < fullPageCount; i += 1)
		{
			result.Add(fullText.Substring(offset, pageLength));
			offset += pageLength;
		}
		// Account for excess
		if (excessPageCharCount > 0) result.Add(fullText.Substring(offset, excessPageCharCount));
		return result;
	}
	#endregion	

	#region CREATE PROFILE
	[Signal]
	public delegate void SubmittedEventHandler();

	[Export]
	TextEdit TextBox { get; set; } = null!;
	[Export] 
	Button SubmitBtn { get; set; } = null!;

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
		GD.Print("NewGameSavePopupMenu: change game save");

		EmitSignal(SignalName.Submitted);
	}

	public void ShowCreateProfile()
	{
		CreateProfileContainer.Visible = true;
	}

	public void HideCreateProfile()
	{
		CreateProfileContainer.Visible = false;
	}
	#endregion
}
