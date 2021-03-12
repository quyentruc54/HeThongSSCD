using System.Windows;

namespace NovaAlert.Common
{
    public interface IMessageBoxService
    {
        MessageBoxResult Show(string text, string caption, MessageBoxButton buttons, MessageBoxImage image);

        MessageBoxResult AskYesNo(string text, string caption = "Chú ý");

        void ShowInfo(string text, string caption = "Lưu ý");
        void ShowError(string text);

    }
}
