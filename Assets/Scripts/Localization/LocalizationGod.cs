using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;


public static class LocalizationGod
{
    private class Table
    {
        public readonly string Name;

        public IReadOnlyDictionary<string, string> English
        {
            get => _english;
        }

        public IReadOnlyDictionary<string, string> Spanish
        {
            get => _spanish;
        }


        private Dictionary<string, string> _english, _spanish;


        public Table(string name, string filePath)
        {
            Name = name;
            LoadCSV(filePath);
        }
        
        private void LoadCSV(string filePath)
        {
            // Inicializar los diccionarios
            _english = new Dictionary<string, string>();
            _spanish = new Dictionary<string, string>();

            // Verificar si el archivo existe
            if (!File.Exists(filePath))
            {
                throw new Exception($"El archivo CSV no existe en la ruta: {filePath}");
                
            }

            // Leer las líneas del archivo
            string[] lines;
            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                var content = reader.ReadToEnd();
                lines = content.Split('\n');
            }

            // Procesar cada línea del CSV (suponiendo que la primera línea no contiene encabezados)
            foreach (string line in lines)
            {
                // Dividir la línea por el delimitador ';'
                string[] columns = line.Split(';');

                // Verificar si tiene al menos 4 columnas (0, 1, 2, 3)
                if (columns.Length < 4)
                {
                    UnityEngine.Debug.LogWarning($"Línea ignorada por no tener suficientes columnas: {line}");
                    continue;
                }

                string key = columns[0].Trim();
                string valueSpanish = columns[2].Trim();
                string valueEnglish = columns[3].Trim();

                // Agregar al diccionario si la clave no es nula o vacía
                if (!string.IsNullOrEmpty(key))
                {
                    _spanish[key] = valueSpanish;
                    _english[key] = valueEnglish;
                }
            }

            UnityEngine.Debug.Log("Archivo CSV procesado exitosamente.");
        }
    }


    private static LocalizationGodConfig _config;
    private static Dictionary<string, Table> _tables = new();
    
    public static void Init(LocalizationGodConfig config)
    {
        if (_config is not null) return;
        _config = config;
    }

    public static string GetLocalized(string tableName, string tablekey)
    {
        if (!_tables.TryGetValue(tableName, out var table))
        {
            if (!_config.TableNames.Contains(tableName))
                throw new Exception($"Tabla {tableName} no registrada en config.");
            table = LoadTable(tableName);
        }

        return GetFromTable(table, tablekey);
    }


    private static string GetFromTable(Table table, string key)
    {
        var dict = _config.Spanish ? table.Spanish : table.English;

        if (dict.TryGetValue(key, out var value)) return value;
        throw new Exception($"Key {key} no encontrada en la tabla {table.Name}:");
    }

    private static Table LoadTable(string tableName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, $"{tableName}.csv");
        var table = new Table(tableName, filePath);
        _tables.Add(tableName, table);
        return table;
    }
    
    
    
}