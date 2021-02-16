using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/*
 * ChatManager class is for implementing chat and displaying messages in the chat panel
 */
public class ChatManager : MonoBehaviour {
    // For handling different channel types
    public enum ChannelType {
        announcementChannel,
        hallChannel,
        boothChannel,
        general
    }
    // For storing channel names
    private readonly Dictionary<ChannelType, string> __channelNames = new Dictionary<ChannelType, string>() {
        { ChannelType.announcementChannel, GameConstants.k_AnnouncementChannelName },
        { ChannelType.general, GameConstants.k_GeneralChannelName },
        { ChannelType.hallChannel, GameConstants.k_HallChannelName },
        { ChannelType.boothChannel, string.Empty }
    };
    // For chat client handling
    public IPhotonChatHandler photonChatHandler;
    // For handling user inputs
    public IPlayerInputHandler playerInputHandler;
    // max number of channels
    private const int __maxMessages = 100;
    // Chat panel
    public GameObject chatPanel;
    // Text objects to populate chat panel
    public GameObject textObject;
    // Event info manager object to access event info manager
    public GameObject eventInfoManagerObject;
    // Event info manager to access event info owner
    public IEventInfoManager eventInfoManager;
    // Channel name input field
    public InputField channelBox;
    // message input field
    public InputField chatBox;
    // Color for general chat messages
    public Color playerMessageColor;
    // Color for informational messages
    public Color infoColor;
    // Color for private messages
    public Color privateMessageColor;
    // For keeping track whether connect to service is called
    private bool __connectToServiceIsCalled;

    // messageList keeps tracks of recent messages
    [SerializeField]
    readonly private List<Message> __messageList = new List<Message>();

    // Returns current messages
    public List<Message> GetMessages() {
        return __messageList;
    }

    // Start is called before the first frame update
    void Start() {
        __connectToServiceIsCalled = false;

        if (eventInfoManager == null) {
            eventInfoManager = eventInfoManagerObject.GetComponent<EventInfoManager>();
        }

        if (photonChatHandler == null) {
            photonChatHandler = new PhotonChatHandler();
        }
        if (playerInputHandler == null) {
            playerInputHandler = new PlayerInputHandler();
        }

        photonChatHandler.InitializeChannelNames(new string[] {
            __channelNames[ChannelType.announcementChannel],
            __channelNames[ChannelType.general],
            __channelNames[ChannelType.hallChannel] });
    }

    // Update is called once per frame
    void Update() {
        // Maintain service connection to Photon and is called during every update
        photonChatHandler.MaintainService();

        if (photonChatHandler.IsConnected()) {
            // Get new messages from chat client
            List<Message> messages = photonChatHandler.GetNewMessages();
            foreach (Message message in messages) {
                __SendMessageToChat(message);
            }
            // process chat input fields
            __processChatInput();
        } else if (ExpoEventManager.isNameUpdated && !__connectToServiceIsCalled) {
            photonChatHandler.ConnectToService();
            __connectToServiceIsCalled = true;
        }
    }

    private void __processChatInput() {
        // Enter key either sends a message or activates the chat input field
        if (playerInputHandler.GetReturnKey()) {
            string channelName = channelBox.text;
            string messageText = chatBox.text;

            if (!string.IsNullOrEmpty(messageText)) {
                if (string.IsNullOrWhiteSpace(channelName)) {
                    // channel name not given
                    __SendMessageToChat("No channel or username specified.", Message.MessageType.info);
                } else if (__CheckChannelName(channelName)) {
                    // If channel name does not exist in the channel list, assume private message attempt

                    // Check to see if the receipient is a valid user
                    bool isValidUser = false;
                    // temporary username from channel name + identifier (which is room name)
                    string username = photonChatHandler.GetUsername(channelName);
                    foreach (Player player in PhotonNetwork.PlayerList) {
                        if (username.Equals(player.NickName)) {
                            isValidUser = true;
                            break;
                        }
                    }
                    if (isValidUser) {
                        // Do not allow user to send private message to themselves
                        if (username == PhotonNetwork.NickName) {
                            __SendMessageToChat("Cannot send message to yourself.", Message.MessageType.info);
                            channelBox.text = string.Empty;
                        } else {
                            // Send the private message
                            photonChatHandler.SendPrivateMessage(username, messageText);
                        }
                    } else {
                        // Notify user that the recipient does not exist
                        __SendMessageToChat($"\"{channelName}\" does not exist in the room.",
                            Message.MessageType.info);
                        channelBox.text = string.Empty;
                    }
                    chatBox.text = string.Empty;
                } else if (channelName.Equals(__channelNames[ChannelType.announcementChannel]) &&
                    !photonChatHandler.Username.Equals(eventInfoManager.EventInfoOwner)) {
                    // Notify user that they do not have access to announcement channel
                    __SendMessageToChat($"Only event admins have access to \"{channelName}\".", Message.MessageType.info);
                    channelBox.text = string.Empty;
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

    private bool __CheckChannelName(string channelName) {
        return channelName != __channelNames[ChannelType.announcementChannel] &&
            channelName != __channelNames[ChannelType.hallChannel] &&
            channelName != __channelNames[ChannelType.boothChannel] &&
            channelName != __channelNames[ChannelType.general];
    }

    public void UpdateChannel(string channelName, ChannelType channelType) {
        if (channelType == ChannelType.announcementChannel) {
            return;
        }

        __channelNames[channelType] = channelName;

        if (!photonChatHandler.IsConnected()) {
            if (!string.IsNullOrEmpty(__channelNames[ChannelType.boothChannel])) {
                photonChatHandler.InitializeChannelNames(new string[] {
                    __channelNames[ChannelType.announcementChannel], __channelNames[ChannelType.hallChannel],
                    __channelNames[ChannelType.general], __channelNames[ChannelType.boothChannel] });
            } else {
                photonChatHandler.InitializeChannelNames(new string[] {
                    __channelNames[ChannelType.announcementChannel], __channelNames[ChannelType.hallChannel],
                    __channelNames[ChannelType.general],
                });
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
        return __channelNames[channelType];
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
            case Message.MessageType.privateMessage:
                color = privateMessageColor;
                break;
        }

        return color;
    }
}
