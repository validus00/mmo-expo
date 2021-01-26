using UnityEngine;
using UnityEngine.UI;

public class BoothManager : MonoBehaviour {
    public GameObject boothInfoPanel;
    public Text teamNameText;
    public Text projectNameText;
    public Text descriptionText;
    public Text urlText;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void OpenURL() {
        if (urlText != null && !string.IsNullOrEmpty(urlText.text)) {
            Application.ExternalEval($"window.open(\"{urlText.text}\",\"_blank\")");
        }
    }

    public void CloseBoothInfoPanel() {
        boothInfoPanel.SetActive(false);
    }

    public void OpenBoothInfoPanel(string projectName, string teamName, string description, string url) {
        boothInfoPanel.SetActive(true);
        teamNameText.text = teamName;
        projectNameText.text = projectName;
        descriptionText.text = description;
        urlText.text = url;
    }

    public bool IsBoothInfoPanelActive() {
        return boothInfoPanel.activeSelf;
    }
}
