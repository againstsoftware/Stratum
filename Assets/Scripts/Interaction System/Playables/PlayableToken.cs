using UnityEngine;

public class PlayableToken : APlayableItem
{
    public override bool OnlyVisibleOnOverview => true;
    public override bool CanInteractWithoutOwnership => true;

    [SerializeField] private Token _token;
    public override IActionItem ActionItem => _token;


}
