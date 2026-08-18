using NewGamePlus.States;
using Raylib_cs;

namespace NewGamePlus.Core;

public static class GameLoop
{
    public const int ScreenWidth = 800;
    public const int ScreenHeight = 600;
    public const string WindowTitle = "(Subject To Change Later) DarkWorld";

    private const double FixedTimestep = 1.0 / 60.0;

    private static StateStack s_stack = null!;

    public static void Run()
    {
        Raylib.InitWindow(ScreenWidth, ScreenHeight, WindowTitle);

        s_stack = new StateStack();
        s_stack.Push(new OverworldState(s_stack));

        // One persistent snapshot for the whole run - a press latches in
        // on whichever real frame it happens and carries forward until an
        // Update() call actually consumes it, even if that's several real
        // frames later (see InputSnapshot's comment for why that matters).
        var input = new InputSnapshot();

        double accumulator = 0.0;

        while (!Raylib.WindowShouldClose())
        {
            accumulator += Raylib.GetFrameTime();
            input.CaptureFrame(KeyboardKey.E, KeyboardKey.Enter);

            while (accumulator >= FixedTimestep)
            {
                s_stack.Update((float)FixedTimestep, input);
                accumulator -= FixedTimestep;
            }

            Draw();
        }

        Raylib.CloseWindow();
    }

    private static void Draw()
    {
        Raylib.BeginDrawing();
        s_stack.Draw();
        Raylib.EndDrawing();
    }
}
