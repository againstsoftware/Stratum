using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void EffectCommand(PlayerAction playerAction, Action onCompletedCallback);

public static class EffectCommands
{
    public static GameConfig Config { get; set; }

    public static EffectCommand Get(Effect effect) => effect switch
    {
        Effect.PlacePopulationCardFromPlayer => _placePopulationCardfromPlayer,
        Effect.GrowCarnivore => _growCarnivore,
        Effect.GrowHerbivore => _growHerbivore,
        Effect.KillCarnivore => _killCarnivore,
        Effect.KillHerbivore => _killHerbivore,
        Effect.Discard => _discard,
        Effect.Draw2 => _draw,
        Effect.Draw5 => _draw,
        Effect.OverviewSwitch => _overviewSwitch,
        Effect.GrowMushroomEcosystem => _growMushroomEcosystem,
        Effect.GrowMacrofungi => _growMacrofungi,
        Effect.Construct => _construct,
        Effect.PlaceInitialCards => _placeInitialCards,
        Effect.MovePopulationToEmptySlot => _movePopulationToEmptySlot,

        _ => throw new ArgumentOutOfRangeException()
    };


    private static readonly EffectCommand _placePopulationCardfromPlayer = (action, callback) =>
    {
        var card = action.ActionItem as ICard;
        var owner = action.Actor;
        var slotIndex = action.Receivers[0].Index;
        ServiceLocator.Get<IModel>().RemoveCardFromHand(owner, card);
        ServiceLocator.Get<IModel>().PlaceCardOnSlot(card, owner, slotIndex);
        //update al view asincrono
        var location = new IView.CardLocation() { SlotOwner = owner, SlotIndex = slotIndex };
        ServiceLocator.Get<IView>().PlayCardOnSlotFromPlayer(card as ACard, owner, location, callback);
    };

    private static readonly EffectCommand _growCarnivore = (_, callback) =>
    {
        var (parent, child) = ServiceLocator.Get<IModel>().GrowLastPlacedPopulation(ICard.Population.Carnivore);

        var location = new IView.CardLocation()
            { SlotOwner = parent.Slot.Territory.Owner, SlotIndex = parent.Slot.SlotIndexInTerritory };

        ServiceLocator.Get<IView>().GrowPopulationCard(location, callback);
    };

    private static readonly EffectCommand _growHerbivore = (_, callback) =>
    {
        var (parent, child) = ServiceLocator.Get<IModel>().GrowLastPlacedPopulation(ICard.Population.Herbivore);
        var location = new IView.CardLocation()
            { SlotOwner = parent.Slot.Territory.Owner, SlotIndex = parent.Slot.SlotIndexInTerritory };
        ServiceLocator.Get<IView>().GrowPopulationCard(location, callback);
    };

    private static readonly EffectCommand _killCarnivore = (_, callback) =>
    {
        var killed = ServiceLocator.Get<IModel>().KillLastPlacedPopulation(ICard.Population.Carnivore);
        var location = new IView.CardLocation()
            { SlotOwner = killed.Slot.Territory.Owner, SlotIndex = killed.Slot.SlotIndexInTerritory };
        ServiceLocator.Get<IView>().KillPopulationCard(location, callback);
    };

    private static readonly EffectCommand _killHerbivore = (_, callback) =>
    {
        var killed = ServiceLocator.Get<IModel>().KillLastPlacedPopulation(ICard.Population.Herbivore);

        var location = new IView.CardLocation()
            { SlotOwner = killed.Slot.Territory.Owner, SlotIndex = killed.Slot.SlotIndexInTerritory };
        ServiceLocator.Get<IView>().KillPopulationCard(location, callback);
    };

    private static readonly EffectCommand _discard = (action, callback) =>
    {
        ServiceLocator.Get<IModel>().RemoveCardFromHand(action.Actor, action.ActionItem as ICard);

        ServiceLocator.Get<IView>().Discard(action.Actor, callback);
    };

    private static readonly PlayerCharacter[] _turnOrder = new[] //seria lo suyo usar GameConfig pero zzzz
    {
        PlayerCharacter.Sagitario, PlayerCharacter.Fungaloth, PlayerCharacter.Ygdra,
        PlayerCharacter.Overlord, PlayerCharacter.None
    };


    private static readonly EffectCommand _draw = (_, callback) =>
    {
        Dictionary<PlayerCharacter, IReadOnlyList<ACard>> cardsDrawn = new();
        foreach (var character in _turnOrder)
        {
            if (character is PlayerCharacter.None) continue;
            var cards = ServiceLocator.Get<IModel>().PlayerDrawCards(character /*, 2*/);
            cardsDrawn.Add(character, new List<ACard>(cards.Cast<ACard>()));
        }

        ServiceLocator.Get<IView>().DrawCards(cardsDrawn, callback);
    };


