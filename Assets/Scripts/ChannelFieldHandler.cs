using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/*
 * ChannelFieldHandler class is for implementing the feature to allow users to click on channel/player names to
 * instantiate channel name input field
 */
public class ChannelFieldHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public TextMeshProUGUI Text;
    public ChatManager ChatManagerObject;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(Text, Input.mousePosition, null);
            if (linkIndex > -1)
            {
                TMP_LinkInfo linkInfo = Text.textInfo.linkInfo[linkIndex];
                string linkId = linkInfo.GetLinkID();

                ChatManagerObject.UpdateChannelInputField(linkId);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("test");
    }
}
