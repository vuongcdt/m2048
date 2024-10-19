using System;
using System.Collections.Generic;

public class Observer
{
    static Dictionary<string, Action<object>> Listeners = new();

    public static void On(string name, Action<object> callback)
    {
        if (!Listeners.ContainsKey(name))
        {
            Listeners.Add(name, callback);
        }
        else
        {
            Listeners[name] = callback;
        }
    }

    public static void Remove(string name)
    {
        if (!Listeners.ContainsKey(name))
        {
            return;
        }

        Listeners.Remove(name);
    }

    public static void Emit(string name, object data = null)
    {
        if (!Listeners.ContainsKey(name))
        {
            return;
        }

        Listeners[name].Invoke(data);
    }
}
