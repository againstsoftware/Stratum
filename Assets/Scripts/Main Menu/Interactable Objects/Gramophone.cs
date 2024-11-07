using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.EventSystems;


public class Gramophone : AInteractableObject
{
    [SerializeField] private Collider Language, Handle, Disc;
    
    // para pruebas
    private AudioSource audioSource;
    public AudioClip mp3Clip;

    public override void OnPointerClick(PointerEventData eventData)
    {
        // language
        if (_isEnabled && (eventData.pointerCurrentRaycast.gameObject.GetComponent<Collider>() == Language))
        {
            StartCoroutine(ToggleLanguage());
        }

        // gráficos
        if (_isEnabled && (eventData.pointerCurrentRaycast.gameObject.GetComponent<Collider>() == Handle))
        {
            ToggleGraphicsQuality();
        }

        // volumen
        if (_isEnabled && (eventData.pointerCurrentRaycast.gameObject.GetComponent<Collider>() == Disc))
        {
            AudioVolume();
        }
    }

    public override void EnableInteraction()
    {
        _isEnabled = true;
        audioSource = GetComponent<AudioSource>();

        if (mp3Clip != null)
        {
            audioSource.clip = mp3Clip;
            audioSource.Play(); // Reproducir el audio
        }
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

        Debug.Log("Language actual: " + newLocaleCode);
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
