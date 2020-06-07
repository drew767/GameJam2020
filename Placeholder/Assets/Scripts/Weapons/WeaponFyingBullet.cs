using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFyingBullet : MonoBehaviour
{
    [SerializeField]
    int m_damage = 10;
    [SerializeField]
    float m_speed = 5.0f;
    [SerializeField]
    GameObject m_explosion;

    float m_lifetime = 10.0f;

    void Start()
    {
        var rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * m_speed;
    }

    private void OnHitGameObject(GameObject go)
    {
        if (go.tag.Equals("Enemy"))
        {
            go.SendMessage("TakeDamage", m_damage);
        }

        if (m_explosion)
        {
            GameObject.Instantiate(m_explosion, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnHitGameObject(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
	{
        OnHitGameObject(collision.gameObject);
	}

	void Update()
    {
        m_lifetime -= Time.deltaTime;
        if(m_lifetime <= 0.0f)
		{
            Destroy(gameObject);
		}
    }
}
