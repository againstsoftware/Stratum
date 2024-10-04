
using UnityEngine;
using UnityEngine.Serialization;

public class PlayableCard : APlayableItem, IActionReceiver
{
    public override bool OnlyVisibleOnOverview => false;
    public override bool CanInteractWithoutOwnership => _canInteractWithoutOwnership;

    public override IActionItem ActionItem => Card;
    public bool IsDropEnabled { get; private set; } = false;

    [field:SerializeField] public ACard Card { get; private set; }
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
        OnSelect();
    }

    public void OnDraggingDeselect()
    {
        OnDeselect();
    }

    public void OnChoosingSelect()
    {
        OnSelect();
    }

    public void OnChoosingDeselect()
    {
        OnDeselect();
    }
}