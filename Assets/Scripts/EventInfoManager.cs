using Photon.Pun;
using UnityEngine;

public class EventInfoManager : MonoBehaviourPunCallbacks, IEventInfoManager
{
    private PanelManager _panelManager;
    private string _eventInfoUrl;
    private string _scheduleUrl;
    private string _zoomUrl;
    private string _eventInfoOwner;
    public static bool IsUserInInfoBooth = false;

    void Start()
    {
        _panelManager = GameObject.Find(GameConstants.K_PanelManager).GetComponent<PanelManager>();
    }

    public string EventInfoOwner
    {
        get { return _eventInfoOwner; }
    }

    public void OpenEventInfo()
    {
        if (IsUserInInfoBooth)
        {
            if (string.IsNullOrEmpty(_eventInfoUrl))
            {
                _panelManager.OpenEventInfoFormPanel(this);
            }
            else
            {
                _panelManager.OpenEventInfoPanel(this, PhotonNetwork.NickName == _eventInfoOwner, _eventInfoUrl,
                    _scheduleUrl, _zoomUrl);
            }
        }
    }

    public bool SetUpEventInfo(string eventInfoUrl, string scheduleUrl, string zoomUrl)
    {
        if (string.IsNullOrEmpty(_eventInfoUrl))
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
        _eventInfoUrl = eventInfoUrl;
        _scheduleUrl = scheduleUrl;
        _zoomUrl = zoomUrl;
        _eventInfoOwner = eventInfoOwner;
    }

    public void ResetEventInfo()
    {
        photonView.RPC("ClearEventInfo", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ClearEventInfo()
    {
        _eventInfoUrl = string.Empty;
        _scheduleUrl = string.Empty;
        _zoomUrl = string.Empty;
        _eventInfoOwner = string.Empty;
    }
}
