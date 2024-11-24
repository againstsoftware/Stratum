using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Registry : AInteractableObject
{
    //[SerializeField] Collider Link;

    // para pruebas
    //Material materialOG;

    private Animator _animator;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private TMP_Text _leftText, _rightText;
    [SerializeField] private Collider _Link;
    private float waitTime = 0.5f;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if(_isEnabled && (eventData.pointerCurrentRaycast.gameObject.GetComponent<Collider>() == _Link))
        {
            Application.OpenURL("https://linktr.ee/againstsoftware");
        }
    }

    public override void EnableInteraction()
    {
        _isEnabled = true;
        _animator.SetBool("_isEnabled", _isEnabled);
        if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            StartCoroutine(WaitForOneSecond());
        }

    }

    public override void DisableInteraction()
    {
        _isEnabled = false;
        _animator.SetBool("_isEnabled", _isEnabled);
        if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        {
            HideText();
        }

        gameObject.transform.localScale /= scaleIncrease;
    }

    private void ShowText()
    {
        foreach (Transform child in _canvas.transform)
        {
            child.gameObject.SetActive(true);   
        }

        string language = PlayerPrefs.GetString(GamePrefs.LanguagePrefKey, "en");
        if(language == "en")
        {
            _leftText.text = "CREDITS";
            _rightText.text = "Press to visit our social medias!";
        }
        if(language == "es")
        {
            _leftText.text = "CRÉDITOS";
            _rightText.text = "¡Pulsa para visitar nuestras redes sociales!";
        }
    }

    private void HideText()
    {
        foreach (Transform child in _canvas.transform)
        {
            child.gameObject.SetActive(false);   
        }
    }

    IEnumerator WaitForOneSecond()
    {
        yield return new WaitForSeconds(waitTime);
        ShowText();
    }
}
