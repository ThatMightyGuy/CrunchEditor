using System.Runtime.InteropServices;
using System.Threading.Tasks;
using JetFly.Logging;
using CrunchEditor.CrunchBackend.Extensions;

namespace CrunchEditor.CrunchBackend;

public class BadInstallException : Exception
{
    public BadInstallException(string message) : base(message) {}
}
public class CrunchLayer
{
    private readonly LoggerAsync logger;
    private readonly HashSet<string> loggerInstances;
    private readonly LoggerInstanceAsync log;
    public static CrunchLayer? MainClass;

    private CrunchLayer(LoggerAsync logger)
    {
        this.logger = logger;
        this.loggerInstances = new();
        this.log = new(this.logger, "main");
    }
    public async Task<FileStream> Open(string path)
    {
        try
        {
            return File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read | FileShare.Inheritable);
        }
        catch(Exception ex)
        {
            await log.Log($"Error opening '{path}': {ex.Message}", ErrorLevel.ERR);
            throw;
        }
    }
    public static async Task Main(string[] args)
    {
        // Get a canonical CrunchEditor userdata dir path
        string userFiles = Path.GetFullPath(
            Environment.ExpandEnvironmentVariables(
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                "%UserProfile%/Documents/CrunchEditor/" :
                "~/.cruncheditor/"
            )
        );
        if(!File.Exists(userFiles + "CrunchEditor.json"))
        {
            Directory.CreateDirectory(userFiles);
            File.WriteAllText(userFiles + "CrunchEditor.json", "{}");
        }
        Config cfg = new(userFiles + "CrunchEditor.json");
        MainClass = new(
            new(
                Console.Out, Console.Error,
                File.CreateText(userFiles + "latest.log")
            )
        );
        await MainClass.log.Log("User data folder: " + userFiles);
        ExtensionLoader extLoader = new(MainClass.GetLoggerInstance("extldr"));
        Task extensionsLoading = extLoader.LoadExtensionsInDir(userFiles + "Extensions/");
        Task preInit = extensionsLoading.ContinueWith(
            (_) => extLoader.RaiseInitStageEvent(InitStage.PREINIT)
        );
        await preInit;

        CrunchCore.CrunchFrontend.Start(args);
    }
    public LoggerInstanceAsync GetLoggerInstance(string source)
    {
        if(loggerInstances.Add(source))
            return new(logger, source);
        else
            throw new InstanceExistsException("A logger for this source already exists");
    }
}
