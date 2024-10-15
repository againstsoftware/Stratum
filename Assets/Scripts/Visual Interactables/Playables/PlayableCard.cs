using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayableCard : APlayableItem, IActionReceiver, IRulebookEntry
{
    public override bool OnlyVisibleOnOverview => false;
    public override bool CanInteractWithoutOwnership => _canInteractWithoutOwnership;

    public override AActionItem ActionItem => Card;
    public override int IndexInHand { get; set; }
    public bool IsDropEnabled { get; private set; } = false;

    public ACard Card { get; private set; }
    [field:SerializeField] public Transform SnapTransform { get; private set; }
    

    public string GetName() => Card.Name;
    public string GetDescription() => Card.Description;
    public int IndexOnSlot { get; set; } = -1;

    public SlotReceiver SlotWherePlaced { get; private set; }
    public PlayableCard CardWherePlaced { get; private set; }

    public Action<PlayableCard> OnCardPlayed;
    
    
    [SerializeField] private float  _drawTravelDuration, _reposInHandTravelDuration;
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
        OnCardPlayed?.Invoke(this);
        
        if (CurrentState is not State.Playable && IsOnPlayLocation(playLocation))
        {
            OnPlayed(playLocation);
            onPlayedCallback();
            _actionCompletedCallback?.Invoke();
            _actionCompletedCallback = null;
            return;
        }
        
        //no se ha jugado visualmente a la mesa
        Travel(playLocation.SnapTransform, _playTravelDuration, State.Played, () =>
        {
            OnPlayed(playLocation);
            onPlayedCallback();
            _actionCompletedCallback?.Invoke();
            _actionCompletedCallback = null;
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
        if (_destroyed) return;
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


    public override void OnDrop(IActionReceiver dropLocation, Action actionCompletedCallback)
    {
        base.OnDrop(dropLocation, actionCompletedCallback);
        transform.parent = null;
        transform.rotation = dropLocation.SnapTransform.rotation;
    }

    public override void OnDragCancel()
    {
        transform.parent = _hand;
        base.OnDragCancel();
    }


    public Receiver GetReceiverStruct(ValidDropLocation actionDropLocation)
    {
        return new (actionDropLocation, Owner, 
            CardWherePlaced is not null ? 
                    CardWherePlaced.SlotWherePlaced.IndexOnTerritory : SlotWherePlaced.IndexOnTerritory,
            CardWherePlaced is not null ? CardWherePlaced.IndexOnSlot : IndexOnSlot);
    }


    public void DrawTravel(Transform target, Action callback)
    {
        _inHandPosition = target.position;
        _inHandRotation = target.rotation;
        Travel(target, _drawTravelDuration, State.Playable, callback);
    }

    public void ReposInHand(Transform target, Action callback)
    {
        Travel(target, _reposInHandTravelDuration, State.Playable, callback);
    }

    public void Initialize(ACard card, PlayerCharacter owner)
    {
        if (Card is not null) throw new Exception("carta ya asignada no se puede reasignar!");
        Card = card;
        Owner = owner;
    }

}