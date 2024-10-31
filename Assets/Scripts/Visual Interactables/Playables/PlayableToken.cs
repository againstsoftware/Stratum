using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayableToken : APlayableItem, IRulebookEntry
{
    public override bool OnlyVisibleOnOverview => true;
    public override bool CanInteractWithoutOwnership => true;

    [SerializeField] private Token _token;
    [SerializeField] private PlayerCharacter _owner;
    public override AActionItem ActionItem => _token;

    public string GetName() => _token.Name;

    public string GetDescription() => _token.Description;


    protected override void Awake()
    {
        base.Awake();
        InHandPosition = transform.position;
        InHandRotation = transform.rotation;
        Owner = _owner;
    }

    public override void Play(IActionReceiver playLocation, Action onPlayedCallback)
    {
        if (CurrentState is not State.Playable && IsOnPlayLocation(playLocation))
        {
            ReturnToHand(() =>
            {
                _actionCompletedCallback?.Invoke();
                _actionCompletedCallback = null;
                onPlayedCallback();
            });
            return;
        }

        //no se ha jugado visualmente a la mesa
        Travel(playLocation.SnapTransform, _playTravelDuration, State.Played, () =>
        {
            StartCoroutine(WaitAndDo(.5f, () =>
            {
                ReturnToHand(() =>
                {
                    _actionCompletedCallback?.Invoke();
                    _actionCompletedCallback = null;
                    onPlayedCallback();
                });
            }));
        });
    }

    private IEnumerator WaitAndDo(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }

    public override void OnDrop(IActionReceiver dropLocation, Action actionCompletedCallback)
    {
        base.OnDrop(dropLocation, actionCompletedCallback);
    }
}