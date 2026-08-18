using System.Numerics;
using NewGamePlus.World;

namespace NewGamePlus.Entities;

public static class InteractionChecker
{
    // ~1.25 tiles: one tile of reach plus a small buffer so free-pixel
    // movement doesn't require pixel-perfect alignment to interact.
    public const float InteractionRange = Tilemap.TileSize * 1.25f;

    public static bool CanInteract(Vector2 playerPosition, Direction playerFacing, Vector2 targetPosition)
    {
        Vector2 toTarget = targetPosition - playerPosition;
        if (toTarget.Length() > InteractionRange)
        {
            return false;
        }

        return DirectionTo(toTarget) == playerFacing;
    }

    private static Direction DirectionTo(Vector2 delta)
    {
        if (MathF.Abs(delta.X) > MathF.Abs(delta.Y))
        {
            return delta.X > 0 ? Direction.Right : Direction.Left;
        }

        return delta.Y > 0 ? Direction.Down : Direction.Up;
    }
}
