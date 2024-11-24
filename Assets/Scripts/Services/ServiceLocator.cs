using System.Collections.Generic;
using System;

public static class ServiceLocator
{
    private static readonly Dictionary<string, IService> _services = new();

    public static void Register<T>(IService service) where T : IService
    {
        string key = typeof(T).Name;
        if (!_services.TryAdd(key, service))
            //throw new Exception($"servicio {key} ya registrado");
            _services[key] = service;
    }
        
    public static T Get<T>() where T : IService
    {
        string key = typeof(T).Name;
            
        if (_services.TryGetValue(key, out var service))
            return (T) service;
        

        throw new Exception($"servicio {key} no registrado");
    }

    // Método para limpiar los servicios (opcional, útil para reiniciar o tests)
    public static void Clear()
    {
        _services.Clear();
    }
}
