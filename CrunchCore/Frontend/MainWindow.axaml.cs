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
            ExtensionApi.MainWindow = this;
            //this.Title = "Didn't change :c";
            if(CrunchFrontend.PostInitEvent is not null)
            {
                Task initializeComponent = Task.Run(() => InitializeComponent());
                Task postInitTask = Task.Run(CrunchFrontend.PostInitEvent);
            }
        }
    }
}