
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CSVLoader : MonoBehaviour
{
    public Dictionary<string, string> English, Spanish;
    
    // Función que se llamará desde la clase estática
    public void LoadCSVAsync(string fileName, Action onCompleted)
    {
        StartCoroutine(LoadCSVCoroutine(fileName, onCompleted));
    }

    // Corrutina que carga el CSV de manera asíncrona
    private IEnumerator LoadCSVCoroutine(string fileName, Action onCompleted)
    {
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, $"{fileName}.csv");

        UnityWebRequest www = UnityWebRequest.Get(filePath);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Error al cargar el CSV: {www.error}");
            yield break;
        }

        string content = www.downloadHandler.text;
        string[] lines = content.Split('\n');

        English = new();
        Spanish = new();

        foreach (string line in lines)
        {
            string[] columns = line.Split(';');

            if (columns.Length < 4)
            {
                Debug.LogWarning($"Línea ignorada por no tener suficientes columnas: {line}");
                continue;
            }

            string key = columns[0].Trim();
            string valueSpanish = columns[2].Trim();
            string valueEnglish = columns[3].Trim();

            if (!string.IsNullOrEmpty(key))
            {
                Spanish[key] = valueSpanish;
                English[key] = valueEnglish;
            }
        }

        Debug.Log("Archivo CSV procesado exitosamente.");

        // Llamar al callback si se pasó
        onCompleted?.Invoke();

        // Destruir el objeto temporal para limpiar
        Destroy(gameObject);
    }
}
