// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.WebSockets;
using System.Text.Json;
using Azure.Messaging.WebPubSub.Clients;
using RaysApps.Models;
using RaysApps.Services.Message;

namespace RaysApps.Services.WebPub;

public class RaysAppsWebPubClient : IRaysAppsWebPubClient
{
    private readonly HttpClient _raysApi;
    private readonly ILogger<RaysAppsWebPubClient> _logger;
    private Uri? _clientUrl;
    public event Func<string,Task> StateHasChangedCallback = default!;
    private string _eventConnectionId = string.Empty;
    public string EventConnectionId => _eventConnectionId;
   
    public string? ClientUrl => _clientUrl?.ToString();
    private WebPubSubClient client { get; set; } = default!;
    public  WebPubSubClient Client => client;
    private readonly IMessageService _messageService;


    public RaysAppsWebPubClient(HttpClient raysApi, ILogger<RaysAppsWebPubClient> logger, IMessageService messageService)
    {
        _raysApi = raysApi;
        _logger = logger;
        _messageService = messageService;

    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Negotiate method to fectch client access url
            var result = await _raysApi.GetStringAsync("negotiate/User1");
            _clientUrl = new Uri(result);
            client = new WebPubSubClient(_clientUrl, new WebPubSubClientOptions() { AutoReconnect = true });
            client.ServerMessageReceived += serverMessageReceived;
            client.GroupMessageReceived += groupMessageReceived;
            client.Connected += connected;
            client.Disconnected += disconnected;
            client.Stopped += stopped;
            await client.StartAsync();
        }
        catch (WebSocketException ex)
        {
            _logger.LogInformation(ex.Message, ex);
        }
    }
    private Task serverMessageReceived(WebPubSubServerMessageEventArgs eventArgs)
    {
        var singalObj = JsonSerializer.Deserialize<SignalData>(eventArgs.Message.Data);
        _messageService.AddServerMessage(singalObj?.Value ?? string.Empty);
        _logger.LogDebug($"Receive message: {eventArgs.Message.Data}");
        return Task.CompletedTask;
    }
    private Task groupMessageReceived(WebPubSubGroupMessageEventArgs eventArgs)
    {
        _logger.LogDebug($"Receive group message from {eventArgs.Message.Group}: {eventArgs.Message.Data}");
        return Task.CompletedTask;
    }
    private Task disconnected(WebPubSubDisconnectedEventArgs _)
    {
        _eventConnectionId = string.Empty;
        _logger.LogInformation($"Connection is disconnected");
        StateHasChangedCallback?.Invoke("Disconnected");

        return Task.CompletedTask;
    }
    private Task connected(WebPubSubConnectedEventArgs eventargs)
    {
       
        _eventConnectionId = eventargs.ConnectionId;
        if (client.ConnectionId == eventargs.ConnectionId)
        {
            _logger.LogInformation($"Connection Id's are same");
        }
        _logger.LogInformation($"{eventargs.ConnectionId}Connection is connected");
        StateHasChangedCallback?.Invoke("Connected");
        return Task.CompletedTask;
    }
    private Task stopped(WebPubSubStoppedEventArgs _)
    {
        _logger.LogInformation($"Connection is stopped");
        return Task.CompletedTask;
    }


    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await client.StartAsync(cancellationToken);
    }

    public async Task<bool> Send(SignalData message)
    {
        await client.SendEventAsync("push", BinaryData.FromString(JsonSerializer.Serialize(message)), WebPubSubDataType.Json);
        return true;
    }

    public async Task OnLogInSucceededAsync()
    {
        try
        {
            await client.StartAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }

    }
}

    