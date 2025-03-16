using Godot;
using System;
using System.Linq;

public partial class PcMagnetArea : Area2D 
{ 
	[Export]	
	CollisionShape2D _collisionShape; 
	
	IPcInventoryService _pcInventoryService;
	
	public override void _Ready()
	{
		_pcInventoryService = GetNode<IPcInventoryService>(Constants.SingletonNodes.PcInventoryService);
		ApplyMagnetBenefit();
	}

	void ApplyMagnetBenefit()
	{
		int magnetRadiusBenefit = _pcInventoryService.GetInvItemsWithPassiveEffect()
			.Sum(x => x.MagnetRadiusBenefit);
		((CircleShape2D)_collisionShape.Shape).Radius += magnetRadiusBenefit;
	} 
}
