// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor;
using MudBlazor.Services;

namespace RaysApps.Services.Theme;

public class RaysThemeService : IRaysTheme
{
    // Depenedency
    private readonly IResizeService _resizeService;
    private readonly IBreakpointService _breakPointService;
    private readonly ILogger<RaysThemeService> _logger;
    private readonly DateTime _startTime = DateTime.Now;
    public int LapseTime
    {
        get
        {
            var time = DateTime.Now - _startTime;
            return Convert.ToInt32(time.TotalSeconds);
        }
    }
    public BrowserWindowSize BrowserWindowSize { get; private set; } = new BrowserWindowSize();
    public Breakpoint CurrentBreakPoint { get; private set; }
    public event Action SubscribeWindowResizedEvent;
    public event Action SubscribeBreakPointChangedEvent;

    private void notifyWindowResized() => SubscribeWindowResizedEvent?.Invoke();
    private void notifyBreakPointChanged() => SubscribeBreakPointChangedEvent?.Invoke();

    private Guid _resizeSubscriptionId;
    private Guid _breakpointSubscriptionId;
    private async Task initResizeService()
    {
        _resizeSubscriptionId = await _resizeService.Subscribe((size) =>
        {
            BrowserWindowSize = size;
            notifyWindowResized();
        }, new ResizeOptions
        {
            ReportRate = 50,
            NotifyOnBreakpointOnly = false,
        });
        BrowserWindowSize = await _resizeService.GetBrowserWindowSize();

    }
    private async Task initBreakPointService()
    {
        var subscriptionResult = await _breakPointService.Subscribe((btPoint) =>
        {
            CurrentBreakPoint = btPoint;
            notifyBreakPointChanged();
        }, new ResizeOptions
        {
            ReportRate = 250,
            NotifyOnBreakpointOnly = true,
        });
        CurrentBreakPoint = await _breakPointService.GetBreakpoint();
        _breakpointSubscriptionId = subscriptionResult.SubscriptionId;

    }
    public RaysThemeService(IResizeService resizer, IBreakpointService breakPointListner, ILogger<RaysThemeService> log)
    {
        _resizeService = resizer;
        _breakPointService = breakPointListner;
        _logger = log;
        SubscribeBreakPointChangedEvent += onBreakpointChanged;
        SubscribeWindowResizedEvent += onResized;
    }
    public async Task InitAsync()
    {

        await initResizeService();
        await initBreakPointService();
        _logger.LogInformation("Inside init......");
    }
    public bool IsDarkMode { get; set; }
    public string BrowserCssHeight(int adjusttrueVal) => BrowserWindowSize.Height - adjusttrueVal + "px";
    public MudTheme Theme { get; init; } = new()
    {
        Palette = new Palette()
        {
            Primary = Colors.Teal.Darken2,
            Secondary = Colors.Orange.Accent3,
            //   AppbarBackground = Colors.Teal.Darken4
            AppbarBackground = "#eae6df",
            AppbarText = "rgb(17,27,33)",
            Background = "rgb(240, 242, 245)"

        },
        PaletteDark = new Palette()
        {
            Primary = Colors.Indigo.Lighten3,
            PrimaryContrastText = Colors.Shades.Black,
            Secondary = Colors.Orange.Lighten3,
            SecondaryContrastText = Colors.Shades.Black,
            AppbarBackground = "#323739",
            AppbarText = Colors.Shades.White,
            TextPrimary = Colors.Shades.White,
            Black = "#27272f",
            Background = "#32333d",
            BackgroundGrey = "#27272f",
            Surface = "#373740",
            DrawerBackground = "#32333d",
            DrawerText = "rgba(255,255,255, 0.50)",
            DrawerIcon = "rgba(255,255,255, 0.50)",
            ActionDefault = Colors.Shades.White,
            ActionDisabled = "rgba(255,255,255, 0.26)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)"
        }
,
        LayoutProperties = new LayoutProperties()
        {
            // DrawerWidthLeft = "400px"
            DrawerWidthLeft = "400px"
        }

    };
    public bool IsPlugAndPlayMode { get; set; } = true;

    // Event CallBacks;

    private void onResized() { }

    private void onBreakpointChanged()
    {
        var width = "400px";
        if (CurrentBreakPoint == Breakpoint.Sm || CurrentBreakPoint == Breakpoint.Xs)
            width = "80%";
        Theme.LayoutProperties.DrawerWidthLeft = width;
    }

    public async ValueTask DisposeAsync()
    {
        await _breakPointService.Unsubscribe(_breakpointSubscriptionId);
        await _resizeService.Unsubscribe(_resizeSubscriptionId);
    }

}
