using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour, IView
{

    [SerializeField] private ViewPlayer _sagitario, _ygdra, _fungaloth, _overlord;

    private Dictionary<PlayerCharacter, ViewPlayer> _players;

    private bool _playersInitialized;
    private void Awake()
    {
        if(!_playersInitialized) InitPlayers();
    }

    public ViewPlayer GetViewPlayer(PlayerCharacter character)
    {
        if(!_playersInitialized) InitPlayers();
        return _players[character];
    }

    private void InitPlayers()
    {
        _playersInitialized = true;
        
        _players = new()
        {
            { PlayerCharacter.Sagitario, _sagitario },
            { PlayerCharacter.Ygdra, _ygdra },
            { PlayerCharacter.Fungaloth, _fungaloth },
            { PlayerCharacter.Overlord, _overlord },
            { PlayerCharacter.None, null },
        };
        
        foreach (var (character, viewPlayer) in _players)
        {
            if(character is PlayerCharacter.None) continue;
            viewPlayer.Initialize(character);
        }
    }

    public void PlayCardOnSlot(PlayerCharacter actor, PlayerCharacter slotOwner, int cardIndex, int slotIndex, Action callback)
    {
        var playerActor = _players[actor];
        var playerSlotOwner = _players[slotOwner];
        var playableCard = playerActor.Cards[cardIndex];
        var slot = playerSlotOwner.Territory.Slots[slotIndex];
        
        playableCard.Play(slot, callback);
    }


    public void GrowPopulationCard(PlayerCharacter slotOwner, int slotIndex, Action callback)
    {
        
    }

    public void KillPopulationCard(PlayerCharacter slotOwner, int slotIndex, Action callback)
    {
        
    }

    public void Discard(PlayerCharacter actor, int cardIndex, Action callback)
    {
        var playerActor = _players[actor];
        var playableCard = playerActor.Cards[cardIndex];
        var discardPile = playerActor.DiscardPile;
        playableCard.Play(discardPile, () =>
        {
            StartCoroutine(DestroyCard(playableCard.gameObject));
            callback?.Invoke();
        });
    }

    private IEnumerator DestroyCard(GameObject card)
    {
        yield return null;
        Destroy(card);
    }

    public void DrawCards(PlayerCharacter actor, IReadOnlyList<ACard> cards, Action callback)
    {
        StartCoroutine(Draw(actor, cards, callback));
    }

    private IEnumerator Draw(PlayerCharacter actor, IReadOnlyList<ACard> cards, Action callback)
    {
        var playerActor = _players[actor];
        foreach (var card in cards)
        {
            bool done = false;
            playerActor.DrawCard(card,() => done = true);
            while (!done) yield return null;
        }
        callback?.Invoke();
    }
}