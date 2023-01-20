using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;

namespace ChatApp;

public partial class GroupChatPage : ContentPage
{
    private readonly HubConnection _connection;
    public ObservableCollection<string> chatLog { get; } = new();
    public GroupChatPage()
    {
        InitializeComponent();
        BindingContext = this;

        _connection = new HubConnectionBuilder()
            .WithUrl("http://172.16.3.137:5066/chat")
            .Build();

        _connection.On<string>("RoomMessageReceived", (message) =>
        {
            chatLog.Add($"{Environment.NewLine}{message}");
        });

        Task.Run(() =>
        {
            Dispatcher.Dispatch(async () =>
            await _connection.StartAsync());
        });
    }

    private async void subButton_Clicked(object sender, EventArgs e)
    {
        if (myGroupNameField.Text != "")
        {
            if (subButton.Text == "Subscribe")
            {
                await _connection.InvokeCoreAsync("JoinRoom", args: new[]
                {myGroupNameField.Text});
                subButton.Text = "Unsubscribe";
                myGroupNameField.IsReadOnly = true;
                myGroupChatMessageField.IsEnabled = true;
            }
            else
            {
                await _connection.InvokeCoreAsync("LeaveRoom", args: new[]
                {myGroupNameField.Text});
                subButton.Text = "Subscribe";
                myGroupNameField.IsReadOnly = false;
                myGroupChatMessageField.IsEnabled = false;

            }
        }
    }

    private async void sendButton_Clicked(object sender, EventArgs e)
    {
        if (myGroupChatMessageField.Text != "")
        {
            await _connection.InvokeCoreAsync("SendRoomMessage", args: new[]
            {myGroupChatMessageField.Text, myGroupNameField.Text});
            myGroupChatMessageField.Text = String.Empty;
        }
    }
}