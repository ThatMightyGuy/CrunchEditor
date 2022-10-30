using System.Reflection;
using System.Runtime.InteropServices;
using JetFly.Logging;
using CrunchEditor.CrunchCommon;

namespace CrunchEditor.CrunchCommon.Extensions;

public interface IBasicExtension
{
    public string GetMetadata(string property);
}
public interface IInitiatableExtension : IBasicExtension
{
    public void PreInit(object sender, InitStageEventArgs e);
    public void Init(object sender, InitStageEventArgs e);
    public void PostInit(object sender, InitStageEventArgs e);
}
public class InitStageEventArgs : EventArgs
{
    public InitStage Stage;
    public InitStageEventArgs(InitStage stage)
    {
        Stage = stage;
    }
}
public class InvalidExtensionLibraryException : Exception
{
    public InvalidExtensionLibraryException(string message) : base(message) {}
}
public enum InitStage
{
    PREINIT,
    INIT,
    POSTINIT
}
public class ExtensionLoader
{
    public event EventHandler<InitStageEventArgs>? OnPreInit;
    public event EventHandler<InitStageEventArgs>? OnInit;
    public event EventHandler<InitStageEventArgs>? OnPostInit;
    private LoggerInstanceAsync logger;
    
    public virtual void RaiseInitStageEvent(InitStage stage)
    {
        EventHandler<InitStageEventArgs>? handler = stage switch
        {
            InitStage.PREINIT => OnPreInit,
            InitStage.INIT => OnInit,
            InitStage.POSTINIT => OnPostInit,
            _ => throw new ArgumentException("Stage is not a part of InitStage enum")
        };
        if(handler is not null)
        {
            InitStageEventArgs args = new(stage);
            handler(this, args);
        }
    }
    public ExtensionLoader(LoggerInstanceAsync logger)
    {
        this.logger = logger;
    }
    public async Task<List<IBasicExtension>> LoadExtensionsInDir(string path)
    {
        List<IBasicExtension> extensions = new();
        string? dirName = Path.GetDirectoryName(path);
        if(dirName is null)
        {
            string ex = "Extensions directory doesn't exist; your install is most likely corrupted!";
            await logger.Log(ex, ErrorLevel.FATAL);
            throw new BadInstallException(ex);
        }
        foreach(string dir in Directory.GetDirectories(dirName))
        {
            string file = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Extension.dll" : "Extension.so";
            file = Path.Combine(dir, file);
            if(File.Exists(file))
                extensions.Add(await Load(file));
            else
                await logger.Log($"{dir} does not provide an Extension.(dll/so). Skipping...");
        }
        return extensions;
    }
    public async Task<IBasicExtension> Load(string path)
    {
        (IInitiatableExtension?, IBasicExtension) ext = new();
        try
        {
            Assembly asm = Assembly.LoadFrom(path);
            Type? type = asm.GetType("CrunchExtensionDefinition");
            if(type is null)
                throw new InvalidExtensionLibraryException($"'{path}' does not have an CrunchExtensionDefinition");
            object? instance = Activator.CreateInstance(type);
            if(instance is not null)
            {
                ext.Item1 = (IInitiatableExtension?)instance;
                ext.Item2 = (IBasicExtension)instance;
            }
            if(ext.Item2 is null)
            {
                string ex = $"{path} does not implement IBasicExtension";
                await logger.Log(ex, ErrorLevel.FATAL);
                await logger.Log("It's most likely not a valid extension anyway", ErrorLevel.NOTE);
                throw new InvalidExtensionLibraryException(ex);
            }
        }
        catch(Exception ex)
        {
            await logger.Log($"Error loading extension: {ex.Message}", ErrorLevel.ERR);
            throw;
        }
        string GetProperty(string property)
        {
            return ext.Item1 is null ? ext.Item2.GetMetadata(property) : ext.Item1.GetMetadata(property);
        }
        string name = GetProperty("name");
        string id = GetProperty("id");
        string ver = GetProperty("version");
        EventHandler<InitStageEventArgs> GetEventHandler(string e)
        {
            if(ext.Item1 is not null)
            {
                MethodInfo? mi = ext.Item1.GetType().GetMethod(e);
                if(mi is null) throw new InvalidExtensionLibraryException("Extension is IBasicExtension");
                Action<object?, InitStageEventArgs> action = mi.CreateDelegate<Action<object?, InitStageEventArgs>>(ext.Item1);
                return new EventHandler<InitStageEventArgs>(action);
            }
            throw new InvalidCastException("Extension is IBasicExtension");
        }
        if(ext.Item1 is not null)
        {
            await logger.Log($"Subscribing {name}:{id} to extension init events", ErrorLevel.INFO);
            OnPreInit += GetEventHandler("PreInit");
            OnInit += GetEventHandler("Init");
            OnPostInit += GetEventHandler("PostInit");
        }
        await logger.Log($"{name} version {ver} was successfully loaded");
        return ext.Item1 is null ? ext.Item2 : ext.Item1;
    }
}
