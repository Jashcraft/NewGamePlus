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
    private readonly List<Npc> _npcs;
    private Camera2D _camera;

    public OverworldState(StateStack stack)
    {
        _stack = stack;
        _map = Tilemap.CreateTestMap();
        _player = new Player(new Vector2(2 * Tilemap.TileSize + Tilemap.TileSize / 2f, 2 * Tilemap.TileSize + Tilemap.TileSize / 2f));
        var npcPosition = new Vector2(10 * Tilemap.TileSize + Tilemap.TileSize / 2f, 3 * Tilemap.TileSize + Tilemap.TileSize / 2f);
        _npcs = new List<Npc>
        {
            new(
                npcPosition,
                new[]
                {
                    "Hey there! Watch out for the water to the south.",
                    "This map's just a placeholder for now, but hey - dialogue works!",
                }),
        };
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

    public void Update(float dt, InputSnapshot input)
    {
        _player.Update(dt, _map);
        _camera.Target = _player.Position;

        if (input.WasPressed(KeyboardKey.E))
        {
            Npc? target = _npcs.Find(npc => InteractionChecker.CanInteract(_player.Position, _player.Facing, npc.Position));
            if (target != null)
            {
                _stack.Push(new DialogueState(_stack, target.DialogueLines));
            }
        }
    }

    public void Draw()
    {
        Raylib.ClearBackground(Color.Black);

        Raylib.BeginMode2D(_camera);
        TilemapRenderer.Draw(_map);
        foreach (var npc in _npcs)
        {
            npc.Draw();
        }
        _player.Draw();
        Raylib.EndMode2D();
    }
}
