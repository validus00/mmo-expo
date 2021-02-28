using Photon.Pun;
using UnityEngine;

/*
 * InfoBoothSetup class is for implementing business logic surrounding event information
 */
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

    // For handling event info panel opening logic
    public void OpenEventInfo()
    {
        _eventInfoManager.OpenEventInfo();
    }
}
