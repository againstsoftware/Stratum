
using System;
using UnityEngine;

public class LocalizationGodInitializer : MonoBehaviour
{
    [SerializeField] private LocalizationGodConfig _config;

    private void Awake()
    {
        LocalizationGod.Init(_config);
    }
}
