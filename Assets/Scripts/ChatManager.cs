using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * ChatManager class is for implementing chat and displaying messages in the chat panel
 */
public class ChatManager : MonoBehaviour {
    // For handling different channel types
    public enum ChannelType {
        announcementChannel,
        hallChannel,
        boothChannel
    }

    // For chat client handling
    public IPhotonChatHandler photonChatHandler;
    // For handling user inputs
    public IPlayerInputHandler playerInputHandler;
    // max number of channels
    private const int __maxMessages = 100;
    // Showcase-wide channel
    readonly private string __announcementChannelName = GameConstants.k_AnnouncementChannelName;
    // Hall specific channel
    private string __hallChannelName = GameConstants.k_HallChannelName;
    // Booth specific channel
    private string __boothChannelName = string.Empty;
    // Chat panel
    public GameObject chatPanel;
    // Text objects to populate chat panel
    public GameObject textObject;
    // Channel name input field
    public InputField channelBox;
    // message input field
    public InputField chatBox;
    // Color for general chat messages
    public Color playerMessageColor;
    // Color for informational messages
    public Color infoColor;

    // messageList keeps tracks of recent messages
    [SerializeField]
    readonly private List<Message> __messageList = new List<Message>();

    // Returns current messages
    public List<Message> GetMessages() {
        return __messageList;
    }

    // Start is called before the first frame update
    void Start() {
        if (photonChatHandler == null) {
            photonChatHandler = new PhotonChatHandler();
        }
        if (playerInputHandler == null) {
            playerInputHandler = new PlayerInputHandler();
        }

        photonChatHandler.InitializeChannels(new string[] { __announcementChannelName, __hallChannelName });
    }

    // Update is called once per frame
    void Update() {
        // Maintain service connection to Photon
        photonChatHandler.MaintainService();

        // Get new messages from chat client
        List<Message> messages = photonChatHandler.GetNewMessages();
        foreach (Message message in messages) {
            __SendMessageToChat(message);
        }

        string channelName = channelBox.text;
        string messageText = chatBox.text;
        // Enter key either sends a message or activates the chat input field
        if (playerInputHandler.GetReturnKey()) {
            if (!string.IsNullOrEmpty(messageText)) {
                if (string.IsNullOrWhiteSpace(channelName)) {
                    // channel name not given
                    __SendMessageToChat("No channel or username specified.", Message.MessageType.info);
                } else if (!photonChatHandler.IsConnected()) {
                    // Chat client not connected
                    __SendMessageToChat("Not connected to chat yet.", Message.MessageType.info);
                } else if (__CheckChannelBox(channelName)) {
                    // channel name not correct or does not exist
                    string warning = string.Format("You are not in \"{0}\" channel. Cannot send message.", channelName);
                    __SendMessageToChat(warning, Message.MessageType.info);
                } else {
                    // Send message
                    photonChatHandler.SendChannelMessage(channelName, messageText);
                    chatBox.text = string.Empty;
                }
            } else {
                chatBox.ActivateInputField();
            }
        }
    }

    private bool __CheckChannelBox(string channelName) {
        return channelName != __announcementChannelName && channelName != __hallChannelName && channelName != __boothChannelName;
    }

    public void UpdateChannel(string channelName, ChannelType channelType) {
        switch (channelType) {
            case ChannelType.hallChannel:
                __hallChannelName = channelName;
                break;
            case ChannelType.boothChannel:
                __boothChannelName = channelName;
                break;
            case ChannelType.announcementChannel:
                // Changing announcements channel is not allowed
                return;
        }

        if (!photonChatHandler.IsConnected()) {
            if (!string.IsNullOrEmpty(__boothChannelName)) {
                photonChatHandler.InitializeChannels(new string[] { __announcementChannelName, __hallChannelName, __boothChannelName });
            } else {
                photonChatHandler.InitializeChannels(new string[] { __announcementChannelName, __hallChannelName });
            }
        }
    }

    // For leaving a specific channel
    public void LeaveChannel(string channelName) {
        if (!string.IsNullOrWhiteSpace(channelName)) {
            photonChatHandler.LeaveChannel(channelName);
        }
    }

    // For entering a specific channel
    public void EnterChannel(string channelName) {
        if (!string.IsNullOrWhiteSpace(channelName)) {
            photonChatHandler.EnterChannel(channelName);
        }
    }

    public string GetChannelName(ChannelType channelType) {
        string channelName = __boothChannelName;
        switch (channelType) {
            case ChannelType.announcementChannel:
                channelName = __announcementChannelName;
                break;
            case ChannelType.hallChannel:
                channelName = __hallChannelName;
                break;
        }

        return channelName;
    }

    // This method is for displaying received messages in chat panel
    private void __SendMessageToChat(string text, Message.MessageType messageType) {
        // Limit number of messages
        __LimitNumberOfMessages();

        // Create new Message object and add to list of messages
        Message message = new Message();
        GameObject newText = Instantiate(textObject, chatPanel.transform);
        message.messageText = text;
        message.messageType = messageType;
        message.textObject = newText.GetComponent<Text>();
        message.textObject.text = text;
        message.textObject.color = __MessageTypeColor(messageType);

        __messageList.Add(message);
    }

    // This method is for displaying received messages in chat panel
    private void __SendMessageToChat(Message message) {
        // Limit number of messages
        __LimitNumberOfMessages();

        GameObject newText = Instantiate(textObject, chatPanel.transform);
        message.textObject = newText.GetComponent<Text>();
        message.textObject.text = message.messageText;
        message.textObject.color = __MessageTypeColor(message.messageType);

        __messageList.Add(message);
    }

    private void __LimitNumberOfMessages() {
        // Keep only 99 most recent messages before adding a new message to list
        if (__messageList.Count >= __maxMessages) {
            Destroy(__messageList[0].textObject.gameObject);
            __messageList.Remove(__messageList[0]);
        }
    }

    // This method is for determining which color the message type is
    private Color __MessageTypeColor(Message.MessageType messageType) {
        // Default color
        Color color = infoColor;

        switch (messageType) {
            case Message.MessageType.playerMessage:
                color = playerMessageColor;
                break;
        }

        return color;
    }
}
