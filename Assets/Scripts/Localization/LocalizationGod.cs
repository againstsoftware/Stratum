using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;


public static class LocalizationGod
{
    private class Table
    {
        public readonly string Name;
        
        public Dictionary<string, string> English, Spanish;


        public Table(string name)
        {
            Name = name;
        }
        
        // private void LoadCSV()
        // {
        //     string filePath = Path.Combine(Application.streamingAssetsPath, $"{Name}.csv");
        //
        //     // Inicializar los diccionarios
        //     English = new Dictionary<string, string>();
        //     Spanish = new Dictionary<string, string>();
        //
        //     // Verificar si el archivo existe
        //     if (!File.Exists(filePath))
        //     {
        //         throw new Exception($"El archivo CSV no existe en la ruta: {filePath}");
        //         
        //     }
        //
        //     // Leer las líneas del archivo
        //     string[] lines;
        //     using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
        //     {
        //         var content = reader.ReadToEnd();
        //         lines = content.Split('\n');
        //     }
        //
        //     // Procesar cada línea del CSV (suponiendo que la primera línea no contiene encabezados)
        //     foreach (string line in lines)
        //     {
        //         // Dividir la línea por el delimitador ';'
        //         string[] columns = line.Split(';');
        //
        //         // Verificar si tiene al menos 4 columnas (0, 1, 2, 3)
        //         if (columns.Length < 4)
        //         {
        //             UnityEngine.Debug.LogWarning($"Línea ignorada por no tener suficientes columnas: {line}");
        //             continue;
        //         }
        //
        //         string key = columns[0].Trim();
        //         string valueSpanish = columns[2].Trim();
        //         string valueEnglish = columns[3].Trim();
        //
        //         // Agregar al diccionario si la clave no es nula o vacía
        //         if (!string.IsNullOrEmpty(key))
        //         {
        //             Spanish[key] = valueSpanish;
        //             English[key] = valueEnglish;
        //         }
        //     }
        //
        //     UnityEngine.Debug.Log("Archivo CSV procesado exitosamente.");
        // }
        
        public void LoadCSV(Action onCompleted = null)
        {
            // Llamar a un MonoBehaviour para hacer la solicitud asíncrona
            var loaderGameObject = new GameObject("CSVLoader");
            var loaderComponent = loaderGameObject.AddComponent<CSVLoader>();
            loaderComponent.LoadCSVAsync(Name, () =>
            {
                English = loaderComponent.English;
                Spanish = loaderComponent.Spanish;
                onCompleted?.Invoke();
            });
        }
    }


    public static bool Spanish { get; private set; } = true;
    
    private static readonly Dictionary<string, Table> _tables = new();

    public static bool IsInitialized
    {
        get => _cardsLoaded && _tutorialLoaded;
    }
    
    private static bool _cardsLoaded = false;
    private static bool _tutorialLoaded = false;

    public static void Init()
    {
        if (IsInitialized) return;
        
        
        LoadTable("Cards", () => _cardsLoaded = true);
        LoadTable("Tutorial", () => _tutorialLoaded = true);
    }

    public static string GetLocalized(string tableName, string tablekey)
    {
        if (!_tables.TryGetValue(tableName, out var table))
        {
            // table = LoadTable(tableName);
            throw new Exception($"tabla {tableName} no encontrada o no cargada");
        }

        return GetFromTable(table, tablekey);
    }

    public static void ToggleLanguage()
    {
        Spanish = !Spanish;
        
        if(Spanish) Debug.Log("Lenguaje español");
        else Debug.Log("English language");
    }


    private static string GetFromTable(Table table, string key)
    {
        var dict = Spanish ? table.Spanish : table.English;

        if (dict.TryGetValue(key, out var value)) return value;
        throw new Exception($"Key {key} no encontrada en la tabla {table.Name}:");
    }

    private static void LoadTable(string tableName, Action callback)
    {
        var table = new Table(tableName);

        table.LoadCSV(() =>
        {
            _tables.Add(tableName, table);
            callback?.Invoke();
        });

    }
    
    
    
}