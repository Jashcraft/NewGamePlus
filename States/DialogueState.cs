using NewGamePlus.Core;
using Raylib_cs;

namespace NewGamePlus.States;

// Placeholder for Ticket #5's real dialogue system - proves the state
// stack's push/pop mechanics work end-to-end.
public class DialogueState : IGameState
{
    private const int BoxHeight = 120;
    private const int BoxMargin = 20;

    private readonly StateStack _stack;

    public DialogueState(StateStack stack)
    {
        _stack = stack;
    }

    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

    public void Update(float dt)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Enter))
        {
            _stack.Pop();
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
        Raylib.DrawText("Dialogue placeholder - press Enter to close", BoxMargin + 20, boxY + 20, 20, Color.White);
    }
}
