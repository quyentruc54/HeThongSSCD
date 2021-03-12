namespace NovaAlert.Entities.ViewModel
{
    public class HostPhoneViewModel: PhoneViewModel
    {
        public HostPhone HostPhone { get { return this.Phone as HostPhone; } }
        public HostPhoneViewModel(HostPhone phone):base(phone)
        {
        }

        public virtual bool CanMakeCall(string areaCode, string excludeAreaCodes)
        {
            if (this.HostPhone.IsRestricted)
            {
                if (string.IsNullOrEmpty(this.AreaCode))
                {
                    return string.IsNullOrEmpty(areaCode);
                }

                return string.Equals(this.AreaCode, areaCode);
            }
            else
            {
                if (!string.IsNullOrEmpty(excludeAreaCodes))
                {
                    return string.IsNullOrEmpty(areaCode) || excludeAreaCodes.IndexOf(areaCode) == -1;
                }

                return true;
            }
        }
    }
}
