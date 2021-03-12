using System.Windows;
using DevExpress.Xpf.Core;

namespace NovaAlert.Common
{
    public class DXMessageBoxService : IMessageBoxService
    {
        public MessageBoxResult Show(string text, string caption, MessageBoxButton buttons, MessageBoxImage image)
        {
            return DXMessageBox.Show(text, caption, buttons, image);
        }

        public MessageBoxResult AskYesNo(string text, string caption = "Chú ý")
        {
            return DXMessageBox.Show(text, caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        public void ShowInfo(string text, string caption = "Lưu ý")
        {
            DXMessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowError(string text)
        {
            DXMessageBox.Show(text, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
