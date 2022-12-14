using CrunchEditor.Extensions;
using JetFly.Logging;
namespace JetFly.TestExtension
{
    public class Extension
    {
        private readonly LoggerInstanceAsync logger;
        public Extension(LoggerInstanceAsync logger)
        {
            this.logger = logger;
        }
        public void PreInit()
        {
            Task log = logger.Log("PRE INIT");
        }
        public void Init()
        {
            Task log = logger.Log("INIT");
        }
        public void PostInit()
        {
            Task log = logger.Log("POST INIT");
        }
    }
}
public class CrunchExtensionDefinition : IInitiatableExtension, IBasicExtension
{
    private readonly JetFly.TestExtension.Extension ext;
    private readonly LoggerInstanceAsync log;
    private static readonly Dictionary<string, string> properties = new()
    {
        {"name", "TestExtension"},
        {"version", "1.0"},
        {"id", "1"},
        {"developers", "JetFly"}
    };
    public CrunchExtensionDefinition()
    {
        log = ExtensionApi.GetLoggerInstance(this);
        ext = new JetFly.TestExtension.Extension(log);
    }
    public string GetMetadata(string name)
    {
        return properties[name];
    }
    public void PreInit(object sender, InitStageEventArgs e)
    {
        ext.PreInit();
    }
    public void Init(object sender, InitStageEventArgs e)
    {
        ext.Init();
    }
    public void PostInit(object sender, InitStageEventArgs e)
    {
        ext.PostInit();
    }
}

