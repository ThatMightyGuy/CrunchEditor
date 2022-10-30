using Avalonia.Controls;
using System.Threading.Tasks;
namespace CrunchEditor.CrunchCore;
public class ExtensionApi
{
    public static Window? MainWindow;

    public ExtensionApi(Window mainWindow)
    {
        MainWindow = mainWindow;
    }

}