using System.Windows;
using System.Windows.Controls;

namespace NovaAlert.Common.Mvvm
{
    public static class DialogCloser
    {
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.RegisterAttached(
                "DialogResult",
                typeof(bool?),
                typeof(DialogCloser),
                new PropertyMetadata(DialogResultChanged));

        private static void DialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window window = null;
            if (d is UserControl)
            {
                var ctrl = d as UserControl;
                window = Window.GetWindow(ctrl.Parent);
            }
            else
            {
                window = d as Window;
            }

            if (window != null && window.IsVisible)
            {
                window.DialogResult = e.NewValue as bool?;
            }
        }
        public static void SetDialogResult(Control target, bool? value)
        {
            target.SetValue(DialogResultProperty, value);
        }
    }
}
