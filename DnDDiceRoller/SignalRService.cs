using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SignalRService
{
    private HubConnection _connection;

    public event EventHandler<RollEventArgs> RollReceived;

    public async Task InitializeAsync(string hubUrl)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl("https://dnddicerollerapi20240821154836.azurewebsites.net/rollHub")
            .WithAutomaticReconnect()
            .Build();

        _connection.On<string, int, List<int>, Dictionary<int, int>, int>("ReceiveRoll", (user, total, individualRolls, diceUsed, modifier) =>
        {
            RollReceived?.Invoke(this, new RollEventArgs(user, total, individualRolls, diceUsed, modifier));
        });

        await _connection.StartAsync();
    }

    public async Task SendRoll(string user, int total, List<int> individualRolls, Dictionary<int, int> diceUsed, int modifier)
    {
        if (_connection.State == HubConnectionState.Connected)
        {
            await _connection.InvokeAsync("SendRoll", user, total, individualRolls, diceUsed, modifier);
        }
    }

    public async Task DisconnectAsync()
    {
        if (_connection != null)
        {
            await _connection.StopAsync();
            await _connection.DisposeAsync();
        }
    }
}

public class RollEventArgs : EventArgs
{
    public string User { get; set; }
    public int Total { get; set; }
    public List<int> IndividualRolls { get; set; }
    public Dictionary<int, int> DiceUsed { get; set; }
    public int Modifier { get; set; }

    public RollEventArgs(string user, int total, List<int> individualRolls, Dictionary<int, int> diceUsed, int modifier)
    {
        User = user;
        Total = total;
        IndividualRolls = individualRolls;
        DiceUsed = diceUsed;
        Modifier = modifier;
    }
} 