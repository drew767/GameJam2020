using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    List<Pickup> m_pickUp = new List<Pickup>();

    // Start is called before the first frame update
    void Start()
    {
        // GET ALL PICKUPS
        for (int i = 0; i < transform.childCount; i++)
        {
            m_pickUp.Add(transform.GetChild(i).gameObject.GetComponent<Pickup>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_pickUp.Count; i++)
        {
            if (m_pickUp[i].TimeToActivate())
            {
                m_pickUp[i].gameObject.SetActive(true);
            }
        }
    }
}
