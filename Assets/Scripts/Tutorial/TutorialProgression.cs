
using System;
using UnityEngine;

public class TutorialProgression : MonoBehaviour
{
    public static TutorialProgression Instance { get; private set; }

    [SerializeField] private ATutorialSequence[] _tutorials;
    private int _currentTutorialIndex = 0;
    private void Awake()
    {
        if(Instance is not null && Instance != this) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);   
        }
    }

    public ATutorialSequence GetTutorial() => _tutorials[_currentTutorialIndex++];
}
