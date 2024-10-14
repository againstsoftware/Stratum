using System;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class APlayableItem : MonoBehaviour, IInteractable
{
    public PlayerCharacter Owner { get; protected set; } = PlayerCharacter.None;
    
    public abstract bool OnlyVisibleOnOverview { get; }
    public abstract bool CanInteractWithoutOwnership { get; }
    public abstract AActionItem ActionItem { get; }
    public abstract int IndexInHand { get; set; }
    public event Action OnDiscard;
    
    public enum State { Playable, Dragging, Traveling, Waiting, Played }

    public State CurrentState { get; protected set; } = State.Playable;
    

    [SerializeField] private bool _meshInChild;
    [SerializeField] protected float _playTravelDuration;
    private float _travelDuration;
    private Transform _meshTransform;
    private Vector3 _defaultMeshScale;
    protected Vector3 _inHandPosition;
    protected Quaternion _inHandRotation;
    private Collider _collider;
    private float _t;
    private Vector3 _travelStartPosition, _travelEndPosition;
    private Quaternion _travelStartRotation, _travelEndRotation;
    private State _travelEndState;
    private Action _onTravelEndCallback;
    protected Action _actionCompletedCallback;

    protected bool _destroyed;

    protected virtual void Awake()
    {
        _meshTransform = _meshInChild ? transform.GetChild(0) : transform;
        if (!_meshTransform.TryGetComponent<MeshRenderer>(out _))
            throw new Exception($"Playable Item without mesh child! ({gameObject.name})");

        _collider = _meshTransform.GetComponent<Collider>();
        _defaultMeshScale = _meshTransform.localScale;
    }

    protected virtual void Start()
    {
    }

    private void Update()
    {
        if (CurrentState is not State.Traveling) return;

        transform.position = Vector3.Lerp(_travelStartPosition, _travelEndPosition,  _t);
        transform.rotation = Quaternion.Lerp( _travelStartRotation, _travelEndRotation, _t);
        _t += Time.deltaTime / _travelDuration;
        if (_t >= 1f)
        {
            _collider.enabled = true;
            transform.SetPositionAndRotation(_travelEndPosition, _travelEndRotation);
            CurrentState = _travelEndState;
            _onTravelEndCallback?.Invoke();
            _onTravelEndCallback = null;
        }
    }

    private void OnDestroy()
    {
        _destroyed = true;
        OnDiscard?.Invoke();
    }

    public abstract void Play(IActionReceiver playLocation, Action onPlayedCallback);


    public virtual void OnSelect()
    {
        _meshTransform.localScale = _defaultMeshScale * 1.075f;
    }

    public virtual void OnDeselect()
    {
        if (_destroyed) return;
        _meshTransform.localScale = _defaultMeshScale;
    }

    public virtual void OnDrag()
    {
        CurrentState = State.Dragging;
        OnDeselect();
    }

    public virtual void OnDragCancel()
    {
        ReturnToHand(null);
    }

    public virtual void OnDrop(IActionReceiver dropLocation, Action actionCompletedCallback)
    {
        //se snappea a la drop location
        transform.position = dropLocation.SnapTransform.position;
        CurrentState = State.Waiting;
        _actionCompletedCallback = actionCompletedCallback;
    }
 

    public void SetColliderActive(bool active) => _collider.enabled = active;


    protected void ReturnToHand(Action callback)
    {
        //animacion para volver a su pos inicial
        CurrentState = State.Traveling;
        _travelEndState = State.Playable;
        _t = 0f;
        _travelDuration = _playTravelDuration;
        _collider.enabled = false;
        _travelStartPosition = transform.position;
        _travelStartRotation = transform.rotation;
        _travelEndPosition = _inHandPosition;
        _travelEndRotation = _inHandRotation;
        _onTravelEndCallback = callback;
    }

    protected void Travel(Transform target, float duration, State endState, Action callback)
    {
        //animacion para viajar a la drop location
        CurrentState = State.Traveling;
        _travelDuration = duration;
        _travelEndState = endState;
        _t = 0f;
        _collider.enabled = false;
        _travelStartPosition = transform.position;
        _travelStartRotation = transform.rotation;
        _travelEndPosition = target.position;
        _travelEndRotation = target.rotation;
        _onTravelEndCallback = callback;
    }

}
