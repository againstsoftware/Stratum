using System;
using System.Linq;

public delegate void EffectCommand(PlayerAction playerAction, Action onCompletedCallback);

public static class EffectCommands
{
    public static EffectCommand Get(Effect effect) => effect switch
    {
        Effect.PlacePopulationCard => _placePopulationCard,
        
        
        
        _ => throw new ArgumentOutOfRangeException()
    };

    
    private static readonly EffectCommand _placePopulationCard = (action, callback) =>
    {
        var card = action.ActionItem as ICard;
        var owner = action.Actor;
        var slotIndex = action.Receivers[0].Index;
        var cardIndex = action.CardIndexInHand;
        ServiceLocator.Get<IModel>().PlaceCardOnSlot(card, owner, slotIndex);
        //update al view asincrono
        ServiceLocator.Get<IView>().PlayCardOnSlot(owner, action.ActionItem as ACard, cardIndex, slotIndex, callback);
    };
}






