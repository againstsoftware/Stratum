using System;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class APlayableItem : MonoBehaviour, IInteractable
{
    public PlayerCharacter Owner { get; protected set; } = PlayerCharacter.None;

    public abstract bool OnlyVisibleOnOverview { get; }
    public abstract bool CanInteractWithoutOwnership { get; }
    public abstract AActionItem ActionItem { get; }
    public event Action OnDiscard;

    public enum State
    {
        Playable,
        Dragging,
        Traveling,
        Waiting,
        Played
    }

    public State CurrentState { get; protected set; } = State.Playable;

    public Action<APlayableItem> OnItemDrag, OnItemDrop;
    public Vector3 InHandPosition { get; set; }
    public Quaternion InHandRotation { get; set; }


    [SerializeField] private bool _meshInChild;
    [SerializeField] protected float _playTravelDuration;
    private float _travelDuration;
    private Transform _meshTransform;
    private Vector3 _defaultMeshScale;
    private Collider _collider;
    private float _t;
    private Vector3 _travelStartPosition, _travelEndPosition;
    private Quaternion _travelStartRotation, _travelEndRotation;
    private State _travelEndState;
    private Action _onTravelEndCallback;

    protected bool _destroyed;

    private static int s_id = 0;
    private int c_id;

    protected virtual void Awake()
    {
        _meshTransform = _meshInChild ? transform.GetChild(0) : transform;
        // if (!_meshTransform.TryGetComponent<MeshRenderer>(out _))
        //     throw new Exception($"Playable Item without mesh child! ({gameObject.name})");

        _collider = _meshTransform.GetComponent<Collider>();
        _defaultMeshScale = _meshTransform.localScale;

        c_id = s_id++;
    }

    protected virtual void Start()
    {
    }

    protected bool _dbWasObv = false;
    private void Update()
    {
        if (CurrentState is not State.Traveling) return;

        transform.position = Vector3.Lerp(_travelStartPosition, _travelEndPosition, _t);
        transform.rotation = Quaternion.Lerp(_travelStartRotation, _travelEndRotation, _t);
        _t += Time.deltaTime / _travelDuration;
        if (_dbIsOv || _dbWasObv)
        {
            Debug.Log($"T de carta {c_id}: {_t}. es ov:{_dbIsOv}");
        }
        if (_t >= 1f)
        {
            if(_dbWasObv) Debug.Log("t > 1 en carta de oberlord");
            _collider.enabled = true;
            transform.SetPositionAndRotation(_travelEndPosition, _travelEndRotation);
            CurrentState = _travelEndState;
            _onTravelEndCallback?.Invoke();
            _onTravelEndCallback = null;
        }
    }

    private void OnDestroy()
    {
        if (_dbWasObv)
        {
            Debug.Log("destroyendo carta de overlord");
        }
        _destroyed = true;
        OnDiscard?.Invoke();
    }
    

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
        if (_dbWasObv)
        {
            Debug.Log("Drageando item de overlord");
        }
        CurrentState = State.Dragging;
        OnDeselect();
        OnItemDrag?.Invoke(this);
    }

    public virtual void OnDragCancel()
    {
        ReturnToHand(null);
    }

    public virtual void OnDrop(IActionReceiver dropLocation)
    {
        //se snappea a la drop location
        transform.position = dropLocation.GetSnapTransform(Owner).position;
        if (_dbWasObv)
        {
            Debug.Log("Droppeando item de overlord");
        }
        CurrentState = State.Waiting;
        // _actionCompletedCallback = actionCompletedCallback;
        OnItemDrop?.Invoke(this);
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
        _travelEndPosition = InHandPosition;
        _travelEndRotation = InHandRotation;
        _onTravelEndCallback = callback;
    }

    protected bool _dbIsOv = false;
    protected void Travel(Transform target, float duration, State endState, Action callback, bool dbIsOv = false)
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

        if (_onTravelEndCallback != null)
        {
            Debug.LogWarning("Volviendo a viajar item que no ha ejecutado el callback!!");
            _onTravelEndCallback.Invoke();
        }
        
        _onTravelEndCallback = callback;

        if (_dbIsOv) _dbWasObv = true;
        _dbIsOv = dbIsOv;
        if (_dbWasObv)
        {
            Debug.Log("viajando carta de overlord");
        }
    }

    protected bool IsOnPlayLocation(IActionReceiver playLocation)
    {
        var distance = playLocation.GetSnapTransform(Owner).position - transform.position;
        return distance.magnitude < .01f;
    }
}