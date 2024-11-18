using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameModel : IModel
{
    public PlayerCharacter PlayerOnTurn { get; private set; }
    public bool IsOnEcosystemTurn { get; private set; }

    public GameConfig Config { get; private set; }
    public Ecosystem Ecosystem { get; private set; } = new();
    
    public int NumberOfConstructions { get; private set; }

    public event Action<TableCard, TableCard> OnPopulationGrow;
    public event Action<TableCard> OnPopulationDie;
    public event Action OnCardPlaced, OnCardRemoved;
    

    private readonly Dictionary<PlayerCharacter, Player> _players = new();

    private Slot _lastDeathSlot;
    

    public GameModel(GameConfig config, Deck[] _decks)
    {
        Config = config;
        PlayerOnTurn = Config.TurnOrder[0];
        foreach (PlayerCharacter character in Enum.GetValues(typeof(PlayerCharacter)))
        {
            if (character is PlayerCharacter.None) continue;
            _players.Add(character, new Player(character, _decks.Single(deck => deck.Owner == character)));
        }
    }

    public Player GetPlayer(PlayerCharacter character) => _players[character];


    public void RemoveCardFromHand(PlayerCharacter player, ACard card)
    {
        var playerHand = _players[player].HandOfCards;

        if (!playerHand.RemoveCard(card))
            throw new Exception("error!! intentado quitar carta que no existe en la mano!");
    }

    public void PlaceCardOnSlot(ACard card, PlayerCharacter slotOwner, int slotIndex, bool atTheBottom = false)
    {
        var ownerPlayer = _players[slotOwner];
        PlaceCardOnSlot(card, ownerPlayer.Territory.Slots[slotIndex], atTheBottom);
    }

    public void PlaceCardOnSlot(ACard card, Slot slot, bool atTheBottom = false)
    {
        var tableCard = slot.PlaceCard(card, atTheBottom);
        if (card is PopulationCard) Ecosystem.OnPopulationCardPlace(tableCard);
        else if(card is MushroomCard) Ecosystem.OnMushroomCardPlace(tableCard);
        else if(card is MacrofungiCard) Ecosystem.OnMacrofungiCardPlace(tableCard);
        
        OnCardPlaced?.Invoke();
    }

    public void GrowLastPlacedPopulation(Population population, out TableCard parent, out TableCard child)
    {
        parent = population switch
        {
            Population.Plant => Ecosystem.LastPlant,
            Population.Herbivore => Ecosystem.LastHerbivore,
            Population.Carnivore => Ecosystem.LastCarnivore,
            _ => throw new ArgumentOutOfRangeException()
        };

        GrowPopulation(parent, out child);
    }

    public void GrowPopulation(TableCard parent, out TableCard child)
    {
        child = parent.Slot.PlaceCard(parent.Card);
        Ecosystem.OnPopulationCardPlace(child);
        OnPopulationGrow?.Invoke(parent, child);
        OnCardPlaced?.Invoke();
    }

    public TableCard KillLastPlacedPopulation(Population population)
    {
        TableCard killCard = population switch
        {
            Population.Plant => Ecosystem.LastPlant,
            Population.Herbivore => Ecosystem.LastHerbivore,
            Population.Carnivore => Ecosystem.LastCarnivore,
            _ => throw new ArgumentOutOfRangeException()
        };

        RemoveCardFromSlot(killCard);
        return killCard;
    }


    public TableCard GrowMushroomOverLastDeadPopulation()
    {
        var slot = _lastDeathSlot;
        if (slot is null) throw new Exception("Error! no hay slot registrado donde poner la seta!");
        _lastDeathSlot = null;

        return GrowMushroom(slot.Territory.Owner, slot.SlotIndexInTerritory);
    }

    public TableCard GrowMushroom(PlayerCharacter slotOwner, int slotIndex)
    {
        var ownerPlayer = _players[slotOwner];
        var slot = ownerPlayer.Territory.Slots[slotIndex];
        return GrowMushroom(slot);
    }

    public TableCard GrowMushroom(Slot slot)
    {
        var mushroom = slot.PlaceCard(Config.Mushroom, true);
        OnCardPlaced?.Invoke();
        return mushroom;
    }



    public void PlaceInlfuenceCardOnCard(ACard influenceCard, PlayerCharacter slotOwner,
        int slotIndex, int cardIndex)
    {
        var ownerPlayer = _players[slotOwner];
        var slot = ownerPlayer.Territory.Slots[slotIndex];
        var tableCard = slot.PlacedCards[cardIndex];
        var card = tableCard.Card;
        if (!card.CanHaveInfluenceCardOnTop || tableCard.InfluenceCardOnTop is not null)
            throw new Exception("Error! Peticion incorrecta al modelo.");

        PlaceInlfuenceCardOnCard(influenceCard, tableCard);
    }

    public void PlaceInlfuenceCardOnCard(ACard influenceCard, TableCard tableCard)
    {
        tableCard.PlaceInfluenceCard(influenceCard);
    }

    public void MoveCardBetweenSlots(PlayerCharacter slotOwner, int slotIndex, int cardIndex,
        PlayerCharacter targetSlotOwner,
        int targetSlotIndex)
    {
        var ownerPlayer = _players[slotOwner];
        var slot = ownerPlayer.Territory.Slots[slotIndex];
        var tableCard = slot.PlacedCards[cardIndex];
        var targetPlayer = _players[targetSlotOwner];
        var targetSlot = targetPlayer.Territory.Slots[targetSlotIndex];
        MoveCardBetweenSlots(tableCard, targetSlot);
    }

    public void MoveCardBetweenSlots(TableCard card, Slot target)
    {
        var slot = card.Slot;
        slot.RemoveCard(card);
        
        target.MoveCard(card);

        if(card.Card is PopulationCard)
            Ecosystem.OnPopulationMoved(card, slot);
    }

    public void RemoveCardFromSlot(PlayerCharacter slotOwner, int slotIndex, int cardIndex)
    {
        var ownerPlayer = _players[slotOwner];
        var slot = ownerPlayer.Territory.Slots[slotIndex];
        var tableCard = slot.PlacedCards[cardIndex];
        
        RemoveCardFromSlot(tableCard);
    }

    public void RemoveCardFromSlot(TableCard tableCard)
    {
        var slot = tableCard.Slot;
        slot.RemoveCard(tableCard);
        if (tableCard.Card is PopulationCard)
        {
            _lastDeathSlot = slot;
            Ecosystem.OnPopulationCardDie(tableCard);
            OnPopulationDie?.Invoke(tableCard);
        }
        else if(tableCard.Card is MushroomCard) Ecosystem.OnMushroomCardDie(tableCard);
        else if(tableCard.Card is MacrofungiCard) Ecosystem.OnMacrofungiCardDie(tableCard);
        
        OnCardRemoved?.Invoke();
    }

    public void RemoveInfluenceCardFromCard(ACard card, PlayerCharacter slotOwner, int slotIndex, int cardIndex)
    {
        var ownerPlayer = _players[slotOwner];
        var slot = ownerPlayer.Territory.Slots[slotIndex];
        var tableCard = slot.PlacedCards[cardIndex];

        if (tableCard.Card != card || tableCard.InfluenceCardOnTop is null)
            throw new Exception("Error! Peticion incorrecta al modelo.");
        
        RemoveInfluenceCardFromCard(tableCard);
    }

    public void RemoveInfluenceCardFromCard(TableCard cardWherePlaced)
    {
        cardWherePlaced.RemoveInfluenceCard();
    }

    //el filtro debe dar true para las cartas que se quieran salvar de eliminar
    public void RemoveCardsFromSlot(PlayerCharacter slotOwner, int slotIndex, Predicate<TableCard> filter = null)
    {
        var ownerPlayer = _players[slotOwner];
        var slot = ownerPlayer.Territory.Slots[slotIndex];
        RemoveCardsFromSlot(slot, filter);
    }

    public void RemoveCardsFromSlot(Slot slot, Predicate<TableCard> filter = null)
    {
        Stack<TableCard> toBeRemoved = new();
        foreach (var tableCard in slot.PlacedCards)
        {
            if (filter is not null && filter(tableCard)) continue;
            toBeRemoved.Push(tableCard);
        }

        while (toBeRemoved.Any())
        {
            var tableCard = toBeRemoved.Pop();
            RemoveCardFromSlot(tableCard);
        }
    }


    //el filtro debe dar true para las cartas que se quieran salvar de eliminar
    public void RemoveCardsFromTerritory(PlayerCharacter owner, Predicate<TableCard> filter = null)
    {
        foreach(var slot in _players[owner].Territory.Slots) RemoveCardsFromSlot(slot, filter);
    }

    public void PlaceConstruction(PlayerCharacter territoryOwner, out TableCard plant1, out TableCard plant2)
    {
        var ownerPlayer = _players[territoryOwner];

        if (ownerPlayer.Territory.HasConstruction)
            throw new Exception("Error! Construyendo en territorio ya construido.");
        
        
        
        var plants = new List<TableCard>();

        foreach (var slot in ownerPlayer.Territory.Slots)
        {
            foreach (var tableCard in slot.PlacedCards)
            {
                if (tableCard.Card is not PopulationCard ||
                    !tableCard.GetPopulations().Contains(Population.Plant)) continue;

                plants.Add(tableCard);
            }
        }

        if (plants.Count < 2) throw new Exception("Error! menos de 2 plantas para construir!");

        if (plants.Count > 2)
        {
            //esto hace que plants se ordene de mas vieja a mas nueva jugada
            var ecosystemPlants = new List<TableCard>(Ecosystem.Plants);
            plants.Sort((x, y) => plants.FindIndex(card => card == x).CompareTo(
                ecosystemPlants.FindIndex(card => card == y)));
        }

        //quitamos las 2 ultimas plantas
        plant1 = plants[^1];
        plant2 = plants[^2];
        RemoveCardFromSlot(plant1);
        RemoveCardFromSlot(plant2);

        
        ownerPlayer.Territory.HasConstruction = true;
        NumberOfConstructions++;
        
        
        //actualizamos el ecosistema
        Ecosystem.OnConstructionPlaced(ownerPlayer.Territory);
    }

    public void RemoveConstruction(PlayerCharacter territoryOwner)
    {
        var ownerPlayer = _players[territoryOwner];

        if (!ownerPlayer.Territory.HasConstruction)
            throw new Exception("Error! Peticion incorrecta al modelo.");

        ownerPlayer.Territory.HasConstruction = false;
        NumberOfConstructions--;
        
        Ecosystem.OnConstructionRemoved(ownerPlayer.Territory);
    }

    public void GiveRabies(PlayerCharacter slotOwner, int slotIndex, int cardIndex)
    {
        var ownerPlayer = _players[slotOwner];
        var card = ownerPlayer.Territory.Slots[slotIndex].PlacedCards[cardIndex];
        card.HasRabids = true;
    }

    public void MakeOmnivore(PlayerCharacter slotOwner, int slotIndex, int cardIndex)
    {
        var ownerPlayer = _players[slotOwner];
        var card = ownerPlayer.Territory.Slots[slotIndex].PlacedCards[cardIndex];

        card.IsOmnivore = true;
    }

    public TableCard GetLastMushroomInTerritory(PlayerCharacter owner)
    {
        var mushrooms = new List<TableCard>();
        var ownerPlayer = _players[owner];
        
        foreach (var slot in ownerPlayer.Territory.Slots)
        {
            foreach (var tableCard in slot.PlacedCards)
            {
                if (tableCard.Card is not MushroomCard) continue;

                mushrooms.Add(tableCard);
            }
        }

        if (mushrooms.Count == 0) return null;
        else if (mushrooms.Count == 1) return mushrooms[0];

        //esto hace que las setas se ordenen de mas vieja a mas nueva jugada
        var ecosystemMushrooms = new List<TableCard>(Ecosystem.Mushrooms);
        mushrooms.Sort((x, y) => mushrooms.FindIndex(card => card == x).CompareTo(
            ecosystemMushrooms.FindIndex(card => card == y)));

        return mushrooms[^1]; //ultimo elemento
    }


    public void AdvanceTurn(PlayerCharacter playerOnTurn)
    {
        PlayerOnTurn = playerOnTurn;
        IsOnEcosystemTurn = PlayerOnTurn is PlayerCharacter.None;

        foreach (var player in _players.Values)
        {
            player.AdvanceTurn();
        }
    }

    public IReadOnlyList<ACard> PlayerDrawCards(PlayerCharacter character /*, int amount*/)
    {
        List<ACard> drawnCards = new();
        var player = _players[character];

        int amount = 5 - player.HandOfCards.Count;

        for (int i = 0; i < amount; i++)
        {
            var card = player.DrawCard();
            drawnCards.Add(card);
        }

        return drawnCards;
    }
}