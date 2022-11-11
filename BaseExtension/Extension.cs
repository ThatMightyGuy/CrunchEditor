﻿using CrunchEditor.Core;
using CrunchEditor.Extensions;
using CrunchCore.Frontend.UIElements;
using Avalonia.Controls;
using JetFly.Logging;
namespace JetFly.BaseExtension
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
        }
        public void Init()
        {
        }
        public void PostInit()
        {
            MainWindow? mainWindow = ExtensionApi.GetMainWindow();
            if(mainWindow is null) return;
            Panel? mainPanel = (Panel?)mainWindow.Content;
            if(mainPanel is not null)
            {
                var testButton = new EditorButton();
                testButton.Content = "Something";
                mainPanel.Children.Add(testButton);
            }
            else
            {
                Task.Run(() => logger.Log("No panel yet", ErrorLevel.ERR));
            }
        }
    }
}
// literally never look further pls

// TODO: Make this autogenerated!
public class CrunchExtensionDefinition : IInitiatableExtension, IBasicExtension
{
    private readonly JetFly.BaseExtension.Extension ext;
    private readonly LoggerInstanceAsync log;
    private static readonly Dictionary<string, string> properties = new()
    {
        {"name", "CrunchEditor"},
        {"version", "1.0"},
        {"id", "1"},
        {"developers", "JetFly"},
        {"description", "Base editor extension"}
    };
    public CrunchExtensionDefinition()
    {
        log = ExtensionApi.GetLoggerInstance(this);
        ext = new JetFly.BaseExtension.Extension(log);
    }
    public string GetMetadata(string name) => properties[name];
    public void PreInit(object sender, InitStageEventArgs e) => ext.PreInit();
    public void Init(object sender, InitStageEventArgs e) => ext.Init();
    public void PostInit(object sender, InitStageEventArgs e) => ext.PostInit();
}

