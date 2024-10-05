using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

[System.Serializable]
public class EffectCommand
{
    public enum What
    {
        Place,
        Move,
        Discard,
        Banish,
        Destroy
    }

    public enum Who
    {
        PlayedItem,
        Receiver,
        Construction,
        NewCard
    }

    [SerializeField] private What _action;
    [SerializeField] private Who _subject;
    [SerializeField] private int _whoReceiverIndex;
    [SerializeField] private int _whereReceiverIndex;
    [SerializeField] private int _fromReceiverIndex;
    [SerializeField] private int _toReceiverIndex;
    [SerializeField] private ACard _newCard;

    
    public bool IsCombinationValid(What what, Who who)
    {
        var dict = new Dictionary<What, Who[]>()
        {
            { What.Place, new[] { Who.PlayedItem, Who.Construction, Who.NewCard } },
            { What.Move, new[] { Who.Receiver } },
            { What.Discard, new[] { Who.PlayedItem } },
            { What.Banish, new[] { Who.PlayedItem, Who.Receiver } },
            { What.Destroy, new[] { Who.Construction, Who.Receiver } },
        };
        return dict[what].Contains(who);
    }
}