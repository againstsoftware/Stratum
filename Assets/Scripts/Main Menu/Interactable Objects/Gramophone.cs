using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
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

        // language
        if(_isEnabled && (eventData.pointerCurrentRaycast.gameObject.GetComponent<Collider>() == Language))
        {
            StartCoroutine(ToggleLanguage());
        }

        // gráficos
        if(_isEnabled && (eventData.pointerCurrentRaycast.gameObject.GetComponent<Collider>() == Handle))
        {
            ToggleGraphicsQuality();
        }

        // volumen
        if(_isEnabled && (eventData.pointerCurrentRaycast.gameObject.GetComponent<Collider>() == Disc))
        {
            AudioVolume();
        }
    }

    public void Interact()
    {
        
    }

    public void EnableInteraction()
    {
        _isEnabled = true;
        audioSource = GetComponent<AudioSource>();

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

    private IEnumerator ToggleLanguage()
    {
        yield return LocalizationSettings.InitializationOperation;

        var currentLocale = LocalizationSettings.SelectedLocale;
        string newLocaleCode = currentLocale.Identifier.Code == "en" ? "es" : "en";

        var newLocale = LocalizationSettings.AvailableLocales.GetLocale(newLocaleCode);
        LocalizationSettings.SelectedLocale = newLocale;

        PlayerPrefs.SetString(GamePrefs.LanguagePrefKey, newLocaleCode);
        PlayerPrefs.Save();

        Debug.Log("Language actual: " + GamePrefs.LanguagePrefKey);
    }
    private void ToggleGraphicsQuality()
    {
        int currentQuality = QualitySettings.GetQualityLevel();

        int newQuality = (currentQuality + 1) % 3;
        QualitySettings.SetQualityLevel(newQuality);

        PlayerPrefs.SetInt(GamePrefs.QualityPrefKey, newQuality);
        PlayerPrefs.Save();
    }
    
    private void AudioVolume()
    {
        // así porque falta un soundmanager
        audioSource.volume = (audioSource.volume + 0.2f > 1.0f) ? 0f : audioSource.volume + 0.2f;
        PlayerPrefs.SetFloat(GamePrefs.AudioPrefKey, audioSource.volume);
        PlayerPrefs.Save();
    }
}


