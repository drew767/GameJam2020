using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeout
{
    float m_expirationTime;
    public Timeout(float seconds)
    {
        m_expirationTime = Time.time + seconds;
    }

    public bool Expired() 
    {
        return Time.time > m_expirationTime; 
    }
}


public static class TimeoutExtension
{
    public static bool ExpiredOrNull(this Timeout list)
    {
        return list == null || list.Expired();
    }

}