using NovaAlert.Common.Mvvm;
using NovaAlert.Entities;

namespace NovaAlert.Config.ViewModels
{
    public class ResultDataViewModel : ViewModelBase
    {
        public ResultData CTT { get; set; }
        public ResultData CCPK { get; set; }

        public virtual bool IsSubResult { get { return false; } }

        public string UnitName
        {
            get
            {
                if (this.CTT != null && !string.IsNullOrEmpty(this.CTT.UnitName))
                {
                    return this.CTT.UnitName;
                }
                else
                {
                    if (this.CCPK != null && !string.IsNullOrEmpty(this.CCPK.UnitName))
                    {
                        return this.CCPK.UnitName;
                    }
                }
                return null;
            }
        }

        public virtual string DisplayId
        {
            get
            {
                return GetDisplayId();
            }
        }

        protected string GetDisplayId()
        {
            if (this.CTT != null)
            {
                return this.CTT.DisplayId.ToString();
            }
            else
            {
                if (this.CCPK != null)
                {
                    return this.CCPK.DisplayId.ToString();
                }
            }
            return null;
        }

        public void UpdateData(ResultDataViewModel newData)
        {
            this.CTT = newData.CTT;
            this.CCPK = newData.CCPK;

            OnPropertyChanged("UnitName");
            OnPropertyChanged("CTT");
            OnPropertyChanged("CCPK");
        }
    }
}
