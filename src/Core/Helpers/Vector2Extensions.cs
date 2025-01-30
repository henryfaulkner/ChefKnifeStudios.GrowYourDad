using Godot;

public static class Vector2Extensions 
{
    public static string ToString(this Vector2 vector)
    {
        return $"X: {vector.X}, Y: {vector.Y}";
    }
}