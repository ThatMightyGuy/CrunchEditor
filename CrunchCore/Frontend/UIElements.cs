using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;
namespace CrunchCore.Frontend.UIElements;
public class SidebarButton : Button
{
    public SidebarButton() : base()
    {
        base.Classes.Add("sidebar-button");
    }
}
public class EditorButton : Button
{
    public EditorButton() : base()
    {
        base.Classes.Add("editor-button");
    }
}
public class RadioButton : Avalonia.Controls.RadioButton
{
    public RadioButton() : base()
    {
        base.Classes.Add("radio-button");
    }
}
public class SwitchButton : Avalonia.Controls.Primitives.ToggleButton
{
    public SwitchButton() : base()
    {
        base.Classes.Add("switch-button");
    }
}