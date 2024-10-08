using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayableCard : APlayableItem, IActionReceiver, IRulebookEntry
{
    public override bool OnlyVisibleOnOverview => false;
    public override bool CanInteractWithoutOwnership => _canInteractWithoutOwnership;

    public override IActionItem ActionItem => Card;
    public override int IndexInHand { get; set; }
    public bool IsDropEnabled { get; private set; } = false;

    [field:SerializeField] public ACard Card { get; private set; }
    [field:SerializeField] public Transform SnapTransform { get; private set; }


    public string GetName() => Card.Name;
    public string GetDescription() => Card.Description;
    public int IndexOnSlot { get; set; } = -1;

    public SlotReceiver SlotWherePlaced { get; private set; }
    public PlayableCard CardWherePlaced { get; private set; }
    
    [SerializeField] private float _closestCardZ;
    
    
    private float _startZ;
    private bool _canInteractWithoutOwnership = false;

    private Transform _hand;

    protected override void Start()
    {
        _hand = transform.parent;
    }
    
    
    public override void Play(IActionReceiver playLocation, Action onPlayedCallback)
    {
        if (CurrentState is not State.Playable && IsOnPlayLocation(playLocation))
        {
            OnPlayed(playLocation);
            onPlayedCallback();
            return;
        }
        
        //no se ha jugado visualmente a la mesa
        Travel(playLocation.SnapTransform, State.Played, () =>
        {
            OnPlayed(playLocation);
            onPlayedCallback();
        });
        
    }

    protected bool IsOnPlayLocation(IActionReceiver playLocation)
    {
        var distance = playLocation.SnapTransform.position - transform.position;
        return distance.magnitude < .01f;
    }

    private void OnPlayed(IActionReceiver playLocation)
    {
        CurrentState = State.Played;
        IsDropEnabled = true;
        _canInteractWithoutOwnership = true;

        SlotWherePlaced = playLocation as SlotReceiver;
        CardWherePlaced = playLocation as PlayableCard;

        //moverla a la playLocation
        if (SlotWherePlaced is not null) SlotWherePlaced.AddCardOnTop(this);
    }
    
    

    public override void OnSelect()
    {
        _startZ = transform.localPosition.z;
        if(CurrentState is State.Playable)
            transform.localPosition = new(transform.localPosition.x, transform.localPosition.y, _closestCardZ);
        base.OnSelect();
    }

    public override void OnDeselect()
    {
        base.OnDeselect();
        if(CurrentState is State.Playable)
            transform.localPosition = new(transform.localPosition.x, transform.localPosition.y, _startZ);
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


    public override void OnDrop(IActionReceiver dropLocation)
    {
        base.OnDrop(dropLocation);
        transform.parent = null;
        transform.rotation = dropLocation.SnapTransform.rotation;
    }

    public override void OnDragCancel()
    {
        transform.parent = _hand;
        base.OnDragCancel();
    }
    
    
    public Receiver GetReceiverStruct(ValidDropLocation actionDropLocation) => 
        new (actionDropLocation, Owner, 
            CardWherePlaced is not null ? CardWherePlaced.SlotWherePlaced.IndexOnTerritory : SlotWherePlaced.IndexOnTerritory,
            CardWherePlaced is not null ? CardWherePlaced.IndexOnSlot : IndexOnSlot);

    
    
}