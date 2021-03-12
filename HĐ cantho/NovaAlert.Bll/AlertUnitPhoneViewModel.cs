using NovaAlert.Entities;
using NovaAlert.Entities.ViewModel;

namespace NovaAlert.Bll
{
    public class AlertUnitPhoneViewModel: UnitPhoneViewModel
    {        
        public override string TextTask
        {
            get
            {
                if (this.Task != null)
                {
                    return string.Format("{0}-{1}",
                        this.Task.CurrentTask == eTask.None ? string.Empty : this.Task.CurrentTask.ToString(),
                        this.Task.Level == eTaskLevel.None ? string.Empty : this.Task.Level.ToString());
                }
                return null;
            }
        }

        public override string TextResult
        {
            get
            {
                if (this.Task != null)
                {
                    if (this.Task.Result == eTaskResult.NL)
                    {
                        return "Đã NL";
                    }
                    else if (this.Task.Result == eTaskResult.CTT)
                    {
                        return "Đã CTT";
                    }
                }
                return null;
            }
        }

        public AlertUnitPhoneViewModel(UnitPhone phone):base(phone)
        {
        }
    }

    public class CCPK_ALertUnitPhoneViewModel: UnitPhoneViewModel
    {
        public override string TextTask
        {
            get
            {
                if (this.Task != null)
                {
                    if (this.Task.Level == eTaskLevel.TC)
                    {
                        return "Cấp 1";
                    }
                    else
                    {
                        if (this.Task.Level == eTaskLevel.CA)
                        {
                            return "Cấp 2";
                        }
                        return string.Empty;
                    }
                }
                return null;
            }
        }

        public override string TextResult
        {
            get
            {
                if (this.Task != null)
                {
                    if (this.Task.Result == eTaskResult.NL)
                    {
                        return "Đã NL";
                    }
                    else if (this.Task.Result == eTaskResult.CTT)
                    {
                        return "Đã CC";
                    }
                }

                return null;
            }
        }

        public CCPK_ALertUnitPhoneViewModel(UnitPhone phone):base(phone)
        {

        }
    }

    public class TSL_ALertUnitPhoneViewModel: UnitPhoneViewModel
    {
        eTslStatus _prepareStatus;
        public eTslStatus PrepareStatus
        {
            get { return _prepareStatus; }
            set { _prepareStatus = value; OnPropertyChanged("PrepareStatus"); OnPropertyChanged("TextTask"); }
        }

        eTslStatus _resultStatus;
        public eTslStatus ResultStatus
        {
            get { return _resultStatus; }
            set { _resultStatus = value; OnPropertyChanged("ResultStatus"); OnPropertyChanged("TextResult"); }
        }

        public override string TextTask
        {
            get
            {
                if (_prepareStatus == eTslStatus.Ready)
                {
                    return "Đã CBNL";
                }
                return GetStatusText(_prepareStatus);
            }
        }

        public override string TextResult
        {
            get
            {
                if (_resultStatus == eTslStatus.Ready)
                {
                    return "Đã nhận KQ";
                }
                return GetStatusText(_resultStatus);
            }
        }

        string GetStatusText(eTslStatus status)
        {
            switch (status)
            {
                case eTslStatus.InProgress:
                    return "Đang kết nối";

                case eTslStatus.Ready:
                    return "Đã CBNL";

                case eTslStatus.CanNotConnected:
                    return "Không kết nối";

                case eTslStatus.NotResponsed:
                    return "Không phản hồi";

                case eTslStatus.Canceled:
                    return "Hủy thao tác";

                default:
                    return null;
            }            
        }

        public TSL_ALertUnitPhoneViewModel(UnitPhone phone)
            : base(phone)
        {
            _prepareStatus = eTslStatus.None;
            _resultStatus = eTslStatus.None;
        }
    }

    public static class UnitPhoneFactory
    {
        public static UnitPhoneViewModel Create(eWorkingMode mode, UnitPhone item)
        {
            UnitPhoneViewModel up = null;
            switch (mode)
            {
                case eWorkingMode.Alert:
                    up = new AlertUnitPhoneViewModel(item);
                    break;

                case eWorkingMode.MultiDest:
                    up = new UnitPhoneViewModel(item);
                    break;

                case eWorkingMode.CCPK_Alert:
                    up = new CCPK_ALertUnitPhoneViewModel(item);
                    break;

                case eWorkingMode.TSL_Alert:
                    up = new TSL_ALertUnitPhoneViewModel(item);
                    break;
            }
            return up;
        }
    }
}
