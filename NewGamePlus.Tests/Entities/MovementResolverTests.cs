using System.Numerics;
using NewGamePlus.Entities;
using NewGamePlus.World;

namespace NewGamePlus.Tests.Entities;

public class MovementResolverTests
{
    private static readonly Vector2 ColliderSize = new(32, 32);
    private const float Speed = 100f;
    private const float Dt = 0.1f;

    private static Tilemap BuildOpenMap()
    {
        var tiles = new int[5, 5];
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                tiles[x, y] = TileIds.Grass;
            }
        }

        return new Tilemap(tiles);
    }

    private static Vector2 TileCenter(int tileX, int tileY)
    {
        return new Vector2(
            tileX * Tilemap.TileSize + Tilemap.TileSize / 2f,
            tileY * Tilemap.TileSize + Tilemap.TileSize / 2f);
    }

    [Fact]
    public void Resolve_MovesPosition_WhenPathClear()
    {
        var map = BuildOpenMap();
        var start = TileCenter(2, 2);
        var input = new Vector2(1f, 0f);

        var result = MovementResolver.Resolve(start, input, Speed, Dt, map, ColliderSize);

        Assert.Equal(start.X + Speed * Dt, result.X, precision: 3);
        Assert.Equal(start.Y, result.Y, precision: 3);
    }

    [Fact]
    public void Resolve_ReturnsSamePosition_WhenNoInput()
    {
        var map = BuildOpenMap();
        var start = TileCenter(2, 2);

        var result = MovementResolver.Resolve(start, Vector2.Zero, Speed, Dt, map, ColliderSize);

        Assert.Equal(start, result);
    }

    [Fact]
    public void Resolve_BlocksMovement_WhenWallDirectlyAhead()
    {
        var tiles = new int[5, 5];
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                tiles[x, y] = TileIds.Grass;
            }
        }
        tiles[3, 2] = TileIds.Wall;
        var map = new Tilemap(tiles);

        // Start right against the wall's left edge so a small rightward step
        // would push the collider into it.
        var start = new Vector2(3 * Tilemap.TileSize - ColliderSize.X / 2f - 1f, TileCenter(2, 2).Y);
        var input = new Vector2(1f, 0f);

        var result = MovementResolver.Resolve(start, input, Speed, Dt, map, ColliderSize);

        Assert.Equal(start, result);
    }

    [Fact]
    public void Resolve_SlidesAlongWall_WhenDiagonalMoveBlockedOnOneAxisOnly()
    {
        var tiles = new int[5, 5];
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                tiles[x, y] = TileIds.Grass;
            }
        }
        tiles[3, 2] = TileIds.Wall;
        var map = new Tilemap(tiles);

        // Positioned close enough to the wall on the X axis that a diagonal
        // step pushes into it, while the Y axis stays open the whole time.
        var start = new Vector2(3 * Tilemap.TileSize - ColliderSize.X / 2f - 1f, TileCenter(2, 2).Y);
        var input = Vector2.Normalize(new Vector2(1f, 1f));

        var result = MovementResolver.Resolve(start, input, Speed, Dt, map, ColliderSize);

        Assert.Equal(start.X, result.X, precision: 3);
        Assert.Equal(start.Y + input.Y * Speed * Dt, result.Y, precision: 3);
    }
}
