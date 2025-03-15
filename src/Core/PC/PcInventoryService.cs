using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IPcInventoryService
{
	List<ItemBase> ItemInventory { get; }
	void AddToInventory(ItemBase item);
	
	IEnumerable<ItemWithHealingEffect> GetInvItemsWithHealingEffect();
	IEnumerable<ItemWithBlastingEffect> GetInvItemsWithBlastingEffect();
	IEnumerable<ItemWithPassiveEffect> GetInvItemsWithPassiveEffect();

	int GetPcHpMax();
	int GetPcSpMax();
	int GetPcDamage();
}

public partial class PcInventoryService : Node, IPcInventoryService
{
	IPcMeterService _pcMeterService;

	public List<ItemBase> ItemInventory { get; private set; } = new();

	public override void _Ready()
	{
		_pcMeterService = GetNode<IPcMeterService>(Constants.SingletonNodes.PcMeterService);
	}

	public void AddToInventory(ItemBase item)
	{
		switch (item)
		{
			case ItemWithHealingEffect itemWithHealingEffect:
				if (_pcMeterService.HpValue + itemWithHealingEffect.OneTimeHpBenefit > _pcMeterService.HpMax)
				{
					_pcMeterService.HpValue = _pcMeterService.HpMax;
				}
				else
				{
					_pcMeterService.HpValue += itemWithHealingEffect.OneTimeHpBenefit;
				}
				break;
			case ItemWithBlastingEffect itemWithBlastingEffect:
				ItemInventory.RemoveAll(x => x is ItemWithBlastingEffect);
				break;
			case ItemWithPassiveEffect itemWithPassiveEffect:
				// when an item is added, which will improve the PC's base health, 
				// their current health should benefit the same amount.
				_pcMeterService.HpValue += itemWithPassiveEffect.BaseHpBenefit;
				break;
			default:
				break;
		}

		ItemInventory.Add(item);
	}

	public IEnumerable<ItemWithHealingEffect> GetInvItemsWithHealingEffect()
	{
		return ItemInventory.OfType<ItemWithHealingEffect>();
	}

	public IEnumerable<ItemWithBlastingEffect> GetInvItemsWithBlastingEffect()
	{
		return ItemInventory.OfType<ItemWithBlastingEffect>();
	}

	public IEnumerable<ItemWithPassiveEffect> GetInvItemsWithPassiveEffect()
	{
		return ItemInventory.OfType<ItemWithPassiveEffect>();
	}

	public int GetPcHpMax()
	{
		int baseHp = 3;
		int additionalHp = GetInvItemsWithPassiveEffect()
			.Sum(x => x.BaseHpBenefit);
		return baseHp + additionalHp;
	}

	public int GetPcSpMax()
	{
		int baseSp = 3;
		int additionalSp = GetInvItemsWithPassiveEffect()
			.Sum(x => x.BaseSpBenefit);
		return baseSp + additionalSp;
	}

	public int GetPcDamage()
	{
		int weaponDamage = GetInvItemsWithBlastingEffect()
			.Sum(x => x.DamageBase);
		int passiveDamage = GetInvItemsWithPassiveEffect()
			.Sum(x => x.DamageBenefit);
		return weaponDamage + passiveDamage;
	}
}
