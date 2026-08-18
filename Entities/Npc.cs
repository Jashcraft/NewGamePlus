using System.Numerics;
using Raylib_cs;

namespace NewGamePlus.Entities;

public class Npc
{
    private static readonly Vector2 Size = new(32, 32);

    public Vector2 Position;
    public IReadOnlyList<string> DialogueLines { get; }

    public Npc(Vector2 position, IReadOnlyList<string> dialogueLines)
    {
        Position = position;
        DialogueLines = dialogueLines;
    }

    public void Draw()
    {
        Raylib.DrawRectangle(
            (int)(Position.X - Size.X / 2f),
            (int)(Position.Y - Size.Y / 2f),
            (int)Size.X,
            (int)Size.Y,
            Color.Purple);
    }
}
