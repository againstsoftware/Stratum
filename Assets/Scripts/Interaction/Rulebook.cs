using System;
using System.Collections;
using TMPro;
using UnityEngine;


public class Rulebook : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText, _descriptionText;
    
    private Animator _animator;
    private int _upAnim, _downAnim;
    private bool _isUp;
    private IRulebookEntry _currentEntry;

    private void Awake()
    {
        _upAnim = Animator.StringToHash("Up");
        _downAnim = Animator.StringToHash("Down");
        _animator = GetComponentInChildren<Animator>();
        _nameText.text = _descriptionText.text = "";
    }

    public void ShowRulebookEntry(IRulebookEntry entry)
    {
        if (_currentEntry == entry) return;
        
        _currentEntry = entry;
        _currentEntry.OnDiscard += OnEntryDiscard;
        _nameText.text = entry.GetName();
        _descriptionText.text = entry.GetDescription();

        if (_isUp) return;
        
        _animator.Play(_upAnim);
        _isUp = true;
    }

    public void HideRulebook()
    {
        _currentEntry = null;
        StartCoroutine(HideDelayAux());
    }

    private IEnumerator HideDelayAux()
    {
        yield return null;
        if(_currentEntry is null) _animator.Play(_downAnim);
        _isUp = false;
    }

    private void OnEntryDiscard()
    {
        if (_currentEntry is null) return;
        _currentEntry.OnDiscard -= OnEntryDiscard;

        HideRulebook();
    }
    
    
}
