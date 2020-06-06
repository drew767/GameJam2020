using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public enum EPickupType
{
    Ammo,
    Shields,
    Health
}

public class Pickup : MonoBehaviour
{
    public EPickupType m_pickupType = EPickupType.Ammo;
    public float m_respawnTime = 2.0f;

    float m_timeSincePickUp = 10000.0f;
    // Start is called before the first frame update
    void Start()
    {
        m_timeSincePickUp = m_respawnTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        m_timeSincePickUp = 0.0f;
        gameObject.SetActive(false);
        other.gameObject.GetComponent<PlayerController>().AddBonus(m_pickupType);
    }

    public bool TimeToActivate()
    {
        m_timeSincePickUp += Time.deltaTime;

        if (!gameObject.activeSelf && m_timeSincePickUp > m_respawnTime)
        {
            return true;
        }
        return false;
    }

    //adjust this to change speed
    float rotationSpeed = 1f;
    //adjust this to change speed
    public float movementSpeed = 5f;
    //adjust this to change how high it goes
    public float height = 0.002f;

    void Update()
    {
        //get the objects current position and put it in a variable so we can access it later with less code
        Vector3 pos = transform.position;
        //calculate what the new Y position will be
        float newY = pos.y + (Mathf.Sin(Time.time * movementSpeed) * height);
        //set the object's Y to the new calculated Y
        Vector3 newPos = new Vector3(pos.x, newY, pos.z);
        transform.position = newPos;
        transform.Rotate(0, rotationSpeed, 0, Space.Self);
    }
}

