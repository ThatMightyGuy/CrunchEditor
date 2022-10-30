using Avalonia.Controls;
using System.Threading.Tasks;
namespace CrunchEditor.Core;
public class ExtensionApi
{
    public static Window? MainWindow;

    public ExtensionApi(Window mainWindow)
    {
        MainWindow = mainWindow;
    }

}