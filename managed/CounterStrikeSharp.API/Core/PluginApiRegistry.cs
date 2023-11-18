using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CounterStrikeSharp.API.Core;

public class PluginApiRegistry
{
    private readonly Dictionary<Type, object> _libraries = new();

    public void AddApi<T>(object instance) where T : class
    {
        _libraries[typeof(T)] = instance;
    }

    public void RemoveApi<T>()
    {
        if (_libraries.ContainsKey(typeof(T)))
            _libraries.Remove(typeof(T));
    }
    
    public void RemoveApi(Type type) 
    {
        if (_libraries.ContainsKey(type))
            _libraries.Remove(type);
    }

    public T? GetApi<T>() where T : class
    {
        if (_libraries.TryGetValue(typeof(T), out var library))
        {
            return (T)library;
        }

        return null;
    }

    public bool Contains<T>()
    {
        return _libraries.ContainsKey(typeof(T));
    }
}