/*
 * Message class is for creating objects that hold text objects
 */
using UnityEngine.UI;

[System.Serializable]
public class Message
{
    // For displaying text in chat in different colors depending on type of message 
    public enum MessageType
    {
        playerMessage,
        info,
        privateMessage
    }

    public Text TextObject;
    public string MessageText;
    public MessageType MsgType;
}
