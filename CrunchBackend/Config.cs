using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CrunchEditor.CrunchBackend;
public sealed class Config
{
    public static Config? StaticConfig;
    private Dictionary<string, object> _config;
    private string _path;
    public Config(string path)
    {
        _path = path;
        if(!File.Exists(path))
            throw new FileNotFoundException($"'{path}' does not exist");
        // read config as a dictionary<string, object>
        var cfg = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(path));
        if(cfg is not null)
            _config = cfg;
        else
            throw new InvalidDataException($"'{path}' cannot be read");
    }
    public T GetProperty<T>(string key) => (T)_config[key];
    public void SetProperty(string key, object value) => _config[key] = value;
    public void Save()
    {
        File.WriteAllText(_path, JsonConvert.SerializeObject(_config));
    }
}
