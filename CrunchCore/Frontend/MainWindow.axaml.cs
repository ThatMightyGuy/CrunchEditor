using Avalonia.Controls;
using System.Threading.Tasks;
using System.Collections.Generic;
using CrunchEditor.Extensions;
namespace CrunchEditor.Core
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