using Godot;
using System;

public partial class StartUpScene : Node
{
	NavigationAuthority _navigationAuthority = null!;

	public override void _Ready()
	{
		using (var dbContext = new AppDbContext())
		{
			dbContext.DropAndRecreateXpLevelTable();
		}

		_navigationAuthority = GetNode<NavigationAuthority>(Constants.SingletonNodes.NavigationAuthority);
		// Use call_deferred to safely change the scene
		_navigationAuthority.CallDeferred("ChangeToPreActionLevel");
	}
}
