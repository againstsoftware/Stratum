using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class GamePrefsInitializer : MonoBehaviour
{
    private void Start()
    {
        // cargar ajustes
        StartCoroutine(LoadSavedLanguage());
        LoadGraphicsQuality();
        LoadAudioVolume();
    }
    private IEnumerator LoadSavedLanguage()
    {
        yield return LocalizationSettings.InitializationOperation;

        string savedLocaleCode = PlayerPrefs.GetString(GamePrefs.LanguagePrefKey);
        var savedLocale = LocalizationSettings.AvailableLocales.GetLocale(savedLocaleCode);
        LocalizationSettings.SelectedLocale = savedLocale;
        Debug.Log($"language {savedLocaleCode}");
    }
    private void LoadAudioVolume()
    {
        MusicManager.Instance.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(GamePrefs.AudioPrefKey, 1.0f);
        MusicManager.Instance.PlayMusic("MenuTheme");

    }

    private void LoadGraphicsQuality()
    {
        int savedQuality = PlayerPrefs.GetInt(GamePrefs.QualityPrefKey, 1);
        QualitySettings.SetQualityLevel(savedQuality);
        Debug.Log($"quality {savedQuality}");

    }
}
