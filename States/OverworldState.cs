using System.Numerics;
using NewGamePlus.Core;
using NewGamePlus.Entities;
using NewGamePlus.World;
using Raylib_cs;

namespace NewGamePlus.States;

public class OverworldState : IGameState
{
    private readonly StateStack _stack;
    private readonly Tilemap _map;
    private readonly Player _player;
    private Camera2D _camera;

    public OverworldState(StateStack stack)
    {
        _stack = stack;
        _map = Tilemap.CreateTestMap();
        _player = new Player(new Vector2(2 * Tilemap.TileSize + Tilemap.TileSize / 2f, 2 * Tilemap.TileSize + Tilemap.TileSize / 2f));
        _camera = new Camera2D
        {
            Target = _player.Position,
            Offset = new Vector2(GameLoop.ScreenWidth / 2f, GameLoop.ScreenHeight / 2f),
            Rotation = 0f,
            Zoom = 1f,
        };
    }

    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

    public void Update(float dt)
    {
        _player.Update(dt, _map);
        _camera.Target = _player.Position;

        // Throwaway trigger to prove the state stack push/pop works
        // end-to-end. Real NPC-triggered dialogue is Ticket #5.
        if (Raylib.IsKeyPressed(KeyboardKey.E))
        {
            _stack.Push(new DialogueState(_stack));
        }
    }

    public void Draw()
    {
        Raylib.ClearBackground(Color.Black);

        Raylib.BeginMode2D(_camera);
        TilemapRenderer.Draw(_map);
        _player.Draw();
        Raylib.EndMode2D();
    }
}