    private static readonly EffectCommand _overviewSwitch = (_, callback) =>
    {
        ServiceLocator.Get<IView>().SwitchCamToOverview(callback);
    };

    private static readonly EffectCommand _growMushroomEcosystem = (_, callback) =>
    {
        var mushroom = ServiceLocator.Get<IModel>().GrowMushroomEcosystem();
        var slotOwner = mushroom.Slot.Territory.Owner;
        var slotIndex = mushroom.Slot.SlotIndexInTerritory;
        ServiceLocator.Get<IView>()
            .GrowMushroom(new IView.CardLocation() { SlotOwner = slotOwner, SlotIndex = slotIndex }, callback);
    };

    private static readonly EffectCommand _growMacrofungi = (action, callback) =>
    {
        var locations = new List<IView.CardLocation>();

        PlayerCharacter slotOwner = default;
        int slotIndex = default;
        foreach (var receiver in action.Receivers)
        {
            slotOwner = receiver.LocationOwner;
            slotIndex = receiver.Index;
            locations.Add(new IView.CardLocation() { SlotOwner = slotOwner, SlotIndex = slotIndex });
            var cardIndex = receiver.SecondIndex;
            ServiceLocator.Get<IModel>().RemoveCardFromSlot(slotOwner, slotIndex, cardIndex);
        }

        var macrofungiCard = ServiceLocator.Get<IModel>().GetPlayer(PlayerCharacter.Sagitario).Deck.Macrofungi;
        ServiceLocator.Get<IModel>().PlaceCardOnSlot(macrofungiCard, slotOwner, slotIndex, true);

        ServiceLocator.Get<IView>().GrowMacrofungi(locations.ToArray(), callback);
    };

    private static readonly EffectCommand _construct = (action, callback) =>
    {
        var owner = action.Receivers[0].LocationOwner;
        ServiceLocator.Get<IModel>().PlaceConstruction(owner, out var plant1, out var plant2);

        var location1 = new IView.CardLocation()
        {
            SlotIndex = plant1.Slot.SlotIndexInTerritory, CardIndex = plant1.IndexInSlot,
            SlotOwner = plant1.Slot.Territory.Owner
        };

        var location2 = new IView.CardLocation()
        {
            SlotIndex = plant2.Slot.SlotIndexInTerritory, CardIndex = plant2.IndexInSlot,
            SlotOwner = plant2.Slot.Territory.Owner
        };

        Debug.Log("construccion plantas tal:");
        Debug.Log($"1: si: {location1.SlotIndex}, ci: {location1.CardIndex}");
        Debug.Log($"2: si: {location2.SlotIndex}, ci: {location2.CardIndex}");


        ServiceLocator.Get<IView>().PlaceConstruction(location1, location2, callback);
    };

    private static readonly EffectCommand _placeInitialCards = (_, callback) =>
    {
        var initialCards = new Stack<PopulationCard>(Config.InitialCards);
        var players = Config.TurnOrder.ToList();
        players.Remove(PlayerCharacter.None);
        
        var cardsAndLocations = new List<(ACard card, IView.CardLocation location)>();

        int count = Mathf.Min(initialCards.Count, players.Count);
        while (count > 0)
        {
            count--;
            var index = UnityEngine.Random.Range(0, players.Count);
            var slotOwner = players[index];
            players.RemoveAt(index);

            var initialCard = initialCards.Pop();
            int slotIndex = 0;
            ServiceLocator.Get<IModel>().PlaceCardOnSlot(initialCard, slotOwner, slotIndex);
            //update al view asincrono
            var location = new IView.CardLocation() { SlotOwner = slotOwner, SlotIndex = slotIndex };
            
            cardsAndLocations.Add((initialCard, location));
        }
        
        ServiceLocator.Get<IView>().PlaceInitialCards(cardsAndLocations, callback);
    };

    private static readonly EffectCommand _movePopulationToEmptySlot = (action, callback) =>
    {
        var card = action.ActionItem as ICard;
        var slotOwner = action.Actor;
        var slotIndex = action.Receivers[0].Index;
        var cardIndex = action.Receivers[0].SecondIndex;
        var targetSlotOwner = action.Receivers[1].LocationOwner;
        var targetSlotIndex = action.Receivers[1].Index;
        ServiceLocator.Get<IModel>()
            .MoveCardBetweenSlots(card, slotOwner, slotIndex, cardIndex, targetSlotOwner, targetSlotIndex);
        
        
    };
}