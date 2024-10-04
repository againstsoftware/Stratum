
using System;
using UnityEngine;

[RequireComponent(typeof(PlayableCard))]
public class CardRulebookOpener : MonoBehaviour, IRulebookOpener
{
    public IRulebookEntry RulebookEntry => _playableCard.Card;

    private PlayableCard _playableCard;

    private void Start()
    {
        _playableCard = GetComponent<PlayableCard>();
    }
}
