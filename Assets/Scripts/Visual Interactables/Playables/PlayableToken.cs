using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayableToken : APlayableItem, IRulebookEntry
{
    public override bool OnlyVisibleOnOverview => true;
    public override bool CanInteractWithoutOwnership => true;

    [SerializeField] private AToken _token;
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

    public void Play(IActionReceiver playLocation, Action onPlayedCallback)
    {
        if (CurrentState is not State.Playable/* && IsOnPlayLocation(playLocation)*/)
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
        Travel(playLocation.GetSnapTransform(Owner), _playTravelDuration, State.Played, () =>
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
    
}