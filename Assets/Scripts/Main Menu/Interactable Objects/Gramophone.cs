using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;


public class Gramophone : AInteractableObject
{
    [SerializeField] private Collider Language, Graphics, Volume;
    [SerializeField] private RenderPipelineAsset[] qualityLevels;

    public override void OnPointerClick(PointerEventData eventData)
    {
        // language
        if (_isEnabled && (eventData.pointerCurrentRaycast.gameObject.GetComponent<Collider>() == Language))
        {
            StartCoroutine(ToggleLanguage());
        }

        // gráficos
        if (_isEnabled && (eventData.pointerCurrentRaycast.gameObject.GetComponent<Collider>() == Graphics))
        {
            ToggleGraphicsQuality();
        }

        // volumen
        if (_isEnabled && (eventData.pointerCurrentRaycast.gameObject.GetComponent<Collider>() == Volume))
        {
            AudioVolume();
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

        QualitySettings.renderPipeline = qualityLevels[newQuality];

        PlayerPrefs.SetInt(GamePrefs.QualityPrefKey, newQuality);
        PlayerPrefs.Save();

    }

    private void AudioVolume()
    {
        Debug.Log("disco tocado");
        AudioSource audioSource = MusicManager.Instance.GetComponent<AudioSource>();
        audioSource.volume = (audioSource.volume + 0.2f > 1.0f) ? 0f : audioSource.volume + 0.2f;
        PlayerPrefs.SetFloat(GamePrefs.AudioPrefKey, audioSource.volume);
        PlayerPrefs.Save();
    }
}
