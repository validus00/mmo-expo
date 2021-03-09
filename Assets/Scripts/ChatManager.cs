using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * ChatManager class is for implementing chat and displaying messages in the chat panel
 */
public class ChatManager : MonoBehaviour
{
    // For handling different channel types
    public enum ChannelType
    {
        announcementChannel,
        hallChannel,
        boothChannel,
        general
    }
    // For storing channel names
    private readonly Dictionary<ChannelType, string> _channelNames = new Dictionary<ChannelType, string>() {
        { ChannelType.announcementChannel, GameConstants.K_AnnouncementChannelName },
        { ChannelType.general, GameConstants.K_GeneralChannelName },
        { ChannelType.hallChannel, GameConstants.K_MainHallChannelName },
        { ChannelType.boothChannel, string.Empty }
    };
    // For chat client handling
    public IPhotonChatHandler PhotonChatHandler;
    // For handling user inputs
    public IPlayerInputHandler PlayerInputHandler;
    // max number of channels
    private const int K_MaxMessages = 100;
    // Chat panel
    public GameObject ChatPanel;
    // TMP text object to populate chat panel
    public GameObject TextObject;
    // Event info manager object to access event info manager
    public GameObject EventInfoManagerObject;
    // Secret area
    public GameObject SecretArea;
    // Event info manager to access event info owner
    public IEventInfoManager EventInfoManager;
    // Channel name input field
    public InputField ChannelBox;
    // message input field
    public InputField ChatBox;
    // Color for general chat messages
    public Color PlayerMessageColor;
    // Color for informational messages
    public Color InfoColor;
    // Color for private messages
    public Color PrivateMessageColor;
    // For keeping track whether connect to service is called
    private bool _connectToServiceIsCalled;
    // messageList keeps tracks of recent messages
    private readonly List<Message> _messageList = new List<Message>();
    private MovementController _movementController;

    // Returns current messages
    public List<Message> GetMessages()
    {
        return _messageList;
    }

    // Start is called before the first frame update
    void Start()
    {
        _connectToServiceIsCalled = false;

        if (EventInfoManager == null)
        {
            EventInfoManager = EventInfoManagerObject.GetComponent<EventInfoManager>();
        }

        if (PhotonChatHandler == null)
        {
            PhotonChatHandler = new PhotonChatHandler();
        }
        if (PlayerInputHandler == null)
        {
            PlayerInputHandler = new PlayerInputHandler();
        }

        PhotonChatHandler.InitializeChannelNames(new string[] {
            _channelNames[ChannelType.announcementChannel],
            _channelNames[ChannelType.general],
            _channelNames[ChannelType.hallChannel] });
    }

    // Update is called once per frame
    void Update()
    {
        // Maintain service connection to Photon and is called during every update
        PhotonChatHandler.MaintainService();

        if (PhotonChatHandler.IsConnected())
        {
            // Get new messages from chat client
            List<Message> messages = PhotonChatHandler.GetNewMessages();
            foreach (Message message in messages)
            {
                SendMessageToChat(message);
            }
            // process chat input fields
            ProcessChatInput();
        }
        else if (ExpoEventManager.IsNameUpdated && !_connectToServiceIsCalled)
        {
            PhotonChatHandler.ConnectToService();
            _connectToServiceIsCalled = true;
        }
    }

    // For updating channel input field
    public void UpdateChannelInputField(string channelName)
    {
        ChannelBox.text = channelName;
    }

    private void ProcessChatInput()
    {
        // Enter key either sends a message or activates the chat input field
        if (PlayerInputHandler.GetReturnKey())
        {
            string channelName = ChannelBox.text;
            string messageText = ChatBox.text;

            if (!string.IsNullOrEmpty(messageText))
            {
                if (messageText.Equals(GameConstants.K_EasterEggSecretPhrase))
                {
                    TeleportUserToSecretArea();
                    ClearChatBox();
                }
                else if (messageText.Equals(GameConstants.K_GodModeSecretPhrase))
                {
                    ToggleGodMode();
                    ClearChatBox();
                }
                else if (string.IsNullOrWhiteSpace(channelName))
                {
                    // channel name not given
                    SendMessageToChat("No channel or username specified.", Message.MessageType.info);
                }
                else if (PhotonChatHandler.UserEnteredName.Equals(channelName))
                {
                    // Do not allow user to send private message to themselves
                    SendMessageToChat("Cannot send message to yourself.", Message.MessageType.info);
                    UpdateChannelInputField(string.Empty);
                }
                else if (PhotonChatHandler.IsValidUsername(channelName))
                {
                    // Send the private message
                    PhotonChatHandler.SendPrivateMessage(channelName, messageText);
                }
                else if (IsChannelName(channelName))
                {
                    if (channelName.Equals(_channelNames[ChannelType.announcementChannel]) &&
                        !PhotonChatHandler.Username.Equals(EventInfoManager.EventInfoOwner))
                    {
                        // Notify user that they do not have access to announcement channel
                        SendMessageToChat($"Only event admins have access to \"{channelName}\".", Message.MessageType.info);
                        UpdateChannelInputField(string.Empty);
                    }
                    else
                    {
                        // Send message
                        PhotonChatHandler.SendChannelMessage(channelName, messageText);
                        ClearChatBox();
                    }
                }
                else
                {
                    // Notify user that the recipient does not exist or user not in channel
                    SendMessageToChat($"\"{channelName}\" is not in your channel list or player list.",
                        Message.MessageType.info);
                    UpdateChannelInputField(string.Empty);
                    ClearChatBox();
                }
            }
            else
            {
                ChatBox.ActivateInputField();
            }
        }
    }

