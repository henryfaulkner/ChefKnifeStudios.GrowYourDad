using Godot;
using System;
using System.Threading.Tasks;

public interface IPcWalletService
{
	int ProteinInWallet { get; set; }
}

public partial class PcWalletService : GameStateSingletonBase, IPcWalletService
{
	[Signal]
	public delegate void RefreshWalletUIEventHandler();
	
	int _proteinInWallet = 0;

	public int ProteinInWallet
	{
		get => _proteinInWallet;
		set 
		{
			_proteinInWallet = value;
			EmitSignal(SignalName.RefreshWalletUI);
		}
	}

    public override void Clear()
    {
        ProteinInWallet = 0;
    }
}
