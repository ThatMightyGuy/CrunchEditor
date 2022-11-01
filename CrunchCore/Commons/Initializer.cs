using Newtonsoft.Json;
namespace CrunchEditor;

public struct FilesystemItem
{
    public string Name;
    public bool Type;
    public string? FileContents;
    public List<FilesystemItem>? Files;
    public FilesystemItem(string json)
    {
        this = JsonConvert.DeserializeObject<FilesystemItem>(json,
            new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }
        );
    }
    public FilesystemItem(string name, bool type, string? fileContents, List<FilesystemItem>? files)
    {
        Name = name;
        Type = type;
        FileContents = fileContents;
        Files = files;
    }
    public void Create(string parentPath)
    {
        string path = $"{parentPath}{Name}/";
        if(Type)
        {
            if(!File.Exists(Name))
                File.WriteAllText(Name, FileContents);
        }
        else
        {
            if(!Directory.Exists(path))
            {
                DirectoryInfo dir = Directory.CreateDirectory(path);
                if(Files is null) return;
                foreach(FilesystemItem item in Files)
                    item.Create(path);
            }
        }
    }
}