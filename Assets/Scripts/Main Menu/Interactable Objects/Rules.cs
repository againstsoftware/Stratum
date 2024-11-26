using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Rules : AInteractableObject
{
    [SerializeField] private Canvas _canvas;
    private Animator _animator;
    private float waitTime = 0.3f;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Libro pulsaod");
    }

    public override void EnableInteraction()
    {
        _isEnabled = true;
        _animator.SetBool("_isEnabled", _isEnabled);
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            StartCoroutine(WaitForOneSecond());
        }

    }

    public override void DisableInteraction()
    {
        _isEnabled = false;
        _animator.SetBool("_isEnabled", _isEnabled);
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
        {
            HideText();
        }

        gameObject.transform.localScale /= scaleIncrease;
    }

    public void TutorialButtonPress() => SceneManager.LoadScene("Tutorial");

    private void ShowText()
    {
        foreach (Transform child in _canvas.transform)
        {
            child.gameObject.SetActive(true);
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
