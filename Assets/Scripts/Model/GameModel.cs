using System;
using System.Collections.Generic;
using System.Linq;

public class GameModel : IModel
{
    public PlayerCharacter PlayerOnTurn { get; private set; }
    public bool IsOnEcosystemTurn { get; private set; }

    public Ecosystem Ecosystem { get; private set; } = new();

    private readonly Dictionary<PlayerCharacter, Player> _players = new();


    public GameModel(PlayerCharacter starter, IDeck[] _decks)
    {
        PlayerOnTurn = starter;
        
        foreach (PlayerCharacter character in Enum.GetValues(typeof(PlayerCharacter)))
        {
            if (character is PlayerCharacter.None) continue;
            _players.Add(character, new Player(character, _decks.Single(deck => deck.Owner == character)));
        }
    }
    
    public Player GetPlayer(PlayerCharacter character) => _players[character];
    
    public void PlaceCardOnSlot(ICard card, PlayerCharacter slotOwner, int slotIndex, bool atTheBottom = false)
    {
        var ownerPlayer = _players[slotOwner];
        var tableCard = ownerPlayer.Territory.Slots[slotIndex].PlaceCard(card, atTheBottom);
        if (card.CardType is ICard.Card.Population) Ecosystem.OnPopulationCardPlace(tableCard);
    }

    public void PlaceInlfuenceCardOnCard(ICard influenceCard, ICard card, PlayerCharacter slotOwner,
        int slotIndex, int cardIndex)
    {
        var ownerPlayer = _players[slotOwner];
        var slot = ownerPlayer.Territory.Slots[slotIndex];
        var tableCard = slot.PlacedCards[cardIndex];

        if (!card.CanHaveInfluenceCardOnTop || tableCard.Card != card)
            throw new Exception("Error! Peticion incorrecta al modelo.");

        tableCard.PlaceInlfuenceCard(influenceCard);
    }

    public void MoveCardBetweenSlots(ICard card, PlayerCharacter slotOwner, int slotIndex, int cardIndex,
        PlayerCharacter targetSlotOwner,
        int targetSlotIndex)
    {
        var ownerPlayer = _players[slotOwner];
        var slot = ownerPlayer.Territory.Slots[slotIndex];
        var tableCard = slot.PlacedCards[cardIndex];

        if (tableCard.Card != card) throw new Exception("Error! Peticion incorrecta al modelo.");

        slot.RemoveCard(tableCard);

        var targetPlayer = _players[targetSlotOwner];
        var targetSlot = targetPlayer.Territory.Slots[targetSlotIndex];
        targetSlot.MoveCard(tableCard);
    }

    public void RemoveCardFromSlot(ICard card, PlayerCharacter slotOwner, int slotIndex, int cardIndex)
    {
        var ownerPlayer = _players[slotOwner];
        var slot = ownerPlayer.Territory.Slots[slotIndex];
        var tableCard = slot.PlacedCards[cardIndex];

        if (tableCard.Card != card) throw new Exception("Error! Peticion incorrecta al modelo.");

        slot.RemoveCard(tableCard);

        if (card.CardType is ICard.Card.Population) Ecosystem.OnPopulationCardDie(tableCard);
    }

    public void RemoveInfluenceCardFromCard(ICard influenceCard, ICard card, PlayerCharacter slotOwner,
        int slotIndex, int cardIndex)
    {
        var ownerPlayer = _players[slotOwner];
        var slot = ownerPlayer.Territory.Slots[slotIndex];
        var tableCard = slot.PlacedCards[cardIndex];

        if (tableCard.Card != card || tableCard.InfluenceCardOnTop is null ||
            tableCard.InfluenceCardOnTop.Card != influenceCard)
            throw new Exception("Error! Peticion incorrecta al modelo.");

        tableCard.RemoveInfluenceCard();
    }

    //el filtro debe dar true para las cartas que se quieran salvar de eliminar
    public void RemoveCardsFromSlot(PlayerCharacter slotOwner, int slotIndex, Predicate<TableCard> filter = null)
    {
        var ownerPlayer = _players[slotOwner];
        var slot = ownerPlayer.Territory.Slots[slotIndex];
        foreach (var tableCard in slot.PlacedCards)
        {
            if (filter is not null && filter(tableCard)) continue;

            if (tableCard.Card.CardType is ICard.Card.Population) Ecosystem.OnPopulationCardDie(tableCard);
            slot.RemoveCard(tableCard);
        }
    }

    //el filtro debe dar true para las cartas que se quieran salvar de eliminar
    public void RemoveCardsFromTerritory(PlayerCharacter owner, Predicate<TableCard> filter = null)
    {
        for (int i = 0; i < 5; i++) RemoveCardsFromSlot(owner, i, filter);
    }

    public void PlaceConstruction(PlayerCharacter territoryOwner)
    {
        var ownerPlayer = _players[territoryOwner];

        if (ownerPlayer.Territory.HasConstruction)
            throw new Exception("Error! Peticion incorrecta al modelo.");

        ownerPlayer.Territory.HasConstruction = true;
    }

    public void RemoveConstruction(PlayerCharacter territoryOwner)
    {
        var ownerPlayer = _players[territoryOwner];

        if (!ownerPlayer.Territory.HasConstruction)
            throw new Exception("Error! Peticion incorrecta al modelo.");

        ownerPlayer.Territory.HasConstruction = false;
    }

    public void AdvanceTurn(PlayerCharacter playerOnTurn)
    {
        PlayerOnTurn = playerOnTurn;
        foreach (var player in _players.Values)
        {
            player.AdvanceTurn();
        }
    }

    public void AdvanceTurnToEcosystem()
    {
        IsOnEcosystemTurn = true;
        foreach (var player in _players.Values) player.AdvanceTurn();
    }

    public void PlayerDrawCards(PlayerCharacter character, int amount)
    {
        var player = _players[character];
        for (int i = 0; i < amount; i++)
        {
            player.DrawCard();
        }
    }
}