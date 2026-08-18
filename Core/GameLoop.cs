using System.Numerics;
using NewGamePlus.Entities;
using NewGamePlus.World;
using Raylib_cs;

namespace NewGamePlus.Core;

public static class GameLoop
{
    public const int ScreenWidth = 800;
    public const int ScreenHeight = 600;
    public const string WindowTitle = "(Subject To Change Later) DarkWorld";

    private const double FixedTimestep = 1.0 / 60.0;

    private static Tilemap s_testMap = null!;
    private static Player s_player = null!;
    private static Camera2D s_camera;

    public static void Run()
    {
        Raylib.InitWindow(ScreenWidth, ScreenHeight, WindowTitle);

        s_testMap = Tilemap.CreateTestMap();
        s_player = new Player(new Vector2(2 * Tilemap.TileSize + Tilemap.TileSize / 2f, 2 * Tilemap.TileSize + Tilemap.TileSize / 2f));
        s_camera = new Camera2D
        {
            Target = s_player.Position,
            Offset = new Vector2(ScreenWidth / 2f, ScreenHeight / 2f),
            Rotation = 0f,
            Zoom = 1f,
        };

        double accumulator = 0.0;

        while (!Raylib.WindowShouldClose())
        {
            accumulator += Raylib.GetFrameTime();

            while (accumulator >= FixedTimestep)
            {
                Update((float)FixedTimestep);
                accumulator -= FixedTimestep;
            }

            Draw();
        }

        Raylib.CloseWindow();
    }

    private static void Update(float dt)
    {
        // State stack update goes here once States are implemented (M4).

        s_player.Update(dt, s_testMap);
        s_camera.Target = s_player.Position;
    }

    private static void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        Raylib.BeginMode2D(s_camera);
        TilemapRenderer.Draw(s_testMap);
        s_player.Draw();
        Raylib.EndMode2D();

        Raylib.EndDrawing();
    }
}
