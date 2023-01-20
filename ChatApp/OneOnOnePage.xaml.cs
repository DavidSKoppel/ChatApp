using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.ObjectModel;
using System.Data.Common;

namespace ChatApp;

public partial class OneOnOnePage : ContentPage
{
    private readonly HubConnection _connection;
    public ObservableCollection<string> chatLog { get; } = new();
    public OneOnOnePage()
    {
        InitializeComponent();
        BindingContext = this;

        _connection = new HubConnectionBuilder()
        .WithUrl("http://172.16.3.137:5066/chat")
            .Build();

        _connection.On<string>("broadcasttoclient", (message) =>
        {
            chatLog.Add($"{Environment.NewLine}{message}");
        });

        Task.Run(() =>
        {
            Dispatcher.Dispatch(async () =>
            await _connection.StartAsync());
        });
    }

    private async void sendButton_Clicked(object sender, EventArgs e)
    {
        if (userIdField.Text != "")
        {
            await _connection.InvokeCoreAsync("BroadcastToConnection", args: new[]
            {UserChatField.Text, userIdField.Text});
            
            UserChatField.Text = String.Empty;
        }
    }

    private void connectionId_Clicked(object sender, EventArgs e)
    {
        Task.Run(() =>
        {
            Dispatcher.Dispatch(async () =>
            await DisplayAlert("Session Id", _connection.ConnectionId, "OK"));
        });
    }
}