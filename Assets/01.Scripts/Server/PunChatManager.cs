using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;
using System;

public class PunChatManager : MonoBehaviour, IChatClientListener
{
    private ChatClient chatClient;

    public event Action<string, string> OnMessageReceived;

    void Start()
    {
        ConnectToPhotonChat();
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

    public void OnConnected()
    {
        Debug.Log("Connected to Photon Chat");
        chatClient.Subscribe(new string[] { "global" });
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected from Photon Chat");
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("Chat state changed: " + state);
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            Debug.Log(string.Format("{0}: {1}", senders[i], messages[i]));
            OnMessageReceived?.Invoke(senders[i], messages[i].ToString());
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log(string.Format("Private message from {0}: {1}", sender, message));
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("Subscribed to channels: " + string.Join(", ", channels));
    }

    public void OnUnsubscribed(string[] channels)
    {
        Debug.Log("Unsubscribed from channels: " + string.Join(", ", channels));
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log(string.Format("User {0} is {1}. Message: {2}", user, status, message));
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log(string.Format("Debug Return - {0}: {1}", level, message));
    }

    public void SendChatMessage(string message)
    {
        if (chatClient != null && chatClient.CanChat)
        {
            chatClient.PublishMessage("global", message);
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
}
