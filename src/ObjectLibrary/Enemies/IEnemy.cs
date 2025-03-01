using Godot;

public interface IEnemy
{
    void HandleHit(int pcArea);
    void HandleHurt(int pcArea);
}