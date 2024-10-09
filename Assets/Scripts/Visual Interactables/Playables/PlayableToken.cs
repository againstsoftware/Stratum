using System;
using UnityEngine;

public class PlayableToken : APlayableItem, IRulebookEntry
{
    public override bool OnlyVisibleOnOverview => true;
    public override bool CanInteractWithoutOwnership => true;

    [SerializeField] private Token _token;
    public override AActionItem ActionItem => _token;
    public override int IndexInHand { get; set; } = -1;



    public string GetName() => _token.Name;

    public string GetDescription() => _token.Description;
    
    
    public override void Play(IActionReceiver playLocation, Action onPlayedCallback)
    {
        throw new NotImplementedException();
    }
}