using Avalonia.Controls;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace CrunchEditor.CrunchCore
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            if(CrunchFrontend.PostInitEvent is not null)
            {
                Task postInitTask = Task.Run(CrunchFrontend.PostInitEvent);
                ExtensionApi extensionApi = new(this);
                InitializeComponent();
            }
        }
    }
}