
using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameNetwork : NetworkBehaviour, ICommunicationSystem
{
    public bool IsAuthority
    {
        get
        {
            if (!IsSpawned)
                throw new Exception("Error! Comprobando autoridad sin estar spawneado");
            return IsServer;
        }
    }

    public bool IsRNGSynced { get; private set; }

    [SerializeField] private GameConfig _config;

    private PlayerCharacter _localPlayer;
    private void Awake()
    {
        var localID = NetworkManager.Singleton.LocalClientId;

        var networkPlayers = FindObjectsByType<NetworkPlayer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        
        var localNetworkPlayer = networkPlayers.FirstOrDefault(np => np.ID.Value == localID);
        _localPlayer = localNetworkPlayer.Character.Value;
        foreach (PlayerCharacter character in Enum.GetValues(typeof(PlayerCharacter)))
        {
            if(character == _localPlayer || character is PlayerCharacter.None) continue;
            var cam = ServiceLocator.Get<IView>().GetViewPlayer(character).Camera;
            Destroy(cam.gameObject);
        
        }

        ServiceLocator.Get<IInteractionSystem>().LocalPlayer = _localPlayer;
    }

    public void SyncRNGs()
    {
        if (!IsServer) return;
        GenerateSeedServerRpc();
    }

    
    
    [ServerRpc]
    private void GenerateSeedServerRpc()
    {
        var seed = Guid.NewGuid().GetHashCode();
        SendSeedToClientRpc(seed);
    }

    [ClientRpc]
    private void SendSeedToClientRpc(int seed)
    {
        Random.InitState(seed);
        IsRNGSynced = true;
    }


    private struct NetworkPlayerAction : INetworkSerializable
    {
        public PlayerCharacter Actor;
        public int ActionItemID;
        public Receiver[] Receivers;
        public int CardIndexInHand;

        public NetworkPlayerAction(PlayerAction action, GameConfig config)
        {
            Actor = action.Actor;
            Receivers = action.Receivers;
            CardIndexInHand = action.CardIndexInHand;
            ActionItemID = config.ActionItemToID(action.ActionItem);
        }

        public PlayerAction ToPlayerAction(GameConfig config)
        {
            return new PlayerAction(Actor, config.IDToActionItem(ActionItemID), Receivers, CardIndexInHand);
        }
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Actor);
            serializer.SerializeValue(ref ActionItemID);
            serializer.SerializeValue(ref Receivers);
            serializer.SerializeValue(ref CardIndexInHand);
        }
    }
    
    
    public void SendActionToAuthority(PlayerAction action)
    {
        if (!IsSpawned)
            throw new Exception("Error! Mandando rpcs sin estar spawneado");

        if (action.Actor == _localPlayer)
            ServiceLocator.Get<IExecutor>().ExecutePlayerActionEffects(action);
        
        
        if (IsServer) 
            CheckRulesThenExec(action);
        else 
            SendActionToServerRpc(new NetworkPlayerAction(action, _config));
        
    }

    public void SendTurnChange(PlayerCharacter playerOnTurn)
    {
        SendTurnChangeToClientRpc(playerOnTurn);
    }

    [ClientRpc]
    private void SendTurnChangeToClientRpc(PlayerCharacter playerOnTurn)
    {
        ServiceLocator.Get<ITurnSystem>().ChangeTurn(playerOnTurn);
    }


    [ServerRpc(RequireOwnership = false)]
    private void SendActionToServerRpc(NetworkPlayerAction action)
    {
        CheckRulesThenExec(action.ToPlayerAction(_config));
    }

    //Solo invocado en el SERVER
    private void CheckRulesThenExec(PlayerAction action)
    {
        if (!ServiceLocator.Get<IRulesSystem>().IsValidAction(action))
            throw new Exception($"JUGADOR {action.Actor} HA HECHO TRAMPA! HAY INCONSISTENCIAS!");
       
        SendActionToExecuteInClientRpc(new NetworkPlayerAction(action, _config));
    }

    [ClientRpc]
    private void SendActionToExecuteInClientRpc(NetworkPlayerAction action)
    {
        if (action.Actor == _localPlayer) return;
        
        ServiceLocator.Get<IExecutor>().ExecutePlayerActionEffects(action.ToPlayerAction(_config));
    }
}
