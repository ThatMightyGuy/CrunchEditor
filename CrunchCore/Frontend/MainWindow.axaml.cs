using Avalonia.Controls;
using CrunchEditor.Extensions;
namespace CrunchEditor.Core
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ExtensionApi.MainWindow = this;
            if(CrunchFrontend.PostInitEvent is not null)
            {
                Task initializeComponent = Task.Run(() => InitializeComponent());
                Task postInitTask = Task.Run(CrunchFrontend.PostInitEvent);
            }
        }
    }
}