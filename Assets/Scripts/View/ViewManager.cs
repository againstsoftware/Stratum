using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour, IView
{

    [SerializeField] private ViewPlayer _sagitario, _ygdra, _fungaloth, _overlord;
    [SerializeField] private GameConfig _config;
    
    private Dictionary<PlayerCharacter, ViewPlayer> _players;
    private bool _playersInitialized;
    private CameraMovement _cameraMovement;
    
    private void Awake()
    {
        if(!_playersInitialized) InitPlayers();
    }

    private void Start()
    {
        _cameraMovement = Camera.main.GetComponent<CameraMovement>();
    }
    
    
    

    public ViewPlayer GetViewPlayer(PlayerCharacter character)
    {
        if(!_playersInitialized) InitPlayers();
        return _players[character];
    }

    public void PlayCardOnSlot(ACard card, PlayerCharacter actor, PlayerCharacter slotOwner, int slotIndex, Action callback)
    {
        var playerActor = _players[actor];
        var playerSlotOwner = _players[slotOwner];
        var slot = playerSlotOwner.Territory.Slots[slotIndex];
        playerActor.PlayCardOnSlot(card, slot, callback);
    }


    public void GrowPopulationCard(PlayerCharacter slotOwner, int slotIndex, Action callback)
    {
        var playerOwner = _players[slotOwner];
        var slot = playerOwner.Territory.Slots[slotIndex]; //la carta de mas arriba del slot
        var card = slot.Cards[^1].Card;
        
        var newCardGO = Instantiate(_config.CardPrefab, slot.SnapTransform.position, slot.SnapTransform.rotation);
        var newPlayableCard = newCardGO.GetComponent<PlayableCard>();
        newPlayableCard.Initialize(card, slotOwner, APlayableItem.State.Played);
        slot.AddCardOnTop(newPlayableCard);
        StartCoroutine(DelayCall(callback, .5f)); //de prueba
    }


    public void KillPopulationCard(PlayerCharacter slotOwner, int slotIndex, Action callback)
    {
        var playerOwner = _players[slotOwner];
        var slot = playerOwner.Territory.Slots[slotIndex]; //la carta de mas arriba del slot
        var card = slot.Cards[^1];
        slot.RemoveCard(card);
        // StartCoroutine(DestroyCard(card.gameObject, callback));
        Destroy(card);
        callback?.Invoke();
    }

    public void Discard(PlayerCharacter actor, Action callback)
    {
        var playerActor = _players[actor];
        playerActor.DiscardCard(callback);
    }

    public void DrawCards(PlayerCharacter actor, IReadOnlyList<ACard> cards, Action callback)
    {
        StartCoroutine(Draw(actor, cards, callback));
    }

    public void SwitchCamToOverview(Action callback)
    {
        _cameraMovement.ChangeToOverview(callback);
    }

    // private IEnumerator DestroyCard(GameObject card, Action callback = null)
    // {
    //     yield return null;
    //     Destroy(card);
    //     callback?.Invoke();
    // }
    
    
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
    
    private IEnumerator DelayCall(Action a, float delay)
    {
        yield return new WaitForSeconds(delay);
        a?.Invoke();   
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