﻿using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests
{
    public class ChatManagerTests
    {
        private const string K_Message = "Hi there";
        private const string K_TestChannel = "Test";
        private const string K_ProjectName = "MMO Expo";
        private const string K_BoothsHallName = "Booths Hall North";

        [UnityTest]
        public IEnumerator WhenOnStartThenGetNoMessages()
        {
            GameObject eventManager = new GameObject(GameConstants.K_ExpoEventManager);
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());

            SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual(GameConstants.K_AnnouncementChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual(GameConstants.K_HallChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
        }

        [UnityTest]
        public IEnumerator WhenMessagesIsAtNumberLimitThenGetLimitNumberOfMessages()
        {
            GameObject eventManager = new GameObject(GameConstants.K_ExpoEventManager);
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            List<Message> messages = new List<Message>();
            for (int i = 0; i < 200; i++)
            {
                Message newMessage = new Message();
                newMessage.MessageText = K_Message;
                newMessage.MsgType = Message.MessageType.playerMessage;
                messages.Add(newMessage);
            }
            photonChatHandler.GetNewMessages().Returns(messages);
            photonChatHandler.IsConnected().Returns(true);

            SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            yield return null;

            Assert.AreEqual(100, chatManager.GetMessages().Count);
            Assert.AreEqual(GameConstants.K_AnnouncementChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual(GameConstants.K_HallChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
        }

        [UnityTest]
        public IEnumerator WhenAChannelMessageIsSentThenGetMessage()
        {
            GameObject eventManager = new GameObject(GameConstants.K_ExpoEventManager);
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetReturnKey().Returns(true);

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            List<Message> messages = new List<Message>();
            Message newMessage = new Message();
            newMessage.MessageText = K_Message;
            newMessage.MsgType = Message.MessageType.playerMessage;
            messages.Add(newMessage);
            photonChatHandler.GetNewMessages().Returns(messages);
            photonChatHandler.IsConnected().Returns(true);

            SetUpChatManager(chatManager, playerInputHandler, photonChatHandler,
                GameConstants.K_HallChannelName, K_Message);

            yield return null;

            Assert.AreEqual(1, chatManager.GetMessages().Count);
            Assert.AreEqual(GameConstants.K_AnnouncementChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual(GameConstants.K_HallChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.Received(1).SendChannelMessage(GameConstants.K_HallChannelName, K_Message);
        }

        [UnityTest]
        public IEnumerator WhenChannelNameIsEmptyThenGetWarning()
        {
            GameObject eventManager = new GameObject(GameConstants.K_ExpoEventManager);
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetReturnKey().Returns(true);

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(true);

            SetUpChatManager(chatManager, playerInputHandler, photonChatHandler, null, K_Message);

            yield return null;

            Assert.AreEqual(1, chatManager.GetMessages().Count);
            Assert.AreEqual("No channel or username specified.", chatManager.GetMessages()[0].MessageText);
            Assert.AreEqual(Message.MessageType.info, chatManager.GetMessages()[0].MsgType);
            Assert.AreEqual(GameConstants.K_AnnouncementChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual(GameConstants.K_HallChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
        }

        [UnityTest]
        public IEnumerator WhenChannelNameIsWrongThenGetWarning()
        {
            GameObject eventManager = new GameObject(GameConstants.K_ExpoEventManager);
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPlayerInputHandler playerInputHandler = Substitute.For<IPlayerInputHandler>();
            playerInputHandler.GetReturnKey().Returns(true);

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(true);

            SetUpChatManager(chatManager, playerInputHandler, photonChatHandler, K_TestChannel, K_Message);

            yield return null;

            Assert.AreEqual(1, chatManager.GetMessages().Count);
            Assert.AreEqual("\"Test\" does not exist in the room.",
                chatManager.GetMessages()[0].MessageText);
            Assert.AreEqual(Message.MessageType.info, chatManager.GetMessages()[0].MsgType);
            Assert.AreEqual(GameConstants.K_AnnouncementChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual(GameConstants.K_HallChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
        }

        [UnityTest]
        public IEnumerator WhenUpdateAnnouncementsChannelThenChannelIsNotUpdated()
        {
            GameObject eventManager = new GameObject(GameConstants.K_ExpoEventManager);
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            chatManager.UpdateChannel("General", ChatManager.ChannelType.announcementChannel);

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual(GameConstants.K_AnnouncementChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual(GameConstants.K_HallChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.Received(1).InitializeChannelNames(Arg.Any<string[]>());
        }

        [UnityTest]
        public IEnumerator WhenUpdateBoothChannelAndPhotonChatIsConnectedThenInitializeChannelsIsNotCalled()
        {
            GameObject eventManager = new GameObject(GameConstants.K_ExpoEventManager);
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(true);

            SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            chatManager.UpdateChannel(K_ProjectName, ChatManager.ChannelType.boothChannel);

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual(GameConstants.K_AnnouncementChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual(GameConstants.K_HallChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(K_ProjectName, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.Received(1).InitializeChannelNames(Arg.Any<string[]>());
        }

        [UnityTest]
        public IEnumerator WhenUpdateBoothChannelThenBoothChannelIsUpdated()
        {
            GameObject eventManager = new GameObject(GameConstants.K_ExpoEventManager);
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            chatManager.UpdateChannel(K_ProjectName, ChatManager.ChannelType.boothChannel);

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual(GameConstants.K_AnnouncementChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual(GameConstants.K_HallChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(K_ProjectName, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.Received(2).InitializeChannelNames(Arg.Any<string[]>());
        }

        [UnityTest]
        public IEnumerator WhenUpdateHallChannelThenHallChannelIsUpdated()
        {
            GameObject eventManager = new GameObject(GameConstants.K_ExpoEventManager);
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            chatManager.UpdateChannel(K_BoothsHallName, ChatManager.ChannelType.hallChannel);

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual(GameConstants.K_AnnouncementChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual(K_BoothsHallName, chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.Received(2).InitializeChannelNames(Arg.Any<string[]>());
        }

        [UnityTest]
        public IEnumerator WhenLeaveChannelIsCalledThenHandlerLeaveChannelIsCalled()
        {
            GameObject eventManager = new GameObject(GameConstants.K_ExpoEventManager);
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            chatManager.UpdateChannel(K_TestChannel, ChatManager.ChannelType.boothChannel);
            chatManager.LeaveChannel(K_TestChannel);

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual(GameConstants.K_AnnouncementChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual(GameConstants.K_HallChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(K_TestChannel, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.Received(1).LeaveChannel(K_TestChannel);
        }

        [UnityTest]
        public IEnumerator WhenLeaveChannelIsCalledOnEmptyStringThenHandlerLeaveChannelIsNotCalled()
        {
            GameObject eventManager = new GameObject(GameConstants.K_ExpoEventManager);
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            chatManager.LeaveChannel(string.Empty);

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual(GameConstants.K_AnnouncementChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual(GameConstants.K_HallChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.DidNotReceive().LeaveChannel(Arg.Any<string>());
        }

        [UnityTest]
        public IEnumerator WhenEnterChannelIsCalledThenHandlerEnterChannelIsCalled()
        {
            GameObject eventManager = new GameObject(GameConstants.K_ExpoEventManager);
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            chatManager.EnterChannel(K_TestChannel);

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual(GameConstants.K_AnnouncementChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual(GameConstants.K_HallChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.Received(1).EnterChannel(K_TestChannel);
        }

        [UnityTest]
        public IEnumerator WhenEnterChannelIsCalledOnEmptyStringThenHandlerEnterChannelIsNotCalled()
        {
            GameObject eventManager = new GameObject(GameConstants.K_ExpoEventManager);
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.GetNewMessages().Returns(new List<Message>());
            photonChatHandler.IsConnected().Returns(false);

            SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            chatManager.EnterChannel(string.Empty);

            yield return null;

            Assert.AreEqual(0, chatManager.GetMessages().Count);
            Assert.AreEqual(GameConstants.K_AnnouncementChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.announcementChannel));
            Assert.AreEqual(GameConstants.K_HallChannelName,
                chatManager.GetChannelName(ChatManager.ChannelType.hallChannel));
            Assert.AreEqual(string.Empty, chatManager.GetChannelName(ChatManager.ChannelType.boothChannel));
            photonChatHandler.DidNotReceive().EnterChannel(Arg.Any<string>());
        }

        [UnityTest]
        public IEnumerator WhenNameIsUpdatedThenConnectToServiceIsCalled()
        {
            GameObject eventManager = new GameObject(GameConstants.K_ExpoEventManager);
            ChatManager chatManager = eventManager.AddComponent<ChatManager>();
            ExpoEventManager.IsNameUpdated = false;

            IPhotonChatHandler photonChatHandler = Substitute.For<IPhotonChatHandler>();
            photonChatHandler.IsConnected().Returns(false);

            SetUpChatManager(chatManager, null, photonChatHandler, null, null);

            yield return null;

            photonChatHandler.DidNotReceive().ConnectToService();

            ExpoEventManager.IsNameUpdated = true;

            yield return null;

            photonChatHandler.Received(1).ConnectToService();
        }

        private void SetUpChatManager(ChatManager chatManager, IPlayerInputHandler playerInputHandler,
            IPhotonChatHandler photonChatHandler, string channelName, string message)
        {
            chatManager.PlayerInputHandler = playerInputHandler;
            chatManager.PhotonChatHandler = photonChatHandler;
            chatManager.PlayerMessageColor = new Color(255, 255, 255);
            chatManager.InfoColor = new Color(255, 255, 0);

            GameObject chatPanel = new GameObject("ChatPanel");
            chatManager.ChatPanel = chatPanel;

            GameObject textObject = new GameObject("Text");
            textObject.AddComponent<TextMeshProUGUI>();
            textObject.AddComponent<ChannelFieldHandler>();
            chatManager.TextObject = textObject;

            GameObject channelBoxObject = new GameObject("ChannelBoxObject");
            InputField channelBox = channelBoxObject.AddComponent<InputField>();
            chatManager.ChannelBox = channelBox;
            if (channelName != null)
            {
                channelBox.text = channelName;
            }

            GameObject chatBoxObject = new GameObject("ChatBoxObject");
            InputField chatBox = chatBoxObject.AddComponent<InputField>();
            chatManager.ChatBox = chatBox;
            if (message != null)
            {
                chatBox.text = message;
            }

            chatManager.EventInfoManager = Substitute.For<IEventInfoManager>();
        }
    }
}
