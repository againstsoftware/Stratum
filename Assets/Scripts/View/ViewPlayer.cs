using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ViewPlayer : MonoBehaviour
{
    [field: SerializeField] public TerritoryReceiver Territory { get; private set; }
    [field: SerializeField] public DiscardPileReceiver DiscardPile { get; private set; }
    [field: SerializeField] public PlayableToken Token { get; private set; }
    [field: SerializeField] public Camera Camera { get; private set; }

    public readonly List<PlayableCard> Cards = new(5);
    public bool IsLocalPlayer { get; set; }

    public PlayerCharacter Character { get; private set; } = PlayerCharacter.None;

    [SerializeField] private Transform _hand;
    [SerializeField] private Transform _deckSnap;
    [SerializeField] private Transform[] _cardLocations;
    [SerializeField] private GameConfig _config;

    private PlayableCard _droppedCard;

    public void Initialize(PlayerCharacter character)
    {
        if (Character is not PlayerCharacter.None) throw new Exception("ya inicializado!!");
        Character = character;
    }

    public void DrawCard(ACard card, Action callback)
    {
        if (Cards.Count == 5) throw new Exception("mano llena de cartas no se puede robar!");

        var newCardGO = Instantiate(_config.CardPrefab, _deckSnap.position, _deckSnap.rotation, _hand);
        var newPlayableCard = newCardGO.GetComponent<PlayableCard>();

        // if (!IsLocalPlayer) card = null;
        newPlayableCard.Initialize(card, Character);

        Cards.Add(newPlayableCard);
        newPlayableCard.OnCardPlayed += OnCardPlayed;
        newPlayableCard.OnItemDrag += OnCardDragged;
        newPlayableCard.OnItemDrop += OnCardDropped;
        // newPlayableCard.IndexInHand = Cards.Count - 1;

        var location = _cardLocations[Cards.Count - 1];
        newPlayableCard.DrawTravel(location, callback);
    }

    public void PlayCardOnSlot(ACard card, SlotReceiver slot, Action callback)
    {

        PlayableCard playableCard = null;
        if (IsLocalPlayer)
        {
            playableCard = _droppedCard;
            if (card != playableCard.Card) throw new Exception("carta diferente en el view!!");
        }
        else
        {
            playableCard = Cards[0];
            playableCard.SetCard(card);
        }
        playableCard.Play(slot, callback);
    }

    public void DiscardCard(Action callback)
    {
        PlayableCard playableCard = IsLocalPlayer ? _droppedCard : Cards[0];
        playableCard.Play(DiscardPile, () =>
        {
            StartCoroutine(DestroyCard(playableCard.gameObject, callback));
        });
    }

    private void OnCardPlayed(PlayableCard card)
    {
        card.OnCardPlayed -= OnCardPlayed;
        Cards.Remove(card);
        StartCoroutine(ReposCardsInHand());
    }

    private void OnCardDragged(APlayableItem item)
    {
        if (!IsLocalPlayer) return;
        var card = item as PlayableCard;
        int newIndex = Cards.Count - 1;
        Cards.Remove(card);
        Cards.Add(card); //metemos la carta al final
        card.InHandPosition = _cardLocations[newIndex].position;
        card.InHandRotation = _cardLocations[newIndex].rotation;
        StartCoroutine(ReposCardsInHand(new[] { card })); //repos a todas las cartas menos a ella
    }

    private void OnCardDropped(APlayableItem item)
    {
        if (!IsLocalPlayer) return;
        var card = item as PlayableCard;
        _droppedCard = card;
    }

    private IEnumerator ReposCardsInHand(PlayableCard[] exclude = null)
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            bool cardReposed = false;
            var card = Cards[i];
            var newLocation = _cardLocations[i];
            
            if (card.transform.position == newLocation.position) continue;
            if (exclude is not null && exclude.Contains(card)) continue;

            card.ReposInHand(newLocation, () => cardReposed = true);

            yield return new WaitUntil(() => cardReposed);
        }
    }
    
    private IEnumerator DestroyCard(GameObject card, Action callback = null)
    {
        yield return null;
        Destroy(card);
        callback?.Invoke();
    }
}