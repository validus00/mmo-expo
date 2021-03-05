using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

/*
 * BoothSetup class is for implementing business logic surrounding entering and leaving booths
 */
public class BoothSetup : MonoBehaviour
{
    public TextMeshProUGUI BoothText;
    public GameObject PosterBoard;
    private PanelManager _panelManager;
    private ChatManager _chatManager;
    private Camera _camera;
    private PhotonView _photonView;
    private bool _isUserInBooth = false;
    private string _boothOwner;
    private string _projectName;
    private string _teamName;
    private string _projectDescription;
    private string _projectUrl;

    void Start()
    {
        _panelManager = GameObject.Find(GameConstants.K_PanelManager).GetComponent<PanelManager>();
        _chatManager = GameObject.Find(GameConstants.K_ExpoEventManager).GetComponent<ChatManager>();
        _photonView = GetComponent<PhotonView>();
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
            _isUserInBooth = true;
            if (!string.IsNullOrWhiteSpace(_projectName))
            {
                JoinChannel();
            }
        }
    }

    // This is called when the user leaves a booth area
    void OnTriggerExit(Collider other)
    {
        if (other.name == GameConstants.K_MyUser)
        {
            LeaveChannel();
            _isUserInBooth = false;
        }
    }

    private void JoinChannel()
    {
        _chatManager.UpdateChannel(_projectName, ChatManager.ChannelType.boothChannel);
        _chatManager.EnterChannel(_projectName);
    }

    private void LeaveChannel()
    {
        _chatManager.UpdateChannel(string.Empty, ChatManager.ChannelType.boothChannel);
        _chatManager.LeaveChannel(_projectName);
    }

    // For handling booth panel opening logic
    public void OpenBoothPanel()
    {
        if (_isUserInBooth)
        {
            if (string.IsNullOrEmpty(_projectName))
            {
                _panelManager.OpenBoothFormPanel(this);
            }
            else
            {
                _panelManager.OpenBoothInfoPanel(this, PhotonNetwork.NickName == _boothOwner, _projectName,
                    _teamName, _projectDescription, _projectUrl);
            }
        }
    }

    // For handling booth setup logic
    public bool SetUpBooth(string projectName, string teamName, string projectDescription, string projectUrl,
        string posterUrl)
    {
        if (string.IsNullOrEmpty(_projectName))
        {
            _photonView.RPC("SyncBooth", RpcTarget.AllBuffered, PhotonNetwork.NickName, projectName, teamName,
                projectDescription, projectUrl, posterUrl);
            return true;
        }
        return false;
    }

    private IEnumerator SetPosterTexture(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Material posterMaterial = new Material(Shader.Find("Legacy Shaders/Diffuse"));
            posterMaterial.mainTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            PosterBoard.GetComponent<MeshRenderer>().material = posterMaterial;
        }
    }

    // RPC for updating booth info
    [PunRPC]
    void SyncBooth(string boothOwner, string projectName, string teamName, string projectDescription,
        string projectUrl, string posterUrl)
    {
        BoothText.text = projectName;
        AssignBoothValues(boothOwner, projectName, teamName, projectDescription, projectUrl);
        if (_isUserInBooth)
        {
            JoinChannel();
        }
        if (!string.IsNullOrWhiteSpace(posterUrl))
        {
            StartCoroutine(SetPosterTexture(posterUrl));
        }
    }

    private void AssignBoothValues(string boothOwner, string projectName, string teamName, string projectDescription,
        string url)
    {
        _boothOwner = boothOwner;
        _projectName = projectName;
        _teamName = teamName;
        _projectDescription = projectDescription;
        _projectUrl = url;
    }

    // For handling booth info reset logic
    public void ResetBooth()
    {
        _photonView.RPC("ClearBooth", RpcTarget.AllBuffered);
    }

    // PRC for resetting booth
    [PunRPC]
    void ClearBooth()
    {
        if (_isUserInBooth)
        {
            LeaveChannel();
        }
        BoothText.text = string.Empty;
        AssignBoothValues(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        StartCoroutine(SetPosterTexture("https://i.imgur.com/EFfwKyC.png"));
    }
}
