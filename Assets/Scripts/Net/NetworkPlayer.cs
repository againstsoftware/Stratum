using System;
using UnityEngine;
using Unity.Netcode;
public class NetworkPlayer : NetworkBehaviour
{
    public NetworkVariable<PlayerCharacter> Character = new(PlayerCharacter.None);
    [HideInInspector]
    public NetworkVariable<ulong> ID = new(0);


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Character.OnValueChanged += (_, pc) => Debug.Log($"player {ID.Value} is {Character.Value}");
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) ID.Value = GetComponent<NetworkObject>().OwnerClientId;
    }
    
    //server code
    public void SetCharacter(PlayerCharacter character)
    {
        if (IsServer) Character.Value = character;
    }
}
