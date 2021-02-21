using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChannelFieldHandler : MonoBehaviour, IPointerClickHandler
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
}
