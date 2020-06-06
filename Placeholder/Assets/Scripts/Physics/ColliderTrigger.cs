using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTrigger : MonoBehaviour
{
    public bool IsTriggered { get; private set; }

    private void OnTriggerStay(Collider other)
    {
        IsTriggered = true;
    }

    private void OnTriggerExit(Collider other)
    {
        IsTriggered = false;
    }

}
