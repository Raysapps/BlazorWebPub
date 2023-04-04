// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace RaysApps.Services.WebPub;

// To my understanding IHostedService is not Supported in Blazor.
// for code Consistency and  dependency sharing with other projectd , created a similar interface 
// Reserach more to find the better method.
public interface IHostedService
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}
