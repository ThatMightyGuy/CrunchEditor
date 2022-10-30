using Avalonia;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CrunchEditor.CrunchCommon.Extensions;

namespace CrunchEditor.CrunchCore;

public class CrunchFrontend
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    public static Action? PostInitEvent;
    public static List<IBasicExtension>? Extensions;

    [STAThread]
    public static void Start(Action init, Action postInit, string[] args)
    {
        PostInitEvent = postInit;
        Task initTask = Task.Run(init);
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}

