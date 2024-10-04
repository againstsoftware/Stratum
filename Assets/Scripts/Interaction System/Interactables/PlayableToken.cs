using UnityEngine;

public class PlayableToken : APlayableItem, IActionItem //<- cuestionable pero simple
{
    public override bool OnlyVisibleOnOverview => true;
    public override bool CanInteractWithoutOwnership => true;

    public override IActionItem ActionItem => this;//DEMOMENTO creo
    
    [field:SerializeField] public ValidAction[] ValidActions { get; private set; }
}
