// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using RaysApps.Models;
using System.Collections.ObjectModel;

namespace RaysApps.Services.Message;

public class MessagesService : IMessageService
{
    public const string Client = "Client";
    public const string Server = "Server";
    public event EventHandler DataChanged = default!;
    private readonly ILogger<IMessageService> _log;
    private readonly ObservableCollection<MessageModel> _items = new()
    {
        new MessageModel(Client, "Hi"),
        new MessageModel(Server,"How are you?")
    };

    public ObservableCollection<MessageModel> Items
    {
        get
        {

            return _items;
        }
    }
    public MessagesService(ILogger<MessagesService> logger)
    {
        _log = logger;
    }
    public void AddClientMessage(string message)
    {
        _items.Add(new MessageModel(Client, message));
        var count = GetMessageCount();
        DataChanged?.Invoke(this, EventArgs.Empty);
        _log.LogWarning($"MessageCount{count}");
    }

    public void AddServerMessage(string message)
    {
        _items.Add(new MessageModel(Server, message));
        var count = GetMessageCount();
        DataChanged?.Invoke(this, EventArgs.Empty);
        _log.LogWarning($"MessageCount{count}");

    }
    public int GetMessageCount()
    {
        int count = Items.Count();
        return count;
    }


}
