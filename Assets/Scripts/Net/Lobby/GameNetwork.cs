
using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameNetwork : MonoBehaviour, INetworkSystem
{
    private void Awake()
    {
        var localID = NetworkManager.Singleton.LocalClientId;

        var networkPlayers = FindObjectsByType<NetworkPlayer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        
        var localNetworkPlayer = networkPlayers.FirstOrDefault(np => np.ID.Value == localID);
        var localCharacter = localNetworkPlayer.Character.Value;
        foreach (PlayerCharacter character in Enum.GetValues(typeof(PlayerCharacter)))
        {
            if(character == localCharacter || character is PlayerCharacter.None) continue;
            var cam = ServiceLocator.Get<IView>().GetViewPlayer(character).Camera;
            Destroy(cam.gameObject);
        
        }

        ServiceLocator.Get<IInteractionSystem>().LocalPlayer = localCharacter;
    }
}
