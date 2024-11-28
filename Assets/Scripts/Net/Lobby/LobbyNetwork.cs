using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyNetwork : NetworkBehaviour
{
    public event System.Action<int> OnPlayerCountChange;
    private NetworkVariable<int> _playerCount = new(0);
    
    //server only
    private Dictionary<PlayerCharacter, NetworkPlayer> _networkPlayers = new();
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Host updates player count when players connect/disconnect
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }
        
        _playerCount.OnValueChanged += OnPlayerCountChanged;
        
        base.OnNetworkSpawn();
    }
    
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
        _playerCount.OnValueChanged -= OnPlayerCountChanged;
        
        base.OnNetworkDespawn();
    }

    private void OnClientConnected(ulong clientID)
    {
        if (IsServer) _playerCount.Value++;
    }
    
    private void OnClientDisconnected(ulong clientID)
    {
        if (IsServer) _playerCount.Value--;
    }

    private void OnPlayerCountChanged(int lastV, int newV)
    {
        OnPlayerCountChange?.Invoke(newV);
        
        if(newV != 4 || !IsServer) return;

        var players =
            new List<NetworkPlayer>(FindObjectsByType<NetworkPlayer>(FindObjectsInactive.Exclude,
                FindObjectsSortMode.None));
        var characters = new List<PlayerCharacter>(new[]
            { PlayerCharacter.Ygdra, PlayerCharacter.Sagitario, PlayerCharacter.Fungaloth, PlayerCharacter.Overlord });

        while (players.Any())
        {
            int randomIndex = Random.Range(0, players.Count);
            Debug.Log($"random: {randomIndex}");
            var player = players[randomIndex];
            players.RemoveAt(randomIndex);
            var character = characters[^1];
            characters.RemoveAt(characters.Count - 1);
            _networkPlayers.Add(character, player);
            player.SetCharacter(character);
        }


        // NetworkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        SceneTransition.Instance.TransitionToScene("Game", true);


    }
}
