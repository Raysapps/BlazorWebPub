// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Azure.Messaging.WebPubSub.Clients;
using RaysApps.Models;

namespace RaysApps.Services.WebPub;

public interface IRaysAppsWebPubClient : IHostedService
{
    Task<bool> Send(SignalData message);
    string? ClientUrl { get; }
    WebPubSubClient Client { get; }
    string EventConnectionId { get; }
    event Func<String,Task> StateHasChangedCallback;
}
