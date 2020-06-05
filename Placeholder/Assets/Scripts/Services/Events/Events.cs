using UnityEngine;

// Event Example
//class PauseMenuActivated
//{
//}
//EventSystem.RegisterListener<PauseMenuActivated>(OnPauseRequested);
// Event calls
//private void OnPauseRequested(object incomingEvent) {}
//EventSystem.SendEvent(new PauseMenuActivated() { });

class OnPortalDesstroyedEvent
{
    public Vector3 position = Vector3.zero;
}