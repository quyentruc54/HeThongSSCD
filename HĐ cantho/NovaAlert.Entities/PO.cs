namespace NovaAlert.Entities
{
    public partial class PO: ISwitchAddress
    {
        public byte Id { get; set; }
        public byte Address { get; set; }

        public string Name
        {
            get
            {
                return string.Format("PO {0}", this.Id + 1);
            }
        }
    }
}
