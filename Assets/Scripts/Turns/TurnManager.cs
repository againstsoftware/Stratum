using System;
using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour, ITurnSystem
{
    public PlayerCharacter PlayerOnTurn { get; private set; } = PlayerCharacter.None;
    public event Action<PlayerCharacter> OnTurnChanged;

    [SerializeField] private GameConfig _config;
    private int _orderIdx = -1;
    private int _actionsLeft;
    private PlayerCharacter[] _order;
    private int _numberOfActions;

    private bool _turnCompleted;

    private void Awake()
    {
        _order = _config.TurnOrder;
        _numberOfActions = _config.ActionsPerTurn;
    }



    public void StartInitialTurn()
    {
        _orderIdx = 0;
        PlayerOnTurn = _order[_orderIdx];
        _actionsLeft = _numberOfActions;
        OnTurnChanged?.Invoke(PlayerOnTurn);
    }

    public void OnActionEnded()
    {
        _actionsLeft--;
        if (_actionsLeft > 0) return;
        NextTurn(); 
    }

    private void NextTurn() // solo en la autoridad (server)
    {
        _turnCompleted = true;
        
        if (!ServiceLocator.Get<ICommunicationSystem>().IsAuthority) return;

        _orderIdx = (_orderIdx + 1) % _order.Length;
        PlayerOnTurn = _order[_orderIdx];
        _actionsLeft = _numberOfActions;
        
        ServiceLocator.Get<ICommunicationSystem>().SendTurnChange(PlayerOnTurn);
    }

    public void ChangeTurn(PlayerCharacter playerOnTurn) //en los clientes
    {
        StartCoroutine(WaitExecutionAndChangeTurn(playerOnTurn));
    }

    private IEnumerator WaitExecutionAndChangeTurn(PlayerCharacter playerOnTurn)
    {
        while (!_turnCompleted) yield return null;
        
        PlayerOnTurn = playerOnTurn;
        OnTurnChanged?.Invoke(PlayerOnTurn);

        _turnCompleted = false;
    }


}