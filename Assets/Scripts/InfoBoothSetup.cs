using Photon.Pun;
using UnityEngine;

public class InfoBoothSetup : MonoBehaviourPunCallbacks
{
    private Camera _camera;
    private EventInfoManager _eventInfoManager;

    void Start()
    {
        _eventInfoManager = GameObject.Find(GameConstants.K_EventInfoManager).GetComponent<EventInfoManager>();
    }

    // This is called when the user first enters a booth area
    void OnTriggerEnter(Collider other)
    {
        if (_camera == null)
        {
            _camera = GameObject.Find(GameConstants.K_Camera).GetComponent<Camera>();
            GetComponentInChildren<Canvas>().worldCamera = _camera;
        }

        if (other.name == GameConstants.K_MyUser)
        {
            EventInfoManager.IsUserInInfoBooth = true;
        }
    }

    // This is called when the user leaves a booth area
    void OnTriggerExit(Collider other)
    {
        if (other.name == GameConstants.K_MyUser)
        {
            EventInfoManager.IsUserInInfoBooth = false;
        }
    }

    public void OpenEventInfo()
    {
        _eventInfoManager.OpenEventInfo();
    }
}
