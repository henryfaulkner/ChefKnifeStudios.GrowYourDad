using Godot;
using System;
using System.Threading.Tasks;

public interface IPcWalletService
{
	int ProteinInWallet { get; set; }
}

public partial class PcWalletService : Node, IPcWalletService
{
	[Signal]
	public delegate void RefreshWalletUIEventHandler();
	
	int _proteinInWallet;

	public int ProteinInWallet
	{
		get => _proteinInWallet;
		set 
		{
			_proteinInWallet = value;
			EmitSignal(SignalName.RefreshWalletUI);
		}
	}
}
