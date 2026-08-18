using System.Numerics;
using NewGamePlus.World;

namespace NewGamePlus.Tests.World;

public class TileCollisionTests
{
    // 5x5 map, all grass except: wall at (2,2), water at (3,2).
    private static Tilemap BuildMap()
    {
        var tiles = new int[5, 5];
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                tiles[x, y] = TileIds.Grass;
            }
        }

        tiles[2, 2] = TileIds.Wall;
        tiles[3, 2] = TileIds.Water;

        return new Tilemap(tiles);
    }

    private static readonly Vector2 ColliderSize = new(32, 32);

    private static Vector2 TileCenter(int tileX, int tileY)
    {
        return new Vector2(
            tileX * Tilemap.TileSize + Tilemap.TileSize / 2f,
            tileY * Tilemap.TileSize + Tilemap.TileSize / 2f);
    }

    [Fact]
    public void IsBlocked_ReturnsFalse_OnOpenGrass()
    {
        var map = BuildMap();

        Assert.False(TileCollision.IsBlocked(map, TileCenter(1, 1), ColliderSize));
    }

    [Fact]
    public void IsBlocked_ReturnsTrue_OnWallTile()
    {
        var map = BuildMap();

        Assert.True(TileCollision.IsBlocked(map, TileCenter(2, 2), ColliderSize));
    }

    [Fact]
    public void IsBlocked_ReturnsTrue_OnWaterTile()
    {
        var map = BuildMap();

        Assert.True(TileCollision.IsBlocked(map, TileCenter(3, 2), ColliderSize));
    }

    [Fact]
    public void IsBlocked_ReturnsTrue_PastMapBoundary()
    {
        var map = BuildMap();

        // Well outside the 5x5 map -> Void.
        Assert.True(TileCollision.IsBlocked(map, TileCenter(10, 10), ColliderSize));
    }

    [Fact]
    public void IsBlocked_ReturnsFalse_WhenColliderOnlyTouchesAdjacentOpenTile()
    {
        var map = BuildMap();

        // Collider centered just to the left of the wall tile (2,2), fully
        // inside tile (1,2) with no overlap into the wall's tile bounds.
        var position = TileCenter(1, 2);

        Assert.False(TileCollision.IsBlocked(map, position, ColliderSize));
    }

    [Fact]
    public void IsBlocked_ReturnsTrue_WhenColliderOverlapsWallAcrossTileBoundary()
    {
        var map = BuildMap();

        // Collider straddling the boundary between open tile (1,2) and wall
        // tile (2,2) - half the collider is inside the wall tile.
        var wallLeftEdge = 2 * Tilemap.TileSize;
        var position = new Vector2(wallLeftEdge, TileCenter(1, 2).Y);

        Assert.True(TileCollision.IsBlocked(map, position, ColliderSize));
    }
}
