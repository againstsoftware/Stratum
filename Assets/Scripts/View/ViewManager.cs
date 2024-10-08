using System;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour, IView
{

    [SerializeField] private ViewPlayer _sagitario, _ygdra, _fungaloth, _overlord;

    private Dictionary<PlayerCharacter, ViewPlayer> _players;
    private void Awake()
    {
        _players = new()
        {
            { PlayerCharacter.Sagitario, _sagitario },
            { PlayerCharacter.Ygdra, _ygdra },
            { PlayerCharacter.Fungaloth, _fungaloth },
            { PlayerCharacter.Overlord, _overlord },
            { PlayerCharacter.None, null },
        };
    }

    public void PlayCardOnSlot(PlayerCharacter actor, ACard card, int cardIndex, int slotIndex, Action callback)
    {
        var playerActor = _players[actor];

        var slot = playerActor.Territory.Slots[slotIndex];
        var playableCard = playerActor.HandOfCards[cardIndex];
        
        playableCard.Play(slot, callback);
    }
}