using CrunchEditor.CrunchCommon.Extensions;
namespace JetFly.TestExtension
{
    public class Extension
    {
        public Extension() {}
        public void PreInit() => Console.WriteLine("pre init");
        public void Init() => Console.WriteLine("init");
        public void PostInit() => Console.WriteLine("post init");
    }
}
public class CrunchExtensionDefinition : IInitiatableExtension, IBasicExtension
{
    private readonly JetFly.TestExtension.Extension ext;
    private static readonly Dictionary<string, string> properties = new()
    {
        {"name", "TestExtension"},
        {"version", "1.0"},
        {"id", "1"},
        {"developers", "JetFly"}
    };
    public CrunchExtensionDefinition()
    {
        ext = new JetFly.TestExtension.Extension();
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

