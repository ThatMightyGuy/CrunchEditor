using JetFly.Logging;
namespace CrunchEditor.Extensions;

public static class ExtensionApi
{
    private static dynamic? mainClass;
    public static void Init(dynamic mainClass)
    {
        ExtensionApi.mainClass = mainClass;
    }
    public static LoggerInstanceAsync GetLoggerInstance(IBasicExtension self)
    {
        if(mainClass is null)
            throw new InvalidOperationException("MainClass is not yet initialized, somehow");
        return mainClass.GetLoggerInstance($"{self.GetMetadata("name")}/{self.GetMetadata("id")}");
    }
}