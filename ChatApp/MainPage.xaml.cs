using Microsoft.AspNetCore.SignalR.Client;

namespace ChatApp;

public partial class MainPage : ContentPage
{
    private readonly HubConnection _connection;
    public MainPage()
    {
        InitializeComponent();

        _connection = new HubConnectionBuilder()
            .WithUrl("http://172.16.3.137:5066/chat")
            .Build();

        _connection.On<string>("MessageReceived", (message) =>
        {
            chatMessages.Text += $"\r\n{message}";
        });

        Task.Run(() =>
        {
            Dispatcher.Dispatch(async () =>
            await _connection.StartAsync());
        });
    }

    private async void SendMessage(object sender, EventArgs e)
    {
        await _connection.InvokeCoreAsync("SendMessage", args: new[]
        {_connection.ConnectionId, myChatMessageField.Text});

        myChatMessageField.Text = String.Empty;
    }
}

