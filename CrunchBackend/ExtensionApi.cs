using JetFly.Logging;
namespace CrunchEditor.CrunchBackend.Extensions;

public static class ExtensionApi
{
    public static LoggerInstanceAsync GetLoggerInstance(IBasicExtension self)
    {
        if(CrunchLayer.MainClass is null)
            throw new InvalidOperationException("MainClass is not yet initialized, somehow");
        return CrunchLayer.MainClass.GetLoggerInstance($"{self.GetMetadata("name")}/{self.GetMetadata("id")}");
    }
}