using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    private static Dictionary<Type, List<Action<object>>> m_listeneres = new Dictionary<Type, List<Action<object>>>();

    public static void RegisterListener<T>(Action<object> listener) where T : class
    {
        if (m_listeneres.ContainsKey(typeof(T)) == false)
        {
            m_listeneres.Add(typeof(T), new List<Action<object>>());
        }
        m_listeneres[typeof(T)].Add(listener);
    }

    public static void UnregisterListener<T>(Action<object> listener) where T : class
    {
        if (m_listeneres.ContainsKey(typeof(T)) == false)
        {
            throw new Exception("There is no such listener");
        }
        m_listeneres[typeof(T)].Remove(listener);
    }

    public static void SendEvent<T>(T incomingEvent) where T : class
    {
        if (m_listeneres.ContainsKey(typeof(T)) == false)
        {
            return;
        }

        foreach (var listener in m_listeneres[typeof(T)])
        {
            listener.Invoke(incomingEvent);
        }
    }
}
