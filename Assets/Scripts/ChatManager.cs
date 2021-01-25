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
    private IPhotonChatHandler __photonChatHandler;
    // For handling user inputs
    private IPlayerInputHandler __playerInputHandler;
    // max number of channels
    private const int __maxMessages = 100;
    // Showcase-wide channel
    readonly private string __announcementChannel = "Announcements";
    // Hall specific channel
    private string __hallChannel = "Main Hall";
    // Booth specific channel
    private string __boothChannel = string.Empty;

    [SerializeField]
    private GameObject __chatPanel, __textObject;

    [SerializeField]
    private InputField __channelBox;

    [SerializeField]
    private InputField __chatBox;

    [SerializeField]
    private Color __playerMessage, __info;

    // messageList keeps tracks of recent messages
    [SerializeField]
    readonly private List<Message> __messageList = new List<Message>();

    // For initializing playerInputHandler
    public void InitializePlayerInputHandler(IPlayerInputHandler playerInputHandler) {
        __playerInputHandler = playerInputHandler;
    }

    // For initializing playerInputHandler
    public void InitializePhotonChatHandler(IPhotonChatHandler photonChatHandler) {
        __photonChatHandler = photonChatHandler;
    }

    // For initializing chatPanel
    public void InitializeChatPanel(GameObject gameObject) {
        __chatPanel = gameObject;
    }

    // For initializing textObject
    public void InitializeTextObject(GameObject gameObject) {
        __textObject = gameObject;
    }

    // For initializing channelBox
    public void InitializeChannelBox(InputField inputField) {
        __channelBox = inputField;
    }

    // For initializing chatBox
    public void InitializeChatBox(InputField inputField) {
        __chatBox = inputField;
    }

    // For initializing playerMessage color
    public void InitializePlayerMessageColor(Color color) {
        __playerMessage = color;
    }

    // For initializing info color
    public void InitializeInfoColor(Color color) {
        __info = color;
    }

    // Returns current messages
    public List<Message> GetMessages() {
        return __messageList;
    }

    // Start is called before the first frame update
    void Start() {
        if (__photonChatHandler == null) {
            __photonChatHandler = new PhotonChatHandler();
            __photonChatHandler.Initialize();
        }
        if (__playerInputHandler == null) {
            __playerInputHandler = new PlayerInputHandler();
        }

        __photonChatHandler.InitializeChannels(new string[] { __announcementChannel, __hallChannel });
    }

    // Update is called once per frame
    void Update() {
        // Maintain service connection to Photon
        __photonChatHandler.MaintainService();

        // Get new messages from chat client
        List<Message> messages = __photonChatHandler.GetNewMessages();
        foreach (Message message in messages) {
            __SendMessageToChat(message);
        }

        string channelName = __channelBox.text;
        string messageText = __chatBox.text;
        // Enter key either sends a message or activates the chat input field
        if (__playerInputHandler.GetReturnKey()) {
            if (!string.IsNullOrEmpty(messageText)) {
                if (string.IsNullOrWhiteSpace(channelName)) {
                    // channel name not given
                    __SendMessageToChat("No channel or username specified.", Message.MessageType.info);
                } else if (!__photonChatHandler.IsConnected()) {
                    // Chat client not connected
                    __SendMessageToChat("Not connected to chat yet.", Message.MessageType.info);
                } else if (__CheckChannelBox(channelName)) {
                    // channel name not correct or does not exist
                    string warning = string.Format("You are not in \"{0}\" channel. Cannot send message.", channelName);
                    __SendMessageToChat(warning, Message.MessageType.info);
                } else {
                    // Send message
                    __photonChatHandler.SendChannelMessage(channelName, messageText);
                    __chatBox.text = string.Empty;
                }
            } else {
                __chatBox.ActivateInputField();
            }
        }
    }

    private bool __CheckChannelBox(string channelName) {
        return channelName != __announcementChannel && channelName != __hallChannel && channelName != __boothChannel;
    }

    public void UpdateChannel(string channelName, ChannelType channelType) {
        switch (channelType) {
            case ChannelType.hallChannel:
                __hallChannel = channelName;
                break;
            case ChannelType.boothChannel:
                __boothChannel = channelName;
                break;
        }

        if (!__photonChatHandler.IsConnected()) {
            if (!string.IsNullOrEmpty(__boothChannel)) {
                __photonChatHandler.InitializeChannels(new string[] { __announcementChannel, __hallChannel, __boothChannel });
            } else {
                __photonChatHandler.InitializeChannels(new string[] { __announcementChannel, __hallChannel });
            }
        }
    }

    // For leaving a specific channel
    public void LeaveChannel(string channelName) {
        if (!string.IsNullOrWhiteSpace(channelName)) {
            __photonChatHandler.LeaveChannel(channelName);
        }
    }

    // For entering a specific channel
    public void EnterChannel(string channelName) {
        if (!string.IsNullOrWhiteSpace(channelName)) {
            __photonChatHandler.EnterChannel(channelName);
        }
    }

    public string GetChannelName(ChannelType channelType) {
        string channelName = __boothChannel;
        switch (channelType) {
            case ChannelType.announcementChannel:
                channelName = __announcementChannel;
                break;
            case ChannelType.hallChannel:
                channelName = __hallChannel;
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
        GameObject newText = Instantiate(__textObject, __chatPanel.transform);
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

        GameObject newText = Instantiate(__textObject, __chatPanel.transform);
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
        Color color = __info;

        switch (messageType) {
            case Message.MessageType.playerMessage:
                color = __playerMessage;
                break;
        }

        return color;
    }
}
