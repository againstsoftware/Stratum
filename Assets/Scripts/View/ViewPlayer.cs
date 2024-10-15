
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ViewPlayer : MonoBehaviour
{
    [field:SerializeField] public TerritoryReceiver Territory { get; private set; }
    [field:SerializeField] public DiscardPileReceiver DiscardPile { get; private set; }
    [field:SerializeField] public PlayableToken Token { get; private set; }
    [field:SerializeField] public Camera Camera { get; private set; }

    public readonly List<PlayableCard> Cards = new(5);

    public PlayerCharacter Character { get; private set; } = PlayerCharacter.None;

    [SerializeField] private Transform _hand;
    [SerializeField] private Transform _deckSnap;
    [SerializeField] private Transform[] _cardLocations;
    [SerializeField] private GameConfig _config;

    private void Start()
    {
    }

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
        
        newPlayableCard.OnCardPlayed += OnCardPlayed;
        newPlayableCard.Initialize(card, Character);
        Cards.Add(newPlayableCard);
        newPlayableCard.IndexInHand = Cards.Count - 1;
        
        var location = _cardLocations[Cards.Count - 1];
        newPlayableCard.DrawTravel(location, callback);
    }

    private void OnCardPlayed(PlayableCard card)
    {
        card.OnCardPlayed -= OnCardPlayed;
        Cards.Remove(card);
        StartCoroutine(ReposCardsInHand());
    }

    private IEnumerator ReposCardsInHand()
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            bool done = false;
            var card = Cards[i];
            var newLocation = _cardLocations[i];
            card.IndexInHand = i;
            if (card.transform.position == newLocation.position) continue;
            card.ReposInHand(newLocation, () => done = true);
            while (!done) yield return null;
        }
    }
}
