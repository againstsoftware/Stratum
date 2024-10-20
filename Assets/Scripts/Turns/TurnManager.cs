using System;
using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour, ITurnSystem
{
    public PlayerCharacter PlayerOnTurn { get; private set; } = PlayerCharacter.None;
    public event Action<PlayerCharacter> OnTurnChanged;
    public event Action OnGameStart;


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



    public void StartGame()
    {
        // _orderIdx = 0;
        // PlayerOnTurn = _order[_orderIdx];
        // _actionsLeft = _numberOfActions;
        // OnTurnChanged?.Invoke(PlayerOnTurn);


        _orderIdx = -1;
        _actionsLeft = 1;
        PlayerOnTurn = PlayerCharacter.None;
        OnGameStart?.Invoke();
    }

    public void OnActionEnded() //lo llama el ejecutor de comandos
    {
        _actionsLeft--;
        if (_actionsLeft == 0 || PlayerOnTurn is PlayerCharacter.None) NextTurn(); 
    }

    private void NextTurn() 
    {
        _turnCompleted = true;
        
        if (!ServiceLocator.Get<ICommunicationSystem>().IsAuthority) return; // solo en la autoridad (server)

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
        while (!_turnCompleted) yield return null; //si todavia no se han ejecutado todos los efectos se espera
        
        PlayerOnTurn = playerOnTurn;
        _actionsLeft = _numberOfActions;
        ServiceLocator.Get<IModel>().AdvanceTurn(PlayerOnTurn);
        OnTurnChanged?.Invoke(PlayerOnTurn);
        _turnCompleted = false;
    }


}