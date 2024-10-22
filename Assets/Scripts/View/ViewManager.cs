using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardLocation = IView.CardLocation;

public class ViewManager : MonoBehaviour, IView
{
    [SerializeField] private ViewPlayer _sagitario, _ygdra, _fungaloth, _overlord;
    [SerializeField] private TableCenter _tableCenter;
    [SerializeField] private GameConfig _config;

    private Dictionary<PlayerCharacter, ViewPlayer> _players;
    private bool _playersInitialized;
    private CameraMovement _cameraMovement;

    private void Awake()
    {
        if (!_playersInitialized) InitPlayers();
    }

    private void Start()
    {
        _cameraMovement = Camera.main.GetComponent<CameraMovement>();
    }


    public ViewPlayer GetViewPlayer(PlayerCharacter character)
    {
        if (!_playersInitialized) InitPlayers();
        return _players[character];
    }

    public void PlayCardOnSlot(ACard card, PlayerCharacter actor, CardLocation location, Action callback)
    {
        var playerActor = _players[actor];
        var playerSlotOwner = _players[location.SlotOwner];
        var slot = playerSlotOwner.Territory.Slots[location.SlotIndex];
        playerActor.PlayCardOnSlot(card, slot, callback);
    }


    public void GrowPopulationCard(CardLocation location, Action callback)
    {
        var playerOwner = _players[location.SlotOwner];
        var slot = playerOwner.Territory.Slots[location.SlotIndex]; //la carta de mas arriba del slot
        var card = slot.Cards[^1].Card;

        var newCardGO = Instantiate(_config.CardPrefab, slot.SnapTransform.position, slot.SnapTransform.rotation);
        var newPlayableCard = newCardGO.GetComponent<PlayableCard>();
        newPlayableCard.InitializeOnSlot(card, location.SlotOwner, slot);
        slot.AddCardOnTop(newPlayableCard);
        StartCoroutine(DelayCall(callback, 1f)); //de prueba
    }


    public void KillPopulationCard(CardLocation location, Action callback)
    {
        var playerOwner = _players[location.SlotOwner];
        var slot = playerOwner.Territory.Slots[location.SlotIndex];
        var card = slot.Cards[^1]; //la carta de mas arriba del slot
        if (card.Card is not PopulationCard) throw new Exception("Error! La carta a matar no es de poblacion");
        slot.RemoveCard(card);
        // StartCoroutine(DestroyCard(card.gameObject, callback));
        Destroy(card.gameObject);
        // callback?.Invoke();
        StartCoroutine(DelayCall(callback, 1f)); //de prueba
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

    public void GrowMushroom(CardLocation location, Action callback)
    {
        var card = _config.Mushroom;
        var playerOwner = _players[location.SlotOwner];
        var slot = playerOwner.Territory.Slots[location.SlotIndex];

        var newCardGO = Instantiate(_config.CardPrefab, slot.SnapTransform.position, slot.SnapTransform.rotation);
        var newPlayableCard = newCardGO.GetComponent<PlayableCard>();
        newPlayableCard.InitializeOnSlot(card, location.SlotOwner, slot);
        slot.AddCardAtTheBottom(newPlayableCard);
        StartCoroutine(DelayCall(callback, 1f)); //de prueba
    }

    public void GrowMacrofungi(CardLocation[] locations, Action callback)
    {
        if (locations.Length != 3) throw new Exception("Error! != 3 setas para macrohongo en view!");

        var token = GetViewPlayer(PlayerCharacter.Fungaloth).Token;
        token.Play(_tableCenter, () => { StartCoroutine(DestroyMushroomsAndGrowMacrofungi(locations, callback)); });
    }

    public void PlaceConstruction(CardLocation plant1Location, CardLocation plant2Location, Action callback)
    {
        var ownerPlayer = GetViewPlayer(plant1Location.SlotOwner);
        var territory = ownerPlayer.Territory;
        if (territory.HasConstruction) 
            throw new Exception("Error!!! Construyendo en territorio ya construido. (view)");

        var token = GetViewPlayer(PlayerCharacter.Overlord).Token;
        
        token.Play(_tableCenter, () =>
        {
            StartCoroutine(DestroyPlantsAndConstruct(plant1Location, plant2Location, callback));
        });
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
            if (character is PlayerCharacter.None) continue;
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
            playerActor.DrawCard(card, () => done = true);
            while (!done) yield return null;
        }

        callback?.Invoke();
    }
    
    private IEnumerator DestroyMushroomsAndGrowMacrofungi(CardLocation[] locations, Action callback)
    {
        CardLocation location = default;
        ViewPlayer playerOwner;
        SlotReceiver slot;
        foreach (var l in locations)
        {
            location = l;
            playerOwner = _players[location.SlotOwner];
            slot = playerOwner.Territory.Slots[location.SlotIndex];
            var card = slot.Cards[0]; //la carta de mas abajo del slot
            if (card.Card is not MushroomCard) throw new Exception("Error! La carta para macrohongo no es seta!");

            DestroyCard(card, slot);
            
            yield return new WaitForSeconds(.5f);
        }

        //la ultima location es donde crece el macrohongo
        playerOwner = _players[location.SlotOwner];
        slot = playerOwner.Territory.Slots[location.SlotIndex];

        var newCardGO = Instantiate(_config.CardPrefab, slot.SnapTransform.position, slot.SnapTransform.rotation);
        var newPlayableCard = newCardGO.GetComponent<PlayableCard>();
        newPlayableCard.InitializeOnSlot(_config.Macrofungi, location.SlotOwner, slot);
        slot.AddCardAtTheBottom(newPlayableCard);

        yield return new WaitForSeconds(.5f);
        callback?.Invoke();
    }

    private IEnumerator DestroyPlantsAndConstruct(CardLocation plant1Location, CardLocation plant2Location,
        Action callback)
    {
        var player = GetViewPlayer(plant1Location.SlotOwner);
        var territory = player.Territory;

        var slot1 = player.Territory.Slots[plant1Location.SlotIndex];
        var card1 = slot1.Cards[plant1Location.CardIndex];
        
        var slot2 = player.Territory.Slots[plant2Location.SlotIndex];
        var card2 = slot2.Cards[plant2Location.CardIndex];
        
        DestroyCard(card1, slot1);
        yield return new WaitForSeconds(.25f);
        DestroyCard(card2, slot2);
        yield return new WaitForSeconds(.25f);
        
        
        territory.BuildConstruction(_config.ConstructionPrefab);

        yield return new WaitForSeconds(.5f);
        callback?.Invoke();
    }

    private void DestroyCard(PlayableCard card, SlotReceiver slot)
    {
        slot.RemoveCard(card);
        Destroy(card.gameObject);
    }
}