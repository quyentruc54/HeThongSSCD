namespace NovaAlert.Config.ViewModels
{
    public class SubResultDataViewModel : ResultDataViewModel
    {
        public override bool IsSubResult { get { return true; } }
        public ResultDataViewModel Parent { get; private set; }

        public override string DisplayId
        {
            get
            {
                return string.Format("{0}.{1}", this.Parent.DisplayId, GetDisplayId());
            }
        }

        public SubResultDataViewModel(ResultDataViewModel parent)
        {
            this.Parent = parent;
        }
    }
}
