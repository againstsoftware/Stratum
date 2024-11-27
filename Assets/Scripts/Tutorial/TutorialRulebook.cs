
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TutorialRulebook : MonoBehaviour
{
    
    [SerializeField] private float _minSpeed, _maxSpeed;
    
    private Rulebook _rulebook;

    private Animator _animator;
    private static readonly int _speed = Animator.StringToHash("Speed");
    private static readonly int _yap = Animator.StringToHash("yap");
    
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void RandomizeSpeed()
    {
        _animator.SetFloat(_speed, Random.Range(_minSpeed, _maxSpeed));
    }
    
    public void DisplayTutorialDialogue(TutorialDialogue dialogue, Action onFinished)
    {
        _animator.SetBool(_yap, true);
        _rulebook.DisplayDialogue(dialogue, () => _animator.SetBool(_yap, false),onFinished);
    }

    public void SetLocalPlayer(PlayerCharacter localPlayer, Camera cam)
    {
        _rulebook = ServiceLocator.Get<IView>().GetViewPlayer(localPlayer).GetComponentInChildren<Rulebook>();

        //muy cutre perdon pero son las 12:04 y me quiero sobar

        var angle = localPlayer switch
        {
            PlayerCharacter.Sagitario => 0f,
            PlayerCharacter.Fungaloth => -90f,
            PlayerCharacter.Ygdra => 180f,
            PlayerCharacter.Overlord => 90f,
        };
        transform.rotation = Quaternion.identity;
        transform.RotateAround(Vector3.zero, Vector3.up, angle);
        
        transform.LookAt(cam.transform);
        
    }
}
