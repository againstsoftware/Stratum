using UnityEngine;

public class PlayableToken : APlayableItem, IRulebookEntry
{
    public override bool OnlyVisibleOnOverview => true;
    public override bool CanInteractWithoutOwnership => true;

    [SerializeField] private Token _token;
    public override IActionItem ActionItem => _token;


    public string GetName() => _token.Name;

    public string GetDescription() => _token.Description;
}
