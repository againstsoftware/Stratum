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
        Effect.GrowCarnivoreEcosystem => _growCarnivore,
        Effect.GrowHerbivoreEcosystem => _growHerbivore,
        Effect.KillCarnivoreEcosystem => _killCarnivore,
        Effect.KillHerbivoreEcosystem => _killHerbivore,
        Effect.Discard => _discard,
        Effect.Draw2 => _draw,
        Effect.Draw5 => _draw,
        Effect.OverviewSwitch => _overviewSwitch,
        Effect.GrowMushroomEcosystem => _growMushroomEcosystem,
        Effect.GrowMushroom => _growMushroom,
        Effect.GrowMushroomEndOfAction => _growMushroomEOA,
        Effect.GrowMacrofungi => _growMacrofungi,
        Effect.Construct => _construct,
        Effect.PlaceInitialCards => _placeInitialCards,
        Effect.PlayAndDiscardInfluenceCard => _playAndDiscardInfluenceCard,
        Effect.MovePopulationToEmptySlot => _movePopulationToEmptySlot,
        Effect.PlaceInfluenceOnPopulation => _placeInfluenceOnPopulation,
        Effect.GiveRabies => _giveRabies,
        Effect.DestroyAllInTerritory => _destroyAllInTerritory,
        Effect.DestroyNonFungiInTerritory => _destroyNonFungiInTerritory,
        Effect.MakeOmnivore => _makeOmnivore,
        Effect.GrowPlant => _growPlant,
        Effect.GrowPlantEndOfAction => _growPlantEOA,
        Effect.KillPlantEndOfAction => _killPlantEOA,

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
        var location = new IView.CardLocation() { Owner = owner, SlotIndex = slotIndex };
        ServiceLocator.Get<IView>().PlayCardOnSlotFromPlayer(card as ACard, owner, location, callback);
    };

    private static readonly EffectCommand _growCarnivore = (_, callback) =>
    {
        var (parent, child) = ServiceLocator.Get<IModel>().GrowLastPlacedPopulation(ICard.Population.Carnivore);

        var location = new IView.CardLocation()
            { Owner = parent.Slot.Territory.Owner, SlotIndex = parent.Slot.SlotIndexInTerritory };

        ServiceLocator.Get<IView>().GrowPopulationCardEcosystem(location, callback);
    };

    private static readonly EffectCommand _growHerbivore = (_, callback) =>
    {
        var (parent, child) = ServiceLocator.Get<IModel>().GrowLastPlacedPopulation(ICard.Population.Herbivore);
        var location = new IView.CardLocation()
            { Owner = parent.Slot.Territory.Owner, SlotIndex = parent.Slot.SlotIndexInTerritory };
        ServiceLocator.Get<IView>().GrowPopulationCardEcosystem(location, callback);
    };

    private static readonly EffectCommand _killCarnivore = (_, callback) =>
    {
        var killed = ServiceLocator.Get<IModel>().KillLastPlacedPopulation(ICard.Population.Carnivore);
        var location = new IView.CardLocation()
            { Owner = killed.Slot.Territory.Owner, SlotIndex = killed.Slot.SlotIndexInTerritory };
        ServiceLocator.Get<IView>().KillPopulationCardEcosystem(location, callback);
    };

    private static readonly EffectCommand _killHerbivore = (_, callback) =>
    {
        var killed = ServiceLocator.Get<IModel>().KillLastPlacedPopulation(ICard.Population.Herbivore);

        var location = new IView.CardLocation()
            { Owner = killed.Slot.Territory.Owner, SlotIndex = killed.Slot.SlotIndexInTerritory };
        ServiceLocator.Get<IView>().KillPopulationCardEcosystem(location, callback);
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
        var mushroom = ServiceLocator.Get<IModel>().GrowMushroom();
        var slotOwner = mushroom.Slot.Territory.Owner;
        var slotIndex = mushroom.Slot.SlotIndexInTerritory;
        ServiceLocator.Get<IView>()
            .GrowMushroom(PlayerCharacter.None, new IView.CardLocation() { Owner = slotOwner, SlotIndex = slotIndex }, callback);
    };
    
    private static readonly EffectCommand _growMushroom = (action, callback) =>
    {
        var slotOwner = action.Receivers[0].LocationOwner;
        var slotIndex = action.Receivers[0].Index;
        var mushroom = ServiceLocator.Get<IModel>().GrowMushroom(slotOwner, slotIndex);
        ServiceLocator.Get<IView>()
            .GrowMushroom(action.Actor,new IView.CardLocation() { Owner = slotOwner, SlotIndex = slotIndex }, callback, false);
    };
    
    private static readonly EffectCommand _growMushroomEOA = (action, callback) =>
    {
        var slotOwner = action.Receivers[0].LocationOwner;
        var slotIndex = action.Receivers[0].Index;
        var mushroom = ServiceLocator.Get<IModel>().GrowMushroom(slotOwner, slotIndex);
        ServiceLocator.Get<IView>()
            .GrowMushroom(action.Actor,new IView.CardLocation() { Owner = slotOwner, SlotIndex = slotIndex }, callback, true);
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
            locations.Add(new IView.CardLocation() { Owner = slotOwner, SlotIndex = slotIndex });
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
            Owner = plant1.Slot.Territory.Owner
        };

        var location2 = new IView.CardLocation()
        {
            SlotIndex = plant2.Slot.SlotIndexInTerritory, CardIndex = plant2.IndexInSlot,
            Owner = plant2.Slot.Territory.Owner
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
            var location = new IView.CardLocation() { Owner = slotOwner, SlotIndex = slotIndex };

            cardsAndLocations.Add((initialCard, location));
        }

        ServiceLocator.Get<IView>().PlaceInitialCards(cardsAndLocations, callback);
    };

    private static readonly EffectCommand _playAndDiscardInfluenceCard = (action, callback) =>
    {
        ServiceLocator.Get<IModel>().RemoveCardFromHand(action.Actor, action.ActionItem as ICard);

        bool isTerritory = action.Receivers[0].Location is ValidDropLocation.AnyTerritory;
        var location = new IView.CardLocation()
        {
            IsTerritory = isTerritory,
            Owner = action.Receivers[0].LocationOwner,
            SlotIndex = isTerritory ? -1 : action.Receivers[0].Index,
            CardIndex = isTerritory ? -1 : action.Receivers[0].SecondIndex,
        };
        var influence = action.ActionItem as InfluenceCard;
        ServiceLocator.Get<IView>().PlayAndDiscardInfluenceCard(action.Actor, influence, location, callback);
    };

    private static readonly EffectCommand _movePopulationToEmptySlot = (action, callback) =>
    {
        var slotOwner = action.Receivers[0].LocationOwner;
        var slotIndex = action.Receivers[0].Index;
        var cardIndex = action.Receivers[0].SecondIndex;
        var targetSlotOwner = action.Receivers[1].LocationOwner;
        var targetSlotIndex = action.Receivers[1].Index;
        ServiceLocator.Get<IModel>()
            .MoveCardBetweenSlots(slotOwner, slotIndex, cardIndex, targetSlotOwner, targetSlotIndex);

        var from = new IView.CardLocation()
        {
            Owner = slotOwner,
            SlotIndex = slotIndex,
            CardIndex = cardIndex
        };

        var to = new IView.CardLocation()
        {
            Owner = targetSlotOwner,
            SlotIndex = targetSlotIndex
        };

        ServiceLocator.Get<IView>().MovePopulationToEmptySlot(action.Actor, from, to, callback);
    };

    private static readonly EffectCommand _placeInfluenceOnPopulation = (action, callback) =>
    {
        var slotOwner = action.Receivers[0].LocationOwner;
        var slotIndex = action.Receivers[0].Index;
        var cardIndex = action.Receivers[0].SecondIndex;
        var influenceCard = action.ActionItem as InfluenceCard;
        ServiceLocator.Get<IModel>().RemoveCardFromHand(action.Actor, action.ActionItem as ICard);
        ServiceLocator.Get<IModel>().PlaceInlfuenceCardOnCard(influenceCard, slotOwner, slotIndex, cardIndex);

        var location = new IView.CardLocation()
        {
            Owner = slotOwner,
            SlotIndex = slotIndex,
            CardIndex = cardIndex
        };

        ServiceLocator.Get<IView>().PlaceInfluenceOnPopulation(action.Actor, influenceCard, location, callback);
    };

    private static readonly EffectCommand _giveRabies = (action, callback) =>
    {
        var slotOwner = action.Receivers[0].LocationOwner;
        var slotIndex = action.Receivers[0].Index;
        var cardIndex = action.Receivers[0].SecondIndex;

        ServiceLocator.Get<IModel>().GiveRabies(slotOwner, slotIndex, cardIndex);

        var location = new IView.CardLocation()
        {
            Owner = slotOwner,
            SlotIndex = slotIndex,
            CardIndex = cardIndex
        };

        ServiceLocator.Get<IView>().GiveRabies(action.Actor, location, callback);
    };

    private static readonly EffectCommand _destroyAllInTerritory = (action, callback) =>
    {
        DestroyInTerritory(action, callback, null);
    };

    private static readonly EffectCommand _destroyNonFungiInTerritory = (action, callbaack) =>
    {
        DestroyInTerritory(action, callbaack, card => card is MushroomCard or MacrofungiCard);
    };

    private static void DestroyInTerritory(PlayerAction action, Action callback, Predicate<ICard> filter)
    {
        var owner = action.Receivers[0].LocationOwner;
        Predicate<TableCard> modelFilter = filter is null ? null : tCard => filter(tCard.Card);
        ServiceLocator.Get<IModel>().RemoveCardsFromTerritory(owner, modelFilter);

        if (ServiceLocator.Get<IModel>().GetPlayer(owner).Territory.HasConstruction)
            ServiceLocator.Get<IModel>().RemoveConstruction(owner);
        
        ServiceLocator.Get<IView>().DestroyInTerritory(action.Actor, owner, callback, filter);
    }

    private static readonly EffectCommand _makeOmnivore = (action, callback) =>
    {
        var slotOwner = action.Receivers[0].LocationOwner;
        var slotIndex = action.Receivers[0].Index;
        var cardIndex = action.Receivers[0].SecondIndex;

        ServiceLocator.Get<IModel>().MakeOmnivore(slotOwner, slotIndex, cardIndex);

        var location = new IView.CardLocation()
        {
            Owner = slotOwner,
            SlotIndex = slotIndex,
            CardIndex = cardIndex
        };

        ServiceLocator.Get<IView>().MakeOmnivore(action.Actor, location, callback);
    };

    private static readonly EffectCommand _growPlant = (action, callback) =>
    {
        var card = action.ActionItem as ICard;
        var slotOwner = action.Receivers[0].LocationOwner;
        var slotIndex = action.Receivers[0].Index;
        ServiceLocator.Get<IModel>().PlaceCardOnSlot(card, slotOwner, slotIndex);
        
        var location = new IView.CardLocation()
        {
            Owner = slotOwner,
            SlotIndex = slotIndex,
        };
        
        ServiceLocator.Get<IView>().GrowPopulation(action.Actor, location, ICard.Population.Plant, callback);
    };
    
    private static readonly EffectCommand _growPlantEOA = (action, callback) =>
    {
        var card = Config.GetPopulationCard(ICard.Population.Plant);
        var slotOwner = action.Receivers[0].LocationOwner;
        var slotIndex = action.Receivers[0].Index;
        ServiceLocator.Get<IModel>().PlaceCardOnSlot(card, slotOwner, slotIndex);
        
        var location = new IView.CardLocation()
        {
            Owner = slotOwner,
            SlotIndex = slotIndex,
        };
        
        ServiceLocator.Get<IView>().GrowPopulation(action.Actor, location, ICard.Population.Plant, callback, true);
    };


    private static readonly EffectCommand _killPlantEOA = (action, callback) =>
    {
        var slotOwner = action.Receivers[0].LocationOwner;
        var slotIndex = action.Receivers[0].Index;
        var cardIndex = action.Receivers[0].SecondIndex;
        
        ServiceLocator.Get<IModel>().RemoveCardFromSlot(slotOwner, slotIndex, cardIndex);
        var location = new IView.CardLocation()
        {
            Owner = slotOwner,
            SlotIndex = slotIndex,
            CardIndex = cardIndex
        };
        ServiceLocator.Get<IView>().KillPopulation(action.Actor, location, callback, true);
    };

}
