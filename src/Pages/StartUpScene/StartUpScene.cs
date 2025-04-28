using Godot;
using System;
using System.Linq;

public partial class StartUpScene : Node
{
	NavigationAuthority _navigationAuthority = null!;
	IUnitOfWork _unitOfWork = null!;
	ICrawlStatsService _crawlStatsService = null!;

	public override void _EnterTree()
	{
		_navigationAuthority = GetNode<NavigationAuthority>(Constants.SingletonNodes.NavigationAuthority);
		_unitOfWork = GetNode<IUnitOfWork>(Constants.SingletonNodes.UnitOfWork);
		_crawlStatsService = GetNode<ICrawlStatsService>(Constants.SingletonNodes.CrawlStatsService);

		base._EnterTree();
	}


	public override void _Ready()
	{
		using (var dbContext = new AppDbContext())
		{
			dbContext.DropAndRecreateXpLevelTable();
			dbContext.DropAndRecreatePcLevelView();
		}

		var currentGameSave = _unitOfWork.GameSaveRepository
				.QueryScalar(dbSet => 
					dbSet.Where(x => x.IsCurrent)
					.FirstOrDefault()
				);

		if (currentGameSave is null)
		{
			// Use call_deferred to safely change the scene
			_navigationAuthority.CallDeferred("ChangeToIntro");
		}
		else 
		{
			_crawlStatsService.GameSave = currentGameSave; 
			// Use call_deferred to safely change the scene
			_navigationAuthority.CallDeferred("ChangeToPreActionLevel");
		}

		base._Ready();
	}
}
