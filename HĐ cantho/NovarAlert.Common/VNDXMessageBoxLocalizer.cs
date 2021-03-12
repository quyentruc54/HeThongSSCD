using DevExpress.Xpf.Core;

namespace NovaAlert.Common
{
    public class VNDXMessageBoxLocalizer : DXMessageBoxLocalizer
    {
        protected override void PopulateStringTable()
        {
            this.AddString(DXMessageBoxStringId.Yes, "Có");
            this.AddString(DXMessageBoxStringId.No, "Không");
            this.AddString(DXMessageBoxStringId.Ok, "Đồng ý");
            this.AddString(DXMessageBoxStringId.Cancel, "Bỏ qua");
        }
    }
}
