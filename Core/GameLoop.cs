using Raylib_cs;

namespace NewGamePlus.Core;

public static class GameLoop
{
    public const int ScreenWidth = 800;
    public const int ScreenHeight = 600;
    public const string WindowTitle = "(Subject To Change Later) DarkWorld";

    private const double FixedTimestep = 1.0 / 60.0;

    public static void Run()
    {
        Raylib.InitWindow(ScreenWidth, ScreenHeight, WindowTitle);

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
    }

    private static void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.DarkGray);
        Raylib.EndDrawing();
    }
}
