using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour, IPooledObject
{
    #region IPooledObject
    public void OnSpawned() { }
    public void OnDestroy() 
    {
        EventSystem.SendEvent(new OnPortalDesstroyedEvent() { position = transform.position });
    }
    public ESpawnItemType GetObjectType() { return ESpawnItemType.Portal; }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