    private void ClearChatBox()
    {
        ChatBox.text = string.Empty;
    }

    private void ToggleGodMode()
    {
        if (_movementController == null)
        {
            _movementController = GameObject.Find(GameConstants.K_MyUser).GetComponent<MovementController>();
        }
        _movementController.ToggleGodMode();
    }

    private void TeleportUserToSecretArea()
    {
        Vector3 secretAreaPosition = SecretArea.transform.position;
        int randomPoint = Random.Range(-20, 20);
        Vector3 destination = new Vector3(secretAreaPosition.x + randomPoint, secretAreaPosition.y, secretAreaPosition.z + randomPoint);

        GameObject user = GameObject.Find(GameConstants.K_MyUser);
        TeleportManager.Teleport(user, destination);

        LeaveChannel(_channelNames[ChannelType.hallChannel]);
        UpdateChannel(GameConstants.K_SecretHallChannelName, ChannelType.hallChannel);
        EnterChannel(GameConstants.K_SecretHallChannelName);
    }

    private bool IsChannelName(string channelName)
    {
        return channelName == _channelNames[ChannelType.announcementChannel] ||
            channelName == _channelNames[ChannelType.hallChannel] ||
            channelName == _channelNames[ChannelType.boothChannel] ||
            channelName == _channelNames[ChannelType.general];
    }

    // For updating channel dictionary
    public void UpdateChannel(string channelName, ChannelType channelType)
    {
        if (channelType == ChannelType.announcementChannel)
        {
            return;
        }

        _channelNames[channelType] = channelName;

        if (!PhotonChatHandler.IsConnected())
        {
            if (!string.IsNullOrEmpty(_channelNames[ChannelType.boothChannel]))
            {
                PhotonChatHandler.InitializeChannelNames(new string[] {
                    _channelNames[ChannelType.announcementChannel], _channelNames[ChannelType.hallChannel],
                    _channelNames[ChannelType.general], _channelNames[ChannelType.boothChannel] });
            }
            else
            {
                PhotonChatHandler.InitializeChannelNames(new string[] {
                    _channelNames[ChannelType.announcementChannel], _channelNames[ChannelType.hallChannel],
                    _channelNames[ChannelType.general],
                });
            }
        }
    }

    // For leaving a specific channel
    public void LeaveChannel(string channelName)
    {
        if (!string.IsNullOrWhiteSpace(channelName))
        {
            PhotonChatHandler.LeaveChannel(channelName);
        }
    }

    // For entering a specific channel
    public void EnterChannel(string channelName)
    {
        if (!string.IsNullOrWhiteSpace(channelName))
        {
            PhotonChatHandler.EnterChannel(channelName);
        }
    }

    // For getting channel names from dictionary
    public string GetChannelName(ChannelType channelType)
    {
        return _channelNames[channelType];
    }

    // This method is for displaying received messages in chat panel
    private void SendMessageToChat(string text, Message.MessageType messageType)
    {
        // Limit number of messages
        LimitNumberOfMessages();

        // Create new Message object and add to list of messages
        Message message = new Message();
        GameObject newText = Instantiate(TextObject, ChatPanel.transform);
        newText.GetComponent<ChannelFieldHandler>().ChatManager = this;
        message.MessageText = text;
        message.MsgType = messageType;
        message.TextObject = newText.GetComponent<TextMeshProUGUI>();
        message.TextObject.text = text;
        message.TextObject.color = MessageTypeColor(messageType);

        _messageList.Add(message);
    }

    // This method is for displaying received messages in chat panel
    private void SendMessageToChat(Message message)
    {
        // Limit number of messages
        LimitNumberOfMessages();

        GameObject newText = Instantiate(TextObject, ChatPanel.transform);
        newText.GetComponent<ChannelFieldHandler>().ChatManager = this;
        message.TextObject = newText.GetComponent<TextMeshProUGUI>();
        message.TextObject.text = message.MessageText;
        message.TextObject.color = MessageTypeColor(message.MsgType);

        _messageList.Add(message);
    }

    private void LimitNumberOfMessages()
    {
        // Keep only 99 most recent messages before adding a new message to list
        if (_messageList.Count >= K_MaxMessages)
        {
            Destroy(_messageList[0].TextObject.gameObject);
            _messageList.Remove(_messageList[0]);
        }
    }

    // This method is for determining which color the message type is
    private Color MessageTypeColor(Message.MessageType messageType)
    {
        // Default color
        Color color = InfoColor;

        switch (messageType)
        {
            case Message.MessageType.playerMessage:
                color = PlayerMessageColor;
                break;
            case Message.MessageType.privateMessage:
                color = PrivateMessageColor;
                break;
        }

        return color;
    }
}
