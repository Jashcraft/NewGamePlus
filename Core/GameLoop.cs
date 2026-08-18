using System.Numerics;
using NewGamePlus.World;
using Raylib_cs;

namespace NewGamePlus.Core;

public static class GameLoop
{
    public const int ScreenWidth = 800;
    public const int ScreenHeight = 600;
    public const string WindowTitle = "(Subject To Change Later) DarkWorld";

    // Throwaway until a real player entity drives the camera (M3).
    private const float DebugCameraPanSpeed = 200f;

    private const double FixedTimestep = 1.0 / 60.0;

    private static Tilemap s_testMap = null!;
    private static Camera2D s_camera;

    public static void Run()
    {
        Raylib.InitWindow(ScreenWidth, ScreenHeight, WindowTitle);

        s_testMap = Tilemap.CreateTestMap();
        s_camera = new Camera2D
        {
            Target = Vector2.Zero,
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

        // Temporary arrow-key camera pan, just to prove BeginMode2D/EndMode2D
        // panning works. Replace with real player-driven camera in M3.
        var pan = Vector2.Zero;
        if (Raylib.IsKeyDown(KeyboardKey.Right)) pan.X += 1f;
        if (Raylib.IsKeyDown(KeyboardKey.Left)) pan.X -= 1f;
        if (Raylib.IsKeyDown(KeyboardKey.Down)) pan.Y += 1f;
        if (Raylib.IsKeyDown(KeyboardKey.Up)) pan.Y -= 1f;

        s_camera.Target += pan * DebugCameraPanSpeed * dt;
    }

    private static void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        Raylib.BeginMode2D(s_camera);
        TilemapRenderer.Draw(s_testMap);
        Raylib.EndMode2D();

        Raylib.EndDrawing();
    }
}
