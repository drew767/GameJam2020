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

public class BulletPickup : MonoBehaviour
{
    [SerializeField]
    float m_speed = 1.0f;
    [SerializeField]
    float m_pickupDistance = 1.0f;
    [SerializeField]
    float m_attractionDistance = 10.0f;
    [SerializeField]
    EPickupType m_pickupType = EPickupType.Ammo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerController player = GameManager.GetInstance().Player;
        if (!player)
        {
            return;
        }
        Vector3 distance = player.transform.position - transform.position;
        if (distance.magnitude < m_pickupDistance)
        {
            player.gameObject.SendMessage("OnPickup", this, SendMessageOptions.DontRequireReceiver);
            Destroy(this.gameObject);
        }
        else if (distance.magnitude < m_attractionDistance)
        {
            Vector3 direction = distance.normalized;
            direction = new Vector3(direction.x, 0.0f, direction.z);
            Rigidbody rb = GetComponent<Rigidbody>();

            rb.velocity = direction * m_speed * Time.deltaTime * 10.0f;
        }
    }
}
