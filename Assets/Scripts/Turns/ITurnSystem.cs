using System;
public interface ITurnSystem : IService
{
    public PlayerCharacter PlayerOnTurn { get; }
    public event Action<PlayerCharacter> OnTurnChanged;

    public void StartInitialTurn();

    public void OnActionEnded();
}
