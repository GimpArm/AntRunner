using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AntRunner.Models
{
    public static class MouseCommands
    {
        private static void LeftClickChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = (UIElement)sender;

            if (args.NewValue is ICommand command)
            {
                var newBinding = new MouseBinding(command, new MouseGesture(MouseAction.LeftClick)) {CommandParameter = control};
                control.InputBindings.Add(newBinding);
            }
            else
            {
                var oldBinding = control.InputBindings.Cast<InputBinding>().First(b => b.Command.Equals(args.OldValue));
                control.InputBindings.Remove(oldBinding);
            }
        }

        public static readonly DependencyProperty LeftClickProperty = DependencyProperty.RegisterAttached("LeftClick", typeof(ICommand), typeof(MouseCommands), new UIPropertyMetadata(LeftClickChanged));

        public static void SetLeftClick(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LeftClickProperty, value);
        }

        public static ICommand GetLeftClick(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(LeftClickProperty);
        }
    }
}
