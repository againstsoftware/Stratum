using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Rulebook : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText, _descriptionText, _dialogueText;
    [SerializeField] private float _dialogueSpeed;
    [SerializeField] private float _dialogueEndDelay;
    
    private Animator _animator;
    private int _upAnim, _downAnim;
    private bool _isUp;
    private IRulebookEntry _currentEntry;
    private bool _isOnDialogue;

    private Action _nextDialogueCallback, _dialogueEndCallback;
    private bool _hasClickedOnDialogue;
    
    private void Awake()
    {
        _upAnim = Animator.StringToHash("Up");
        _downAnim = Animator.StringToHash("Down");
        _animator = GetComponentInChildren<Animator>();
        _nameText.text = _descriptionText.text = "";
    }

    public void ShowRulebookEntry(IRulebookEntry entry)
    {
        if (_isOnDialogue) return;
        if (_currentEntry == entry) return;

        _dialogueText.text = "";
        
        if (entry is null)
        {
            HideRulebook();
            return;
        }
        
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
        if (_isOnDialogue) return;
        _currentEntry = null;
        StartCoroutine(HideDelayAux());
    }

    public void DisplayDialogue(TutorialDialogue dialogue, Action endCallback, Action nextCallback)
    {
        if (_isOnDialogue) return;

        ServiceLocator.Get<IInteractionSystem>().Input.Press += OnPress;
        _hasClickedOnDialogue = false;

        
        Debug.Log("comenzando dialogo");
        if (_nextDialogueCallback is not null)
            Debug.LogWarning("dialogue callback no era null, se ha perdido el anterior callback");
        
        _dialogueEndCallback = endCallback;
        _nextDialogueCallback = nextCallback;
        
        if (!_isUp)
        {
            _animator.Play(_upAnim);
            _isUp = true;
        }
        
        StartCoroutine(TypeDialogue(dialogue));
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

    private IEnumerator TypeDialogue(TutorialDialogue dialogue)
    {
        _isOnDialogue = true;
        _nameText.text = "";
        _descriptionText.text = "";
        _dialogueText.text = dialogue.Text;
        _dialogueText.ForceMeshUpdate();

        _dialogueText.alpha = 0; // Inicialmente, el texto completo es transparente
        TMP_TextInfo textInfo = _dialogueText.textInfo;
        Color32[] newVertexColors;
        int totalVisibleCharacters = textInfo.characterCount;

        HashSet<char> slowedChars = new() { '.', ',' };

        var dialogueDelay = 1f / _dialogueSpeed;
        
        for (int i = 0; i < totalVisibleCharacters; i++)
        {
            // Obtener el índice de material (importante para mallas con varios materiales)
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            // Obtén el array de colores de los vértices
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // Modifica el alfa del color del carácter
            for (int j = 0; j < 4; j++) // Cada carácter tiene 4 vértices
            {
                newVertexColors[vertexIndex + j].a = 255; // Cambia alfa a opaco
            }

            // Actualiza la malla
            _dialogueText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

            var currentChar = _dialogueText.text[i];

            if (_hasClickedOnDialogue) yield return null;
            else
            {
                var delay = slowedChars.Contains(currentChar) ? dialogueDelay * 4f : dialogueDelay;
                yield return new WaitForSeconds(delay);
            }
        }
        
        _dialogueEndCallback?.Invoke();
        _dialogueEndCallback = null;
        _hasClickedOnDialogue = false;
        yield return new WaitUntil(() => _hasClickedOnDialogue);
        _isOnDialogue = false;
        ServiceLocator.Get<IInteractionSystem>().Input.Press -= OnPress;
        _nextDialogueCallback.Invoke();
        _nextDialogueCallback = null;
        _dialogueText.text = "";
        HideRulebook();
    }

    private void OnPress()
    {
        if (!_isOnDialogue) return;

        _hasClickedOnDialogue = true;
    }
    
}
