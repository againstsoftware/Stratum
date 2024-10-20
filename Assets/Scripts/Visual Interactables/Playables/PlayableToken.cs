using System;
using UnityEngine;

public class PlayableToken : APlayableItem, IRulebookEntry
{
    public override bool OnlyVisibleOnOverview => true;
    public override bool CanInteractWithoutOwnership => true;

    [SerializeField] private Token _token;
    public override AActionItem ActionItem => _token;
    
    public string GetName() => _token.Name;

    public string GetDescription() => _token.Description;


    protected override void Awake()
    {
        base.Awake();
        InHandPosition = transform.position;
        InHandRotation = transform.rotation;
    }
    
    public override void Play(IActionReceiver playLocation, Action onPlayedCallback)
    {
        throw new NotImplementedException();
    }
}