using NewGamePlus.Core;
using Raylib_cs;

namespace NewGamePlus.States;

public class DialogueState : IGameState
{
    private const int BoxHeight = 120;
    private const int BoxMargin = 20;

    private readonly StateStack _stack;
    private readonly DialogueProgress _progress;

    public DialogueState(StateStack stack, IReadOnlyList<string> lines)
    {
        _stack = stack;
        _progress = new DialogueProgress(lines);
    }

    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

    public void Update(float dt, InputSnapshot input)
    {
        if (input.WasPressed(KeyboardKey.Enter))
        {
            // Enter on the last line closes the dialogue directly, rather
            // than requiring one extra press after it's shown.
            if (!_progress.Advance())
            {
                _stack.Pop();
            }
        }
    }

    public void Draw()
    {
        // Deliberately does not ClearBackground - the frozen Overworld
        // below should stay visible behind this box.
        int boxY = GameLoop.ScreenHeight - BoxHeight - BoxMargin;
        int boxWidth = GameLoop.ScreenWidth - BoxMargin * 2;

        Raylib.DrawRectangle(BoxMargin, boxY, boxWidth, BoxHeight, Color.DarkBlue);
        Raylib.DrawRectangleLines(BoxMargin, boxY, boxWidth, BoxHeight, Color.White);
        Raylib.DrawText(_progress.CurrentLine, BoxMargin + 20, boxY + 20, 20, Color.White);
    }
}
