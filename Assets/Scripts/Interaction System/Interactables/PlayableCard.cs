
using UnityEngine;

public class PlayableCard : APlayableItem, IActionReceiver
{
    public override bool OnlyVisibleOnOverview => false;
    public override bool CanInteractWithoutOwnership => _canInteractWithoutOwnership;

    public override IActionItem ActionItem => _card;
    public bool IsPlayed { get; private set; }
    public bool IsDropEnabled { get; private set; } = false;

    [SerializeField] private ACard _card;
    private bool _canInteractWithoutOwnership = false;

    public void PlayCard(IActionReceiver playLocation)
    {
        IsPlayed = true;
        IsDraggable = false;
        IsDropEnabled = true;
        _canInteractWithoutOwnership = true;
        //moverla a la playLocation
    }

    public void OnDraggingSelect()
    {
        throw new System.NotImplementedException();
    }

    public void OnDraggingDeselect()
    {
        throw new System.NotImplementedException();
    }
}