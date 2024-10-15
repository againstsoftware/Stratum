using System;
public interface ITurnSystem : IService
{
    public PlayerCharacter PlayerOnTurn { get; }
    public event Action<PlayerCharacter> OnTurnChanged;
    public event Action OnGameStart;

    public void StartGame();

    public void OnActionEnded();

    public void ChangeTurn(PlayerCharacter playerOnTurn);
}
