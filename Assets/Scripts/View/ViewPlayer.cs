
using System;
using UnityEngine;

public class ViewPlayer : MonoBehaviour
{
    [field:SerializeField] public PlayableCard[] HandOfCards { get; private set; }
    [field:SerializeField] public TerritoryReceiver Territory { get; private set; }
    [field:SerializeField] public DiscardPileReceiver DiscardPile { get; private set; }
    [field:SerializeField] public PlayableToken Token { get; private set; }
    [field:SerializeField] public Camera Camera { get; private set; }
    

    private void Start()
    {
        int i = 0;
        foreach (var card in HandOfCards) card.IndexInHand = i++;
    }
}
