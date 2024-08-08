using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;
using System;

public class PunChatManager : MonoBehaviourPunCallbacks, IChatClientListener
{
    public ChatClient chatClient;
    public string currentChannel; // 현재 방의 채널 이름

    public event Action<string, string> OnMessageReceived;

    void Start()
    {
        ConnectToPhotonChat();
        DontDestroyOnLoad(gameObject);
    }

    void ConnectToPhotonChat()
    {
        chatClient = new ChatClient(this);
        chatClient.ChatRegion = "US";
        AuthenticationValues authValues = new AuthenticationValues();
        authValues.UserId = PhotonNetwork.NickName;
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", authValues);
    }

    void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }
    }

    //public void OnConnected()
    //{
    //    Utils.Log("Connected to Photon Chat");
    //    JoinRoomChannel();
    //}

    public void OnDisconnected()
    {
        Utils.Log("Disconnected from Photon Chat");
    }

    public void OnChatStateChange(ChatState state)
    {
        Utils.Log("Chat state changed: " + state);
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            Utils.Log(string.Format("{0}: {1}", senders[i], messages[i]));
            OnMessageReceived?.Invoke(senders[i], messages[i].ToString());
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Utils.Log(string.Format("Private message from {0}: {1}", sender, message));
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Utils.Log("Subscribed to channels: " + string.Join(", ", channels));
    }

    public void OnUnsubscribed(string[] channels)
    {
        Utils.Log("Unsubscribed from channels: " + string.Join(", ", channels));
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Utils.Log(string.Format("User {0} is {1}. Message: {2}", user, status, message));
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        Utils.Log(string.Format("Debug Return - {0}: {1}", level, message));
    }

    public void SendChatMessage(string message)
    {
        if (chatClient != null && chatClient.CanChat)
        {
            currentChannel = PhotonNetwork.CurrentRoom.Name;
            chatClient.PublishMessage(currentChannel, message);
        }
        else
        {
            Utils.LogRed($"chatClient = {chatClient}");
            Utils.LogRed($"chatClient.CanChat = {chatClient.CanChat}");
        }
    }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new NotImplementedException();
    }

    public override void OnJoinedRoom()
    {
        currentChannel = PhotonNetwork.CurrentRoom.Name;
        chatClient.Subscribe(new string[] { currentChannel });
    }

    public override void OnLeftRoom()
    {
        if (chatClient != null && !string.IsNullOrEmpty(currentChannel))
        {
            chatClient.Unsubscribe(new string[] { currentChannel });
        }
    }
}
