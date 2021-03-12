using DevExpress.Xpf.Editors;

namespace NovaAlert.Common
{
    public class VNDXEditorLocalizer : EditorLocalizer
    {
        protected override void PopulateStringTable()
        {
            //this.AddString(EditorStringId.ExpressionEditor_GetHour_Description, "Giờ");
            //this.AddString(EditorStringId.ExpressionEditor_GetMinute_Description, "Phút");
            AddString(EditorStringId.DatePickerHours, "giờ");
            AddString(EditorStringId.DatePickerMinutes, "phút");
        }
    }
}
