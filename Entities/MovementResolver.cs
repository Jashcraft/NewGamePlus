using System.Numerics;
using NewGamePlus.World;

namespace NewGamePlus.Entities;

public static class MovementResolver
{
    // Resolves X and Y independently so movement into a corner slides along
    // the open axis instead of stopping dead on diagonal input.
    public static Vector2 Resolve(Vector2 position, Vector2 inputDirection, float speed, float dt, Tilemap map, Vector2 colliderSize)
    {
        Vector2 result = position;

        float deltaX = inputDirection.X * speed * dt;
        if (deltaX != 0f)
        {
            var candidate = new Vector2(result.X + deltaX, result.Y);
            if (!TileCollision.IsBlocked(map, candidate, colliderSize))
            {
                result.X = candidate.X;
            }
        }

        float deltaY = inputDirection.Y * speed * dt;
        if (deltaY != 0f)
        {
            var candidate = new Vector2(result.X, result.Y + deltaY);
            if (!TileCollision.IsBlocked(map, candidate, colliderSize))
            {
                result.Y = candidate.Y;
            }
        }

        return result;
    }
}
