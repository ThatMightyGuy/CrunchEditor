using System.Runtime.InteropServices;
using JetFly.Logging;
using CrunchEditor.Extensions;
namespace CrunchEditor.Backend;

public class CrunchLayer
{
    private readonly LoggerAsync logger;
    private readonly HashSet<string> loggerInstances;
    private readonly LoggerInstanceAsync log;
    public List<IBasicExtension>? Extensions;
    private static Config? cfg;
    public static CrunchLayer? MainClass;

    private CrunchLayer(LoggerAsync logger, string userFiles, string[] args)
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
        string userFiles = Path.GetFullPath(
            Environment.ExpandEnvironmentVariables(
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                "%UserProfile%/Documents/CrunchEditor/" :
                "%HOME%/.cruncheditor/"
            )
        );
        FilesystemItem userDir = new(File.ReadAllText("DefaultStructure.json"));
        userDir.Create(userFiles);
        MainClass = new(
            new(
                Console.Out, Console.Error,
                File.CreateText(userFiles + "latest.log")
            ),
            userFiles, args
        );

        // Get a canonical CrunchEditor userdata dir path
        if(!File.Exists(userFiles + "CrunchEditor.json"))
        {
            Directory.CreateDirectory(userFiles);
            File.WriteAllText(userFiles + "CrunchEditor.json", "{}");
        }
        cfg = new(userFiles + "CrunchEditor.json");
        await MainClass.log.Log("User data folder: " + userFiles);
        ExtensionLoader extLoader = new(MainClass.GetLoggerInstance("extldr"));
        ExtensionApi.Init(MainClass);
        Task<List<IBasicExtension>> extensionsLoading = extLoader.LoadExtensionsInDir(userFiles + "Extensions/");
        MainClass.Extensions = await extensionsLoading.WaitAsync(TimeSpan.FromMilliseconds(-1));
        Task preInit = extensionsLoading.ContinueWith(
            (_) => extLoader.RaiseInitStageEvent(InitStage.PREINIT)
        );
        await preInit;
        Action init = () => extLoader.RaiseInitStageEvent(InitStage.INIT);
        Action postInit = () => extLoader.RaiseInitStageEvent(InitStage.POSTINIT); 
        Core.CrunchFrontend.Start(init, postInit, args);
    }
    public LoggerInstanceAsync GetLoggerInstance(string source)
    {
        if(loggerInstances.Add(source))
            return new(logger, source);
        else
            throw new InstanceExistsException("A logger for this source already exists");
    }
    public static string GetConfigProperty(string key) => cfg is null ? "Empty config" : cfg.GetProperty<string>(key);
    public IBasicExtension? GetExtensionById(string id)
    {
        if(MainClass is null || MainClass.Extensions is null) return null;
        return MainClass.Extensions.Find(ext => ext.GetMetadata("id") == id);
    }
}
