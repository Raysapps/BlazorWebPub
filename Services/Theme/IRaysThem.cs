// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor;
using MudBlazor.Services;

namespace RaysApps.Services.Theme;

public interface IRaysTheme
{
    public BrowserWindowSize BrowserWindowSize { get; }
    public Breakpoint CurrentBreakPoint { get; }
    public event Action SubscribeWindowResizedEvent;
    public event Action SubscribeBreakPointChangedEvent;

    public MudTheme Theme { get; }
    public bool IsDarkMode { get; set; }
    public bool IsPlugAndPlayMode { get; set; }
    public string BrowserCssHeight(int adjustVal);
    public int LapseTime { get; }
    public Task InitAsync();
}
