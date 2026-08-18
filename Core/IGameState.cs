namespace NewGamePlus.Core;

public interface IGameState
{
    void OnEnter();
    void OnExit();
    void Update(float dt);
    void Draw();
}
