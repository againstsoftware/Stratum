
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TutorialRulebook : MonoBehaviour
{
    public PlayerCharacter LocalPlayer;
    
    [SerializeField] private float _minSpeed, _maxSpeed;
    
    
    private Rulebook _rulebook;

    private Animator _animator;
    private static readonly int _speed = Animator.StringToHash("Speed");
    private static readonly int _yap = Animator.StringToHash("yap");
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void RandomizeSpeed()
    {
        _animator.SetFloat(_speed, Random.Range(_minSpeed, _maxSpeed));
    }
    
    public void DisplayTutorialDialogue(TutorialDialogue dialogue, Action onFinished)
    {
        _rulebook ??= ServiceLocator.Get<IView>().GetViewPlayer(LocalPlayer).GetComponentInChildren<Rulebook>();
        
        _rulebook.DisplayDialogue(dialogue, () => _animator.SetBool(_yap, false),onFinished);
        
        _animator.SetBool(_yap, true);
    }
}
