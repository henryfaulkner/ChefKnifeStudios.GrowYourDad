using Godot;
using System.Threading.Tasks;

public interface IEnemy
{
    void HandleHit(int pcArea);
    void HandleHurt(int pcArea);
    void TakeDamage();
    Task StartFlashing();
}