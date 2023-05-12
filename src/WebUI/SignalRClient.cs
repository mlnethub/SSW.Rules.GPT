﻿using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using OpenAI.GPT3.ObjectModels.RequestModels;
using WebUI.Models;
using WebUI.Services;

namespace WebUI;

public class SignalRClient
{
    private readonly DataState _dataState;
    private readonly NotifierService _notifierService;
    private readonly HubConnection _connection;

    public SignalRClient(
        DataState dataState,
        IWebAssemblyHostEnvironment hostEnvironment,
        NotifierService notifierService
    )
    {
        _dataState = dataState;
        _notifierService = notifierService;
        var hubeBaseUrl = hostEnvironment.IsDevelopment()
            ? "https://localhost:7104"
            : "https://statusapp1.azurewebsites.net";
        var hubUrl = $"{hubeBaseUrl}/ruleshub";
        _connection = new HubConnectionBuilder().WithUrl(hubUrl).WithAutomaticReconnect().Build();
        RegisterHandlers();
        _connection.Closed += async (exception) =>
        {
            if (exception == null)
            {
                Console.WriteLine("Connection closed without error.");
            }
            else
            {
                Console.WriteLine($"Connection closed due to an error: {exception}");
            }
        };
    }

    public async Task StartAsync()
    {
        await _connection.StartAsync();
    }

    public async Task StopAsync()
    {
        await _connection.StopAsync();
    }

    //TODO: Refactor to separate file
    public enum StatusHubConnectionState
    {
        Disconnected,
        Connected,
        Connecting,
        Reconnecting
    }

    public StatusHubConnectionState GetConnectionState()
    {
        StatusHubConnectionState state;
        state = _connection.State switch
        {
            HubConnectionState.Disconnected => StatusHubConnectionState.Disconnected,
            HubConnectionState.Connected => StatusHubConnectionState.Connected,
            HubConnectionState.Connecting => StatusHubConnectionState.Connecting,
            HubConnectionState.Reconnecting => StatusHubConnectionState.Reconnecting,
            _ => throw new ArgumentOutOfRangeException()
        };
        return state;
    }

    // Methods the client can call on the server


    public async Task BroadcastMessageAsync(string userName, string message)
    {
        await _connection.InvokeAsync("BroadcastMessage", userName, message);
    }

    public async IAsyncEnumerable<ChatMessage?> RequestNewCompletionMessage(
        List<ChatMessage> messageList,
        string? apiKey
    )
    {
        //await _connection.StartAsync();
        var completionResult = _connection.StreamAsync<ChatMessage?>(
            "RequestNewCompletionMessage",
            messageList,
            apiKey
        );
        await foreach (var message in completionResult)
        {
            yield return message;
        }
    }

    //Methods that the client listens for
    private void RegisterHandlers()
    {
        _connection.On<string, string>(
            "ReceiveBroadcast",
            (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                Console.WriteLine(encodedMsg);
            }
        );
    }
}