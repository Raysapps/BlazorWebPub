// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using RaysApps.Models;
using System.Collections.ObjectModel;

namespace RaysApps.Services.Message;

public interface IMessageService
{
    public ObservableCollection<MessageModel> Items { get; }
    public void AddServerMessage(string message);
    public void AddClientMessage(string message);
    public int GetMessageCount();
    event EventHandler DataChanged;
}
