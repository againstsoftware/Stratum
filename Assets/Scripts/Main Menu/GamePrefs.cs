using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GamePrefs 
{
    // Variables necesarias para cambios PlayerPrefs
    public enum LanguageEnum { English, Spanish }

    // PlayerPrefs    
    public const string LanguagePrefKey = "language_selected";
    public const string AudioPrefKey = "audio_volume";
    public const string QualityPrefKey = "graphics_quality"; 

    
}
