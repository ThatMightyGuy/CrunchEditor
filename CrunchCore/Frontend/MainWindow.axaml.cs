using Avalonia.Controls;
using CrunchEditor.Extensions;
namespace CrunchEditor.Core
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ExtensionApi.MainWindow = this;
            InitializeComponent();
            if(CrunchFrontend.PostInitEvent is not null)
            {
                this.Opened += (object? _, EventArgs _) => CrunchFrontend.PostInitEvent();
                InitializeComponent();
            }
        }
    }
}