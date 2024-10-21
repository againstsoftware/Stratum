
using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
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
            if (character is PlayerCharacter.None) continue;

            var viewPlayer = ServiceLocator.Get<IView>().GetViewPlayer(character);
            var cam = viewPlayer.Camera;
            
            if (character != _localPlayer)
            {
                Destroy(cam.gameObject);
            }
            else
            {
                //temporal mientras no hay mas luces
                var lightTransform = FindAnyObjectByType<Light>().transform.parent;
                lightTransform.LookAt(cam.transform.forward);
                
                var input = FindAnyObjectByType<PlayerInput>().camera = cam;

                viewPlayer.IsLocalPlayer = true;
            }
        
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




    public void SendActionToAuthority(PlayerAction action)
    {
        if (!IsSpawned)
            throw new Exception("Error! Mandando rpcs sin estar spawneado");

        if (action.Actor != _localPlayer)
            throw new Exception("Error! Mandando rpcs de accion sin ser el jugador local");

        ServiceLocator.Get<IExecutor>().ExecutePlayerActionEffects(action);
        if (IsServer) //si somos servidor no hace falta recheck, solo mandamos la accion a los clientes
        {
            SendActionToExecuteInClientRpc(new NetworkPlayerAction(action, _config));
        }
        else // pero si no somos server, enviamos la accion al server para check de trampas
        {
            SendActionToServerRpc(new NetworkPlayerAction(action, _config));
        }

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
    private void SendActionToServerRpc(NetworkPlayerAction networkAction) //comporueba las reglas en el server 
    {
        var action = networkAction.ToPlayerAction(_config);
        
        if (!ServiceLocator.Get<IRulesSystem>().IsValidAction(action))
            throw new Exception($"JUGADOR {action.Actor} HA HECHO TRAMPA! HAY INCONSISTENCIAS!");
       
        else SendActionToExecuteInClientRpc(networkAction);
    }

    [ClientRpc]
    private void SendActionToExecuteInClientRpc(NetworkPlayerAction action)
    {
        if (action.Actor == _localPlayer) return;
        
        ServiceLocator.Get<IExecutor>().ExecutePlayerActionEffects(action.ToPlayerAction(_config));
    }
}
