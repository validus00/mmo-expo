using Photon.Pun;
using UnityEngine;

public class EventInfoManager : MonoBehaviour, IEventInfoManager
{
    private PanelManager _panelManager;
    private PhotonView _photonView;
    private string _eventInfoUrl;
    private string _scheduleUrl;
    private string _zoomUrl;
    private string _eventInfoOwner;
    public static bool IsUserInInfoBooth = false;

    void Start()
    {
        _panelManager = GameObject.Find(GameConstants.K_PanelManager).GetComponent<PanelManager>();
        _photonView = GetComponent<PhotonView>();
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
            _photonView.RPC("SyncEventInfo", RpcTarget.AllBuffered, eventInfoUrl, scheduleUrl, zoomUrl,
                PhotonNetwork.NickName);
            return true;
        }
        return false;
    }

    // RPC for setting up event info panel data
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
        _photonView.RPC("ClearEventInfo", RpcTarget.AllBuffered);
    }

    // PRC for resetting event info panel data
    [PunRPC]
    void ClearEventInfo()
    {
        _eventInfoUrl = string.Empty;
        _scheduleUrl = string.Empty;
        _zoomUrl = string.Empty;
        _eventInfoOwner = string.Empty;
    }
}
