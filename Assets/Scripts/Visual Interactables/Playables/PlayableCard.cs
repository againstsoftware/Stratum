using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayableCard : APlayableItem, IActionReceiver, IRulebookEntry
{
    public override bool OnlyVisibleOnOverview => false;
    public override bool CanInteractWithoutOwnership => _canInteractWithoutOwnership;

    public override AActionItem ActionItem => Card;
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
    [SerializeField] private MeshRenderer _mesh;

    
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
            _actionCompletedCallback?.Invoke();
            _actionCompletedCallback = null;
            onPlayedCallback();
            return;
        }
        
        //no se ha jugado visualmente a la mesa
        Travel(playLocation.SnapTransform, _playTravelDuration, State.Played, () =>
        {
            OnPlayed(playLocation);
            _actionCompletedCallback?.Invoke();
            _actionCompletedCallback = null;
            onPlayedCallback();
        });
        
    }
    

    private void OnPlayed(IActionReceiver playLocation)
    {
        CurrentState = State.Played;
        IsDropEnabled = true;
        _canInteractWithoutOwnership = true;

        SlotWherePlaced = playLocation as SlotReceiver;
        CardWherePlaced = playLocation as PlayableCard;

        if (SlotWherePlaced is not null) SlotWherePlaced.AddCardOnTop(this);

        if (playLocation is DiscardPileReceiver)
        {
            //se puede destruir aqui tal vez? en vez de en el viewplayer
        }
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
        transform.rotation = dropLocation.SnapTransform.rotation; //?????? hay que cambiarlo
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
        InHandPosition = target.position;
        InHandRotation = target.rotation;
        Travel(target, _drawTravelDuration, State.Playable, callback);
    }

    public void ReposInHand(Transform target, Action callback)
    {
        Travel(target, _reposInHandTravelDuration, State.Playable, callback);
    }

    public void Initialize(ACard card, PlayerCharacter owner, State initialState = State.Playable)
    {
        // if (Card is not null) throw new Exception("carta ya asignada no se puede inicializar!");
        if (card is null)
        {
            return;
        }
        
        SetCard(card);
        Owner = owner;
        CurrentState = initialState;
            
    }

    public void InitializeOnSlot(ACard card, PlayerCharacter slotOwner, SlotReceiver slot)
    {
        // if (Card is not null) throw new Exception("carta ya asignada no se puede inicializar!");
        if (card is null)
        {
            return;
        }
        
        SetCard(card);
        Owner = slotOwner;
        CurrentState = State.Played;
        IsDropEnabled = true;
        _canInteractWithoutOwnership = true;
        SlotWherePlaced = slot;
    }
    
    

    public void SetCard(ACard card)
    {

        Card = card;
        _mesh.materials[1].mainTexture = card.ObverseTex;

        _mesh.GetComponent<Collider>().enabled = true;
    }

}