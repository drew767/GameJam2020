using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootComponent : MonoBehaviour
{
    public GameObject loot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnStateChanged(EnemyController enemy)
    {
        if (enemy.State == eState.DEAD)
        {
            GameObject instFoam = Instantiate(foam, transform.position, Quaternion.identity);
        }
    }
}
