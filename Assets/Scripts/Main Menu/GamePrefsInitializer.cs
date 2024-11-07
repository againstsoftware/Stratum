using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class GamePrefsInitializer : MonoBehaviour
{
    private void Start()
    {
        // cargar ajustes
        LoadSavedLanguage();
        LoadAudioVolume();
        LoadGraphicsQuality();

    }
    private IEnumerator LoadSavedLanguage()
    {
        yield return LocalizationSettings.InitializationOperation;

        if (PlayerPrefs.HasKey(GamePrefs.LanguagePrefKey))
        {
            string savedLocaleCode = PlayerPrefs.GetString(GamePrefs.LanguagePrefKey);
            var savedLocale = LocalizationSettings.AvailableLocales.GetLocale(savedLocaleCode);
            LocalizationSettings.SelectedLocale = savedLocale;
            Debug.Log($"language {savedLocaleCode}");

        }
        
    }
    private void LoadAudioVolume()
    {
        // por ahora así porque no hay un soundmanager
        GameObject gramophone = GameObject.Find("Gramophone");
        gramophone.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(GamePrefs.AudioPrefKey, 1.0f);
        Debug.Log($"volumen {gramophone.GetComponent<AudioSource>().volume}");

    }

    private void LoadGraphicsQuality()
    {
        int savedQuality = PlayerPrefs.GetInt(GamePrefs.QualityPrefKey, 1);
        QualitySettings.SetQualityLevel(savedQuality);
        Debug.Log($"quality {savedQuality}");

    }
}
