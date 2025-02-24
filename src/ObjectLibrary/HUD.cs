using Godot;
using System;

public partial class HUD : CanvasLayer
{
	[ExportGroup("Meters")]
	[Export]
	public Meter HpMeter { get; set; } = null!;
	[Export]
	public Meter SpMeter { get; set; } = null!;

	Observables _observables;
	ILoggerService _logger;

	public override void _Ready()
	{
		_observables = GetNode<Observables>(Constants.SingletonNodes.Observables);
		_logger = GetNode<ILoggerService>(Constants.SingletonNodes.LoggerService);

		_observables.UpdateHpMeterValue += HpMeter.UpdateValue;
		_observables.UpdateHpMeterMax += HpMeter.UpdateMax;

		_observables.UpdateSpMeterValue += SpMeter.UpdateValue;
		_observables.UpdateSpMeterMax += SpMeter.UpdateMax;
	}
}
