using System;
using System.Collections.Generic;
using System.Linq;

public delegate void EffectCommand(PlayerAction playerAction, Action onCompletedCallback);

public static class EffectCommands
{
    public static EffectCommand Get(Effect effect) => effect switch
    {
        Effect.PlacePopulationCard => _placePopulationCard,
        Effect.GrowCarnivore => _growCarnivore,
        Effect.GrowHerbivore => _growHerbivore,
        Effect.KillCarnivore => _killCarnivore,
        Effect.KillHerbivore => _killHerbivore,
        Effect.Discard => _discard,
        Effect.Draw2 => _draw2,
        Effect.Draw5 => _draw5,
        Effect.OverviewSwitch => _overviewSwitch,

        _ => throw new ArgumentOutOfRangeException()
    };


    private static readonly EffectCommand _placePopulationCard = (action, callback) =>
    {
        var card = action.ActionItem as ICard;
        var owner = action.Actor;
        var slotIndex = action.Receivers[0].Index;
        ServiceLocator.Get<IModel>().RemoveCardFromHand(owner, card);
        ServiceLocator.Get<IModel>().PlaceCardOnSlot(card, owner, slotIndex);
        //update al view asincrono
        ServiceLocator.Get<IView>().PlayCardOnSlot(card as ACard, owner, owner, slotIndex, callback);
    };

    private static readonly EffectCommand _growCarnivore = (_, callback) =>
    {
        var (parent, child) = ServiceLocator.Get<IModel>().GrowPopulation(ICard.Population.Carnivore);

        ServiceLocator.Get<IView>().GrowPopulationCard(parent.Slot.Territory.Owner, parent.Slot.SlotIndex, callback);
    };

    private static readonly EffectCommand _growHerbivore = (_, callback) =>
    {
        var (parent, child) = ServiceLocator.Get<IModel>().GrowPopulation(ICard.Population.Herbivore);

        ServiceLocator.Get<IView>().GrowPopulationCard(parent.Slot.Territory.Owner, parent.Slot.SlotIndex, callback);
    };

    private static readonly EffectCommand _killCarnivore = (_, callback) =>
    {
        var killed = ServiceLocator.Get<IModel>().KillPopulation(ICard.Population.Carnivore);

        ServiceLocator.Get<IView>().KillPopulationCard(killed.Slot.Territory.Owner, killed.Slot.SlotIndex, callback);
    };

    private static readonly EffectCommand _killHerbivore = (_, callback) =>
    {
        var killed = ServiceLocator.Get<IModel>().KillPopulation(ICard.Population.Carnivore);

        ServiceLocator.Get<IView>().KillPopulationCard(killed.Slot.Territory.Owner, killed.Slot.SlotIndex, callback);
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


    private static readonly EffectCommand _draw2 = (_, callback) =>
    {
        Dictionary<PlayerCharacter, List<ACard>> cardsDrawn = new();
        foreach(var character in _turnOrder)
        {
            if (character is PlayerCharacter.None) continue;
            var cards = ServiceLocator.Get<IModel>().PlayerDrawCards(character, 2);
            cardsDrawn.Add(character, new List<ACard>(cards.Cast<ACard>()));
        }
        
        DrawCardInView(cardsDrawn, 0, callback);
    };

    private static readonly EffectCommand _draw5 = (_, callback) =>
    {
        Dictionary<PlayerCharacter, List<ACard>> cardsDrawn = new();
        foreach(var character in _turnOrder)
        {
            if (character is PlayerCharacter.None) continue;
            var cards = ServiceLocator.Get<IModel>().PlayerDrawCards(character, 5);
            cardsDrawn.Add(character, new List<ACard>(cards.Cast<ACard>()));
        }
        
        DrawCardInView(cardsDrawn, 0, callback);
    };

    private static void DrawCardInView(Dictionary<PlayerCharacter, List<ACard>> cardsDrawn, int index, Action callback)
    {
        if (index == _turnOrder.Length)
        {
            callback();
            return;
        }

        var actor = _turnOrder[index];

        if (actor is PlayerCharacter.None)
        {
            DrawCardInView(cardsDrawn, index+1, callback);
            return;
        }
        
        var cards = cardsDrawn[actor];
        ServiceLocator.Get<IView>().DrawCards(actor, cards, () => DrawCardInView(cardsDrawn, index + 1, callback));
    }

    private static readonly EffectCommand _overviewSwitch = (_, callback) =>
    {
        ServiceLocator.Get<IView>().SwitchCamToOverview(callback);
    };
}