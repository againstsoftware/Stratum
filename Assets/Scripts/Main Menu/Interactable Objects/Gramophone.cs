using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class Gramophone : MonoBehaviour, IMenuInteractable
{
    private float scaleIncrease = 1.2f;
    public InteractablesObjects InteractableObject {get; private set; } = InteractablesObjects.Gramophone;

    // variables propias
    [SerializeField] private Collider Language, Handle, Disc;

    private bool _isEnabled = false;


    // para pruebas
    private AudioSource audioSource;
    public AudioClip mp3Clip; 

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        if(!_isEnabled) gameObject.transform.localScale *= scaleIncrease;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!_isEnabled) gameObject.transform.localScale /= scaleIncrease;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerCLick - MenuObject"); 

        if(_isEnabled && (eventData.pointerCurrentRaycast.gameObject.GetComponent<Collider>() == Language))
        {

        }
        if(_isEnabled && (eventData.pointerCurrentRaycast.gameObject.GetComponent<Collider>() == Handle))
        {

        }

        if(_isEnabled && (eventData.pointerCurrentRaycast.gameObject.GetComponent<Collider>() == Disc))
        {
            audioSource.volume = (audioSource.volume + 0.2f > 1.0f) ? 0f : audioSource.volume + 0.2f;
        }
    }

    public void Interact()
    {
        
    }

    public void EnableInteraction()
    {
        _isEnabled = true;
        audioSource = GetComponent<AudioSource>();

        // Asignar el AudioClip (tu archivo MP3) al AudioSource
        if (mp3Clip != null)
        {
            audioSource.clip = mp3Clip;
            audioSource.Play(); // Reproducir el audio
        }
    }

    public void DisableInteraction()
    {
        _isEnabled = false;
    }
}
