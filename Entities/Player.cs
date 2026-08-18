using System.Numerics;
using NewGamePlus.World;
using Raylib_cs;

namespace NewGamePlus.Entities;

public class Player
{
    private const float Speed = 150f;
    private static readonly Vector2 ColliderSize = new(32, 32);

    // Position is in pixels (world space), not tile coordinates - see
    // "Movement decision" in game-architecture.md.
    public Vector2 Position;

    // Unused this ticket, but tracked now since Ticket #5's NPC interaction
    // needs to know which way the player is facing.
    public Direction Facing { get; private set; } = Direction.Down;

    public Player(Vector2 startPosition)
    {
        Position = startPosition;
    }

    public void Update(float dt, Tilemap map)
    {
        Vector2 input = ReadInput();
        if (input != Vector2.Zero)
        {
            Facing = FacingFromInput(input);
        }

        Position = MovementResolver.Resolve(Position, input, Speed, dt, map, ColliderSize);
    }

    public void Draw()
    {
        Raylib.DrawRectangle(
            (int)(Position.X - ColliderSize.X / 2f),
            (int)(Position.Y - ColliderSize.Y / 2f),
            (int)ColliderSize.X,
            (int)ColliderSize.Y,
            Color.Red);
    }

    private static Vector2 ReadInput()
    {
        var dir = Vector2.Zero;
        if (Raylib.IsKeyDown(KeyboardKey.Right) || Raylib.IsKeyDown(KeyboardKey.D)) dir.X += 1f;
        if (Raylib.IsKeyDown(KeyboardKey.Left) || Raylib.IsKeyDown(KeyboardKey.A)) dir.X -= 1f;
        if (Raylib.IsKeyDown(KeyboardKey.Down) || Raylib.IsKeyDown(KeyboardKey.S)) dir.Y += 1f;
        if (Raylib.IsKeyDown(KeyboardKey.Up) || Raylib.IsKeyDown(KeyboardKey.W)) dir.Y -= 1f;

        return dir == Vector2.Zero ? dir : Vector2.Normalize(dir);
    }

    private static Direction FacingFromInput(Vector2 input)
    {
        if (MathF.Abs(input.X) > MathF.Abs(input.Y))
        {
            return input.X > 0 ? Direction.Right : Direction.Left;
        }

        return input.Y > 0 ? Direction.Down : Direction.Up;
    }
}
