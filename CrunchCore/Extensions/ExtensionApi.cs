using JetFly.Logging;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using CrunchEditor.Backend;
using CrunchEditor.Core;
namespace CrunchEditor.Extensions;

public static class ExtensionApi
{
    private static CrunchLayer? mainClass;
    internal static MainWindow? MainWindow;
    internal static void Init(CrunchLayer mainClass)
    {
        ExtensionApi.mainClass = mainClass;
    }
    public static LoggerInstanceAsync GetLoggerInstance(IBasicExtension self)
    {
        if(mainClass is null)
            throw new InvalidOperationException("MainClass is not yet initialized, somehow");
        return mainClass.GetLoggerInstance(self.GetMetadata("name"));
    }
    public static string GetConfigProperty(string key) => CrunchLayer.GetConfigProperty(key);
    public static string GetExtensionData(string id, string key)
    {
        if(mainClass is null)
            throw new InvalidOperationException("MainClass is not yet initialized, somehow");
        IBasicExtension? ext = mainClass.GetExtensionById(id);
        if(ext is not null)
            return ext.GetMetadata(key);
        else
            throw new KeyNotFoundException("No such property");
    }
    public static MainWindow? GetMainWindow() => MainWindow;
    public static Task Call(Action action)
    {
        return Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Background);
    }
    // public static Button? TryCreateDockButton(string icon, Action action)
    // {
        
    // }
    // public static Button? TryCreateOrGetDockButton(string icon, Action action)
    // {
        
    // }
    // public static Button? TryGetDockButton()
    // {
        
    // }
}