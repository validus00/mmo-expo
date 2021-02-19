using Photon.Pun;
using UnityEngine;

public class EventInfoManager : MonoBehaviourPunCallbacks, IEventInfoManager
{

    private PanelManager __panelManager;
    private string __eventInfoUrl;
    private string __scheduleUrl;
    private string __zoomUrl;
    private string __eventInfoOwner;
    public static bool isUserInInfoBooth = false;

    void Start()
    {
        __panelManager = GameObject.Find(GameConstants.k_PanelManager).GetComponent<PanelManager>();
    }

    public string EventInfoOwner
    {
        get { return __eventInfoOwner; }
    }

    public void OpenEventInfo()
    {
        if (isUserInInfoBooth)
        {
            if (string.IsNullOrEmpty(__eventInfoUrl))
            {
                __panelManager.OpenEventInfoFormPanel(this);
            }
            else
            {
                __panelManager.OpenEventInfoPanel(this, PhotonNetwork.NickName == __eventInfoOwner, __eventInfoUrl,
                    __scheduleUrl, __zoomUrl);
            }
        }
    }

    public bool SetUpEventInfo(string eventInfoUrl, string scheduleUrl, string zoomUrl)
    {
        if (string.IsNullOrEmpty(__eventInfoUrl))
        {
            photonView.RPC("SyncEventInfo", RpcTarget.AllBuffered, eventInfoUrl, scheduleUrl, zoomUrl,
                PhotonNetwork.NickName);
            return true;
        }
        return false;
    }

    [PunRPC]
    void SyncEventInfo(string eventInfoUrl, string scheduleUrl, string zoomUrl, string eventInfoOwner)
    {
        __eventInfoUrl = eventInfoUrl;
        __scheduleUrl = scheduleUrl;
        __zoomUrl = zoomUrl;
        __eventInfoOwner = eventInfoOwner;
    }

    public void ResetEventInfo()
    {
        photonView.RPC("ClearEventInfo", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ClearEventInfo()
    {
        __eventInfoUrl = string.Empty;
        __scheduleUrl = string.Empty;
        __zoomUrl = string.Empty;
        __eventInfoOwner = string.Empty;
    }
}
