using UnityEngine;

// Event Example
//class PauseMenuActivated
//{
//}
//EventSystem.RegisterListener<PauseMenuActivated>(OnPauseRequested);
// Event calls
//private void OnPauseRequested(object incomingEvent) {}
//EventSystem.SendEvent(new MobDied() { });

class OnPortalDesstroyedEvent
{
    public Vector3 position = Vector3.zero;
}

class OnPortalBeginTrigger
{
    public PortalTrigger portalTrigger = null;
}
class OnPortalEndTrigger
{
}

class MobDied
{
}